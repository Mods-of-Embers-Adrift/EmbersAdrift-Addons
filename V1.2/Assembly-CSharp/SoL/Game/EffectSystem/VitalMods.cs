using System;
using Cysharp.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C19 RID: 3097
	[Serializable]
	public class VitalMods
	{
		// Token: 0x170016C2 RID: 5826
		// (get) Token: 0x06005F76 RID: 24438 RVA: 0x00080447 File Offset: 0x0007E647
		public CombatFlags CombatFlags
		{
			get
			{
				return this.m_combatFlags;
			}
		}

		// Token: 0x170016C3 RID: 5827
		// (get) Token: 0x06005F77 RID: 24439 RVA: 0x0008044F File Offset: 0x0007E64F
		public int ValueAdditive
		{
			get
			{
				return this.m_valueAdditive;
			}
		}

		// Token: 0x170016C4 RID: 5828
		// (get) Token: 0x06005F78 RID: 24440 RVA: 0x00080457 File Offset: 0x0007E657
		public float ValueMultiplier
		{
			get
			{
				return this.m_valueMultiplier;
			}
		}

		// Token: 0x170016C5 RID: 5829
		// (get) Token: 0x06005F79 RID: 24441 RVA: 0x0008045F File Offset: 0x0007E65F
		public float ThreatMultiplier
		{
			get
			{
				return this.m_threatMultiplier;
			}
		}

		// Token: 0x170016C6 RID: 5830
		// (get) Token: 0x06005F7A RID: 24442 RVA: 0x00080467 File Offset: 0x0007E667
		public int HitModifier
		{
			get
			{
				return this.m_hitModifier;
			}
		}

		// Token: 0x170016C7 RID: 5831
		// (get) Token: 0x06005F7B RID: 24443 RVA: 0x0008046F File Offset: 0x0007E66F
		public float ArmorModifier
		{
			get
			{
				return this.m_armorModifier;
			}
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x00080477 File Offset: 0x0007E677
		public VitalMods()
		{
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x001F9CA0 File Offset: 0x001F7EA0
		public VitalMods(VitalMods other)
		{
			if (other == null)
			{
				return;
			}
			this.m_combatFlags = other.m_combatFlags;
			this.m_valueAdditive = other.m_valueAdditive;
			this.m_valueMultiplier = other.m_valueMultiplier;
			this.m_threatMultiplier = other.m_threatMultiplier;
			this.m_hitModifier = other.m_hitModifier;
			this.m_armorModifier = other.m_armorModifier;
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x00080495 File Offset: 0x0007E695
		private string GetSign(float value)
		{
			if (value < 0f)
			{
				return "";
			}
			return "+";
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x000804AA File Offset: 0x0007E6AA
		private string GetSign(int value)
		{
			if (value < 0)
			{
				return "";
			}
			return "+";
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x001F9D18 File Offset: 0x001F7F18
		private string GetDescription(ReagentItem reagentItem)
		{
			string result = string.Empty;
			TooltipExtensions.ToCombine.Clear();
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (this.m_combatFlags.HasBitFlag(CombatFlags.IgnoreActiveDefenses))
				{
					TooltipExtensions.ToCombine.Add("Ignore Active DEF");
				}
				if (this.m_combatFlags.HasBitFlag(CombatFlags.Advantage))
				{
					TooltipExtensions.ToCombine.Add("ADVANTAGE".Color(UIManager.RequirementsMetColor));
				}
				else if (this.m_combatFlags.HasBitFlag(CombatFlags.Disadvantage))
				{
					TooltipExtensions.ToCombine.Add("DISADVANTAGE".Color(UIManager.RequirementsNotMetColor));
				}
				ReagentItem.ReagentInstantMods reagentInstantMods = (reagentItem != null) ? reagentItem.GetInstantMods() : null;
				if (this.m_valueAdditive != 0 || (reagentInstantMods != null && reagentInstantMods.ValueAdditive != 0))
				{
					int num = this.m_valueAdditive;
					bool flag = reagentInstantMods != null && reagentInstantMods.ValueAdditive != 0;
					if (flag)
					{
						num += reagentInstantMods.ValueAdditive;
					}
					string text = this.GetSign(num) + num.ToString();
					if (flag && reagentInstantMods.ValueAdditive != 0)
					{
						text = text.Color(UIManager.ReagentBonusColor);
					}
					TooltipExtensions.ToCombine.Add(text);
				}
				if (this.m_valueMultiplier != 1f || (reagentInstantMods != null && reagentInstantMods.ValueMultiplier != 0f))
				{
					float num2 = this.m_valueMultiplier;
					bool flag2 = reagentInstantMods != null && reagentInstantMods.ValueMultiplier != 0f;
					if (flag2)
					{
						num2 += reagentInstantMods.ValueMultiplier;
					}
					string text2 = string.Format("x{0}{1:F1}", this.GetSign(num2), num2);
					if (flag2 && reagentInstantMods.ValueMultiplier != 0f)
					{
						text2 = text2.Color(UIManager.ReagentBonusColor);
					}
					TooltipExtensions.ToCombine.Add(text2);
				}
				if (this.m_threatMultiplier != 1f || (reagentInstantMods != null && reagentInstantMods.ThreatMultiplier != 0f))
				{
					float num3 = this.m_threatMultiplier;
					bool flag3 = reagentInstantMods != null && reagentInstantMods.ThreatMultiplier != 0f;
					if (flag3)
					{
						num3 += reagentInstantMods.ThreatMultiplier;
					}
					string text3 = string.Format("x{0}{1:F1}", this.GetSign(num3), num3);
					if (flag3 && reagentInstantMods.ThreatMultiplier != 0f)
					{
						text3 = text3.Color(UIManager.ReagentBonusColor);
					}
					TooltipExtensions.ToCombine.Add(text3 + " THREAT");
				}
				if (this.m_hitModifier != 0)
				{
					TooltipExtensions.ToCombine.Add(this.GetSign(this.m_hitModifier) + this.m_hitModifier.ToString() + " HIT");
				}
				if (this.m_armorModifier != 0f)
				{
					TooltipExtensions.ToCombine.Add(this.GetSign(this.m_armorModifier) + Mathf.FloorToInt(this.m_armorModifier * 100f).ToString() + "% ARMOR IGNORE");
				}
				utf16ValueStringBuilder.AppendJoin<string>(',', TooltipExtensions.ToCombine);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005F81 RID: 24449 RVA: 0x001FA014 File Offset: 0x001F8214
		public void FillTooltip(TooltipTextBlock block, ReagentItem reagentItem = null)
		{
			bool flag = this.m_combatFlags != CombatFlags.None || this.m_valueAdditive != 0 || this.m_valueMultiplier != 1f || this.m_threatMultiplier != 1f || this.m_hitModifier != 0 || this.m_threatMultiplier != 0f;
			bool flag2 = reagentItem != null && reagentItem.ColorInstantMods;
			if (flag || flag2)
			{
				string description = this.GetDescription(reagentItem);
				if (!string.IsNullOrEmpty(description))
				{
					block.AppendLine("<size=80%>MODS: <i>" + description + "</i></size>", 0);
				}
			}
		}

		// Token: 0x06005F82 RID: 24450 RVA: 0x001FA0A4 File Offset: 0x001F82A4
		public void FillCombatFlagsLine(TooltipTextBlock block)
		{
			if (block == null || this.m_combatFlags == CombatFlags.None)
			{
				return;
			}
			TooltipExtensions.ToCombine.Clear();
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				bool tooltipShowMore = UIManager.TooltipShowMore;
				if (this.m_combatFlags.HasBitFlag(CombatFlags.IgnoreActiveDefenses))
				{
					if (tooltipShowMore)
					{
						utf16ValueStringBuilder.AppendLine(CombatFlags.IgnoreActiveDefenses.GetLongTooltipDescription());
					}
					else
					{
						TooltipExtensions.ToCombine.Add(CombatFlags.IgnoreActiveDefenses.GetShortTooltipDescription());
					}
				}
				if (this.m_combatFlags.HasBitFlag(CombatFlags.Advantage))
				{
					if (tooltipShowMore)
					{
						utf16ValueStringBuilder.AppendLine(CombatFlags.Advantage.GetLongTooltipDescription());
					}
					else
					{
						TooltipExtensions.ToCombine.Add(CombatFlags.Advantage.GetShortTooltipDescription());
					}
				}
				else if (this.m_combatFlags.HasBitFlag(CombatFlags.Disadvantage))
				{
					if (tooltipShowMore)
					{
						utf16ValueStringBuilder.AppendLine(CombatFlags.Disadvantage.GetLongTooltipDescription());
					}
					else
					{
						TooltipExtensions.ToCombine.Add(CombatFlags.Disadvantage.GetShortTooltipDescription());
					}
				}
				if (!tooltipShowMore)
				{
					utf16ValueStringBuilder.AppendLine(string.Join(", ", TooltipExtensions.ToCombine));
				}
				block.Append(utf16ValueStringBuilder.ToString(), 0);
			}
		}

		// Token: 0x06005F83 RID: 24451 RVA: 0x001FA1B8 File Offset: 0x001F83B8
		public string GetModifiersLine(ReagentItem reagentItem = null)
		{
			string text;
			bool flag = this.TryGetValueModLines(out text, reagentItem);
			string text2;
			bool flag2 = this.TryGetAlwaysShowModifierLine(out text2, reagentItem);
			if (flag && flag2)
			{
				return ZString.Format<string, string>("{0}, {1}", text, text2);
			}
			if (flag)
			{
				return text;
			}
			if (flag2)
			{
				return text2;
			}
			return string.Empty;
		}

		// Token: 0x06005F84 RID: 24452 RVA: 0x001FA1FC File Offset: 0x001F83FC
		public bool TryGetValueModLines(out string txt, ReagentItem reagentItem = null)
		{
			txt = string.Empty;
			bool flag = this.m_valueAdditive != 0 || this.m_valueMultiplier != 1f;
			bool flag2 = reagentItem != null && reagentItem.ColorInstantMods;
			if (flag || flag2)
			{
				TooltipExtensions.ToCombine.Clear();
				ReagentItem.ReagentInstantMods reagentInstantMods = (reagentItem != null) ? reagentItem.GetInstantMods() : null;
				if (this.m_valueAdditive != 0 || (reagentInstantMods != null && reagentInstantMods.ValueAdditive != 0))
				{
					int num = this.m_valueAdditive;
					bool flag3 = reagentInstantMods != null && reagentInstantMods.ValueAdditive != 0;
					if (flag3)
					{
						num += reagentInstantMods.ValueAdditive;
					}
					string text = this.GetSign(num) + num.ToString();
					if (flag3 && reagentInstantMods.ValueAdditive != 0)
					{
						text = text.Color(UIManager.ReagentBonusColor);
					}
					TooltipExtensions.ToCombine.Add(text);
				}
				if (this.m_valueMultiplier != 1f || (reagentInstantMods != null && reagentInstantMods.ValueMultiplier != 0f))
				{
					float num2 = this.m_valueMultiplier;
					bool flag4 = reagentInstantMods != null && reagentInstantMods.ValueMultiplier != 0f;
					if (flag4)
					{
						num2 += reagentInstantMods.ValueMultiplier;
					}
					string text2 = this.GetMultiplierDisplayValue(num2);
					if (flag4 && reagentInstantMods.ValueMultiplier != 0f)
					{
						text2 = text2.Color(UIManager.ReagentBonusColor);
					}
					string arg = "Dmg";
					TooltipExtensions.ToCombine.Add(ZString.Format<string, string>("{0} {1}", text2, arg));
				}
				txt = string.Join(", ", TooltipExtensions.ToCombine);
				return true;
			}
			return false;
		}

		// Token: 0x06005F85 RID: 24453 RVA: 0x001FA370 File Offset: 0x001F8570
		public bool TryGetAlwaysShowModifierLine(out string txt, ReagentItem reagentItem = null)
		{
			txt = string.Empty;
			bool flag = this.m_threatMultiplier != 1f || this.m_hitModifier != 0 || this.m_armorModifier != 0f;
			bool flag2 = this.m_combatFlags > CombatFlags.None;
			bool flag3 = reagentItem != null && reagentItem.ColorInstantMods;
			if (flag || flag3 || flag2)
			{
				TooltipExtensions.ToCombine.Clear();
				if (this.m_combatFlags.HasBitFlag(CombatFlags.IgnoreActiveDefenses))
				{
					string item = UIManager.TooltipShowMore ? "+Ignore Active Defenses" : ZString.Format<string>("+{0}", CombatFlags.IgnoreActiveDefenses.GetShortTooltipDescription());
					TooltipExtensions.ToCombine.Add(item);
				}
				if (this.m_combatFlags.HasBitFlag(CombatFlags.Advantage))
				{
					string item2 = UIManager.TooltipShowMore ? ZString.Format<string>("+{0}", CombatFlags.Advantage.ToString()) : ZString.Format<string>("+{0}", CombatFlags.Advantage.GetShortTooltipDescription());
					TooltipExtensions.ToCombine.Add(item2);
				}
				else if (this.m_combatFlags.HasBitFlag(CombatFlags.Disadvantage))
				{
					string item3 = UIManager.TooltipShowMore ? ZString.Format<string>("+{0}", CombatFlags.Disadvantage.ToString()) : ZString.Format<string>("+{0}", CombatFlags.Disadvantage.GetShortTooltipDescription());
					TooltipExtensions.ToCombine.Add(item3);
				}
				ReagentItem.ReagentInstantMods reagentInstantMods = (reagentItem != null) ? reagentItem.GetInstantMods() : null;
				if (this.m_threatMultiplier != 1f || (reagentInstantMods != null && reagentInstantMods.ThreatMultiplier != 0f))
				{
					float num = this.m_threatMultiplier;
					bool flag4 = reagentInstantMods != null && reagentInstantMods.ThreatMultiplier != 0f;
					if (flag4)
					{
						num += reagentInstantMods.ThreatMultiplier;
					}
					string text = this.GetMultiplierDisplayValue(num);
					if (flag4 && reagentInstantMods.ThreatMultiplier != 0f)
					{
						text = text.Color(UIManager.ReagentBonusColor);
					}
					string threatDescription = TooltipExtensions.GetThreatDescription();
					TooltipExtensions.ToCombine.Add(ZString.Format<string, string>("{0} {1}", text, threatDescription));
				}
				if (this.m_hitModifier != 0)
				{
					TooltipExtensions.ToCombine.Add(ZString.Format<string, int>("{0}{1} Hit", this.GetSign(this.m_hitModifier), this.m_hitModifier));
				}
				if (this.m_armorModifier != 0f)
				{
					string arg = UIManager.TooltipShowMore ? "Armor Ignore" : "AI";
					TooltipExtensions.ToCombine.Add(ZString.Format<int, string>("{0}% {1}", Mathf.FloorToInt(this.m_armorModifier * 100f), arg));
				}
				txt = string.Join(", ", TooltipExtensions.ToCombine);
				return true;
			}
			return false;
		}

		// Token: 0x06005F86 RID: 24454 RVA: 0x001FA5E4 File Offset: 0x001F87E4
		private string GetMultiplierDisplayValue(float value)
		{
			int arg = Mathf.FloorToInt(value * 100f);
			return ZString.Format<int>("{0}%", arg);
		}

		// Token: 0x06005F87 RID: 24455 RVA: 0x000804BB File Offset: 0x0007E6BB
		public bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.HitModifier <= 1;
		}

		// Token: 0x06005F88 RID: 24456 RVA: 0x000804C7 File Offset: 0x0007E6C7
		public bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.HitModifier)
			{
				this.m_hitModifier = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_hitModifier);
				return true;
			}
			if (assignerName != ComponentEffectAssignerName.ArmorModifier)
			{
				return false;
			}
			this.m_armorModifier = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_armorModifier);
			return true;
		}

		// Token: 0x04005273 RID: 21107
		[SerializeField]
		protected CombatFlags m_combatFlags;

		// Token: 0x04005274 RID: 21108
		[SerializeField]
		protected int m_valueAdditive;

		// Token: 0x04005275 RID: 21109
		[SerializeField]
		protected float m_valueMultiplier = 1f;

		// Token: 0x04005276 RID: 21110
		[SerializeField]
		protected float m_threatMultiplier = 1f;

		// Token: 0x04005277 RID: 21111
		[SerializeField]
		protected int m_hitModifier;

		// Token: 0x04005278 RID: 21112
		[SerializeField]
		protected float m_armorModifier;
	}
}
