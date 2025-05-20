using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Pooling;
using SoL.Game.Quests;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Settings
{
	// Token: 0x0200071A RID: 1818
	[Serializable]
	public class AshenSettings
	{
		// Token: 0x0600368B RID: 13963 RVA: 0x000655BB File Offset: 0x000637BB
		public int GetAshenDistribution(int levelDelta)
		{
			return Mathf.FloorToInt(this.m_distributionCurve.Evaluate((float)levelDelta));
		}

		// Token: 0x0600368C RID: 13964 RVA: 0x00169ED0 File Offset: 0x001680D0
		public int GetNonAshenDistribution(int levelDelta, int currentEssenceCount, int maxCapacity)
		{
			if (this.m_deliverNonAshenEssence && (float)currentEssenceCount < (float)maxCapacity * this.m_nonAshenEssenceCapFraction)
			{
				float num = this.m_nonAshenChanceCurve.Evaluate((float)levelDelta);
				if (UnityEngine.Random.Range(0f, 1f) < num)
				{
					return this.m_nonAshenEssence;
				}
			}
			return 0;
		}

		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x0600368D RID: 13965 RVA: 0x000655CF File Offset: 0x000637CF
		private bool m_showCostReductionMultiplier
		{
			get
			{
				return this.m_costReductionProfile != null;
			}
		}

		// Token: 0x0600368E RID: 13966 RVA: 0x00169F1C File Offset: 0x0016811C
		public int GetMonolithCost(MonolithFlags flags)
		{
			int num = 0;
			if (this.m_bypassMonolithCosts)
			{
				return num;
			}
			if (this.m_monolithCostData != null)
			{
				for (int i = 0; i < this.m_monolithCostData.Length; i++)
				{
					if (this.m_monolithCostData[i].Flags == flags)
					{
						num = this.m_monolithCostData[i].Cost;
						break;
					}
				}
			}
			if (num > 0 && this.m_costReductionProfile && this.m_costReductionProfile.IsAvailable())
			{
				num = Mathf.CeilToInt((float)num * this.m_costReductionMultiplier);
			}
			return num;
		}

		// Token: 0x0600368F RID: 13967 RVA: 0x000655DD File Offset: 0x000637DD
		public void GetTravelEssenceConversionValues(int purchaseAmount, out int essenceCost, out ulong currencyCost)
		{
			essenceCost = Mathf.CeilToInt((float)purchaseAmount / (float)this.m_travelConversionRatio);
			currencyCost = (ulong)((long)purchaseAmount * (long)((ulong)this.m_travelEssencePurchaseCost));
		}

		// Token: 0x06003690 RID: 13968 RVA: 0x000655FC File Offset: 0x000637FC
		public int GetMaxTravelCanPurchase(int currentEssence)
		{
			return Mathf.FloorToInt((float)currentEssence * (float)this.m_travelConversionRatio);
		}

		// Token: 0x06003691 RID: 13969 RVA: 0x00169FA0 File Offset: 0x001681A0
		private string ConversionCost()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				string text = new CurrencyConverter((ulong)this.m_travelEssencePurchaseCost).ToString();
				utf16ValueStringBuilder.AppendFormat<string, int>("Cost: {0} per TE at the conversion rate of 1 EE --> {1} TE\n", text.ToString(), this.m_travelConversionRatio);
				if (GlobalSettings.Values && GlobalSettings.Values.Progression != null && GlobalSettings.Values.Progression.StartingEmberStone)
				{
					utf16ValueStringBuilder.AppendLine(" ::STONES::");
					utf16ValueStringBuilder.AppendLine("\tCost\t\tEE Cost\tTE Total\tStone Name");
					EmberStone emberStone = GlobalSettings.Values.Progression.StartingEmberStone;
					while (emberStone != null)
					{
						int arg;
						ulong currency;
						this.GetTravelEssenceConversionValues(emberStone.MaxCapacity, out arg, out currency);
						CurrencyConverter currencyConverter = new CurrencyConverter(currency);
						utf16ValueStringBuilder.AppendFormat<string, int, int, string>("\t{0}\t\t{1}\t{2}\t{3}\n", currencyConverter.ToString(), arg, emberStone.MaxCapacity, emberStone.DisplayName);
						emberStone = emberStone.NextEmberStone;
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06003692 RID: 13970 RVA: 0x0006560D File Offset: 0x0006380D
		public bool ReduceSpawnHealth(out float spawnHealthMultiplier)
		{
			spawnHealthMultiplier = Mathf.Clamp(this.m_spawnHealthMultiplier, 0.1f, 1f);
			return spawnHealthMultiplier < 1f;
		}

		// Token: 0x06003693 RID: 13971 RVA: 0x0006562F File Offset: 0x0006382F
		public float GetRandomRippleDelay()
		{
			return UnityEngine.Random.Range(this.m_ruptureRippleDelay.Min, this.m_ruptureRippleDelay.Max);
		}

		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x06003694 RID: 13972 RVA: 0x0006564C File Offset: 0x0006384C
		public bool AddDistortionFilter
		{
			get
			{
				return this.m_addDistortionFilter;
			}
		}

		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x06003695 RID: 13973 RVA: 0x00065654 File Offset: 0x00063854
		public float DistortionLevel
		{
			get
			{
				return this.m_distortionLevel;
			}
		}

		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x06003696 RID: 13974 RVA: 0x0006565C File Offset: 0x0006385C
		public bool AddChorusFilter
		{
			get
			{
				return this.m_addChorusFilter;
			}
		}

		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x06003697 RID: 13975 RVA: 0x00065664 File Offset: 0x00063864
		public float ChorusRate
		{
			get
			{
				return this.m_chorusRate;
			}
		}

		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x06003698 RID: 13976 RVA: 0x0006566C File Offset: 0x0006386C
		public float ChorusDepth
		{
			get
			{
				return this.m_chorusDepth;
			}
		}

		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x06003699 RID: 13977 RVA: 0x00065674 File Offset: 0x00063874
		public bool AllowEmberRingTeleportsFromMonolith
		{
			get
			{
				return this.m_allowEmberRingTeleportsFromMonolith;
			}
		}

		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x0600369A RID: 13978 RVA: 0x0016A0D4 File Offset: 0x001682D4
		public int EmberRingFromMonolithEssenceCost
		{
			get
			{
				int num = this.m_emberRingFromMonolithEssenceCost;
				if (num > 0 && this.m_costReductionProfile && this.m_costReductionProfile.IsAvailable())
				{
					num = Mathf.CeilToInt((float)num * this.m_costReductionMultiplier);
				}
				return num;
			}
		}

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x0600369B RID: 13979 RVA: 0x0006567C File Offset: 0x0006387C
		public bool AllowGroupTeleportsFromMonolith
		{
			get
			{
				return this.m_allowGroupTeleportsFromMonolith;
			}
		}

		// Token: 0x0600369C RID: 13980 RVA: 0x00065684 File Offset: 0x00063884
		public bool AllowGroupTeleportsFromMonolithToZone(ZoneId zoneId)
		{
			return this.AllowGroupTeleportsFromMonolith && this.m_allowGroupTeleportTo != null && this.m_allowGroupTeleportTo.Contains(zoneId);
		}

		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x0600369D RID: 13981 RVA: 0x0016A118 File Offset: 0x00168318
		public int GroupFromMonolithEssenceCost
		{
			get
			{
				int num = this.m_groupFromMonolithEssenceCost;
				if (num > 0 && this.m_costReductionProfile && this.m_costReductionProfile.IsAvailable())
				{
					num = Mathf.CeilToInt((float)num * this.m_costReductionMultiplier);
				}
				return num;
			}
		}

		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x0600369E RID: 13982 RVA: 0x000656A4 File Offset: 0x000638A4
		public int GroupTeleportMemberThreshold
		{
			get
			{
				return this.m_groupTeleportMemberThreshold;
			}
		}

		// Token: 0x0600369F RID: 13983 RVA: 0x0016A15C File Offset: 0x0016835C
		public LeyLinkProfile GetLeyLinkProfileByIndex(byte index)
		{
			if (index > 0 && this.m_allowGroupTeleportsFromMonolith && this.m_availableLeyLinkProfiles != null)
			{
				index -= 1;
				for (int i = 0; i < this.m_availableLeyLinkProfiles.Length; i++)
				{
					if (i == (int)index)
					{
						return this.m_availableLeyLinkProfiles[i];
					}
				}
			}
			return null;
		}

		// Token: 0x060036A0 RID: 13984 RVA: 0x0016A1A4 File Offset: 0x001683A4
		public bool TryGetLeyLinkIndex(LeyLinkProfile profile, out byte index)
		{
			index = 0;
			if (this.AllowGroupTeleportsFromMonolith && this.m_availableLeyLinkProfiles != null && profile)
			{
				for (int i = 0; i < this.m_availableLeyLinkProfiles.Length; i++)
				{
					if (this.m_availableLeyLinkProfiles[i] != null && this.m_availableLeyLinkProfiles[i].Id == profile.Id)
					{
						index = (byte)(i + 1);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x060036A1 RID: 13985 RVA: 0x0016A214 File Offset: 0x00168414
		public bool AllowAlchemy
		{
			get
			{
				if (this.m_disableAlchemy)
				{
					return false;
				}
				if (this.m_alchemyBranchFlags == DeploymentBranchFlags.None)
				{
					return true;
				}
				DeploymentBranchFlags branchFlags = DeploymentBranchFlagsExtensions.GetBranchFlags();
				return branchFlags == DeploymentBranchFlags.None || this.m_alchemyBranchFlags.HasBitFlag(branchFlags);
			}
		}

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x060036A2 RID: 13986 RVA: 0x000656AC File Offset: 0x000638AC
		public UniqueId AlchemyQuestId
		{
			get
			{
				if (!this.m_alchemyQuestRequirement)
				{
					return UniqueId.Empty;
				}
				return this.m_alchemyQuestRequirement.Id;
			}
		}

		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x060036A3 RID: 13987 RVA: 0x000656CC File Offset: 0x000638CC
		public int AlchemyLevelThreshold
		{
			get
			{
				return this.m_alchemyLevelThreshold;
			}
		}

		// Token: 0x17000C18 RID: 3096
		// (get) Token: 0x060036A4 RID: 13988 RVA: 0x000656D4 File Offset: 0x000638D4
		public float GlowAnimationSpeed
		{
			get
			{
				return this.m_glowAnimationSpeed;
			}
		}

		// Token: 0x060036A5 RID: 13989 RVA: 0x0016A24C File Offset: 0x0016844C
		public int GetAlchemyAdditionalExecutionTime(AlchemyPowerLevel alchemyPowerLevel)
		{
			int result = 0;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyII != null)
					{
						result = this.m_alchemyII.AdditionalExecutionTime;
					}
				}
			}
			else if (this.m_alchemyI != null)
			{
				result = this.m_alchemyI.AdditionalExecutionTime;
			}
			return result;
		}

		// Token: 0x060036A6 RID: 13990 RVA: 0x0016A290 File Offset: 0x00168490
		public int GetAlchemyEssenceCost(AlchemyPowerLevel alchemyPowerLevel)
		{
			int result = 0;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyII != null)
					{
						result = this.m_alchemyII.Cost;
					}
				}
			}
			else if (this.m_alchemyI != null)
			{
				result = this.m_alchemyI.Cost;
			}
			return result;
		}

		// Token: 0x060036A7 RID: 13991 RVA: 0x0016A2D4 File Offset: 0x001684D4
		public PooledVFX GetAlchemyPooledVFX(AlchemyPowerLevel alchemyPowerLevel)
		{
			PooledVFX result = null;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyII != null)
					{
						result = this.m_alchemyII.VFX;
					}
				}
			}
			else if (this.m_alchemyI != null)
			{
				result = this.m_alchemyI.VFX;
			}
			return result;
		}

		// Token: 0x060036A8 RID: 13992 RVA: 0x0016A318 File Offset: 0x00168518
		public float GetGlowIntensity(AlchemyPowerLevel alchemyPowerLevel)
		{
			float result = this.m_defaultGlowIntensity;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyII != null)
					{
						result = this.m_alchemyII.Intensity;
					}
				}
			}
			else if (this.m_alchemyI != null)
			{
				result = this.m_alchemyI.Intensity;
			}
			return result;
		}

		// Token: 0x060036A9 RID: 13993 RVA: 0x000656DC File Offset: 0x000638DC
		public Color GetUIHighlightColor(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyII != null)
					{
						return this.m_alchemyII.UIHighlightColor;
					}
				}
			}
			else if (this.m_alchemyI != null)
			{
				return this.m_alchemyI.UIHighlightColor;
			}
			return Color.white;
		}

		// Token: 0x060036AA RID: 13994 RVA: 0x0016A364 File Offset: 0x00168564
		public bool TryGetAlchemyCooldownTime(AlchemyPowerLevel alchemyPowerLevel, out float alchemyCooldownTime)
		{
			alchemyCooldownTime = 0f;
			float? alchemyCooldownTime2 = this.GetAlchemyCooldownTime(alchemyPowerLevel);
			if (alchemyCooldownTime2 != null)
			{
				alchemyCooldownTime = alchemyCooldownTime2.Value;
				return true;
			}
			return false;
		}

		// Token: 0x060036AB RID: 13995 RVA: 0x0016A398 File Offset: 0x00168598
		public float? GetAlchemyCooldownTime(AlchemyPowerLevel alchemyPowerLevel)
		{
			float? result = null;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyII != null)
					{
						result = new float?((float)this.m_alchemyII.CooldownTime);
					}
				}
			}
			else if (this.m_alchemyI != null)
			{
				result = new float?((float)this.m_alchemyI.CooldownTime);
			}
			if (result != null && ZoneSettings.Instance && ZoneSettings.Instance.Profile && ZoneSettings.Instance.Profile.ExtendAlchemyCooldowns)
			{
				result = new float?(result.Value * this.m_extendedAlchemyCooldownMultiplier);
			}
			return result;
		}

		// Token: 0x060036AC RID: 13996 RVA: 0x0016A43C File Offset: 0x0016863C
		public int GetAlchemyUsageThreshold(AlchemyPowerLevel alchemyPowerLevel)
		{
			switch (alchemyPowerLevel)
			{
			case AlchemyPowerLevel.None:
				return 0;
			case AlchemyPowerLevel.I:
				if (this.m_alchemyI != null)
				{
					return this.m_alchemyI.UsageThreshold;
				}
				break;
			case AlchemyPowerLevel.II:
				if (this.m_alchemyII != null)
				{
					return this.m_alchemyII.UsageThreshold;
				}
				break;
			}
			return int.MaxValue;
		}

		// Token: 0x060036AD RID: 13997 RVA: 0x0016A48C File Offset: 0x0016868C
		public bool AlchemyAvailableForEntity(GameEntity entity)
		{
			if (this.m_disableAlchemy)
			{
				return false;
			}
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.CollectionController == null || !entity.CollectionController.CurrentEmberStone)
			{
				return false;
			}
			if (this.m_alchemyQuestRequirement)
			{
				if (!entity.AlchemyQuestRequirementsMet)
				{
					entity.AlchemyQuestRequirementsMet = this.m_alchemyQuestRequirement.IsComplete(entity);
				}
			}
			else
			{
				entity.AlchemyQuestRequirementsMet = true;
			}
			return entity.AlchemyQuestRequirementsMet && entity.CharacterData && entity.CharacterData.AdventuringLevel >= this.m_alchemyLevelThreshold;
		}

		// Token: 0x060036AE RID: 13998 RVA: 0x0016A530 File Offset: 0x00168730
		public bool AlchemyPowerLevelAvailable(GameEntity entity, ArchetypeInstance abilityInstance, AlchemyPowerLevel requestedPowerLevel, bool considerCooldown, out string msg)
		{
			msg = null;
			if (this.m_disableAlchemy)
			{
				return false;
			}
			if (requestedPowerLevel == AlchemyPowerLevel.None)
			{
				return true;
			}
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			if (abilityInstance == null)
			{
				throw new ArgumentNullException("abilityInstance");
			}
			if (!abilityInstance.Archetype)
			{
				throw new ArgumentNullException("Archetype");
			}
			if (abilityInstance.AbilityData == null)
			{
				throw new ArgumentNullException("AbilityData");
			}
			IExecutable executable;
			if (!abilityInstance.Archetype.TryGetAsType(out executable) || !executable.AllowAlchemy)
			{
				return false;
			}
			if (!entity.CharacterData || entity.CharacterData.AdventuringLevel < this.m_alchemyLevelThreshold)
			{
				msg = "Not high enough level!";
				return false;
			}
			CooldownData cooldownData = null;
			if (requestedPowerLevel != AlchemyPowerLevel.I)
			{
				if (requestedPowerLevel == AlchemyPowerLevel.II)
				{
					cooldownData = abilityInstance.AbilityData.Cooldown_AlchemyII;
				}
			}
			else
			{
				cooldownData = abilityInstance.AbilityData.Cooldown_AlchemyI;
			}
			if (considerCooldown && (cooldownData == null || cooldownData.Active))
			{
				msg = "Alchemy cooldown not met!";
				return false;
			}
			if (this.m_alchemyQuestRequirement)
			{
				if (!entity.AlchemyQuestRequirementsMet)
				{
					entity.AlchemyQuestRequirementsMet = this.m_alchemyQuestRequirement.IsComplete(entity);
				}
			}
			else
			{
				entity.AlchemyQuestRequirementsMet = true;
			}
			if (!entity.AlchemyQuestRequirementsMet)
			{
				return false;
			}
			int usageThreshold = ((requestedPowerLevel == AlchemyPowerLevel.II) ? this.m_alchemyII : this.m_alchemyI).UsageThreshold;
			if (usageThreshold <= 0)
			{
				return true;
			}
			AlchemyPowerLevel previousPowerLevel = requestedPowerLevel.GetPreviousPowerLevel();
			bool flag = abilityInstance.AbilityData.GetUsageCount(previousPowerLevel) >= usageThreshold;
			if (!flag)
			{
				msg = "You cannot use that yet!";
			}
			return flag;
		}

		// Token: 0x060036AF RID: 13999 RVA: 0x0016A69C File Offset: 0x0016889C
		public bool TryGetAudioClip(AlchemyPowerLevel alchemyPowerLevel, bool activate, out AudioClip audioClip, out float volume, out float pitch)
		{
			audioClip = null;
			volume = 1f;
			pitch = 1f;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					audioClip = this.m_alchemyII.ActivateClip;
					volume = (activate ? this.m_alchemyII.Volume.x : this.m_alchemyII.Volume.y);
					pitch = (activate ? this.m_alchemyII.Pitch.x : this.m_alchemyII.Pitch.y);
				}
			}
			else
			{
				audioClip = this.m_alchemyI.ActivateClip;
				volume = (activate ? this.m_alchemyI.Volume.x : this.m_alchemyI.Volume.y);
				pitch = (activate ? this.m_alchemyI.Pitch.x : this.m_alchemyI.Pitch.y);
			}
			return audioClip != null;
		}

		// Token: 0x060036B0 RID: 14000 RVA: 0x00065715 File Offset: 0x00063915
		public int GetEssenceForFlatworm()
		{
			return this.m_essencePerFlatworm.RandomWithinRange();
		}

		// Token: 0x0400348B RID: 13451
		private const float kMinSpawnHealthMultiplier = 0.1f;

		// Token: 0x0400348C RID: 13452
		private const float kMaxSpawnHealthMultiplier = 1f;

		// Token: 0x0400348D RID: 13453
		private const string kAshenGroup = "Ashen";

		// Token: 0x0400348E RID: 13454
		private const string kNonAshenGroup = "Non Ashen";

		// Token: 0x0400348F RID: 13455
		[SerializeField]
		private AnimationCurve m_distributionCurve;

		// Token: 0x04003490 RID: 13456
		[Range(0.1f, 1f)]
		[SerializeField]
		private float m_spawnHealthMultiplier = 1f;

		// Token: 0x04003491 RID: 13457
		[SerializeField]
		private bool m_deliverNonAshenEssence;

		// Token: 0x04003492 RID: 13458
		[SerializeField]
		private AnimationCurve m_nonAshenChanceCurve;

		// Token: 0x04003493 RID: 13459
		[SerializeField]
		private int m_nonAshenEssence = 1;

		// Token: 0x04003494 RID: 13460
		[SerializeField]
		private float m_nonAshenEssenceCapFraction = 0.25f;

		// Token: 0x04003495 RID: 13461
		private const string kMonolithGrp = "Monoliths";

		// Token: 0x04003496 RID: 13462
		[SerializeField]
		private bool m_bypassMonolithCosts;

		// Token: 0x04003497 RID: 13463
		[SerializeField]
		private AshenSettings.MonolithCostEntry[] m_monolithCostData;

		// Token: 0x04003498 RID: 13464
		[SerializeField]
		private MonolithProfile m_costReductionProfile;

		// Token: 0x04003499 RID: 13465
		[Range(0f, 1f)]
		[SerializeField]
		private float m_costReductionMultiplier = 1f;

		// Token: 0x0400349A RID: 13466
		[SerializeField]
		private uint m_travelEssencePurchaseCost = 100U;

		// Token: 0x0400349B RID: 13467
		[SerializeField]
		private int m_travelConversionRatio = 1;

		// Token: 0x0400349C RID: 13468
		[SerializeField]
		private MinMaxFloatRange m_ruptureRippleDelay = new MinMaxFloatRange(20f, 30f);

		// Token: 0x0400349D RID: 13469
		private const string kAudioGroup = "Audio";

		// Token: 0x0400349E RID: 13470
		[SerializeField]
		private bool m_addDistortionFilter;

		// Token: 0x0400349F RID: 13471
		[SerializeField]
		private float m_distortionLevel = 0.4f;

		// Token: 0x040034A0 RID: 13472
		[SerializeField]
		private bool m_addChorusFilter;

		// Token: 0x040034A1 RID: 13473
		[SerializeField]
		private float m_chorusRate = 2f;

		// Token: 0x040034A2 RID: 13474
		[SerializeField]
		private float m_chorusDepth = 0.5f;

		// Token: 0x040034A3 RID: 13475
		private const string kMonoToEmberRing = "Monolith to Ember Ring";

		// Token: 0x040034A4 RID: 13476
		[SerializeField]
		private bool m_allowEmberRingTeleportsFromMonolith;

		// Token: 0x040034A5 RID: 13477
		[SerializeField]
		private int m_emberRingFromMonolithEssenceCost = 5;

		// Token: 0x040034A6 RID: 13478
		private const string kMonoToGroupEmberRing = "Monolith to Group";

		// Token: 0x040034A7 RID: 13479
		[SerializeField]
		private bool m_allowGroupTeleportsFromMonolith;

		// Token: 0x040034A8 RID: 13480
		[SerializeField]
		private int m_groupFromMonolithEssenceCost = 30;

		// Token: 0x040034A9 RID: 13481
		[Range(1f, 5f)]
		[SerializeField]
		private int m_groupTeleportMemberThreshold = 3;

		// Token: 0x040034AA RID: 13482
		[FormerlySerializedAs("m_availableGroupEmberRingProfiles")]
		[SerializeField]
		private LeyLinkProfile[] m_availableLeyLinkProfiles;

		// Token: 0x040034AB RID: 13483
		[SerializeField]
		private List<ZoneId> m_allowGroupTeleportTo;

		// Token: 0x040034AC RID: 13484
		[SerializeField]
		private bool m_disableAlchemy;

		// Token: 0x040034AD RID: 13485
		[SerializeField]
		private DeploymentBranchFlags m_alchemyBranchFlags;

		// Token: 0x040034AE RID: 13486
		[SerializeField]
		private Quest m_alchemyQuestRequirement;

		// Token: 0x040034AF RID: 13487
		[Range(1f, 50f)]
		[SerializeField]
		private int m_alchemyLevelThreshold = 20;

		// Token: 0x040034B0 RID: 13488
		[SerializeField]
		private float m_defaultGlowIntensity = 2f;

		// Token: 0x040034B1 RID: 13489
		[SerializeField]
		private float m_glowAnimationSpeed = 5f;

		// Token: 0x040034B2 RID: 13490
		[SerializeField]
		private float m_extendedAlchemyCooldownMultiplier = 2f;

		// Token: 0x040034B3 RID: 13491
		[SerializeField]
		private AshenSettings.AlchemySettings m_alchemyI;

		// Token: 0x040034B4 RID: 13492
		[SerializeField]
		private AshenSettings.AlchemySettings m_alchemyII;

		// Token: 0x040034B5 RID: 13493
		[SerializeField]
		private MinMaxIntRange m_essencePerFlatworm = new MinMaxIntRange(10, 20);

		// Token: 0x0200071B RID: 1819
		[Serializable]
		private class MonolithCostEntry
		{
			// Token: 0x17000C19 RID: 3097
			// (get) Token: 0x060036B2 RID: 14002 RVA: 0x00065722 File Offset: 0x00063922
			public MonolithFlags Flags
			{
				get
				{
					return this.m_monolithFlags;
				}
			}

			// Token: 0x17000C1A RID: 3098
			// (get) Token: 0x060036B3 RID: 14003 RVA: 0x0006572A File Offset: 0x0006392A
			public int Cost
			{
				get
				{
					return this.m_essenceCost;
				}
			}

			// Token: 0x040034B6 RID: 13494
			[SerializeField]
			private MonolithFlags m_monolithFlags;

			// Token: 0x040034B7 RID: 13495
			[SerializeField]
			private int m_essenceCost;
		}

		// Token: 0x0200071C RID: 1820
		[Serializable]
		private class AlchemySettings
		{
			// Token: 0x17000C1B RID: 3099
			// (get) Token: 0x060036B5 RID: 14005 RVA: 0x00065732 File Offset: 0x00063932
			public int AdditionalExecutionTime
			{
				get
				{
					return this.m_additionalExecutionTime;
				}
			}

			// Token: 0x17000C1C RID: 3100
			// (get) Token: 0x060036B6 RID: 14006 RVA: 0x0006573A File Offset: 0x0006393A
			public int Cost
			{
				get
				{
					return this.m_cost;
				}
			}

			// Token: 0x17000C1D RID: 3101
			// (get) Token: 0x060036B7 RID: 14007 RVA: 0x00065742 File Offset: 0x00063942
			public PooledVFX VFX
			{
				get
				{
					return this.m_sourceExecution;
				}
			}

			// Token: 0x17000C1E RID: 3102
			// (get) Token: 0x060036B8 RID: 14008 RVA: 0x0006574A File Offset: 0x0006394A
			public float Intensity
			{
				get
				{
					return this.m_stoneGlowIntensity;
				}
			}

			// Token: 0x17000C1F RID: 3103
			// (get) Token: 0x060036B9 RID: 14009 RVA: 0x00065752 File Offset: 0x00063952
			public Color UIHighlightColor
			{
				get
				{
					return this.m_uiHighlightColor;
				}
			}

			// Token: 0x17000C20 RID: 3104
			// (get) Token: 0x060036BA RID: 14010 RVA: 0x0006575A File Offset: 0x0006395A
			public int CooldownTime
			{
				get
				{
					return this.m_cooldownTime;
				}
			}

			// Token: 0x17000C21 RID: 3105
			// (get) Token: 0x060036BB RID: 14011 RVA: 0x00065762 File Offset: 0x00063962
			public int UsageThreshold
			{
				get
				{
					return this.m_usageThreshold;
				}
			}

			// Token: 0x17000C22 RID: 3106
			// (get) Token: 0x060036BC RID: 14012 RVA: 0x0006576A File Offset: 0x0006396A
			public AudioClip ActivateClip
			{
				get
				{
					return this.m_clip;
				}
			}

			// Token: 0x17000C23 RID: 3107
			// (get) Token: 0x060036BD RID: 14013 RVA: 0x00065772 File Offset: 0x00063972
			public Vector2 Volume
			{
				get
				{
					return this.m_volume;
				}
			}

			// Token: 0x17000C24 RID: 3108
			// (get) Token: 0x060036BE RID: 14014 RVA: 0x0006577A File Offset: 0x0006397A
			public Vector2 Pitch
			{
				get
				{
					return this.m_pitch;
				}
			}

			// Token: 0x040034B8 RID: 13496
			[SerializeField]
			private int m_additionalExecutionTime = 1;

			// Token: 0x040034B9 RID: 13497
			[SerializeField]
			private int m_cost = 10;

			// Token: 0x040034BA RID: 13498
			[SerializeField]
			private PooledVFX m_sourceExecution;

			// Token: 0x040034BB RID: 13499
			[SerializeField]
			private float m_stoneGlowIntensity = 4f;

			// Token: 0x040034BC RID: 13500
			[SerializeField]
			private Color m_uiHighlightColor;

			// Token: 0x040034BD RID: 13501
			[SerializeField]
			private int m_cooldownTime = 10;

			// Token: 0x040034BE RID: 13502
			[Tooltip("How many uses of the previous tier required to unlock this tier?")]
			[SerializeField]
			private int m_usageThreshold;

			// Token: 0x040034BF RID: 13503
			[SerializeField]
			private AudioClip m_clip;

			// Token: 0x040034C0 RID: 13504
			[SerializeField]
			private Vector2 m_volume = Vector2.one;

			// Token: 0x040034C1 RID: 13505
			[SerializeField]
			private Vector2 m_pitch = Vector2.one;
		}
	}
}
