using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005F0 RID: 1520
	public class PlayerVitals_LocalClient : PlayerVitals_Client
	{
		// Token: 0x06003028 RID: 12328 RVA: 0x00061439 File Offset: 0x0005F639
		protected override void InitInternal()
		{
			base.InitInternal();
			this.InitCurrentEffects();
		}

		// Token: 0x06003029 RID: 12329 RVA: 0x00158A84 File Offset: 0x00156C84
		protected override void Subscribe()
		{
			base.Subscribe();
			if (base.GameEntity && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Masteries != null)
			{
				base.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
			}
			if (base.PlayerReplicator)
			{
				base.PlayerReplicator.ArmorClass.Changed += this.ArmorClassOnChanged;
				base.PlayerReplicator.MaxArmorClass.Changed += this.MaxArmorClassOnChanged;
			}
		}

		// Token: 0x0600302A RID: 12330 RVA: 0x00158B2C File Offset: 0x00156D2C
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			if (base.GameEntity && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Masteries != null)
			{
				base.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
			}
			if (base.PlayerReplicator)
			{
				base.PlayerReplicator.ArmorClass.Changed -= this.ArmorClassOnChanged;
				base.PlayerReplicator.MaxArmorClass.Changed -= this.MaxArmorClassOnChanged;
			}
		}

		// Token: 0x0600302B RID: 12331 RVA: 0x00061447 File Offset: 0x0005F647
		protected override void RefreshStatPanel()
		{
			ClientGameManager.UIManager.StatPanel.UpdatePanels();
		}

		// Token: 0x0600302C RID: 12332 RVA: 0x00061458 File Offset: 0x0005F658
		protected override void RefreshArmorClassPanel()
		{
			ClientGameManager.UIManager.StatPanel.UpdateArmorClass();
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x00061469 File Offset: 0x0005F669
		protected override void RefreshArmorBudgetPanel()
		{
			ClientGameManager.UIManager.StatPanel.UpdateArmorBudget();
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x0006147A File Offset: 0x0005F67A
		protected override void CurrentHealthStateOnChanged(HealthState obj)
		{
			base.CurrentHealthStateOnChanged(obj);
			LocalPlayer.HealthStateUpdated(obj);
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x00158BD4 File Offset: 0x00156DD4
		protected override void EffectsOnChanged(SynchronizedCollection<UniqueId, EffectSyncData>.Operation op, UniqueId id, EffectSyncData previous, EffectSyncData current)
		{
			base.EffectsOnChanged(op, id, previous, current);
			bool flag = true;
			EffectSyncData? effectSyncData = null;
			switch (op)
			{
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Add:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Insert:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.InitialAdd:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.InitialAddFinal:
				effectSyncData = new EffectSyncData?(current);
				goto IL_71;
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.RemoveAt:
				effectSyncData = new EffectSyncData?(previous);
				flag = false;
				goto IL_71;
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Set:
				throw new NotImplementedException("No set yet!");
			}
			Debug.LogWarning(string.Format("Unimplemented Operation {0}", op));
			return;
			IL_71:
			if (effectSyncData != null)
			{
				CombatEffect combatEffect = effectSyncData.Value.CombatEffect;
				if (combatEffect != null)
				{
					combatEffect.ApplyStatusEffects(flag, base.GameEntity, effectSyncData.Value.ReagentItem, effectSyncData.Value.StackCount);
				}
				this.RefreshStatPanel();
				if (effectSyncData.Value.EffectSource is AuraAbility)
				{
					if (effectSyncData.Value.SourceNetworkId == LocalPlayer.NetworkEntity.NetworkId.Value)
					{
						if (flag)
						{
							LocalPlayer.ActiveAura = new EffectSyncData?(effectSyncData.Value);
						}
						else
						{
							LocalPlayer.ActiveAura = null;
						}
					}
					this.ToggleAuraAudio(effectSyncData.Value, flag);
				}
			}
		}

		// Token: 0x06003030 RID: 12336 RVA: 0x00158D0C File Offset: 0x00156F0C
		private void InitCurrentEffects()
		{
			for (int i = 0; i < base.PlayerReplicator.Effects.Count; i++)
			{
				EffectSyncData effectSyncData = base.PlayerReplicator.Effects[i];
				this.EffectsOnChanged(SynchronizedCollection<UniqueId, EffectSyncData>.Operation.InitialAdd, effectSyncData.InstanceId, default(EffectSyncData), effectSyncData);
			}
		}

		// Token: 0x06003031 RID: 12337 RVA: 0x00061489 File Offset: 0x0005F689
		protected override void EquipmentOnInstanceChanged(ArchetypeInstance instance, bool adding)
		{
			base.EquipmentOnInstanceChanged(instance, adding);
			ClientGameManager.UIManager.StatPanel.UpdatePanels();
		}

		// Token: 0x06003032 RID: 12338 RVA: 0x000614A2 File Offset: 0x0005F6A2
		protected override void MasteriesOnContentsChanged()
		{
			base.MasteriesOnContentsChanged();
			ClientGameManager.UIManager.StatPanel.UpdatePanels();
		}

		// Token: 0x06003033 RID: 12339 RVA: 0x00061458 File Offset: 0x0005F658
		private void MaxArmorClassOnChanged(int obj)
		{
			ClientGameManager.UIManager.StatPanel.UpdateArmorClass();
		}

		// Token: 0x06003034 RID: 12340 RVA: 0x00061458 File Offset: 0x0005F658
		private void ArmorClassOnChanged(int obj)
		{
			ClientGameManager.UIManager.StatPanel.UpdateArmorClass();
		}

		// Token: 0x06003035 RID: 12341 RVA: 0x00158D60 File Offset: 0x00156F60
		private void ToggleAuraAudio(EffectSyncData syncData, bool start)
		{
			NetworkEntity networkEntity;
			if (NetworkManager.EntityManager.TryGetNetworkEntity(syncData.SourceNetworkId, out networkEntity) && networkEntity.GameEntity && networkEntity.GameEntity.AudioController)
			{
				if (start)
				{
					AuraAbility auraAbility;
					if (InternalGameDatabase.Archetypes.TryGetAsType<AuraAbility>(syncData.ArchetypeId, out auraAbility))
					{
						networkEntity.GameEntity.AudioController.StartAuraAudio(auraAbility);
						return;
					}
				}
				else
				{
					networkEntity.GameEntity.AudioController.StopAuraAudio();
				}
			}
		}
	}
}
