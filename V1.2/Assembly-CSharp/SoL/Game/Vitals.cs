using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005F7 RID: 1527
	public abstract class Vitals : GameEntityComponent
	{
		// Token: 0x140000A7 RID: 167
		// (add) Token: 0x06003085 RID: 12421 RVA: 0x0015A548 File Offset: 0x00158748
		// (remove) Token: 0x06003086 RID: 12422 RVA: 0x0015A57C File Offset: 0x0015877C
		public static event Action<NetworkEntity> RefreshEffectPanelForEntity;

		// Token: 0x06003087 RID: 12423 RVA: 0x0015A5B0 File Offset: 0x001587B0
		public static int GetMaxHealth(int constitution)
		{
			int num = constitution * 2;
			if (constitution >= 16)
			{
				num += 5;
			}
			if (constitution >= 18)
			{
				num += 10;
			}
			if (constitution >= 20)
			{
				num += 15;
			}
			return num;
		}

		// Token: 0x06003088 RID: 12424 RVA: 0x0006190D File Offset: 0x0005FB0D
		public static int GetBonus(int stat, int perPointIncrease, int baseValue)
		{
			return (stat - 10) * perPointIncrease + baseValue;
		}

		// Token: 0x06003089 RID: 12425 RVA: 0x00061917 File Offset: 0x0005FB17
		private static float GetMaxTargetDistance(int perception)
		{
			return (float)Vitals.GetBonus(perception, 2, 40);
		}

		// Token: 0x0600308A RID: 12426 RVA: 0x00061923 File Offset: 0x0005FB23
		public Stance GetCurrentStance()
		{
			if (!this.m_replicator)
			{
				return Stance.Idle;
			}
			return this.m_replicator.CurrentStance.Value;
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x00061944 File Offset: 0x0005FB44
		public HealthState GetCurrentHealthState()
		{
			if (!this.m_replicator)
			{
				return HealthState.None;
			}
			return this.m_replicator.CurrentHealthState.Value;
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x0600308C RID: 12428 RVA: 0x00061965 File Offset: 0x0005FB65
		// (set) Token: 0x0600308D RID: 12429 RVA: 0x00061977 File Offset: 0x0005FB77
		public virtual Stance Stance
		{
			get
			{
				return this.m_replicator.CurrentStance.Value;
			}
			set
			{
				if (base.GameEntity.NetworkEntity.IsLocal)
				{
					this.m_replicator.CurrentStance.Value = value;
				}
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x0600308E RID: 12430 RVA: 0x000612B2 File Offset: 0x0005F4B2
		public float HealthPercent
		{
			get
			{
				return this.Health / (float)this.MaxHealth;
			}
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x0600308F RID: 12431 RVA: 0x0006199C File Offset: 0x0005FB9C
		public float MaxTargetDistance
		{
			get
			{
				return this.m_maxTargetDistance;
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06003090 RID: 12432 RVA: 0x000619A4 File Offset: 0x0005FBA4
		public float MaxTargetDistanceSqr
		{
			get
			{
				return this.m_maxTargetDistanceSqr;
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06003091 RID: 12433 RVA: 0x000619AC File Offset: 0x0005FBAC
		public DateTime LastCombatTimestamp
		{
			get
			{
				return this.m_lastCombatTimestamp;
			}
		}

		// Token: 0x140000A8 RID: 168
		// (add) Token: 0x06003092 RID: 12434 RVA: 0x0015A5E0 File Offset: 0x001587E0
		// (remove) Token: 0x06003093 RID: 12435 RVA: 0x0015A618 File Offset: 0x00158818
		public event Action StatsChanged;

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06003094 RID: 12436 RVA: 0x000619B4 File Offset: 0x0005FBB4
		protected VitalsReplicator m_replicator
		{
			get
			{
				return base.GameEntity.VitalsReplicator;
			}
		}

		// Token: 0x06003095 RID: 12437 RVA: 0x000619C1 File Offset: 0x0005FBC1
		private void Awake()
		{
			base.GameEntity.Vitals = this;
		}

		// Token: 0x06003096 RID: 12438 RVA: 0x0015A650 File Offset: 0x00158850
		protected virtual void OnDestroy()
		{
			this.Unsubscribe();
			StatTypeExtensions.ReturnStatTypeCollection(this.m_baseStats);
			StatTypeExtensions.ReturnStatTypeCollection(this.m_roleStats);
			StatTypeExtensions.ReturnStatTypeCollection(this.m_combatStats);
			StatTypeExtensions.ReturnStatTypeCollection(this.m_weaponStats);
			StatTypeExtensions.ReturnStatTypeCollection(this.m_transientStats);
			StatTypeExtensions.ReturnStatTypeCollection(this.m_armorAugmentStats);
			this.m_baseStats = null;
			this.m_roleStats = null;
			this.m_combatStats = null;
			this.m_weaponStats = null;
			this.m_transientStats = null;
			this.m_armorAugmentStats = null;
			StaticDictionaryPool<SetBonusProfile, int>.ReturnToPool(this.m_armorSetBonuses);
			this.m_armorSetBonuses = null;
			if (this.m_lastingVfx != null)
			{
				foreach (KeyValuePair<UniqueId, PooledVFX> keyValuePair in this.m_lastingVfx)
				{
					if (keyValuePair.Value != null)
					{
						keyValuePair.Value.ReturnToPool();
					}
				}
				this.m_lastingVfx.Clear();
			}
			if (NullifyMemoryLeakSettings.CleanVitals)
			{
				Vitals.CombatHandStats weaponSetActive = this.m_weaponSetActive;
				if (weaponSetActive != null)
				{
					weaponSetActive.CleanupReferences();
				}
				this.m_weaponSetActive = null;
				Vitals.CombatHandStats weaponSetPrimary = this.m_weaponSetPrimary;
				if (weaponSetPrimary != null)
				{
					weaponSetPrimary.CleanupReferences();
				}
				this.m_weaponSetPrimary = null;
				Vitals.CombatHandStats weaponSetSecondary = this.m_weaponSetSecondary;
				if (weaponSetSecondary != null)
				{
					weaponSetSecondary.CleanupReferences();
				}
				this.m_weaponSetSecondary = null;
			}
		}

		// Token: 0x06003097 RID: 12439 RVA: 0x0015A7A0 File Offset: 0x001589A0
		protected virtual void Subscribe()
		{
			if (GameManager.IsServer || (base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal))
			{
				if (this.m_equipment != null)
				{
					this.m_equipment.InstanceAdded += this.EquipmentOnInstanceAdded;
					this.m_equipment.InstanceRemoved += this.EquipmentOnInstanceRemoved;
				}
				if (this.m_replicator)
				{
					this.m_replicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
				}
				if (base.GameEntity.CharacterData)
				{
					base.GameEntity.CharacterData.HandConfigurationChanged += this.CharacterDataOnHandConfigurationChanged;
				}
			}
			if (!GameManager.IsServer && this.m_replicator)
			{
				this.m_replicator.Effects.Changed += this.EffectsOnChanged;
			}
		}

		// Token: 0x06003098 RID: 12440 RVA: 0x0015A8B0 File Offset: 0x00158AB0
		protected virtual void Unsubscribe()
		{
			if (GameManager.IsServer || (base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal))
			{
				if (this.m_equipment != null)
				{
					this.m_equipment.InstanceAdded -= this.EquipmentOnInstanceAdded;
					this.m_equipment.InstanceRemoved -= this.EquipmentOnInstanceRemoved;
				}
				if (this.m_replicator)
				{
					this.m_replicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
				if (base.GameEntity.CharacterData)
				{
					base.GameEntity.CharacterData.HandConfigurationChanged -= this.CharacterDataOnHandConfigurationChanged;
				}
			}
			if (!GameManager.IsServer && this.m_replicator)
			{
				this.m_replicator.Effects.Changed -= this.EffectsOnChanged;
			}
		}

		// Token: 0x06003099 RID: 12441 RVA: 0x0015A9C0 File Offset: 0x00158BC0
		public void Init(CharacterRecord record)
		{
			this.m_record = record;
			this.InitInternal();
			if (this.m_equipmentEnumerable != null)
			{
				foreach (ArchetypeInstance instance in this.m_equipmentEnumerable)
				{
					this.EquipmentOnInstanceAdded(instance);
				}
			}
			this.Subscribe();
			this.m_initialized = true;
		}

		// Token: 0x0600309A RID: 12442 RVA: 0x000619CF File Offset: 0x0005FBCF
		public void InitRemote()
		{
			this.Subscribe();
		}

		// Token: 0x0600309B RID: 12443 RVA: 0x0015AA30 File Offset: 0x00158C30
		protected virtual void InitInternal()
		{
			this.m_baseStats = StatTypeExtensions.GetStatTypeCollection();
			this.m_roleStats = StatTypeExtensions.GetStatTypeCollection();
			this.m_combatStats = StatTypeExtensions.GetStatTypeCollection();
			this.m_weaponStats = StatTypeExtensions.GetStatTypeCollection();
			this.m_transientStats = StatTypeExtensions.GetStatTypeCollection();
			this.m_armorAugmentStats = StatTypeExtensions.GetStatTypeCollection();
			this.m_armorSetBonuses = StaticDictionaryPool<SetBonusProfile, int>.GetFromPool();
			this.m_weaponSetPrimary = new Vitals.CombatHandStats(true, this);
			this.m_weaponSetSecondary = new Vitals.CombatHandStats(false, this);
			this.RefreshActiveWeaponSet();
			this.RefreshShieldArmor();
			this.RefreshVitals();
			this.RefreshRoleStats();
			this.RefreshAugmentStats(false);
			if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.TryGetInstance(ContainerType.Equipment, out this.m_equipment))
			{
				this.m_equipmentEnumerable = this.m_equipment.Instances;
			}
		}

		// Token: 0x0600309C RID: 12444 RVA: 0x000619D7 File Offset: 0x0005FBD7
		public virtual void FinalizeExternal()
		{
			this.m_finalized = true;
		}

		// Token: 0x0600309D RID: 12445 RVA: 0x000619E0 File Offset: 0x0005FBE0
		public void RefreshStats()
		{
			this.RefreshVitals();
			this.RefreshRoleStats();
			this.RefreshAugmentStats(false);
			this.RefreshArmorBudget();
			Action statsChanged = this.StatsChanged;
			if (statsChanged == null)
			{
				return;
			}
			statsChanged();
		}

		// Token: 0x0600309E RID: 12446 RVA: 0x00061A0B File Offset: 0x0005FC0B
		protected virtual void MasteriesOnContentsChanged()
		{
			this.RefreshStats();
		}

		// Token: 0x0600309F RID: 12447 RVA: 0x0015AAFC File Offset: 0x00158CFC
		private bool TryGetSpecializationModifiers(ArchetypeInstance masteryInstance, out IStatModifier modifiers)
		{
			modifiers = null;
			return masteryInstance != null && masteryInstance.MasteryData != null && masteryInstance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<IStatModifier>(masteryInstance.MasteryData.Specialization.Value, out modifiers);
		}

		// Token: 0x060030A0 RID: 12448 RVA: 0x0015AB50 File Offset: 0x00158D50
		private void RefreshVitals()
		{
			if (!base.GameEntity || base.GameEntity.CollectionController == null || base.GameEntity.CollectionController.Masteries == null)
			{
				return;
			}
			float f = 0f;
			float f2 = 0f;
			foreach (ArchetypeInstance archetypeInstance in base.GameEntity.CollectionController.Masteries.Instances)
			{
				int associatedLevelInteger = archetypeInstance.GetAssociatedLevelInteger(base.GameEntity);
				IStatModifier modifiers;
				if (archetypeInstance.Archetype.TryGetAsType(out modifiers))
				{
					this.RefreshVitalsInternal(modifiers, (float)associatedLevelInteger, ref f, ref f2);
				}
				if (this.TryGetSpecializationModifiers(archetypeInstance, out modifiers))
				{
					this.RefreshVitalsInternal(modifiers, (float)associatedLevelInteger, ref f, ref f2);
				}
			}
			this.UpdateMaxHealth(70 + Mathf.FloorToInt(f));
			this.m_maxTargetDistance = 40f;
			this.m_maxTargetDistanceSqr = this.m_maxTargetDistance * this.m_maxTargetDistance;
			this.ArmorWeightCapacity = Mathf.FloorToInt(f2);
		}

		// Token: 0x060030A1 RID: 12449 RVA: 0x0015AC60 File Offset: 0x00158E60
		private void RefreshVitalsInternal(IStatModifier modifiers, float level, ref float healthBonus, ref float armorWeightCapacity)
		{
			if (modifiers == null || modifiers.Vitals == null)
			{
				return;
			}
			for (int i = 0; i < modifiers.Vitals.Length; i++)
			{
				VitalScalingValue vitalScalingValue = modifiers.Vitals[i];
				VitalType type = vitalScalingValue.Type;
				if (type != VitalType.Health)
				{
					if (type == VitalType.ArmorWeightCapacity)
					{
						armorWeightCapacity += (float)vitalScalingValue.GetValueForLevel(level);
					}
				}
				else
				{
					healthBonus += (float)vitalScalingValue.GetValueForLevel(level);
				}
			}
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateMaxHealth(int value)
		{
		}

		// Token: 0x060030A3 RID: 12451
		public abstract float GetHealthPercent();

		// Token: 0x060030A4 RID: 12452
		public abstract float GetArmorClassPercent();

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x060030A5 RID: 12453
		public abstract float Health { get; }

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x060030A6 RID: 12454
		public abstract float HealthWound { get; }

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x060030A7 RID: 12455
		public abstract int MaxHealth { get; }

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x060030A8 RID: 12456
		public abstract float Stamina { get; }

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x060030A9 RID: 12457
		public abstract float StaminaWound { get; }

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x060030AA RID: 12458
		public abstract int ArmorClass { get; }

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x060030AB RID: 12459
		public abstract int MaxArmorClass { get; }

		// Token: 0x060030AC RID: 12460 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AlterHealth(float delta)
		{
		}

		// Token: 0x060030AD RID: 12461 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AlterStamina(float delta)
		{
		}

		// Token: 0x060030AE RID: 12462 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AbsorbDamage(float delta)
		{
		}

		// Token: 0x060030AF RID: 12463 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AlterHealthWound(float delta)
		{
		}

		// Token: 0x060030B0 RID: 12464 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AlterStaminaWound(float delta)
		{
		}

		// Token: 0x060030B1 RID: 12465 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void TriggerUndonesWrath(GameEntity sourceEntity)
		{
		}

		// Token: 0x060030B2 RID: 12466 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void CancelSpawnNoTarget()
		{
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x060030B3 RID: 12467 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ArmorScalesWithPlayerLevel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060030B4 RID: 12468 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual int GetScaledArmorClass(int playerLevel)
		{
			return 0;
		}

		// Token: 0x060030B5 RID: 12469 RVA: 0x00061A13 File Offset: 0x0005FC13
		public virtual float GetMaxAbsorbDamageReduction()
		{
			return 0.5f;
		}

		// Token: 0x060030B6 RID: 12470 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ApplyMaxArmorClassDelta(int delta)
		{
		}

		// Token: 0x060030B7 RID: 12471 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ApplyArmorClassDelta(int delta)
		{
		}

		// Token: 0x060030B8 RID: 12472 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void RecalculateTotalArmorClass()
		{
		}

		// Token: 0x060030B9 RID: 12473 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AddCampfireEffect(CampfireEffectApplicator applicator)
		{
		}

		// Token: 0x060030BA RID: 12474 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void RemoveCampfireEffect(CampfireEffectApplicator applicator)
		{
		}

		// Token: 0x060030BB RID: 12475 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void TakeFallDamage(float distanceFallen)
		{
		}

		// Token: 0x060030BC RID: 12476 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RefreshStatPanel()
		{
		}

		// Token: 0x060030BD RID: 12477 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RefreshArmorClassPanel()
		{
		}

		// Token: 0x060030BE RID: 12478 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RefreshArmorBudgetPanel()
		{
		}

		// Token: 0x060030BF RID: 12479 RVA: 0x00061A1A File Offset: 0x0005FC1A
		protected virtual void CurrentStanceOnChanged(Stance obj)
		{
			this.RefreshStatPanel();
			this.RefreshArmorClassPanel();
		}

		// Token: 0x060030C0 RID: 12480 RVA: 0x00061A28 File Offset: 0x0005FC28
		protected virtual void CharacterDataOnHandConfigurationChanged()
		{
			this.RefreshActiveWeaponSet();
			this.RefreshShieldArmor();
			this.RefreshRoleStats();
			this.RefreshStatPanel();
		}

		// Token: 0x060030C1 RID: 12481 RVA: 0x0015ACC4 File Offset: 0x00158EC4
		public void RefreshShieldArmor()
		{
			if (base.GameEntity.Type == GameEntityType.Npc)
			{
				return;
			}
			Vitals.CombatHandStats weaponSetActive = this.m_weaponSetActive;
			IArmorClass armorClass;
			if (((weaponSetActive != null) ? weaponSetActive.OffHand : null) != null && this.m_weaponSetActive.OffHand.Archetype && this.m_weaponSetActive.OffHand.Archetype is ShieldItem && this.m_weaponSetActive.OffHand.Archetype.TryGetAsType(out armorClass))
			{
				this.m_shieldArmorClass = armorClass.GetCurrentArmorClass((float)this.m_weaponSetActive.OffHand.ItemData.Durability.Absorbed);
				this.m_shieldMaxArmorClass = armorClass.BaseArmorClass;
			}
			else
			{
				this.m_shieldArmorClass = 0;
				this.m_shieldMaxArmorClass = 0;
			}
			this.RefreshArmorClassPanel();
		}

		// Token: 0x060030C2 RID: 12482 RVA: 0x00061A42 File Offset: 0x0005FC42
		private void RefreshActiveWeaponSet()
		{
			this.m_weaponSetActive = (base.GameEntity.CharacterData.MainHand_SecondaryActive ? this.m_weaponSetSecondary : this.m_weaponSetPrimary);
		}

		// Token: 0x060030C3 RID: 12483 RVA: 0x0015AD84 File Offset: 0x00158F84
		public void MasteryLevelChanged(ArchetypeInstance masteryInstance, float previousLevel, float currentLevel)
		{
			int num = Mathf.FloorToInt(previousLevel);
			int num2 = Mathf.FloorToInt(currentLevel);
			if (num != num2)
			{
				this.RefreshRoleStats();
				this.RefreshVitals();
				this.RefreshArmorBudget();
				this.RefreshStatPanel();
			}
		}

		// Token: 0x060030C4 RID: 12484 RVA: 0x00061A6A File Offset: 0x0005FC6A
		private void EquipmentOnInstanceAdded(ArchetypeInstance instance)
		{
			this.EquipmentOnInstanceChanged(instance, true);
		}

		// Token: 0x060030C5 RID: 12485 RVA: 0x00061A74 File Offset: 0x0005FC74
		private void EquipmentOnInstanceRemoved(ArchetypeInstance instance)
		{
			this.EquipmentOnInstanceChanged(instance, false);
		}

		// Token: 0x060030C6 RID: 12486 RVA: 0x0015ADBC File Offset: 0x00158FBC
		protected virtual void EquipmentOnInstanceChanged(ArchetypeInstance instance, bool adding)
		{
			int num = adding ? 1 : -1;
			bool flag = false;
			Vitals.EquipmentStatTypes.Clear();
			IEquipable equipable;
			if (instance.Archetype.TryGetAsType(out equipable))
			{
				if ((adding && instance.Index == 65536) || (!adding && instance.PreviousIndex == 65536))
				{
					return;
				}
				if (equipable.Type.IsWeaponSlot())
				{
					this.m_weaponSetPrimary.RefreshInstances();
					this.m_weaponSetSecondary.RefreshInstances();
					this.RefreshShieldArmor();
					flag = true;
					base.GameEntity.ResetHandheldItemCacheFrame();
				}
				else if (equipable.StatModifiers != null && equipable.StatModifiers.Length != 0)
				{
					for (int i = 0; i < equipable.StatModifiers.Length; i++)
					{
						if (equipable.StatModifiers[i] != null && !equipable.StatModifiers[i].StatType.IsInvalid() && !Vitals.EquipmentStatTypes.Contains(equipable.StatModifiers[i].StatType))
						{
							Vitals.EquipmentStatTypes.Add(equipable.StatModifiers[i].StatType);
							Dictionary<StatType, int> effects = equipable.StatModifiers[i].StatType.IsCombatOnly() ? this.m_combatStats : this.m_baseStats;
							equipable.StatModifiers[i].ModifyValue(effects, adding);
						}
					}
				}
				if (!flag && equipable.SetBonus && this.m_armorSetBonuses != null)
				{
					int num2 = 0;
					int num3 = 0;
					if (this.m_armorSetBonuses.TryGetValue(equipable.SetBonus, out num2))
					{
						num3 = (adding ? (num2 + 1) : (num2 - 1));
						num3 = Mathf.Clamp(num3, 0, int.MaxValue);
						if (num3 > 0)
						{
							this.m_armorSetBonuses[equipable.SetBonus] = num3;
						}
						else
						{
							this.m_armorSetBonuses.Remove(equipable.SetBonus);
						}
					}
					else if (adding)
					{
						this.m_armorSetBonuses.Add(equipable.SetBonus, 1);
						num3 = 1;
					}
					equipable.SetBonus.ModifyPieceCount(num2, num3, this.m_baseStats, this.m_combatStats);
				}
			}
			IArmorClass armorClass = null;
			if (!flag && instance.Archetype.TryGetAsType(out armorClass) && armorClass.Type.ContributesArmorClass())
			{
				this.ApplyMaxArmorClassDelta(num * armorClass.BaseArmorClass);
				this.ApplyArmorClassDelta(num * armorClass.GetCurrentArmorClass((float)instance.ItemData.Durability.Absorbed));
			}
			this.RefreshRoleStats();
			this.RefreshAugmentStats(false);
			this.ArmorItemModified(armorClass, adding);
		}

		// Token: 0x060030C7 RID: 12487 RVA: 0x0015B028 File Offset: 0x00159228
		public void RefreshAugmentStats(bool refreshStatPanel = false)
		{
			if (!base.GameEntity || base.GameEntity.CollectionController == null || base.GameEntity.CollectionController.Equipment == null)
			{
				return;
			}
			for (int i = 0; i < StatTypeExtensions.StatTypes.Length; i++)
			{
				this.m_armorAugmentStats[StatTypeExtensions.StatTypes[i]] = 0;
			}
			List<Vitals.ActiveArmorAugment> activeAugments = this.m_activeAugments;
			if (activeAugments != null)
			{
				activeAugments.Clear();
			}
			ContainerInstance equipment = base.GameEntity.CollectionController.Equipment;
			for (int j = 0; j < equipment.Count; j++)
			{
				ArchetypeInstance index = equipment.GetIndex(j);
				ArmorAugmentItem armorAugmentItem;
				if (index != null && index.Index != 65536 && index.ItemData != null && index.ItemData.Augment != null && !index.ItemData.Augment.ArchetypeId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<ArmorAugmentItem>(index.ItemData.Augment.ArchetypeId, out armorAugmentItem))
				{
					if (GameManager.IsServer)
					{
						if (this.m_activeAugments == null)
						{
							this.m_activeAugments = new List<Vitals.ActiveArmorAugment>(100);
						}
						this.m_activeAugments.Add(new Vitals.ActiveArmorAugment
						{
							AugmentItem = armorAugmentItem,
							Instance = index
						});
					}
					for (int k = 0; k < armorAugmentItem.Augments.Length; k++)
					{
						if (armorAugmentItem.Augments[k] != null)
						{
							armorAugmentItem.Augments[k].ModifyValue(this.m_armorAugmentStats, true);
						}
					}
				}
			}
			if (refreshStatPanel)
			{
				this.RefreshStatPanel();
			}
		}

		// Token: 0x060030C8 RID: 12488 RVA: 0x0015B1B8 File Offset: 0x001593B8
		protected virtual void EffectsOnChanged(SynchronizedCollection<UniqueId, EffectSyncData>.Operation op, UniqueId id, EffectSyncData previous, EffectSyncData current)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			if (this.m_lastingVfx == null)
			{
				this.m_lastingVfx = new Dictionary<UniqueId, PooledVFX>(default(UniqueIdComparer));
			}
			switch (op)
			{
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Add:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Insert:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.InitialAdd:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.InitialAddFinal:
			{
				PooledVFX pooledVFX;
				IVfxSource vfxSource;
				AbilityVFX abilityVFX;
				if (!this.m_lastingVfx.TryGetValue(current.ArchetypeId, out pooledVFX) && InternalGameDatabase.Archetypes.TryGetAsType<IVfxSource>(current.ArchetypeId, out vfxSource) && vfxSource.TryGetEffects((int)current.Level, current.AlchemyPowerLevel, current.IsSecondary, out abilityVFX) && abilityVFX.TargetLasting != null)
				{
					PooledVFX pooledInstance = abilityVFX.TargetLasting.GetPooledInstance<PooledVFX>();
					pooledInstance.Initialize(base.GameEntity, null);
					this.m_lastingVfx.Add(current.ArchetypeId, pooledInstance);
					return;
				}
				break;
			}
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Clear:
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.Set:
				break;
			case SynchronizedCollection<UniqueId, EffectSyncData>.Operation.RemoveAt:
			{
				PooledVFX obj;
				if (this.m_lastingVfx.TryGetValue(previous.ArchetypeId, out obj))
				{
					obj.ReturnToPool();
					this.m_lastingVfx.Remove(previous.ArchetypeId);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060030C9 RID: 12489 RVA: 0x0015B2D0 File Offset: 0x001594D0
		public void UpdateLastingEffectTriggerCount(UniqueId instanceId, byte newCount)
		{
			EffectSyncData value;
			if (this.m_replicator && this.m_replicator.Effects != null && this.m_replicator.Effects.TryGetValue(instanceId, out value))
			{
				value.TriggerCount = new byte?(newCount);
				this.m_replicator.Effects[instanceId] = value;
				Action<NetworkEntity> refreshEffectPanelForEntity = Vitals.RefreshEffectPanelForEntity;
				if (refreshEffectPanelForEntity == null)
				{
					return;
				}
				refreshEffectPanelForEntity(base.GameEntity.NetworkEntity);
			}
		}

		// Token: 0x060030CA RID: 12490 RVA: 0x0015B348 File Offset: 0x00159548
		private void RefreshRoleStats()
		{
			for (int i = 0; i < StatTypeExtensions.StatTypes.Length; i++)
			{
				this.m_roleStats[StatTypeExtensions.StatTypes[i]] = 0;
			}
			UniqueId activeMasteryId = base.GameEntity.CharacterData.ActiveMasteryId;
			bool flag = false;
			CombatMasteryArchetype combatMasteryArchetype;
			ArchetypeInstance archetypeInstance;
			if (!activeMasteryId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<CombatMasteryArchetype>(activeMasteryId, out combatMasteryArchetype) && base.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(activeMasteryId, out archetypeInstance))
			{
				flag = combatMasteryArchetype.EntityHasCompatibleWeapons(base.GameEntity.HandHeldItemCache);
				int associatedLevelInteger = archetypeInstance.GetAssociatedLevelInteger(base.GameEntity);
				IStatModifier modifiers;
				if (combatMasteryArchetype.TryGetAsType(out modifiers))
				{
					this.ToggleRoleStatsInternal(this.m_roleStats, modifiers, true, associatedLevelInteger, flag);
				}
				if (this.TryGetSpecializationModifiers(archetypeInstance, out modifiers))
				{
					this.ToggleRoleStatsInternal(this.m_roleStats, modifiers, true, associatedLevelInteger, flag);
				}
			}
			this.m_weaponSetActive.RefreshActiveStats(flag || base.GameEntity.Type == GameEntityType.Npc);
		}

		// Token: 0x060030CB RID: 12491 RVA: 0x0015B440 File Offset: 0x00159640
		private void ToggleRoleStatsInternal(Dictionary<StatType, int> collection, IStatModifier modifiers, bool adding, int level, bool hasCompatibleWeapons)
		{
			if (modifiers == null || modifiers.Stats == null || modifiers.Stats.Length == 0)
			{
				return;
			}
			for (int i = 0; i < modifiers.Stats.Length; i++)
			{
				if (!modifiers.Stats[i].StatType.IsInvalid() && (hasCompatibleWeapons || !modifiers.Stats[i].StatType.IsCombatOnly()))
				{
					modifiers.Stats[i].ModifyValue(collection, adding, level);
				}
			}
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x060030CC RID: 12492 RVA: 0x00061A7E File Offset: 0x0005FC7E
		// (set) Token: 0x060030CD RID: 12493 RVA: 0x00061A86 File Offset: 0x0005FC86
		public int ArmorWeightCapacity { get; private set; }

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x060030CE RID: 12494 RVA: 0x00061A8F File Offset: 0x0005FC8F
		// (set) Token: 0x060030CF RID: 12495 RVA: 0x00061A97 File Offset: 0x0005FC97
		public int ArmorCost { get; private set; }

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x060030D0 RID: 12496 RVA: 0x00061AA0 File Offset: 0x0005FCA0
		// (set) Token: 0x060030D1 RID: 12497 RVA: 0x00061AA8 File Offset: 0x0005FCA8
		public int ArmorCostModifier { get; private set; }

		// Token: 0x060030D2 RID: 12498 RVA: 0x0015B4B4 File Offset: 0x001596B4
		private void ArmorItemModified(IArmorClass armorItem, bool adding)
		{
			if (armorItem != null && armorItem.ArmorCost != 0)
			{
				int num = adding ? 1 : -1;
				this.ArmorCost += armorItem.ArmorCost * num;
				this.RefreshArmorBudget();
			}
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x0015B4F0 File Offset: 0x001596F0
		private void RefreshArmorBudget()
		{
			if (this.ArmorWeightCapacity < this.ArmorCost)
			{
				float t = Mathf.Clamp01((float)(this.ArmorCost - this.ArmorWeightCapacity) / (float)this.ArmorWeightCapacity);
				this.ArmorCostModifier = Mathf.FloorToInt(Mathf.Lerp(0f, -80f, t));
			}
			else
			{
				this.ArmorCostModifier = 0;
			}
			if (base.GameEntity.Motor != null)
			{
				base.GameEntity.Motor.SpeedModifier = (float)this.GetStatusEffectValue(StatType.Movement) * 0.01f;
			}
			this.RefreshArmorBudgetPanel();
		}

		// Token: 0x060030D4 RID: 12500 RVA: 0x0015B580 File Offset: 0x00159780
		private int GetStatTypeValueInternal(StatType statType, bool inCombat, out int diminishableValue, out int nonDiminishableValue, out StatSettings.ClampSettings clampSettings, out bool isMaxCapped)
		{
			diminishableValue = 0;
			nonDiminishableValue = 0;
			clampSettings = null;
			isMaxCapped = false;
			int num = 0;
			if (this.m_baseStats == null || statType == StatType.None || statType.IsInvalid())
			{
				return num;
			}
			clampSettings = GlobalSettings.Values.Stats.GetSettingForStat(statType);
			num += this.m_baseStats[statType];
			num += this.m_weaponStats[statType];
			num += this.m_armorAugmentStats[statType];
			if (statType.IsCombatOnly() && inCombat)
			{
				num += this.m_combatStats[statType];
			}
			diminishableValue = num;
			if (clampSettings != null && num > clampSettings.MaxEquipped)
			{
				num = clampSettings.MaxEquipped;
			}
			nonDiminishableValue = this.m_roleStats[statType] + this.m_transientStats[statType];
			num += nonDiminishableValue;
			if (clampSettings != null && num >= clampSettings.MaxTotal)
			{
				num = clampSettings.MaxTotal;
				isMaxCapped = true;
			}
			int num2 = num;
			num = this.ModifyForArmorCapacity(statType, num, clampSettings);
			if (num2 != num)
			{
				float num3 = (float)num / (float)num2;
				int num4 = Mathf.FloorToInt((float)nonDiminishableValue * num3);
				int num5 = num - num4;
				diminishableValue = num5;
				nonDiminishableValue = num4;
			}
			return num;
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x00061AB1 File Offset: 0x0005FCB1
		private bool ArmorAugmentsAreContributing(StatType statType)
		{
			return this.m_baseStats != null && statType != StatType.None && this.m_armorAugmentStats[statType] != 0;
		}

		// Token: 0x060030D6 RID: 12502 RVA: 0x0015B69C File Offset: 0x0015989C
		public int GetStatusEffectValues(StatType statType, out int diminishableValue, out int nonDiminishableValue)
		{
			StatSettings.ClampSettings clampSettings;
			bool flag;
			return this.GetStatTypeValueInternal(statType, this.Stance == Stance.Combat, out diminishableValue, out nonDiminishableValue, out clampSettings, out flag);
		}

		// Token: 0x060030D7 RID: 12503 RVA: 0x00061ACF File Offset: 0x0005FCCF
		public int GetStatusEffectValue(StatType statType)
		{
			return this.GetStatusEffectValueSpecifiedCombat(statType, this.Stance == Stance.Combat);
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x00061AE1 File Offset: 0x0005FCE1
		public int GetStatusEffectValueInCombat(StatType statType)
		{
			return this.GetStatusEffectValueSpecifiedCombat(statType, true);
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x00061AEB File Offset: 0x0005FCEB
		public int GetStatusEffectValueOutOfCombat(StatType statType)
		{
			return this.GetStatusEffectValueSpecifiedCombat(statType, false);
		}

		// Token: 0x060030DA RID: 12506 RVA: 0x0015B6C0 File Offset: 0x001598C0
		private int GetStatusEffectValueSpecifiedCombat(StatType statType, bool inCombat)
		{
			int num;
			int num2;
			StatSettings.ClampSettings clampSettings;
			bool flag;
			return this.GetStatTypeValueInternal(statType, inCombat, out num, out num2, out clampSettings, out flag);
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x00061AF5 File Offset: 0x0005FCF5
		public virtual int GetHaste()
		{
			return this.GetStatusEffectValue(StatType.Haste);
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateCachedHaste()
		{
		}

		// Token: 0x060030DD RID: 12509 RVA: 0x0015B6E0 File Offset: 0x001598E0
		private int ModifyForArmorCapacity(StatType statType, int value, StatSettings.ClampSettings clampSettings)
		{
			if (statType.ModifiedByArmorCapacity() && base.GameEntity && base.GameEntity.Type == GameEntityType.Player)
			{
				int max = (clampSettings != null) ? clampSettings.MaxTotal : int.MaxValue;
				value = Mathf.Clamp(value + this.ArmorCostModifier, -100, max);
			}
			return value;
		}

		// Token: 0x060030DE RID: 12510 RVA: 0x00061AFF File Offset: 0x0005FCFF
		private string GetValueSign(int value)
		{
			if (value == 0)
			{
				return string.Empty;
			}
			if (value <= 0)
			{
				return "-";
			}
			return "+";
		}

		// Token: 0x060030DF RID: 12511 RVA: 0x00061B19 File Offset: 0x0005FD19
		private Color GetValueColor(int value)
		{
			if (value <= 0)
			{
				return UIManager.RedColor;
			}
			return UIManager.BlueColor;
		}

		// Token: 0x060030E0 RID: 12512 RVA: 0x0015B734 File Offset: 0x00159934
		private Color GetClampedValueColor(int value, StatSettings.ClampSettings clampSettings, int equippedValue, bool isMaxCapped, StatType statType)
		{
			if (clampSettings != null && value > 0)
			{
				if (equippedValue > clampSettings.MaxEquipped)
				{
					return Color.red;
				}
				if (equippedValue == clampSettings.MaxEquipped)
				{
					return Color.green;
				}
				if (isMaxCapped)
				{
					return Color.magenta;
				}
			}
			if (value > 0 && this.ArmorAugmentsAreContributing(statType))
			{
				return UIManager.AugmentColor;
			}
			if (value <= 0)
			{
				return UIManager.RedColor;
			}
			return UIManager.BlueColor;
		}

		// Token: 0x060030E1 RID: 12513 RVA: 0x0015B794 File Offset: 0x00159994
		private string GetFormattedCombatDisplayValue(int combatValue, int baseValue)
		{
			string valueSign = this.GetValueSign(combatValue);
			Color valueColor = this.GetValueColor(combatValue);
			combatValue = Mathf.Abs(combatValue);
			string valueSign2 = this.GetValueSign(baseValue);
			Color valueColor2 = this.GetValueColor(baseValue);
			baseValue = Mathf.Abs(baseValue);
			string arg = (combatValue > 0 || combatValue < 0) ? ZString.Format<string, string, string, int>("<size=80%>(<size=80%>{0}</size><color={1}>{2}{3}</color>)</size>", "<sprite=\"SolIcons\" name=\"Swords\" tint=1>", valueColor.ToHex(), valueSign, combatValue) : ZString.Format<string, string, int>("<size=80%>(<size=80%>{0}</size>{1}{2})</size>", "<sprite=\"SolIcons\" name=\"Swords\" tint=1>", valueSign, combatValue);
			string arg2 = (baseValue > 0 || baseValue < 0) ? ZString.Format<string, string, string>("<color={0}>{1}{2}</color>", valueColor2.ToHex(), valueSign2, baseValue.ToString()) : ZString.Format<string, int>("{0}{1}", valueSign2, baseValue);
			return ZString.Format<string, string>("{0} {1}", arg, arg2);
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x0015B844 File Offset: 0x00159A44
		public string GetStatusEffectDisplayValue(StatType statType)
		{
			if (statType == StatType.Movement && base.GameEntity && base.GameEntity.Motor != null && base.GameEntity.Motor.IsRooted)
			{
				return ZString.Format<string, string>("<color={0}>-100</color> {1}", UIManager.RedColor.ToHex(), "<color=#00000000><size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size></color>");
			}
			if (this.Stance != Stance.Combat && statType.ShowAsCombatOnly())
			{
				int equippedValue;
				int num2;
				StatSettings.ClampSettings clampSettings;
				bool isMaxCapped;
				int num = this.GetStatTypeValueInternal(statType, true, out equippedValue, out num2, out clampSettings, out isMaxCapped);
				string valueSign = this.GetValueSign(num);
				Color clampedValueColor = this.GetClampedValueColor(num, clampSettings, equippedValue, isMaxCapped, statType);
				num = Mathf.Abs(num);
				if (num <= 0)
				{
					return ZString.Format<string>("0 {0}", "<color=#00000000><size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size></color>");
				}
				return ZString.Format<string, string, string, string>("<color={0}>{1}{2}</color> <size=80%>{3}</size>", clampedValueColor.ToHex(), valueSign, num.ToString(), "<sprite=\"SolIcons\" name=\"Swords\" tint=1>");
			}
			else
			{
				int equippedValue2;
				int num4;
				StatSettings.ClampSettings clampSettings2;
				bool isMaxCapped2;
				int num3 = this.GetStatTypeValueInternal(statType, this.Stance == Stance.Combat, out equippedValue2, out num4, out clampSettings2, out isMaxCapped2);
				string text = this.GetValueSign(num3);
				if (statType == StatType.DamageReduction || statType == StatType.DamageReductionEmber)
				{
					text = "";
				}
				Color clampedValueColor2 = this.GetClampedValueColor(num3, clampSettings2, equippedValue2, isMaxCapped2, statType);
				num3 = Mathf.Abs(num3);
				if (num3 > 0 || num3 < 0)
				{
					return ZString.Format<string, string, string, string>("<color={0}>{1}{2}</color> {3}", clampedValueColor2.ToHex(), text, num3.ToString(), "<color=#00000000><size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size></color>");
				}
				return ZString.Format<string, string, string>("{0}{1} {2}", text, num3.ToString(), "<color=#00000000><size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size></color>");
			}
		}

		// Token: 0x060030E3 RID: 12515 RVA: 0x0015B9A0 File Offset: 0x00159BA0
		public void ModifyStatusEffectValue(StatType statType, int delta)
		{
			Dictionary<StatType, int> transientStats = this.m_transientStats;
			transientStats[statType] += delta;
			if (statType == StatType.Haste)
			{
				this.UpdateCachedHaste();
			}
		}

		// Token: 0x060030E4 RID: 12516 RVA: 0x00061B2A File Offset: 0x0005FD2A
		public void UpdateLastCombatTimestamp()
		{
			this.m_lastCombatTimestamp = DateTime.UtcNow;
		}

		// Token: 0x060030E5 RID: 12517 RVA: 0x0015B9D4 File Offset: 0x00159BD4
		public bool ApplyDamageToGearForDeath()
		{
			if (base.GameEntity && base.GameEntity.Type == GameEntityType.Player)
			{
				ICollectionController collectionController = base.GameEntity.CollectionController;
				if (((collectionController != null) ? collectionController.Equipment : null) != null)
				{
					if (LocalZoneManager.IsWithinPvpCollider(base.GameEntity.gameObject.transform.position))
					{
						return false;
					}
					ContainerInstance equipment = base.GameEntity.CollectionController.Equipment;
					for (int i = 0; i < equipment.Count; i++)
					{
						ArchetypeInstance index = equipment.GetIndex(i);
						IDurability durability;
						if (index != null && index.Index != 65536 && index.ItemData != null && index.ItemData.Durability != null && index.Archetype && index.Archetype.TryGetAsType(out durability))
						{
							int num = Mathf.FloorToInt((float)durability.MaxDamageAbsorption * GlobalSettings.Values.Player.DurabilityLossOnDeath);
							index.ItemData.Durability.Absorbed = Mathf.Clamp(num + index.ItemData.Durability.Absorbed, 0, durability.MaxDamageAbsorption);
						}
					}
					this.RecalculateTotalArmorClass();
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002F15 RID: 12053
		public const int kBaseHealth = 70;

		// Token: 0x04002F16 RID: 12054
		private const float kBaseMaxTargetDistance = 40f;

		// Token: 0x04002F17 RID: 12055
		private static readonly HashSet<StatType> EquipmentStatTypes = new HashSet<StatType>(default(StatTypeComparer));

		// Token: 0x04002F19 RID: 12057
		protected bool m_initialized;

		// Token: 0x04002F1A RID: 12058
		protected CharacterRecord m_record;

		// Token: 0x04002F1B RID: 12059
		protected Dictionary<StatType, int> m_baseStats;

		// Token: 0x04002F1C RID: 12060
		private Dictionary<StatType, int> m_roleStats;

		// Token: 0x04002F1D RID: 12061
		private Dictionary<StatType, int> m_combatStats;

		// Token: 0x04002F1E RID: 12062
		private Dictionary<StatType, int> m_weaponStats;

		// Token: 0x04002F1F RID: 12063
		private Dictionary<StatType, int> m_transientStats;

		// Token: 0x04002F20 RID: 12064
		private Dictionary<StatType, int> m_armorAugmentStats;

		// Token: 0x04002F21 RID: 12065
		private Dictionary<SetBonusProfile, int> m_armorSetBonuses;

		// Token: 0x04002F22 RID: 12066
		private Vitals.CombatHandStats m_weaponSetActive;

		// Token: 0x04002F23 RID: 12067
		private Vitals.CombatHandStats m_weaponSetPrimary;

		// Token: 0x04002F24 RID: 12068
		private Vitals.CombatHandStats m_weaponSetSecondary;

		// Token: 0x04002F25 RID: 12069
		private IEnumerable<ArchetypeInstance> m_equipmentEnumerable;

		// Token: 0x04002F26 RID: 12070
		private ContainerInstance m_equipment;

		// Token: 0x04002F27 RID: 12071
		private float m_maxTargetDistance = 40f;

		// Token: 0x04002F28 RID: 12072
		private float m_maxTargetDistanceSqr = 1600f;

		// Token: 0x04002F29 RID: 12073
		protected DateTime m_lastCombatTimestamp = DateTime.MinValue;

		// Token: 0x04002F2A RID: 12074
		protected List<Vitals.ActiveArmorAugment> m_activeAugments;

		// Token: 0x04002F2B RID: 12075
		protected bool m_finalized;

		// Token: 0x04002F2C RID: 12076
		protected int m_shieldArmorClass;

		// Token: 0x04002F2D RID: 12077
		protected int m_shieldMaxArmorClass;

		// Token: 0x04002F2E RID: 12078
		private Dictionary<UniqueId, PooledVFX> m_lastingVfx;

		// Token: 0x020005F8 RID: 1528
		private class CombatHandStats
		{
			// Token: 0x17000A65 RID: 2661
			// (get) Token: 0x060030E8 RID: 12520 RVA: 0x00061B60 File Offset: 0x0005FD60
			public ArchetypeInstance OffHand
			{
				get
				{
					return this.m_offHand;
				}
			}

			// Token: 0x060030E9 RID: 12521 RVA: 0x00061B68 File Offset: 0x0005FD68
			public CombatHandStats(bool isPrimary, Vitals controller)
			{
				if (controller == null)
				{
					throw new ArgumentNullException("controller");
				}
				this.m_isPrimary = isPrimary;
				this.m_controller = controller;
				this.RefreshInstances();
			}

			// Token: 0x060030EA RID: 12522 RVA: 0x0015BB28 File Offset: 0x00159D28
			public void RefreshInstances()
			{
				if (!this.m_controller.GameEntity || this.m_controller.GameEntity.CollectionController == null)
				{
					return;
				}
				EquipmentSlot index = this.m_isPrimary ? EquipmentSlot.PrimaryWeapon_MainHand : EquipmentSlot.SecondaryWeapon_MainHand;
				this.m_controller.GameEntity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out this.m_mainHand);
				IEquipable equipable;
				this.m_mainHandSetBonus = ((this.m_mainHand != null && this.m_mainHand.Archetype && this.m_mainHand.Archetype.TryGetAsType(out equipable)) ? equipable.SetBonus : null);
				EquipmentSlot index2 = this.m_isPrimary ? EquipmentSlot.PrimaryWeapon_OffHand : EquipmentSlot.SecondaryWeapon_OffHand;
				this.m_controller.GameEntity.CollectionController.Equipment.TryGetInstanceForIndex((int)index2, out this.m_offHand);
				IEquipable equipable2;
				this.m_offHandSetBonus = ((this.m_offHand != null && this.m_offHand.Archetype && this.m_offHand.Archetype.TryGetAsType(out equipable2)) ? equipable2.SetBonus : null);
			}

			// Token: 0x060030EB RID: 12523 RVA: 0x0015BC34 File Offset: 0x00159E34
			public void RefreshActiveStats(bool hasCompatibleWeapons)
			{
				for (int i = 0; i < StatTypeExtensions.StatTypes.Length; i++)
				{
					this.m_controller.m_weaponStats[StatTypeExtensions.StatTypes[i]] = 0;
				}
				if (hasCompatibleWeapons)
				{
					this.AddStatsForItem(this.m_mainHand);
					this.AddStatsForItem(this.m_offHand);
					if (this.m_mainHandSetBonus && this.m_offHandSetBonus && this.m_mainHandSetBonus == this.m_offHandSetBonus)
					{
						this.m_mainHandSetBonus.ModifyPieceCount(0, 2, this.m_controller.m_weaponStats, this.m_controller.m_weaponStats);
					}
				}
			}

			// Token: 0x060030EC RID: 12524 RVA: 0x0015BCD8 File Offset: 0x00159ED8
			private void AddStatsForItem(ArchetypeInstance instance)
			{
				if (instance == null || instance.Archetype == null)
				{
					return;
				}
				Vitals.EquipmentStatTypes.Clear();
				IEquipable equipable;
				if (instance.Archetype.TryGetAsType(out equipable) && equipable.StatModifiers != null && equipable.StatModifiers.Length != 0)
				{
					for (int i = 0; i < equipable.StatModifiers.Length; i++)
					{
						if (equipable.StatModifiers[i] != null && !equipable.StatModifiers[i].StatType.IsInvalid() && !Vitals.EquipmentStatTypes.Contains(equipable.StatModifiers[i].StatType))
						{
							Vitals.EquipmentStatTypes.Add(equipable.StatModifiers[i].StatType);
							equipable.StatModifiers[i].ModifyValue(this.m_controller.m_weaponStats, true);
						}
					}
				}
			}

			// Token: 0x060030ED RID: 12525 RVA: 0x00061B98 File Offset: 0x0005FD98
			public void CleanupReferences()
			{
				this.m_mainHand = null;
				this.m_offHand = null;
				this.m_mainHandSetBonus = null;
				this.m_offHandSetBonus = null;
				this.m_controller = null;
			}

			// Token: 0x04002F32 RID: 12082
			private readonly bool m_isPrimary;

			// Token: 0x04002F33 RID: 12083
			private Vitals m_controller;

			// Token: 0x04002F34 RID: 12084
			private ArchetypeInstance m_mainHand;

			// Token: 0x04002F35 RID: 12085
			private ArchetypeInstance m_offHand;

			// Token: 0x04002F36 RID: 12086
			private SetBonusProfile m_mainHandSetBonus;

			// Token: 0x04002F37 RID: 12087
			private SetBonusProfile m_offHandSetBonus;
		}

		// Token: 0x020005F9 RID: 1529
		protected struct ActiveArmorAugment
		{
			// Token: 0x04002F38 RID: 12088
			public ArmorAugmentItem AugmentItem;

			// Token: 0x04002F39 RID: 12089
			public ArchetypeInstance Instance;
		}
	}
}
