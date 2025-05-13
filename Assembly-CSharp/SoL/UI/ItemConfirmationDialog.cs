using System;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000353 RID: 851
	public class ItemConfirmationDialog : ConfirmationDialog
	{
		// Token: 0x0600173A RID: 5946 RVA: 0x00101F54 File Offset: 0x00100154
		protected override void InitInternal()
		{
			base.InitInternal();
			if (this.m_archetypeIcon && this.m_itemLabel)
			{
				ArchetypeInstance instance = this.m_currentOptions.Instance;
				if (instance != null && instance.Archetype)
				{
					string text = instance.Archetype.GetModifiedDisplayName(instance);
					Color color;
					if (instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
					{
						text = text.Color(color);
					}
					this.m_itemLabel.ZStringSetText(text);
					this.m_archetypeIcon.SetIcon(instance.Archetype, new Color?(instance.Archetype.GetInstanceColor(instance)));
				}
				else
				{
					this.m_itemLabel.text = string.Empty;
					this.m_archetypeIcon.SetIcon(null, null);
				}
			}
			if (this.m_subHeaderText && this.m_subHeaderLabel)
			{
				if (this.m_currentOptions.Instance != null && this.m_currentOptions.Instance.ContainerInstance != null)
				{
					this.m_subHeaderLabel.ZStringSetText("Location:");
					this.m_subHeaderText.ZStringSetText(this.m_currentOptions.Instance.ContainerInstance.ContainerType.GetDescription());
				}
				else
				{
					this.m_subHeaderLabel.text = string.Empty;
					this.m_subHeaderText.text = string.Empty;
				}
				if (this.m_currencyDisplay)
				{
					if (this.m_currentOptions.Currency != null)
					{
						this.m_subHeaderLabel.ZStringSetText(this.m_currentOptions.CurrencyLabel);
						this.m_subHeaderText.text = string.Empty;
						this.m_currencyDisplay.UpdateCoin(this.m_currentOptions.Currency.Value);
						this.m_currencyDisplay.gameObject.SetActive(true);
					}
					else
					{
						this.m_currencyDisplay.gameObject.SetActive(false);
					}
				}
			}
			if (this.m_tooltip)
			{
				this.m_tooltip.Instance = this.m_currentOptions.Instance;
			}
		}

		// Token: 0x04001EF9 RID: 7929
		[SerializeField]
		private ArchetypeIconUI m_archetypeIcon;

		// Token: 0x04001EFA RID: 7930
		[SerializeField]
		private TextMeshProUGUI m_itemLabel;

		// Token: 0x04001EFB RID: 7931
		[SerializeField]
		private TextMeshProUGUI m_subHeaderLabel;

		// Token: 0x04001EFC RID: 7932
		[SerializeField]
		private TextMeshProUGUI m_subHeaderText;

		// Token: 0x04001EFD RID: 7933
		[SerializeField]
		private CurrencyDisplayPanelUI m_currencyDisplay;

		// Token: 0x04001EFE RID: 7934
		[SerializeField]
		private SimpleInstanceTooltipTrigger m_tooltip;
	}
}
