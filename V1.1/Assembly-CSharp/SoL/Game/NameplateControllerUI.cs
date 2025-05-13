using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Nameplates;
using SoL.Game.Objects;
using SoL.Game.Settings;
using SoL.Game.Targeting;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x0200059D RID: 1437
	public class NameplateControllerUI : MonoBehaviour, IContextMenu, IInteractiveBase, IPointerDownHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x06002CD0 RID: 11472 RVA: 0x0014B5DC File Offset: 0x001497DC
		private bool HasEffectsPanel()
		{
			NameplateControllerUI.NameplateType type = this.m_type;
			return type - NameplateControllerUI.NameplateType.Offensive <= 1;
		}

		// Token: 0x06002CD1 RID: 11473 RVA: 0x0014B5FC File Offset: 0x001497FC
		private bool SubscribeToPortraitChange()
		{
			switch (this.m_type)
			{
			case NameplateControllerUI.NameplateType.Player:
			case NameplateControllerUI.NameplateType.Defensive:
			case NameplateControllerUI.NameplateType.Group:
			case NameplateControllerUI.NameplateType.TargetOfTarget:
				return true;
			}
			return false;
		}

		// Token: 0x06002CD2 RID: 11474 RVA: 0x0014B638 File Offset: 0x00149838
		private bool SubscribeToTextChanges()
		{
			NameplateControllerUI.NameplateType type = this.m_type;
			return type == NameplateControllerUI.NameplateType.Player || type - NameplateControllerUI.NameplateType.Defensive <= 4;
		}

		// Token: 0x06002CD3 RID: 11475 RVA: 0x0014B65C File Offset: 0x0014985C
		private bool ShowStaminaBar()
		{
			NameplateControllerUI.NameplateType type = this.m_type;
			return type != NameplateControllerUI.NameplateType.Offensive && type != NameplateControllerUI.NameplateType.TargetOfTarget;
		}

		// Token: 0x06002CD4 RID: 11476 RVA: 0x0014B67C File Offset: 0x0014987C
		private bool ShowLevel()
		{
			NameplateControllerUI.NameplateType type = this.m_type;
			return type - NameplateControllerUI.NameplateType.Defensive <= 1 || type == NameplateControllerUI.NameplateType.Raid;
		}

		// Token: 0x06002CD5 RID: 11477 RVA: 0x0005F17F File Offset: 0x0005D37F
		private bool HasPositionLock()
		{
			NameplateControllerUI.NameplateType type = this.m_type;
			return false;
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x0014B6A0 File Offset: 0x001498A0
		internal bool TryGetImageColor(out Color color)
		{
			color = Color.white;
			switch (this.m_type)
			{
			case NameplateControllerUI.NameplateType.Offensive:
				color = NameplateControllerUI.kOffensiveColor;
				return true;
			case NameplateControllerUI.NameplateType.Defensive:
				color = NameplateControllerUI.kDefensiveColor;
				return true;
			case NameplateControllerUI.NameplateType.Group:
				color = NameplateControllerUI.kGroupColor;
				return true;
			case NameplateControllerUI.NameplateType.Raid:
				color = UIManager.RaidColor;
				return true;
			}
			return false;
		}

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x06002CD7 RID: 11479 RVA: 0x0014B718 File Offset: 0x00149918
		// (remove) Token: 0x06002CD8 RID: 11480 RVA: 0x0014B750 File Offset: 0x00149950
		public event Action<HealthState> PlayerHealthStateUpdated;

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x06002CD9 RID: 11481 RVA: 0x0005F189 File Offset: 0x0005D389
		private bool m_showGroupController
		{
			get
			{
				return this.m_type == NameplateControllerUI.NameplateType.Group || this.m_type == NameplateControllerUI.NameplateType.Raid;
			}
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06002CDA RID: 11482 RVA: 0x0005F19F File Offset: 0x0005D39F
		// (set) Token: 0x06002CDB RID: 11483 RVA: 0x0005F1A7 File Offset: 0x0005D3A7
		public bool ProcessUpdates { private get; set; } = true;

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x06002CDC RID: 11484 RVA: 0x0005F1B0 File Offset: 0x0005D3B0
		public bool IsEmpty
		{
			get
			{
				return this.Targetable == null;
			}
		}

		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06002CDD RID: 11485 RVA: 0x0005F1BB File Offset: 0x0005D3BB
		public DraggableUIWindow Draggable
		{
			get
			{
				return this.m_draggableWindow;
			}
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06002CDE RID: 11486 RVA: 0x0005F1C3 File Offset: 0x0005D3C3
		public TextMeshProUGUI NameText
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x06002CDF RID: 11487 RVA: 0x0005F1CB File Offset: 0x0005D3CB
		public TextMeshProUGUI TitleText
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x06002CE0 RID: 11488 RVA: 0x0005F1D3 File Offset: 0x0005D3D3
		public TextMeshProUGUI GuildText
		{
			get
			{
				return this.m_guild;
			}
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x06002CE1 RID: 11489 RVA: 0x0005F1DB File Offset: 0x0005D3DB
		internal NameplateControllerUI.NameplateType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x06002CE2 RID: 11490 RVA: 0x0005F1E3 File Offset: 0x0005D3E3
		internal Image[] ImagesToColor
		{
			get
			{
				return this.m_imagesToColor;
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x06002CE3 RID: 11491 RVA: 0x0005F1EB File Offset: 0x0005D3EB
		// (set) Token: 0x06002CE4 RID: 11492 RVA: 0x0014B788 File Offset: 0x00149988
		public ITargetable Targetable
		{
			get
			{
				return this.m_targetable;
			}
			set
			{
				if (this.m_targetable == value && this.m_initialized)
				{
					return;
				}
				if (this.m_targetable != null)
				{
					this.Unsubscribe();
				}
				this.m_targetable = value;
				this.ResetLastValues();
				this.RefreshPortrait();
				this.RefreshHideStaminaBar();
				if (this.m_targetable != null)
				{
					this.UpdateStatBars(true, true);
					this.Subscribe();
					if (this.m_groupNameplateControllerUi == null && this.m_draggableWindow != null && this.m_draggableWindow.isActiveAndEnabled)
					{
						this.m_draggableWindow.Show(false);
					}
				}
				else if (this.m_groupNameplateControllerUi == null && this.m_draggableWindow != null && this.m_draggableWindow.isActiveAndEnabled)
				{
					this.m_draggableWindow.Hide(false);
				}
				this.m_initialized = true;
			}
		}

		// Token: 0x06002CE5 RID: 11493 RVA: 0x0014B858 File Offset: 0x00149A58
		private void Awake()
		{
			if (!this.m_initialized)
			{
				this.PlayerFlagsOnChanged(PlayerFlags.None);
				if (this.m_leaderIcon != null)
				{
					this.m_leaderIcon.Refresh(false, false);
				}
			}
			if (this.m_type == NameplateControllerUI.NameplateType.Player)
			{
				int i = 0;
				while (i < this.m_playerFlagsIcons.Length)
				{
					if (this.m_playerFlagsIcons[i].Flag == PlayerFlags.MissingBag)
					{
						DeathIconUI component = this.m_playerFlagsIcons[i].Panel.GetComponent<DeathIconUI>();
						if (component)
						{
							component.SetIsLocal(true);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			if (this.m_draggableWindow)
			{
				this.m_draggableWindow.PreventDragging = true;
			}
			if (this.m_statBarContentWindow)
			{
				this.m_statBarContentWindowDefaultHeight = this.m_statBarContentWindow.sizeDelta.y;
			}
			if (this.m_name)
			{
				this.m_nameMarginDefaultValue = this.m_name.margin;
			}
			if (this.m_debug)
			{
				this.m_debug.gameObject.SetActive(false);
			}
		}

		// Token: 0x06002CE6 RID: 11494 RVA: 0x0014B958 File Offset: 0x00149B58
		private void Start()
		{
			if (this.m_findOnStart)
			{
				ITargetable componentInParent = base.gameObject.GetComponentInParent<ITargetable>();
				this.Init(componentInParent);
			}
			this.RefreshHideStaminaBar();
			if (!this.ShowStaminaBar())
			{
				for (int i = 0; i < this.m_statBars.Length; i++)
				{
					if (this.m_statBars[i] != null && this.m_statBars[i].BarType == StatBarType.Health)
					{
						if (this.m_statBars[i].LeftText)
						{
							this.m_statBars[i].LeftText.verticalAlignment = VerticalAlignmentOptions.Bottom;
						}
						if (this.m_statBars[i].CenterText)
						{
							this.m_statBars[i].CenterText.verticalAlignment = VerticalAlignmentOptions.Bottom;
						}
						if (this.m_statBars[i].RightText)
						{
							this.m_statBars[i].RightText.verticalAlignment = VerticalAlignmentOptions.Bottom;
						}
					}
				}
			}
			if (!GameManager.IsServer && this.m_offensiveTargetIndicator != null)
			{
				this.m_offensiveTargetIndicator.SetActive(false);
			}
			if (!GameManager.IsServer && this.m_defensiveTargetIndicator != null)
			{
				this.m_defensiveTargetIndicator.SetActive(false);
			}
			if (this.m_type == NameplateControllerUI.NameplateType.Offensive && ClientGameManager.GroupManager != null)
			{
				ClientGameManager.GroupManager.GroupIdChanged += this.GroupManagerOnGroupIdChanged;
			}
			if (this.m_groupNameplateControllerUi != null && this.m_type != NameplateControllerUI.NameplateType.Raid)
			{
				this.m_type = NameplateControllerUI.NameplateType.Group;
			}
			if (this.m_backpackDirectionIndicatorUi != null)
			{
				this.m_backpackDirectionIndicatorUi.gameObject.SetActive(this.m_type == NameplateControllerUI.NameplateType.Player);
			}
			Options.GameOptions.ShowGelIndicators.Changed += this.ShowGelIndicatorsOnChanged;
			this.RefreshRoleIndicator();
			this.RefreshFlankingIndicator();
			Color color;
			if (this.TryGetImageColor(out color))
			{
				for (int j = 0; j < this.m_imagesToColor.Length; j++)
				{
					if (this.m_imagesToColor[j] != null)
					{
						this.m_imagesToColor[j].color = color;
					}
				}
			}
			this.ResetLastValues();
		}

		// Token: 0x06002CE7 RID: 11495 RVA: 0x0014BB68 File Offset: 0x00149D68
		private void OnDestroy()
		{
			this.Unsubscribe();
			if (this.m_type == NameplateControllerUI.NameplateType.Offensive && ClientGameManager.GroupManager != null)
			{
				ClientGameManager.GroupManager.GroupIdChanged -= this.GroupManagerOnGroupIdChanged;
			}
			Options.GameOptions.ShowGelIndicators.Changed -= this.ShowGelIndicatorsOnChanged;
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x0005F1F3 File Offset: 0x0005D3F3
		private void Update()
		{
			if (this.Targetable == null || !this.ProcessUpdates)
			{
				return;
			}
			this.UpdateStatBars(false, false);
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x06002CE9 RID: 11497 RVA: 0x0005F20E File Offset: 0x0005D40E
		// (set) Token: 0x06002CEA RID: 11498 RVA: 0x0014BBC0 File Offset: 0x00149DC0
		private bool HideStaminaBar
		{
			get
			{
				return this.m_hideStaminaBar;
			}
			set
			{
				if (this.m_hideStaminaBar == value)
				{
					return;
				}
				this.m_hideStaminaBar = value;
				if (this.m_statBarContentWindow)
				{
					if (this.HideStaminaBar)
					{
						Vector2 sizeDelta = this.m_statBarContentWindow.sizeDelta;
						sizeDelta.y = this.m_statBarContentWindowDefaultHeight * 0.5f;
						this.m_statBarContentWindow.sizeDelta = sizeDelta;
						Vector4 nameMarginDefaultValue = this.m_nameMarginDefaultValue;
						nameMarginDefaultValue.x *= 2f;
						this.m_name.margin = nameMarginDefaultValue;
					}
					else
					{
						Vector2 sizeDelta2 = this.m_statBarContentWindow.sizeDelta;
						sizeDelta2.y = this.m_statBarContentWindowDefaultHeight;
						this.m_statBarContentWindow.sizeDelta = sizeDelta2;
						Vector4 nameMarginDefaultValue2 = this.m_nameMarginDefaultValue;
						this.m_name.margin = nameMarginDefaultValue2;
					}
				}
				if (this.m_statBars != null)
				{
					bool active = !this.HideStaminaBar;
					for (int i = 0; i < this.m_statBars.Length; i++)
					{
						if (this.m_statBars[i] != null && this.m_statBars[i].BarType == StatBarType.Stamina)
						{
							this.m_statBars[i].gameObject.SetActive(active);
						}
					}
				}
			}
		}

		// Token: 0x06002CEB RID: 11499 RVA: 0x0005F216 File Offset: 0x0005D416
		private void RefreshHideStaminaBar()
		{
			this.HideStaminaBar = (!this.ShowStaminaBar() || (this.m_targetable != null && this.m_targetable.IsNpc));
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x0014BCE0 File Offset: 0x00149EE0
		private void Subscribe()
		{
			if (this.m_targetable == null)
			{
				return;
			}
			if (this.m_targetable.PlayerFlags != null)
			{
				this.PlayerFlagsOnChanged(this.m_targetable.PlayerFlags.Value);
				this.m_targetable.PlayerFlags.Changed += this.PlayerFlagsOnChanged;
			}
			if (this.SubscribeToTextChanges() && this.m_targetable.Name != null)
			{
				this.m_targetable.Name.Changed += this.NameOnChanged;
			}
			if (this.SubscribeToTextChanges() && this.m_targetable.Title != null)
			{
				this.m_targetable.Title.Changed += this.TitleOnChanged;
				if (this.m_targetable.Entity && this.m_targetable.Entity.CharacterData)
				{
					this.m_targetable.Entity.CharacterData.PresenceChanged += this.CharacterDataOnPresenceChanged;
				}
			}
			if (this.SubscribeToTextChanges() && this.m_targetable.Guild != null)
			{
				this.m_targetable.Guild.Changed += this.GuildOnChanged;
			}
			if (this.SubscribeToPortraitChange())
			{
				this.m_targetable.Entity.CharacterData.PortraitId.Changed += this.PortraitIdOnChanged;
			}
			if (this.m_targetable.Entity.Type == GameEntityType.Npc)
			{
				this.m_targetable.Entity.VitalsReplicator.CurrentHealthState.Changed += this.NpcCurrentHealthStateOnChanged;
				InteractiveNpc interactiveNpc = this.m_targetable.Entity.Interactive as InteractiveNpc;
				if (interactiveNpc != null)
				{
					interactiveNpc.Tagger.Changed += this.InteractiveTaggerChanged;
					interactiveNpc.GroupId.Changed += this.InteractiveGroupChanged;
					interactiveNpc.RaidId.Changed += this.InteractiveRaidChanged;
					interactiveNpc.NpcInteractiveFlags.Changed += this.NpcInteractiveFlagsOnChanged;
				}
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
				{
					((PlayerCharacterData)LocalPlayer.GameEntity.CharacterData).SyncedNearbyGroupInfo.Changed += this.SyncedNearbyGroupInfoOnChanged;
				}
			}
			if (this.m_targetable.Entity.Type == GameEntityType.Player)
			{
				this.m_targetable.Entity.VitalsReplicator.CurrentHealthState.Changed += this.PlayerCurrentHealthStateOnChanged;
				this.m_targetable.Entity.CharacterData.RoleChanged += this.RefreshRoleIndicator;
				this.m_targetable.Entity.CharacterData.PresenceChanged += this.RefreshAfkPanel;
				this.m_targetable.Entity.CharacterData.AdventuringLevelSync.Changed += this.RefreshLevelLabelEvent;
				if (this.m_type == NameplateControllerUI.NameplateType.Overhead)
				{
					this.m_targetable.Entity.CharacterData.LfgChanged += this.RefreshDisplayName;
				}
			}
			if (this.m_targetOfTargetNameplate && this.Targetable != null && this.Targetable.Entity != null)
			{
				bool flag = this.Targetable.Entity.TargetController != null;
				this.m_targetOfTargetNameplate.gameObject.SetActive(flag);
				if (flag)
				{
					this.TargetControllerOnOffensiveTargetChanged(this.Targetable.Entity.TargetController.OffensiveTarget);
					this.Targetable.Entity.TargetController.OffensiveTargetChanged += this.TargetControllerOnOffensiveTargetChanged;
				}
			}
			this.RefreshPendingLootRollIndicator();
			this.RefreshEncounterLockIndicator();
			this.RefreshDifficultyIndicator();
			this.RefreshRoleIndicator();
			this.RefreshLevelLabel();
			this.RefreshAfkPanel();
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x0014C0C0 File Offset: 0x0014A2C0
		private void Unsubscribe()
		{
			if (this.m_targetable == null)
			{
				return;
			}
			if (this.m_targetable.PlayerFlags != null)
			{
				this.m_targetable.PlayerFlags.Changed -= this.PlayerFlagsOnChanged;
			}
			if (this.SubscribeToTextChanges() && this.m_targetable.Name != null)
			{
				this.m_targetable.Name.Changed -= this.NameOnChanged;
			}
			if (this.SubscribeToTextChanges() && this.m_targetable.Title != null)
			{
				this.m_targetable.Title.Changed -= this.TitleOnChanged;
				if (this.m_targetable.Entity && this.m_targetable.Entity.CharacterData)
				{
					this.m_targetable.Entity.CharacterData.PresenceChanged -= this.CharacterDataOnPresenceChanged;
				}
			}
			if (this.SubscribeToTextChanges() && this.m_targetable.Guild != null)
			{
				this.m_targetable.Guild.Changed -= this.GuildOnChanged;
			}
			if (this.SubscribeToPortraitChange())
			{
				this.m_targetable.Entity.CharacterData.PortraitId.Changed -= this.PortraitIdOnChanged;
			}
			if (this.m_targetable.Entity.Type == GameEntityType.Npc)
			{
				this.m_targetable.Entity.VitalsReplicator.CurrentHealthState.Changed -= this.NpcCurrentHealthStateOnChanged;
				InteractiveNpc interactiveNpc = this.m_targetable.Entity.Interactive as InteractiveNpc;
				if (interactiveNpc != null)
				{
					interactiveNpc.Tagger.Changed -= this.InteractiveTaggerChanged;
					interactiveNpc.GroupId.Changed -= this.InteractiveGroupChanged;
					interactiveNpc.RaidId.Changed -= this.InteractiveRaidChanged;
					interactiveNpc.NpcInteractiveFlags.Changed -= this.NpcInteractiveFlagsOnChanged;
				}
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
				{
					((PlayerCharacterData)LocalPlayer.GameEntity.CharacterData).SyncedNearbyGroupInfo.Changed -= this.SyncedNearbyGroupInfoOnChanged;
				}
			}
			if (this.m_targetable.Entity.Type == GameEntityType.Player)
			{
				this.m_targetable.Entity.VitalsReplicator.CurrentHealthState.Changed -= this.PlayerCurrentHealthStateOnChanged;
				this.m_targetable.Entity.CharacterData.RoleChanged -= this.RefreshRoleIndicator;
				this.m_targetable.Entity.CharacterData.PresenceChanged -= this.RefreshAfkPanel;
				this.m_targetable.Entity.CharacterData.AdventuringLevelSync.Changed -= this.RefreshLevelLabelEvent;
				if (this.m_type == NameplateControllerUI.NameplateType.Overhead)
				{
					this.m_targetable.Entity.CharacterData.LfgChanged -= this.RefreshDisplayName;
				}
			}
			if (this.m_targetOfTargetNameplate && this.Targetable != null && this.Targetable.Entity != null && this.Targetable.Entity.TargetController != null)
			{
				this.Targetable.Entity.TargetController.OffensiveTargetChanged -= this.TargetControllerOnOffensiveTargetChanged;
			}
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x0014C434 File Offset: 0x0014A634
		public void InitWorldSpace(ITargetable targetable)
		{
			this.m_isDead = false;
			this.m_initialized = false;
			if (this.m_offensiveTargetIndicator)
			{
				this.m_offensiveTargetIndicator.SetActive(false);
			}
			if (this.m_defensiveTargetIndicator)
			{
				this.m_defensiveTargetIndicator.SetActive(false);
			}
			this.Init(targetable);
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x0014C488 File Offset: 0x0014A688
		public void Init(ITargetable targetable)
		{
			this.m_isDead = false;
			this.m_titleColor = null;
			this.Targetable = targetable;
			if (this.Targetable != null)
			{
				this.RefreshTitleColor(false);
				SynchronizedString title = targetable.Title;
				this.TitleOnChanged((title != null) ? title.Value : null);
				SynchronizedString guild = targetable.Guild;
				this.GuildOnChanged((guild != null) ? guild.Value : null);
				if (this.HasEffectsPanel() && this.m_effectIconPanel != null)
				{
					this.m_effectIconPanel.gameObject.SetActive(true);
					this.m_effectIconPanel.Init(targetable.Entity.NetworkEntity, false);
				}
				if (targetable.Entity != null && targetable.Entity.Type == GameEntityType.Npc)
				{
					this.NpcCurrentHealthStateOnChanged(targetable.Entity.VitalsReplicator.CurrentHealthState.Value);
				}
				this.RefreshDisplayName();
			}
			else
			{
				this.NameOnChanged(null);
				this.TitleOnChanged(null);
				this.GuildOnChanged(null);
				if (this.HasEffectsPanel() && this.m_effectIconPanel != null)
				{
					this.m_effectIconPanel.Init(null, false);
					this.m_effectIconPanel.gameObject.SetActive(false);
				}
			}
			this.RefreshDirectionalIndicator();
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x0014C5C0 File Offset: 0x0014A7C0
		private void RefreshDirectionalIndicator()
		{
			if (!this.m_directionIndicatorUI)
			{
				return;
			}
			NameplateControllerUI.NameplateType type = this.m_type;
			if (type == NameplateControllerUI.NameplateType.Player || type == NameplateControllerUI.NameplateType.Overhead)
			{
				this.m_directionIndicatorUI.gameObject.SetActive(false);
				return;
			}
			this.m_directionIndicatorUI.SetTarget(this.Targetable);
			bool flag = this.Targetable != null && this.Targetable.Entity != LocalPlayer.GameEntity;
			if (this.m_directionIndicatorUI.gameObject.activeSelf != flag)
			{
				this.m_directionIndicatorUI.gameObject.SetActive(flag);
			}
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x0014C654 File Offset: 0x0014A854
		private void RefreshPortrait()
		{
			if (this.m_portraitImage == null)
			{
				return;
			}
			if (this.Targetable == null || this.Targetable.Entity.CharacterData.PortraitId.Value.IsEmpty)
			{
				this.TogglePortrait(false);
				return;
			}
			UniqueId value = this.Targetable.Entity.CharacterData.PortraitId.Value;
			Sprite overrideSprite;
			if (GlobalSettings.Values && GlobalSettings.Values.Portraits != null && GlobalSettings.Values.Portraits.AllPortraits && GlobalSettings.Values.Portraits.AllPortraits.TryGetObject(value, out overrideSprite))
			{
				this.m_portraitImage.overrideSprite = overrideSprite;
			}
			else
			{
				MaleFemaleSpriteCollection maleFemaleSpriteCollection;
				if (!InternalGameDatabase.Archetypes.TryGetAsType<MaleFemaleSpriteCollection>(value, out maleFemaleSpriteCollection))
				{
					this.TogglePortrait(false);
					this.m_portraitImage.overrideSprite = null;
					return;
				}
				Sprite index = maleFemaleSpriteCollection.GetIndex(this.Targetable.Entity.CharacterData.Sex, this.Targetable.Entity.CharacterData.PortraitIndex);
				if (index == null)
				{
					this.TogglePortrait(false);
					this.m_portraitImage.overrideSprite = null;
					return;
				}
				this.m_portraitImage.overrideSprite = index;
			}
			this.TogglePortrait(true);
			this.RefreshPortraitColor();
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x0014C7A8 File Offset: 0x0014A9A8
		private void RefreshPortraitColor()
		{
			if (this.m_portraitImage)
			{
				this.m_portraitImage.color = ((this.Targetable != null && this.Targetable.IsNpc && this.Targetable.Entity && this.Targetable.Entity.IsDead) ? Color.gray : Color.white);
			}
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x0005F23F File Offset: 0x0005D43F
		private void TogglePortrait(bool isEnabled)
		{
			if (this.m_portraitImage)
			{
				this.m_portraitImage.enabled = isEnabled;
			}
			if (this.m_portraitQuestionMark)
			{
				this.m_portraitQuestionMark.enabled = !isEnabled;
			}
		}

		// Token: 0x06002CF4 RID: 11508 RVA: 0x0014C814 File Offset: 0x0014AA14
		public void RefreshDifficultyIndicator()
		{
			if (!this.m_difficultyIndicatorUi)
			{
				return;
			}
			if (this.m_type == NameplateControllerUI.NameplateType.Overhead && this.m_isDead)
			{
				this.m_difficultyIndicatorUi.Deactivate();
				return;
			}
			NameplateControllerUI.NameplateType type = this.m_type;
			if (type == NameplateControllerUI.NameplateType.Offensive)
			{
				this.m_difficultyIndicatorUi.RefreshIndicator(this);
				return;
			}
			if (type != NameplateControllerUI.NameplateType.Overhead)
			{
				this.m_difficultyIndicatorUi.Deactivate();
				return;
			}
			if (this.m_targetable != null && this.m_targetable.IsNpc)
			{
				this.m_difficultyIndicatorUi.RefreshIndicator(this);
				return;
			}
			this.m_difficultyIndicatorUi.Deactivate();
		}

		// Token: 0x06002CF5 RID: 11509 RVA: 0x0005F276 File Offset: 0x0005D476
		public void RefreshRoleIndicator()
		{
			if (!this.m_roleIndicatorUi)
			{
				return;
			}
			if (this.m_type == NameplateControllerUI.NameplateType.Offensive)
			{
				this.m_roleIndicatorUi.gameObject.SetActive(false);
				return;
			}
			this.m_roleIndicatorUi.RefreshIndicator(this);
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x0005F2AD File Offset: 0x0005D4AD
		public void RefreshFlankingIndicator()
		{
			if (!this.m_flankingIndicator)
			{
				return;
			}
			if (this.m_type == NameplateControllerUI.NameplateType.Offensive)
			{
				this.m_flankingIndicator.gameObject.SetActive(true);
				return;
			}
			this.m_flankingIndicator.gameObject.SetActive(false);
		}

		// Token: 0x06002CF7 RID: 11511 RVA: 0x0005F2E9 File Offset: 0x0005D4E9
		private void RefreshLevelLabelEvent(byte level)
		{
			this.RefreshLevelLabel();
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x0014C8A4 File Offset: 0x0014AAA4
		public void RefreshLevelLabel()
		{
			if (!this.m_levelLabel)
			{
				return;
			}
			if (this.Targetable != null && this.Targetable.IsPlayer && this.ShowLevel())
			{
				this.m_levelLabel.text = this.Targetable.Level.ToString();
				if (!this.m_levelLabel.enabled)
				{
					this.m_levelLabel.enabled = true;
					return;
				}
			}
			else
			{
				this.m_levelLabel.enabled = false;
			}
		}

		// Token: 0x06002CF9 RID: 11513 RVA: 0x0005F2F1 File Offset: 0x0005D4F1
		internal void RefreshEncounterLockIndicator()
		{
			if (this.m_encounterLockUi)
			{
				this.m_encounterLockUi.RefreshIndicator(this);
			}
		}

		// Token: 0x06002CFA RID: 11514 RVA: 0x0005F30C File Offset: 0x0005D50C
		internal void RefreshPendingLootRollIndicator()
		{
			if (this.m_pendingLootRollIndicator)
			{
				this.m_pendingLootRollIndicator.RefreshIndicator(this);
			}
		}

		// Token: 0x06002CFB RID: 11515 RVA: 0x0005F327 File Offset: 0x0005D527
		public void DisableRoleIndicator()
		{
			if (this.m_roleIndicatorUi)
			{
				this.m_roleIndicatorUi.DisableIndicator();
			}
		}

		// Token: 0x06002CFC RID: 11516 RVA: 0x0005F341 File Offset: 0x0005D541
		private void NpcInteractiveFlagsOnChanged(InteractiveFlags obj)
		{
			this.RefreshEncounterLockIndicator();
			this.RefreshPendingLootRollIndicator();
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x0005F34F File Offset: 0x0005D54F
		private void InteractiveTaggerChanged(string obj)
		{
			this.RefreshEncounterLockIndicator();
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x0005F34F File Offset: 0x0005D54F
		private void InteractiveGroupChanged(UniqueId obj)
		{
			this.RefreshEncounterLockIndicator();
		}

		// Token: 0x06002CFF RID: 11519 RVA: 0x0005F34F File Offset: 0x0005D54F
		private void InteractiveRaidChanged(UniqueId obj)
		{
			this.RefreshEncounterLockIndicator();
		}

		// Token: 0x06002D00 RID: 11520 RVA: 0x0005F357 File Offset: 0x0005D557
		private void SyncedNearbyGroupInfoOnChanged(NearbyGroupInfo obj)
		{
			this.RefreshDifficultyIndicator();
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x0005F357 File Offset: 0x0005D557
		private void ShowGelIndicatorsOnChanged()
		{
			this.RefreshDifficultyIndicator();
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x0014C920 File Offset: 0x0014AB20
		private void RefreshAfkPanel()
		{
			if (this.m_afkPanel == null)
			{
				return;
			}
			this.m_afkPanel.gameObject.SetActive(this.m_targetable != null && this.m_targetable.IsPlayer && this.m_targetable.Entity != null && this.m_targetable.Entity.CharacterData != null && this.m_targetable.Entity.CharacterData.PresenceFlags.IsAFK());
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x0005F35F File Offset: 0x0005D55F
		internal bool AllowLockIndicator()
		{
			return this.m_type == NameplateControllerUI.NameplateType.Offensive || (this.m_type == NameplateControllerUI.NameplateType.Overhead && this.m_targetable != null && this.m_targetable.IsNpc);
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x0005F35F File Offset: 0x0005D55F
		internal bool AllowPendingLootRollIndicator()
		{
			return this.m_type == NameplateControllerUI.NameplateType.Offensive || (this.m_type == NameplateControllerUI.NameplateType.Overhead && this.m_targetable != null && this.m_targetable.IsNpc);
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x0014C9AC File Offset: 0x0014ABAC
		private void UpdateStatBars(bool immediate, bool force)
		{
			for (int i = 0; i < this.m_statBars.Length; i++)
			{
				if (this.m_statBars[i])
				{
					this.m_statBars[i].UpdateStatBar(this.Targetable.Entity, immediate, force);
				}
			}
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x0014C9F8 File Offset: 0x0014ABF8
		private void ResetLastValues()
		{
			for (int i = 0; i < this.m_statBars.Length; i++)
			{
				if (this.m_statBars[i])
				{
					this.m_statBars[i].ResetLastValues();
				}
			}
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x0005F38C File Offset: 0x0005D58C
		private void NpcCurrentHealthStateOnChanged(HealthState obj)
		{
			if (!this.m_isDead && obj == HealthState.Dead && this.m_targetable != null && this.m_targetable.IsNpc)
			{
				this.m_isDead = true;
				this.RefreshDifficultyIndicator();
				this.RefreshDisplayName();
				this.RefreshPortraitColor();
			}
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x0005F3C8 File Offset: 0x0005D5C8
		private void PlayerCurrentHealthStateOnChanged(HealthState obj)
		{
			Action<HealthState> playerHealthStateUpdated = this.PlayerHealthStateUpdated;
			if (playerHealthStateUpdated == null)
			{
				return;
			}
			playerHealthStateUpdated(obj);
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x0005F3DB File Offset: 0x0005D5DB
		private void PortraitIdOnChanged(UniqueId obj)
		{
			this.RefreshPortrait();
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x0014CA34 File Offset: 0x0014AC34
		private void PlayerFlagsOnChanged(PlayerFlags obj)
		{
			for (int i = 0; i < this.m_playerFlagsIcons.Length; i++)
			{
				bool active = obj.HasBitFlag(this.m_playerFlagsIcons[i].Flag);
				if (this.m_playerFlagsIcons[i].Flag == PlayerFlags.InGroup && this.m_groupNameplateControllerUi != null)
				{
					this.m_playerFlagsIcons[i].Panel.SetActive(false);
				}
				else
				{
					this.m_playerFlagsIcons[i].Panel.SetActive(active);
				}
			}
		}

		// Token: 0x06002D0B RID: 11531 RVA: 0x0014CAB0 File Offset: 0x0014ACB0
		public void OffensiveTargetChanged(GameEntity obj)
		{
			if (!GameManager.IsServer && this.m_offensiveTargetIndicator)
			{
				bool flag = this.m_targetable != null && this.m_targetable.Entity == obj;
				if (this.m_offensiveTargetIndicator.activeSelf != flag)
				{
					this.m_offensiveTargetIndicator.SetActive(flag);
				}
			}
		}

		// Token: 0x06002D0C RID: 11532 RVA: 0x0014CB08 File Offset: 0x0014AD08
		public void DefensiveTargetChanged(GameEntity obj)
		{
			if (!GameManager.IsServer && this.m_defensiveTargetIndicator)
			{
				bool flag = this.m_targetable != null && this.m_targetable.Entity == obj;
				if (this.m_defensiveTargetIndicator.activeSelf != flag)
				{
					this.m_defensiveTargetIndicator.SetActive(flag);
				}
			}
		}

		// Token: 0x06002D0D RID: 11533 RVA: 0x0005F3E3 File Offset: 0x0005D5E3
		private void GuildOnChanged(string obj)
		{
			this.SetText(this.m_guild, string.IsNullOrEmpty(obj) ? null : ("<" + obj + ">"));
		}

		// Token: 0x06002D0E RID: 11534 RVA: 0x0005F40C File Offset: 0x0005D60C
		private void CharacterDataOnPresenceChanged()
		{
			this.RefreshTitleColor(true);
		}

		// Token: 0x06002D0F RID: 11535 RVA: 0x0014CB60 File Offset: 0x0014AD60
		private void RefreshTitleColor(bool updateTitle)
		{
			Color? color = null;
			if (this.Targetable != null && this.Targetable.Entity && this.Targetable.Entity.CharacterData)
			{
				if (this.Targetable.Entity.CharacterData.IsGM)
				{
					color = new Color?(UIManager.EmberColor);
				}
				else if (this.Targetable.Entity.CharacterData.IsSubscriber)
				{
					color = new Color?(UIManager.SubscriberColor);
				}
				else if (this.Targetable.Entity.CharacterData.IsTrial)
				{
					color = new Color?(UIManager.TrialColor);
				}
			}
			bool flag = color != this.m_titleColor;
			this.m_titleColor = color;
			if (flag && updateTitle && this.Targetable != null)
			{
				SynchronizedString title = this.Targetable.Title;
				this.TitleOnChanged((title != null) ? title.Value : null);
			}
		}

		// Token: 0x06002D10 RID: 11536 RVA: 0x0014CC84 File Offset: 0x0014AE84
		private void TitleOnChanged(string obj)
		{
			this.SetText(this.m_title, obj);
			if (this.m_title)
			{
				this.m_title.color = ((this.m_titleColor != null) ? this.m_titleColor.Value : NameplateControllerUI.m_defaultTitleColor);
			}
		}

		// Token: 0x06002D11 RID: 11537 RVA: 0x0005F415 File Offset: 0x0005D615
		private void NameOnChanged(string obj)
		{
			this.RefreshDisplayName();
		}

		// Token: 0x06002D12 RID: 11538 RVA: 0x0005F34F File Offset: 0x0005D54F
		private void GroupManagerOnGroupIdChanged()
		{
			this.RefreshEncounterLockIndicator();
		}

		// Token: 0x06002D13 RID: 11539 RVA: 0x0014CCD8 File Offset: 0x0014AED8
		private void RefreshDisplayName()
		{
			if (this.Targetable == null)
			{
				this.m_name.text = string.Empty;
				return;
			}
			if (this.m_type == NameplateControllerUI.NameplateType.Overhead && this.Targetable.IsPlayer && this.Targetable.Entity != null && this.Targetable.Entity.CharacterData != null && this.Targetable.Entity.CharacterData.IsLfg)
			{
				if (string.IsNullOrEmpty(NameplateControllerUI.m_lfgTagColored))
				{
					NameplateControllerUI.m_lfgTagColored = "<color=" + GlobalSettings.Values.Nameplates.GroupColor.ToHex() + "><font=\"Font Awesome 5 Free-Solid-900 SDF\"></font> </color>";
				}
				this.m_name.SetTextFormat("{0}{1}", NameplateControllerUI.m_lfgTagColored, this.Targetable.Name.Value);
				return;
			}
			if (this.m_isDead && this.Targetable.IsNpc)
			{
				this.m_name.SetTextFormat("{0}'s corpse", this.Targetable.Name.Value);
				return;
			}
			this.m_name.ZStringSetText(this.Targetable.Name.Value);
		}

		// Token: 0x06002D14 RID: 11540 RVA: 0x0005F41D File Offset: 0x0005D61D
		private void TargetControllerOnOffensiveTargetChanged(GameEntity obj)
		{
			if (this.m_targetOfTargetNameplate)
			{
				this.m_targetOfTargetNameplate.Init((obj == null) ? null : obj.Targetable);
			}
		}

		// Token: 0x06002D15 RID: 11541 RVA: 0x0005F449 File Offset: 0x0005D649
		private void SetText(TextMeshProUGUI tmp, string txt)
		{
			if (tmp != null)
			{
				tmp.ZStringSetText(txt);
				tmp.enabled = !string.IsNullOrEmpty(txt);
			}
		}

		// Token: 0x06002D16 RID: 11542 RVA: 0x0005F46A File Offset: 0x0005D66A
		public void ToggleLeaderIcon(bool isLeader, bool isRaidLeader)
		{
			if (this.m_leaderIcon)
			{
				this.m_leaderIcon.Refresh(isLeader, isRaidLeader);
			}
		}

		// Token: 0x06002D17 RID: 11543 RVA: 0x0014CE08 File Offset: 0x0014B008
		string IContextMenu.FillActionsGetTitle()
		{
			string result = "";
			switch (this.m_type)
			{
			case NameplateControllerUI.NameplateType.Player:
				result = "Player";
				ContextMenuUI.AddContextAction("Change Portrait", true, delegate()
				{
					ClientGameManager.UIManager.SelectPortraitWindow.Init(LocalPlayer.GameEntity.CollectionController.Record, LocalPlayer.GameEntity.CharacterData.Name.Value);
				}, null, null);
				break;
			case NameplateControllerUI.NameplateType.Offensive:
				result = "Offensive";
				break;
			case NameplateControllerUI.NameplateType.Defensive:
				result = "Defensive";
				if (this.m_targetable != null && this.m_targetable.Entity && this.m_targetable.Entity.Interactive != null)
				{
					InteractivePlayer interactivePlayer = this.m_targetable.Entity.Interactive as InteractivePlayer;
					if (interactivePlayer)
					{
						interactivePlayer.FillActionsGetTitle();
					}
				}
				break;
			case NameplateControllerUI.NameplateType.Group:
			case NameplateControllerUI.NameplateType.Raid:
				if (this.m_groupNameplateControllerUi)
				{
					result = this.m_groupNameplateControllerUi.FillActionsGetTitle();
				}
				break;
			case NameplateControllerUI.NameplateType.TargetOfTarget:
				result = "Target of Target";
				ContextMenuUI.AddContextAction("Reset Position", true, delegate()
				{
					Transform transform = base.gameObject.transform;
					if (transform.parent)
					{
						RectTransform rectTransform = transform.parent as RectTransform;
						if (rectTransform != null)
						{
							rectTransform.anchorMin = Vector2.right;
							rectTransform.anchorMax = Vector2.right;
							rectTransform.pivot = new Vector2(1f, 0.5f);
							rectTransform.anchoredPosition = Vector2.zero;
						}
					}
					RectTransform rectTransform2 = transform as RectTransform;
					if (rectTransform2 != null)
					{
						rectTransform2.anchorMin = Vector2.one * 0.5f;
						rectTransform2.anchorMax = Vector2.one * 0.5f;
						rectTransform2.pivot = Vector2.one * 0.5f;
						rectTransform2.anchoredPosition = Vector2.zero;
					}
				}, null, null);
				break;
			}
			if (ContextMenuUI.ActionList.Count <= 0)
			{
				return string.Empty;
			}
			return result;
		}

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06002D18 RID: 11544 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002D19 RID: 11545 RVA: 0x0014CF34 File Offset: 0x0014B134
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				NameplateControllerUI.NameplateType type = this.m_type;
				if (type == NameplateControllerUI.NameplateType.Player)
				{
					LocalPlayer.GameEntity.TargetController.SetTarget(TargetType.Defensive, LocalPlayer.GameEntity.Targetable);
					return;
				}
				if (type == NameplateControllerUI.NameplateType.TargetOfTarget)
				{
					if (this.Targetable != null && this.Targetable.Entity != null && this.Targetable.Entity.CharacterData != null)
					{
						TargetType playerTargetType = this.Targetable.Entity.CharacterData.Faction.GetPlayerTargetType();
						LocalPlayer.GameEntity.TargetController.SetTarget(playerTargetType, this.Targetable);
						return;
					}
				}
			}
			if (this.m_groupNameplateControllerUi != null)
			{
				this.m_groupNameplateControllerUi.OnPointerDown(eventData);
			}
		}

		// Token: 0x06002D1A RID: 11546 RVA: 0x0005F486 File Offset: 0x0005D686
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_portraitHighlight != null)
			{
				this.m_portraitHighlight.enabled = true;
			}
		}

		// Token: 0x06002D1B RID: 11547 RVA: 0x0005F4A2 File Offset: 0x0005D6A2
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (this.m_portraitHighlight != null)
			{
				this.m_portraitHighlight.enabled = false;
			}
		}

		// Token: 0x06002D1E RID: 11550 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002C87 RID: 11399
		internal static Color kOffensiveColor = Color.red;

		// Token: 0x04002C88 RID: 11400
		internal static Color kDefensiveColor = Color.blue;

		// Token: 0x04002C89 RID: 11401
		internal static Color kGroupColor = Color.cyan;

		// Token: 0x04002C8B RID: 11403
		[SerializeField]
		private NameplateControllerUI.NameplateType m_type;

		// Token: 0x04002C8C RID: 11404
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04002C8D RID: 11405
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04002C8E RID: 11406
		[SerializeField]
		private TextMeshProUGUI m_guild;

		// Token: 0x04002C8F RID: 11407
		[SerializeField]
		private NameplateControllerStatBarUI[] m_statBars;

		// Token: 0x04002C90 RID: 11408
		[SerializeField]
		private DraggableUIWindow m_draggableWindow;

		// Token: 0x04002C91 RID: 11409
		[SerializeField]
		private bool m_findOnStart;

		// Token: 0x04002C92 RID: 11410
		[SerializeField]
		private GameObject m_offensiveTargetIndicator;

		// Token: 0x04002C93 RID: 11411
		[SerializeField]
		private GameObject m_defensiveTargetIndicator;

		// Token: 0x04002C94 RID: 11412
		[SerializeField]
		private EffectIconPanelUI m_effectIconPanel;

		// Token: 0x04002C95 RID: 11413
		[SerializeField]
		private NameplateControllerUI.PlayerFlagsIconSetting[] m_playerFlagsIcons;

		// Token: 0x04002C96 RID: 11414
		[SerializeField]
		private LeaderIcon m_leaderIcon;

		// Token: 0x04002C97 RID: 11415
		[SerializeField]
		private GroupNameplateControllerUI m_groupNameplateControllerUi;

		// Token: 0x04002C98 RID: 11416
		[SerializeField]
		private Image[] m_imagesToColor;

		// Token: 0x04002C99 RID: 11417
		private const string kPortrait = "Portrait";

		// Token: 0x04002C9A RID: 11418
		[SerializeField]
		private Image m_portraitImage;

		// Token: 0x04002C9B RID: 11419
		[SerializeField]
		private Image m_portraitHighlight;

		// Token: 0x04002C9C RID: 11420
		[SerializeField]
		private Image m_portraitQuestionMark;

		// Token: 0x04002C9D RID: 11421
		[SerializeField]
		private BackpackDirectionIndicatorUI m_backpackDirectionIndicatorUi;

		// Token: 0x04002C9E RID: 11422
		[SerializeField]
		private DifficultyIndicatorUI m_difficultyIndicatorUi;

		// Token: 0x04002C9F RID: 11423
		[SerializeField]
		private RoleIndicator m_roleIndicatorUi;

		// Token: 0x04002CA0 RID: 11424
		[SerializeField]
		private FlankingIndicator m_flankingIndicator;

		// Token: 0x04002CA1 RID: 11425
		[SerializeField]
		private EncounterLockUI m_encounterLockUi;

		// Token: 0x04002CA2 RID: 11426
		[SerializeField]
		private DirectionIndicatorUI m_directionIndicatorUI;

		// Token: 0x04002CA3 RID: 11427
		[SerializeField]
		private PendingLootRollIndicator m_pendingLootRollIndicator;

		// Token: 0x04002CA4 RID: 11428
		[SerializeField]
		private RectTransform m_statBarContentWindow;

		// Token: 0x04002CA5 RID: 11429
		[SerializeField]
		private NameplateControllerUI m_targetOfTargetNameplate;

		// Token: 0x04002CA6 RID: 11430
		[SerializeField]
		private NameplateDebug m_debug;

		// Token: 0x04002CA7 RID: 11431
		[SerializeField]
		private TextMeshProUGUI m_levelLabel;

		// Token: 0x04002CA8 RID: 11432
		[SerializeField]
		private GameObject m_afkPanel;

		// Token: 0x04002CA9 RID: 11433
		[SerializeField]
		private ToggleController m_lockToggle;

		// Token: 0x04002CAA RID: 11434
		private float m_statBarContentWindowDefaultHeight = 32f;

		// Token: 0x04002CAB RID: 11435
		private Vector4 m_nameMarginDefaultValue;

		// Token: 0x04002CAD RID: 11437
		private bool m_initialized;

		// Token: 0x04002CAE RID: 11438
		private bool m_isDead;

		// Token: 0x04002CAF RID: 11439
		private ITargetable m_targetable;

		// Token: 0x04002CB0 RID: 11440
		private bool m_hideStaminaBar;

		// Token: 0x04002CB1 RID: 11441
		private static readonly Color m_defaultTitleColor = new Color(0.7921569f, 0.77254903f, 0.76862746f, 1f);

		// Token: 0x04002CB2 RID: 11442
		private Color? m_titleColor;

		// Token: 0x04002CB3 RID: 11443
		private const string kLfgTag = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font> ";

		// Token: 0x04002CB4 RID: 11444
		private static string m_lfgTagColored = null;

		// Token: 0x0200059E RID: 1438
		internal enum NameplateType
		{
			// Token: 0x04002CB6 RID: 11446
			None,
			// Token: 0x04002CB7 RID: 11447
			Player,
			// Token: 0x04002CB8 RID: 11448
			Offensive,
			// Token: 0x04002CB9 RID: 11449
			Defensive,
			// Token: 0x04002CBA RID: 11450
			Group,
			// Token: 0x04002CBB RID: 11451
			Overhead,
			// Token: 0x04002CBC RID: 11452
			TargetOfTarget,
			// Token: 0x04002CBD RID: 11453
			Raid
		}

		// Token: 0x0200059F RID: 1439
		[Serializable]
		private class PlayerFlagsIconSetting
		{
			// Token: 0x04002CBE RID: 11454
			public PlayerFlags Flag;

			// Token: 0x04002CBF RID: 11455
			public GameObject Panel;
		}
	}
}
