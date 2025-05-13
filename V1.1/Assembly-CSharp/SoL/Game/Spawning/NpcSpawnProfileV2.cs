using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Cysharp.Text;
using Sirenix.OdinInspector;
using SoL.Game.Animation;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.HuntingLog;
using SoL.Game.Influence;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Game.NPCs;
using SoL.Game.NPCs.Interactions;
using SoL.Game.NPCs.Senses;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Randomization;
using SoL.Game.Settings;
using SoL.Game.Spawning.Behavior;
using SoL.Game.Targeting;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006A5 RID: 1701
	[CreateAssetMenu(menuName = "SoL/Profiles/Npc Spawn 2")]
	public class NpcSpawnProfileV2 : SpawnProfile
	{
		// Token: 0x060033F6 RID: 13302 RVA: 0x00063B13 File Offset: 0x00061D13
		private IEnumerable GetColorIndexDropdowns()
		{
			if (this.m_prefabReference != null && this.m_prefabReference.IndexDescriptions != null && this.m_prefabReference.IndexDescriptions.Length != 0)
			{
				int num;
				for (int i = 0; i < this.m_prefabReference.IndexDescriptions.Length; i = num + 1)
				{
					yield return new ValueDropdownItem(this.m_prefabReference.IndexDescriptions[i], i);
					num = i;
				}
			}
			yield break;
		}

		// Token: 0x060033F7 RID: 13303 RVA: 0x00163278 File Offset: 0x00161478
		private string GetDecadalScalingDescription()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				for (int i = 0; i < GlobalSettings.kDecades.Length; i++)
				{
					int num = GlobalSettings.kDecades[i];
					int arg = Mathf.FloorToInt(this.m_health.Evaluate((float)num));
					int arg2 = Mathf.FloorToInt(this.m_armorClass.Evaluate((float)num));
					int arg3 = Mathf.FloorToInt(this.m_absorption.Evaluate((float)num));
					float arg4 = (this.m_prefabReference != null) ? Mathf.Lerp(this.m_prefabReference.ScaleRange.Min, this.m_prefabReference.ScaleRange.Max, this.m_transformScale.Evaluate((float)GlobalSettings.kDecades[i])) : 0f;
					utf16ValueStringBuilder.AppendFormat<int, int, int, int, float>("[{0:00}] Health={1}, AC={2}, Absorb={3}, Scale={4:F02}", GlobalSettings.kDecades[i], arg, arg2, arg3, arg4);
					if (i != GlobalSettings.kDecades.Length - 1)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060033F8 RID: 13304 RVA: 0x00063B23 File Offset: 0x00061D23
		private string GetDetails()
		{
			return "Decadal Breakdown";
		}

		// Token: 0x060033F9 RID: 13305 RVA: 0x001633A4 File Offset: 0x001615A4
		private string GetDecadalCurrencyDescription()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				for (int i = 0; i < GlobalSettings.kDecades.Length; i++)
				{
					int num = GlobalSettings.kDecades[i];
					int num2 = Mathf.FloorToInt(this.m_minCurrency.Evaluate((float)num));
					CurrencyConverter currencyConverter = new CurrencyConverter((ulong)((long)num2));
					int num3 = Mathf.FloorToInt(this.m_maxCurrency.Evaluate((float)num));
					CurrencyConverter currencyConverter2 = new CurrencyConverter((ulong)((long)num3));
					string arg = (currencyConverter.TotalCurrency > 0UL) ? currencyConverter.ToString() : "0cp";
					string arg2 = (currencyConverter2.TotalCurrency > 0UL) ? currencyConverter2.ToString() : "0cp";
					utf16ValueStringBuilder.AppendFormat<int, string, string>("[{0:00}] ({1}) to ({2})", GlobalSettings.kDecades[i], arg, arg2);
					if (i != GlobalSettings.kDecades.Length - 1)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x060033FA RID: 13306 RVA: 0x00063B2A File Offset: 0x00061D2A
		private bool m_showNpcProfileWarning
		{
			get
			{
				return this.m_npcProfile != null;
			}
		}

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x060033FB RID: 13307 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_assignPositionUpdater
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x060033FC RID: 13308 RVA: 0x00063B38 File Offset: 0x00061D38
		private bool m_showSortOverrides
		{
			get
			{
				return this.m_overrides != null && this.m_overrides.Length != 0;
			}
		}

		// Token: 0x060033FD RID: 13309 RVA: 0x00063B4E File Offset: 0x00061D4E
		private void SortOverrides()
		{
			Array.Sort<NpcSpawnProfileV2.NpcTierWithOverrides>(this.m_overrides, (NpcSpawnProfileV2.NpcTierWithOverrides a, NpcSpawnProfileV2.NpcTierWithOverrides b) => a.LevelThreshold.CompareTo(b.LevelThreshold));
		}

		// Token: 0x060033FE RID: 13310 RVA: 0x001634B4 File Offset: 0x001616B4
		public void LoadStaticSpawnData(StaticSpawnData spawnData)
		{
			this.m_base = new NpcSpawnProfileV2.BaseNpcTier
			{
				Metadata = new NpcSpawnProfileV2.MetadataSettings(spawnData),
				Visuals = new NpcSpawnProfileV2.VisualSettings(spawnData),
				Config = new NpcSpawnProfileV2.ConfigSettings(),
				Loot = new NpcLootSettings(),
				CallForHelp = new CallForHelpProfileWithOverride(),
				Behavior = new NpcSpawnProfileV2.BaseBehaviorSettings()
			};
			this.m_fixedVitals = new NpcSpawnProfileV2.FixedVitals?(new NpcSpawnProfileV2.FixedVitals
			{
				MaxHealth = (float)spawnData.MaxHealth,
				MaxArmorClass = (float)spawnData.MaxArmorClass,
				MaxAbsorption = (float)spawnData.MaxDamageAbsorption
			});
		}

		// Token: 0x060033FF RID: 13311 RVA: 0x00163550 File Offset: 0x00161750
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			GlobalCounters.SpawnedNpcs += 1U;
			base.SpawnInternal(controller, gameEntity);
			int? num = null;
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
			int level = controller.GetLevel();
			NpcSpawnProfileV2.MetadataSettings metadataSettings = null;
			NpcSpawnProfileV2.VisualSettings visualSettings = null;
			NpcSpawnProfileV2.ConfigSettings configSettings = null;
			NpcSpawnProfileV2.BehaviorSettings behaviorSettings = null;
			NpcLootSettings npcLootSettings = null;
			ICallForHelpSettings callForHelpSettings = null;
			if (this.m_overrides != null && this.m_overrides.Length != 0)
			{
				for (int i = this.m_overrides.Length - 1; i >= 0; i--)
				{
					NpcSpawnProfileV2.NpcTierWithOverrides npcTierWithOverrides = this.m_overrides[i];
					if (npcTierWithOverrides.LevelThreshold <= level)
					{
						if (metadataSettings == null && npcTierWithOverrides.OverrideMetadata)
						{
							metadataSettings = npcTierWithOverrides.Metadata;
						}
						if (visualSettings == null && npcTierWithOverrides.OverrideVisuals)
						{
							visualSettings = npcTierWithOverrides.Visuals;
						}
						if (configSettings == null && npcTierWithOverrides.OverrideConfig)
						{
							configSettings = npcTierWithOverrides.Config;
						}
						if (behaviorSettings == null && npcTierWithOverrides.OverrideBehavior)
						{
							behaviorSettings = npcTierWithOverrides.Behavior;
						}
						if (callForHelpSettings == null && npcTierWithOverrides.OverrideCallForHelp)
						{
							callForHelpSettings = npcTierWithOverrides.GetCallForHelpSettings(controller);
						}
						if (npcLootSettings == null && npcTierWithOverrides.OverrideLoot)
						{
							npcLootSettings = npcTierWithOverrides.Loot;
						}
					}
				}
			}
			if (metadataSettings == null)
			{
				metadataSettings = this.m_base.Metadata;
			}
			if (visualSettings == null)
			{
				visualSettings = this.m_base.Visuals;
			}
			if (configSettings == null)
			{
				configSettings = this.m_base.Config;
			}
			if (behaviorSettings == null)
			{
				behaviorSettings = this.m_base.Behavior;
			}
			if (callForHelpSettings == null)
			{
				callForHelpSettings = this.m_base.GetCallForHelpSettings(controller);
			}
			if (npcLootSettings == null)
			{
				npcLootSettings = this.m_base.Loot;
			}
			else
			{
				LootTableSampleCount itemLootTable = npcLootSettings.ItemLootTable;
				if (itemLootTable != null)
				{
					itemLootTable.OverrideMinMaxCount(this.m_base.Loot.ItemLootTable);
				}
				LootTableSampleCount resourceLootTable = npcLootSettings.ResourceLootTable;
				if (resourceLootTable != null)
				{
					resourceLootTable.OverrideMinMaxCount(this.m_base.Loot.ResourceLootTable);
				}
			}
			NpcLoadoutWithOverride loadout = configSettings.GetLoadout();
			AnimancerAnimationSet animancerAnimationSet;
			if (loadout == null)
			{
				animancerAnimationSet = null;
			}
			else
			{
				NpcLoadout loadout2 = loadout.Loadout;
				animancerAnimationSet = ((loadout2 != null) ? loadout2.InitializeProfileForNpc(this.m_npcProfile, gameEntity, level, containerRecord, containerRecord2, containerRecord3) : null);
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
			gameEntity.BypassLevelDeltaCombatAdjustments = this.m_bypassLevelDeltaCombatAdjustments;
			if (gameEntity.EffectController != null)
			{
				gameEntity.EffectController.Init(record);
			}
			if (gameEntity.CollectionController != null)
			{
				gameEntity.CollectionController.Initialize(record);
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
					float f = this.m_health.Evaluate((float)level);
					float num2 = this.m_armorClass.Evaluate((float)level);
					float num3 = this.m_absorption.Evaluate((float)level);
					if (this.m_fixedVitals != null)
					{
						f = this.m_fixedVitals.Value.MaxHealth;
						num2 = this.m_fixedVitals.Value.MaxArmorClass;
						num3 = this.m_fixedVitals.Value.MaxAbsorption;
					}
					ArchetypeInstance archetypeInstance;
					IArmorClass armorClass;
					if (gameEntity.CollectionController != null && gameEntity.CollectionController.Equipment != null && gameEntity.CollectionController.Equipment.TryGetInstanceForIndex(2, out archetypeInstance) && archetypeInstance.Archetype && archetypeInstance.Archetype is ShieldItem && archetypeInstance.Archetype.TryGetAsType(out armorClass))
					{
						num2 += (float)armorClass.BaseArmorClass;
						num3 += (float)armorClass.MaxDamageAbsorption;
					}
					float maxAbsorbDamageReduction = this.m_overrideMaxAbsorbDamageReduction ? this.m_maxAbsorbDamageReduction : 0.8f;
					npcVitals_Server.SetMaxValues(Mathf.CeilToInt(f), Mathf.CeilToInt(num2), Mathf.CeilToInt(num3), maxAbsorbDamageReduction);
				}
				gameEntity.Vitals.Init(record);
				if (npcVitals_Server != null)
				{
					npcVitals_Server.ApplyStatModifiers(configSettings.StatModifiers);
				}
			}
			if (gameEntity.CharacterData != null)
			{
				gameEntity.CharacterData.MatchAttackerLevel = controller.MatchAttackerLevel;
				gameEntity.CharacterData.InitNpcLevel((controller.MatchAttackerLevel || this.m_bypassLevelDeltaCombatAdjustments) ? 0 : level);
				gameEntity.CharacterData.NpcTagsSet = NpcTagSet.Clone(behaviorSettings.TagSet);
				gameEntity.CharacterData.Faction = behaviorSettings.Faction;
				gameEntity.CharacterData.ChallengeRating = this.m_challengeRating;
				string text = metadataSettings.Name;
				string title = metadataSettings.Title;
				SpawnControllerOverrideData spawnControllerOverrideData;
				if (controller != null && controller.TryGetOverrideData(this, out spawnControllerOverrideData))
				{
					if (spawnControllerOverrideData.OverrideName)
					{
						text = spawnControllerOverrideData.Name;
					}
					if (spawnControllerOverrideData.OverrideTitle)
					{
						title = spawnControllerOverrideData.Title;
					}
					if (spawnControllerOverrideData.IsPlaceholder)
					{
						text = ZString.Format<string>("*{0}", text);
					}
				}
				gameEntity.CharacterData.Name.Value = text;
				gameEntity.CharacterData.Title.Value = title;
				gameEntity.CharacterData.CurrentCombatId.Value = (animancerAnimationSet2 ? animancerAnimationSet2.Id : UniqueId.Empty);
				NpcInitData npcInitData = new NpcInitData
				{
					BypassLevelDeltaCombatAdjustments = this.m_bypassLevelDeltaCombatAdjustments
				};
				if (this.m_npcProfile != null)
				{
					num = new int?(this.m_npcProfile.ProfileSeed);
					npcInitData.ProfileType = VisualProfileType.Individual;
					npcInitData.ProfileId = this.m_npcProfile.Id;
				}
				else
				{
					if (visualSettings.PopulationVisualsProfile != null)
					{
						npcInitData.ProfileType = VisualProfileType.Population;
						npcInitData.ProfileId = visualSettings.PopulationVisualsProfile.Id;
					}
					if (visualSettings.Ensemble != null)
					{
						npcInitData.EnsembleId = visualSettings.Ensemble.Id;
						if (visualSettings.Ensemble.HipMountedEmberStone)
						{
							gameEntity.CharacterData.VisibleEquipment.Add(512, new EquipableItemVisualData
							{
								ArchetypeId = visualSettings.Ensemble.HipMountedEmberStone.Id,
								VisualIndex = null,
								ColorIndex = null
							});
							gameEntity.CharacterData.VisibleEquipment.ResetDirty();
						}
					}
				}
				if (controller.OverrideDialogue != null)
				{
					npcInitData.OverrideDialogueId = controller.OverrideDialogue.Id;
				}
				gameEntity.CharacterData.NpcInitData = npcInitData;
				gameEntity.CharacterData.PortraitId.Value = visualSettings.GetPortraitId();
				float t = this.m_transformScale.Evaluate((float)level);
				float num4 = ((this.m_prefabReference != null) ? Mathf.Lerp(this.m_prefabReference.ScaleRange.Min, this.m_prefabReference.ScaleRange.Max, t) : 1f) * this.m_scaleMultiplier - 1f;
				if (!Mathf.Approximately(num4, 0f))
				{
					gameEntity.CharacterData.TransformScale = new float?(num4);
				}
			}
			if (gameEntity.SkillsController != null)
			{
				gameEntity.SkillsController.CacheAbilities(this.m_useHealthFractionAutoAttack, this.m_bypassLevelDeltaCombatAdjustments);
			}
			if (gameEntity.Motor != null)
			{
				NpcMotor npcMotor = gameEntity.Motor as NpcMotor;
				if (npcMotor != null)
				{
					npcMotor.OverallSpeedMultiplier = configSettings.OverallSpeedMultiplier;
					npcMotor.PathSpeedMultiplier = configSettings.PathSpeedMultiplier;
					npcMotor.RunOnPath = configSettings.RunOnPath;
				}
			}
			if (NpcSpawnProfileV2.m_externalBehaviorTrees == null)
			{
				NpcSpawnProfileV2.m_externalBehaviorTrees = new Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree>(default(BehaviorTreeNodeNameComparer));
			}
			else
			{
				NpcSpawnProfileV2.m_externalBehaviorTrees.Clear();
			}
			if (gameEntity.ServerNpcController != null)
			{
				gameEntity.ServerNpcController.DespawnOnDeath = controller.DespawnOnDeath;
				NpcInteractionFlags npcInteractionFlags;
				gameEntity.ServerNpcController.InteractionFlags = (controller.OverrideInteractionFlags(out npcInteractionFlags) ? npcInteractionFlags : behaviorSettings.InteractionFlags);
				BehaviorTree tree = gameEntity.ServerNpcController.Tree;
				if (tree != null)
				{
					NpcSpawnProfileV2.BaseBehaviorSettings behavior = this.m_base.Behavior;
					if (behavior != null)
					{
						BehaviorProfileV2WithOverrides behaviorProfile = behavior.BehaviorProfile;
						if (behaviorProfile != null)
						{
							behaviorProfile.PopulateExternalBehaviorTrees(NpcSpawnProfileV2.m_externalBehaviorTrees);
						}
					}
					NpcSpawnProfileV2.OverrideBehaviorSettings overrideBehaviorSettings = behaviorSettings as NpcSpawnProfileV2.OverrideBehaviorSettings;
					if (overrideBehaviorSettings != null)
					{
						BehaviorSubTreeCollectionWithOverride behavior2 = overrideBehaviorSettings.Behavior;
						if (behavior2 != null)
						{
							behavior2.PopulateSubTrees(NpcSpawnProfileV2.m_externalBehaviorTrees);
						}
					}
					BehaviorSubTreeCollection behaviorOverrides = controller.BehaviorOverrides;
					if (behaviorOverrides != null)
					{
						behaviorOverrides.PopulateSubTrees(NpcSpawnProfileV2.m_externalBehaviorTrees);
					}
					tree.StartWhenEnabled = false;
					Behavior behavior3 = tree;
					NpcSpawnProfileV2.BaseBehaviorSettings behavior4 = this.m_base.Behavior;
					behavior3.ExternalBehavior = ((behavior4 != null) ? behavior4.BaseTree : null);
					foreach (KeyValuePair<BehaviorTreeNodeName, ExternalBehaviorTree> keyValuePair in NpcSpawnProfileV2.m_externalBehaviorTrees)
					{
						if (keyValuePair.Key != BehaviorTreeNodeName.None && keyValuePair.Value != null)
						{
							tree.LoadExternalSubTree(keyValuePair.Key, keyValuePair.Value);
						}
					}
					SharedVariable variable = tree.GetVariable("LeashDistance");
					if (variable != null)
					{
						NpcSpawnProfileV2.BaseNpcTier @base = this.m_base;
						float? num5;
						if (@base == null)
						{
							num5 = null;
						}
						else
						{
							NpcSpawnProfileV2.BaseBehaviorSettings behavior5 = @base.Behavior;
							num5 = ((behavior5 != null) ? new float?(behavior5.DefaultLeashDistance) : null);
						}
						float? num6 = num5;
						float num7 = (controller.LeashDistance != null) ? controller.LeashDistance.Value : num6.Value;
						variable.SetValue(num7);
						SharedVariable variable2 = tree.GetVariable("ResetDistance");
						if (variable2 != null)
						{
							float num8 = (controller.ResetDistance != null) ? Mathf.Max(controller.ResetDistance.Value, num7 * 1.1f) : Mathf.Max(GlobalSettings.Values.Npcs.ResetDistanceMinimum, num7 * GlobalSettings.Values.Npcs.ResetDistanceLeashMultiplier);
							variable2.SetValue(num8);
						}
					}
					tree.enabled = true;
				}
			}
			NpcTargetController npcTargetController;
			if (gameEntity.TargetController != null && gameEntity.TargetController.TryGetAsType(out npcTargetController))
			{
				if (behaviorSettings.IndoorSensorProfile && ((ZoneSettings.SettingsProfile && ZoneSettings.SettingsProfile.IndoorSensorProfiles) || controller.ForceIndoorProfiles))
				{
					npcTargetController.SensorProfile = behaviorSettings.IndoorSensorProfile;
				}
				else
				{
					npcTargetController.SensorProfile = behaviorSettings.SensorProfile;
				}
				npcTargetController.RotateAwayInCombat = false;
				npcTargetController.CallForHelpSettings = callForHelpSettings;
				npcTargetController.CallForHelpRequiresLos = controller.CallForHelpRequiresLos;
				npcTargetController.IsHostileToTags = behaviorSettings.IsHostileTo;
				npcTargetController.NpcIgnoreGuards = behaviorSettings.IgnoreGuards;
				if (this.m_assignPositionUpdater)
				{
					npcTargetController.SpawnControllerPositionUpdater = controller;
				}
			}
			if (gameEntity.CallForHelp != null)
			{
				gameEntity.CallForHelp.CallForHelpSettings = callForHelpSettings;
				gameEntity.CallForHelp.CallForHelpRequiresLos = controller.CallForHelpRequiresLos;
			}
			if (gameEntity.NpcStanceManager != null)
			{
				gameEntity.NpcStanceManager.CombatStance = animancerAnimationSet2;
				NpcStanceManager npcStanceManager = gameEntity.NpcStanceManager;
				CombatMasteryArchetype combatMastery;
				if (loadout == null)
				{
					combatMastery = null;
				}
				else
				{
					NpcLoadout loadout3 = loadout.Loadout;
					combatMastery = ((loadout3 != null) ? loadout3.CombatMastery : null);
				}
				npcStanceManager.CombatMastery = combatMastery;
				gameEntity.NpcStanceManager.WeaponsAlwaysMounted = visualSettings.WeaponsAlwaysMounted;
				NpcStanceManager npcStanceManager2 = gameEntity.NpcStanceManager;
				ICollectionController collectionController = gameEntity.CollectionController;
				ArchetypeInstance archetypeInstance2;
				npcStanceManager2.HasLight = (((collectionController != null) ? collectionController.Equipment : null) != null && gameEntity.CollectionController.Equipment.TryGetInstanceForIndex(256, out archetypeInstance2) && archetypeInstance2.Archetype && archetypeInstance2.Archetype is LightItem);
			}
			InteractiveNpc interactiveNpc;
			if (gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveNpc))
			{
				interactiveNpc.SpawnController = controller;
				interactiveNpc.SpawnProfile = this;
				interactiveNpc.IsAshen = this.m_isAshen;
				interactiveNpc.AlwaysGoAshen = (!this.m_isAshen && this.m_alwaysGoAshen);
				interactiveNpc.AshenSpawnProfile = (this.m_isAshen ? null : this.m_ashenSpawnProfile);
				interactiveNpc.Level = level;
				interactiveNpc.LogLoot = npcLootSettings.LogLoot;
				interactiveNpc.SetHuntingLogProfileOverride(this.m_huntingLogProfileOverride);
				if (!controller.DespawnOnDeath)
				{
					interactiveNpc.SpawnTier = SpawnTier.Normal;
					interactiveNpc.ItemTable = npcLootSettings.ItemLootTable;
					GatheringAbility gatheringAbility;
					if (npcLootSettings.ResourceLootTable != null && npcLootSettings.ResourceLootTable.Table != null && npcLootSettings.RequiredTool != CraftingToolType.None && npcLootSettings.RequiredTool.TryGetAbilityForToolType(out gatheringAbility))
					{
						interactiveNpc.ResourceTable = npcLootSettings.ResourceLootTable;
						interactiveNpc.GatheringParams = new GatheringParameters
						{
							RequiredTool = npcLootSettings.RequiredTool,
							AbilityId = gatheringAbility.Id,
							GatherTime = npcLootSettings.GatherTime,
							Level = npcLootSettings.ResourceLevel
						};
					}
					interactiveNpc.ChallengeRating = this.m_challengeRating;
					interactiveNpc.ExtendCorpseDecay = this.m_extendCorpseDecay;
					if (this.m_zeroXp)
					{
						interactiveNpc.XpAdjustmentMultiplier = new float?(0f);
					}
					else if (this.m_xpAdjustment != 0 || controller.XpAdjustment != 0)
					{
						int num9 = this.m_xpAdjustment + controller.XpAdjustment;
						interactiveNpc.XpAdjustmentMultiplier = new float?(1f + (float)num9 * 0.01f);
					}
					interactiveNpc.Currency = new MinMaxIntRange(Mathf.FloorToInt(this.m_minCurrency.Evaluate((float)level)), Mathf.FloorToInt(this.m_maxCurrency.Evaluate((float)level)));
					if (this.m_includeEventCurrency && this.m_eventCurrency > 0U)
					{
						interactiveNpc.EventCurrency = (ulong)this.m_eventCurrency;
					}
				}
			}
			InteractiveMerchantNpc interactiveMerchantNpc;
			if (gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveMerchantNpc))
			{
				interactiveMerchantNpc.MerchantBundleCollection = this.m_merchantBundleCollection;
				interactiveMerchantNpc.MerchantBundles = this.m_merchantBundles;
				interactiveMerchantNpc.MerchantType = this.m_merchantType;
				interactiveMerchantNpc.ItemFlagsToSet = this.m_merchantItemFlagsToSet;
				interactiveMerchantNpc.MarkAsSoulbound = this.m_merchantMarkAsSoulbound;
			}
			if (gameEntity.SeedReplicator != null)
			{
				int num10;
				if (this.m_visualIndex != null && this.m_visualIndex.TryGetVisualIndex(this.m_prefabReference, out num10))
				{
					gameEntity.SeedReplicator.VisualIndexOverride = new byte?((byte)num10);
				}
				if (num != null)
				{
					gameEntity.SeedReplicator.Seed = num.Value;
				}
			}
			if (gameEntity.InfluenceSource != null)
			{
				gameEntity.InfluenceSource.InfluenceProfile = this.m_influenceProfile;
			}
		}

		// Token: 0x06003400 RID: 13312 RVA: 0x00063AFE File Offset: 0x00061CFE
		private IEnumerable GetInfluenceProfile()
		{
			return SolOdinUtilities.GetDropdownItems<InfluenceProfile>();
		}

		// Token: 0x06003401 RID: 13313 RVA: 0x00063B05 File Offset: 0x00061D05
		private IEnumerable GetNpcProfile()
		{
			return SolOdinUtilities.GetDropdownItems<NpcProfile>();
		}

		// Token: 0x06003402 RID: 13314 RVA: 0x00063B7A File Offset: 0x00061D7A
		private IEnumerable GetMerchantBundleCollection()
		{
			return SolOdinUtilities.GetDropdownItems<MerchantBundleCollection>();
		}

		// Token: 0x06003403 RID: 13315 RVA: 0x00063B81 File Offset: 0x00061D81
		private IEnumerable GetMerchantBundle()
		{
			return SolOdinUtilities.GetDropdownItems<MerchantBundle>();
		}

		// Token: 0x040031E7 RID: 12775
		public const string kXpAdjustmentTooltip = "% XP Adjustment. Spawn Profile values stack with Spawn Controller values.  25=+25%";

		// Token: 0x040031E8 RID: 12776
		private const string kScalingGroup = "Scaling Properties";

		// Token: 0x040031E9 RID: 12777
		private const string kCurrencyGroup = "Currency";

		// Token: 0x040031EA RID: 12778
		private const string kNpcGroup = "Npc Stuff";

		// Token: 0x040031EB RID: 12779
		private const string kNpcMerchantGroup = "Npc Stuff/Merchant";

		// Token: 0x040031EC RID: 12780
		private const string kXpModifier = "Npc Stuff/Experience";

		// Token: 0x040031ED RID: 12781
		private const string kAbsorbDamageReduction = "Npc Stuff/AbsorbDamageReduction";

		// Token: 0x040031EE RID: 12782
		private const string kColorIndex = "Color Index";

		// Token: 0x040031EF RID: 12783
		private const float kDefaultHealth = 20f;

		// Token: 0x040031F0 RID: 12784
		private const float kDefaultArmorClass = 200f;

		// Token: 0x040031F1 RID: 12785
		private const float kDefaultAbsorption = 200f;

		// Token: 0x040031F2 RID: 12786
		private static Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree> m_externalBehaviorTrees;

		// Token: 0x040031F3 RID: 12787
		private const int kMaxColorIndex = 10;

		// Token: 0x040031F4 RID: 12788
		[SerializeField]
		private VisualIndex m_visualIndex;

		// Token: 0x040031F5 RID: 12789
		[SerializeField]
		private float m_scaleMultiplier = 1f;

		// Token: 0x040031F6 RID: 12790
		[SerializeField]
		private AnimationCurve m_transformScale = AnimationCurve.Linear(1f, 0f, 50f, 1f);

		// Token: 0x040031F7 RID: 12791
		[SerializeField]
		private AnimationCurve m_health = AnimationCurve.Linear(1f, 20f, 50f, 20f);

		// Token: 0x040031F8 RID: 12792
		[SerializeField]
		private AnimationCurve m_armorClass = AnimationCurve.Linear(1f, 200f, 50f, 200f);

		// Token: 0x040031F9 RID: 12793
		[SerializeField]
		private AnimationCurve m_absorption = AnimationCurve.Linear(1f, 200f, 50f, 200f);

		// Token: 0x040031FA RID: 12794
		[SerializeField]
		private DummyClass m_scalingDummy;

		// Token: 0x040031FB RID: 12795
		[SerializeField]
		private bool m_includeEventCurrency;

		// Token: 0x040031FC RID: 12796
		[SerializeField]
		private uint m_eventCurrency;

		// Token: 0x040031FD RID: 12797
		[SerializeField]
		private AnimationCurve m_minCurrency = AnimationCurve.Linear(1f, 0f, 50f, 0f);

		// Token: 0x040031FE RID: 12798
		[SerializeField]
		private AnimationCurve m_maxCurrency = AnimationCurve.Linear(1f, 5f, 50f, 200f);

		// Token: 0x040031FF RID: 12799
		[SerializeField]
		private DummyClass m_currencyDummy;

		// Token: 0x04003200 RID: 12800
		[SerializeField]
		private NpcProfile m_npcProfile;

		// Token: 0x04003201 RID: 12801
		[SerializeField]
		private DummyClass m_dummy2;

		// Token: 0x04003202 RID: 12802
		[SerializeField]
		private InfluenceProfile m_influenceProfile;

		// Token: 0x04003203 RID: 12803
		[SerializeField]
		private bool m_isAshen;

		// Token: 0x04003204 RID: 12804
		[SerializeField]
		private bool m_alwaysGoAshen;

		// Token: 0x04003205 RID: 12805
		[Tooltip("If selected the NPC's auto attack will be based on the target's health fraction")]
		[SerializeField]
		private bool m_useHealthFractionAutoAttack;

		// Token: 0x04003206 RID: 12806
		[Tooltip("If selected the NPC will be treated equal for all players, and treat all players equal, regardless of level delta")]
		[SerializeField]
		private bool m_bypassLevelDeltaCombatAdjustments;

		// Token: 0x04003207 RID: 12807
		[SerializeField]
		private NpcSpawnProfileV2 m_ashenSpawnProfile;

		// Token: 0x04003208 RID: 12808
		[SerializeField]
		private HuntingLogProfile m_huntingLogProfileOverride;

		// Token: 0x04003209 RID: 12809
		[SerializeField]
		private MerchantType m_merchantType;

		// Token: 0x0400320A RID: 12810
		[SerializeField]
		private ItemFlags m_merchantItemFlagsToSet;

		// Token: 0x0400320B RID: 12811
		[SerializeField]
		private bool m_merchantMarkAsSoulbound;

		// Token: 0x0400320C RID: 12812
		[SerializeField]
		private MerchantBundleCollection m_merchantBundleCollection;

		// Token: 0x0400320D RID: 12813
		[SerializeField]
		private MerchantBundle[] m_merchantBundles;

		// Token: 0x0400320E RID: 12814
		[SerializeField]
		private ChallengeRating m_challengeRating = ChallengeRating.CR1;

		// Token: 0x0400320F RID: 12815
		[Tooltip("Double the corpse decay time")]
		[SerializeField]
		private bool m_extendCorpseDecay;

		// Token: 0x04003210 RID: 12816
		[SerializeField]
		private bool m_zeroXp;

		// Token: 0x04003211 RID: 12817
		[Tooltip("% XP Adjustment. Spawn Profile values stack with Spawn Controller values.  25=+25%")]
		[SerializeField]
		private int m_xpAdjustment;

		// Token: 0x04003212 RID: 12818
		[SerializeField]
		private bool m_overrideMaxAbsorbDamageReduction;

		// Token: 0x04003213 RID: 12819
		[Range(0f, 1f)]
		[SerializeField]
		private float m_maxAbsorbDamageReduction = 0.8f;

		// Token: 0x04003214 RID: 12820
		[SerializeField]
		private NpcSpawnProfileV2.BaseNpcTier m_base;

		// Token: 0x04003215 RID: 12821
		[SerializeField]
		private NpcSpawnProfileV2.NpcTierWithOverrides[] m_overrides;

		// Token: 0x04003216 RID: 12822
		private NpcSpawnProfileV2.FixedVitals? m_fixedVitals;

		// Token: 0x020006A6 RID: 1702
		private struct FixedVitals
		{
			// Token: 0x04003217 RID: 12823
			public float MaxHealth;

			// Token: 0x04003218 RID: 12824
			public float MaxArmorClass;

			// Token: 0x04003219 RID: 12825
			public float MaxAbsorption;
		}

		// Token: 0x020006A7 RID: 1703
		[Serializable]
		internal class MetadataSettings
		{
			// Token: 0x06003405 RID: 13317 RVA: 0x00044765 File Offset: 0x00042965
			public MetadataSettings()
			{
			}

			// Token: 0x06003406 RID: 13318 RVA: 0x00063B88 File Offset: 0x00061D88
			public MetadataSettings(StaticSpawnData staticSpawnData)
			{
				if (staticSpawnData != null)
				{
					this.Name = staticSpawnData.Name;
					this.Title = staticSpawnData.Title;
				}
			}

			// Token: 0x0400321A RID: 12826
			private const string kGroupName = "Metadata";

			// Token: 0x0400321B RID: 12827
			public string Name;

			// Token: 0x0400321C RID: 12828
			public string Title;
		}

		// Token: 0x020006A8 RID: 1704
		[Serializable]
		private class VisualSettings
		{
			// Token: 0x17000B3F RID: 2879
			// (get) Token: 0x06003407 RID: 13319 RVA: 0x00063BAB File Offset: 0x00061DAB
			private bool m_showPortraits
			{
				get
				{
					return this.m_portraitOverride == null;
				}
			}

			// Token: 0x06003408 RID: 13320 RVA: 0x00044765 File Offset: 0x00042965
			public VisualSettings()
			{
			}

			// Token: 0x06003409 RID: 13321 RVA: 0x00063BB9 File Offset: 0x00061DB9
			public VisualSettings(StaticSpawnData staticSpawnData)
			{
				if (staticSpawnData != null)
				{
					this.m_portraitConfig = staticSpawnData.PortraitConfig;
				}
			}

			// Token: 0x0600340A RID: 13322 RVA: 0x00063BD0 File Offset: 0x00061DD0
			public UniqueId GetPortraitId()
			{
				if (this.m_portraitConfig == null)
				{
					return UniqueId.Empty;
				}
				return this.m_portraitConfig.GetPortraitId();
			}

			// Token: 0x0600340B RID: 13323 RVA: 0x000637F4 File Offset: 0x000619F4
			private IEnumerable GetMaleFemaleSpriteCollection()
			{
				return SolOdinUtilities.GetDropdownItems<MaleFemaleSpriteCollection>();
			}

			// Token: 0x0600340C RID: 13324 RVA: 0x00063BEB File Offset: 0x00061DEB
			private IEnumerable GetEnsembles()
			{
				return SolOdinUtilities.GetDropdownItems<WardrobeRecipePairEnsemble>();
			}

			// Token: 0x0600340D RID: 13325 RVA: 0x00063BF2 File Offset: 0x00061DF2
			public void NormalizeProbabilities()
			{
				PortraitConfig portraitConfig = this.m_portraitConfig;
				if (portraitConfig == null)
				{
					return;
				}
				portraitConfig.Normalize();
			}

			// Token: 0x0400321D RID: 12829
			private const string kGroupName = "Visuals";

			// Token: 0x0400321E RID: 12830
			private const string kPortraitGroupNameOld = "Visuals/Portraits (OLD)";

			// Token: 0x0400321F RID: 12831
			private const string kPortraitGroupName = "Visuals/Portraits";

			// Token: 0x04003220 RID: 12832
			public bool WeaponsAlwaysMounted;

			// Token: 0x04003221 RID: 12833
			public WardrobeRecipePairEnsemble Ensemble;

			// Token: 0x04003222 RID: 12834
			public NpcPopulationVisualsProfile PopulationVisualsProfile;

			// Token: 0x04003223 RID: 12835
			[SerializeField]
			private MaleFemaleSpriteCollection m_portraitOverride;

			// Token: 0x04003224 RID: 12836
			[SerializeField]
			private PortraitConfig m_portraitConfig;
		}

		// Token: 0x020006A9 RID: 1705
		[Serializable]
		private class ConfigSettings
		{
			// Token: 0x0600340E RID: 13326 RVA: 0x001644CC File Offset: 0x001626CC
			public NpcLoadoutWithOverride GetLoadout()
			{
				NpcLoadoutWithOverride result = null;
				NpcSpawnProfileV2.ConfigSettings.LoadoutType loadoutType = this.m_loadoutType;
				if (loadoutType != NpcSpawnProfileV2.ConfigSettings.LoadoutType.Single)
				{
					if (loadoutType == NpcSpawnProfileV2.ConfigSettings.LoadoutType.Collection)
					{
						NpcLoadoutProbabilityCollection loadouts = this.m_loadouts;
						NpcLoadoutProbabilityEntry npcLoadoutProbabilityEntry = (loadouts != null) ? loadouts.GetEntry(null, false) : null;
						result = ((npcLoadoutProbabilityEntry != null) ? npcLoadoutProbabilityEntry.Obj : null);
					}
				}
				else
				{
					result = this.m_loadout;
				}
				return result;
			}

			// Token: 0x0600340F RID: 13327 RVA: 0x00063C04 File Offset: 0x00061E04
			public void NormalizeProbabilities()
			{
				NpcLoadoutProbabilityCollection loadouts = this.m_loadouts;
				if (loadouts == null)
				{
					return;
				}
				loadouts.Normalize();
			}

			// Token: 0x04003225 RID: 12837
			private const string kGroupName = "Config";

			// Token: 0x04003226 RID: 12838
			private const string kLoadoutGroup = "Config/Loadouts";

			// Token: 0x04003227 RID: 12839
			[Tooltip("This multiplier is applied before the path speed multiplier")]
			public float OverallSpeedMultiplier = 1f;

			// Token: 0x04003228 RID: 12840
			[Tooltip("This multiplier is applied to the final speed of the NPC on a path. If RunOnPath is true then this will be multiplied to the run speed.")]
			public float PathSpeedMultiplier = 1f;

			// Token: 0x04003229 RID: 12841
			[Tooltip("False means the npc will walk, True means they will run.")]
			public bool RunOnPath;

			// Token: 0x0400322A RID: 12842
			public StatModifier[] StatModifiers;

			// Token: 0x0400322B RID: 12843
			[SerializeField]
			private NpcSpawnProfileV2.ConfigSettings.LoadoutType m_loadoutType;

			// Token: 0x0400322C RID: 12844
			[SerializeField]
			private NpcLoadoutWithOverride m_loadout;

			// Token: 0x0400322D RID: 12845
			[SerializeField]
			private NpcLoadoutProbabilityCollection m_loadouts;

			// Token: 0x020006AA RID: 1706
			private enum LoadoutType
			{
				// Token: 0x0400322F RID: 12847
				Single,
				// Token: 0x04003230 RID: 12848
				Collection
			}
		}

		// Token: 0x020006AB RID: 1707
		[Serializable]
		private abstract class BehaviorSettings
		{
			// Token: 0x06003411 RID: 13329 RVA: 0x00063AF0 File Offset: 0x00061CF0
			private IEnumerable GetNpcSensorProfiles()
			{
				return SolOdinUtilities.GetDropdownItems<NpcSensorProfile>();
			}

			// Token: 0x04003231 RID: 12849
			protected const string kGroupName = "Behavior";

			// Token: 0x04003232 RID: 12850
			private const string kTagsGroup = "Behavior/Tags";

			// Token: 0x04003233 RID: 12851
			public Faction Faction = Faction.Neutral;

			// Token: 0x04003234 RID: 12852
			public NpcInteractionFlags InteractionFlags;

			// Token: 0x04003235 RID: 12853
			public NpcTags Tags;

			// Token: 0x04003236 RID: 12854
			public NpcTagSet TagSet;

			// Token: 0x04003237 RID: 12855
			public NpcTagMatch IsHostileTo;

			// Token: 0x04003238 RID: 12856
			[Tooltip("If set then this npc will not be attacked by guards, nor attack guards.")]
			public bool IgnoreGuards;

			// Token: 0x04003239 RID: 12857
			public NpcSensorProfile SensorProfile;

			// Token: 0x0400323A RID: 12858
			public NpcSensorProfile IndoorSensorProfile;
		}

		// Token: 0x020006AC RID: 1708
		[Serializable]
		private class BaseBehaviorSettings : NpcSpawnProfileV2.BehaviorSettings
		{
			// Token: 0x06003413 RID: 13331 RVA: 0x00060BBC File Offset: 0x0005EDBC
			private IEnumerable GetTree()
			{
				return SolOdinUtilities.GetDropdownItems<ExternalBehaviorTree>();
			}

			// Token: 0x0400323B RID: 12859
			public float DefaultLeashDistance = 20f;

			// Token: 0x0400323C RID: 12860
			public ExternalBehaviorTree BaseTree;

			// Token: 0x0400323D RID: 12861
			public BehaviorProfileV2WithOverrides BehaviorProfile;
		}

		// Token: 0x020006AD RID: 1709
		[Serializable]
		private class OverrideBehaviorSettings : NpcSpawnProfileV2.BehaviorSettings
		{
			// Token: 0x0400323E RID: 12862
			public BehaviorSubTreeCollectionWithOverride Behavior;
		}

		// Token: 0x020006AE RID: 1710
		[Serializable]
		private abstract class NpcTier
		{
			// Token: 0x17000B40 RID: 2880
			// (get) Token: 0x06003416 RID: 13334 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowMetadata
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000B41 RID: 2881
			// (get) Token: 0x06003417 RID: 13335 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowVisuals
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000B42 RID: 2882
			// (get) Token: 0x06003418 RID: 13336 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowConfig
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000B43 RID: 2883
			// (get) Token: 0x06003419 RID: 13337 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowLoot
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000B44 RID: 2884
			// (get) Token: 0x0600341A RID: 13338 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowCallForHelp
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000B45 RID: 2885
			// (get) Token: 0x0600341B RID: 13339 RVA: 0x00063C5E File Offset: 0x00061E5E
			private bool ShowIndoorIndoorCallForHelp
			{
				get
				{
					return this.ShowCallForHelp && this.m_hasIndoorCallForHelp;
				}
			}

			// Token: 0x0600341C RID: 13340 RVA: 0x00164518 File Offset: 0x00162718
			public CallForHelpProfileWithOverride GetCallForHelpSettings(ISpawnController controller)
			{
				if (this.m_hasIndoorCallForHelp && this.CallForHelpIndoor != null && ((ZoneSettings.SettingsProfile && ZoneSettings.SettingsProfile.IndoorCallForHelp) || (controller != null && controller.ForceIndoorProfiles)))
				{
					return this.CallForHelpIndoor;
				}
				return this.CallForHelp;
			}

			// Token: 0x0600341D RID: 13341 RVA: 0x00164568 File Offset: 0x00162768
			public void NormalizeProbabilities()
			{
				NpcSpawnProfileV2.VisualSettings visuals = this.Visuals;
				if (visuals != null)
				{
					visuals.NormalizeProbabilities();
				}
				NpcSpawnProfileV2.ConfigSettings config = this.Config;
				if (config != null)
				{
					config.NormalizeProbabilities();
				}
				CallForHelpProfileWithOverride callForHelp = this.CallForHelp;
				if (callForHelp != null)
				{
					callForHelp.NormalizeProbabilities();
				}
				CallForHelpProfileWithOverride callForHelpIndoor = this.CallForHelpIndoor;
				if (callForHelpIndoor == null)
				{
					return;
				}
				callForHelpIndoor.NormalizeProbabilities();
			}

			// Token: 0x0400323F RID: 12863
			private const string kCallForHelp = "Call For Help";

			// Token: 0x04003240 RID: 12864
			protected const int kMetadataOrder = 10;

			// Token: 0x04003241 RID: 12865
			protected const int kVisualsOrder = 20;

			// Token: 0x04003242 RID: 12866
			protected const int kConfigOrder = 30;

			// Token: 0x04003243 RID: 12867
			protected const int kBehaviorOrder = 40;

			// Token: 0x04003244 RID: 12868
			protected const int kCallForHelpOrder = 50;

			// Token: 0x04003245 RID: 12869
			protected const int kLootOrder = 60;

			// Token: 0x04003246 RID: 12870
			public NpcSpawnProfileV2.MetadataSettings Metadata;

			// Token: 0x04003247 RID: 12871
			public NpcSpawnProfileV2.VisualSettings Visuals;

			// Token: 0x04003248 RID: 12872
			public NpcSpawnProfileV2.ConfigSettings Config;

			// Token: 0x04003249 RID: 12873
			public NpcLootSettings Loot;

			// Token: 0x0400324A RID: 12874
			public CallForHelpProfileWithOverride CallForHelp;

			// Token: 0x0400324B RID: 12875
			[SerializeField]
			private bool m_hasIndoorCallForHelp;

			// Token: 0x0400324C RID: 12876
			public CallForHelpProfileWithOverride CallForHelpIndoor;
		}

		// Token: 0x020006AF RID: 1711
		[Serializable]
		private class BaseNpcTier : NpcSpawnProfileV2.NpcTier
		{
			// Token: 0x0400324D RID: 12877
			public NpcSpawnProfileV2.BaseBehaviorSettings Behavior;
		}

		// Token: 0x020006B0 RID: 1712
		[Serializable]
		private class NpcTierWithOverrides : NpcSpawnProfileV2.NpcTier
		{
			// Token: 0x17000B46 RID: 2886
			// (get) Token: 0x06003420 RID: 13344 RVA: 0x00063C78 File Offset: 0x00061E78
			public int LevelThreshold
			{
				get
				{
					return this.m_levelThreshold;
				}
			}

			// Token: 0x17000B47 RID: 2887
			// (get) Token: 0x06003421 RID: 13345 RVA: 0x00063C80 File Offset: 0x00061E80
			public bool OverrideMetadata
			{
				get
				{
					return this.m_overrideMetadata;
				}
			}

			// Token: 0x17000B48 RID: 2888
			// (get) Token: 0x06003422 RID: 13346 RVA: 0x00063C88 File Offset: 0x00061E88
			public bool OverrideVisuals
			{
				get
				{
					return this.m_overrideVisuals;
				}
			}

			// Token: 0x17000B49 RID: 2889
			// (get) Token: 0x06003423 RID: 13347 RVA: 0x00063C90 File Offset: 0x00061E90
			public bool OverrideConfig
			{
				get
				{
					return this.m_overrideConfig;
				}
			}

			// Token: 0x17000B4A RID: 2890
			// (get) Token: 0x06003424 RID: 13348 RVA: 0x00063C98 File Offset: 0x00061E98
			public bool OverrideBehavior
			{
				get
				{
					return this.m_overrideBehavior;
				}
			}

			// Token: 0x17000B4B RID: 2891
			// (get) Token: 0x06003425 RID: 13349 RVA: 0x00063CA0 File Offset: 0x00061EA0
			public bool OverrideLoot
			{
				get
				{
					return this.m_overrideLoot;
				}
			}

			// Token: 0x17000B4C RID: 2892
			// (get) Token: 0x06003426 RID: 13350 RVA: 0x00063CA8 File Offset: 0x00061EA8
			public bool OverrideCallForHelp
			{
				get
				{
					return this.m_overrideCallForHelp;
				}
			}

			// Token: 0x17000B4D RID: 2893
			// (get) Token: 0x06003427 RID: 13351 RVA: 0x00063C80 File Offset: 0x00061E80
			protected override bool ShowMetadata
			{
				get
				{
					return this.m_overrideMetadata;
				}
			}

			// Token: 0x17000B4E RID: 2894
			// (get) Token: 0x06003428 RID: 13352 RVA: 0x00063C88 File Offset: 0x00061E88
			protected override bool ShowVisuals
			{
				get
				{
					return this.m_overrideVisuals;
				}
			}

			// Token: 0x17000B4F RID: 2895
			// (get) Token: 0x06003429 RID: 13353 RVA: 0x00063C90 File Offset: 0x00061E90
			protected override bool ShowConfig
			{
				get
				{
					return this.m_overrideConfig;
				}
			}

			// Token: 0x17000B50 RID: 2896
			// (get) Token: 0x0600342A RID: 13354 RVA: 0x00063CA0 File Offset: 0x00061EA0
			protected override bool ShowLoot
			{
				get
				{
					return this.m_overrideLoot;
				}
			}

			// Token: 0x17000B51 RID: 2897
			// (get) Token: 0x0600342B RID: 13355 RVA: 0x00063CA8 File Offset: 0x00061EA8
			protected override bool ShowCallForHelp
			{
				get
				{
					return this.m_overrideCallForHelp;
				}
			}

			// Token: 0x0400324E RID: 12878
			[Range(1f, 50f)]
			[SerializeField]
			private int m_levelThreshold = 1;

			// Token: 0x0400324F RID: 12879
			[SerializeField]
			private bool m_overrideMetadata;

			// Token: 0x04003250 RID: 12880
			[SerializeField]
			private bool m_overrideVisuals;

			// Token: 0x04003251 RID: 12881
			[SerializeField]
			private bool m_overrideConfig;

			// Token: 0x04003252 RID: 12882
			[SerializeField]
			private bool m_overrideBehavior;

			// Token: 0x04003253 RID: 12883
			[SerializeField]
			private bool m_overrideLoot;

			// Token: 0x04003254 RID: 12884
			[SerializeField]
			private bool m_overrideCallForHelp;

			// Token: 0x04003255 RID: 12885
			public NpcSpawnProfileV2.OverrideBehaviorSettings Behavior;
		}
	}
}
