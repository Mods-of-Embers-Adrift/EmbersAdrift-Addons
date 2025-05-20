using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000814 RID: 2068
	public class NpcSkillsController : SkillsController, ITargetData
	{
		// Token: 0x17000DC6 RID: 3526
		// (get) Token: 0x06003BDF RID: 15327 RVA: 0x00062B3E File Offset: 0x00060D3E
		GameEntity ITargetData.Self
		{
			get
			{
				return base.GameEntity;
			}
		}

		// Token: 0x17000DC7 RID: 3527
		// (get) Token: 0x06003BE0 RID: 15328 RVA: 0x00068831 File Offset: 0x00066A31
		GameEntity ITargetData.Offensive
		{
			get
			{
				return base.GameEntity.TargetController.OffensiveTarget;
			}
		}

		// Token: 0x17000DC8 RID: 3528
		// (get) Token: 0x06003BE1 RID: 15329 RVA: 0x00068843 File Offset: 0x00066A43
		GameEntity ITargetData.Defensive
		{
			get
			{
				return base.GameEntity.TargetController.DefensiveTarget;
			}
		}

		// Token: 0x17000DC9 RID: 3529
		// (get) Token: 0x06003BE2 RID: 15330 RVA: 0x00164B40 File Offset: 0x00162D40
		Vector3? ITargetData.GroundPosition
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000DCA RID: 3530
		// (get) Token: 0x06003BE3 RID: 15331 RVA: 0x00068855 File Offset: 0x00066A55
		internal GameObject PrimaryTargetPoint
		{
			get
			{
				if (!base.GameEntity)
				{
					return base.gameObject;
				}
				return base.GameEntity.PrimaryTargetPoint;
			}
		}

		// Token: 0x17000DCB RID: 3531
		// (get) Token: 0x06003BE4 RID: 15332 RVA: 0x00068876 File Offset: 0x00066A76
		internal float PrimaryTargetPointDistance
		{
			get
			{
				if (!base.GameEntity)
				{
					return 0f;
				}
				return base.GameEntity.PrimaryTargetPointDistance;
			}
		}

		// Token: 0x06003BE5 RID: 15333 RVA: 0x00068896 File Offset: 0x00066A96
		private void Start()
		{
			this.m_executionCache = StaticPool<ExecutionCache>.GetFromPool();
			this.m_executionCache.InitNpc(base.GameEntity);
			this.m_dynamicProbabilityCollection = StaticPool<DynamicProbabilityCollection<NpcSkillsController.AbilityEntry>>.GetFromPool();
		}

		// Token: 0x06003BE6 RID: 15334 RVA: 0x0017D2F8 File Offset: 0x0017B4F8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_executionCache != null)
			{
				StaticPool<ExecutionCache>.ReturnToPool(this.m_executionCache);
				this.m_executionCache = null;
			}
			if (this.m_combatAbilities != null)
			{
				StaticListPool<ArchetypeInstance>.ReturnToPool(this.m_combatAbilities);
				this.m_combatAbilities = null;
			}
			if (this.m_pursueAbilities != null)
			{
				StaticListPool<ArchetypeInstance>.ReturnToPool(this.m_pursueAbilities);
				this.m_pursueAbilities = null;
			}
			if (this.m_autoAttackInstance != null)
			{
				StaticPool<ArchetypeInstance>.ReturnToPool(this.m_autoAttackInstance);
				this.m_autoAttackInstance = null;
			}
			if (this.m_dynamicProbabilityCollection != null)
			{
				StaticPool<DynamicProbabilityCollection<NpcSkillsController.AbilityEntry>>.ReturnToPool(this.m_dynamicProbabilityCollection);
				this.m_dynamicProbabilityCollection = null;
			}
		}

		// Token: 0x06003BE7 RID: 15335 RVA: 0x000688BF File Offset: 0x00066ABF
		protected override bool BypassCooldownUpdate()
		{
			return base.BypassCooldownUpdate() || !base.GameEntity.ServerNpcController.EnableBehavior;
		}

		// Token: 0x06003BE8 RID: 15336 RVA: 0x000688DE File Offset: 0x00066ADE
		private void Update()
		{
			base.UpdateCooldowns();
			this.AttemptPursueAbilities();
		}

		// Token: 0x06003BE9 RID: 15337 RVA: 0x000688EC File Offset: 0x00066AEC
		public WeaponItem GetCurrentWeaponItem()
		{
			ExecutionCache executionCache = this.m_executionCache;
			if (executionCache == null)
			{
				return null;
			}
			return executionCache.MainHand.WeaponItem;
		}

		// Token: 0x06003BEA RID: 15338 RVA: 0x00068904 File Offset: 0x00066B04
		public float GetSelfDistanceBuffer()
		{
			if (base.GameEntity && base.GameEntity.Targetable != null)
			{
				return base.GameEntity.Targetable.DistanceBuffer;
			}
			return 0f;
		}

		// Token: 0x06003BEB RID: 15339 RVA: 0x0017D390 File Offset: 0x0017B590
		public bool TryGetWeaponStoppingDistance(out float distance)
		{
			distance = 0f;
			ExecutionCache executionCache = this.m_executionCache;
			if (((executionCache != null) ? executionCache.MainHand : null) == null || !this.m_executionCache.MainHand.WeaponItem)
			{
				return false;
			}
			distance = this.m_executionCache.MainHand.WeaponItem.GetWeaponDistance().Max;
			if (base.GameEntity && base.GameEntity.Targetable != null)
			{
				distance += base.GameEntity.Targetable.DistanceBuffer;
			}
			return true;
		}

		// Token: 0x06003BEC RID: 15340 RVA: 0x00068936 File Offset: 0x00066B36
		public void UpdateMovementMode(NpcMovementMode value)
		{
			if (this.m_movementMode == value)
			{
				return;
			}
			NpcMovementMode movementMode = this.m_movementMode;
			this.m_movementMode = value;
			if (!movementMode.IsCombatVariant() && this.m_movementMode == NpcMovementMode.Pursue)
			{
				this.m_timeOfNextPursueCheck = Time.time + 4f;
			}
		}

		// Token: 0x06003BED RID: 15341 RVA: 0x0017D420 File Offset: 0x0017B620
		private void AttemptPursueAbilities()
		{
			if (this.m_movementMode.CanAttemptPursueAbility() && Time.time >= this.m_timeOfNextPursueCheck && this.m_pursueAbilities != null && this.m_pursueAbilities.Count > 0 && base.GameEntity.TargetController && base.GameEntity.TargetController.OffensiveTarget)
			{
				float num = this.TryExecuteAbility(base.GameEntity.TargetController.OffensiveTarget, this.m_pursueAbilities, false, true) ? 3f : 1f;
				this.m_timeOfNextPursueCheck = Time.time + num;
			}
		}

		// Token: 0x06003BEE RID: 15342 RVA: 0x0017D4C4 File Offset: 0x0017B6C4
		public override void CacheAbilities(bool useHealthFractionAutoAttack, bool bypassLevelDeltaCombatAdjustments)
		{
			base.CacheAbilities(useHealthFractionAutoAttack, bypassLevelDeltaCombatAdjustments);
			this.m_npcLevel = (bypassLevelDeltaCombatAdjustments ? 50f : ((float)base.GameEntity.CharacterData.AdventuringLevel));
			if (base.GameEntity.CollectionController.Abilities.Count > 0)
			{
				this.m_combatAbilities = StaticListPool<ArchetypeInstance>.GetFromPool();
				for (int i = 0; i < base.GameEntity.CollectionController.Abilities.Count; i++)
				{
					ArchetypeInstance index = base.GameEntity.CollectionController.Abilities.GetIndex(i);
					AbilityArchetype abilityArchetype;
					if (index != null && index.Archetype.TryGetAsType(out abilityArchetype) && this.m_npcLevel >= (float)abilityArchetype.MinimumLevel)
					{
						AbilityArchetype.NpcUseCases npcUseCase = abilityArchetype.NpcUseCase;
						if (npcUseCase != AbilityArchetype.NpcUseCases.CombatOnly)
						{
							if (npcUseCase != AbilityArchetype.NpcUseCases.PursueOnly)
							{
								if (npcUseCase == AbilityArchetype.NpcUseCases.Both)
								{
									if (this.m_pursueAbilities == null)
									{
										this.m_pursueAbilities = StaticListPool<ArchetypeInstance>.GetFromPool();
									}
									this.m_combatAbilities.Add(index);
									this.m_pursueAbilities.Add(index);
								}
							}
							else
							{
								if (this.m_pursueAbilities == null)
								{
									this.m_pursueAbilities = StaticListPool<ArchetypeInstance>.GetFromPool();
								}
								this.m_pursueAbilities.Add(index);
							}
						}
						else
						{
							this.m_combatAbilities.Add(index);
						}
					}
				}
			}
			if (this.m_autoAttackInstance == null)
			{
				UniqueId instanceId;
				if (useHealthFractionAutoAttack && GlobalSettings.Values.Combat.AutoAttackHealthFraction)
				{
					GlobalSettings.Values.Combat.AutoAttackHealthFraction.DynamicallyLoad(base.GameEntity.CollectionController.Abilities);
					instanceId = new UniqueId(GlobalSettings.Values.Combat.AutoAttackHealthFraction.GetInstanceId());
				}
				else
				{
					instanceId = new UniqueId("AUTOATTACK");
				}
				base.GameEntity.CollectionController.Abilities.TryGetInstanceForInstanceId(instanceId, out this.m_autoAttackInstance);
			}
		}

		// Token: 0x06003BEF RID: 15343 RVA: 0x0017D680 File Offset: 0x0017B880
		private float GetCachedDistance(GameEntity targetEntity)
		{
			if (!targetEntity)
			{
				return float.MaxValue;
			}
			if (targetEntity == base.GameEntity)
			{
				return 0f;
			}
			if (this.m_cachedDistance == null)
			{
				this.m_cachedDistance = new float?(base.GameEntity.PrimaryTargetPoint.DistanceTo(targetEntity.PrimaryTargetPoint));
			}
			return this.m_cachedDistance.Value;
		}

		// Token: 0x06003BF0 RID: 15344 RVA: 0x0017D6E8 File Offset: 0x0017B8E8
		private float GetCachedAngle(GameEntity targetEntity)
		{
			if (!targetEntity)
			{
				return float.MaxValue;
			}
			if (targetEntity == base.GameEntity)
			{
				return 0f;
			}
			if (this.m_cachedAngle == null)
			{
				this.m_cachedAngle = new float?(Mathf.Abs(base.GameEntity.PrimaryTargetPoint.AngleTo(targetEntity.PrimaryTargetPoint, true)));
			}
			return this.m_cachedAngle.Value;
		}

		// Token: 0x06003BF1 RID: 15345 RVA: 0x0017D758 File Offset: 0x0017B958
		private void CacheBaseRole()
		{
			if (this.m_cachedBaseRole)
			{
				return;
			}
			for (int i = 0; i < base.GameEntity.CollectionController.Masteries.Count; i++)
			{
				ArchetypeInstance index = base.GameEntity.CollectionController.Masteries.GetIndex(i);
				BaseRole npcBaseRole;
				if (index != null && index.Archetype && index.Archetype.TryGetAsType(out npcBaseRole))
				{
					this.m_executionCache.NpcBaseRole = npcBaseRole;
					if (this.m_executionCache.NpcBaseRole)
					{
						this.m_executionCache.MasteryInstance = index;
						break;
					}
				}
			}
			this.m_cachedBaseRole = true;
		}

		// Token: 0x06003BF2 RID: 15346 RVA: 0x00068970 File Offset: 0x00066B70
		public bool TryExecuteCombatAbility(GameEntity targetEntity)
		{
			return this.TryExecuteAbility(targetEntity, this.m_combatAbilities, true, false);
		}

		// Token: 0x06003BF3 RID: 15347 RVA: 0x0017D7F8 File Offset: 0x0017B9F8
		private bool EarlyOutFromHealing(ICombatEffectSource effectSource, float level)
		{
			if (base.GameEntity.Vitals.HealthPercent <= 0.9f)
			{
				return false;
			}
			CombatEffect combatEffect = effectSource.GetCombatEffect(level, AlchemyPowerLevel.None);
			if (combatEffect.Effects == null)
			{
				return false;
			}
			if (combatEffect.Effects.HasInstantVitals)
			{
				for (int i = 0; i < combatEffect.Effects.Instant.Length; i++)
				{
					if (combatEffect.Effects.Instant[i].ResourceType == EffectResourceType.Health)
					{
						return true;
					}
				}
			}
			if (combatEffect.Effects.HasOverTimeVitals)
			{
				for (int j = 0; j < combatEffect.Effects.OverTime.Length; j++)
				{
					if (combatEffect.Effects.OverTime[j].ResourceType == EffectResourceType.Health)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003BF4 RID: 15348 RVA: 0x0017D8A8 File Offset: 0x0017BAA8
		private bool TryExecuteAbility(GameEntity targetEntity, List<ArchetypeInstance> abilities, bool autoAttackFallback, bool considerLineOfSight)
		{
			if (!targetEntity || !targetEntity.EffectController || !targetEntity.VitalsReplicator || targetEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive)
			{
				return false;
			}
			if (base.GameEntity.ServerNpcController && base.GameEntity.ServerNpcController.PreventSkillExecution())
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (base.NextNpcAttackTimeDelay != null && utcNow < base.NextNpcAttackTimeDelay.Value)
			{
				return false;
			}
			base.NextNpcAttackTimeDelay = null;
			if (utcNow < this.m_globalCooldown)
			{
				return false;
			}
			bool flag = false;
			ArchetypeInstance archetypeInstance = null;
			this.m_cachedDistance = null;
			this.m_cachedAngle = null;
			if (autoAttackFallback && (utcNow - this.m_timeOfLastAutoAttack).TotalSeconds > 8.0)
			{
				this.m_timeOfNextAbilityTry = utcNow.AddSeconds((double)(NpcSkillsController.m_abilityTryTimeRange.RandomWithinRange() * this.GetHasteMultiplier()));
			}
			if (abilities != null && abilities.Count > 0 && utcNow > this.m_timeOfNextAbilityTry && !base.GameEntity.ServerNpcController.IsDazed() && UnityEngine.Random.Range(0f, 1f) <= 0.9f)
			{
				bool? flag2 = null;
				float time = base.GameEntity.Vitals ? base.GameEntity.Vitals.GetHealthPercent() : 1f;
				this.m_dynamicProbabilityCollection.Clear();
				for (int i = 0; i < abilities.Count; i++)
				{
					ArchetypeInstance archetypeInstance2 = abilities[i];
					AppliableEffectAbility appliableEffectAbility;
					if (archetypeInstance2 != null && archetypeInstance2.AbilityData != null && archetypeInstance2.AbilityData.HealthFractionProbabilityCurve != null && archetypeInstance2.Archetype.TryGetAsType(out appliableEffectAbility) && this.m_npcLevel >= (float)appliableEffectAbility.MinimumLevel && archetypeInstance2.AbilityData.Cooldown_Base.Elapsed == null)
					{
						float num = archetypeInstance2.AbilityData.HealthFractionProbabilityCurve.Evaluate(time);
						if (num > 0f)
						{
							float npcLevel = this.m_npcLevel;
							ICombatEffectSource combatEffectSource = appliableEffectAbility;
							TargetingParams targetingParams = combatEffectSource.GetTargetingParams(npcLevel, AlchemyPowerLevel.None);
							GameEntity gameEntity = targetEntity;
							if (targetingParams.TargetType.IsFriendly())
							{
								gameEntity = base.GameEntity;
								if (this.EarlyOutFromHealing(combatEffectSource, npcLevel))
								{
									goto IL_301;
								}
							}
							if (considerLineOfSight && targetingParams.TargetType.RequiresLineOfSight())
							{
								if (flag2 == null)
								{
									flag2 = new bool?(LineOfSight.NpcHasLineOfSight(base.GameEntity, gameEntity));
								}
								if (!flag2.Value)
								{
									goto IL_301;
								}
							}
							if (targetingParams.MeetsDistanceRequirements(base.GameEntity, gameEntity, this.m_executionCache, this.GetCachedDistance(gameEntity)) && targetingParams.MeetsAngleRequirements(base.GameEntity, gameEntity, this.m_executionCache, this.GetCachedAngle(gameEntity)))
							{
								this.m_dynamicProbabilityCollection.Add(new NpcSkillsController.AbilityEntry(archetypeInstance2, num, appliableEffectAbility, gameEntity));
							}
						}
					}
					IL_301:;
				}
				NpcSkillsController.AbilityEntry abilityEntry;
				if (this.m_dynamicProbabilityCollection.TryGetEntry(out abilityEntry, null))
				{
					if (abilityEntry.Appliable && abilityEntry.Appliable.NpcEmotes != null)
					{
						abilityEntry.Appliable.NpcEmotes.EmoteToNearbyPlayers(base.GameEntity, null);
					}
					this.m_timeOfNextAbilityTry = utcNow.AddSeconds((double)(NpcSkillsController.m_abilityTryTimeRange.RandomWithinRange() * this.GetHasteMultiplier()));
					archetypeInstance = abilityEntry.Instance;
					targetEntity = abilityEntry.Target;
				}
			}
			if (autoAttackFallback && archetypeInstance == null && this.m_autoAttackInstance != null && this.m_autoAttackInstance.AbilityData != null && this.m_autoAttackInstance.AbilityData.Cooldown_Base.Elapsed == null && SkillsController.m_autoAttack.MeetsDistanceRequirements(base.GameEntity, targetEntity, this.m_executionCache, this.GetCachedDistance(targetEntity)) && SkillsController.m_autoAttack.MeetsAngleRequirements(base.GameEntity, targetEntity, this.m_executionCache, this.GetCachedAngle(targetEntity)))
			{
				archetypeInstance = this.m_autoAttackInstance;
				flag = true;
			}
			if (archetypeInstance != null)
			{
				this.CacheBaseRole();
				this.m_executionCache.Init(base.GameEntity, archetypeInstance);
				this.m_executionCache.SetTargetGameEntity(targetEntity);
				this.m_executionCache.ApplyEffects();
				archetypeInstance.AbilityData.AbilityExecuted(AlchemyPowerLevel.None);
				this.m_globalCooldown = utcNow.AddSeconds((double)(3f * this.GetHasteMultiplier()));
				if (base.GameEntity.NetworkEntity.NpcRpcHandler)
				{
					if (flag)
					{
						base.GameEntity.NetworkEntity.NpcRpcHandler.Server_Execute_AutoAttack(targetEntity.NetworkEntity, this.m_executionCache.AnimationFlags);
						this.m_timeOfLastAutoAttack = utcNow;
					}
					else
					{
						base.GameEntity.NetworkEntity.NpcRpcHandler.Server_ExecuteAbility(archetypeInstance.ArchetypeId, targetEntity.NetworkEntity, this.m_executionCache.AbilityLevelAsByte);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003BF5 RID: 15349 RVA: 0x0017DDA0 File Offset: 0x0017BFA0
		private float GetHasteMultiplier()
		{
			if (base.GameEntity.Vitals)
			{
				int haste = base.GameEntity.Vitals.GetHaste();
				if (haste > 0)
				{
					return this.GetHasteFactor(haste);
				}
				if (haste < 0)
				{
					return 2f - this.GetHasteFactor(haste);
				}
			}
			return 1f;
		}

		// Token: 0x06003BF6 RID: 15350 RVA: 0x00068981 File Offset: 0x00066B81
		private float GetHasteFactor(int haste)
		{
			return 1f / (1f + (float)Mathf.Abs(haste) * 0.01f);
		}

		// Token: 0x04003A92 RID: 14994
		private const float kNpcGlobalCooldown = 3f;

		// Token: 0x04003A93 RID: 14995
		private const float kAbilityUseOpenerTime = 8f;

		// Token: 0x04003A94 RID: 14996
		private const float kAbilityUseProbability = 0.9f;

		// Token: 0x04003A95 RID: 14997
		private static MinMaxFloatRange m_abilityTryTimeRange = new MinMaxFloatRange(3f, 12f);

		// Token: 0x04003A96 RID: 14998
		private DateTime m_timeOfLastAutoAttack = DateTime.MinValue;

		// Token: 0x04003A97 RID: 14999
		private DateTime m_timeOfNextAbilityTry = DateTime.MinValue;

		// Token: 0x04003A98 RID: 15000
		private DateTime m_globalCooldown = DateTime.MinValue;

		// Token: 0x04003A99 RID: 15001
		private float m_timeOfNextPursueCheck;

		// Token: 0x04003A9A RID: 15002
		private NpcMovementMode m_movementMode;

		// Token: 0x04003A9B RID: 15003
		private ArchetypeInstance m_autoAttackInstance;

		// Token: 0x04003A9C RID: 15004
		private ExecutionCache m_executionCache;

		// Token: 0x04003A9D RID: 15005
		private DynamicProbabilityCollection<NpcSkillsController.AbilityEntry> m_dynamicProbabilityCollection;

		// Token: 0x04003A9E RID: 15006
		private float m_npcLevel = 50f;

		// Token: 0x04003A9F RID: 15007
		private List<ArchetypeInstance> m_combatAbilities;

		// Token: 0x04003AA0 RID: 15008
		private List<ArchetypeInstance> m_pursueAbilities;

		// Token: 0x04003AA1 RID: 15009
		private float? m_cachedDistance;

		// Token: 0x04003AA2 RID: 15010
		private float? m_cachedAngle;

		// Token: 0x04003AA3 RID: 15011
		private bool m_cachedBaseRole;

		// Token: 0x04003AA4 RID: 15012
		private const float kHealingThreshold = 0.9f;

		// Token: 0x02000815 RID: 2069
		private struct AbilityEntry : IProbabilityEntry
		{
			// Token: 0x17000DCC RID: 3532
			// (get) Token: 0x06003BF9 RID: 15353 RVA: 0x000689E6 File Offset: 0x00066BE6
			// (set) Token: 0x06003BFA RID: 15354 RVA: 0x000689EE File Offset: 0x00066BEE
			public float Probability { readonly get; private set; }

			// Token: 0x17000DCD RID: 3533
			// (get) Token: 0x06003BFB RID: 15355 RVA: 0x000689F7 File Offset: 0x00066BF7
			// (set) Token: 0x06003BFC RID: 15356 RVA: 0x000689FF File Offset: 0x00066BFF
			public float Threshold { readonly get; set; }

			// Token: 0x17000DCE RID: 3534
			// (get) Token: 0x06003BFD RID: 15357 RVA: 0x00068A08 File Offset: 0x00066C08
			// (set) Token: 0x06003BFE RID: 15358 RVA: 0x00068A10 File Offset: 0x00066C10
			public float NormalizedProbability { readonly get; private set; }

			// Token: 0x06003BFF RID: 15359 RVA: 0x00068A19 File Offset: 0x00066C19
			public AbilityEntry(ArchetypeInstance instance, float relativeProbability, AppliableEffectAbility appliable, GameEntity targetEntity)
			{
				this.Instance = instance;
				this.Probability = relativeProbability;
				this.Threshold = 1f;
				this.NormalizedProbability = 0f;
				this.Appliable = appliable;
				this.Target = targetEntity;
			}

			// Token: 0x06003C00 RID: 15360 RVA: 0x00068A4E File Offset: 0x00066C4E
			public void SetTotalProbability(float total)
			{
				this.NormalizedProbability = this.Probability / total;
			}

			// Token: 0x04003AA5 RID: 15013
			public ArchetypeInstance Instance;

			// Token: 0x04003AA9 RID: 15017
			public AppliableEffectAbility Appliable;

			// Token: 0x04003AAA RID: 15018
			public GameEntity Target;
		}
	}
}
