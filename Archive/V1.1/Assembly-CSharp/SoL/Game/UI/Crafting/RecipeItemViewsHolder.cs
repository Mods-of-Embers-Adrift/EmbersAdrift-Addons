using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x0200099D RID: 2461
	[Serializable]
	public class RecipeItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x1700104A RID: 4170
		// (get) Token: 0x060049A4 RID: 18852 RVA: 0x00071768 File Offset: 0x0006F968
		// (set) Token: 0x060049A5 RID: 18853 RVA: 0x00071770 File Offset: 0x0006F970
		public RecipesListItem ListItem { get; private set; }

		// Token: 0x1700104B RID: 4171
		// (get) Token: 0x060049A6 RID: 18854 RVA: 0x00071779 File Offset: 0x0006F979
		// (set) Token: 0x060049A7 RID: 18855 RVA: 0x00071781 File Offset: 0x0006F981
		public Recipe Data { get; private set; }

		// Token: 0x060049A8 RID: 18856 RVA: 0x0007178A File Offset: 0x0006F98A
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<RecipesListItem>();
		}

		// Token: 0x060049A9 RID: 18857 RVA: 0x000717A3 File Offset: 0x0006F9A3
		public void UpdateItem(RecipesList parent, Recipe item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, this.ItemIndex);
		}

		// Token: 0x060049AA RID: 18858 RVA: 0x000717D3 File Offset: 0x0006F9D3
		public void RefreshVisuals()
		{
			this.ListItem.RefreshVisuals();
		}
	}
}
