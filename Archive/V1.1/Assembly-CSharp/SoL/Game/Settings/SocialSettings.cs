using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities.Extensions;

namespace SoL.Game.Settings
{
	// Token: 0x02000741 RID: 1857
	[Serializable]
	public class SocialSettings
	{
		// Token: 0x06003779 RID: 14201 RVA: 0x0016BD7C File Offset: 0x00169F7C
		public bool TryGetPostageForItemCategory(ItemCategory category, out ulong postage)
		{
			if (this.m_postagesForCategories == null)
			{
				this.m_postagesForCategories = new Dictionary<ItemCategory, ulong>(this.CategoryPostages.Count);
				foreach (PostageForItemCategory postageForItemCategory in this.CategoryPostages)
				{
					this.m_postagesForCategories.AddOrReplace(postageForItemCategory.Category, postageForItemCategory.Postage);
				}
			}
			return this.m_postagesForCategories.TryGetValue(category, out postage);
		}

		// Token: 0x04003650 RID: 13904
		public int LfgLfmNotesCharacterLimit = 140;

		// Token: 0x04003651 RID: 13905
		public int LfgLfmLineLimit = 6;

		// Token: 0x04003652 RID: 13906
		public int GuildDescriptionCharacterLimit = 140;

		// Token: 0x04003653 RID: 13907
		public int GuildMotdCharacterLimit = 140;

		// Token: 0x04003654 RID: 13908
		public int GuildPublicNoteCharacterLimit = 140;

		// Token: 0x04003655 RID: 13909
		public int GuildOfficerNoteCharacterLimit = 140;

		// Token: 0x04003656 RID: 13910
		public ulong BasePostage = 30UL;

		// Token: 0x04003657 RID: 13911
		public float SellPriceFractionForPostage = 0.25f;

		// Token: 0x04003658 RID: 13912
		public float StackablePostageFraction = 0.1f;

		// Token: 0x04003659 RID: 13913
		public double PostExpirationInDays = 10.0;

		// Token: 0x0400365A RID: 13914
		public List<PostageForItemCategory> CategoryPostages;

		// Token: 0x0400365B RID: 13915
		private Dictionary<ItemCategory, ulong> m_postagesForCategories;
	}
}
