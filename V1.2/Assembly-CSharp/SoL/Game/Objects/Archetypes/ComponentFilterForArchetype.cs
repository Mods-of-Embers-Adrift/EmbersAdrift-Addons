using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A58 RID: 2648
	[Serializable]
	public class ComponentFilterForArchetype
	{
		// Token: 0x1700128A RID: 4746
		// (get) Token: 0x06005213 RID: 21011 RVA: 0x00076C9F File Offset: 0x00074E9F
		public ItemArchetype Archetype
		{
			get
			{
				return this.m_archetype;
			}
		}

		// Token: 0x1700128B RID: 4747
		// (get) Token: 0x06005214 RID: 21012 RVA: 0x00076CA7 File Offset: 0x00074EA7
		public ComponentParentage[] ComponentFilters
		{
			get
			{
				return this.m_componentFilters;
			}
		}

		// Token: 0x1700128C RID: 4748
		// (get) Token: 0x06005215 RID: 21013 RVA: 0x00076CAF File Offset: 0x00074EAF
		private IEnumerable m_components
		{
			get
			{
				return this.GetComponentDropdownItems(this.Archetype, "", null);
			}
		}

		// Token: 0x06005216 RID: 21014 RVA: 0x001D2ED4 File Offset: 0x001D10D4
		private IEnumerable<IValueDropdownItem> GetComponentDropdownItems(ItemArchetype archetype, string rootPath = "", Stack<ComponentInfo> parentage = null)
		{
			if (archetype == null)
			{
				return new List<IValueDropdownItem>();
			}
			if (!string.IsNullOrEmpty(rootPath))
			{
				rootPath += "/";
			}
			List<IValueDropdownItem> list = new List<IValueDropdownItem>();
			if (parentage == null)
			{
				parentage = new Stack<ComponentInfo>();
			}
			List<Recipe> list2 = archetype.FindRecipesThatProduceThisItem();
			foreach (Recipe recipe in list2)
			{
				foreach (RecipeComponent recipeComponent in recipe.Components)
				{
					if (recipeComponent != null)
					{
						string text = string.Concat(new string[]
						{
							rootPath,
							recipeComponent.DisplayName,
							" (",
							recipe.DisplayName,
							")"
						});
						parentage.Push(new ComponentInfo
						{
							RecipeId = recipe.Id,
							ComponentId = recipeComponent.Id
						});
						list.Add(new ValueDropdownItem(text, new ComponentParentage
						{
							ComponentList = parentage.Reverse<ComponentInfo>().ToArray<ComponentInfo>()
						}));
						foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
						{
							if (componentMaterial != null)
							{
								list.AddRange(this.GetComponentDropdownItems(componentMaterial.Archetype, text, parentage));
							}
						}
						parentage.Pop();
					}
				}
			}
			StaticListPool<Recipe>.ReturnToPool(list2);
			return list;
		}

		// Token: 0x0400497F RID: 18815
		[SerializeField]
		private ItemArchetype m_archetype;

		// Token: 0x04004980 RID: 18816
		[SerializeField]
		private ComponentParentage[] m_componentFilters;
	}
}
