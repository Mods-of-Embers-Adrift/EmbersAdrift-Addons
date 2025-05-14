using System;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x0200084C RID: 2124
	[Obsolete]
	public class AbilityPanelUI : ContainerUI<int, ActionBarSlotUI>
	{
		// Token: 0x06003D3A RID: 15674 RVA: 0x000697A2 File Offset: 0x000679A2
		protected override void Awake()
		{
			base.Awake();
			this.m_masteryPanel = base.gameObject.GetComponent<MasteryPanelUI>();
		}

		// Token: 0x06003D3B RID: 15675 RVA: 0x00182180 File Offset: 0x00180380
		protected override void InitializeSlots()
		{
			this.m_slots = new DictionaryList<int, ActionBarSlotUI>(false);
			ActionBarUI actionBar = ClientGameManager.UIManager.ActionBar;
			for (int i = 0; i < actionBar.ActionBarSlots.Length; i++)
			{
				actionBar.ActionBarSlots[i].Initialize(this, i);
				this.m_slots.Add(i, actionBar.ActionBarSlots[i]);
			}
		}

		// Token: 0x06003D3C RID: 15676 RVA: 0x001821DC File Offset: 0x001803DC
		public override void AddInstance(ArchetypeInstance instance)
		{
			ActionBarSlotUI actionBarSlotUI;
			if (instance.Index > -1 && this.m_slots.TryGetValue(instance.Index, out actionBarSlotUI))
			{
				actionBarSlotUI.InstanceAdded(instance);
			}
			AbilityArchetype abilityArchetype;
			MasteryPanelSlotUI masteryPanelSlotUI;
			if (instance.Archetype.TryGetAsType(out abilityArchetype) && this.m_masteryPanel.TryGetMasteryUI(abilityArchetype.Mastery.Id, out masteryPanelSlotUI))
			{
				masteryPanelSlotUI.AbilityAdded(instance);
			}
		}

		// Token: 0x06003D3D RID: 15677 RVA: 0x00182240 File Offset: 0x00180440
		public override void RemoveInstance(ArchetypeInstance instance)
		{
			ActionBarSlotUI actionBarSlotUI;
			if (instance.Index > -1 && this.m_slots.TryGetValue(instance.Index, out actionBarSlotUI))
			{
				actionBarSlotUI.InstanceRemoved(instance);
				return;
			}
			int index = instance.Index;
		}

		// Token: 0x06003D3E RID: 15678 RVA: 0x000697BB File Offset: 0x000679BB
		protected override void LeftInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
			if (instance.Index > -1)
			{
				LocalPlayer.GameEntity.SkillsController.BeginExecution(instance);
			}
		}

		// Token: 0x06003D3F RID: 15679 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void RightInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003D40 RID: 15680 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void MiddleInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x04003C0B RID: 15371
		private MasteryPanelUI m_masteryPanel;
	}
}
