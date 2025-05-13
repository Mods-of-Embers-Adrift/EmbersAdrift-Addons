using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200092F RID: 2351
	public class SkillsAbilityUI : ContainerUI<int, ActionBarSlotUI>
	{
		// Token: 0x17000F85 RID: 3973
		// (get) Token: 0x06004524 RID: 17700 RVA: 0x0019E9C4 File Offset: 0x0019CBC4
		private Dictionary<UniqueId, AbilitySlot> LocalSlots
		{
			get
			{
				if (this.m_localSlots == null)
				{
					this.m_localSlots = new Dictionary<UniqueId, AbilitySlot>(default(UniqueIdComparer));
				}
				return this.m_localSlots;
			}
		}

		// Token: 0x06004525 RID: 17701 RVA: 0x0006EA5D File Offset: 0x0006CC5D
		protected override void Awake()
		{
			base.Awake();
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			this.m_masteryUi.SelectedMasteryUpdated += this.MasteryUiOnSelectedMasteryUpdated;
		}

		// Token: 0x06004526 RID: 17702 RVA: 0x0019E9F8 File Offset: 0x0019CBF8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.m_masteryUi.SelectedMasteryUpdated -= this.MasteryUiOnSelectedMasteryUpdated;
			BaseCollectionController.InteractiveStationChanged -= this.RefreshLocks;
			if (LocalPlayer.GameEntity != null)
			{
				if (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Abilities != null)
				{
					LocalPlayer.GameEntity.CollectionController.Abilities.ContentsChanged -= this.AbilitiesOnContentsChanged;
				}
				if (LocalPlayer.GameEntity.CharacterData != null)
				{
					LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed -= this.AdventuringLevelSyncOnChanged;
				}
			}
			if (GameManager.QuestManager)
			{
				GameManager.QuestManager.QuestUpdated -= this.QuestManagerOnQuestUpdated;
			}
		}

		// Token: 0x06004527 RID: 17703 RVA: 0x0019EAE4 File Offset: 0x0019CCE4
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			BaseCollectionController.InteractiveStationChanged += this.RefreshLocks;
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Abilities != null)
				{
					LocalPlayer.GameEntity.CollectionController.Abilities.ContentsChanged += this.AbilitiesOnContentsChanged;
				}
				if (LocalPlayer.GameEntity.CharacterData != null)
				{
					LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed += this.AdventuringLevelSyncOnChanged;
				}
			}
			if (GameManager.QuestManager)
			{
				GameManager.QuestManager.QuestUpdated += this.QuestManagerOnQuestUpdated;
			}
			this.RefreshAbilities();
			foreach (KeyValuePair<UniqueId, AbilitySlot> keyValuePair in this.m_localSlots)
			{
				keyValuePair.Value.RefreshLock();
			}
		}

		// Token: 0x06004528 RID: 17704 RVA: 0x0006EA8D File Offset: 0x0006CC8D
		private void QuestManagerOnQuestUpdated(ObjectiveIterationCache obj)
		{
			if (obj.QuestId == GlobalSettings.Values.Ashen.AlchemyQuestId)
			{
				this.RefreshAlchemyDisplays();
			}
		}

		// Token: 0x06004529 RID: 17705 RVA: 0x0006EAB1 File Offset: 0x0006CCB1
		private void AdventuringLevelSyncOnChanged(byte obj)
		{
			this.RefreshAlchemyDisplays();
		}

		// Token: 0x0600452A RID: 17706 RVA: 0x0006EAB9 File Offset: 0x0006CCB9
		private void AbilitiesOnContentsChanged()
		{
			this.RefreshAbilities();
		}

		// Token: 0x0600452B RID: 17707 RVA: 0x0019EBF0 File Offset: 0x0019CDF0
		private void RefreshAlchemyDisplays()
		{
			foreach (KeyValuePair<UniqueId, AbilitySlot> keyValuePair in this.m_localSlots)
			{
				keyValuePair.Value.RefreshAlchemyDisplay();
			}
		}

		// Token: 0x0600452C RID: 17708 RVA: 0x0019EC48 File Offset: 0x0019CE48
		protected override void InitializeSlots()
		{
			this.m_slots = new DictionaryList<int, ActionBarSlotUI>(false);
			ActionBarUI actionBar = ClientGameManager.UIManager.ActionBar;
			for (int i = 0; i < actionBar.ActionBarSlots.Length; i++)
			{
				actionBar.ActionBarSlots[i].Initialize(this, i);
				this.m_slots.Add(i, actionBar.ActionBarSlots[i]);
			}
			actionBar.AutoAttackSlot.Initialize(this, 1024);
			this.MasteryUiOnSelectedMasteryUpdated(this.m_masteryUi.CurrentInstance);
		}

		// Token: 0x0600452D RID: 17709 RVA: 0x0019ECC4 File Offset: 0x0019CEC4
		private bool HasMatchingSlot(ArchetypeInstance instance, out AbilitySlot localSlot)
		{
			localSlot = null;
			return this.m_masteryUi.CurrentInstance != null && instance.Ability != null && instance.Ability.Mastery.Id.Equals(this.m_masteryUi.CurrentInstance.Archetype.Id) && this.LocalSlots.TryGetValue(instance.Ability.Id, out localSlot);
		}

		// Token: 0x0600452E RID: 17710 RVA: 0x0019ED38 File Offset: 0x0019CF38
		public override void AddInstance(ArchetypeInstance instance)
		{
			AutoAttackAbility autoAttackAbility;
			if (instance.Archetype.TryGetAsType(out autoAttackAbility))
			{
				ClientGameManager.UIManager.ActionBar.AutoAttackSlot.InstanceAdded(instance);
				return;
			}
			AbilitySlot abilitySlot;
			bool flag = this.HasMatchingSlot(instance, out abilitySlot);
			if (abilitySlot != null)
			{
				abilitySlot.AbilityInstance = instance;
			}
			ActionBarSlotUI actionBarSlotUI;
			if (instance.Index > -1 && this.m_slots.TryGetValue(instance.Index, out actionBarSlotUI))
			{
				actionBarSlotUI.InstanceAdded(instance);
				return;
			}
			(flag ? abilitySlot.ContainerSlotUi : this.m_hiddenSlot).InstanceAdded(instance);
		}

		// Token: 0x0600452F RID: 17711 RVA: 0x0019EDC4 File Offset: 0x0019CFC4
		public override void RemoveInstance(ArchetypeInstance instance)
		{
			ActionBarSlotUI actionBarSlotUI;
			if (instance.Index > -1 && this.m_slots.TryGetValue(instance.Index, out actionBarSlotUI))
			{
				actionBarSlotUI.InstanceRemoved(instance);
			}
			else
			{
				int index = instance.Index;
			}
			AbilitySlot abilitySlot;
			if (this.HasMatchingSlot(instance, out abilitySlot))
			{
				abilitySlot.AbilityInstance = null;
			}
		}

		// Token: 0x06004530 RID: 17712 RVA: 0x0006EAC1 File Offset: 0x0006CCC1
		private void RefreshMastery()
		{
			this.MasteryUiOnSelectedMasteryUpdated(this.m_masteryUi.CurrentInstance);
		}

		// Token: 0x17000F86 RID: 3974
		// (get) Token: 0x06004531 RID: 17713 RVA: 0x0006EAD4 File Offset: 0x0006CCD4
		// (set) Token: 0x06004532 RID: 17714 RVA: 0x0019EE14 File Offset: 0x0019D014
		private ArchetypeInstance MasteryInstance
		{
			get
			{
				return this.m_masteryInstance;
			}
			set
			{
				if (this.m_masteryInstance == value)
				{
					return;
				}
				ArchetypeInstance masteryInstance = this.m_masteryInstance;
				if (((masteryInstance != null) ? masteryInstance.MasteryData : null) != null)
				{
					this.m_masteryInstance.MasteryData.MasteryDataChanged -= this.RefreshSpecializationPanel;
				}
				this.m_masteryInstance = value;
				ArchetypeInstance masteryInstance2 = this.m_masteryInstance;
				if (((masteryInstance2 != null) ? masteryInstance2.MasteryData : null) != null)
				{
					this.m_masteryInstance.MasteryData.MasteryDataChanged += this.RefreshSpecializationPanel;
				}
				this.RefreshSpecializationPanel();
			}
		}

		// Token: 0x06004533 RID: 17715 RVA: 0x0019EE98 File Offset: 0x0019D098
		private void RefreshSpecializationPanel()
		{
			if (this.m_specializationPanel)
			{
				this.m_specializationPanel.SetActive(this.MasteryInstance != null && this.MasteryInstance.MasteryData != null && this.MasteryInstance.MasteryData.Specialization != null);
			}
		}

		// Token: 0x06004534 RID: 17716 RVA: 0x0006EADC File Offset: 0x0006CCDC
		private void MasteryUiOnSelectedMasteryUpdated(ArchetypeInstance obj)
		{
			this.m_specializationChoiceController.InitializeMasteryChoice(obj);
			this.m_baseLevelPanel.SetActive(obj != null);
			this.MasteryInstance = obj;
			this.UpdateAbilitiesFromMastery();
			this.RefreshAbilities();
		}

		// Token: 0x06004535 RID: 17717 RVA: 0x0019EEF0 File Offset: 0x0019D0F0
		private void UpdateAbilitiesFromMastery()
		{
			this.LocalSlots.Clear();
			for (int i = 0; i < this.m_abilityRows.Length; i++)
			{
				bool active = this.m_abilityRows[i].UpdateAbilitiesForMastery(this.MasteryInstance, this.LocalSlots);
				this.m_abilityRowParents[i].SetActive(active);
			}
		}

		// Token: 0x06004536 RID: 17718 RVA: 0x0019EF44 File Offset: 0x0019D144
		private void RefreshAbilities()
		{
			if (this.m_container == null)
			{
				return;
			}
			this.UpdateAbilitiesFromMastery();
			foreach (ArchetypeInstance archetypeInstance in this.m_container.Instances)
			{
				if (archetypeInstance != null && !(archetypeInstance.Ability == null) && !(archetypeInstance.Ability.Mastery == null) && !(archetypeInstance.InstanceUI == null))
				{
					AbilitySlot abilitySlot;
					if (this.LocalSlots.TryGetValue(archetypeInstance.Ability.Id, out abilitySlot))
					{
						abilitySlot.AbilityInstance = archetypeInstance;
					}
					if (archetypeInstance.Index < 0)
					{
						((abilitySlot != null) ? abilitySlot.ContainerSlotUi : this.m_hiddenSlot).InstanceAdded(archetypeInstance);
					}
				}
			}
		}

		// Token: 0x06004537 RID: 17719 RVA: 0x00183AFC File Offset: 0x00181CFC
		protected override void LeftInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			if (instance.InstanceUI != null && instance.InstanceUI.Locked)
			{
				Debug.Log("LOCKED! " + instance.AbilityData.CooldownFlags.ToString());
				return;
			}
			if (instance.InstanceUI != null && !instance.InstanceUI.Locked && instance.Index > -1)
			{
				LocalPlayer.GameEntity.SkillsController.BeginExecution(instance);
			}
		}

		// Token: 0x06004538 RID: 17720 RVA: 0x00069E00 File Offset: 0x00068000
		protected override void RightInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			this.LeftInstanceClicked(eventData, instance);
		}

		// Token: 0x06004539 RID: 17721 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void MiddleInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x0600453A RID: 17722 RVA: 0x00069E00 File Offset: 0x00068000
		protected override void LeftInstanceDoubleClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			this.LeftInstanceClicked(eventData, instance);
		}

		// Token: 0x0600453B RID: 17723 RVA: 0x0019F020 File Offset: 0x0019D220
		public void TriggerIndex(int index)
		{
			ActionBarSlotUI actionBarSlotUI;
			if (this.m_slots != null && this.m_slots.TryGetValue(index, out actionBarSlotUI) && actionBarSlotUI && actionBarSlotUI.Instance != null)
			{
				this.LeftInstanceClicked(null, actionBarSlotUI.Instance);
			}
		}

		// Token: 0x0600453C RID: 17724 RVA: 0x0019F064 File Offset: 0x0019D264
		public void RefreshLocks()
		{
			for (int i = 0; i < this.m_abilityColumns.Length; i++)
			{
				this.m_abilityColumns[i].TriggerLockRefresh(null);
			}
			for (int j = 0; j < this.m_abilityRows.Length; j++)
			{
				this.m_abilityRows[j].TriggerLockRefresh(null);
			}
		}

		// Token: 0x04004199 RID: 16793
		[SerializeField]
		private SkillsMasteryUI m_masteryUi;

		// Token: 0x0400419A RID: 16794
		[SerializeField]
		private RectTransform m_nonFocusedRect;

		// Token: 0x0400419B RID: 16795
		[SerializeField]
		private AbilityColumn[] m_abilityColumns;

		// Token: 0x0400419C RID: 16796
		[SerializeField]
		private AbilityRow[] m_abilityRows;

		// Token: 0x0400419D RID: 16797
		[SerializeField]
		private GameObject[] m_abilityRowParents;

		// Token: 0x0400419E RID: 16798
		[SerializeField]
		private ContainerSlotUI m_hiddenSlot;

		// Token: 0x0400419F RID: 16799
		[SerializeField]
		private SpecializationChoiceController m_specializationChoiceController;

		// Token: 0x040041A0 RID: 16800
		[SerializeField]
		private GameObject m_baseLevelPanel;

		// Token: 0x040041A1 RID: 16801
		[SerializeField]
		private GameObject m_specializationPanel;

		// Token: 0x040041A2 RID: 16802
		private Dictionary<UniqueId, AbilitySlot> m_localSlots;

		// Token: 0x040041A3 RID: 16803
		private ArchetypeInstance m_masteryInstance;
	}
}
