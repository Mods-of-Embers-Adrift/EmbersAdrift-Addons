using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000725 RID: 1829
	[Serializable]
	public class GatheringSettings
	{
		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x060036F4 RID: 14068 RVA: 0x000659DC File Offset: 0x00063BDC
		public RawComponentFailureChance FailureChance
		{
			get
			{
				return this.m_failureChance;
			}
		}

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x060036F5 RID: 14069 RVA: 0x000659E4 File Offset: 0x00063BE4
		public MinMaxFloatRange ToolDamagePercent
		{
			get
			{
				return this.m_toolDamagePercent;
			}
		}

		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x060036F6 RID: 14070 RVA: 0x000659EC File Offset: 0x00063BEC
		public int ToolDecayPerUse
		{
			get
			{
				return this.m_toolDecayPerUse;
			}
		}

		// Token: 0x060036F7 RID: 14071 RVA: 0x000659F4 File Offset: 0x00063BF4
		public bool TryGetAbilityForToolType(CraftingToolType toolType, out GatheringAbility ability)
		{
			ability = null;
			this.InitializeToolTypeDict();
			return this.m_toolTypeDict.TryGetValue(toolType, out ability);
		}

		// Token: 0x060036F8 RID: 14072 RVA: 0x0016AB68 File Offset: 0x00168D68
		public UniqueId GetAbilityIdForToolType(CraftingToolType toolType)
		{
			this.InitializeToolTypeDict();
			GatheringAbility gatheringAbility;
			if (!this.m_toolTypeDict.TryGetValue(toolType, out gatheringAbility))
			{
				return UniqueId.Empty;
			}
			return gatheringAbility.Id;
		}

		// Token: 0x060036F9 RID: 14073 RVA: 0x0016AB98 File Offset: 0x00168D98
		private void InitializeToolTypeDict()
		{
			if (this.m_toolTypeDict == null)
			{
				this.m_toolTypeDict = new Dictionary<CraftingToolType, GatheringAbility>(default(CraftingToolTypeComparer));
				for (int i = 0; i < this.m_toolRequirements.Length; i++)
				{
					this.m_toolTypeDict.Add(this.m_toolRequirements[i].ToolType, this.m_toolRequirements[i].Ability);
				}
			}
		}

		// Token: 0x04003526 RID: 13606
		[SerializeField]
		private GatheringSettings.ToolForAbility[] m_toolRequirements;

		// Token: 0x04003527 RID: 13607
		[SerializeField]
		private MinMaxFloatRange m_toolDamagePercent = new MinMaxFloatRange(0.01f, 0.05f);

		// Token: 0x04003528 RID: 13608
		[SerializeField]
		private int m_toolDecayPerUse = 2;

		// Token: 0x04003529 RID: 13609
		[SerializeField]
		private RawComponentFailureChance m_failureChance;

		// Token: 0x0400352A RID: 13610
		private Dictionary<CraftingToolType, GatheringAbility> m_toolTypeDict;

		// Token: 0x02000726 RID: 1830
		[Serializable]
		private class ToolForAbility
		{
			// Token: 0x17000C49 RID: 3145
			// (get) Token: 0x060036FB RID: 14075 RVA: 0x00065A30 File Offset: 0x00063C30
			private IEnumerable GetAbilities
			{
				get
				{
					return SolOdinUtilities.GetDropdownItems<GatheringAbility>();
				}
			}

			// Token: 0x0400352B RID: 13611
			public CraftingToolType ToolType;

			// Token: 0x0400352C RID: 13612
			public GatheringAbility Ability;
		}
	}
}
