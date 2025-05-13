using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200074B RID: 1867
	[Serializable]
	public class UISettings
	{
		// Token: 0x060037A4 RID: 14244 RVA: 0x0016BFE8 File Offset: 0x0016A1E8
		public bool TryGetContainerIcon(ContainerType containerType, out Sprite icon)
		{
			if (this.m_containerIconDict == null)
			{
				this.m_containerIconDict = new Dictionary<ContainerType, UISettings.ContainerIcon>(default(ContainerTypeComparer));
				for (int i = 0; i < this.m_containerIcons.Length; i++)
				{
					this.m_containerIconDict.AddOrReplace(this.m_containerIcons[i].Type, this.m_containerIcons[i]);
				}
			}
			icon = null;
			UISettings.ContainerIcon containerIcon;
			if (this.m_containerIconDict.TryGetValue(containerType, out containerIcon))
			{
				icon = containerIcon.Icon;
			}
			return icon != null;
		}

		// Token: 0x17000C95 RID: 3221
		// (get) Token: 0x060037A5 RID: 14245 RVA: 0x000660EF File Offset: 0x000642EF
		public StringScriptableProbabilityCollection GeneralLoadingTips
		{
			get
			{
				return this.m_generalLoadingTips;
			}
		}

		// Token: 0x17000C96 RID: 3222
		// (get) Token: 0x060037A6 RID: 14246 RVA: 0x000660F7 File Offset: 0x000642F7
		public StringScriptableProbabilityCollection SceneLoadCompleteMessages
		{
			get
			{
				return this.m_sceneLoadCompleteMessages;
			}
		}

		// Token: 0x17000C97 RID: 3223
		// (get) Token: 0x060037A7 RID: 14247 RVA: 0x000660FF File Offset: 0x000642FF
		public float TooltipBackgroundAlpha
		{
			get
			{
				return this.m_tooltipBackgroundAlpha;
			}
		}

		// Token: 0x17000C98 RID: 3224
		// (get) Token: 0x060037A8 RID: 14248 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x17000C99 RID: 3225
		// (get) Token: 0x060037A9 RID: 14249 RVA: 0x00066107 File Offset: 0x00064307
		public Color MaxFlankingColor
		{
			get
			{
				return this.m_maxFlankingColor;
			}
		}

		// Token: 0x17000C9A RID: 3226
		// (get) Token: 0x060037AA RID: 14250 RVA: 0x0006610F File Offset: 0x0006430F
		public Color MidFlankingColor
		{
			get
			{
				return this.m_midFlankingColor;
			}
		}

		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x060037AB RID: 14251 RVA: 0x00066117 File Offset: 0x00064317
		public Color MinFlankingColor
		{
			get
			{
				return this.m_minFlankingColor;
			}
		}

		// Token: 0x17000C9C RID: 3228
		// (get) Token: 0x060037AC RID: 14252 RVA: 0x0006611F File Offset: 0x0006431F
		public Color TrialColor
		{
			get
			{
				return this.m_trialColor;
			}
		}

		// Token: 0x17000C9D RID: 3229
		// (get) Token: 0x060037AD RID: 14253 RVA: 0x00066127 File Offset: 0x00064327
		public ItemCategory FallbackItemCategory
		{
			get
			{
				return this.m_fallbackItemCategory;
			}
		}

		// Token: 0x17000C9E RID: 3230
		// (get) Token: 0x060037AE RID: 14254 RVA: 0x0006612F File Offset: 0x0006432F
		public List<ItemCategory> CategoryOrder
		{
			get
			{
				return this.m_categoryOrder;
			}
		}

		// Token: 0x17000C9F RID: 3231
		// (get) Token: 0x060037AF RID: 14255 RVA: 0x00066137 File Offset: 0x00064337
		public List<ItemCategory> CraftedCategoryOrder
		{
			get
			{
				return this.m_craftedCategoryOrder;
			}
		}

		// Token: 0x17000CA0 RID: 3232
		// (get) Token: 0x060037B0 RID: 14256 RVA: 0x0006613F File Offset: 0x0006433F
		public int SocialServerStatsIndent
		{
			get
			{
				return this.m_socialServerStatsIndent;
			}
		}

		// Token: 0x17000CA1 RID: 3233
		// (get) Token: 0x060037B1 RID: 14257 RVA: 0x00066147 File Offset: 0x00064347
		public int TimeWindowIndent
		{
			get
			{
				return this.m_timeWindowIndent;
			}
		}

		// Token: 0x04003688 RID: 13960
		public static readonly Color StandardTextColor = ColorExtensions.FromHexLiteral(3401958655U);

		// Token: 0x04003689 RID: 13961
		[SerializeField]
		private UISettings.ContainerIcon[] m_containerIcons;

		// Token: 0x0400368A RID: 13962
		private Dictionary<ContainerType, UISettings.ContainerIcon> m_containerIconDict;

		// Token: 0x0400368B RID: 13963
		[SerializeField]
		private StringScriptableProbabilityCollection m_generalLoadingTips;

		// Token: 0x0400368C RID: 13964
		[SerializeField]
		private StringScriptableProbabilityCollection m_sceneLoadCompleteMessages;

		// Token: 0x0400368D RID: 13965
		[Range(0f, 255f)]
		[SerializeField]
		private float m_tooltipBackgroundAlpha = 217f;

		// Token: 0x0400368E RID: 13966
		[SerializeField]
		private Color m_maxFlankingColor = Color.white;

		// Token: 0x0400368F RID: 13967
		[SerializeField]
		private Color m_midFlankingColor = Color.white;

		// Token: 0x04003690 RID: 13968
		[SerializeField]
		private Color m_minFlankingColor = Color.white;

		// Token: 0x04003691 RID: 13969
		[SerializeField]
		private Color m_trialColor = Colors.Tan;

		// Token: 0x04003692 RID: 13970
		[SerializeField]
		private ItemCategory m_fallbackItemCategory;

		// Token: 0x04003693 RID: 13971
		[SerializeField]
		private List<ItemCategory> m_categoryOrder;

		// Token: 0x04003694 RID: 13972
		[SerializeField]
		private List<ItemCategory> m_craftedCategoryOrder;

		// Token: 0x04003695 RID: 13973
		[SerializeField]
		private int m_socialServerStatsIndent = 100;

		// Token: 0x04003696 RID: 13974
		[SerializeField]
		private int m_timeWindowIndent = 100;

		// Token: 0x0200074C RID: 1868
		[Serializable]
		private class ContainerIcon
		{
			// Token: 0x17000CA2 RID: 3234
			// (get) Token: 0x060037B4 RID: 14260 RVA: 0x00066160 File Offset: 0x00064360
			public ContainerType Type
			{
				get
				{
					return this.m_containerType;
				}
			}

			// Token: 0x17000CA3 RID: 3235
			// (get) Token: 0x060037B5 RID: 14261 RVA: 0x00066168 File Offset: 0x00064368
			public Sprite Icon
			{
				get
				{
					return this.m_icon;
				}
			}

			// Token: 0x04003697 RID: 13975
			[SerializeField]
			private ContainerType m_containerType;

			// Token: 0x04003698 RID: 13976
			[SerializeField]
			private Sprite m_icon;
		}
	}
}
