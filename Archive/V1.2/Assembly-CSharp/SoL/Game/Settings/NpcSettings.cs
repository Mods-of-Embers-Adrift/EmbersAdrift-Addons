using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.Settings
{
	// Token: 0x02000733 RID: 1843
	[Serializable]
	public class NpcSettings
	{
		// Token: 0x06003725 RID: 14117 RVA: 0x00065C17 File Offset: 0x00063E17
		public float GetRegenFraction(bool hasTargets, float recoveryFraction)
		{
			if (hasTargets)
			{
				return this.m_inCombatRegenFraction;
			}
			if (recoveryFraction >= 1f)
			{
				return this.m_outOfCombatRegenFraction;
			}
			if (recoveryFraction <= 0f)
			{
				return this.m_inCombatRegenFraction;
			}
			return Mathf.Lerp(this.m_inCombatRegenFraction, this.m_outOfCombatRegenFraction, recoveryFraction);
		}

		// Token: 0x06003726 RID: 14118 RVA: 0x00065C53 File Offset: 0x00063E53
		public float GetRegenFraction(bool hasTargets)
		{
			if (!hasTargets)
			{
				return this.m_outOfCombatRegenFraction;
			}
			return this.m_inCombatRegenFraction;
		}

		// Token: 0x17000C5D RID: 3165
		// (get) Token: 0x06003727 RID: 14119 RVA: 0x00065C65 File Offset: 0x00063E65
		public float UndonesWrathHealthFraction
		{
			get
			{
				return this.m_undonesWrathHealthFraction;
			}
		}

		// Token: 0x17000C5E RID: 3166
		// (get) Token: 0x06003728 RID: 14120 RVA: 0x00065C6D File Offset: 0x00063E6D
		public BaseArchetype UndonesWrathArchetype
		{
			get
			{
				return this.m_undonesWrathArchetype;
			}
		}

		// Token: 0x17000C5F RID: 3167
		// (get) Token: 0x06003729 RID: 14121 RVA: 0x00065C75 File Offset: 0x00063E75
		public float ResetDistanceLeashMultiplier
		{
			get
			{
				return this.m_resetDistanceLeashMultiplier;
			}
		}

		// Token: 0x17000C60 RID: 3168
		// (get) Token: 0x0600372A RID: 14122 RVA: 0x00065C7D File Offset: 0x00063E7D
		public float ResetDistanceMinimum
		{
			get
			{
				return this.m_resetDistanceMinimum;
			}
		}

		// Token: 0x17000C61 RID: 3169
		// (get) Token: 0x0600372B RID: 14123 RVA: 0x00065C85 File Offset: 0x00063E85
		public float ResetRegenRate
		{
			get
			{
				return this.m_resetRegenRate;
			}
		}

		// Token: 0x17000C62 RID: 3170
		// (get) Token: 0x0600372C RID: 14124 RVA: 0x00065C8D File Offset: 0x00063E8D
		public float WeaponDistanceWalkThresh
		{
			get
			{
				return this.m_weaponDistanceWalkThresh;
			}
		}

		// Token: 0x17000C63 RID: 3171
		// (get) Token: 0x0600372D RID: 14125 RVA: 0x00065C95 File Offset: 0x00063E95
		public float WeaponDistanceRunThresh
		{
			get
			{
				return this.m_weaponDistanceRunThresh;
			}
		}

		// Token: 0x17000C64 RID: 3172
		// (get) Token: 0x0600372E RID: 14126 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x0600372F RID: 14127 RVA: 0x0016AE54 File Offset: 0x00169054
		public string GetDifficultyChallengeText(DifficultyRating difficulty, ChallengeRating challengeRating)
		{
			if (challengeRating == ChallengeRating.CRB)
			{
				challengeRating = ChallengeRating.CR4;
			}
			for (int i = 0; i < this.m_difficultyChallengeTexts.Length; i++)
			{
				if (this.m_difficultyChallengeTexts[i].Difficulty == difficulty && this.m_difficultyChallengeTexts[i].Challenge == challengeRating)
				{
					return this.m_difficultyChallengeTexts[i].Description;
				}
			}
			return "Unknown";
		}

		// Token: 0x06003730 RID: 14128 RVA: 0x0016AEB0 File Offset: 0x001690B0
		public Color GetDifficultyRatingColor(DifficultyRating difficulty)
		{
			if (NpcSettings.m_difficultyColorDict == null)
			{
				NpcSettings.m_difficultyColorDict = new Dictionary<DifficultyRating, Color>(default(DifficultyRatingComparer));
				for (int i = 0; i < this.m_difficultyColors.Length; i++)
				{
					NpcSettings.m_difficultyColorDict.Add(this.m_difficultyColors[i].DifficultyRating, this.m_difficultyColors[i].Color);
				}
			}
			Color white = Color.white;
			NpcSettings.m_difficultyColorDict.TryGetValue(difficulty, out white);
			return white;
		}

		// Token: 0x06003731 RID: 14129 RVA: 0x0016AF28 File Offset: 0x00169128
		public float GetStanceThreatMultiplier(Stance stance)
		{
			if (this.m_stanceMultiplierData == null)
			{
				this.m_stanceMultiplierData = new Dictionary<Stance, NpcSettings.StanceThreatMultiplierData>(default(StanceComparer));
				for (int i = 0; i < this.m_stanceThreatMultipliers.Length; i++)
				{
					this.m_stanceMultiplierData.AddOrReplace(this.m_stanceThreatMultipliers[i].Stance, this.m_stanceThreatMultipliers[i]);
				}
			}
			NpcSettings.StanceThreatMultiplierData stanceThreatMultiplierData;
			if (!this.m_stanceMultiplierData.TryGetValue(stance, out stanceThreatMultiplierData))
			{
				return 1f;
			}
			return stanceThreatMultiplierData.ThreatMultiplier;
		}

		// Token: 0x17000C65 RID: 3173
		// (get) Token: 0x06003732 RID: 14130 RVA: 0x00065C9D File Offset: 0x00063E9D
		public AppliableEffectAbility GlobalSpeedBuff
		{
			get
			{
				return this.m_globalSpeedBuff;
			}
		}

		// Token: 0x17000C66 RID: 3174
		// (get) Token: 0x06003733 RID: 14131 RVA: 0x00065CA5 File Offset: 0x00063EA5
		public AppliableEffectAbility GlobalSpeedDebuff
		{
			get
			{
				return this.m_globalSpeedDebuff;
			}
		}

		// Token: 0x06003734 RID: 14132 RVA: 0x00065CAD File Offset: 0x00063EAD
		public bool ApplySpeedDebuff(int npcLevel, int playerLevel, bool isPlayerInCombatStance)
		{
			return this.m_speedDebuffParams != null && this.m_globalSpeedDebuff != null && this.m_speedDebuffParams.Apply(npcLevel, playerLevel, isPlayerInCombatStance);
		}

		// Token: 0x06003735 RID: 14133 RVA: 0x00065CD5 File Offset: 0x00063ED5
		public float GetSensorDampenerMultiplier(int levelDelta)
		{
			return this.m_sensorDampingCurve.Evaluate((float)levelDelta);
		}

		// Token: 0x06003736 RID: 14134 RVA: 0x00065CE4 File Offset: 0x00063EE4
		public int GetScaledArmorClass(int playerLevel)
		{
			if (this.m_scalingArmorCurve == null)
			{
				return 0;
			}
			return Mathf.FloorToInt(this.m_scalingArmorCurve.Evaluate((float)playerLevel));
		}

		// Token: 0x06003737 RID: 14135 RVA: 0x0016AFA8 File Offset: 0x001691A8
		public void LevelUpEmote(string npcName, System.Random seededRandom)
		{
			string arg = "has increased in power!";
			if (this.m_levelUpMessages)
			{
				StringProbabilityEntry entry = this.m_levelUpMessages.GetEntry(seededRandom);
				if (entry != null)
				{
					arg = entry.Obj;
				}
			}
			string content = ZString.Format<string, string>("{0} {1}", npcName, arg);
			MessageManager.ChatQueue.AddToQueue(MessageType.Emote, content);
		}

		// Token: 0x040035B3 RID: 13747
		private const string kCanHitGroup = "Can Hit";

		// Token: 0x040035B4 RID: 13748
		[Tooltip("If set to true NPCs will NOT check that they have a valid path to the player's IndexedPosition")]
		public bool BypassCanHitPathCheck;

		// Token: 0x040035B5 RID: 13749
		[Tooltip("Distance to check for a nav mesh under the player's IndexedPosition to determine if they can hit the NPC")]
		public float CanHitSampleDistance = 2f;

		// Token: 0x040035B6 RID: 13750
		public float TargetExpirationTime = 30f;

		// Token: 0x040035B7 RID: 13751
		public float CorpseDecayTime = 180f;

		// Token: 0x040035B8 RID: 13752
		[Range(0f, 1f)]
		public float CorpsePermissionTimeout = 1f;

		// Token: 0x040035B9 RID: 13753
		public Color NameplateColor = Color.white;

		// Token: 0x040035BA RID: 13754
		private const string kHealthRegen = "Health Regen";

		// Token: 0x040035BB RID: 13755
		[Range(0f, 1f)]
		[SerializeField]
		private float m_inCombatRegenFraction = 0.005f;

		// Token: 0x040035BC RID: 13756
		[Range(0f, 1f)]
		[SerializeField]
		private float m_outOfCombatRegenFraction = 0.1f;

		// Token: 0x040035BD RID: 13757
		public MinMaxFloatRange DamageToAbsorbOnGenerate = new MinMaxFloatRange(0f, 1f);

		// Token: 0x040035BE RID: 13758
		public LayerMask TerrainLayerMask;

		// Token: 0x040035BF RID: 13759
		public ObstacleAvoidanceType MotorObstacleAvoidanceType;

		// Token: 0x040035C0 RID: 13760
		[Range(0f, 1f)]
		[SerializeField]
		private float m_undonesWrathHealthFraction = 0.1f;

		// Token: 0x040035C1 RID: 13761
		[SerializeField]
		private BaseArchetype m_undonesWrathArchetype;

		// Token: 0x040035C2 RID: 13762
		[SerializeField]
		private float m_resetDistanceLeashMultiplier = 2f;

		// Token: 0x040035C3 RID: 13763
		[SerializeField]
		private float m_resetDistanceMinimum = 50f;

		// Token: 0x040035C4 RID: 13764
		[SerializeField]
		private float m_resetRegenRate = 10f;

		// Token: 0x040035C5 RID: 13765
		[SerializeField]
		private float m_weaponDistanceWalkThresh = 1.2f;

		// Token: 0x040035C6 RID: 13766
		[SerializeField]
		private float m_weaponDistanceRunThresh = 1.6f;

		// Token: 0x040035C7 RID: 13767
		[SerializeField]
		private NpcSettings.DifficultyChallengeText[] m_difficultyChallengeTexts;

		// Token: 0x040035C8 RID: 13768
		[SerializeField]
		private NpcSettings.DifficultyColor[] m_difficultyColors;

		// Token: 0x040035C9 RID: 13769
		[NonSerialized]
		private static Dictionary<DifficultyRating, Color> m_difficultyColorDict;

		// Token: 0x040035CA RID: 13770
		[SerializeField]
		private NpcSettings.StanceThreatMultiplierData[] m_stanceThreatMultipliers;

		// Token: 0x040035CB RID: 13771
		private Dictionary<Stance, NpcSettings.StanceThreatMultiplierData> m_stanceMultiplierData;

		// Token: 0x040035CC RID: 13772
		[SerializeField]
		private AppliableEffectAbility m_globalSpeedBuff;

		// Token: 0x040035CD RID: 13773
		[SerializeField]
		private AppliableEffectAbility m_globalSpeedDebuff;

		// Token: 0x040035CE RID: 13774
		[SerializeField]
		private NpcSettings.SpeedDebuffParams m_speedDebuffParams;

		// Token: 0x040035CF RID: 13775
		[Tooltip("X-Axis is Level Delta. Y-Axis is the multiplier for the sensor Distance")]
		[SerializeField]
		private AnimationCurve m_sensorDampingCurve = AnimationCurve.Linear(0f, 1f, 20f, 1f);

		// Token: 0x040035D0 RID: 13776
		[SerializeField]
		private AnimationCurve m_scalingArmorCurve = AnimationCurve.Linear(0f, 30f, 50f, 1000f);

		// Token: 0x040035D1 RID: 13777
		[SerializeField]
		private StringScriptableProbabilityCollection m_levelUpMessages;

		// Token: 0x02000734 RID: 1844
		[Serializable]
		private class DifficultyChallengeText
		{
			// Token: 0x040035D2 RID: 13778
			private const string kGroup = "DCT";

			// Token: 0x040035D3 RID: 13779
			public DifficultyRating Difficulty;

			// Token: 0x040035D4 RID: 13780
			public ChallengeRating Challenge;

			// Token: 0x040035D5 RID: 13781
			public string Description;
		}

		// Token: 0x02000735 RID: 1845
		[Serializable]
		private class DifficultyColor
		{
			// Token: 0x17000C67 RID: 3175
			// (get) Token: 0x0600373A RID: 14138 RVA: 0x0004F9FB File Offset: 0x0004DBFB
			private IEnumerable GetColorValues
			{
				get
				{
					return SolOdinUtilities.GetColorValues();
				}
			}

			// Token: 0x040035D6 RID: 13782
			public DifficultyRating DifficultyRating;

			// Token: 0x040035D7 RID: 13783
			public Color Color = Color.white;
		}

		// Token: 0x02000736 RID: 1846
		[Serializable]
		private class StanceThreatMultiplierData
		{
			// Token: 0x17000C68 RID: 3176
			// (get) Token: 0x0600373C RID: 14140 RVA: 0x00065D15 File Offset: 0x00063F15
			public Stance Stance
			{
				get
				{
					return this.m_stance;
				}
			}

			// Token: 0x17000C69 RID: 3177
			// (get) Token: 0x0600373D RID: 14141 RVA: 0x00065D1D File Offset: 0x00063F1D
			public float ThreatMultiplier
			{
				get
				{
					return this.m_threatMultiplier;
				}
			}

			// Token: 0x040035D8 RID: 13784
			[SerializeField]
			private Stance m_stance;

			// Token: 0x040035D9 RID: 13785
			[Range(0.5f, 2f)]
			[SerializeField]
			private float m_threatMultiplier = 1f;
		}

		// Token: 0x02000737 RID: 1847
		[Serializable]
		private class SpeedDebuffParams
		{
			// Token: 0x0600373F RID: 14143 RVA: 0x0016B0F8 File Offset: 0x001692F8
			internal bool Apply(int npcLevel, int playerLevel, bool isPlayerInCombatStance)
			{
				float num = this.m_baseChance;
				if (playerLevel > npcLevel)
				{
					float num2 = (float)playerLevel - (float)npcLevel;
					num = ((num2 >= 5f) ? this.m_minChance : Mathf.Lerp(this.m_minChance, this.m_baseChance, 1f - num2 / 5f));
				}
				else if (npcLevel > playerLevel)
				{
					float num3 = (float)npcLevel - (float)playerLevel;
					num = ((num3 >= 5f) ? this.m_maxChance : Mathf.Lerp(this.m_baseChance, this.m_maxChance, num3 / 5f));
				}
				num = Mathf.Clamp(num, this.m_minChance, this.m_maxChance);
				if (isPlayerInCombatStance)
				{
					num *= this.m_combatStanceMultiplier;
				}
				if (playerLevel <= this.m_lowLevelThreshold)
				{
					num *= this.m_lowLevelMultiplier;
				}
				return UnityEngine.Random.Range(0f, 1f) <= num;
			}

			// Token: 0x040035DA RID: 13786
			private const float kLevelThreshold = 5f;

			// Token: 0x040035DB RID: 13787
			[SerializeField]
			private float m_minChance = 0.1f;

			// Token: 0x040035DC RID: 13788
			[SerializeField]
			private float m_baseChance = 0.2f;

			// Token: 0x040035DD RID: 13789
			[SerializeField]
			private float m_maxChance = 0.4f;

			// Token: 0x040035DE RID: 13790
			[SerializeField]
			private int m_lowLevelThreshold = 6;

			// Token: 0x040035DF RID: 13791
			[SerializeField]
			private float m_lowLevelMultiplier = 0.5f;

			// Token: 0x040035E0 RID: 13792
			[SerializeField]
			private float m_combatStanceMultiplier = 0.8f;
		}
	}
}
