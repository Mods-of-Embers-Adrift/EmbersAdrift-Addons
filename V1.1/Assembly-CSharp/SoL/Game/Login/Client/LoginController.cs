using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using SoL.Game.Login.Client.Creation;
using SoL.Game.Login.Client.Selection;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B2E RID: 2862
	public class LoginController : MonoBehaviour
	{
		// Token: 0x14000115 RID: 277
		// (add) Token: 0x060057CB RID: 22475 RVA: 0x001E4644 File Offset: 0x001E2844
		// (remove) Token: 0x060057CC RID: 22476 RVA: 0x001E4678 File Offset: 0x001E2878
		public static event Action<LoginStageType> StageChanged;

		// Token: 0x170014AD RID: 5293
		// (get) Token: 0x060057CD RID: 22477 RVA: 0x0007A90A File Offset: 0x00078B0A
		// (set) Token: 0x060057CE RID: 22478 RVA: 0x0007A912 File Offset: 0x00078B12
		public CinemachineBrain Brain { get; private set; }

		// Token: 0x170014AE RID: 5294
		// (get) Token: 0x060057CF RID: 22479 RVA: 0x0007A91B File Offset: 0x00078B1B
		// (set) Token: 0x060057D0 RID: 22480 RVA: 0x0007A923 File Offset: 0x00078B23
		public int LastFrameStageChanged { get; private set; }

		// Token: 0x170014AF RID: 5295
		// (get) Token: 0x060057D1 RID: 22481 RVA: 0x0007A92C File Offset: 0x00078B2C
		// (set) Token: 0x060057D2 RID: 22482 RVA: 0x0007A934 File Offset: 0x00078B34
		internal bool IsSteam { get; set; }

		// Token: 0x170014B0 RID: 5296
		// (get) Token: 0x060057D3 RID: 22483 RVA: 0x0007A93D File Offset: 0x00078B3D
		// (set) Token: 0x060057D4 RID: 22484 RVA: 0x0007A945 File Offset: 0x00078B45
		public SelectionDirector SelectionDirector { get; private set; }

		// Token: 0x170014B1 RID: 5297
		// (get) Token: 0x060057D5 RID: 22485 RVA: 0x0007A94E File Offset: 0x00078B4E
		// (set) Token: 0x060057D6 RID: 22486 RVA: 0x0007A956 File Offset: 0x00078B56
		public CreationDirector CreationDirector { get; private set; }

		// Token: 0x170014B2 RID: 5298
		// (get) Token: 0x060057D7 RID: 22487 RVA: 0x0007A95F File Offset: 0x00078B5F
		// (set) Token: 0x060057D8 RID: 22488 RVA: 0x0007A967 File Offset: 0x00078B67
		public bool IsLoggedInViaSteam { get; internal set; }

		// Token: 0x170014B3 RID: 5299
		// (get) Token: 0x060057D9 RID: 22489 RVA: 0x0007A970 File Offset: 0x00078B70
		// (set) Token: 0x060057DA RID: 22490 RVA: 0x001E46AC File Offset: 0x001E28AC
		public LoginStageType Stage
		{
			get
			{
				return this.m_stage;
			}
			private set
			{
				if (this.m_stage == value)
				{
					return;
				}
				LoginStage stage = this.GetStage(this.m_stage);
				if (stage != null)
				{
					stage.StageExit();
				}
				if (ClientGameManager.UIManager && ClientGameManager.UIManager.TutorialPopupCenter)
				{
					if ((this.m_stage == LoginStageType.Authenticate || this.m_stage == LoginStageType.AuthenticateSteam) && value == LoginStageType.CharacterSelection)
					{
						TutorialPopupOptions loginNotification = GlobalSettings.Values.General.GetLoginNotification();
						if (!string.IsNullOrEmpty(loginNotification.Title) && !string.IsNullOrEmpty(loginNotification.Text))
						{
							ClientGameManager.UIManager.TutorialPopupCenter.Init(loginNotification);
						}
					}
					else if (ClientGameManager.UIManager.TutorialPopupCenter.Visible)
					{
						ClientGameManager.UIManager.TutorialPopupCenter.Hide(false);
					}
				}
				this.m_stage = value;
				this.LastFrameStageChanged = Time.frameCount;
				foreach (KeyValuePair<LoginStageType, LoginStage> keyValuePair in this.m_stageDict)
				{
					if (!(keyValuePair.Value == null))
					{
						keyValuePair.Value.Toggle(keyValuePair.Key == this.m_stage);
					}
				}
				this.SetBackText(this.m_stage);
				LoginStage stage2 = this.GetStage(this.m_stage);
				if (stage2 != null)
				{
					stage2.StageEnter();
				}
				Action<LoginStageType> stageChanged = LoginController.StageChanged;
				if (stageChanged == null)
				{
					return;
				}
				stageChanged(this.m_stage);
			}
		}

		// Token: 0x060057DB RID: 22491 RVA: 0x001E4820 File Offset: 0x001E2A20
		private LoginStage GetStage(LoginStageType type)
		{
			LoginStage result = null;
			this.m_stageDict.TryGetValue(type, out result);
			return result;
		}

		// Token: 0x060057DC RID: 22492 RVA: 0x0007A978 File Offset: 0x00078B78
		public bool TryGetLoginStage(LoginStageType type, out LoginStage stage)
		{
			return this.m_stageDict.TryGetValue(type, out stage);
		}

		// Token: 0x060057DD RID: 22493 RVA: 0x001E4840 File Offset: 0x001E2A40
		public bool TryGetLoginStageAsType<T>(LoginStageType type, out T stage) where T : LoginStage
		{
			stage = default(T);
			LoginStage loginStage;
			if (this.m_stageDict.TryGetValue(type, out loginStage))
			{
				T t = loginStage as T;
				if (t != null)
				{
					stage = t;
					return true;
				}
			}
			return false;
		}

		// Token: 0x060057DE RID: 22494 RVA: 0x001E4884 File Offset: 0x001E2A84
		private void Awake()
		{
			if (LoginController.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			LoginController.Instance = this;
			this.m_stageDict = new Dictionary<LoginStageType, LoginStage>
			{
				{
					LoginStageType.StartScreen,
					this.m_startScreen
				},
				{
					LoginStageType.Authenticate,
					this.m_authenticate
				},
				{
					LoginStageType.CharacterSelection,
					this.m_characterSelection
				},
				{
					LoginStageType.CharacterCreation,
					this.m_characterCreation
				},
				{
					LoginStageType.AuthenticateSteam,
					this.m_authenticateSteam
				}
			};
			foreach (KeyValuePair<LoginStageType, LoginStage> keyValuePair in this.m_stageDict)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value.AssignController(this, keyValuePair.Key);
				}
			}
			this.m_backButton.onClick.AddListener(new UnityAction(this.PreviousStage));
			this.Stage = LoginStageType.StartScreen;
			if (SessionData.ReturnToCharacterSelection)
			{
				LoginApiManager.LoginWithSessionKey();
				SessionData.ReturnToCharacterSelection = false;
			}
			else
			{
				this.ResetSession();
			}
			MessageManager.ResetQueues();
			this.SelectionDirector = this.m_characterSelection.gameObject.GetComponent<SelectionDirector>();
			this.CreationDirector = this.m_characterCreation.gameObject.GetComponent<CreationDirector>();
			this.m_selectionStage = (this.m_characterSelection as LoginStageCharacterSelection);
		}

		// Token: 0x060057DF RID: 22495 RVA: 0x0007A987 File Offset: 0x00078B87
		private IEnumerator Start()
		{
			while (CinemachineCore.Instance == null || CinemachineCore.Instance.BrainCount <= 0)
			{
				yield return null;
			}
			this.Brain = CinemachineCore.Instance.GetActiveBrain(0);
			yield break;
		}

		// Token: 0x060057E0 RID: 22496 RVA: 0x0007A996 File Offset: 0x00078B96
		private void OnDestroy()
		{
			this.m_backButton.onClick.RemoveAllListeners();
		}

		// Token: 0x060057E1 RID: 22497 RVA: 0x0004479C File Offset: 0x0004299C
		private bool CanAcceptInput()
		{
			return true;
		}

		// Token: 0x060057E2 RID: 22498 RVA: 0x001E49E0 File Offset: 0x001E2BE0
		private void Update()
		{
			if (!this.Brain)
			{
				return;
			}
			if (this.CanAcceptInput() && UIManager.EventSystem)
			{
				if (Input.GetKeyDown(KeyCode.Tab))
				{
					Selectable selectable = null;
					if (UIManager.EventSystem.currentSelectedGameObject != null)
					{
						selectable = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? UIManager.EventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() : UIManager.EventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown());
					}
					if (selectable)
					{
						UIManager.EventSystem.SetSelectedGameObject(selectable.gameObject, new BaseEventData(UIManager.EventSystem));
					}
				}
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					bool flag = false;
					if (UIManager.EventSystem.currentSelectedGameObject)
					{
						Button component = UIManager.EventSystem.currentSelectedGameObject.GetComponent<Button>();
						if (component)
						{
							component.onClick.Invoke();
							flag = true;
						}
					}
					if (!flag)
					{
						LoginStage stage = this.GetStage(this.Stage);
						if (stage != null)
						{
							stage.EnterPressed();
						}
						AudioClip defaultClickClip = GlobalSettings.Values.Audio.DefaultClickClip;
						ClientGameManager.UIManager.PlayClip(defaultClickClip, new float?(1f), new float?(GlobalSettings.Values.Audio.DefaultClickVolume));
					}
				}
			}
			LoginStageType stage2 = this.Stage;
			if (stage2 - LoginStageType.CharacterSelection <= 1)
			{
				if (Input.anyKeyDown || Input.mouseScrollDelta != Vector2.zero || Input.mousePosition != this.m_mousePosLastFrame)
				{
					this.m_timeIdle = 0f;
				}
				else
				{
					this.m_timeIdle += Time.deltaTime;
				}
				if (this.m_timeIdle > 1200f)
				{
					this.Stage = LoginStageType.Authenticate;
				}
			}
			else
			{
				this.m_timeIdle = 0f;
			}
			this.m_mousePosLastFrame = Input.mousePosition;
		}

		// Token: 0x060057E3 RID: 22499 RVA: 0x001E4BC0 File Offset: 0x001E2DC0
		public void RefreshStage()
		{
			LoginStage stage = this.GetStage(this.Stage);
			if (stage != null)
			{
				stage.StageRefresh();
			}
		}

		// Token: 0x060057E4 RID: 22500 RVA: 0x0007A9A8 File Offset: 0x00078BA8
		public void SetStage(LoginStageType stage)
		{
			this.Stage = stage;
		}

		// Token: 0x060057E5 RID: 22501 RVA: 0x001E4BEC File Offset: 0x001E2DEC
		public void PreviousStage()
		{
			switch (this.Stage)
			{
			case LoginStageType.Authenticate:
			case LoginStageType.AuthenticateSteam:
				this.Stage = LoginStageType.StartScreen;
				return;
			case LoginStageType.CharacterSelection:
				this.Stage = (this.IsSteam ? LoginStageType.StartScreen : LoginStageType.Authenticate);
				return;
			case LoginStageType.CharacterCreation:
				this.Stage = LoginStageType.CharacterSelection;
				return;
			default:
				return;
			}
		}

		// Token: 0x060057E6 RID: 22502 RVA: 0x0007A9B1 File Offset: 0x00078BB1
		public void SetStatusText(string msg)
		{
			LoginStage stage = this.GetStage(this.Stage);
			if (stage == null)
			{
				return;
			}
			stage.StatusUpdate(msg);
		}

		// Token: 0x060057E7 RID: 22503 RVA: 0x0007A9CA File Offset: 0x00078BCA
		public void RaiseError(string err)
		{
			LoginStage stage = this.GetStage(this.Stage);
			if (stage == null)
			{
				return;
			}
			stage.StageError(err);
		}

		// Token: 0x060057E8 RID: 22504 RVA: 0x0007A9E3 File Offset: 0x00078BE3
		public void RaiseErrorCritical(string err)
		{
			LoginStage stage = this.GetStage(this.Stage);
			if (stage == null)
			{
				return;
			}
			stage.StageErrorCritical(err);
		}

		// Token: 0x060057E9 RID: 22505 RVA: 0x001E4C3C File Offset: 0x001E2E3C
		private void SetBackText(LoginStageType stageType)
		{
			string text = "";
			switch (stageType)
			{
			case LoginStageType.Authenticate:
			case LoginStageType.AuthenticateSteam:
				text = "Start Screen";
				break;
			case LoginStageType.CharacterSelection:
				text = (this.IsSteam ? "Start Screen" : "Authentication");
				break;
			case LoginStageType.CharacterCreation:
				text = "Character Selection";
				break;
			}
			this.m_backButton.text = (string.IsNullOrEmpty(text) ? text : ("Back to " + text));
		}

		// Token: 0x060057EA RID: 22506 RVA: 0x001E4CB0 File Offset: 0x001E2EB0
		public void LoginComplete()
		{
			if (this.Stage == LoginStageType.Authenticate)
			{
				LoginStageAuthenticateLegacy loginStageAuthenticateLegacy = this.m_authenticate as LoginStageAuthenticateLegacy;
				if (loginStageAuthenticateLegacy != null)
				{
					loginStageAuthenticateLegacy.SaveUsername();
				}
			}
			this.Stage = LoginStageType.CharacterSelection;
		}

		// Token: 0x060057EB RID: 22507 RVA: 0x0007A9FC File Offset: 0x00078BFC
		public void LoginFailed()
		{
			this.ResetSession();
			this.Stage = (this.IsSteam ? LoginStageType.StartScreen : LoginStageType.Authenticate);
		}

		// Token: 0x060057EC RID: 22508 RVA: 0x0007AA16 File Offset: 0x00078C16
		private void ResetSession()
		{
			SessionData.Clear();
		}

		// Token: 0x060057ED RID: 22509 RVA: 0x0007AA1D File Offset: 0x00078C1D
		public void ToggleBackButton(bool enabled)
		{
			if (enabled)
			{
				if (!this.m_backButtonWindow.Visible)
				{
					this.m_backButtonWindow.Show(false);
					return;
				}
			}
			else if (this.m_backButtonWindow.Visible)
			{
				this.m_backButtonWindow.Hide(false);
			}
		}

		// Token: 0x04004D77 RID: 19831
		public static LoginController Instance;

		// Token: 0x04004D79 RID: 19833
		[SerializeField]
		private LoginStage m_startScreen;

		// Token: 0x04004D7A RID: 19834
		[SerializeField]
		private LoginStage m_authenticate;

		// Token: 0x04004D7B RID: 19835
		[SerializeField]
		private LoginStage m_authenticateSteam;

		// Token: 0x04004D7C RID: 19836
		[SerializeField]
		private LoginStage m_characterSelection;

		// Token: 0x04004D7D RID: 19837
		[SerializeField]
		private LoginStage m_characterCreation;

		// Token: 0x04004D7E RID: 19838
		[SerializeField]
		private UIWindow m_backButtonWindow;

		// Token: 0x04004D7F RID: 19839
		[SerializeField]
		private SolButton m_backButton;

		// Token: 0x04004D80 RID: 19840
		[SerializeField]
		protected TextMeshProUGUI m_statusText;

		// Token: 0x04004D81 RID: 19841
		private Dictionary<LoginStageType, LoginStage> m_stageDict;

		// Token: 0x04004D82 RID: 19842
		private LoginStageCharacterSelection m_selectionStage;

		// Token: 0x04004D83 RID: 19843
		private float m_timeIdle;

		// Token: 0x04004D84 RID: 19844
		private Vector3 m_mousePosLastFrame;

		// Token: 0x04004D8B RID: 19851
		private LoginStageType m_stage;
	}
}
