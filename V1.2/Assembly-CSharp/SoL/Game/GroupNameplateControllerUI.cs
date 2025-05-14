using System;
using Cysharp.Text;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Game.Targeting;
using SoL.Game.UI;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x02000599 RID: 1433
	public class GroupNameplateControllerUI : MonoBehaviour, IContextMenu, IInteractiveBase, IPointerDownHandler, IEventSystemHandler
	{
		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x06002CA1 RID: 11425 RVA: 0x0005EF2C File Offset: 0x0005D12C
		public RectTransform RectTransform
		{
			get
			{
				return this.m_rectTransform;
			}
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x06002CA2 RID: 11426 RVA: 0x0005EF34 File Offset: 0x0005D134
		public NameplateControllerUI Nameplate
		{
			get
			{
				return this.m_nameplate;
			}
		}

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06002CA3 RID: 11427 RVA: 0x0005EF3C File Offset: 0x0005D13C
		// (set) Token: 0x06002CA4 RID: 11428 RVA: 0x0005EF44 File Offset: 0x0005D144
		public GroupWindowUI ParentUI { get; set; }

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06002CA5 RID: 11429 RVA: 0x0005EF4D File Offset: 0x0005D14D
		// (set) Token: 0x06002CA6 RID: 11430 RVA: 0x0014A980 File Offset: 0x00148B80
		public GroupMember Member
		{
			get
			{
				return this.m_member;
			}
			set
			{
				if (this.m_member == value)
				{
					return;
				}
				if (this.m_member != null)
				{
					this.m_member.MemberUpdated -= this.MemberUpdated;
				}
				this.m_member = value;
				if (this.m_member != null)
				{
					this.m_member.MemberUpdated += this.MemberUpdated;
				}
			}
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x0005EF55 File Offset: 0x0005D155
		private void Awake()
		{
			if (this.m_nameplate && this.m_nameplate.Draggable)
			{
				this.m_nameplate.Draggable.PreventDragging = true;
			}
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x0014A9DC File Offset: 0x00148BDC
		private void Start()
		{
			if (this.m_nameplate)
			{
				Color color;
				if (this.m_nameplate.TryGetImageColor(out color))
				{
					this.m_defaultImageColor = color;
					if (this.m_overlayFrame)
					{
						this.m_overlayFrame.color = color;
					}
				}
				this.m_nameplate.PlayerHealthStateUpdated += this.NameplateOnPlayerHealthStateUpdated;
			}
			if (this.m_selectedHighlight)
			{
				Color kDefensiveColor = NameplateControllerUI.kDefensiveColor;
				kDefensiveColor.a = 0.5f;
				this.m_selectedHighlight.color = kDefensiveColor;
			}
		}

		// Token: 0x06002CA9 RID: 11433 RVA: 0x0014AA68 File Offset: 0x00148C68
		private void OnDestroy()
		{
			if (this.Member != null)
			{
				this.Member.MemberUpdated -= this.MemberUpdated;
			}
			if (this.m_nameplate)
			{
				this.m_nameplate.PlayerHealthStateUpdated -= this.NameplateOnPlayerHealthStateUpdated;
			}
		}

		// Token: 0x06002CAA RID: 11434 RVA: 0x0005EF87 File Offset: 0x0005D187
		public void AssignMember(GroupMember member)
		{
			this.Member = member;
			this.MemberUpdated();
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06002CAB RID: 11435 RVA: 0x0005EF96 File Offset: 0x0005D196
		public bool IsEmpty
		{
			get
			{
				return this.Member == null;
			}
		}

		// Token: 0x06002CAC RID: 11436 RVA: 0x0005EFA1 File Offset: 0x0005D1A1
		public void SetSiblingIndex(int index)
		{
			this.m_rectTransform.SetSiblingIndex(index);
		}

		// Token: 0x06002CAD RID: 11437 RVA: 0x0014AAB8 File Offset: 0x00148CB8
		private void MemberUpdated()
		{
			if (this.Member == null)
			{
				this.m_nameplate.Init(null);
				if (this.m_window.Visible)
				{
					this.m_window.Hide(true);
				}
			}
			else if (this.Member.Entity == null || this.Member.InDifferentZone)
			{
				this.m_overlayPanel.gameObject.SetActive(true);
				this.m_overlayName.text = this.Member.Name;
				this.m_overlayLocation.ZStringSetText(this.Member.GetStatusString());
				this.m_overlayLevel.ZStringSetText(this.Member.GetLevelString());
				Color color;
				Sprite roleIcon = this.Member.GetRoleIcon(out color);
				this.m_overlayRoleIcon.overrideSprite = roleIcon;
				this.m_overlayRoleIcon.color = color;
				this.m_overlayRoleIcon.enabled = (roleIcon != null);
				this.m_overlayRoleLevelTooltip.Text = this.Member.GetRoleLevelTooltipText();
				this.m_nameplate.DisableRoleIndicator();
				this.m_nameplate.gameObject.SetActive(false);
				if (!this.m_window.Visible)
				{
					this.m_window.Show(false);
				}
			}
			else
			{
				this.m_overlayPanel.gameObject.SetActive(false);
				this.m_nameplate.gameObject.SetActive(true);
				this.m_nameplate.Init(this.Member.Entity.Targetable);
				if (!this.m_window.Visible)
				{
					this.m_window.Show(false);
				}
			}
			if (this.ParentUI != null)
			{
				this.ParentUI.RefreshLabels();
			}
			if (this.m_leyLinkIndicator)
			{
				this.m_leyLinkIndicator.RefreshMember(this.Member);
			}
		}

		// Token: 0x06002CAE RID: 11438 RVA: 0x0005EFAF File Offset: 0x0005D1AF
		public void SetLeader(bool isLeader, bool isRaidLeader)
		{
			this.m_nameplate.ToggleLeaderIcon(isLeader, isRaidLeader);
			this.m_overlayLeaderIcon.Refresh(isLeader, isRaidLeader);
		}

		// Token: 0x06002CAF RID: 11439 RVA: 0x0005EFCB File Offset: 0x0005D1CB
		private void NameplateOnPlayerHealthStateUpdated(HealthState obj)
		{
			this.RefreshColoring();
		}

		// Token: 0x06002CB0 RID: 11440 RVA: 0x0005EFCB File Offset: 0x0005D1CB
		public void RefreshSelected()
		{
			this.RefreshColoring();
		}

		// Token: 0x06002CB1 RID: 11441 RVA: 0x0014AC84 File Offset: 0x00148E84
		private void RefreshColoring()
		{
			bool flag = this.Member != null && this.Member.Entity;
			bool flag2 = flag && LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController && LocalPlayer.GameEntity.TargetController.DefensiveTarget != null && this.Member.Entity == LocalPlayer.GameEntity.TargetController.DefensiveTarget;
			bool flag3 = false;
			bool flag4 = false;
			if (flag && this.Member.Entity.VitalsReplicator)
			{
				flag3 = (this.Member.Entity.VitalsReplicator.CurrentHealthState.Value == HealthState.Dead);
				flag4 = (this.Member.Entity.VitalsReplicator.CurrentHealthState.Value == HealthState.Unconscious);
			}
			Color color = this.m_defaultImageColor;
			if (flag3)
			{
				color = Color.black;
			}
			else if (flag4 && flag2)
			{
				color = NameplateControllerUI.kDefensiveColor + NameplateControllerUI.kOffensiveColor;
			}
			else if (flag4)
			{
				color = NameplateControllerUI.kOffensiveColor;
			}
			else if (flag2)
			{
				color = NameplateControllerUI.kDefensiveColor;
			}
			if (this.m_selectedHighlight)
			{
				this.m_selectedHighlight.color = color;
				this.m_selectedHighlight.enabled = flag2;
			}
			if (this.m_nameplate && this.m_nameplate.ImagesToColor != null)
			{
				for (int i = 0; i < this.m_nameplate.ImagesToColor.Length; i++)
				{
					if (this.m_nameplate.ImagesToColor[i])
					{
						this.m_nameplate.ImagesToColor[i].color = color;
					}
				}
			}
		}

		// Token: 0x06002CB2 RID: 11442 RVA: 0x0005EFD3 File Offset: 0x0005D1D3
		public string FillActionsGetTitle()
		{
			if (this.m_member == null)
			{
				return null;
			}
			InteractivePlayer.FillActionsForEntityName(this.Member.Name, this.Member.Entity);
			return this.Member.Name;
		}

		// Token: 0x06002CB3 RID: 11443 RVA: 0x0014AE20 File Offset: 0x00149020
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left && this.Member != null && this.Member.Entity != null)
			{
				LocalPlayer.GameEntity.TargetController.SetTarget(TargetType.Defensive, this.Member.Entity.Targetable);
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06002CB4 RID: 11444 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002C5E RID: 11358
		[SerializeField]
		private NameplateControllerUI m_nameplate;

		// Token: 0x04002C5F RID: 11359
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04002C60 RID: 11360
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04002C61 RID: 11361
		[SerializeField]
		private GameObject m_overlayPanel;

		// Token: 0x04002C62 RID: 11362
		[SerializeField]
		private TextMeshProUGUI m_overlayName;

		// Token: 0x04002C63 RID: 11363
		[SerializeField]
		private TextMeshProUGUI m_overlayLocation;

		// Token: 0x04002C64 RID: 11364
		[SerializeField]
		private TextMeshProUGUI m_overlayLevel;

		// Token: 0x04002C65 RID: 11365
		[SerializeField]
		private Image m_overlayRoleIcon;

		// Token: 0x04002C66 RID: 11366
		[SerializeField]
		private TextTooltipTrigger m_overlayRoleLevelTooltip;

		// Token: 0x04002C67 RID: 11367
		[SerializeField]
		private LeaderIcon m_overlayLeaderIcon;

		// Token: 0x04002C68 RID: 11368
		[SerializeField]
		private Image m_overlayFrame;

		// Token: 0x04002C69 RID: 11369
		[SerializeField]
		private Image m_selectedHighlight;

		// Token: 0x04002C6A RID: 11370
		[FormerlySerializedAs("m_emberRingIndicator")]
		[SerializeField]
		private LeyLinkIndicator m_leyLinkIndicator;

		// Token: 0x04002C6B RID: 11371
		private Color m_defaultImageColor = NameplateControllerUI.kGroupColor;

		// Token: 0x04002C6D RID: 11373
		private GroupMember m_member;
	}
}
