using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A4D RID: 2637
	[Serializable]
	public class ComponentParentage
	{
		// Token: 0x060051A5 RID: 20901 RVA: 0x001D0F24 File Offset: 0x001CF124
		public string CreateEditorElementName()
		{
			if (this.ComponentList == null || this.ComponentList.Length == 0)
			{
				return "No Filter";
			}
			ComponentInfo componentInfo = this.ComponentList[this.ComponentList.Length - 1];
			Recipe recipe;
			if (InternalGameDatabase.Archetypes.TryGetAsType<Recipe>(componentInfo.RecipeId, out recipe))
			{
				RecipeComponent recipeComponent = null;
				foreach (RecipeComponent recipeComponent2 in recipe.Components)
				{
					if (recipeComponent2.Id == componentInfo.ComponentId)
					{
						recipeComponent = recipeComponent2;
					}
				}
				return recipeComponent.DisplayName + " (" + recipe.DisplayName + ")";
			}
			return this.ComponentList[this.ComponentList.Length - 1].ComponentId;
		}

		// Token: 0x040048EB RID: 18667
		[SerializeField]
		public ComponentInfo[] ComponentList;

		// Token: 0x040048EC RID: 18668
		private const string kNoFilterComponentSelected = "No Filter";
	}
}
