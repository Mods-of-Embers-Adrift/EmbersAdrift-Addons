using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008CF RID: 2255
	public class RaidWindowUI : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x060041ED RID: 16877 RVA: 0x0006C86A File Offset: 0x0006AA6A
		private void Awake()
		{
			UIManager.RaidWindowUI = this;
		}

		// Token: 0x060041EE RID: 16878 RVA: 0x0006C872 File Offset: 0x0006AA72
		private void Start()
		{
			this.SubscribeToEvents();
		}

		// Token: 0x060041EF RID: 16879 RVA: 0x0006C87A File Offset: 0x0006AA7A
		private void OnDestroy()
		{
			this.UnsubscribeToEvents();
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x00191278 File Offset: 0x0018F478
		private void SubscribeToEvents()
		{
			ClientGameManager.SocialManager.RaidUpdated += this.RefreshRaidGroups;
			if (this.m_leaveRaidButton)
			{
				this.m_leaveRaidButton.onClick.AddListener(new UnityAction(this.OnLeaveRaidButtonClicked));
			}
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x001912C4 File Offset: 0x0018F4C4
		private void UnsubscribeToEvents()
		{
			ClientGameManager.SocialManager.RaidUpdated -= this.RefreshRaidGroups;
			if (this.m_leaveRaidButton)
			{
				this.m_leaveRaidButton.onClick.RemoveListener(new UnityAction(this.OnLeaveRaidButtonClicked));
			}
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x00191310 File Offset: 0x0018F510
		private void OnLeaveRaidButtonClicked()
		{
			if (ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.IsLeader && ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsInRaid)
			{
				new SolServerCommand(CommandClass.group, CommandType.raidleave)
				{
					Args = 
					{
						{
							"Receiver",
							null
						}
					}
				}.Send();
			}
		}

		// Token: 0x060041F3 RID: 16883 RVA: 0x0006C882 File Offset: 0x0006AA82
		public void InitLocalPlayer()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.DefensiveTargetChanged += this.TargetControllerOnDefensiveTargetChanged;
			}
		}

		// Token: 0x060041F4 RID: 16884 RVA: 0x0006C8BC File Offset: 0x0006AABC
		public void UnsetLocalPlayer()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.DefensiveTargetChanged -= this.TargetControllerOnDefensiveTargetChanged;
			}
		}

		// Token: 0x060041F5 RID: 16885 RVA: 0x0019137C File Offset: 0x0018F57C
		public void RefreshLeaveRaidButton()
		{
			if (this.m_leaveRaidButton)
			{
				this.m_leaveRaidButton.interactable = (ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsInRaid && ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsLeader);
			}
		}

		// Token: 0x060041F6 RID: 16886 RVA: 0x001913D4 File Offset: 0x0018F5D4
		private void RefreshRaidGroups()
		{
			List<RaidGroup> fromPool = StaticListPool<RaidGroup>.GetFromPool();
			if (ClientGameManager.SocialManager && ClientGameManager.SocialManager.RaidGroups != null)
			{
				UniqueId value = (ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped) ? ClientGameManager.GroupManager.GroupId : UniqueId.Empty;
				foreach (RaidGroup raidGroup in ClientGameManager.SocialManager.RaidGroups)
				{
					if (raidGroup != null && !string.IsNullOrEmpty(raidGroup.GroupId) && raidGroup.GroupId != value)
					{
						fromPool.Add(raidGroup);
					}
				}
			}
			this.RefreshLeaveRaidButton();
			for (int j = 0; j < this.m_raidGroupUis.Length; j++)
			{
				if (this.m_raidGroupUis[j])
				{
					RaidGroup raidGroup2 = (j < fromPool.Count) ? fromPool[j] : null;
					this.m_raidGroupUis[j].RefreshRaidGroup(raidGroup2);
				}
			}
			this.m_activeRaidGroups = fromPool.Count;
			StaticListPool<RaidGroup>.ReturnToPool(fromPool);
			this.RefreshWindowSize();
			if (ClientGameManager.SocialManager.IsInRaid)
			{
				if (this.m_draggable && !this.m_draggable.Visible)
				{
					this.m_draggable.Show(false);
					return;
				}
			}
			else if (this.m_draggable && this.m_draggable.Visible)
			{
				this.m_draggable.Hide(false);
			}
		}

		// Token: 0x060041F7 RID: 16887 RVA: 0x0006C8F6 File Offset: 0x0006AAF6
		private void TargetControllerOnDefensiveTargetChanged(GameEntity obj)
		{
			this.RefreshSelected();
		}

		// Token: 0x060041F8 RID: 16888 RVA: 0x00191540 File Offset: 0x0018F740
		private void RefreshSelected()
		{
			if (this.m_raidGroupUis != null)
			{
				for (int i = 0; i < this.m_raidGroupUis.Length; i++)
				{
					if (this.m_raidGroupUis[i])
					{
						this.m_raidGroupUis[i].RefreshSelected();
					}
				}
			}
		}

		// Token: 0x060041F9 RID: 16889 RVA: 0x00191584 File Offset: 0x0018F784
		private int GetMaxActiveNameplateCount()
		{
			int num = 0;
			if (this.m_raidGroupUis != null)
			{
				for (int i = 0; i < this.m_raidGroupUis.Length; i++)
				{
					if (this.m_raidGroupUis[i] != null)
					{
						int activeNameplateCount = this.m_raidGroupUis[i].GetActiveNameplateCount();
						if (activeNameplateCount > num)
						{
							num = activeNameplateCount;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060041FA RID: 16890 RVA: 0x001915D4 File Offset: 0x0018F7D4
		private void RefreshWindowSize()
		{
			if (!this.m_raidGroupParent || !this.m_nameplateSample || !this.m_rectTransform || !this.m_raidGroupSample)
			{
				return;
			}
			float y = this.m_raidGroupParent.localScale.y;
			int num = Mathf.Max(1, this.m_activeRaidGroups);
			float x = this.m_raidGroupSample.rect.width * y * (float)num;
			float num2 = -1f * this.m_raidGroupParent.anchoredPosition.y;
			float height = this.m_nameplateSample.rect.height;
			int num3 = Application.isPlaying ? this.GetMaxActiveNameplateCount() : 6;
			float y2 = num2 + (float)num3 * height * y + (float)((num3 - 1) * this.m_spacing) * y;
			this.m_rectTransform.sizeDelta = new Vector2(x, y2);
		}

		// Token: 0x17000F06 RID: 3846
		// (get) Token: 0x060041FB RID: 16891 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060041FC RID: 16892 RVA: 0x0006C8FE File Offset: 0x0006AAFE
		string IContextMenu.FillActionsGetTitle()
		{
			if (!this.m_draggable)
			{
				return null;
			}
			return this.m_draggable.FillActionsGetTitle();
		}

		// Token: 0x060041FE RID: 16894 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F1C RID: 16156
		[SerializeField]
		private RaidGroupUI[] m_raidGroupUis;

		// Token: 0x04003F1D RID: 16157
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04003F1E RID: 16158
		[SerializeField]
		private RectTransform m_raidGroupParent;

		// Token: 0x04003F1F RID: 16159
		[SerializeField]
		private RectTransform m_raidGroupSample;

		// Token: 0x04003F20 RID: 16160
		[SerializeField]
		private RectTransform m_nameplateSample;

		// Token: 0x04003F21 RID: 16161
		[SerializeField]
		private DraggableUIWindow m_draggable;

		// Token: 0x04003F22 RID: 16162
		[SerializeField]
		private SolButton m_leaveRaidButton;

		// Token: 0x04003F23 RID: 16163
		[SerializeField]
		private int m_spacing = 6;

		// Token: 0x04003F24 RID: 16164
		private int m_activeRaidGroups;
	}
}
