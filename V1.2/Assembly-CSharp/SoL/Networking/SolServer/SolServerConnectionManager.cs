using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F5 RID: 1013
	public class SolServerConnectionManager : MonoBehaviour
	{
		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06001AEC RID: 6892 RVA: 0x00054E12 File Offset: 0x00053012
		// (set) Token: 0x06001AED RID: 6893 RVA: 0x00054E19 File Offset: 0x00053019
		public static SolServerConnection CurrentConnection { get; private set; }

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06001AEE RID: 6894 RVA: 0x00054E21 File Offset: 0x00053021
		// (set) Token: 0x06001AEF RID: 6895 RVA: 0x00054E28 File Offset: 0x00053028
		public static ClientConfig Config { get; private set; }

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06001AF0 RID: 6896 RVA: 0x00054E30 File Offset: 0x00053030
		public static bool IsOnline
		{
			get
			{
				return SolServerConnectionManager.CurrentConnection != null && SolServerConnectionManager.CurrentConnection.IsConnected;
			}
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x0010A52C File Offset: 0x0010872C
		public static void ClearSession()
		{
			if (SolServerConnectionManager.CurrentConnection == null)
			{
				return;
			}
			CommandClass.client.NewCommand(CommandType.clearsession).Send();
			SessionData.SessionKey = null;
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x00054E4B File Offset: 0x0005304B
		private void Awake()
		{
			if (SolServerConnectionManager.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			SolServerConnectionManager.Instance = this;
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x00054E6C File Offset: 0x0005306C
		private void Start()
		{
			this.m_login = base.gameObject.AddComponent<SolServerConnection>();
			this.m_login.Type = SolServerConnectionType.Login;
			this.m_social = base.gameObject.AddComponent<SolServerConnection>();
			this.m_social.Type = SolServerConnectionType.Social;
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0010A55C File Offset: 0x0010875C
		private void Update()
		{
			if (SolServerConnectionManager.CurrentConnection != null && SolServerConnectionManager.CurrentConnection.IsConnected)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow > this.m_nextPing)
				{
					this.m_nextPing = utcNow.AddSeconds(3.0);
					this.SendPing();
				}
			}
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x0010A5B4 File Offset: 0x001087B4
		private void SendPing()
		{
			if (LocalPlayer.GameEntity != null && LocalZoneManager.ZoneRecord != null)
			{
				Vector3 position = LocalPlayer.GameEntity.transform.position;
				RolePacked packed = GlobalSettings.Values.Roles.GetSpecializedRoleFlag(LocalPlayer.GameEntity.CharacterData.SpecializedRoleId).GetPacked();
				if (packed == RolePacked.Invalid)
				{
					packed = GlobalSettings.Values.Roles.GetBaseRoleFlag(LocalPlayer.GameEntity.CharacterData.BaseRoleId).GetPacked();
				}
				SubZoneId subZoneId = SubZoneId.None;
				SpawnVolumeOverride spawnVolumeOverride;
				if (LocalZoneManager.TryGetSpawnVolumeOverride(LocalPlayer.GameEntity.gameObject.transform.position, out spawnVolumeOverride))
				{
					subZoneId = spawnVolumeOverride.SubZoneId;
				}
				byte level = LocalPlayer.GameEntity.CharacterData ? LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Value : 0;
				byte groupEmberRingIndex = MapUI.GetGroupEmberRingIndex();
				SolServerConnectionManager.CurrentConnection.Ping(position.x, position.y, position.z, LocalZoneManager.ZoneRecord.ZoneId, (byte)subZoneId, (byte)packed, level, groupEmberRingIndex);
				return;
			}
			SolServerConnectionManager.CurrentConnection.Ping(100f, 100f, 100f, 0, 0, 0, 0, 0);
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x0010A6DC File Offset: 0x001088DC
		public void InitializeConnection()
		{
			this.GetSolLoginConfig(new Action<ClientConfig>(this.SolLoginConfigLoaded));
			this.m_nextPing = DateTime.UtcNow.AddSeconds(3.0);
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x00054EA8 File Offset: 0x000530A8
		public void Disconnect()
		{
			if (SolServerConnectionManager.CurrentConnection != null)
			{
				SolServerConnectionManager.CurrentConnection.Disconnect();
				SolServerConnectionManager.CurrentConnection = null;
			}
		}

		// Token: 0x06001AF8 RID: 6904 RVA: 0x00054EC7 File Offset: 0x000530C7
		private void InitializeConnection(SolServerConnection connection, string address, int port, Action<bool> callback)
		{
			Debug.Log(string.Format("Connecting to {0} @ {1}:{2}", connection.Type, address, port));
			connection.Initialize(address, port, callback);
			SolServerConnectionManager.CurrentConnection = connection;
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x00054EFA File Offset: 0x000530FA
		private void LoginConnectedCallback()
		{
			Debug.Log("Connected to login!");
		}

		// Token: 0x06001AFA RID: 6906 RVA: 0x0010A718 File Offset: 0x00108918
		private void SocialConnectedCallback(bool isReauth)
		{
			Debug.Log("Connected to social!");
			Dictionary<string, object> dictionary = SolServerCommandDictionaryPool.GetDictionary();
			dictionary.Add("Username", SessionData.User.UserName);
			dictionary.Add("session_key", SessionData.SessionKey);
			dictionary.Add("charId", SessionData.SelectedCharacter.Id);
			CommandClass.client.NewCommand(isReauth ? CommandType.reauth : CommandType.auth, dictionary).Send();
		}

		// Token: 0x06001AFB RID: 6907 RVA: 0x00054F06 File Offset: 0x00053106
		public void SwitchToSocial(WorldRecord world)
		{
			this.m_login.Disconnect(true);
			this.InitializeConnection(this.m_social, world.Address, world.Port, new Action<bool>(this.SocialConnectedCallback));
		}

		// Token: 0x06001AFC RID: 6908 RVA: 0x00054F38 File Offset: 0x00053138
		private void SolLoginConfigLoaded(ClientConfig config)
		{
			if (config == null)
			{
				Debug.LogError("Could not find a suitable configuration!");
				return;
			}
		}

		// Token: 0x06001AFD RID: 6909 RVA: 0x0010A788 File Offset: 0x00108988
		private void GetSolLoginConfig(Action<ClientConfig> callback)
		{
			ClientConfig clientConfig = ConfigHelpers.GetClientConfig();
			if (clientConfig != null)
			{
				SolServerConnectionManager.Config = clientConfig;
				callback(clientConfig);
				return;
			}
		}

		// Token: 0x0400222A RID: 8746
		public static SolServerConnectionManager Instance;

		// Token: 0x0400222B RID: 8747
		private const string kBaseConfigURL = "http://stormhavenstudios.com/sol-cfg/?v=1";

		// Token: 0x0400222C RID: 8748
		private SolServerConnection m_login;

		// Token: 0x0400222D RID: 8749
		private SolServerConnection m_social;

		// Token: 0x0400222E RID: 8750
		private const float kPingDelay = 3f;

		// Token: 0x0400222F RID: 8751
		private DateTime m_nextPing = DateTime.MinValue;
	}
}
