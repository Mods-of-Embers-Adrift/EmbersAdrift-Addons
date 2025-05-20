using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using TMPro;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200033C RID: 828
	public static class TextMeshProExtensions
	{
		// Token: 0x060016AC RID: 5804 RVA: 0x00100DA4 File Offset: 0x000FEFA4
		public static void SetTextColor(this TextMeshProUGUI tmp, string hex)
		{
			if (!hex.StartsWith("#"))
			{
				hex = "#" + hex;
			}
			Color color;
			if (ColorUtility.TryParseHtmlString(hex, out color))
			{
				tmp.SetTextColor(color);
			}
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00051DEB File Offset: 0x0004FFEB
		public static void SetTextColor(this TextMeshProUGUI tmp, uint hexLiteral)
		{
			tmp.SetTextColor(ColorExtensions.FromHexLiteral(hexLiteral));
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x00051DF9 File Offset: 0x0004FFF9
		public static void SetTextColor(this TextMeshProUGUI tmp, Color color)
		{
			tmp.color = color;
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x00051E02 File Offset: 0x00050002
		public static void SetTextWithReplacements(this TextMeshProUGUI tmp, string text)
		{
			tmp.ZStringSetText(text.ReplaceActionMappings());
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x00051E10 File Offset: 0x00050010
		public static string CreatePlayerLink(string playerName)
		{
			return ZString.Format<string, string>("<link=\"{0}\">{1}</link>", "playerName", playerName);
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x00100DDC File Offset: 0x000FEFDC
		public static string CreateArchetypeLink(BaseArchetype archetype)
		{
			Color color;
			if (!archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
			{
				return ZString.Format<string, string, string>("<link=\"{0}:{1}\">[{2}]</link>", "archetypeId", archetype.Id.Value, archetype.DisplayName);
			}
			return ZString.Format<string, string, string, string>("<link=\"{0}:{1}\"><color={2}>[{3}]</color></link>", "archetypeId", archetype.Id.Value, color.ToHex(), archetype.DisplayName);
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x00100E44 File Offset: 0x000FF044
		public static string CreateInstanceLink(ArchetypeInstance instance)
		{
			if (instance == null || !instance.Archetype)
			{
				return string.Empty;
			}
			Color color;
			if (!instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
			{
				return ZString.Format<string, string, string>("<link=\"{0}:{1}\">[{2}]</link>", "instanceId", instance.InstanceId.Value, instance.Archetype.GetModifiedDisplayName(instance));
			}
			return ZString.Format<string, string, string, string>("<link=\"{0}:{1}\"><color={2}>[{3}]</color></link>", "instanceId", instance.InstanceId.Value, color.ToHex(), instance.Archetype.GetModifiedDisplayName(instance));
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x00051E22 File Offset: 0x00050022
		public static string CreateTextTooltipLink(string tooltipText, string displayValue)
		{
			return ZString.Format<string, string, string>("<link=\"{0}:{1}\">{2}</link>", "text", tooltipText, displayValue);
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x00100ECC File Offset: 0x000FF0CC
		public static string CreateLongTextTooltipLink(string tooltipText, string displayValue, int? manualId = null)
		{
			int num = (manualId != null) ? manualId.Value : tooltipText.GetHashCode();
			TextMeshProExtensions.LongTooltips.AddOrReplace(num, tooltipText);
			return ZString.Format<string, int, string>("<link=\"{0}:{1}\">{2}</link>", "longtext", num, displayValue);
		}

		// Token: 0x04001E60 RID: 7776
		public static readonly Dictionary<int, string> LongTooltips = new Dictionary<int, string>(100);

		// Token: 0x04001E61 RID: 7777
		private const string kIconAsset = "SolIcons";

		// Token: 0x04001E62 RID: 7778
		public const string kFontAwesomeAsset = "<font=\"Font Awesome 5 Free-Solid-900 SDF\">";

		// Token: 0x04001E63 RID: 7779
		private const string kFontAwesomeRegularAsset = "<font=\"Font Awesome 5 Free-Regular-400 SDF\">";

		// Token: 0x04001E64 RID: 7780
		public const string kFontEnd = "</font>";

		// Token: 0x04001E65 RID: 7781
		public const string kSunIconUnicode = "";

		// Token: 0x04001E66 RID: 7782
		public const string kMoonIconUnicode = "";

		// Token: 0x04001E67 RID: 7783
		public const string kNoSubscriptionUnicode = "";

		// Token: 0x04001E68 RID: 7784
		public const string kActiveSubcriptionUnicode = "";

		// Token: 0x04001E69 RID: 7785
		public const string kHourglassStart = "";

		// Token: 0x04001E6A RID: 7786
		public const string kHourglassHalf = "";

		// Token: 0x04001E6B RID: 7787
		public const string kHourglassEnd = "";

		// Token: 0x04001E6C RID: 7788
		public const string Bullet = "<sprite=\"SolIcons\" name=\"Circle\" tint=1>";

		// Token: 0x04001E6D RID: 7789
		public const string Dice = "<sprite=\"SolIcons\" name=\"NeedIcon\" tint=1>";

		// Token: 0x04001E6E RID: 7790
		public const string Coin = "<sprite=\"SolIcons\" name=\"GreedIcon\" tint=1>";

		// Token: 0x04001E6F RID: 7791
		public const string Pass = "<sprite=\"SolIcons\" name=\"PassIcon\" tint=1>";

		// Token: 0x04001E70 RID: 7792
		public const string Swords = "<sprite=\"SolIcons\" name=\"Swords\" tint=1>";

		// Token: 0x04001E71 RID: 7793
		public const string FunctionSymbol = "<sprite=\"SolIcons\" name=\"FunctionSymbol\" tint=1>";

		// Token: 0x04001E72 RID: 7794
		public const string kChevronSingle = "<sprite=\"SolIcons\" name=\"Chevron_Single\" tint=1>";

		// Token: 0x04001E73 RID: 7795
		public const string kChevronDouble = "<sprite=\"SolIcons\" name=\"Chevron_Double\" tint=1>";

		// Token: 0x04001E74 RID: 7796
		public const string kChevronTriple = "<sprite=\"SolIcons\" name=\"Chevron_Triple\" tint=1>";

		// Token: 0x04001E75 RID: 7797
		public const string Degrees = "°";

		// Token: 0x04001E76 RID: 7798
		public const string TrashCan = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E77 RID: 7799
		public const string ChevronLeft = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E78 RID: 7800
		public const string ChevronRight = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E79 RID: 7801
		public const string ChevronDown = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E7A RID: 7802
		public const string CaretDown = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E7B RID: 7803
		public const string SquareCaretDown = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E7C RID: 7804
		public const string ArrowDownWideShort = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E7D RID: 7805
		public const string User = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E7E RID: 7806
		public const string Ban = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E7F RID: 7807
		public const string Trademark = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E80 RID: 7808
		public const string Edit = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E81 RID: 7809
		public const string kInfinityIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E82 RID: 7810
		public const string kCheckboxEmpty = "<font=\"Font Awesome 5 Free-Regular-400 SDF\"></font>";

		// Token: 0x04001E83 RID: 7811
		public const string kCheckboxChecked = "<font=\"Font Awesome 5 Free-Regular-400 SDF\"></font>";

		// Token: 0x04001E84 RID: 7812
		public const string kSolidCheckboxChecked = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E85 RID: 7813
		public const string kTrophyIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E86 RID: 7814
		public const string kCertificateIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E87 RID: 7815
		public const string kSunIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E88 RID: 7816
		public const string kMoonIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E89 RID: 7817
		public const string kInfoIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E8A RID: 7818
		public const string kUserIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E8B RID: 7819
		public const string kUsersIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E8C RID: 7820
		public const string kLockIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E8D RID: 7821
		public const string kUnlockIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E8E RID: 7822
		public const string kRetweetIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E8F RID: 7823
		public const string ArrowUp = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E90 RID: 7824
		public const string ArrowDown = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E91 RID: 7825
		public const string ArrowLeft = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E92 RID: 7826
		public const string ArrowRight = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E93 RID: 7827
		public const string ArrowFull = "<sprite=\"SolIcons\" name=\"ArrowFull\" tint=1>";

		// Token: 0x04001E94 RID: 7828
		public const string ArrowHalf = "<sprite=\"SolIcons\" name=\"ArrowHalf\" tint=1>";

		// Token: 0x04001E95 RID: 7829
		public const string ArrowEmpty = "<sprite=\"SolIcons\" name=\"ArrowEmpty\" tint=1>";

		// Token: 0x04001E96 RID: 7830
		private const string kSeasonIconAsset = "SeasonIcons";

		// Token: 0x04001E97 RID: 7831
		public const string Spring = "<sprite=\"SeasonIcons\" name=\"Spring\" tint=1>";

		// Token: 0x04001E98 RID: 7832
		public const string Summer = "<sprite=\"SeasonIcons\" name=\"Summer\" tint=1>";

		// Token: 0x04001E99 RID: 7833
		public const string Fall = "<sprite=\"SeasonIcons\" name=\"Fall\" tint=1>";

		// Token: 0x04001E9A RID: 7834
		public const string Winter = "<sprite=\"SeasonIcons\" name=\"Winter\" tint=1>";

		// Token: 0x04001E9B RID: 7835
		public const string GELIcon = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E9C RID: 7836
		public const string TurnDown = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";

		// Token: 0x04001E9D RID: 7837
		public const string kTransparentHexColor = "#00000000";

		// Token: 0x04001E9E RID: 7838
		public const string kTransparentSwords = "<color=#00000000><size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size></color>";
	}
}
