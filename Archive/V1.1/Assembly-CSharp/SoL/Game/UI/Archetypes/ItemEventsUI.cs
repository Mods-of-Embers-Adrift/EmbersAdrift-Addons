using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009CF RID: 2511
	public class ItemEventsUI : ArchetypeInstanceEventsUI<FixedCooldownUI>
	{
		// Token: 0x170010E1 RID: 4321
		// (get) Token: 0x06004C6E RID: 19566 RVA: 0x00073B14 File Offset: 0x00071D14
		protected override bool ShowTargetOverlay
		{
			get
			{
				return this.m_showTargetOverlay;
			}
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x00073B1C File Offset: 0x00071D1C
		protected override void ResetInternal()
		{
			base.ResetInternal();
			this.m_consumable = null;
			this.m_countItem = null;
			this.m_chargeItem = null;
			this.m_count = null;
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x00073B45 File Offset: 0x00071D45
		protected override bool EarlyOutOfUpdate()
		{
			return base.EarlyOutOfUpdate() || !this.m_consumable;
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x001BC974 File Offset: 0x001BAB74
		protected override void Subscribe()
		{
			base.Subscribe();
			if (base.m_instance == null || !base.m_instance.Archetype || !base.m_instance.IsItem || !LocalPlayer.GameEntity)
			{
				return;
			}
			this.ItemDataOnLockFlagsChanged(base.m_instance.ItemData.Locked);
			base.m_instance.ItemData.LockFlagsChanged += this.ItemDataOnLockFlagsChanged;
			this.m_showTargetOverlay = (base.m_instance.Archetype is ConsumableItemAppliable);
			if (base.m_instance.Archetype.TryGetAsType(out this.m_consumable))
			{
				LocalPlayer.GameEntity.SkillsController.LastConsumableConsumedChanged += this.SkillsControllerOnLastConsumableConsumedChanged;
				LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged += base.SkillsControllerOnPendingExecutionChanged;
				LocalPlayer.GameEntity.SkillsController.TriggerGlobalCooldown += base.SkillsControllerOnTriggerGlobalCooldown;
				LocalPlayer.GameEntity.VitalsReplicator.BehaviorFlags.Changed += this.BehaviorFlagsOnChanged;
				LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
				this.SkillsControllerOnLastConsumableConsumedChanged();
				this.BehaviorFlagsOnChanged(LocalPlayer.GameEntity.VitalsReplicator.BehaviorFlags.Value);
			}
			bool active = this.m_consumable != null;
			this.m_primary.gameObject.SetActive(active);
			this.m_execution.gameObject.SetActive(active);
			this.m_global.gameObject.SetActive(active);
			ItemArchetype itemArchetype;
			if (base.m_instance.Archetype.TryGetAsType(out itemArchetype))
			{
				if (itemArchetype.ArchetypeHasCount())
				{
					this.m_countItem = itemArchetype;
					this.ItemDataOnCountChanged();
					base.m_instance.ItemData.CountChanged += this.ItemDataOnCountChanged;
					return;
				}
				if (itemArchetype.ArchetypeHasCharges())
				{
					this.m_chargeItem = itemArchetype;
					this.ItemDataOnChargesChanged();
					base.m_instance.ItemData.ChargesChanged += this.ItemDataOnChargesChanged;
				}
			}
		}

		// Token: 0x06004C72 RID: 19570 RVA: 0x001BCB8C File Offset: 0x001BAD8C
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			if (base.m_instance == null || !base.m_instance.IsItem || !LocalPlayer.GameEntity)
			{
				return;
			}
			base.m_instance.ItemData.LockFlagsChanged -= this.ItemDataOnLockFlagsChanged;
			if (this.m_consumable)
			{
				if (LocalPlayer.GameEntity.SkillsController)
				{
					LocalPlayer.GameEntity.SkillsController.LastConsumableConsumedChanged -= this.SkillsControllerOnLastConsumableConsumedChanged;
					LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged -= base.SkillsControllerOnPendingExecutionChanged;
					LocalPlayer.GameEntity.SkillsController.TriggerGlobalCooldown -= base.SkillsControllerOnTriggerGlobalCooldown;
				}
				if (LocalPlayer.GameEntity.VitalsReplicator)
				{
					LocalPlayer.GameEntity.VitalsReplicator.BehaviorFlags.Changed -= this.BehaviorFlagsOnChanged;
					LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
			}
			if (this.m_countItem)
			{
				base.m_instance.ItemData.CountChanged -= this.ItemDataOnCountChanged;
			}
			if (this.m_chargeItem)
			{
				base.m_instance.ItemData.ChargesChanged -= this.ItemDataOnChargesChanged;
			}
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x00073B5F File Offset: 0x00071D5F
		private void BehaviorFlagsOnChanged(BehaviorEffectTypeFlags obj)
		{
			this.m_ui.BehaviorCanvas.alpha = ((obj.HasBitFlag(BehaviorEffectTypeFlags.Stunned) || obj.HasBitFlag(BehaviorEffectTypeFlags.Dazed)) ? 1f : 0f);
		}

		// Token: 0x06004C74 RID: 19572 RVA: 0x00073B8F File Offset: 0x00071D8F
		private void CurrentStanceOnChanged(Stance obj)
		{
			this.m_ui.RefreshDisabledPanel();
		}

		// Token: 0x06004C75 RID: 19573 RVA: 0x001BCCF4 File Offset: 0x001BAEF4
		private void SkillsControllerOnLastConsumableConsumedChanged()
		{
			if (!this.m_consumable)
			{
				return;
			}
			float elapsedSinceLastConsumable = LocalPlayer.GameEntity.SkillsController.GetElapsedSinceLastConsumable(this.m_consumable.ConsumableCategory);
			if (elapsedSinceLastConsumable < (float)this.m_consumable.Cooldown)
			{
				this.m_primary.StartCooldown(Time.time - elapsedSinceLastConsumable, (float)this.m_consumable.Cooldown);
				return;
			}
			this.m_primary.ClearCooldown();
		}

		// Token: 0x06004C76 RID: 19574 RVA: 0x00073B9C File Offset: 0x00071D9C
		private void ItemDataOnLockFlagsChanged(bool obj)
		{
			this.m_ui.LockedCanvas.alpha = (obj ? 1f : 0f);
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x001BCD64 File Offset: 0x001BAF64
		private void ItemDataOnChargesChanged()
		{
			this.m_ui.UpperRightLabel.text = ((base.m_instance.Archetype.ArchetypeHasCharges() && base.m_instance.ItemData.Charges != null) ? base.m_instance.ItemData.Charges.Value.ToString() : null);
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x001BCDD0 File Offset: 0x001BAFD0
		private void ItemDataOnCountChanged()
		{
			int? count = this.m_count;
			this.m_count = null;
			string text = string.Empty;
			if (base.m_instance.Archetype.ArchetypeHasCount() && base.m_instance.ItemData.Count != null)
			{
				this.m_count = new int?(base.m_instance.ItemData.Count.Value);
				text = this.m_count.Value.ToString();
				if (count != null && count.Value < this.m_count.Value && this.m_countChangedHighlight)
				{
					this.m_countChangedHighlight.gameObject.SetActive(true);
					this.m_countChangedHighlight.CustomCrossFadeAlpha(1f, 0f, 1.5f);
				}
			}
			this.m_ui.UpperRightLabel.text = text;
		}

		// Token: 0x04004662 RID: 18018
		private ConsumableItem m_consumable;

		// Token: 0x04004663 RID: 18019
		private ItemArchetype m_countItem;

		// Token: 0x04004664 RID: 18020
		private ItemArchetype m_chargeItem;

		// Token: 0x04004665 RID: 18021
		private int? m_count;

		// Token: 0x04004666 RID: 18022
		private bool m_showTargetOverlay = true;

		// Token: 0x04004667 RID: 18023
		[SerializeField]
		private Image m_countChangedHighlight;
	}
}
