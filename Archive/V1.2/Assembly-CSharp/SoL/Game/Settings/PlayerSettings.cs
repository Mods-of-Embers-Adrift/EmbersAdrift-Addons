using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.LiveMigrators;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000738 RID: 1848
	[Serializable]
	public class PlayerSettings
	{
		// Token: 0x06003741 RID: 14145 RVA: 0x00065D38 File Offset: 0x00063F38
		public float GetDurabilityLossOnHitMultiplier(float playerLevel)
		{
			return this.m_damageLossOnHitMultiplierCurve.Evaluate(playerLevel);
		}

		// Token: 0x17000C6A RID: 3178
		// (get) Token: 0x06003742 RID: 14146 RVA: 0x00065D46 File Offset: 0x00063F46
		public float ArmorDegradationAllowedFraction
		{
			get
			{
				return this.m_armorDegradationAllowedFraction;
			}
		}

		// Token: 0x17000C6B RID: 3179
		// (get) Token: 0x06003743 RID: 14147 RVA: 0x00065D4E File Offset: 0x00063F4E
		public float WeaponDegradationAllowedFraction
		{
			get
			{
				return this.m_weaponDegradationAllowedFraction;
			}
		}

		// Token: 0x17000C6C RID: 3180
		// (get) Token: 0x06003744 RID: 14148 RVA: 0x00065D56 File Offset: 0x00063F56
		public BankProfile PersonalBankProfile
		{
			get
			{
				return this.m_personalBankProfile;
			}
		}

		// Token: 0x17000C6D RID: 3181
		// (get) Token: 0x06003745 RID: 14149 RVA: 0x00065D5E File Offset: 0x00063F5E
		private IEnumerable GetProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BankProfile>();
			}
		}

		// Token: 0x17000C6E RID: 3182
		// (get) Token: 0x06003746 RID: 14150 RVA: 0x00065D65 File Offset: 0x00063F65
		public float StartingHealthWounds
		{
			get
			{
				return this.m_startingHealthWounds;
			}
		}

		// Token: 0x17000C6F RID: 3183
		// (get) Token: 0x06003747 RID: 14151 RVA: 0x00065D6D File Offset: 0x00063F6D
		public float StartingStaminaWounds
		{
			get
			{
				return this.m_startingStaminaWounds;
			}
		}

		// Token: 0x17000C70 RID: 3184
		// (get) Token: 0x06003748 RID: 14152 RVA: 0x00065D75 File Offset: 0x00063F75
		public float OnRoadSpeedMod
		{
			get
			{
				return this.m_onRoadSpeedMod;
			}
		}

		// Token: 0x06003749 RID: 14153 RVA: 0x00065D7D File Offset: 0x00063F7D
		public float GetDroppedCurrencyMultiplier(int level)
		{
			if (this.m_droppedCurrencyMultiplier == null)
			{
				return 1f;
			}
			return this.m_droppedCurrencyMultiplier.Evaluate((float)level);
		}

		// Token: 0x17000C71 RID: 3185
		// (get) Token: 0x0600374A RID: 14154 RVA: 0x00065D9A File Offset: 0x00063F9A
		public bool BagBuybackEnabled
		{
			get
			{
				return this.m_bagBuybackEnabled;
			}
		}

		// Token: 0x0600374B RID: 14155 RVA: 0x0016B214 File Offset: 0x00169414
		private ulong GetMinimumBagBuybackCost(GameEntity entity)
		{
			if (entity && entity.CharacterData && entity.CharacterData.AdventuringLevel > 0)
			{
				return (ulong)((long)Mathf.FloorToInt(this.m_bagBuybackMinimum.Evaluate((float)entity.CharacterData.AdventuringLevel)));
			}
			return 10000UL;
		}

		// Token: 0x0600374C RID: 14156 RVA: 0x0016B268 File Offset: 0x00169468
		private string GetDecadalBreakdown()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				for (int i = 0; i < GlobalSettings.kEveryFiveLevels.Length; i++)
				{
					int num = GlobalSettings.kEveryFiveLevels[i];
					ulong currency = (ulong)((long)Mathf.FloorToInt(this.m_bagBuybackMinimum.Evaluate((float)num)));
					CurrencyConverter currencyConverter = new CurrencyConverter(currency);
					utf16ValueStringBuilder.AppendFormat<int, string>("[{0:00}] {1}", num, currencyConverter.ToString());
					if (i < GlobalSettings.kEveryFiveLevels.Length - 1)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600374D RID: 14157 RVA: 0x0016B318 File Offset: 0x00169518
		public ulong GetBagBuybackCost(GameEntity entity, ArchetypeInstance instance)
		{
			if (!entity || instance == null)
			{
				return 0UL;
			}
			ulong sellPrice = instance.GetSellPrice();
			ulong minimumBagBuybackCost = this.GetMinimumBagBuybackCost(entity);
			return (ulong)(sellPrice * this.m_bagBuybackSellPriceMultiplier) + minimumBagBuybackCost;
		}

		// Token: 0x0600374E RID: 14158 RVA: 0x0016B350 File Offset: 0x00169550
		public bool LiveMigrateItems(CharacterRecord record)
		{
			if (record == null || this.m_liveMigrators == null || this.m_liveMigrators.Length == 0)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < this.m_liveMigrators.Length; i++)
			{
				flag = ((this.m_liveMigrators[i] && this.m_liveMigrators[i].LiveMigrate(record)) || flag);
			}
			return flag;
		}

		// Token: 0x17000C72 RID: 3186
		// (get) Token: 0x0600374F RID: 14159 RVA: 0x00065DA2 File Offset: 0x00063FA2
		public float HealthRegenStatRateMultiplier
		{
			get
			{
				return this.m_healthRegenStatRateMultiplier;
			}
		}

		// Token: 0x040035E1 RID: 13793
		public GameObject PlayerPrefab;

		// Token: 0x040035E2 RID: 13794
		public GameObject PlayerCorpse;

		// Token: 0x040035E3 RID: 13795
		public bool PlayersUseProximity = true;

		// Token: 0x040035E4 RID: 13796
		public bool NpcsUseProximity = true;

		// Token: 0x040035E5 RID: 13797
		public StartingData StartingData;

		// Token: 0x040035E6 RID: 13798
		public WoundProfile HealthWoundProfile;

		// Token: 0x040035E7 RID: 13799
		public WoundProfile StaminaWoundProfile;

		// Token: 0x040035E8 RID: 13800
		public float WakeUpTime = 30f;

		// Token: 0x040035E9 RID: 13801
		public float GiveUpTime = 100f;

		// Token: 0x040035EA RID: 13802
		public float DeadDelayTime = 10f;

		// Token: 0x040035EB RID: 13803
		public float StaminaRecoveryTime = 60f;

		// Token: 0x040035EC RID: 13804
		[Range(0f, 1f)]
		public float StaminaCostJump = 0.1f;

		// Token: 0x040035ED RID: 13805
		public bool StaminaDrainsDuringExecution;

		// Token: 0x040035EE RID: 13806
		public bool AllowJumpToCancelExecution;

		// Token: 0x040035EF RID: 13807
		public bool AlwaysRun = true;

		// Token: 0x040035F0 RID: 13808
		public float ExecutionMovementPenalty = -0.8f;

		// Token: 0x040035F1 RID: 13809
		public float MinGroundSlowAngle = 20f;

		// Token: 0x040035F2 RID: 13810
		public float MaxGroundSlowFraction = 0.9f;

		// Token: 0x040035F3 RID: 13811
		public float MinGroundSlowFraciton = 0.4f;

		// Token: 0x040035F4 RID: 13812
		public float MaxStableSlopeAngle = 60f;

		// Token: 0x040035F5 RID: 13813
		public AnimationCurve FallDamageCurve;

		// Token: 0x040035F6 RID: 13814
		public AnimationCurve SafeFallEffectiveness;

		// Token: 0x040035F7 RID: 13815
		public GameObject ReticlePrefab;

		// Token: 0x040035F8 RID: 13816
		public CurrencyValue StartingCurrency = new CurrencyValue();

		// Token: 0x040035F9 RID: 13817
		public float BlacksmithRepairMultiplier = 0.5f;

		// Token: 0x040035FA RID: 13818
		public int PrimaryStatBonus = 2;

		// Token: 0x040035FB RID: 13819
		public int SecondaryStatBonus = 1;

		// Token: 0x040035FC RID: 13820
		public int TertiaryStatBonus;

		// Token: 0x040035FD RID: 13821
		public ulong StartingAdventuringPool = 10UL;

		// Token: 0x040035FE RID: 13822
		public AnimationCurve MasteryPointAwardCurve;

		// Token: 0x040035FF RID: 13823
		public AnimationCurve ExperienceAwardCurve;

		// Token: 0x04003600 RID: 13824
		public bool LimitBackpackIndicatorByDistance;

		// Token: 0x04003601 RID: 13825
		public float BackpackIndicatorDistance = 50f;

		// Token: 0x04003602 RID: 13826
		[Range(0f, 1f)]
		[Obsolete]
		public float DurabilityLossOnHitMultiplier = 1f;

		// Token: 0x04003603 RID: 13827
		[Tooltip("X-Axis is player level, Y-Axis is the value multiplied by absorbed to lower/increase the value that hits item durability per hit")]
		[SerializeField]
		private AnimationCurve m_damageLossOnHitMultiplierCurve = AnimationCurve.Linear(0f, 1f, 50f, 1f);

		// Token: 0x04003604 RID: 13828
		[Range(0f, 1f)]
		public float DurabilityLossOnDeath = 0.1f;

		// Token: 0x04003605 RID: 13829
		[Range(0f, 1f)]
		[SerializeField]
		private float m_armorDegradationAllowedFraction = 0.2f;

		// Token: 0x04003606 RID: 13830
		[Range(0f, 1f)]
		[SerializeField]
		private float m_weaponDegradationAllowedFraction = 0.8f;

		// Token: 0x04003607 RID: 13831
		[SerializeField]
		private BankProfile m_personalBankProfile;

		// Token: 0x04003608 RID: 13832
		[Range(0f, 100f)]
		[SerializeField]
		private float m_startingHealthWounds = 5f;

		// Token: 0x04003609 RID: 13833
		[Range(0f, 100f)]
		[SerializeField]
		private float m_startingStaminaWounds = 2f;

		// Token: 0x0400360A RID: 13834
		[SerializeField]
		private float m_onRoadSpeedMod = 0.1f;

		// Token: 0x0400360B RID: 13835
		[SerializeField]
		private AnimationCurve m_droppedCurrencyMultiplier = AnimationCurve.Linear(0f, 1f, 50f, 1f);

		// Token: 0x0400360C RID: 13836
		private const string kBagBuyback = "Bag Buyback";

		// Token: 0x0400360D RID: 13837
		[SerializeField]
		private bool m_bagBuybackEnabled;

		// Token: 0x0400360E RID: 13838
		[SerializeField]
		private float m_bagBuybackSellPriceMultiplier = 2f;

		// Token: 0x0400360F RID: 13839
		[SerializeField]
		private AnimationCurve m_bagBuybackMinimum = AnimationCurve.Linear(0f, 10000f, 50f, 10000f);

		// Token: 0x04003610 RID: 13840
		[SerializeField]
		private BaseLiveItemMigrator[] m_liveMigrators;

		// Token: 0x04003611 RID: 13841
		[Tooltip("Health Regen Stat Contribution Multiplier.\nregenRate += X health regen * <this value>")]
		[SerializeField]
		private float m_healthRegenStatRateMultiplier;
	}
}
