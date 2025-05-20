using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x0200084F RID: 2127
	public class AlchemySelectionUI : MonoBehaviour
	{
		// Token: 0x17000E31 RID: 3633
		// (get) Token: 0x06003D57 RID: 15703 RVA: 0x000698EF File Offset: 0x00067AEF
		private SolToggle m_toggleI
		{
			get
			{
				return this.m_alchemyI.Toggle;
			}
		}

		// Token: 0x17000E32 RID: 3634
		// (get) Token: 0x06003D58 RID: 15704 RVA: 0x000698FC File Offset: 0x00067AFC
		private SolToggle m_toggleII
		{
			get
			{
				return this.m_alchemyII.Toggle;
			}
		}

		// Token: 0x17000E33 RID: 3635
		// (get) Token: 0x06003D59 RID: 15705 RVA: 0x00069909 File Offset: 0x00067B09
		// (set) Token: 0x06003D5A RID: 15706 RVA: 0x00069911 File Offset: 0x00067B11
		private bool MuteEvents { get; set; }

		// Token: 0x17000E34 RID: 3636
		// (get) Token: 0x06003D5B RID: 15707 RVA: 0x0006991A File Offset: 0x00067B1A
		// (set) Token: 0x06003D5C RID: 15708 RVA: 0x00069922 File Offset: 0x00067B22
		private BaseCollectionController CollectionController { get; set; }

		// Token: 0x17000E35 RID: 3637
		// (get) Token: 0x06003D5D RID: 15709 RVA: 0x0006992B File Offset: 0x00067B2B
		// (set) Token: 0x06003D5E RID: 15710 RVA: 0x00069932 File Offset: 0x00067B32
		public static AlchemyPowerLevelFlags LevelFlags
		{
			get
			{
				return AlchemySelectionUI.m_levelFlags;
			}
			private set
			{
				if (AlchemySelectionUI.m_levelFlags != value)
				{
					AlchemySelectionUI.m_levelFlags = value;
					Action alchemyLevelChanged = AlchemySelectionUI.AlchemyLevelChanged;
					if (alchemyLevelChanged == null)
					{
						return;
					}
					alchemyLevelChanged();
				}
			}
		}

		// Token: 0x140000B7 RID: 183
		// (add) Token: 0x06003D5F RID: 15711 RVA: 0x001824CC File Offset: 0x001806CC
		// (remove) Token: 0x06003D60 RID: 15712 RVA: 0x00182500 File Offset: 0x00180700
		public static event Action AlchemyLevelChanged;

		// Token: 0x06003D61 RID: 15713 RVA: 0x00182534 File Offset: 0x00180734
		private void Awake()
		{
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.AlchemySelectionUI = this;
			}
			AlchemySelectionUI.m_levelFlags = AlchemyPowerLevelFlags.None;
			if (!GameManager.AllowAlchemy)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_alchemyI.Init(this);
			this.m_toggleI.isOn = false;
			this.m_toggleI.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleIChanged));
			this.m_alchemyII.Init(this);
			this.m_toggleII.isOn = false;
			this.m_toggleII.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleIIChanged));
		}

		// Token: 0x06003D62 RID: 15714 RVA: 0x001825DC File Offset: 0x001807DC
		private void Start()
		{
			if (LocalPlayer.IsInitialized)
			{
				this.LocalPlayerOnLocalPlayerInitialized();
			}
			else
			{
				LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			}
			LocalClientSkillsController.AlchemyExecutionComplete += this.LocalClientSkillsControllerOnAlchemyExecutionComplete;
			LocalClientSkillsController.PendingCancelled += this.LocalClientSkillsControllerPendingCancelled;
			UIManager.AbilityContainerChanged += this.UIManagerOnAbilityContainerChanged;
			LocalPlayer.AlchemyIIUnlocked += this.LocalPlayerOnAlchemyIIUnlocked;
		}

		// Token: 0x06003D63 RID: 15715 RVA: 0x00182650 File Offset: 0x00180850
		private void OnDestroy()
		{
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.AlchemySelectionUI = null;
			}
			this.m_toggleI.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleIChanged));
			this.m_toggleII.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleIIChanged));
			if (this.CollectionController)
			{
				this.CollectionController.EmberStoneChanged -= this.CollectionControllerOnEmberStoneChanged;
				this.CollectionController = null;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed -= this.AdventuringLevelSyncOnChanged;
			}
			if (GameManager.QuestManager)
			{
				GameManager.QuestManager.QuestUpdated -= this.QuestManagerOnQuestUpdated;
			}
			LocalClientSkillsController.AlchemyExecutionComplete -= this.LocalClientSkillsControllerOnAlchemyExecutionComplete;
			LocalClientSkillsController.PendingCancelled -= this.LocalClientSkillsControllerPendingCancelled;
			UIManager.AbilityContainerChanged -= this.UIManagerOnAbilityContainerChanged;
			LocalPlayer.AlchemyIIUnlocked -= this.LocalPlayerOnAlchemyIIUnlocked;
		}

		// Token: 0x06003D64 RID: 15716 RVA: 0x00069951 File Offset: 0x00067B51
		private void Update()
		{
			this.m_alchemyI.Update();
			this.m_alchemyII.Update();
		}

		// Token: 0x06003D65 RID: 15717 RVA: 0x0018277C File Offset: 0x0018097C
		public void HandleInput()
		{
			if (SolInput.GetButtonDown(106))
			{
				if (this.m_toggleI.InteractableAndActiveInHierarchy())
				{
					this.m_toggleI.isOn = !this.m_toggleI.isOn;
					return;
				}
			}
			else if (SolInput.GetButtonDown(107))
			{
				if (this.m_toggleII.InteractableAndActiveInHierarchy())
				{
					this.m_toggleII.isOn = !this.m_toggleII.isOn;
					return;
				}
			}
			else if (SolInput.GetButtonDown(108))
			{
				bool flag = this.m_toggleI.InteractableAndActiveInHierarchy();
				if (flag && this.m_toggleII.InteractableAndActiveInHierarchy())
				{
					if (AlchemySelectionUI.LevelFlags == AlchemyPowerLevelFlags.None)
					{
						this.m_toggleI.isOn = true;
						return;
					}
					bool flag2 = AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.I);
					bool flag3 = AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.II);
					if (flag2 && !flag3)
					{
						this.m_toggleII.isOn = true;
						return;
					}
					if (flag2 && flag3)
					{
						this.m_toggleI.isOn = false;
						this.m_toggleII.isOn = false;
						return;
					}
				}
				else if (flag)
				{
					this.m_toggleI.isOn = !this.m_toggleI.isOn;
				}
			}
		}

		// Token: 0x06003D66 RID: 15718 RVA: 0x00182890 File Offset: 0x00180A90
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			if (LocalPlayer.GameEntity)
			{
				BaseCollectionController baseCollectionController;
				if (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.TryGetAsType(out baseCollectionController))
				{
					this.CollectionController = baseCollectionController;
					baseCollectionController.EmberStoneChanged += this.CollectionControllerOnEmberStoneChanged;
					this.CollectionControllerOnEmberStoneChanged();
				}
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed += this.AdventuringLevelSyncOnChanged;
					this.AdventuringLevelSyncOnChanged(LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Value);
				}
				if (GameManager.QuestManager)
				{
					GameManager.QuestManager.QuestUpdated += this.QuestManagerOnQuestUpdated;
				}
			}
		}

		// Token: 0x06003D67 RID: 15719 RVA: 0x00069969 File Offset: 0x00067B69
		private void QuestManagerOnQuestUpdated(ObjectiveIterationCache obj)
		{
			if (obj.QuestId == GlobalSettings.Values.Ashen.AlchemyQuestId)
			{
				this.RefreshAvailability();
			}
		}

		// Token: 0x06003D68 RID: 15720 RVA: 0x0006998D File Offset: 0x00067B8D
		private void AdventuringLevelSyncOnChanged(byte obj)
		{
			this.RefreshAvailability();
			Action alchemyLevelChanged = AlchemySelectionUI.AlchemyLevelChanged;
			if (alchemyLevelChanged == null)
			{
				return;
			}
			alchemyLevelChanged();
		}

		// Token: 0x06003D69 RID: 15721 RVA: 0x000699A4 File Offset: 0x00067BA4
		private void LocalClientSkillsControllerOnAlchemyExecutionComplete()
		{
			this.ResetAlchemyToggles();
			this.RefreshAvailability();
		}

		// Token: 0x06003D6A RID: 15722 RVA: 0x000699B2 File Offset: 0x00067BB2
		private void LocalClientSkillsControllerPendingCancelled(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel != AlchemyPowerLevel.None && !Options.GameOptions.KeepAlchemyActiveOnCancel.Value)
			{
				this.ResetAlchemyToggles();
				this.RefreshAvailability();
			}
		}

		// Token: 0x06003D6B RID: 15723 RVA: 0x000699CF File Offset: 0x00067BCF
		private void LocalPlayerOnAlchemyIIUnlocked()
		{
			this.RefreshAvailability();
		}

		// Token: 0x06003D6C RID: 15724 RVA: 0x000699CF File Offset: 0x00067BCF
		private void UIManagerOnAbilityContainerChanged()
		{
			this.RefreshAvailability();
		}

		// Token: 0x06003D6D RID: 15725 RVA: 0x000699D7 File Offset: 0x00067BD7
		private void ResetAlchemyToggles()
		{
			this.MuteEvents = true;
			this.m_toggleI.isOn = false;
			this.m_toggleII.isOn = false;
			this.MuteEvents = false;
			this.RefreshAlchemyLevel();
		}

		// Token: 0x06003D6E RID: 15726 RVA: 0x000699CF File Offset: 0x00067BCF
		private void CollectionControllerOnEmberStoneChanged()
		{
			this.RefreshAvailability();
		}

		// Token: 0x06003D6F RID: 15727 RVA: 0x00182968 File Offset: 0x00180B68
		private void RefreshAvailability()
		{
			if (GlobalSettings.Values.Ashen.AlchemyAvailableForEntity(LocalPlayer.GameEntity))
			{
				int emberEssenceCount = this.CollectionController.GetEmberEssenceCount();
				bool flag = false;
				bool flag2 = false;
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Abilities != null)
				{
					foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Abilities.Instances)
					{
						if (archetypeInstance.Index != -1)
						{
							if (!flag)
							{
								flag = AlchemyExtensions.AlchemyPowerLevelAvailableBypassCooldown(LocalPlayer.GameEntity, archetypeInstance, AlchemyPowerLevel.I);
							}
							if (!flag2)
							{
								flag2 = AlchemyExtensions.AlchemyPowerLevelAvailableBypassCooldown(LocalPlayer.GameEntity, archetypeInstance, AlchemyPowerLevel.II);
							}
							if (flag2 && flag)
							{
								break;
							}
						}
					}
				}
				this.MuteEvents = true;
				this.m_alchemyI.SetToggleAvailable(emberEssenceCount, AlchemyPowerLevel.I, flag);
				this.m_alchemyII.SetToggleAvailable(emberEssenceCount, AlchemyPowerLevel.II, flag2);
				this.MuteEvents = false;
				this.RefreshAlchemyLevel();
				base.gameObject.SetActive(true);
				return;
			}
			base.gameObject.SetActive(false);
			this.ResetAlchemyToggles();
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x00069A05 File Offset: 0x00067C05
		private void ToggleIChanged(bool arg0)
		{
			if (this.MuteEvents)
			{
				return;
			}
			this.MuteEvents = true;
			this.MuteEvents = false;
			this.RefreshAlchemyLevel();
		}

		// Token: 0x06003D71 RID: 15729 RVA: 0x00069A05 File Offset: 0x00067C05
		private void ToggleIIChanged(bool arg0)
		{
			if (this.MuteEvents)
			{
				return;
			}
			this.MuteEvents = true;
			this.MuteEvents = false;
			this.RefreshAlchemyLevel();
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x00182A94 File Offset: 0x00180C94
		private void RefreshAlchemyLevel()
		{
			AlchemyPowerLevelFlags alchemyPowerLevelFlags = AlchemyPowerLevelFlags.None;
			if (this.m_toggleII.isOn)
			{
				alchemyPowerLevelFlags |= AlchemyPowerLevelFlags.II;
			}
			if (this.m_toggleI.isOn)
			{
				alchemyPowerLevelFlags |= AlchemyPowerLevelFlags.I;
			}
			AlchemySelectionUI.LevelFlags = alchemyPowerLevelFlags;
		}

		// Token: 0x04003C17 RID: 15383
		[SerializeField]
		private AlchemySelectionUI.ToggleData m_alchemyI;

		// Token: 0x04003C18 RID: 15384
		[SerializeField]
		private AlchemySelectionUI.ToggleData m_alchemyII;

		// Token: 0x04003C1B RID: 15387
		private static AlchemyPowerLevelFlags m_levelFlags;

		// Token: 0x02000850 RID: 2128
		[Serializable]
		private class ToggleData
		{
			// Token: 0x17000E36 RID: 3638
			// (get) Token: 0x06003D74 RID: 15732 RVA: 0x00069A24 File Offset: 0x00067C24
			public SolToggle Toggle
			{
				get
				{
					return this.m_toggle;
				}
			}

			// Token: 0x06003D75 RID: 15733 RVA: 0x00069A2C File Offset: 0x00067C2C
			public void Init(AlchemySelectionUI controller)
			{
				this.m_controller = controller;
				this.m_retractedHeight = this.m_extender.sizeDelta.y;
				this.m_targetHeight = this.m_retractedHeight;
			}

			// Token: 0x06003D76 RID: 15734 RVA: 0x00182ACC File Offset: 0x00180CCC
			public void Update()
			{
				this.m_targetHeight = (this.m_toggle.isOn ? this.m_extendedHeight : this.m_retractedHeight);
				float num = Mathf.MoveTowards(this.m_extender.sizeDelta.y, this.m_targetHeight, Time.deltaTime * GlobalSettings.Values.Stance.StanceBubbleAnimationSpeed);
				if (this.m_extender.sizeDelta.y != num)
				{
					this.m_extender.sizeDelta = new Vector2(this.m_extender.sizeDelta.x, num);
				}
			}

			// Token: 0x06003D77 RID: 15735 RVA: 0x00182B60 File Offset: 0x00180D60
			public void SetToggleAvailable(int essenceCount, AlchemyPowerLevel alchemyPowerLevel, bool isAvailable)
			{
				if (isAvailable)
				{
					this.m_toggle.interactable = (essenceCount >= alchemyPowerLevel.GetRequiredEmberEssence());
					this.m_toggle.gameObject.SetActive(true);
					this.m_extender.gameObject.SetActive(true);
					return;
				}
				this.m_toggle.isOn = false;
				this.m_toggle.interactable = false;
				this.m_toggle.gameObject.SetActive(false);
				this.m_extender.gameObject.SetActive(false);
			}

			// Token: 0x04003C1D RID: 15389
			[SerializeField]
			private SolToggle m_toggle;

			// Token: 0x04003C1E RID: 15390
			[SerializeField]
			private RectTransform m_extender;

			// Token: 0x04003C1F RID: 15391
			[SerializeField]
			private float m_extendedHeight = 45f;

			// Token: 0x04003C20 RID: 15392
			private AlchemySelectionUI m_controller;

			// Token: 0x04003C21 RID: 15393
			private float m_retractedHeight;

			// Token: 0x04003C22 RID: 15394
			private float m_targetHeight;
		}
	}
}
