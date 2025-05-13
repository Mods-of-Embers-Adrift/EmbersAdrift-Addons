using System;
using System.Text;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities.Extensions;
using Steamworks;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000515 RID: 1301
	[DisallowMultipleComponent]
	public class SteamManager : MonoBehaviour
	{
		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06002620 RID: 9760 RVA: 0x0005AE79 File Offset: 0x00059079
		public static bool SteamIsAvailable
		{
			get
			{
				return SteamManager.Initialized && SteamManager.SteamIsRunning;
			}
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x0004475B File Offset: 0x0004295B
		public static void UpdateSteamAppIdFile(bool isGMClient)
		{
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06002622 RID: 9762 RVA: 0x0005AE89 File Offset: 0x00059089
		// (set) Token: 0x06002623 RID: 9763 RVA: 0x0005AE90 File Offset: 0x00059090
		public static bool Initialized { get; private set; }

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06002624 RID: 9764 RVA: 0x0005AE98 File Offset: 0x00059098
		// (set) Token: 0x06002625 RID: 9765 RVA: 0x0005AE9F File Offset: 0x0005909F
		private static bool SteamIsRunning { get; set; }

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06002626 RID: 9766 RVA: 0x0005AEA7 File Offset: 0x000590A7
		// (set) Token: 0x06002627 RID: 9767 RVA: 0x0005AEAE File Offset: 0x000590AE
		public static SteamConfig Config { get; set; }

		// Token: 0x06002628 RID: 9768 RVA: 0x0005AEB6 File Offset: 0x000590B6
		private static void SteamApiDebugTextHook(int severity, StringBuilder debugText)
		{
			Debug.LogWarning(debugText);
		}

		// Token: 0x06002629 RID: 9769 RVA: 0x0005AEBE File Offset: 0x000590BE
		private void Awake()
		{
			this.InitSteam();
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x0005AEC6 File Offset: 0x000590C6
		private void OnDestroy()
		{
			this.ShutdownSteam();
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x00134C78 File Offset: 0x00132E78
		private void Update()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}
			SteamManager.SteamIsRunning = SteamAPI.IsSteamRunning();
			if (!SteamManager.SteamIsRunning)
			{
				this.ShutdownSteam();
				base.enabled = false;
				return;
			}
			SteamAPI.RunCallbacks();
			if (this.m_overlayRequestTime != null && Time.time - this.m_overlayRequestTime.Value > 5f)
			{
				this.SteamLog("NO OVERLAY ENABLED?");
				if (ClientGameManager.UIManager && ClientGameManager.UIManager.ConfirmationDialog)
				{
					DialogOptions opts = new DialogOptions
					{
						ShowCloseButton = false,
						HideCancelButton = true,
						Title = "Steam Overlay Error",
						Text = "Your Steam Overlay failed to open! Please make sure that the game is running through Steam with the overlay enabled.",
						ConfirmationText = "OK"
					};
					ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
				}
				this.m_overlayRequestTime = null;
			}
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x00134D60 File Offset: 0x00132F60
		public void InitSteam()
		{
			if (SteamManager.Initialized)
			{
				return;
			}
			this.m_overlayRequestTime = null;
			if (!Packsize.Test())
			{
				Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
			}
			if (!DllCheck.Test())
			{
				Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
			}
			try
			{
				SteamManager.Initialized = SteamAPI.Init();
				SteamManager.SteamIsRunning = SteamAPI.IsSteamRunning();
			}
			catch (DllNotFoundException ex)
			{
				string str = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
				DllNotFoundException ex2 = ex;
				Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null), this);
				Application.Quit();
				return;
			}
			if (!SteamManager.Initialized)
			{
				Debug.LogWarning("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
				base.enabled = false;
				return;
			}
			if (this.m_steamApiWarningMessageHook == null)
			{
				this.m_steamApiWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamApiDebugTextHook);
				SteamClient.SetWarningMessageHook(this.m_steamApiWarningMessageHook);
			}
			Callback<GameOverlayActivated_t> gameOverlayActivated = this.m_gameOverlayActivated;
			if (gameOverlayActivated != null)
			{
				gameOverlayActivated.Dispose();
			}
			this.m_gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.GameOverlayActivatedCallback));
			Callback<MicroTxnAuthorizationResponse_t> microTxnAuthorizationResponse = this.m_microTxnAuthorizationResponse;
			if (microTxnAuthorizationResponse != null)
			{
				microTxnAuthorizationResponse.Dispose();
			}
			this.m_microTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.MicroTxnAuthorizationResponseCallback));
			CallResult<MicroTxnAuthorizationResponse_t> microTxnAuthorizationResult = this.m_microTxnAuthorizationResult;
			if (microTxnAuthorizationResult != null)
			{
				microTxnAuthorizationResult.Dispose();
			}
			this.m_microTxnAuthorizationResult = CallResult<MicroTxnAuthorizationResponse_t>.Create(new CallResult<MicroTxnAuthorizationResponse_t>.APIDispatchDelegate(this.MicroTxnAuthorizationResponseCallResult));
			this.SteamLog("Steam Initialized: " + SteamManager.Initialized.ToString());
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x00134EC8 File Offset: 0x001330C8
		public void ShutdownSteam()
		{
			if (SteamManager.Initialized)
			{
				SteamAPI.Shutdown();
				SteamManager.Initialized = false;
			}
			this.m_steamApiWarningMessageHook = null;
			Callback<GameOverlayActivated_t> gameOverlayActivated = this.m_gameOverlayActivated;
			if (gameOverlayActivated != null)
			{
				gameOverlayActivated.Dispose();
			}
			this.m_gameOverlayActivated = null;
			Callback<MicroTxnAuthorizationResponse_t> microTxnAuthorizationResponse = this.m_microTxnAuthorizationResponse;
			if (microTxnAuthorizationResponse != null)
			{
				microTxnAuthorizationResponse.Dispose();
			}
			this.m_microTxnAuthorizationResponse = null;
			CallResult<MicroTxnAuthorizationResponse_t> microTxnAuthorizationResult = this.m_microTxnAuthorizationResult;
			if (microTxnAuthorizationResult != null)
			{
				microTxnAuthorizationResult.Dispose();
			}
			this.m_microTxnAuthorizationResult = null;
			this.m_overlayRequestTime = null;
			this.SteamLog("Steam Shutdown");
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x0005AECE File Offset: 0x000590CE
		private void GameOverlayActivatedCallback(GameOverlayActivated_t param)
		{
			this.SteamLog("GAME OVERLAY ACTIVATED");
			this.m_overlayRequestTime = null;
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x00134F50 File Offset: 0x00133150
		private void MicroTxnAuthorizationResponseCallback(MicroTxnAuthorizationResponse_t param)
		{
			this.SteamLog(string.Format("[MicroTxnAuthorizationResponseCallback] MicroTxnAuthorizationResponse_t data m_unAppID={0}, m_ulOrderID={1}, m_bAuthorized={2}", param.m_unAppID, param.m_ulOrderID, param.m_bAuthorized));
			if (param.m_bAuthorized == 1)
			{
				LoginApiManager.FinalizeSteamTransaction(param.m_ulOrderID, delegate(bool success, string resultText)
				{
					if (ClientGameManager.UIManager && ClientGameManager.UIManager.InformationDialog)
					{
						DialogOptions opts = new DialogOptions
						{
							ShowCloseButton = false,
							Title = (success ? "Subscription Success" : "Subscription Failure"),
							Text = (success ? "Subscription activated! Please log out and back in to gain subscriber perks." : ("Subscription activation failure: " + resultText)),
							ConfirmationText = "OK"
						};
						ClientGameManager.UIManager.InformationDialog.Init(opts);
					}
				});
			}
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x00134FC4 File Offset: 0x001331C4
		private void MicroTxnAuthorizationResponseCallResult(MicroTxnAuthorizationResponse_t param, bool biofailure)
		{
			this.SteamLog(string.Format("[MicroTxnAuthorizationResponseCallResult] MicroTxnAuthorizationResponse_t data m_unAppID={0}, m_ulOrderID={1}, m_bAuthorized={2}, bioFailure={3}", new object[]
			{
				param.m_unAppID,
				param.m_ulOrderID,
				param.m_bAuthorized,
				biofailure
			}));
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x0013501C File Offset: 0x0013321C
		public void PurchaseSubscriptionRequest()
		{
			if (SteamManager.Initialized)
			{
				this.SteamLog("Requesting Subscription Purchase");
				LoginApiManager.PurchaseSteamSubscription(delegate(bool success, string resultText)
				{
					this.SteamLog(string.Format("Subscription Request: {0} : {1}", success, resultText));
					if (success)
					{
						this.m_overlayRequestTime = new float?(Time.time);
						return;
					}
					if (ClientGameManager.UIManager && ClientGameManager.UIManager.InformationDialog)
					{
						DialogOptions opts2 = new DialogOptions
						{
							ShowCloseButton = false,
							Title = "Subscription Failure",
							Text = "Subscription activation failure: " + resultText,
							ConfirmationText = "OK"
						};
						ClientGameManager.UIManager.InformationDialog.Init(opts2);
					}
				});
				return;
			}
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.InformationDialog)
			{
				DialogOptions opts = new DialogOptions
				{
					ShowCloseButton = false,
					Title = "Steam Failure",
					Text = "Steam failed to initialize. Please make sure you are launching the game through Steam and have the overlay enabled.",
					ConfirmationText = "OK"
				};
				ClientGameManager.UIManager.InformationDialog.Init(opts);
			}
		}

		// Token: 0x06002632 RID: 9778 RVA: 0x0005AEE7 File Offset: 0x000590E7
		private void SteamLog(string txt)
		{
			Debug.Log("[STEAM] " + txt);
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x0005AEF9 File Offset: 0x000590F9
		private void PrintSteamId()
		{
			Debug.Log(SteamUtils.GetAppID());
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x001350B0 File Offset: 0x001332B0
		private void InitSteamSubDialog()
		{
			SteamSubscriptionDialogOptions opts = new SteamSubscriptionDialogOptions
			{
				Title = "Test Dialog",
				ConfirmationText = "YES",
				CancelText = "Cancel",
				ShowCloseButton = false,
				AllowDragging = false,
				BlockInteractions = true,
				BackgroundBlockerColor = Color.black.GetColorWithAlpha(0.5f)
			};
			ClientGameManager.UIManager.SteamSubscriptionDialog.Init(opts);
		}

		// Token: 0x04002846 RID: 10310
		private const int kAppId = 3336530;

		// Token: 0x04002847 RID: 10311
		private float? m_overlayRequestTime;

		// Token: 0x04002848 RID: 10312
		private SteamAPIWarningMessageHook_t m_steamApiWarningMessageHook;

		// Token: 0x04002849 RID: 10313
		private Callback<GameOverlayActivated_t> m_gameOverlayActivated;

		// Token: 0x0400284A RID: 10314
		private Callback<MicroTxnAuthorizationResponse_t> m_microTxnAuthorizationResponse;

		// Token: 0x0400284B RID: 10315
		private CallResult<MicroTxnAuthorizationResponse_t> m_microTxnAuthorizationResult;
	}
}
