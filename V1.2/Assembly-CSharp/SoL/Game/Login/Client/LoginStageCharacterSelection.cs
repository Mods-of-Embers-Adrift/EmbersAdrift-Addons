using System;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.Subscription;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B37 RID: 2871
	public class LoginStageCharacterSelection : LoginStage
	{
		// Token: 0x170014B8 RID: 5304
		// (get) Token: 0x06005837 RID: 22583 RVA: 0x001E55E4 File Offset: 0x001E37E4
		public bool AllowSelectionChange
		{
			get
			{
				if (this.m_centeredEnterWorld)
				{
					return this.m_centeredEnterWorld.interactable && this.m_centeredEnterWorld.gameObject.activeInHierarchy;
				}
				return this.m_enterWorld && this.m_enterWorld.interactable && this.m_enterWorld.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x06005838 RID: 22584 RVA: 0x001E564C File Offset: 0x001E384C
		protected override void Awake()
		{
			base.Awake();
			base.StatusUpdate(string.Empty);
			this.m_create.onClick.AddListener(new UnityAction(this.OnCreatePressed));
			if (this.m_centeredEnterWorld)
			{
				this.m_centeredEnterWorld.onClick.AddListener(new UnityAction(this.OnEnterWorldPressed));
				this.m_enterWorldPanel.SetActive(false);
				return;
			}
			if (this.m_enterWorld)
			{
				this.m_enterWorld.onClick.AddListener(new UnityAction(this.OnEnterWorldPressed));
				this.m_enterWorldRect = (this.m_enterWorld.gameObject.transform as RectTransform);
				this.m_enterWorld.gameObject.SetActive(false);
				this.m_originalEnterWorldParent = this.m_enterWorld.gameObject.transform;
			}
		}

		// Token: 0x06005839 RID: 22585 RVA: 0x001E5728 File Offset: 0x001E3928
		private void OnDestroy()
		{
			this.m_create.onClick.RemoveAllListeners();
			if (this.m_centeredEnterWorld)
			{
				this.m_centeredEnterWorld.onClick.RemoveAllListeners();
				return;
			}
			if (this.m_enterWorld)
			{
				this.m_enterWorld.onClick.RemoveAllListeners();
			}
		}

		// Token: 0x0600583A RID: 22586 RVA: 0x001E5780 File Offset: 0x001E3980
		public override void StageEnter()
		{
			base.StageEnter();
			this.RefreshButtons();
			if (!GameObject.Find("UserDpsViewer"))
			{
				Debug.Log("[UserDpsViewer] Launching overlay from character selection screen.");
				UserDpsTracker.Launch();
				UserDpsViewer.Launch();
			}
			if (!this.m_backButtonWindow.Visible)
			{
				this.m_backButtonWindow.Show(false);
			}
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.ResetRaid();
			}
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.ResetGroup();
			}
			UIManager.EventSystem.SetSelectedGameObject(null);
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x0007ADC6 File Offset: 0x00078FC6
		public override void StageExit()
		{
			base.StageExit();
			if (ClientGameManager.UIManager.SelectPortraitWindow.Visible)
			{
				ClientGameManager.UIManager.SelectPortraitWindow.HideWindow();
			}
		}

		// Token: 0x0600583C RID: 22588 RVA: 0x0007ADEE File Offset: 0x00078FEE
		public override void StageRefresh()
		{
			base.StageRefresh();
			this.RefreshButtons();
		}

		// Token: 0x0600583D RID: 22589 RVA: 0x0007ADFC File Offset: 0x00078FFC
		public override void StageError(string err)
		{
			base.StageError(err);
			this.RefreshButtons();
		}

		// Token: 0x0600583E RID: 22590 RVA: 0x001E5810 File Offset: 0x001E3A10
		public override void EnterPressed()
		{
			base.EnterPressed();
			if (this.m_centeredEnterWorld)
			{
				if (this.m_centeredEnterWorld.interactable)
				{
					this.OnEnterWorldPressed();
					return;
				}
			}
			else if (this.m_enterWorld && this.m_enterWorld.interactable)
			{
				this.OnEnterWorldPressed();
			}
		}

		// Token: 0x0600583F RID: 22591 RVA: 0x0007AE0B File Offset: 0x0007900B
		private void OnCreatePressed()
		{
			this.m_controller.SetStage(LoginStageType.CharacterCreation);
		}

		// Token: 0x06005840 RID: 22592 RVA: 0x001E5864 File Offset: 0x001E3A64
		private void OnEnterWorldPressed()
		{
			if (SessionData.SelectedCharacter == null)
			{
				return;
			}
			Debug.Log("Enter world!");
			this.m_create.interactable = false;
			if (this.m_centeredEnterWorld)
			{
				this.m_centeredEnterWorld.interactable = false;
			}
			if (this.m_enterWorld)
			{
				this.m_enterWorld.interactable = false;
			}
			LoginApiManager.PerformZoneCheckForSelection((ZoneId)SessionData.SelectedCharacter.Location.ZoneId, SessionData.SelectedCharacter, new Action<bool>(this.PlayCallback));
		}

		// Token: 0x06005841 RID: 22593 RVA: 0x0007AE19 File Offset: 0x00079019
		private void PlayCallback(bool success)
		{
			if (success)
			{
				SolServerConnectionManager.Instance.SwitchToSocial(SessionData.World);
				return;
			}
			this.StageError(LoginApiManager.LastZoneCheckError);
		}

		// Token: 0x06005842 RID: 22594 RVA: 0x001E58E8 File Offset: 0x001E3AE8
		public void RefreshButtons()
		{
			bool flag = this.IsActive(SessionData.SelectedCharacter);
			if (this.m_centeredEnterWorld)
			{
				this.m_centeredEnterWorld.interactable = flag;
				if (flag)
				{
					this.m_enterWorldLabel.SetText(SessionData.SelectedCharacter.Name);
				}
				this.m_enterWorldPanel.SetActive(flag);
			}
			else
			{
				this.m_enterWorld.interactable = flag;
				this.m_enterWorld.gameObject.SetActive(flag);
			}
			this.m_create.interactable = (SessionData.Characters != null && SessionData.Characters.Length < SessionData.MaxCharacters);
			this.m_activateSubscription.RefreshMode();
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x0007AE39 File Offset: 0x00079039
		public void PlayCharacterSuccess(int port)
		{
			PlayerPrefs.SetString("LastPlayedCharacter", SessionData.SelectedCharacter.Name);
			GameManager.SceneCompositionManager.LoadZoneId((ZoneId)SessionData.SelectedCharacter.Location.ZoneId);
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x001E598C File Offset: 0x001E3B8C
		public void ParentEnterWorldButton(GameObject buttonParent, CharacterRecord record)
		{
			if (this.m_centeredEnterWorld == null)
			{
				this.m_enterWorldRect.SetParent((buttonParent == null) ? this.m_originalEnterWorldParent : buttonParent.transform, true);
				this.m_enterWorldRect.anchoredPosition = Vector2.zero;
				this.m_enterWorld.gameObject.SetActive(buttonParent != null && this.IsActive(record));
			}
			this.RefreshButtons();
		}

		// Token: 0x06005845 RID: 22597 RVA: 0x001E5A04 File Offset: 0x001E3C04
		private bool IsActive(CharacterRecord record)
		{
			return record != null && (record.RequiresRenaming == null || !record.RequiresRenaming.Value);
		}

		// Token: 0x04004DAB RID: 19883
		[SerializeField]
		private Button m_create;

		// Token: 0x04004DAC RID: 19884
		[SerializeField]
		private SolButton m_enterWorld;

		// Token: 0x04004DAD RID: 19885
		[SerializeField]
		private ActivateSubscriptionButton m_activateSubscription;

		// Token: 0x04004DAE RID: 19886
		[SerializeField]
		private UIWindow m_backButtonWindow;

		// Token: 0x04004DAF RID: 19887
		[SerializeField]
		private SolButton m_centeredEnterWorld;

		// Token: 0x04004DB0 RID: 19888
		[SerializeField]
		private GameObject m_enterWorldPanel;

		// Token: 0x04004DB1 RID: 19889
		[SerializeField]
		private TextMeshProUGUI m_enterWorldLabel;

		// Token: 0x04004DB2 RID: 19890
		private RectTransform m_enterWorldRect;

		// Token: 0x04004DB3 RID: 19891
		private Transform m_originalEnterWorldParent;
	}
}
