using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x02000996 RID: 2454
	public class ItemButton : MonoBehaviour
	{
		// Token: 0x0600496D RID: 18797 RVA: 0x00071533 File Offset: 0x0006F733
		private void Start()
		{
			this.m_button.MouseEnter += this.OnPointerEnter;
			this.m_button.MouseExit += this.OnPointerLeave;
		}

		// Token: 0x0600496E RID: 18798 RVA: 0x00071563 File Offset: 0x0006F763
		private void OnDestroy()
		{
			this.m_button.MouseEnter -= this.OnPointerEnter;
			this.m_button.MouseExit -= this.OnPointerLeave;
		}

		// Token: 0x0600496F RID: 18799 RVA: 0x001AF5F4 File Offset: 0x001AD7F4
		public void SetSpecial(SpecialItemListItemType type)
		{
			this.m_itemName.text = "Deselect Material";
			this.m_itemName.fontStyle &= ~FontStyles.Italic;
			this.m_itemName.color = this.kDefaultTextColor;
			this.m_icon.sprite = this.m_deselectIcon;
			this.m_icon.color = this.m_deselectIconTint;
			this.m_iconText.text = string.Empty;
			this.m_iconGradient.gameObject.SetActive(false);
			this.m_frame.color = this.kDefaultFrameColor;
			this.m_iconFrame.color = this.kDefaultFrameColor;
			this.m_insufficientCount = false;
		}

		// Token: 0x06004970 RID: 18800 RVA: 0x001AF6A4 File Offset: 0x001AD8A4
		public void SetItem(List<ArchetypeInstance> instances, int amountRequired = -1, bool optional = false)
		{
			if (instances != null && instances.Count > 0)
			{
				ItemArchetype itemArchetype = (ItemArchetype)instances[0].Archetype;
				this.m_itemName.text = itemArchetype.DisplayName;
				this.m_itemName.fontStyle &= ~FontStyles.Italic;
				this.m_itemName.color = this.kDefaultTextColor;
				this.m_icon.sprite = itemArchetype.Icon;
				this.m_icon.color = itemArchetype.IconTint;
				int num = 0;
				foreach (ArchetypeInstance archetypeInstance in instances)
				{
					int num2 = num;
					ItemInstanceData itemData = archetypeInstance.ItemData;
					num = num2 + (((itemData != null) ? itemData.Count : null) ?? 1);
				}
				if (amountRequired != -1)
				{
					if (num < amountRequired)
					{
						this.m_iconText.text = string.Format("<color={0}>{1}</color>/{2}", UIManager.RedColor.ToHex(), num, amountRequired);
						this.m_insufficientCount = true;
						this.m_frame.color = UIManager.RedColor;
						this.m_iconFrame.color = UIManager.RedColor;
					}
					else
					{
						this.m_iconText.text = string.Format("{0}/{1}", num, amountRequired);
						this.m_insufficientCount = false;
						this.m_frame.color = this.kDefaultFrameColor;
						this.m_iconFrame.color = this.kDefaultFrameColor;
					}
					this.m_iconGradient.gameObject.SetActive(true);
					return;
				}
				if (num > 1)
				{
					this.m_iconText.text = num.ToString();
					this.m_iconGradient.gameObject.SetActive(true);
					this.m_insufficientCount = false;
					this.m_frame.color = this.kDefaultFrameColor;
					this.m_iconFrame.color = this.kDefaultFrameColor;
					return;
				}
			}
			else
			{
				if (this.m_button.interactable)
				{
					this.m_itemName.text = "Choose an item...";
					this.m_itemName.fontStyle |= FontStyles.Italic;
					this.m_itemName.color = this.kDefaultTextColor;
					this.m_icon.sprite = this.m_defaultIcon;
					this.m_icon.color = Color.white;
					this.m_iconText.text = string.Empty;
					this.m_iconGradient.gameObject.SetActive(false);
					this.m_frame.color = this.kDefaultFrameColor;
					this.m_iconFrame.color = this.kDefaultFrameColor;
					this.m_insufficientCount = false;
					return;
				}
				this.m_itemName.text = "No item found";
				this.m_itemName.fontStyle &= ~FontStyles.Italic;
				this.m_itemName.color = (optional ? this.kDefaultTextColor : UIManager.RedColor);
				this.m_icon.sprite = this.m_defaultIcon;
				this.m_icon.color = Color.white;
				this.m_iconText.text = string.Empty;
				this.m_iconGradient.gameObject.SetActive(false);
				this.m_frame.color = (optional ? this.kDefaultFrameColor : UIManager.RedColor);
				this.m_iconFrame.color = (optional ? this.kDefaultFrameColor : UIManager.RedColor);
				this.m_insufficientCount = false;
			}
		}

		// Token: 0x06004971 RID: 18801 RVA: 0x001AFA1C File Offset: 0x001ADC1C
		private void OnPointerEnter(SolButton button)
		{
			if (this.m_button.interactable && !this.m_insufficientCount)
			{
				this.m_frame.color = this.kHightlighFrameColor;
				this.m_iconFrame.color = this.kHightlighFrameColor;
				return;
			}
			if (this.m_button.interactable)
			{
				this.m_frame.color = Colors.LightCrimson;
				this.m_iconFrame.color = Colors.LightCrimson;
			}
		}

		// Token: 0x06004972 RID: 18802 RVA: 0x001AFA90 File Offset: 0x001ADC90
		private void OnPointerLeave(SolButton button)
		{
			if (this.m_button.interactable && !this.m_insufficientCount)
			{
				this.m_frame.color = this.kDefaultFrameColor;
				this.m_iconFrame.color = this.kDefaultFrameColor;
				return;
			}
			if (this.m_button.interactable)
			{
				this.m_frame.color = UIManager.RedColor;
				this.m_iconFrame.color = UIManager.RedColor;
			}
		}

		// Token: 0x04004474 RID: 17524
		private const string kDefaultItemText = "Choose an item...";

		// Token: 0x04004475 RID: 17525
		private const string kInactiveText = "No item found";

		// Token: 0x04004476 RID: 17526
		private const string kDeselectText = "Deselect Material";

		// Token: 0x04004477 RID: 17527
		private readonly Color kDefaultTextColor = new Color(0.7921569f, 0.772549f, 0.7686275f, 1f);

		// Token: 0x04004478 RID: 17528
		private readonly Color kDefaultFrameColor = new Color(0.3921569f, 0.4078432f, 0.4156863f, 1f);

		// Token: 0x04004479 RID: 17529
		private readonly Color kHightlighFrameColor = Colors.Titanium;

		// Token: 0x0400447A RID: 17530
		[SerializeField]
		private SolButton m_button;

		// Token: 0x0400447B RID: 17531
		[SerializeField]
		private TextMeshProUGUI m_itemName;

		// Token: 0x0400447C RID: 17532
		[SerializeField]
		private Image m_icon;

		// Token: 0x0400447D RID: 17533
		[SerializeField]
		private Sprite m_defaultIcon;

		// Token: 0x0400447E RID: 17534
		[SerializeField]
		private Sprite m_deselectIcon;

		// Token: 0x0400447F RID: 17535
		[SerializeField]
		private Color m_deselectIconTint = Color.white;

		// Token: 0x04004480 RID: 17536
		[SerializeField]
		private Image m_iconGradient;

		// Token: 0x04004481 RID: 17537
		[SerializeField]
		private TextMeshProUGUI m_iconText;

		// Token: 0x04004482 RID: 17538
		[SerializeField]
		private Image m_frame;

		// Token: 0x04004483 RID: 17539
		[SerializeField]
		private Image m_iconFrame;

		// Token: 0x04004484 RID: 17540
		private bool m_insufficientCount;
	}
}
