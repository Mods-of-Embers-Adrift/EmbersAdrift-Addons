using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SoL.Game.EffectSystem;
using SoL.Game.Flanking;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D93 RID: 3475
	[CreateAssetMenu(menuName = "SoL/Tests/CombatHitTester")]
	public class CombatHitTester : ScriptableObject, IHandHeldItems
	{
		// Token: 0x1700190B RID: 6411
		// (get) Token: 0x0600686C RID: 26732 RVA: 0x000861C2 File Offset: 0x000843C2
		private bool m_showCustomMaxAbsorb
		{
			get
			{
				return this.m_maxAbsorbDamageReductionType == CombatHitTester.TargetMaxAbsorb.Custom;
			}
		}

		// Token: 0x0600686D RID: 26733 RVA: 0x000861CD File Offset: 0x000843CD
		private void CacheHandheldItems()
		{
			if (this.m_mainHandCache == null)
			{
				this.m_mainHandCache = new CachedHandHeldItem(true);
			}
			if (this.m_offHandCache == null)
			{
				this.m_offHandCache = new CachedHandHeldItem(false);
			}
		}

		// Token: 0x1700190C RID: 6412
		// (get) Token: 0x0600686E RID: 26734 RVA: 0x000861F7 File Offset: 0x000843F7
		CachedHandHeldItem IHandHeldItems.MainHand
		{
			get
			{
				this.CacheHandheldItems();
				return this.m_mainHandCache;
			}
		}

		// Token: 0x1700190D RID: 6413
		// (get) Token: 0x0600686F RID: 26735 RVA: 0x00086205 File Offset: 0x00084405
		CachedHandHeldItem IHandHeldItems.OffHand
		{
			get
			{
				this.CacheHandheldItems();
				return this.m_offHandCache;
			}
		}

		// Token: 0x06006870 RID: 26736 RVA: 0x00086213 File Offset: 0x00084413
		private IEnumerable GetHandHeldItems()
		{
			return SolOdinUtilities.GetDropdownItems<EquipableItem>((EquipableItem x) => x is IHandheldItem);
		}

		// Token: 0x06006871 RID: 26737 RVA: 0x0021446C File Offset: 0x0021266C
		private void Roll()
		{
			int num = Mathf.Clamp(this.m_rollCount, 1, int.MaxValue);
			Dictionary<HitType, int> dictionary = new Dictionary<HitType, int>(default(HitTypeComparer));
			CombatHitTester.RollResult rollResult = default(CombatHitTester.RollResult);
			for (int i = 0; i < num; i++)
			{
				CombatHitTester.RollResult rollResult2 = this.GetRollResult();
				rollResult.AddResultFromAnother(rollResult2);
				int num2;
				if (dictionary.TryGetValue(rollResult2.HitType, out num2))
				{
					dictionary[rollResult2.HitType] = num2 + 1;
				}
				else
				{
					dictionary.Add(rollResult2.HitType, 1);
				}
			}
			rollResult.Average((float)num);
			rollResult.LogAverages();
			Debug.Log(string.Join(", ", from a in dictionary
			select a.Key.ToString() + "=" + a.Value.ToString()));
		}

		// Token: 0x06006872 RID: 26738 RVA: 0x00214540 File Offset: 0x00212740
		private CombatHitTester.RollResult GetRollResult()
		{
			WeaponFlankingBonusType weaponFlankingBonusType = WeaponFlankingBonusType.None;
			float num = 0f;
			float num2 = 1f;
			WeaponItem weaponItem;
			WeaponFlankingBonusType weaponFlankingBonusType2;
			int num3;
			if (this.m_considerWeaponFlankingBonus && this.m_mainHandWeapon != null && this.m_mainHandWeapon.TryGetAsType(out weaponItem) && weaponItem.FlankingBonus.TryGetWeaponFlankingBonus(this.m_flankingPosition, out weaponFlankingBonusType2, out num3, out num2))
			{
				weaponFlankingBonusType = weaponFlankingBonusType2;
				num = (float)num3 * 0.01f;
			}
			RoleFlankingBonusType roleFlankingBonusType = this.m_roleFlankingBonusType;
			float num4 = (this.m_roleFlankingBonusType != RoleFlankingBonusType.None) ? ((float)this.m_roleFlankingBonusValue * 0.01f) : 0f;
			float hitModifier = (float)this.m_instantVitals.Mods.HitModifier;
			float num5 = this.m_sourceHitBonus * 100f;
			float num6 = (hitModifier + num5) * 0.01f;
			if (weaponFlankingBonusType == WeaponFlankingBonusType.Hit)
			{
				num6 += num;
			}
			HitType hitTypeForRoll = HitTypeExtensions.GetHitTypeForRoll(SolMath.Gaussian() + 1f * num6);
			EffectApplicationFlags effectApplicationFlags = hitTypeForRoll.GetEffectApplicationFlags();
			float num7 = 0f;
			float num8 = this.m_sourcePenetrationBonus;
			if (weaponFlankingBonusType == WeaponFlankingBonusType.Penetration)
			{
				num8 += num;
			}
			EffectResourceType resourceType = this.m_instantVitals.ResourceType;
			if (resourceType != EffectResourceType.Armor)
			{
				if (resourceType != EffectResourceType.Threat)
				{
					float num9 = (float)this.m_targetArmorClass;
					float num10 = this.m_instantVitals.Mods.ArmorModifier + num8;
					if (num10 > 0f)
					{
						num10 = Mathf.Clamp01(num10);
						num9 = num9.PercentModification(-num10);
					}
					float maxReduction = 0.8f;
					switch (this.m_maxAbsorbDamageReductionType)
					{
					case CombatHitTester.TargetMaxAbsorb.Player:
						maxReduction = 0.5f;
						break;
					case CombatHitTester.TargetMaxAbsorb.Npc:
						maxReduction = 0.8f;
						break;
					case CombatHitTester.TargetMaxAbsorb.Custom:
						maxReduction = this.m_maxAbsorbDamageReduction;
						break;
					}
					num7 = SolMath.GetDamageReduction(num9, (float)this.m_sourceLevel, maxReduction);
				}
			}
			else
			{
				num7 = 1f;
			}
			WeaponItem weaponItem2;
			ArchetypeInstance archetypeInstance;
			DiceSet diceSet;
			if (!this.m_instantVitals.TryGetDiceSet(this, this.m_instantVitals.ApplyOffHandDamageMultiplier, out weaponItem2, out archetypeInstance, out diceSet))
			{
				Debug.LogWarning("No dice set!");
				return default(CombatHitTester.RollResult);
			}
			float num11 = this.m_sourceDamageBonus;
			if (weaponFlankingBonusType == WeaponFlankingBonusType.Damage)
			{
				num11 += num;
			}
			float valueMultiplier = this.m_instantVitals.Mods.ValueMultiplier;
			float num12 = (float)diceSet.RollDice();
			bool flag = this.m_sourceCombatFlags.HasBitFlag(CombatFlags.Advantage);
			bool flag2 = this.m_sourceCombatFlags.HasBitFlag(CombatFlags.Disadvantage);
			if ((flag || flag2) && (!flag || !flag2))
			{
				float b = (float)diceSet.RollDice();
				num12 = (flag ? Mathf.Max(num12, b) : Mathf.Min(num12, b));
			}
			if (hitTypeForRoll != HitType.Miss)
			{
				num12 += (float)this.m_instantVitals.Mods.ValueAdditive;
			}
			num12 *= valueMultiplier;
			num12 = num12.PercentModification(num11);
			num12 *= hitTypeForRoll.GetModifier();
			float targetResist = this.m_targetResist;
			num12 = num12.PercentModification(-1f * targetResist);
			float num13 = num12 * num7;
			float num14 = num12 - num13;
			if (roleFlankingBonusType == RoleFlankingBonusType.ArmorDamage)
			{
				num13 += num13 * num4;
			}
			float num15;
			if (this.m_instantVitals.ResourceType == EffectResourceType.Threat)
			{
				num15 = num14 * this.m_instantVitals.Mods.ThreatMultiplier;
				num14 = 0f;
				num13 = 0f;
			}
			else
			{
				float num16;
				float num17;
				effectApplicationFlags.GetThreatMultipliers(out num16, out num17);
				num15 = Mathf.Abs(num14 * num16) + Mathf.Abs(num13 * num17);
				num15 *= this.m_instantVitals.Mods.ThreatMultiplier;
				if (roleFlankingBonusType != RoleFlankingBonusType.IncreaseThreat)
				{
					if (roleFlankingBonusType == RoleFlankingBonusType.ReduceThreat)
					{
						num15 -= Mathf.Abs(num15) * num4;
					}
				}
				else
				{
					num15 += Mathf.Abs(num15) * num4;
				}
			}
			num14 *= -1f;
			return new CombatHitTester.RollResult
			{
				HitType = hitTypeForRoll,
				Adjustment = num14,
				DiceResult = num12,
				Absorbed = num13,
				Threat = num15
			};
		}

		// Token: 0x04005AA2 RID: 23202
		[SerializeField]
		private VitalsEffect_Instant m_instantVitals;

		// Token: 0x04005AA3 RID: 23203
		[SerializeField]
		private EquipableItem m_mainHandWeapon;

		// Token: 0x04005AA4 RID: 23204
		[SerializeField]
		private EquipableItem m_offHandWeapon;

		// Token: 0x04005AA5 RID: 23205
		[NonSerialized]
		private CachedHandHeldItem m_mainHandCache;

		// Token: 0x04005AA6 RID: 23206
		[NonSerialized]
		private CachedHandHeldItem m_offHandCache;

		// Token: 0x04005AA7 RID: 23207
		private const string kSourceGroup = "Source";

		// Token: 0x04005AA8 RID: 23208
		private const string kTargetGroup = "Target";

		// Token: 0x04005AA9 RID: 23209
		[Range(1f, 50f)]
		[SerializeField]
		private int m_sourceLevel = 1;

		// Token: 0x04005AAA RID: 23210
		[SerializeField]
		private CombatFlags m_sourceCombatFlags;

		// Token: 0x04005AAB RID: 23211
		[Range(0f, 1f)]
		[SerializeField]
		private float m_sourceHitBonus;

		// Token: 0x04005AAC RID: 23212
		[Range(0f, 1f)]
		[SerializeField]
		private float m_sourcePenetrationBonus;

		// Token: 0x04005AAD RID: 23213
		[Range(0f, 1f)]
		[SerializeField]
		private float m_sourceDamageBonus;

		// Token: 0x04005AAE RID: 23214
		private const string kSourceFlankingGroup = "Source/Flanking";

		// Token: 0x04005AAF RID: 23215
		[SerializeField]
		private FlankingPosition m_flankingPosition;

		// Token: 0x04005AB0 RID: 23216
		[SerializeField]
		private bool m_considerWeaponFlankingBonus;

		// Token: 0x04005AB1 RID: 23217
		[SerializeField]
		private RoleFlankingBonusType m_roleFlankingBonusType;

		// Token: 0x04005AB2 RID: 23218
		[SerializeField]
		private int m_roleFlankingBonusValue;

		// Token: 0x04005AB3 RID: 23219
		[Range(1f, 50f)]
		[SerializeField]
		private int m_targetLevel = 1;

		// Token: 0x04005AB4 RID: 23220
		[Range(0f, 2000f)]
		[SerializeField]
		private int m_targetArmorClass;

		// Token: 0x04005AB5 RID: 23221
		[SerializeField]
		private CombatHitTester.TargetMaxAbsorb m_maxAbsorbDamageReductionType = CombatHitTester.TargetMaxAbsorb.Npc;

		// Token: 0x04005AB6 RID: 23222
		[Range(0f, 1f)]
		[SerializeField]
		private float m_maxAbsorbDamageReduction = 0.8f;

		// Token: 0x04005AB7 RID: 23223
		[Range(0f, 1f)]
		[SerializeField]
		private float m_targetResist;

		// Token: 0x04005AB8 RID: 23224
		[SerializeField]
		private int m_rollCount = 1;

		// Token: 0x02000D94 RID: 3476
		private enum TargetMaxAbsorb
		{
			// Token: 0x04005ABA RID: 23226
			Player,
			// Token: 0x04005ABB RID: 23227
			Npc,
			// Token: 0x04005ABC RID: 23228
			Custom
		}

		// Token: 0x02000D95 RID: 3477
		private struct RollResult
		{
			// Token: 0x06006874 RID: 26740 RVA: 0x002148EC File Offset: 0x00212AEC
			public void LogResults()
			{
				Debug.Log(string.Concat(new string[]
				{
					this.HitType.ToString(),
					" for ",
					this.Adjustment.ToString(CultureInfo.InvariantCulture),
					" (",
					this.DiceResult.ToString(CultureInfo.InvariantCulture),
					" total, ",
					this.Absorbed.ToString(CultureInfo.InvariantCulture),
					" absorbed, ",
					this.Threat.ToString(CultureInfo.InvariantCulture),
					" threat)"
				}));
			}

			// Token: 0x06006875 RID: 26741 RVA: 0x00214994 File Offset: 0x00212B94
			public void LogAverages()
			{
				Debug.Log(string.Concat(new string[]
				{
					"AVG DMG=",
					this.Adjustment.ToString(CultureInfo.InvariantCulture),
					", AVG DICE=",
					this.DiceResult.ToString(CultureInfo.InvariantCulture),
					", AVG ABSORBED=",
					this.Absorbed.ToString(CultureInfo.InvariantCulture),
					", AVG THREAT=",
					this.Threat.ToString(CultureInfo.InvariantCulture)
				}));
			}

			// Token: 0x06006876 RID: 26742 RVA: 0x00214A20 File Offset: 0x00212C20
			public void AddResultFromAnother(CombatHitTester.RollResult other)
			{
				this.Adjustment += other.Adjustment;
				this.DiceResult += other.DiceResult;
				this.Absorbed += other.Absorbed;
				this.Threat += other.Threat;
			}

			// Token: 0x06006877 RID: 26743 RVA: 0x00086268 File Offset: 0x00084468
			public void Average(float count)
			{
				this.Adjustment /= count;
				this.DiceResult /= count;
				this.Absorbed /= count;
				this.Threat /= count;
			}

			// Token: 0x04005ABD RID: 23229
			public HitType HitType;

			// Token: 0x04005ABE RID: 23230
			public float Adjustment;

			// Token: 0x04005ABF RID: 23231
			public float DiceResult;

			// Token: 0x04005AC0 RID: 23232
			public float Absorbed;

			// Token: 0x04005AC1 RID: 23233
			public float Threat;
		}
	}
}
