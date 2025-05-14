using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000682 RID: 1666
	[Serializable]
	public class NpcLoadout
	{
		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x0600337B RID: 13179 RVA: 0x00063694 File Offset: 0x00061894
		public CombatMasteryArchetype CombatMastery
		{
			get
			{
				return this.m_combatMastery;
			}
		}

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x0600337C RID: 13180 RVA: 0x0006369C File Offset: 0x0006189C
		public AnimancerAnimationSet CombatStance
		{
			get
			{
				return this.m_combatStance;
			}
		}

		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x0600337D RID: 13181 RVA: 0x000636A4 File Offset: 0x000618A4
		private bool m_showWeapons
		{
			get
			{
				return this.m_weaponOverride == null;
			}
		}

		// Token: 0x0600337E RID: 13182 RVA: 0x001621A8 File Offset: 0x001603A8
		public AnimancerAnimationSet InitializeProfileForNpc(NpcProfile npcProfile, GameEntity entity, int level, ContainerRecord equipment, ContainerRecord masteries, ContainerRecord abilities)
		{
			NpcLoadout.m_instanceIds.Clear();
			level = Mathf.Clamp(level, 1, 100);
			NpcWeaponLoadout weapons = this.m_weapons;
			EquipableItem equipableItem = (weapons != null) ? weapons.MainHand : null;
			NpcWeaponLoadout weapons2 = this.m_weapons;
			EquipableItem equipableItem2 = (weapons2 != null) ? weapons2.OffHand : null;
			NpcWeaponLoadout weapons3 = this.m_weapons;
			LightItem lightItem = (weapons3 != null) ? weapons3.LightItem : null;
			NpcWeaponLoadout weapons4 = this.m_weapons;
			RangedAmmoItem rangedAmmoItem = (weapons4 != null) ? weapons4.RangedAmmo : null;
			NpcWeaponLoadout weapons5 = this.m_weapons;
			AnimancerAnimationSet animancerAnimationSet = (weapons5 != null) ? weapons5.AnimationSet : null;
			if (this.m_weaponOverride != null)
			{
				NpcWeaponLoadout loadout = this.m_weaponOverride.GetLoadout();
				if (loadout != null)
				{
					equipableItem = loadout.MainHand;
					equipableItem2 = loadout.OffHand;
					lightItem = loadout.LightItem;
					rangedAmmoItem = loadout.RangedAmmo;
					animancerAnimationSet = loadout.AnimationSet;
				}
			}
			if (npcProfile != null && npcProfile.HasVisualWeapons)
			{
				EquipableItem equipableItem3;
				if (npcProfile.TryGetVisibleWeapon(EquipmentSlot.PrimaryWeapon_MainHand, out equipableItem3))
				{
					equipableItem = equipableItem3;
				}
				EquipableItem equipableItem4;
				if (npcProfile.TryGetVisibleWeapon(EquipmentSlot.PrimaryWeapon_OffHand, out equipableItem4))
				{
					equipableItem2 = equipableItem4;
				}
				EquipableItem archetype;
				LightItem lightItem2;
				if (npcProfile.TryGetVisibleWeapon(EquipmentSlot.LightSource, out archetype) && archetype.TryGetAsType(out lightItem2))
				{
					lightItem = lightItem2;
				}
			}
			if (entity)
			{
				if (entity.NpcStanceManager)
				{
					entity.NpcStanceManager.CombatStance = animancerAnimationSet;
				}
				if (this.m_thresholdActions && entity.Vitals)
				{
					NpcVitals_Server npcVitals_Server = entity.Vitals as NpcVitals_Server;
					if (npcVitals_Server != null)
					{
						npcVitals_Server.ThresholdActions = this.m_thresholdActions;
					}
				}
			}
			if (equipableItem != null)
			{
				this.InitializeEquipmentItem(equipableItem, EquipmentSlot.PrimaryWeapon_MainHand, equipment, level);
			}
			if (equipableItem2 != null)
			{
				this.InitializeEquipmentItem(equipableItem2, EquipmentSlot.PrimaryWeapon_OffHand, equipment, level);
			}
			if (lightItem != null)
			{
				this.InitializeEquipmentItem(lightItem, EquipmentSlot.LightSource, equipment, level);
			}
			if (rangedAmmoItem != null)
			{
				this.InitializeEquipmentItem(rangedAmmoItem, EquipmentSlot.PrimaryWeapon_OffHand, equipment, level);
			}
			if (this.m_abilities != null && this.m_abilities.Length != 0)
			{
				List<MasteryArchetype> fromPool = StaticListPool<MasteryArchetype>.GetFromPool();
				int num = 0;
				for (int i = 0; i < this.m_abilities.Length; i++)
				{
					if (this.m_abilities[i].Ability && this.m_abilities[i].Ability.Mastery)
					{
						if (this.m_abilities[i].Ability is ReagentAbility)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Skipping add of ReagentAbility to an NPC!  ",
								this.m_abilities[i].Ability.DisplayName,
								" (ID: ",
								this.m_abilities[i].Ability.Id.ToString(),
								")"
							}));
						}
						else if (this.m_abilities[i].Ability.MinimumLevel <= level)
						{
							ArchetypeInstance archetypeInstance = this.m_abilities[i].Ability.CreateNewInstance();
							if (this.CanAddInstance(archetypeInstance, "Abilities"))
							{
								archetypeInstance.AbilityData.HealthFractionProbabilityCurve = this.m_abilities[i];
								archetypeInstance.AbilityData.MemorizationTimestamp = new DateTime?(DateTime.UtcNow);
								archetypeInstance.Index = num;
								abilities.Instances.Add(archetypeInstance);
								num++;
								if (!fromPool.Contains(this.m_abilities[i].Ability.Mastery))
								{
									fromPool.Add(this.m_abilities[i].Ability.Mastery);
								}
							}
						}
					}
				}
				bool flag = false;
				for (int j = 0; j < fromPool.Count; j++)
				{
					this.InitializeMastery(fromPool[j], masteries, level);
					flag = (flag || (this.m_combatMastery && this.m_combatMastery.Id == fromPool[j].Id));
				}
				if (!flag && this.m_combatMastery)
				{
					this.InitializeMastery(this.m_combatMastery, masteries, level);
				}
				StaticListPool<MasteryArchetype>.ReturnToPool(fromPool);
			}
			NpcLoadout.m_instanceIds.Clear();
			return animancerAnimationSet;
		}

		// Token: 0x0600337F RID: 13183 RVA: 0x001625B0 File Offset: 0x001607B0
		private void InitializeEquipmentItem(EquipableItem item, EquipmentSlot slot, ContainerRecord equipment, int level)
		{
			ArchetypeInstance archetypeInstance = CharacterRecordExtensions.InitializeItem(item, null);
			if (this.CanAddInstance(archetypeInstance, "Equipment"))
			{
				archetypeInstance.Index = (int)slot;
				equipment.Instances.Add(archetypeInstance);
				if (slot.IsWeaponSlot())
				{
					WeaponItem weaponItem = item as WeaponItem;
					if (weaponItem != null)
					{
						weaponItem.GenerateDynamicDiceSet(archetypeInstance, (float)level);
					}
				}
			}
		}

		// Token: 0x06003380 RID: 13184 RVA: 0x00162608 File Offset: 0x00160808
		private void InitializeMastery(MasteryArchetype mastery, ContainerRecord masteries, int level)
		{
			ArchetypeInstance archetypeInstance = mastery.CreateNewInstance();
			if (this.CanAddInstance(archetypeInstance, "Masteries"))
			{
				archetypeInstance.MasteryData.BaseLevel = (float)level;
				masteries.Instances.Add(archetypeInstance);
			}
		}

		// Token: 0x06003381 RID: 13185 RVA: 0x00162644 File Offset: 0x00160844
		private bool CanAddInstance(ArchetypeInstance instance, string description)
		{
			if (instance == null)
			{
				return false;
			}
			if (NpcLoadout.m_instanceIds.Contains(instance.InstanceId))
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"InstanceId GUARD! Duplicate InstanceId attempted to add to Npc ",
					description,
					"!  ",
					instance.Archetype.name,
					" of ArchetypeId ",
					instance.ArchetypeId.ToString()
				}));
				StaticPool<ArchetypeInstance>.ReturnToPool(instance);
				return false;
			}
			NpcLoadout.m_instanceIds.Add(instance.InstanceId);
			return true;
		}

		// Token: 0x06003382 RID: 13186 RVA: 0x000636B2 File Offset: 0x000618B2
		private IEnumerable GetMasteries()
		{
			return SolOdinUtilities.GetDropdownItems<CombatMasteryArchetype>();
		}

		// Token: 0x06003383 RID: 13187 RVA: 0x000636B9 File Offset: 0x000618B9
		private IEnumerable GetEquipable()
		{
			return SolOdinUtilities.GetDropdownItems<EquipableItem>();
		}

		// Token: 0x06003384 RID: 13188 RVA: 0x000636C0 File Offset: 0x000618C0
		private IEnumerable GetRangedAmmo()
		{
			return SolOdinUtilities.GetDropdownItems<RangedAmmoItem>();
		}

		// Token: 0x06003385 RID: 13189 RVA: 0x000636B2 File Offset: 0x000618B2
		private IEnumerable GetCombatMastery()
		{
			return SolOdinUtilities.GetDropdownItems<CombatMasteryArchetype>();
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x000636C7 File Offset: 0x000618C7
		private IEnumerable GetWeaponProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<NpcWeaponProfile>();
		}

		// Token: 0x06003387 RID: 13191 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetAnimSets()
		{
			return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
		}

		// Token: 0x0400318C RID: 12684
		private static readonly HashSet<UniqueId> m_instanceIds = new HashSet<UniqueId>(default(UniqueIdComparer));

		// Token: 0x0400318D RID: 12685
		[SerializeField]
		private CombatMasteryArchetype m_combatMastery;

		// Token: 0x0400318E RID: 12686
		[SerializeField]
		private AnimancerAnimationSet m_combatStance;

		// Token: 0x0400318F RID: 12687
		[SerializeField]
		private NpcWeaponProfile m_weaponOverride;

		// Token: 0x04003190 RID: 12688
		[SerializeField]
		private NpcWeaponLoadout m_weapons;

		// Token: 0x04003191 RID: 12689
		[SerializeField]
		private HealthThresholdActions m_thresholdActions;

		// Token: 0x04003192 RID: 12690
		[SerializeField]
		private NpcLoadout.MasteryAbilitySet[] m_masterySets;

		// Token: 0x04003193 RID: 12691
		[SerializeField]
		private NpcLoadout.AbilityData[] m_abilities;

		// Token: 0x02000683 RID: 1667
		[Serializable]
		private class MasteryAbilitySet
		{
			// Token: 0x17000B1F RID: 2847
			// (get) Token: 0x0600338A RID: 13194 RVA: 0x000636D5 File Offset: 0x000618D5
			public Vector2 HealthFractionRange
			{
				get
				{
					return this.m_healthFractionRange;
				}
			}

			// Token: 0x17000B20 RID: 2848
			// (get) Token: 0x0600338B RID: 13195 RVA: 0x000636DD File Offset: 0x000618DD
			public int Level
			{
				get
				{
					return this.m_level;
				}
			}

			// Token: 0x17000B21 RID: 2849
			// (get) Token: 0x0600338C RID: 13196 RVA: 0x000636E5 File Offset: 0x000618E5
			public MasteryArchetype Mastery
			{
				get
				{
					return this.m_mastery;
				}
			}

			// Token: 0x17000B22 RID: 2850
			// (get) Token: 0x0600338D RID: 13197 RVA: 0x000636ED File Offset: 0x000618ED
			public AbilityArchetype[] Abilities
			{
				get
				{
					return this.m_abilities;
				}
			}

			// Token: 0x0600338E RID: 13198 RVA: 0x000636F5 File Offset: 0x000618F5
			private IEnumerable GetMastery()
			{
				return SolOdinUtilities.GetDropdownItems<MasteryArchetype>();
			}

			// Token: 0x04003194 RID: 12692
			[Range(1f, 100f)]
			[SerializeField]
			private int m_level = 100;

			// Token: 0x04003195 RID: 12693
			[SerializeField]
			private Vector2 m_healthFractionRange = new Vector2(0f, 1f);

			// Token: 0x04003196 RID: 12694
			[SerializeField]
			private MasteryArchetype m_mastery;

			// Token: 0x04003197 RID: 12695
			[SerializeField]
			private AbilityArchetype[] m_abilities;
		}

		// Token: 0x02000684 RID: 1668
		[Serializable]
		private class AbilityData : IAnimationCurve
		{
			// Token: 0x17000B23 RID: 2851
			// (get) Token: 0x06003390 RID: 13200 RVA: 0x00063721 File Offset: 0x00061921
			private bool m_showAnimCurve
			{
				get
				{
					return this.m_healthFractionProbabilityOverride == null;
				}
			}

			// Token: 0x17000B24 RID: 2852
			// (get) Token: 0x06003391 RID: 13201 RVA: 0x0006372F File Offset: 0x0006192F
			public AbilityArchetype Ability
			{
				get
				{
					return this.m_ability;
				}
			}

			// Token: 0x06003392 RID: 13202 RVA: 0x00063737 File Offset: 0x00061937
			public float Evaluate(float time)
			{
				if (!this.m_healthFractionProbabilityOverride)
				{
					return this.m_healthFractionProbability.Evaluate(time);
				}
				return this.m_healthFractionProbabilityOverride.Evaluate(time);
			}

			// Token: 0x04003198 RID: 12696
			[SerializeField]
			private AbilityArchetype m_ability;

			// Token: 0x04003199 RID: 12697
			[SerializeField]
			private ScriptableCurve m_healthFractionProbabilityOverride;

			// Token: 0x0400319A RID: 12698
			[Tooltip("X-Axis represents health fraction (0-1)\nY-Axis represents relative probability (0-1)")]
			[SerializeField]
			private AnimationCurve m_healthFractionProbability = AnimationCurve.Constant(0f, 1f, 1f);
		}
	}
}
