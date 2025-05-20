using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Influence;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Game.NPCs;
using SoL.Game.NPCs.Senses;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Targeting;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006A3 RID: 1699
	[CreateAssetMenu(menuName = "SoL/Profiles/Npc Spawn")]
	public class NpcSpawnProfile : SpawnProfile
	{
		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x060033E3 RID: 13283 RVA: 0x00063A32 File Offset: 0x00061C32
		private bool m_showLocalNameCollection
		{
			get
			{
				return this.m_randomizeName && this.m_namesOverride == null;
			}
		}

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x060033E4 RID: 13284 RVA: 0x00063A4A File Offset: 0x00061C4A
		private bool m_hidePortraitCollection
		{
			get
			{
				return this.m_spriteCollectionOverride != null;
			}
		}

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x060033E5 RID: 13285 RVA: 0x00063A58 File Offset: 0x00061C58
		private bool m_showSingleAbilities
		{
			get
			{
				return this.m_loadouts == null || this.m_loadouts.Count <= 0;
			}
		}

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x060033E6 RID: 13286 RVA: 0x00063A75 File Offset: 0x00061C75
		private bool m_showLootProfileCollection
		{
			get
			{
				return !this.m_despawnOnDeath && this.m_lootTable == null && this.m_lootTableV2 == null;
			}
		}

		// Token: 0x060033E7 RID: 13287 RVA: 0x00063A9D File Offset: 0x00061C9D
		private IEnumerable GetLootTableProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<LootTableProfile>();
		}

		// Token: 0x060033E8 RID: 13288 RVA: 0x00063AA4 File Offset: 0x00061CA4
		private IEnumerable GetLootTables()
		{
			return SolOdinUtilities.GetDropdownItems<LootTable>();
		}

		// Token: 0x060033E9 RID: 13289 RVA: 0x00063AAB File Offset: 0x00061CAB
		public void LoadStaticSpawnData(StaticSpawnData spawnData)
		{
			this.m_name = spawnData.Name;
			this.m_title = spawnData.Title;
			this.m_maxHealth = spawnData.MaxHealth;
			this.m_maxArmorClass = spawnData.MaxArmorClass;
			this.m_maxDamageAbsorption = spawnData.MaxDamageAbsorption;
		}

		// Token: 0x060033EA RID: 13290 RVA: 0x00162AFC File Offset: 0x00160CFC
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			base.SpawnInternal(controller, gameEntity);
			SpawnTier spawnTier = this.m_scaleBySpawnTier ? this.GetSpawnTier() : SpawnTier.Normal;
			ContainerRecord containerRecord = new ContainerRecord
			{
				Type = ContainerType.Equipment,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord2 = new ContainerRecord
			{
				Type = ContainerType.Masteries,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord3 = new ContainerRecord
			{
				Type = ContainerType.Abilities,
				Instances = new List<ArchetypeInstance>()
			};
			int num;
			int level = (controller != null && controller.TryGetLevel(out num)) ? num : this.m_levelRange.RandomWithinRange();
			NpcLoadoutWithOverride npcLoadoutWithOverride = this.m_singleLoadout;
			NpcLoadoutProbabilityCollection loadouts = this.m_loadouts;
			NpcLoadoutProbabilityEntry npcLoadoutProbabilityEntry = (loadouts != null) ? loadouts.GetEntry(null, false) : null;
			if (npcLoadoutProbabilityEntry != null && npcLoadoutProbabilityEntry.Obj != null && npcLoadoutProbabilityEntry.Obj.Loadout != null)
			{
				npcLoadoutWithOverride = npcLoadoutProbabilityEntry.Obj;
			}
			AnimancerAnimationSet animancerAnimationSet;
			if (npcLoadoutWithOverride == null)
			{
				animancerAnimationSet = null;
			}
			else
			{
				NpcLoadout loadout = npcLoadoutWithOverride.Loadout;
				animancerAnimationSet = ((loadout != null) ? loadout.InitializeProfileForNpc(this.m_npcProfile, gameEntity, level, containerRecord, containerRecord2, containerRecord3) : null);
			}
			AnimancerAnimationSet animancerAnimationSet2 = animancerAnimationSet;
			CharacterRecord record = new CharacterRecord
			{
				Storage = new Dictionary<ContainerType, ContainerRecord>(default(ContainerTypeComparer))
				{
					{
						ContainerType.Equipment,
						containerRecord
					},
					{
						ContainerType.Abilities,
						containerRecord3
					},
					{
						ContainerType.Masteries,
						containerRecord2
					}
				},
				Effects = new List<EffectRecord>()
			};
			if (gameEntity.EffectController != null)
			{
				gameEntity.EffectController.Init(record);
			}
			ICollectionController collectionController = gameEntity.CollectionController;
			if (collectionController != null)
			{
				collectionController.Initialize(record);
			}
			if (gameEntity.SkillsController != null)
			{
				gameEntity.SkillsController.Initialize(record);
			}
			if (gameEntity.Vitals != null)
			{
				NpcVitals_Server npcVitals_Server = gameEntity.Vitals as NpcVitals_Server;
				if (npcVitals_Server != null)
				{
					int num2 = this.m_maxHealth;
					int num3 = this.m_maxArmorClass;
					int num4 = this.m_maxDamageAbsorption;
					if (this.m_scaleBySpawnTier)
					{
						float percentage = (float)spawnTier / 100f;
						num2 = Mathf.CeilToInt(num2.PercentModification(percentage));
						num3 = Mathf.CeilToInt(num3.PercentModification(percentage));
						num4 = Mathf.CeilToInt(num4.PercentModification(percentage));
					}
					npcVitals_Server.SetMaxValues(num2, num3, num4, 0.8f);
				}
				gameEntity.Vitals.Init(record);
				if (npcVitals_Server != null)
				{
					npcVitals_Server.ApplyStatModifiers(this.m_statModifiers);
				}
			}
			if (gameEntity.CharacterData != null)
			{
				gameEntity.CharacterData.InitNpcLevel(level);
				gameEntity.CharacterData.Faction = this.m_faction;
				gameEntity.CharacterData.Name.Value = (this.m_randomizeName ? ((this.m_namesOverride == null) ? this.m_names.GetEntry(null, false).Obj : this.m_namesOverride.GetEntry().Obj) : this.m_name);
				gameEntity.CharacterData.Title.Value = ((this.m_useSpawnTierAsTitle && this.m_scaleBySpawnTier && spawnTier != SpawnTier.Normal) ? spawnTier.ToString() : this.m_title);
				gameEntity.CharacterData.CurrentCombatId.Value = (animancerAnimationSet2 ? animancerAnimationSet2.Id : UniqueId.Empty);
				if (this.m_weaponsAlwaysMounted)
				{
					gameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.Weapons;
				}
				NpcInitData npcInitData = default(NpcInitData);
				gameEntity.CharacterData.NpcInitData = npcInitData;
				if (this.m_spriteCollectionOverride != null)
				{
					gameEntity.CharacterData.PortraitId.Value = this.m_spriteCollectionOverride.Id;
				}
				else
				{
					IdentifiableSpriteProbabilityCollection portraits = this.m_portraits;
					IdentifiableSpritePropabilityEntry identifiableSpritePropabilityEntry = (portraits != null) ? portraits.GetEntry(null, false) : null;
					if (identifiableSpritePropabilityEntry != null && !identifiableSpritePropabilityEntry.Obj.Id.IsEmpty)
					{
						gameEntity.CharacterData.PortraitId.Value = identifiableSpritePropabilityEntry.Obj.Id;
					}
				}
				if (this.m_scaleTransformBySpawnTier)
				{
					float num5 = (float)spawnTier * 0.01f;
					float num6 = Mathf.Lerp(-0.15f, 0.15f, UnityEngine.Random.Range(0f, 1f));
					num5 += num6;
					gameEntity.CharacterData.TransformScale = new float?(num5);
				}
				else if (!this.m_transformScale.IsZero)
				{
					gameEntity.CharacterData.TransformScale = new float?(this.m_transformScale.RandomWithinRange());
				}
			}
			BehaviorProfile behaviorProfile = this.m_behaviorProfile;
			BehaviorProfile behaviorProfile2;
			if (controller != null && controller.TryGetBehaviorProfile(out behaviorProfile2))
			{
				behaviorProfile = behaviorProfile2;
			}
			if (behaviorProfile != null && gameEntity.ServerNpcController != null)
			{
				gameEntity.ServerNpcController.DespawnOnDeath = this.m_despawnOnDeath;
				if (gameEntity.ServerNpcController.Tree != null)
				{
					float? num8;
					if (controller != null)
					{
						float? leashDistance = controller.LeashDistance;
						float num7 = 0f;
						if (leashDistance.GetValueOrDefault() > num7 & leashDistance != null)
						{
							num8 = controller.LeashDistance;
							goto IL_4C6;
						}
					}
					num8 = new float?(this.m_leashDistance);
					IL_4C6:
					float? num9 = num8;
					behaviorProfile.LoadBehaviorProfile(gameEntity.ServerNpcController.Tree);
					SharedVariable variable = gameEntity.ServerNpcController.Tree.GetVariable("LeashDistance");
					if (variable != null)
					{
						variable.SetValue(num9);
					}
					gameEntity.ServerNpcController.Tree.enabled = true;
				}
			}
			NpcTargetController npcTargetController;
			if (gameEntity.TargetController.TryGetAsType(out npcTargetController))
			{
				npcTargetController.RotateAwayInCombat = this.m_rotateAwayInComabat;
				npcTargetController.SensorProfile = this.m_sensorProfile;
			}
			if (gameEntity.NpcStanceManager != null)
			{
				gameEntity.NpcStanceManager.CombatStance = animancerAnimationSet2;
				NpcStanceManager npcStanceManager = gameEntity.NpcStanceManager;
				CombatMasteryArchetype combatMastery;
				if (npcLoadoutWithOverride == null)
				{
					combatMastery = null;
				}
				else
				{
					NpcLoadout loadout2 = npcLoadoutWithOverride.Loadout;
					combatMastery = ((loadout2 != null) ? loadout2.CombatMastery : null);
				}
				npcStanceManager.CombatMastery = combatMastery;
				gameEntity.NpcStanceManager.WeaponsAlwaysMounted = this.m_weaponsAlwaysMounted;
			}
			InteractiveNpc interactiveNpc;
			if (!this.m_despawnOnDeath && gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveNpc))
			{
				interactiveNpc.SpawnTier = spawnTier;
				if (this.m_lootTableV2 != null)
				{
					interactiveNpc.LootTable = this.m_lootTableV2;
				}
				else if (this.m_lootTable != null)
				{
					interactiveNpc.LootTable = this.m_lootTable;
				}
				if (this.m_resourceLootTableV2 != null)
				{
					interactiveNpc.ResourceLootTable = this.m_resourceLootTableV2;
				}
				else
				{
					interactiveNpc.ResourceLootTable = this.m_resourceLootTable;
				}
				if (interactiveNpc.ResourceLootTable != null && gameEntity.CharacterData != null)
				{
					gameEntity.CharacterData.ResourceLevel = this.m_resourceLevelTier.LevelRange.Min;
				}
			}
			if (gameEntity.InfluenceSource != null)
			{
				gameEntity.InfluenceSource.InfluenceProfile = this.m_influenceProfile;
			}
		}

		// Token: 0x060033EB RID: 13291 RVA: 0x00163178 File Offset: 0x00161378
		private SpawnTier GetSpawnTier()
		{
			if (this.m_spawnTierFlags == SpawnTierFlags.None)
			{
				return SpawnTier.Normal;
			}
			float num = SolMath.Gaussian();
			if (num < -1f && this.m_spawnTierFlags.HasBitFlag(SpawnTierFlags.Weak))
			{
				return SpawnTier.Weak;
			}
			if (-1f <= num && num <= 1f && this.m_spawnTierFlags.HasBitFlag(SpawnTierFlags.Normal))
			{
				return SpawnTier.Normal;
			}
			if (1f < num && num <= 1.5f && this.m_spawnTierFlags.HasBitFlag(SpawnTierFlags.Strong))
			{
				return SpawnTier.Strong;
			}
			if (1.5f < num && num <= 2f && this.m_spawnTierFlags.HasBitFlag(SpawnTierFlags.Champion))
			{
				return SpawnTier.Champion;
			}
			if (2f < num && this.m_spawnTierFlags.HasBitFlag(SpawnTierFlags.Elite))
			{
				return SpawnTier.Elite;
			}
			return SpawnTier.Normal;
		}

		// Token: 0x060033EC RID: 13292 RVA: 0x00063AE9 File Offset: 0x00061CE9
		private IEnumerable GetWeapons()
		{
			return SolOdinUtilities.GetDropdownItems<WeaponItem>();
		}

		// Token: 0x060033ED RID: 13293 RVA: 0x000636B2 File Offset: 0x000618B2
		private IEnumerable GetCombatMasteries()
		{
			return SolOdinUtilities.GetDropdownItems<CombatMasteryArchetype>();
		}

		// Token: 0x060033EE RID: 13294 RVA: 0x000637F4 File Offset: 0x000619F4
		private IEnumerable GetMaleFemaleSpriteCollection()
		{
			return SolOdinUtilities.GetDropdownItems<MaleFemaleSpriteCollection>();
		}

		// Token: 0x060033EF RID: 13295 RVA: 0x00063AF0 File Offset: 0x00061CF0
		private IEnumerable GetSensorProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<NpcSensorProfile>();
		}

		// Token: 0x060033F0 RID: 13296 RVA: 0x00063AF7 File Offset: 0x00061CF7
		private IEnumerable GetBehaviorProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<BehaviorProfile>();
		}

		// Token: 0x060033F1 RID: 13297 RVA: 0x00063AFE File Offset: 0x00061CFE
		private IEnumerable GetInfluenceProfile()
		{
			return SolOdinUtilities.GetDropdownItems<InfluenceProfile>();
		}

		// Token: 0x060033F2 RID: 13298 RVA: 0x00063B05 File Offset: 0x00061D05
		private IEnumerable GetNpcProfile()
		{
			return SolOdinUtilities.GetDropdownItems<NpcProfile>();
		}

		// Token: 0x060033F3 RID: 13299 RVA: 0x00063B0C File Offset: 0x00061D0C
		private IEnumerable GetNameCollection()
		{
			return SolOdinUtilities.GetDropdownItems<StringScriptableProbabilityCollection>();
		}

		// Token: 0x040031BC RID: 12732
		private const float kTierScaleScatter = 0.15f;

		// Token: 0x040031BD RID: 12733
		private const string kRandomGroup = "Randomization";

		// Token: 0x040031BE RID: 12734
		[SerializeField]
		private bool m_randomizeName;

		// Token: 0x040031BF RID: 12735
		[SerializeField]
		private bool m_scaleBySpawnTier;

		// Token: 0x040031C0 RID: 12736
		private const string kMetaGroup = "Meta";

		// Token: 0x040031C1 RID: 12737
		[SerializeField]
		private StringScriptableProbabilityCollection m_namesOverride;

		// Token: 0x040031C2 RID: 12738
		[SerializeField]
		private StringProbabilityCollection m_names;

		// Token: 0x040031C3 RID: 12739
		[SerializeField]
		private string m_name;

		// Token: 0x040031C4 RID: 12740
		[SerializeField]
		private string m_title;

		// Token: 0x040031C5 RID: 12741
		[SerializeField]
		private bool m_useSpawnTierAsTitle;

		// Token: 0x040031C6 RID: 12742
		[SerializeField]
		private Faction m_faction = Faction.Neutral;

		// Token: 0x040031C7 RID: 12743
		[SerializeField]
		private InfluenceProfile m_influenceProfile;

		// Token: 0x040031C8 RID: 12744
		[SerializeField]
		private NpcProfile m_npcProfile;

		// Token: 0x040031C9 RID: 12745
		[SerializeField]
		private EnsembleProbabilityCollection m_ensembles;

		// Token: 0x040031CA RID: 12746
		[SerializeField]
		private bool m_scaleTransformBySpawnTier;

		// Token: 0x040031CB RID: 12747
		[SerializeField]
		private MinMaxFloatRange m_transformScale = new MinMaxFloatRange(0f, 0f);

		// Token: 0x040031CC RID: 12748
		[SerializeField]
		private IdentifiableSpriteProbabilityCollection m_portraits;

		// Token: 0x040031CD RID: 12749
		[SerializeField]
		private MaleFemaleSpriteCollection m_spriteCollectionOverride;

		// Token: 0x040031CE RID: 12750
		private const string kVitalsGroup = "Vitals";

		// Token: 0x040031CF RID: 12751
		[SerializeField]
		private MinMaxIntRange m_levelRange = new MinMaxIntRange(1, 1);

		// Token: 0x040031D0 RID: 12752
		[SerializeField]
		private int m_maxHealth = 25;

		// Token: 0x040031D1 RID: 12753
		[SerializeField]
		private int m_maxArmorClass = 30;

		// Token: 0x040031D2 RID: 12754
		[SerializeField]
		private int m_maxDamageAbsorption = 100;

		// Token: 0x040031D3 RID: 12755
		[SerializeField]
		private SpawnTierFlags m_spawnTierFlags;

		// Token: 0x040031D4 RID: 12756
		[SerializeField]
		private StatModifier[] m_statModifiers;

		// Token: 0x040031D5 RID: 12757
		private const string kSkillsGroup = "Skills";

		// Token: 0x040031D6 RID: 12758
		[SerializeField]
		private NpcLoadoutWithOverride m_singleLoadout;

		// Token: 0x040031D7 RID: 12759
		[SerializeField]
		private NpcLoadoutProbabilityCollection m_loadouts;

		// Token: 0x040031D8 RID: 12760
		[SerializeField]
		private bool m_weaponsAlwaysMounted;

		// Token: 0x040031D9 RID: 12761
		private const string kBehaviorGroup = "Behavior";

		// Token: 0x040031DA RID: 12762
		[SerializeField]
		private BehaviorProfile m_behaviorProfile;

		// Token: 0x040031DB RID: 12763
		[SerializeField]
		private NpcSensorProfile m_sensorProfile;

		// Token: 0x040031DC RID: 12764
		[SerializeField]
		private float m_leashDistance = 50f;

		// Token: 0x040031DD RID: 12765
		[SerializeField]
		private bool m_rotateAwayInComabat;

		// Token: 0x040031DE RID: 12766
		private const string kLootGroup = "Loot";

		// Token: 0x040031DF RID: 12767
		[SerializeField]
		private bool m_despawnOnDeath;

		// Token: 0x040031E0 RID: 12768
		[SerializeField]
		private LootProfileProbabilityCollection m_lootProfiles;

		// Token: 0x040031E1 RID: 12769
		[SerializeField]
		private LevelTier m_resourceLevelTier = new LevelTier();

		// Token: 0x040031E2 RID: 12770
		[SerializeField]
		private LootTableProfile m_lootTable;

		// Token: 0x040031E3 RID: 12771
		[SerializeField]
		private LootTableProfile m_resourceLootTable;

		// Token: 0x040031E4 RID: 12772
		[SerializeField]
		private LootTable m_lootTableV2;

		// Token: 0x040031E5 RID: 12773
		[SerializeField]
		private LootTable m_resourceLootTableV2;

		// Token: 0x020006A4 RID: 1700
		[Serializable]
		public class NpcResist
		{
			// Token: 0x040031E6 RID: 12774
			public float Value;
		}
	}
}
