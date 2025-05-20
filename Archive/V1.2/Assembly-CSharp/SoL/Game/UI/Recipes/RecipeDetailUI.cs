using System;
using System.Text;
using SoL.Game.Objects.Archetypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Recipes
{
	// Token: 0x02000935 RID: 2357
	public class RecipeDetailUI : MonoBehaviour
	{
		// Token: 0x06004582 RID: 17794 RVA: 0x0006ECDF File Offset: 0x0006CEDF
		public void SubscribeToEvents(RecipesUI recipesUI)
		{
			recipesUI.RecipeSelectionChanged += this.OnRecipeSelectionChanged;
		}

		// Token: 0x06004583 RID: 17795 RVA: 0x0006ECF3 File Offset: 0x0006CEF3
		public void UnsubscribeFromEvents(RecipesUI recipesUI)
		{
			recipesUI.RecipeSelectionChanged -= this.OnRecipeSelectionChanged;
		}

		// Token: 0x06004584 RID: 17796 RVA: 0x001A019C File Offset: 0x0019E39C
		private void OnRecipeSelectionChanged(Recipe newSelection)
		{
			if (newSelection != null)
			{
				this.m_recipeIcon.sprite = newSelection.Icon;
				this.m_recipeNameText.text = newSelection.DisplayName;
				RecipeDetailUI.m_recipeDetailBuilder.Clear();
				RecipeDetailUI.m_recipeDetailBuilder.Append("Components:\n");
				for (int i = 0; i < newSelection.Components.Length; i++)
				{
					RecipeComponent recipeComponent = newSelection.Components[i];
					RecipeDetailUI.m_recipeDetailBuilder.Append("  -");
					RecipeDetailUI.m_recipeDetailBuilder.Append(recipeComponent.DisplayName);
					if (i != newSelection.Components.Length - 1)
					{
						RecipeDetailUI.m_recipeDetailBuilder.Append("\n");
					}
				}
				RecipeDetailUI.m_recipeDetailBuilder.Append("\n\n");
				RecipeDetailUI.m_recipeDetailBuilder.Append(newSelection.Description);
				this.m_recipeDetailText.text = RecipeDetailUI.m_recipeDetailBuilder.ToString();
			}
		}

		// Token: 0x040041DE RID: 16862
		[SerializeField]
		private Image m_recipeIcon;

		// Token: 0x040041DF RID: 16863
		[SerializeField]
		private TextMeshProUGUI m_recipeNameText;

		// Token: 0x040041E0 RID: 16864
		[SerializeField]
		private TextMeshProUGUI m_recipeDetailText;

		// Token: 0x040041E1 RID: 16865
		private static StringBuilder m_recipeDetailBuilder = new StringBuilder();
	}
}
