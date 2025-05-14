using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using SoL.Game;
using SoL.Game.Login.Client;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.Utilities.Extensions;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace SoL.Managers
{
	// Token: 0x020004F9 RID: 1273
	public class LoginApiManager : MonoBehaviour
	{
		// Token: 0x06002418 RID: 9240 RVA: 0x00059D6E File Offset: 0x00057F6E
		private void Start()
		{
			SessionData.SessionDataCleared += this.SessionDataOnSessionDataCleared;
			this.m_refreshCo = this.RefreshCo();
			base.StartCoroutine(this.m_refreshCo);
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x00059D9A File Offset: 0x00057F9A
		private void OnDestroy()
		{
			if (this.m_refreshCo != null)
			{
				base.StopCoroutine(this.m_refreshCo);
				this.m_refreshCo = null;
			}
			SessionData.SessionDataCleared -= this.SessionDataOnSessionDataCleared;
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x00059DC8 File Offset: 0x00057FC8
		private void OnApplicationQuit()
		{
			this.DeleteSessionKeyLocking();
		}

		// Token: 0x0600241B RID: 9243 RVA: 0x0012C790 File Offset: 0x0012A990
		private void SendError(CommandType cmdType, string error)
		{
			SolServerCommand solServerCommand = new SolServerCommand(CommandClass.client, cmdType)
			{
				State = false
			};
			solServerCommand.Args.Add(SolServerCommand.kErrorKey, error);
			CommandRouter.Route(solServerCommand);
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x0012C7C8 File Offset: 0x0012A9C8
		private static string GetLoginWithCredentialsUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/login/",
				SolConfigTypeExtensions.GetApiBranch()
			});
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x0012C81C File Offset: 0x0012AA1C
		private static string GetLoginWithSessionKeyUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/login/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey
			});
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x0012C884 File Offset: 0x0012AA84
		private static string GetLoginWithSteamUri(uint appId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/login/",
				SolConfigTypeExtensions.GetApiBranch(),
				"/steam/",
				appId.ToString()
			});
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x0012C8E8 File Offset: 0x0012AAE8
		private static string GetPurchaseSteamSubscriptionUri(uint appId, ulong steamId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/steam/",
				appId.ToString(),
				"/",
				steamId.ToString(),
				"/purchase_subscription"
			});
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x0012C980 File Offset: 0x0012AB80
		private static string GetFinalizeSteamTransactionUri(uint appId, ulong steamId, ulong orderId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/steam/",
				appId.ToString(),
				"/",
				steamId.ToString(),
				"/finalize_transaction/",
				orderId.ToString()
			});
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x0012CA24 File Offset: 0x0012AC24
		private static string GetUpdateCharacterIndexesUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/reposition_characters"
			});
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x0012CA94 File Offset: 0x0012AC94
		private static string GetDeleteSessionKeyUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/delete_session"
			});
		}

		// Token: 0x06002423 RID: 9251 RVA: 0x0012CB04 File Offset: 0x0012AD04
		private static string GetRefreshSessionKeyUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/refresh_session"
			});
		}

		// Token: 0x06002424 RID: 9252 RVA: 0x0012CB74 File Offset: 0x0012AD74
		private static string GetCharacterDeleteUri(string characterId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/delete_character/",
				characterId
			});
		}

		// Token: 0x06002425 RID: 9253 RVA: 0x0012CBEC File Offset: 0x0012ADEC
		private static string GetDesignatePrimaryCharacterUri(string characterId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/designate_primary_character/",
				characterId
			});
		}

		// Token: 0x06002426 RID: 9254 RVA: 0x0012CC64 File Offset: 0x0012AE64
		private static string GetSetActiveCharactersUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/set_active_characters"
			});
		}

		// Token: 0x06002427 RID: 9255 RVA: 0x0012CCD4 File Offset: 0x0012AED4
		private static string GetCharacterCreateUri()
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/create_character"
			});
		}

		// Token: 0x06002428 RID: 9256 RVA: 0x0012CD44 File Offset: 0x0012AF44
		private static string GetUpdatePortraitUri(string characterId, string portraitId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/update_portrait/",
				characterId,
				"/",
				portraitId
			});
		}

		// Token: 0x06002429 RID: 9257 RVA: 0x0012CDC8 File Offset: 0x0012AFC8
		private static string GetRenameCharacterUri(string characterId, string requestedName)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/rename_character/",
				characterId,
				"/",
				requestedName
			});
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x0012CE4C File Offset: 0x0012B04C
		private static string GetZoneCheckUri(ZoneId targetZoneId)
		{
			string[] array = new string[10];
			array[0] = "http://";
			array[1] = SolServerConnectionManager.Config.LoginHost;
			array[2] = ":";
			array[3] = SolServerConnectionManager.Config.LoginPort;
			array[4] = "/";
			array[5] = SessionData.User.Id;
			array[6] = "/";
			array[7] = SessionData.SessionKey;
			array[8] = "/zone_check/";
			int num = 9;
			int num2 = (int)targetZoneId;
			array[num] = num2.ToString();
			return string.Concat(array);
		}

		// Token: 0x0600242B RID: 9259 RVA: 0x0012CECC File Offset: 0x0012B0CC
		private static string GetZoneCheckUriWithInstanceId(ZoneId targetZoneId, int instanceId)
		{
			string[] array = new string[12];
			array[0] = "http://";
			array[1] = SolServerConnectionManager.Config.LoginHost;
			array[2] = ":";
			array[3] = SolServerConnectionManager.Config.LoginPort;
			array[4] = "/";
			array[5] = SessionData.User.Id;
			array[6] = "/";
			array[7] = SessionData.SessionKey;
			array[8] = "/zone_check_instance/";
			int num = 9;
			int num2 = (int)targetZoneId;
			array[num] = num2.ToString();
			array[10] = "/";
			array[11] = instanceId.ToString();
			return string.Concat(array);
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x0012CF60 File Offset: 0x0012B160
		private static string GetZoneCheckUriForSelection(ZoneId targetZoneId, CharacterRecord character)
		{
			string[] array = new string[12];
			array[0] = "http://";
			array[1] = SolServerConnectionManager.Config.LoginHost;
			array[2] = ":";
			array[3] = SolServerConnectionManager.Config.LoginPort;
			array[4] = "/";
			array[5] = SessionData.User.Id;
			array[6] = "/";
			array[7] = SessionData.SessionKey;
			array[8] = "/zone_check_selection/";
			int num = 9;
			int num2 = (int)targetZoneId;
			array[num] = num2.ToString();
			array[10] = "/";
			array[11] = character.Id;
			return string.Concat(array);
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x0012CFF0 File Offset: 0x0012B1F0
		private static string GetUpdateCharacterVisualsUri(string characterId)
		{
			return string.Concat(new string[]
			{
				"http://",
				SolServerConnectionManager.Config.LoginHost,
				":",
				SolServerConnectionManager.Config.LoginPort,
				"/",
				SessionData.User.Id,
				"/",
				SessionData.SessionKey,
				"/update_character_visuals/",
				characterId
			});
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x00059DD0 File Offset: 0x00057FD0
		public static void LoginWithCredentials(string username, string password)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.LoginWithCredentialsInternal(username, password);
			}
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x0012D068 File Offset: 0x0012B268
		private void LoginWithCredentialsInternal(string username, string password)
		{
			if (Time.time - this.m_timeOfLastLoginAttempt < 1f)
			{
				this.SendError(CommandType.auth, "Trying too soon!");
				return;
			}
			this.m_timeOfLastLoginAttempt = Time.time;
			if (this.m_loginInternalCo != null)
			{
				base.StopCoroutine(this.m_loginInternalCo);
			}
			this.m_loginInternalCo = this.LoginWithCredentialsCo(username, password);
			base.StartCoroutine(this.m_loginInternalCo);
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x00059DEB File Offset: 0x00057FEB
		private IEnumerator LoginWithCredentialsCo(string username, string password)
		{
			while (SolServerConnectionManager.Config == null)
			{
				yield return null;
			}
			string loginWithCredentialsUri = LoginApiManager.GetLoginWithCredentialsUri();
			Debug.Log("Auth via " + loginWithCredentialsUri);
			WWWForm wwwform = new WWWForm();
			wwwform.AddField("ApiVersion", GlobalSettings.Values.Configs.Data.ServerApiVersion);
			wwwform.AddField("Username", username);
			wwwform.AddField("Password", SolServerEncryption.EncryptRsa(password));
			using (UnityWebRequest www = UnityWebRequest.Post(loginWithCredentialsUri, wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning(www.error);
					this.SendError(CommandType.auth, www.error);
				}
				else
				{
					CommandRouter.Route(JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text));
				}
			}
			UnityWebRequest www = null;
			yield break;
			yield break;
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x00059E08 File Offset: 0x00058008
		public static void LoginWithSessionKey()
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.LoginWithSessionKeyInternal();
			}
		}

		// Token: 0x06002432 RID: 9266 RVA: 0x0012D0D0 File Offset: 0x0012B2D0
		private void LoginWithSessionKeyInternal()
		{
			if (Time.time - this.m_timeOfLastLoginAttempt < 1f)
			{
				this.SendError(CommandType.reauth, "Trying too soon!");
				return;
			}
			this.m_timeOfLastLoginAttempt = Time.time;
			if (this.m_loginInternalCo != null)
			{
				base.StopCoroutine(this.m_loginInternalCo);
			}
			this.m_loginInternalCo = this.LoginWithSessionKeyCo();
			base.StartCoroutine(this.m_loginInternalCo);
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x00059E21 File Offset: 0x00058021
		private IEnumerator LoginWithSessionKeyCo()
		{
			while (SolServerConnectionManager.Config == null)
			{
				yield return null;
			}
			string loginWithSessionKeyUri = LoginApiManager.GetLoginWithSessionKeyUri();
			Debug.Log("Auth via " + loginWithSessionKeyUri);
			using (UnityWebRequest www = UnityWebRequest.Get(loginWithSessionKeyUri))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error relogging in! " + www.error);
					if (LoginController.Instance != null)
					{
						LoginController.Instance.LoginFailed();
					}
				}
				else
				{
					CommandRouter.Route(JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text));
				}
			}
			UnityWebRequest www = null;
			yield break;
			yield break;
		}

		// Token: 0x06002434 RID: 9268 RVA: 0x00059E29 File Offset: 0x00058029
		public static void LoginWithSteam()
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.LoginWithSteamInternal();
			}
		}

		// Token: 0x06002435 RID: 9269 RVA: 0x0012D138 File Offset: 0x0012B338
		private void LoginWithSteamInternal()
		{
			if (Time.time - this.m_timeOfLastLoginAttempt < 1f)
			{
				this.SendError(CommandType.auth, "Trying too soon!");
				return;
			}
			this.m_timeOfLastLoginAttempt = Time.time;
			if (this.m_loginInternalCo != null)
			{
				base.StopCoroutine(this.m_loginInternalCo);
			}
			this.m_loginInternalCo = this.LoginWithSteamCo();
			base.StartCoroutine(this.m_loginInternalCo);
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x00059E42 File Offset: 0x00058042
		private IEnumerator LoginWithSteamCo()
		{
			while (SolServerConnectionManager.Config == null)
			{
				yield return null;
			}
			this.m_steamTicket = null;
			Callback<GetTicketForWebApiResponse_t> steamGetTicketCallback = this.m_steamGetTicketCallback;
			if (steamGetTicketCallback != null)
			{
				steamGetTicketCallback.Dispose();
			}
			this.m_steamGetTicketCallback = Callback<GetTicketForWebApiResponse_t>.Create(new Callback<GetTicketForWebApiResponse_t>.DispatchDelegate(this.SteamGetTicketForWebResponse));
			SteamUser.GetAuthTicketForWebApi(null);
			float timeElapsed = 0f;
			while (string.IsNullOrEmpty(this.m_steamTicket) && timeElapsed < 10f)
			{
				yield return null;
				timeElapsed += Time.deltaTime;
			}
			if (string.IsNullOrEmpty(this.m_steamTicket))
			{
				Debug.LogWarning("Steam Ticket never populated");
				this.SendError(CommandType.auth, "No Steam Session Ticket Generated!");
				yield break;
			}
			string loginWithSteamUri = LoginApiManager.GetLoginWithSteamUri(SteamUtils.GetAppID().m_AppId);
			Debug.Log("Auth via " + loginWithSteamUri);
			WWWForm wwwform = new WWWForm();
			wwwform.AddField("ApiVersion", GlobalSettings.Values.Configs.Data.ServerApiVersion);
			wwwform.AddField("Ticket", this.m_steamTicket);
			using (UnityWebRequest www = UnityWebRequest.Post(loginWithSteamUri, wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning(www.error);
					this.SendError(CommandType.auth, www.error);
				}
				else
				{
					CommandRouter.Route(JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text));
				}
			}
			UnityWebRequest www = null;
			Callback<GetTicketForWebApiResponse_t> steamGetTicketCallback2 = this.m_steamGetTicketCallback;
			if (steamGetTicketCallback2 != null)
			{
				steamGetTicketCallback2.Dispose();
			}
			this.m_steamGetTicketCallback = null;
			this.m_steamTicket = null;
			yield break;
			yield break;
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x00059E51 File Offset: 0x00058051
		private void SteamGetTicketForWebResponse(GetTicketForWebApiResponse_t param)
		{
			this.m_steamTicket = BitConverter.ToString(param.m_rgubTicket).Replace("-", string.Empty);
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x00059E73 File Offset: 0x00058073
		private static bool CanUpdateSessionKey()
		{
			return SolServerConnectionManager.Config != null && SessionData.User != null && !string.IsNullOrEmpty(SessionData.User.Id) && !string.IsNullOrEmpty(SessionData.SessionKey);
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x00059EA3 File Offset: 0x000580A3
		private void SessionDataOnSessionDataCleared()
		{
			if (LoginApiManager.CanUpdateSessionKey())
			{
				base.StartCoroutine("DeleteSessionKeyCo");
			}
		}

		// Token: 0x0600243A RID: 9274 RVA: 0x00059EB8 File Offset: 0x000580B8
		private IEnumerator DeleteSessionKeyCo()
		{
			if (!LoginApiManager.CanUpdateSessionKey())
			{
				yield break;
			}
			using (UnityWebRequest www = UnityWebRequest.Get(LoginApiManager.GetDeleteSessionKeyUri()))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error deleting session key! " + www.error);
				}
			}
			UnityWebRequest www = null;
			yield break;
			yield break;
		}

		// Token: 0x0600243B RID: 9275 RVA: 0x0012D1A0 File Offset: 0x0012B3A0
		private void DeleteSessionKeyLocking()
		{
			if (!LoginApiManager.CanUpdateSessionKey())
			{
				return;
			}
			using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(LoginApiManager.GetDeleteSessionKeyUri()))
			{
				unityWebRequest.SendWebRequest();
				float num = 0f;
				while (!unityWebRequest.isDone && num < 1000f)
				{
					Thread.Sleep(100);
					num += 100f;
				}
				if (unityWebRequest.IsWebError())
				{
					Debug.Log("Error deleting session key! " + unityWebRequest.error);
				}
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x0600243C RID: 9276 RVA: 0x00059EC0 File Offset: 0x000580C0
		// (set) Token: 0x0600243D RID: 9277 RVA: 0x00059EC7 File Offset: 0x000580C7
		private static DateTime LastSessionKeyRefresh { get; set; } = DateTime.MinValue;

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x0600243E RID: 9278 RVA: 0x00059ECF File Offset: 0x000580CF
		// (set) Token: 0x0600243F RID: 9279 RVA: 0x00059ED6 File Offset: 0x000580D6
		public static bool ManuallyRefreshingSessionKey { get; set; } = false;

		// Token: 0x06002440 RID: 9280 RVA: 0x00059EDE File Offset: 0x000580DE
		private IEnumerator RefreshCo()
		{
			if (this.m_refreshWait == null)
			{
				this.m_refreshWait = new WaitForSeconds(1f);
			}
			for (;;)
			{
				yield return LoginApiManager.RefreshSessionKey();
				yield return this.m_refreshWait;
			}
			yield break;
		}

		// Token: 0x06002441 RID: 9281 RVA: 0x00059EED File Offset: 0x000580ED
		public static IEnumerator RefreshSessionKey()
		{
			if ((DateTime.UtcNow - LoginApiManager.LastSessionKeyRefresh).TotalSeconds >= 20.0 && LoginApiManager.CanUpdateSessionKey())
			{
				using (UnityWebRequest www = UnityWebRequest.Get(LoginApiManager.GetRefreshSessionKeyUri()))
				{
					yield return www.SendWebRequest();
					if (www.IsWebError())
					{
						Debug.LogWarning("Error refreshing session key! " + www.error);
					}
					else
					{
						LoginApiManager.LastSessionKeyRefresh = DateTime.UtcNow;
					}
				}
				UnityWebRequest www = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06002442 RID: 9282 RVA: 0x00059EF5 File Offset: 0x000580F5
		public static void UpdateSelectionIndexes(List<CharacterRecord> records)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.UpdateSelectionIndexesInternal(records);
			}
		}

		// Token: 0x06002443 RID: 9283 RVA: 0x00059F0F File Offset: 0x0005810F
		private void UpdateSelectionIndexesInternal(List<CharacterRecord> records)
		{
			if (this.m_updatePositionsCo != null)
			{
				return;
			}
			this.m_updatePositionsCo = this.UpdateSelectionIndexesCo(records);
			base.StartCoroutine(this.m_updatePositionsCo);
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x00059F34 File Offset: 0x00058134
		private IEnumerator UpdateSelectionIndexesCo(List<CharacterRecord> records)
		{
			WWWForm wwwform = new WWWForm();
			Dictionary<string, int> dictionary = new Dictionary<string, int>(records.Count);
			for (int i = 0; i < records.Count; i++)
			{
				dictionary.Add(records[i].Id, records[i].SelectionPositionIndex);
			}
			wwwform.AddField("characterIndexes", JsonConvert.SerializeObject(dictionary));
			using (UnityWebRequest www = UnityWebRequest.Post(LoginApiManager.GetUpdateCharacterIndexesUri(), wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error updating indexes! " + www.error);
				}
				else if (www.downloadHandler.text.Contains("returnToLogin"))
				{
					Debug.Log("INVALID SESSION!");
					GameManager.SceneCompositionManager.ReloadStartupScene();
				}
			}
			UnityWebRequest www = null;
			this.m_updatePositionsCo = null;
			yield break;
			yield break;
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x00059F4A File Offset: 0x0005814A
		public static void CreateCharacter(CharacterRecord record)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.CreateCharacterInternal(record);
			}
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x00059F64 File Offset: 0x00058164
		private void CreateCharacterInternal(CharacterRecord record)
		{
			if (this.m_createCharacterCo != null)
			{
				return;
			}
			this.m_createCharacterCo = this.CreateCharacterCo(record);
			base.StartCoroutine(this.m_createCharacterCo);
		}

		// Token: 0x06002447 RID: 9287 RVA: 0x00059F89 File Offset: 0x00058189
		private IEnumerator CreateCharacterCo(CharacterRecord record)
		{
			WWWForm wwwform = new WWWForm();
			wwwform.AddField("character", JsonConvert.SerializeObject(record));
			using (UnityWebRequest www = UnityWebRequest.Post(LoginApiManager.GetCharacterCreateUri(), wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error creating character! " + www.error);
					this.SendError(CommandType.createcharacter, www.error);
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					if (solServerCommand.State)
					{
						SessionData.SelectLastCharacter = true;
					}
					CommandRouter.Route(solServerCommand);
					if (solServerCommand.Args.ContainsKey("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			this.m_createCharacterCo = null;
			yield break;
			yield break;
		}

		// Token: 0x06002448 RID: 9288 RVA: 0x00059F9F File Offset: 0x0005819F
		public static void UpdateCharacterVisuals(CharacterRecord record)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.UpdateCharacterVisualsInternal(record);
			}
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x00059FB9 File Offset: 0x000581B9
		private void UpdateCharacterVisualsInternal(CharacterRecord record)
		{
			if (this.m_updateCharacterVisualsCo != null)
			{
				return;
			}
			this.m_updateCharacterVisualsCo = this.UpdateCharacterVisualsCo(record);
			base.StartCoroutine(this.m_updateCharacterVisualsCo);
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x00059FDE File Offset: 0x000581DE
		private IEnumerator UpdateCharacterVisualsCo(CharacterRecord record)
		{
			WWWForm wwwform = new WWWForm();
			wwwform.AddField("visuals", JsonConvert.SerializeObject(record.Visuals));
			using (UnityWebRequest www = UnityWebRequest.Post(LoginApiManager.GetUpdateCharacterVisualsUri(record.Id), wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error updating character visuals! " + www.error);
					this.SendError(CommandType.updatecharactervisuals, www.error);
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					SessionData.LastCreatedEditedCharacterId = (solServerCommand.State ? record.Id : null);
					CommandRouter.Route(solServerCommand);
					if (solServerCommand.Args.ContainsKey("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			this.m_updateCharacterVisualsCo = null;
			yield break;
			yield break;
		}

		// Token: 0x0600244B RID: 9291 RVA: 0x00059FF4 File Offset: 0x000581F4
		public static void DeleteCharacter(CharacterRecord record)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.DeleteCharacterInternal(record);
			}
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x0005A00E File Offset: 0x0005820E
		private void DeleteCharacterInternal(CharacterRecord record)
		{
			if (this.m_deleteCharacterCo != null)
			{
				return;
			}
			this.m_deleteCharacterCo = this.DeleteCharacterCo(record);
			base.StartCoroutine(this.m_deleteCharacterCo);
		}

		// Token: 0x0600244D RID: 9293 RVA: 0x0005A033 File Offset: 0x00058233
		private IEnumerator DeleteCharacterCo(CharacterRecord record)
		{
			using (UnityWebRequest www = UnityWebRequest.Get(LoginApiManager.GetCharacterDeleteUri(record.Id)))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error deleting character! " + www.error);
					this.SendError(CommandType.deletecharacter, www.error);
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					CommandRouter.Route(solServerCommand);
					if (solServerCommand.Args.ContainsKey("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			this.m_deleteCharacterCo = null;
			yield break;
			yield break;
		}

		// Token: 0x0600244E RID: 9294 RVA: 0x0005A049 File Offset: 0x00058249
		public static void SetActiveCharacters(List<CharacterRecord> records, Action<bool, string> callback)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.SetActiveCharactersInternal(records, callback);
			}
		}

		// Token: 0x0600244F RID: 9295 RVA: 0x0005A064 File Offset: 0x00058264
		private void SetActiveCharactersInternal(List<CharacterRecord> records, Action<bool, string> callback)
		{
			if (this.m_designatePrimaryCharacterCo != null)
			{
				return;
			}
			this.m_designatePrimaryCharacterCo = this.SetActiveCharactersCo(records, callback);
			base.StartCoroutine(this.m_designatePrimaryCharacterCo);
		}

		// Token: 0x06002450 RID: 9296 RVA: 0x0005A08A File Offset: 0x0005828A
		private IEnumerator SetActiveCharactersCo(List<CharacterRecord> records, Action<bool, string> callback)
		{
			bool success = false;
			string error = string.Empty;
			WWWForm wwwform = new WWWForm();
			string[] array = new string[records.Count];
			for (int i = 0; i < records.Count; i++)
			{
				array[i] = records[i].Id;
			}
			wwwform.AddField("activeIds", JsonConvert.SerializeObject(array));
			using (UnityWebRequest www = UnityWebRequest.Post(LoginApiManager.GetSetActiveCharactersUri(), wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error settings active characters! " + www.error);
					this.SendError(CommandType.deletecharacter, www.error);
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					CommandRouter.Route(solServerCommand);
					string text;
					if (solServerCommand.TryGetArgValue(SolServerCommand.kErrorKey, out text))
					{
						error = text;
					}
					else
					{
						success = true;
					}
					if (solServerCommand.Args.ContainsKey("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			if (callback != null)
			{
				callback(success, error);
			}
			this.m_designatePrimaryCharacterCo = null;
			yield break;
			yield break;
		}

		// Token: 0x06002451 RID: 9297 RVA: 0x0005A0A7 File Offset: 0x000582A7
		public static void UpdatePortrait(CharacterRecord record)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.UpdatePortraitInternal(record);
			}
		}

		// Token: 0x06002452 RID: 9298 RVA: 0x0005A0C1 File Offset: 0x000582C1
		private void UpdatePortraitInternal(CharacterRecord record)
		{
			if (this.m_updatePortraitCo != null)
			{
				return;
			}
			this.m_updatePortraitCo = this.UpdatePortraitCo(record);
			base.StartCoroutine(this.m_updatePortraitCo);
		}

		// Token: 0x06002453 RID: 9299 RVA: 0x0005A0E6 File Offset: 0x000582E6
		private IEnumerator UpdatePortraitCo(CharacterRecord record)
		{
			using (UnityWebRequest www = UnityWebRequest.Get(LoginApiManager.GetUpdatePortraitUri(record.Id, record.Settings.PortraitId)))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error updating portrait! " + www.error);
				}
				else if (www.downloadHandler.text.Contains("returnToLogin"))
				{
					Debug.Log("INVALID SESSION!");
					GameManager.SceneCompositionManager.ReloadStartupScene();
				}
			}
			UnityWebRequest www = null;
			this.m_updatePortraitCo = null;
			yield break;
			yield break;
		}

		// Token: 0x06002454 RID: 9300 RVA: 0x0012D228 File Offset: 0x0012B428
		public static void PerformZoneCheckForSelection(ZoneId targetZoneId, CharacterRecord character, Action<bool> callback)
		{
			if (ClientGameManager.LoginApiManager == null || !LoginApiManager.CanUpdateSessionKey())
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			ClientGameManager.LoginApiManager.PerformZoneCheckInternal(targetZoneId, character, null, callback);
		}

		// Token: 0x06002455 RID: 9301 RVA: 0x0012D26C File Offset: 0x0012B46C
		public static void PerformZoneCheck(ZoneId targetZoneId, Action<bool> callback)
		{
			if (ClientGameManager.LoginApiManager == null || !LoginApiManager.CanUpdateSessionKey())
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			ClientGameManager.LoginApiManager.PerformZoneCheckInternal(targetZoneId, null, null, callback);
		}

		// Token: 0x06002456 RID: 9302 RVA: 0x0005A0FC File Offset: 0x000582FC
		public static void RequestInstanceChange(ZoneId targetZoneId, int instanceId, Action<bool> callback)
		{
			if (ClientGameManager.LoginApiManager == null || !LoginApiManager.CanUpdateSessionKey())
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			ClientGameManager.LoginApiManager.PerformZoneCheckInternal(targetZoneId, null, new int?(instanceId), callback);
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06002457 RID: 9303 RVA: 0x0005A130 File Offset: 0x00058330
		// (set) Token: 0x06002458 RID: 9304 RVA: 0x0005A137 File Offset: 0x00058337
		public static string LastZoneCheckError { get; private set; }

		// Token: 0x06002459 RID: 9305 RVA: 0x0012D2B0 File Offset: 0x0012B4B0
		private void PerformZoneCheckInternal(ZoneId targetZoneId, CharacterRecord character, int? requestedInstanceId, Action<bool> callback)
		{
			if (this.m_zoneCheckCo != null)
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			this.m_currentZoneIdCheck = new ZoneId?(targetZoneId);
			this.m_zoneCheckCo = this.PerformZoneCheckCo(targetZoneId, character, requestedInstanceId, callback);
			base.StartCoroutine(this.m_zoneCheckCo);
		}

		// Token: 0x0600245A RID: 9306 RVA: 0x0005A13F File Offset: 0x0005833F
		private IEnumerator PerformZoneCheckCo(ZoneId targetZoneId, CharacterRecord character, int? requestedInstanceId, Action<bool> callback)
		{
			string uri = string.Empty;
			if (character != null)
			{
				uri = LoginApiManager.GetZoneCheckUriForSelection(targetZoneId, character);
			}
			else if (requestedInstanceId != null)
			{
				uri = LoginApiManager.GetZoneCheckUriWithInstanceId(targetZoneId, requestedInstanceId.Value);
			}
			else
			{
				uri = LoginApiManager.GetZoneCheckUri(targetZoneId);
			}
			using (UnityWebRequest www = UnityWebRequest.Get(uri))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					if (callback != null)
					{
						callback(false);
					}
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					if (solServerCommand.State)
					{
						int num = solServerCommand.DeserializeKey("port");
						if (num != 0 && this.m_currentZoneIdCheck != null)
						{
							ZoneRecord zoneRecord = SessionData.GetZoneRecord(this.m_currentZoneIdCheck.Value);
							int instanceId = solServerCommand.Args.ContainsKey("instanceId") ? solServerCommand.DeserializeKey("instanceId") : 0;
							if (zoneRecord != null)
							{
								zoneRecord.Port = num;
								zoneRecord.InstanceId = instanceId;
							}
						}
					}
					else
					{
						string text = (string)solServerCommand.Args["err"];
						if (!string.IsNullOrEmpty(text))
						{
							LoginApiManager.LastZoneCheckError = text;
							if (LocalPlayer.GameEntity != null)
							{
								MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text);
							}
						}
						if (solServerCommand.Args.ContainsKey("returnToLogin"))
						{
							Debug.Log("INVALID SESSION!");
							GameManager.SceneCompositionManager.ReloadStartupScene();
						}
					}
					if (callback != null)
					{
						callback(solServerCommand.State);
					}
				}
			}
			UnityWebRequest www = null;
			this.m_currentZoneIdCheck = null;
			this.m_zoneCheckCo = null;
			yield break;
			yield break;
		}

		// Token: 0x0600245B RID: 9307 RVA: 0x0005A16B File Offset: 0x0005836B
		public static void RenameCharacter(CharacterRecord record, string requestedName, Action<bool, string> callback)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.RenameCharacterInternal(record, requestedName, callback);
			}
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x0005A187 File Offset: 0x00058387
		private void RenameCharacterInternal(CharacterRecord record, string requestedName, Action<bool, string> callback)
		{
			if (this.m_renameCharacterCo != null)
			{
				if (callback != null)
				{
					callback(false, "Unknown Error");
				}
				return;
			}
			this.m_renameCharacterCo = this.RenameCharacterCo(record, requestedName, callback);
			base.StartCoroutine(this.m_renameCharacterCo);
		}

		// Token: 0x0600245D RID: 9309 RVA: 0x0005A1BD File Offset: 0x000583BD
		private IEnumerator RenameCharacterCo(CharacterRecord record, string requestedName, Action<bool, string> callback)
		{
			bool success = false;
			string error = string.Empty;
			string renameCharacterUri = LoginApiManager.GetRenameCharacterUri(record.Id, requestedName);
			using (UnityWebRequest www = UnityWebRequest.Get(renameCharacterUri))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error renaming character! " + www.error);
				}
				else
				{
					string text;
					if (JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text).TryGetArgValue(SolServerCommand.kErrorKey, out text))
					{
						error = text;
					}
					else
					{
						success = true;
					}
					if (www.downloadHandler.text.Contains("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			if (callback != null)
			{
				callback(success, error);
			}
			this.m_renameCharacterCo = null;
			yield break;
			yield break;
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x0005A1E1 File Offset: 0x000583E1
		public static void PurchaseSteamSubscription(Action<bool, string> callback)
		{
			if (SessionData.User == null || string.IsNullOrEmpty(SessionData.SessionKey))
			{
				return;
			}
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.PurchaseSteamSubscriptionInternal(callback);
			}
		}

		// Token: 0x0600245F RID: 9311 RVA: 0x0005A20F File Offset: 0x0005840F
		private void PurchaseSteamSubscriptionInternal(Action<bool, string> callback)
		{
			if (this.m_purchaseSteamSubscriptionCo == null)
			{
				this.m_purchaseSteamSubscriptionCo = this.PurchaseSteamSubscriptionCo(callback);
				base.StartCoroutine(this.m_purchaseSteamSubscriptionCo);
			}
		}

		// Token: 0x06002460 RID: 9312 RVA: 0x0005A233 File Offset: 0x00058433
		private IEnumerator PurchaseSteamSubscriptionCo(Action<bool, string> callback)
		{
			CSteamID steamID = SteamUser.GetSteamID();
			using (UnityWebRequest www = UnityWebRequest.Get(LoginApiManager.GetPurchaseSteamSubscriptionUri(SteamUtils.GetAppID().m_AppId, steamID.m_SteamID)))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error purchasing steam subscription! " + www.error);
					this.SendError(CommandType.deletecharacter, www.error);
					if (callback != null)
					{
						callback(false, www.error);
					}
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					CommandRouter.Route(solServerCommand);
					object obj;
					string arg = solServerCommand.Args.TryGetValue(SolServerCommand.kErrorKey, out obj) ? obj.ToString() : string.Empty;
					if (callback != null)
					{
						callback(solServerCommand.State, arg);
					}
					if (solServerCommand.Args.ContainsKey("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			this.m_purchaseSteamSubscriptionCo = null;
			yield break;
			yield break;
		}

		// Token: 0x06002461 RID: 9313 RVA: 0x0005A249 File Offset: 0x00058449
		public static void FinalizeSteamTransaction(ulong orderId, Action<bool, string> callback)
		{
			if (ClientGameManager.LoginApiManager != null)
			{
				ClientGameManager.LoginApiManager.FinalizeSteamTransactionInternal(orderId, callback);
			}
		}

		// Token: 0x06002462 RID: 9314 RVA: 0x0005A264 File Offset: 0x00058464
		private void FinalizeSteamTransactionInternal(ulong orderId, Action<bool, string> callback)
		{
			if (this.m_finalizeSteamTransactionCo == null)
			{
				this.m_finalizeSteamTransactionCo = this.FinalizeSteamTransactionCo(orderId, callback);
				base.StartCoroutine(this.m_finalizeSteamTransactionCo);
			}
		}

		// Token: 0x06002463 RID: 9315 RVA: 0x0005A289 File Offset: 0x00058489
		private IEnumerator FinalizeSteamTransactionCo(ulong orderId, Action<bool, string> callback)
		{
			CSteamID steamID = SteamUser.GetSteamID();
			using (UnityWebRequest www = UnityWebRequest.Get(LoginApiManager.GetFinalizeSteamTransactionUri(SteamUtils.GetAppID().m_AppId, steamID.m_SteamID, orderId)))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error finalizing steam transaction! " + www.error);
					this.SendError(CommandType.deletecharacter, www.error);
					if (callback != null)
					{
						callback(false, www.error);
					}
				}
				else
				{
					SolServerCommand solServerCommand = JsonConvert.DeserializeObject<SolServerCommand>(www.downloadHandler.text);
					CommandRouter.Route(solServerCommand);
					object obj;
					string arg = solServerCommand.Args.TryGetValue(SolServerCommand.kErrorKey, out obj) ? obj.ToString() : "UNKNOWN";
					if (callback != null)
					{
						callback(solServerCommand.State, arg);
					}
					if (solServerCommand.Args.ContainsKey("returnToLogin"))
					{
						Debug.Log("INVALID SESSION!");
						GameManager.SceneCompositionManager.ReloadStartupScene();
					}
				}
			}
			UnityWebRequest www = null;
			this.m_finalizeSteamTransactionCo = null;
			yield break;
			yield break;
		}

		// Token: 0x04002737 RID: 10039
		private IEnumerator m_refreshCo;

		// Token: 0x04002738 RID: 10040
		private const string kPortKey = "port";

		// Token: 0x04002739 RID: 10041
		private const string kInstanceIdKey = "instanceId";

		// Token: 0x0400273A RID: 10042
		private const string kReturnToLoginKey = "returnToLogin";

		// Token: 0x0400273B RID: 10043
		private const string kToRenameKey = "torename";

		// Token: 0x0400273C RID: 10044
		private float m_timeOfLastLoginAttempt = -1f;

		// Token: 0x0400273D RID: 10045
		private IEnumerator m_loginInternalCo;

		// Token: 0x0400273E RID: 10046
		[NonSerialized]
		private string m_steamTicket;

		// Token: 0x0400273F RID: 10047
		private Callback<GetTicketForWebApiResponse_t> m_steamGetTicketCallback;

		// Token: 0x04002740 RID: 10048
		private const float kSessionKeyRefreshTime = 20f;

		// Token: 0x04002742 RID: 10050
		private WaitForSeconds m_refreshWait;

		// Token: 0x04002744 RID: 10052
		private IEnumerator m_updatePositionsCo;

		// Token: 0x04002745 RID: 10053
		private IEnumerator m_createCharacterCo;

		// Token: 0x04002746 RID: 10054
		private IEnumerator m_updateCharacterVisualsCo;

		// Token: 0x04002747 RID: 10055
		private IEnumerator m_deleteCharacterCo;

		// Token: 0x04002748 RID: 10056
		private IEnumerator m_designatePrimaryCharacterCo;

		// Token: 0x04002749 RID: 10057
		private IEnumerator m_updatePortraitCo;

		// Token: 0x0400274A RID: 10058
		private IEnumerator m_zoneCheckCo;

		// Token: 0x0400274B RID: 10059
		private ZoneId? m_currentZoneIdCheck;

		// Token: 0x0400274D RID: 10061
		private IEnumerator m_renameCharacterCo;

		// Token: 0x0400274E RID: 10062
		private IEnumerator m_purchaseSteamSubscriptionCo;

		// Token: 0x0400274F RID: 10063
		private IEnumerator m_finalizeSteamTransactionCo;
	}
}
