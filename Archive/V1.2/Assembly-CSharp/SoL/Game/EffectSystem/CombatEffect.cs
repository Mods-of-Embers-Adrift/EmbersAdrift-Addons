using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C00 RID: 3072
	[Serializable]
	public class CombatEffect
	{
		// Token: 0x06005E9F RID: 24223 RVA: 0x0007FA09 File Offset: 0x0007DC09
		private Color GetPolarityColor()
		{
			if (this.m_polarity != Polarity.Positive)
			{
				return Color.red;
			}
			return Color.green;
		}

		// Token: 0x17001663 RID: 5731
		// (get) Token: 0x06005EA0 RID: 24224 RVA: 0x0007FA1F File Offset: 0x0007DC1F
		private bool m_showLasting
		{
			get
			{
				return this.Effects != null && this.Effects.IsTimeBased;
			}
		}

		// Token: 0x17001664 RID: 5732
		// (get) Token: 0x06005EA1 RID: 24225 RVA: 0x0007FA36 File Offset: 0x0007DC36
		private bool m_showTrigger
		{
			get
			{
				return this.Effects != null && (this.Effects.HasTriggerEffects || (this.m_expiration != null && this.m_expiration.HasTrigger));
			}
		}

		// Token: 0x17001665 RID: 5733
		// (get) Token: 0x06005EA2 RID: 24226 RVA: 0x0007FA66 File Offset: 0x0007DC66
		private bool m_hideEffects
		{
			get
			{
				return this.m_effectsOverride != null;
			}
		}

		// Token: 0x06005EA3 RID: 24227 RVA: 0x0007FA74 File Offset: 0x0007DC74
		private IEnumerable GetEffectOverrides()
		{
			return SolOdinUtilities.GetDropdownItems<ScriptableEffects>();
		}

		// Token: 0x06005EA4 RID: 24228 RVA: 0x0007FA7B File Offset: 0x0007DC7B
		public bool TryGetAdditionalThreat(EffectApplicationFlags flags, out ThreatParams threatParams)
		{
			threatParams = null;
			if (this.m_threat.ShouldApply(flags))
			{
				threatParams = this.m_threat;
			}
			return threatParams != null;
		}

		// Token: 0x06005EA5 RID: 24229 RVA: 0x0007FA9B File Offset: 0x0007DC9B
		public virtual bool TryGetSecondary(EffectApplicationFlags flags, out TargetingParams targeting, out CombatEffect secondaryEffect)
		{
			targeting = null;
			secondaryEffect = null;
			return false;
		}

		// Token: 0x17001666 RID: 5734
		// (get) Token: 0x06005EA6 RID: 24230 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool HasSecondary
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001667 RID: 5735
		// (get) Token: 0x06005EA7 RID: 24231 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual CombatEffect SecondaryCombatEffect
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001668 RID: 5736
		// (get) Token: 0x06005EA8 RID: 24232 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual TargetingParams SecondaryTargetingParams
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001669 RID: 5737
		// (get) Token: 0x06005EA9 RID: 24233 RVA: 0x0007FAA4 File Offset: 0x0007DCA4
		public Polarity Polarity
		{
			get
			{
				return this.m_polarity;
			}
		}

		// Token: 0x1700166A RID: 5738
		// (get) Token: 0x06005EAA RID: 24234 RVA: 0x0007FAAC File Offset: 0x0007DCAC
		public ExpirationParams Expiration
		{
			get
			{
				return this.m_expiration;
			}
		}

		// Token: 0x1700166B RID: 5739
		// (get) Token: 0x06005EAB RID: 24235 RVA: 0x0007FAB4 File Offset: 0x0007DCB4
		public Effects Effects
		{
			get
			{
				if (!(this.m_effectsOverride == null))
				{
					return this.m_effectsOverride.Effects;
				}
				return this.m_effects;
			}
		}

		// Token: 0x1700166C RID: 5740
		// (get) Token: 0x06005EAC RID: 24236 RVA: 0x0007FAD6 File Offset: 0x0007DCD6
		public bool IsTriggerBased
		{
			get
			{
				return this.m_showTrigger;
			}
		}

		// Token: 0x1700166D RID: 5741
		// (get) Token: 0x06005EAD RID: 24237 RVA: 0x0007FADE File Offset: 0x0007DCDE
		public TriggeredParams TriggerParams
		{
			get
			{
				return this.m_trigger;
			}
		}

		// Token: 0x1700166E RID: 5742
		// (get) Token: 0x06005EAE RID: 24238 RVA: 0x0007FAE6 File Offset: 0x0007DCE6
		public bool IsLasting
		{
			get
			{
				return this.m_showLasting;
			}
		}

		// Token: 0x1700166F RID: 5743
		// (get) Token: 0x06005EAF RID: 24239 RVA: 0x0007FAEE File Offset: 0x0007DCEE
		public bool PreserveOnUnconscious
		{
			get
			{
				return this.m_preserveOnUnconscious;
			}
		}

		// Token: 0x17001670 RID: 5744
		// (get) Token: 0x06005EB0 RID: 24240 RVA: 0x0007FAF6 File Offset: 0x0007DCF6
		public bool AllowSourceOverride
		{
			get
			{
				return this.m_allowSourceOverride;
			}
		}

		// Token: 0x17001671 RID: 5745
		// (get) Token: 0x06005EB1 RID: 24241 RVA: 0x0007FAFE File Offset: 0x0007DCFE
		public bool IsAutoAttack
		{
			get
			{
				return this.m_isAutoAttack;
			}
		}

		// Token: 0x17001672 RID: 5746
		// (get) Token: 0x06005EB2 RID: 24242 RVA: 0x0007FB06 File Offset: 0x0007DD06
		public ThreatParams Threat
		{
			get
			{
				return this.m_threat;
			}
		}

		// Token: 0x17001673 RID: 5747
		// (get) Token: 0x06005EB3 RID: 24243 RVA: 0x0007FB0E File Offset: 0x0007DD0E
		public EffectCategoryKey CategoryKey
		{
			get
			{
				if (this.m_categoryKey == null)
				{
					this.m_categoryKey = new EffectCategoryKey?(new EffectCategoryKey(this.m_polarity, this.m_category));
				}
				return this.m_categoryKey.Value;
			}
		}

		// Token: 0x17001674 RID: 5748
		// (get) Token: 0x06005EB4 RID: 24244 RVA: 0x0007FB44 File Offset: 0x0007DD44
		private bool m_showThreatScalingFraction
		{
			get
			{
				return this.m_threatScalingType > CombatEffect.ThreatScalingTypes.None;
			}
		}

		// Token: 0x17001675 RID: 5749
		// (get) Token: 0x06005EB5 RID: 24245 RVA: 0x0007FB4F File Offset: 0x0007DD4F
		internal CombatEffect.ThreatScalingTypes ThreatScalingType
		{
			get
			{
				return this.m_threatScalingType;
			}
		}

		// Token: 0x17001676 RID: 5750
		// (get) Token: 0x06005EB6 RID: 24246 RVA: 0x0007FB57 File Offset: 0x0007DD57
		internal float ThreatScalingFraction
		{
			get
			{
				return this.m_threatScalingFraction;
			}
		}

		// Token: 0x17001677 RID: 5751
		// (get) Token: 0x06005EB7 RID: 24247 RVA: 0x0007FB5F File Offset: 0x0007DD5F
		private bool m_showOverTimeDetails
		{
			get
			{
				return !this.m_hideEffects && this.m_effects.HasOverTimeVitals;
			}
		}

		// Token: 0x06005EB8 RID: 24248 RVA: 0x001F70EC File Offset: 0x001F52EC
		private string GetOverTimeDetailsLeft()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				string overTimeDetails = this.GetOverTimeDetails(0, 0, 0);
				utf16ValueStringBuilder.Append(overTimeDetails);
				if (this.m_polarity == Polarity.Negative)
				{
					for (int i = 1; i < 5; i++)
					{
						float npcHigherLevelResist = GlobalSettings.Values.Combat.GetNpcHigherLevelResist(i);
						int durationModifier = -1 * Mathf.FloorToInt((float)this.m_expiration.MaxDuration * npcHigherLevelResist);
						utf16ValueStringBuilder.AppendLine("\n");
						utf16ValueStringBuilder.AppendFormat<int, int>("* Diminished against +{0} lvl (-{1}%)\n", i, Mathf.FloorToInt(npcHigherLevelResist * 100f));
						utf16ValueStringBuilder.Append(this.GetOverTimeDetails(0, 0, durationModifier));
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005EB9 RID: 24249 RVA: 0x00045BC3 File Offset: 0x00043DC3
		public string GetOverTimeDetails(int tickRateModifier, int valueModifier, int durationModifier)
		{
			return string.Empty;
		}

		// Token: 0x06005EBA RID: 24250 RVA: 0x0007FB76 File Offset: 0x0007DD76
		private string GetOverTimeDetailsRight()
		{
			return "Over Time Vital Details";
		}

		// Token: 0x06005EBB RID: 24251 RVA: 0x001F71BC File Offset: 0x001F53BC
		public StatType GetResistChannel()
		{
			StatType result = StatType.None;
			if (this.m_showResistChannel)
			{
				result = this.m_resistChannel;
			}
			return result;
		}

		// Token: 0x17001678 RID: 5752
		// (get) Token: 0x06005EBC RID: 24252 RVA: 0x0007FB7D File Offset: 0x0007DD7D
		private StatType[] m_validResistChannels
		{
			get
			{
				return StatTypeExtensions.ValidDebuffTypes;
			}
		}

		// Token: 0x17001679 RID: 5753
		// (get) Token: 0x06005EBD RID: 24253 RVA: 0x0007FB84 File Offset: 0x0007DD84
		private bool m_showResistChannel
		{
			get
			{
				return this.m_polarity == Polarity.Negative && this.m_effects != null && this.m_effects.HasLasting && (this.m_effects.HasStatusEffects || !this.m_effects.HasBehaviorEffects);
			}
		}

		// Token: 0x06005EBE RID: 24254 RVA: 0x001F71DC File Offset: 0x001F53DC
		public void ApplyStatusEffects(bool adding, GameEntity targetEntity, ReagentItem reagentItem, byte? stackCount)
		{
			if (!this.m_effects.HasStatusEffects || !targetEntity || !targetEntity.Vitals)
			{
				return;
			}
			StatusEffect statusEffect = this.m_effects.StatusEffect;
			int num = (reagentItem != null) ? reagentItem.GetStatusEffectMod() : 0;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < statusEffect.Values.Length; i++)
			{
				if (!statusEffect.Values[i].Type.IsInvalid())
				{
					int num2 = statusEffect.Values[i].Value;
					if (stackCount != null)
					{
						num2 *= (int)stackCount.Value;
					}
					int num3 = num2 + num;
					if (this.Polarity == Polarity.Negative)
					{
						num3 *= -1;
					}
					if (statusEffect.Values[i].Type == StatType.Movement)
					{
						flag = true;
						flag2 = (flag2 || num3 <= -100);
					}
					if (!adding)
					{
						num3 *= -1;
					}
					this.ModifyStatusEffectVital(statusEffect.Values[i].Type, num3, targetEntity);
				}
			}
			if (flag && targetEntity.Motor != null)
			{
				targetEntity.Motor.SpeedModifier = (float)targetEntity.Vitals.GetStatusEffectValue(StatType.Movement) * 0.01f;
				if (flag2)
				{
					targetEntity.Motor.ApplyRootEffect(adding, this);
				}
			}
		}

		// Token: 0x06005EBF RID: 24255 RVA: 0x0007FBC2 File Offset: 0x0007DDC2
		private void ModifyStatusEffectVital(StatType statType, int value, GameEntity targetEntity)
		{
			targetEntity.Vitals.ModifyStatusEffectValue(statType, value);
		}

		// Token: 0x06005EC0 RID: 24256 RVA: 0x0007FBD1 File Offset: 0x0007DDD1
		public bool AllowStacking(out int stackLimit)
		{
			stackLimit = ((this.m_category != null) ? this.m_category.StackLimit : 1);
			return stackLimit > 1;
		}

		// Token: 0x040051CB RID: 20939
		public const string kCombatEffectGroupName = "Combat Effect";

		// Token: 0x040051CC RID: 20940
		protected const string kParametersGroupName = "Parameters";

		// Token: 0x040051CD RID: 20941
		protected const string kEffectsGroupName = "Effects";

		// Token: 0x040051CE RID: 20942
		private const string kLastingTitle = "Lasting Parameters";

		// Token: 0x040051CF RID: 20943
		private const string kLastingBase = "m_showLasting";

		// Token: 0x040051D0 RID: 20944
		private const string kLastingGroupName = "m_showLasting/Lasting";

		// Token: 0x040051D1 RID: 20945
		private const string kResistGroupName = "m_showLasting/Lasting/Resist Channel";

		// Token: 0x040051D2 RID: 20946
		private const string kChannelGroupName = "m_showLasting/Lasting/Stacking Channel";

		// Token: 0x040051D3 RID: 20947
		private const string kTriggerBase = "m_showLasting/Lasting/m_showTrigger";

		// Token: 0x040051D4 RID: 20948
		private const string kTriggerGroupName = "m_showLasting/Lasting/m_showTrigger/Trigger";

		// Token: 0x040051D5 RID: 20949
		private EffectCategoryKey? m_categoryKey;

		// Token: 0x040051D6 RID: 20950
		[SerializeField]
		private Polarity m_polarity;

		// Token: 0x040051D7 RID: 20951
		[SerializeField]
		private bool m_isAutoAttack;

		// Token: 0x040051D8 RID: 20952
		[SerializeField]
		private bool m_preserveOnUnconscious;

		// Token: 0x040051D9 RID: 20953
		[Tooltip("If a SourceOverride is present then allow the EffectApplicator to replace the source with it. In other words: use the original source for bonuses, threat, etc.")]
		[SerializeField]
		private bool m_allowSourceOverride;

		// Token: 0x040051DA RID: 20954
		[SerializeField]
		private CombatEffect.ThreatScalingTypes m_threatScalingType;

		// Token: 0x040051DB RID: 20955
		[Range(0f, 1f)]
		[SerializeField]
		private float m_threatScalingFraction;

		// Token: 0x040051DC RID: 20956
		[SerializeField]
		private ThreatParams m_threat;

		// Token: 0x040051DD RID: 20957
		[SerializeField]
		private ScriptableEffects m_effectsOverride;

		// Token: 0x040051DE RID: 20958
		[SerializeField]
		private Effects m_effects;

		// Token: 0x040051DF RID: 20959
		[SerializeField]
		private DummyClass m_scalingDummy;

		// Token: 0x040051E0 RID: 20960
		[SerializeField]
		private ExpirationParams m_expiration;

		// Token: 0x040051E1 RID: 20961
		[SerializeField]
		private StatType m_resistChannel;

		// Token: 0x040051E2 RID: 20962
		[SerializeField]
		private EffectCategory m_category;

		// Token: 0x040051E3 RID: 20963
		[SerializeField]
		private TriggeredParams m_trigger;

		// Token: 0x02000C01 RID: 3073
		internal enum ThreatScalingTypes
		{
			// Token: 0x040051E5 RID: 20965
			None,
			// Token: 0x040051E6 RID: 20966
			Increase,
			// Token: 0x040051E7 RID: 20967
			Decrease
		}
	}
}
