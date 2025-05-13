using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CED RID: 3309
	public class RecipeListUIElement : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17001808 RID: 6152
		// (get) Token: 0x06006433 RID: 25651 RVA: 0x000836B9 File Offset: 0x000818B9
		// (set) Token: 0x06006434 RID: 25652 RVA: 0x000836C6 File Offset: 0x000818C6
		public bool IsActive
		{
			get
			{
				return this.m_toggle.isOn;
			}
			set
			{
				this.m_toggle.isOn = value;
			}
		}

		// Token: 0x17001809 RID: 6153
		// (get) Token: 0x06006435 RID: 25653 RVA: 0x000836D4 File Offset: 0x000818D4
		public Recipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x00209478 File Offset: 0x00207678
		public void SetRecipe(Recipe value, ArchetypeInstance masteryInstance)
		{
			bool flag = this.m_recipe != value;
			this.m_recipe = value;
			this.m_toggle.text = ((this.m_recipe == null) ? "NONE" : this.m_recipe.DisplayName);
			this.m_countLabel.text = string.Empty;
			if (this.m_recipe == null || (flag && this.m_toggle.isOn))
			{
				this.m_toggle.isOn = false;
			}
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x000836DC File Offset: 0x000818DC
		private void Awake()
		{
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
		}

		// Token: 0x06006438 RID: 25656 RVA: 0x000836FA File Offset: 0x000818FA
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
		}

		// Token: 0x06006439 RID: 25657 RVA: 0x00083718 File Offset: 0x00081918
		public void Init(RecipeListUI listUi, int index, ToggleGroup toggleGroup)
		{
			this.m_listUi = listUi;
			this.m_index = index;
			this.m_toggle.group = toggleGroup;
		}

		// Token: 0x0600643A RID: 25658 RVA: 0x00083734 File Offset: 0x00081934
		private void ToggleChanged(bool active)
		{
			if (this.m_listUi != null && active)
			{
				this.m_listUi.ActivateToggle(this.m_index);
			}
		}

		// Token: 0x0600643B RID: 25659 RVA: 0x00083757 File Offset: 0x00081957
		public void ToggleLock(bool locked)
		{
			this.m_toggle.interactable = !locked;
		}

		// Token: 0x0600643C RID: 25660 RVA: 0x00049FFA File Offset: 0x000481FA
		private ITooltipParameter GetTooltipParameter()
		{
			return null;
		}

		// Token: 0x1700180A RID: 6154
		// (get) Token: 0x0600643D RID: 25661 RVA: 0x00083768 File Offset: 0x00081968
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700180B RID: 6155
		// (get) Token: 0x0600643E RID: 25662 RVA: 0x00083776 File Offset: 0x00081976
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x1700180C RID: 6156
		// (get) Token: 0x0600643F RID: 25663 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06006441 RID: 25665 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04005707 RID: 22279
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04005708 RID: 22280
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x04005709 RID: 22281
		private RecipeListUI m_listUi;

		// Token: 0x0400570A RID: 22282
		private int m_index;

		// Token: 0x0400570B RID: 22283
		private Recipe m_recipe;

		// Token: 0x0400570C RID: 22284
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
