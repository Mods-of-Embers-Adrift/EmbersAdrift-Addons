using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x0200085E RID: 2142
	public class CodexAbilityUI : ContainerUI<int, ActionBarSlotUI>
	{
		// Token: 0x06003DD5 RID: 15829 RVA: 0x00069DE7 File Offset: 0x00067FE7
		protected override void Awake()
		{
			base.Awake();
			this.m_masteryUI = base.gameObject.GetComponent<CodexMasteryUI>();
		}

		// Token: 0x06003DD6 RID: 15830 RVA: 0x00183998 File Offset: 0x00181B98
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
		}

		// Token: 0x06003DD7 RID: 15831 RVA: 0x00183A04 File Offset: 0x00181C04
		public override void AddInstance(ArchetypeInstance instance)
		{
			AutoAttackAbility autoAttackAbility;
			if (instance.Archetype.TryGetAsType(out autoAttackAbility))
			{
				ClientGameManager.UIManager.ActionBar.AutoAttackSlot.InstanceAdded(instance);
				return;
			}
			ActionBarSlotUI actionBarSlotUI;
			if (instance.Index > -1 && this.m_slots.TryGetValue(instance.Index, out actionBarSlotUI))
			{
				actionBarSlotUI.InstanceAdded(instance);
			}
			AbilityArchetype abilityArchetype;
			MasteryAbilityContainerUI masteryAbilityContainerUI;
			if (instance.Archetype.TryGetAsType(out abilityArchetype) && this.m_masteryUI.TryGetMasteryUI(abilityArchetype.Mastery.Id, out masteryAbilityContainerUI))
			{
				masteryAbilityContainerUI.AbilityAdded(instance);
			}
		}

		// Token: 0x06003DD8 RID: 15832 RVA: 0x00183A8C File Offset: 0x00181C8C
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
			AbilityArchetype abilityArchetype;
			MasteryAbilityContainerUI masteryAbilityContainerUI;
			if (instance.Archetype.TryGetAsType(out abilityArchetype) && this.m_masteryUI.TryGetMasteryUI(abilityArchetype.Mastery.Id, out masteryAbilityContainerUI))
			{
				masteryAbilityContainerUI.AbilityRemoved(instance);
			}
		}

		// Token: 0x06003DD9 RID: 15833 RVA: 0x00183AFC File Offset: 0x00181CFC
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

		// Token: 0x06003DDA RID: 15834 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void RightInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003DDB RID: 15835 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void MiddleInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003DDC RID: 15836 RVA: 0x00069E00 File Offset: 0x00068000
		protected override void LeftInstanceDoubleClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			this.LeftInstanceClicked(eventData, instance);
		}

		// Token: 0x06003DDD RID: 15837 RVA: 0x00183B80 File Offset: 0x00181D80
		public void TriggerIndex(int index)
		{
			ActionBarSlotUI actionBarSlotUI;
			if (this.m_slots.TryGetValue(index, out actionBarSlotUI) && actionBarSlotUI.Instance != null)
			{
				this.LeftInstanceClicked(null, actionBarSlotUI.Instance);
			}
		}

		// Token: 0x04003C51 RID: 15441
		private CodexMasteryUI m_masteryUI;
	}
}
