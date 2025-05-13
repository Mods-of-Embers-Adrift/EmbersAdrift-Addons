using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Recipes
{
	// Token: 0x02000937 RID: 2359
	public class RecipeListUI : MonoBehaviour
	{
		// Token: 0x140000CD RID: 205
		// (add) Token: 0x06004593 RID: 17811 RVA: 0x001A037C File Offset: 0x0019E57C
		// (remove) Token: 0x06004594 RID: 17812 RVA: 0x001A03B4 File Offset: 0x0019E5B4
		public event Action<Recipe> RecipeSelected;

		// Token: 0x06004595 RID: 17813 RVA: 0x001A03EC File Offset: 0x0019E5EC
		protected void Start()
		{
			for (int i = 0; i < this.m_listItems.Count; i++)
			{
				this.m_listItems[i].Index = i;
				this.m_listItems[i].ValueChanged += this.OnListItemClicked;
			}
		}

		// Token: 0x06004596 RID: 17814 RVA: 0x001A0440 File Offset: 0x0019E640
		protected void OnDestroy()
		{
			for (int i = 0; i < this.m_listItems.Count; i++)
			{
				this.m_listItems[i].ValueChanged -= this.OnListItemClicked;
			}
		}

		// Token: 0x06004597 RID: 17815 RVA: 0x001A0480 File Offset: 0x0019E680
		private void OnListItemClicked(int index, Recipe recipe, bool value)
		{
			if (this.m_ignoreSubsequentEvents)
			{
				return;
			}
			this.m_ignoreSubsequentEvents = true;
			for (int i = 0; i < this.m_listItems.Count; i++)
			{
				if (i != index)
				{
					this.m_listItems[i].Selected = false;
				}
			}
			this.m_ignoreSubsequentEvents = false;
			Action<Recipe> recipeSelected = this.RecipeSelected;
			if (recipeSelected == null)
			{
				return;
			}
			recipeSelected(value ? recipe : null);
		}

		// Token: 0x06004598 RID: 17816 RVA: 0x001A04E8 File Offset: 0x0019E6E8
		public void UpdateRecipeList(List<Recipe> recipes, Recipe selected)
		{
			float num = 0f;
			int i = 0;
			while (i < recipes.Count && i < this.m_listItems.Count)
			{
				this.m_listItems[i].gameObject.SetActive(true);
				this.m_listItems[i].Icon.sprite = recipes[i].Icon;
				this.m_listItems[i].Icon.color = recipes[i].IconTint;
				this.m_listItems[i].Text.text = recipes[i].DisplayName;
				RecipeListItemUI recipeListItemUI = this.m_listItems[i];
				UniqueId id = recipes[i].Id;
				recipeListItemUI.Selected = (id == ((selected != null) ? new UniqueId?(selected.Id) : null));
				this.m_listItems[i].Recipe = recipes[i];
				num += this.m_listItems[i].gameObject.GetComponent<RectTransform>().rect.height;
				i++;
			}
			if (recipes.Count < this.m_listItems.Count)
			{
				this.m_noRecipesText.gameObject.SetActive(i == 0);
				while (i < this.m_listItems.Count)
				{
					this.m_listItems[i].gameObject.SetActive(false);
					i++;
				}
			}
			else
			{
				while (i < recipes.Count)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_listItems[this.m_listItems.Count - 1].gameObject, this.m_contentArea.gameObject.transform);
					RectTransform component = gameObject.GetComponent<RectTransform>();
					component.SetPositionAndRotation(new Vector3(component.position.x, component.position.y - component.rect.height, component.position.z), component.rotation);
					this.m_listItems.Add(gameObject.GetComponent<RecipeListItemUI>());
					this.m_listItems[i].gameObject.SetActive(true);
					this.m_listItems[i].Icon.sprite = recipes[i].Icon;
					this.m_listItems[i].Icon.color = recipes[i].IconTint;
					this.m_listItems[i].Text.text = recipes[i].DisplayName;
					RecipeListItemUI recipeListItemUI2 = this.m_listItems[i];
					UniqueId id = recipes[i].Id;
					recipeListItemUI2.Selected = (id == ((selected != null) ? new UniqueId?(selected.Id) : null));
					this.m_listItems[i].Recipe = recipes[i];
					this.m_listItems[i].Index = i;
					this.m_listItems[i].ValueChanged += this.OnListItemClicked;
					num += component.rect.height;
					i++;
				}
				this.m_noRecipesText.gameObject.SetActive(i == 0);
			}
			this.m_contentArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
		}

		// Token: 0x040041EE RID: 16878
		[SerializeField]
		private RectTransform m_contentArea;

		// Token: 0x040041EF RID: 16879
		[SerializeField]
		private List<RecipeListItemUI> m_listItems;

		// Token: 0x040041F0 RID: 16880
		[SerializeField]
		private TextMeshProUGUI m_noRecipesText;

		// Token: 0x040041F1 RID: 16881
		private bool m_ignoreSubsequentEvents;
	}
}
