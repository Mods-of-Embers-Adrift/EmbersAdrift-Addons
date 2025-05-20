using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CEC RID: 3308
	public class RecipeListUI : MonoBehaviour
	{
		// Token: 0x0600642D RID: 25645 RVA: 0x00209328 File Offset: 0x00207528
		private void Awake()
		{
			this.m_toggleGroup.allowSwitchOff = true;
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].Init(this, i, this.m_toggleGroup);
			}
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x0004475B File Offset: 0x0004295B
		public void ActivateToggle(int index)
		{
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x0020936C File Offset: 0x0020756C
		public Recipe GetActiveRecipe()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].gameObject.activeSelf && this.m_toggles[i].Recipe != null && this.m_toggles[i].IsActive)
				{
					return this.m_toggles[i].Recipe;
				}
			}
			return null;
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x002093D4 File Offset: 0x002075D4
		public void UpdateRecipes(List<Recipe> recipes)
		{
			Recipe activeRecipe = this.GetActiveRecipe();
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				ArchetypeInstance masteryInstance;
				if (i < recipes.Count && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(recipes[i].Mastery.Id, out masteryInstance))
				{
					this.m_toggles[i].SetRecipe(recipes[i], masteryInstance);
					this.m_toggles[i].gameObject.SetActive(true);
					if (activeRecipe != null && recipes[i].Equals(activeRecipe))
					{
						this.m_toggles[i].IsActive = true;
					}
				}
				else
				{
					this.m_toggles[i].SetRecipe(null, null);
					this.m_toggles[i].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006431 RID: 25649 RVA: 0x002094A8 File Offset: 0x002076A8
		public void ToggleLock(bool locked)
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].ToggleLock(locked);
			}
		}

		// Token: 0x04005705 RID: 22277
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x04005706 RID: 22278
		[SerializeField]
		private RecipeListUIElement[] m_toggles;
	}
}
