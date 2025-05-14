using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x0200099B RID: 2459
	public class OutputPreview : MonoBehaviour, ITooltip, IInteractiveBase, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x17001045 RID: 4165
		// (get) Token: 0x0600498C RID: 18828 RVA: 0x000716C1 File Offset: 0x0006F8C1
		// (set) Token: 0x0600498D RID: 18829 RVA: 0x001AFF88 File Offset: 0x001AE188
		public ArchetypeInstance PreviewInstance
		{
			get
			{
				return this.m_previewInstance;
			}
			set
			{
				this.m_previewInstance = value;
				if (this.m_previewInstance != null)
				{
					if (this.m_itemIconBorder)
					{
						if (this.m_defaultBorderColor == null)
						{
							this.m_defaultBorderColor = new Color?(this.m_itemIconBorder.color);
						}
						Color color;
						this.m_itemIconBorder.color = (this.m_previewInstance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.IconBorder, out color) ? color : this.m_defaultBorderColor.Value);
					}
					string text = string.Empty;
					if (this.m_previewInstance.Archetype.TryGetItemCategoryDescription(out text))
					{
						Color color2;
						text = (this.m_previewInstance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.Description, out color2) ? ZString.Format<string, string>(" <size=80%><color={0}>(<i>{1}</i>)</color></size>", color2.ToHex(), text) : ZString.Format<string>(" <size=80%>(<i>{0}</i>)</size>", text));
					}
					Color color3;
					string arg = this.m_previewInstance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color3) ? ZString.Format<string, string, string>("<color={0}>{1}</color>{2}", color3.ToHex(), this.m_previewInstance.Archetype.DisplayName, text) : ZString.Format<string, string>("{0}{1}", this.m_previewInstance.Archetype.DisplayName, text);
					this.m_itemName.ZStringSetText(arg);
					this.m_itemName.fontStyle &= ~FontStyles.Italic;
					this.m_itemIcon.sprite = this.m_previewInstance.Archetype.Icon;
					this.m_itemIcon.color = this.m_previewInstance.Archetype.IconTint;
					this.m_previewParameters.Instance = this.m_previewInstance;
					GameObject gameObject = this.m_iconText.gameObject;
					int? count = this.m_previewInstance.ItemData.Count;
					int num = 1;
					gameObject.SetActive(count.GetValueOrDefault() > num & count != null);
					this.m_iconText.text = this.m_previewInstance.ItemData.Count.ToString();
					return;
				}
				this.m_itemName.text = "Select materials to preview output.";
				this.m_itemName.fontStyle |= FontStyles.Italic;
				this.m_itemIcon.sprite = this.m_defaultIcon;
				this.m_itemIcon.color = Color.white;
				this.m_iconText.gameObject.SetActive(false);
				if (this.m_itemIconBorder && this.m_defaultBorderColor != null)
				{
					this.m_itemIconBorder.color = this.m_defaultBorderColor.Value;
				}
			}
		}

		// Token: 0x0600498E RID: 18830 RVA: 0x001B01F8 File Offset: 0x001AE3F8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_previewInstance == null)
			{
				return new ObjectTextTooltipParameter(null, "Select materials to preview output.", false);
			}
			return this.m_previewParameters;
		}

		// Token: 0x0600498F RID: 18831 RVA: 0x000716C9 File Offset: 0x0006F8C9
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocused = UIManager.IsChatActive;
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x001B0230 File Offset: 0x001AE430
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			bool chatWasFocused = this.m_chatWasFocused;
			this.m_chatWasFocused = false;
			if (this.m_previewInstance != null && eventData.button == PointerEventData.InputButton.Left && ClientGameManager.InputManager.HoldingShift && chatWasFocused)
			{
				UIManager.ActiveChatInput.AddInstanceLink(this.m_previewInstance);
			}
		}

		// Token: 0x17001046 RID: 4166
		// (get) Token: 0x06004991 RID: 18833 RVA: 0x000716D6 File Offset: 0x0006F8D6
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001047 RID: 4167
		// (get) Token: 0x06004992 RID: 18834 RVA: 0x000716E4 File Offset: 0x0006F8E4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17001048 RID: 4168
		// (get) Token: 0x06004993 RID: 18835 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004995 RID: 18837 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004495 RID: 17557
		private const string kDefaultItemText = "Select materials to preview output.";

		// Token: 0x04004496 RID: 17558
		[SerializeField]
		private TextMeshProUGUI m_itemName;

		// Token: 0x04004497 RID: 17559
		[SerializeField]
		private Image m_itemIcon;

		// Token: 0x04004498 RID: 17560
		[SerializeField]
		private Image m_itemIconBorder;

		// Token: 0x04004499 RID: 17561
		[SerializeField]
		private TextMeshProUGUI m_iconText;

		// Token: 0x0400449A RID: 17562
		[SerializeField]
		private Sprite m_defaultIcon;

		// Token: 0x0400449B RID: 17563
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400449C RID: 17564
		private ArchetypeInstance m_previewInstance;

		// Token: 0x0400449D RID: 17565
		private Color? m_defaultBorderColor;

		// Token: 0x0400449E RID: 17566
		private ArchetypeTooltipParameter m_previewParameters;

		// Token: 0x0400449F RID: 17567
		private bool m_chatWasFocused;
	}
}
