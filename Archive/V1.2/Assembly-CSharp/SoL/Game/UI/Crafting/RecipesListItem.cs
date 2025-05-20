using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x0200099E RID: 2462
	public class RecipesListItem : Selectable, ITooltip, IInteractiveBase
	{
		// Token: 0x1700104C RID: 4172
		// (get) Token: 0x060049AC RID: 18860 RVA: 0x000717E0 File Offset: 0x0006F9E0
		public bool IsSelected
		{
			get
			{
				return this.m_itemIndex == this.m_parent.SelectedIndex;
			}
		}

		// Token: 0x1700104D RID: 4173
		// (get) Token: 0x060049AD RID: 18861 RVA: 0x000717F5 File Offset: 0x0006F9F5
		public Recipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
		}

		// Token: 0x1700104E RID: 4174
		// (get) Token: 0x060049AE RID: 18862 RVA: 0x000717FD File Offset: 0x0006F9FD
		// (set) Token: 0x060049AF RID: 18863 RVA: 0x00071805 File Offset: 0x0006FA05
		public bool Hovered
		{
			get
			{
				return this.m_isHovered;
			}
			private set
			{
				this.m_isHovered = value;
				this.RefreshHighlightVisuals();
			}
		}

		// Token: 0x060049B0 RID: 18864 RVA: 0x001B04BC File Offset: 0x001AE6BC
		public void Init(RecipesList parent, Recipe recipe, int index)
		{
			if (this.m_parent == null)
			{
				parent.SelectionChanged += this.OnSelectionChanged;
			}
			this.m_parent = parent;
			this.m_recipe = recipe;
			this.m_itemIndex = index;
			this.m_icon.sprite = this.m_recipe.Icon;
			this.m_icon.color = this.m_recipe.IconTint;
			this.m_label.text = this.m_recipe.DisplayName;
			this.RefreshVisuals();
		}

		// Token: 0x060049B1 RID: 18865 RVA: 0x00071814 File Offset: 0x0006FA14
		public void Reindex(int index)
		{
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x060049B2 RID: 18866 RVA: 0x00071823 File Offset: 0x0006FA23
		protected override void OnDestroy()
		{
			if (this.m_parent != null)
			{
				this.m_parent.SelectionChanged -= this.OnSelectionChanged;
			}
		}

		// Token: 0x060049B3 RID: 18867 RVA: 0x0007184A File Offset: 0x0006FA4A
		public void OnInventoryChanged()
		{
			this.RefreshVisuals();
		}

		// Token: 0x060049B4 RID: 18868 RVA: 0x0007184A File Offset: 0x0006FA4A
		public void OnSelectionChanged(Recipe recipe)
		{
			this.RefreshVisuals();
		}

		// Token: 0x060049B5 RID: 18869 RVA: 0x00071852 File Offset: 0x0006FA52
		public void LockUI()
		{
			base.interactable = false;
		}

		// Token: 0x060049B6 RID: 18870 RVA: 0x0007185B File Offset: 0x0006FA5B
		public void UnlockUI()
		{
			base.interactable = true;
		}

		// Token: 0x060049B7 RID: 18871 RVA: 0x001B0548 File Offset: 0x001AE748
		public void RefreshVisuals()
		{
			this.RefreshHighlightVisuals();
			if (this.m_recipe != null)
			{
				ArchetypeInstance archetypeInstance;
				bool flag = LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_recipe.Ability.Id, out archetypeInstance);
				InteractiveRefinementStation refinementStation = LocalPlayer.GameEntity.CollectionController.RefinementStation;
				bool flag2 = ((refinementStation != null) ? refinementStation.Profile : null) == null || this.m_recipe.StationCategory.HasBitFlag(LocalPlayer.GameEntity.CollectionController.RefinementStation.Profile.Category);
				ContainerInstance containerInstance;
				ContainerInstance containerInstance2;
				if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2) && flag && archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity) >= this.m_recipe.MinimumAbilityLevel && flag2)
				{
					List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
					fromPool.Capacity = containerInstance.Count + containerInstance2.Count;
					fromPool.AddRange(containerInstance2.Instances);
					fromPool.AddRange(containerInstance.Instances);
					List<ItemUsage> list;
					RecipeComponent recipeComponent;
					bool flag3 = this.m_recipe.Match(fromPool, out list, out recipeComponent);
					StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
					if (flag3)
					{
						this.m_label.color = this.kDefaultTextColor;
						this.m_lowLevelIndicator.gameObject.SetActive(archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity) < this.m_recipe.MinimumAbilityLevel);
						return;
					}
					this.m_label.color = this.kInactiveTextColor;
					this.m_lowLevelIndicator.gameObject.SetActive(flag && archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity) < this.m_recipe.MinimumAbilityLevel);
					return;
				}
				else if (LocalPlayer.GameEntity != null)
				{
					this.m_label.color = this.kInactiveTextColor;
					this.m_lowLevelIndicator.gameObject.SetActive(flag && archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity) < this.m_recipe.MinimumAbilityLevel);
					return;
				}
			}
			else
			{
				this.m_label.color = this.kInactiveTextColor;
			}
		}

		// Token: 0x060049B8 RID: 18872 RVA: 0x001B076C File Offset: 0x001AE96C
		private void RefreshHighlightVisuals()
		{
			if (this.IsSelected)
			{
				this.m_highlight.gameObject.SetActive(true);
				this.m_highlight.color = this.m_selectedColor;
				return;
			}
			if (this.m_isHovered)
			{
				this.m_highlight.gameObject.SetActive(true);
				this.m_highlight.color = this.m_highlightColor;
				return;
			}
			this.m_highlight.gameObject.SetActive(false);
		}

		// Token: 0x060049B9 RID: 18873 RVA: 0x00049FFA File Offset: 0x000481FA
		private ITooltipParameter GetTooltipParameter()
		{
			return null;
		}

		// Token: 0x1700104F RID: 4175
		// (get) Token: 0x060049BA RID: 18874 RVA: 0x00071864 File Offset: 0x0006FA64
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001050 RID: 4176
		// (get) Token: 0x060049BB RID: 18875 RVA: 0x00071872 File Offset: 0x0006FA72
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17001051 RID: 4177
		// (get) Token: 0x060049BC RID: 18876 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060049BD RID: 18877 RVA: 0x0007187A File Offset: 0x0006FA7A
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.IsSelected)
			{
				this.m_parent.Select(this.m_itemIndex);
			}
		}

		// Token: 0x060049BE RID: 18878 RVA: 0x00071895 File Offset: 0x0006FA95
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x060049BF RID: 18879 RVA: 0x0007189E File Offset: 0x0006FA9E
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x060049C1 RID: 18881 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040044A5 RID: 17573
		private readonly Color kDefaultTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 1f);

		// Token: 0x040044A6 RID: 17574
		private readonly Color kInactiveTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 0.3f);

		// Token: 0x040044A7 RID: 17575
		[SerializeField]
		private Image m_highlight;

		// Token: 0x040044A8 RID: 17576
		[SerializeField]
		private Image m_icon;

		// Token: 0x040044A9 RID: 17577
		[SerializeField]
		private Image m_lowLevelIndicator;

		// Token: 0x040044AA RID: 17578
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040044AB RID: 17579
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040044AC RID: 17580
		private RecipesList m_parent;

		// Token: 0x040044AD RID: 17581
		private Recipe m_recipe;

		// Token: 0x040044AE RID: 17582
		private int m_itemIndex = -1;

		// Token: 0x040044AF RID: 17583
		private Color m_selectedColor = Colors.CadetGrey;

		// Token: 0x040044B0 RID: 17584
		private Color m_highlightColor = Colors.GoldMetallic;

		// Token: 0x040044B1 RID: 17585
		private bool m_isHovered;
	}
}
