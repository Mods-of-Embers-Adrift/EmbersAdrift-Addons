using System;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x020005A2 RID: 1442
	public class PortraitSelectionItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ITooltip, IInteractiveBase
	{
		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x06002D2C RID: 11564 RVA: 0x0005F5AF File Offset: 0x0005D7AF
		public bool IsOn
		{
			get
			{
				return this.m_toggle.isOn;
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x06002D2D RID: 11565 RVA: 0x0005F5BC File Offset: 0x0005D7BC
		public UniqueId SpriteId
		{
			get
			{
				if (this.m_identifiableSprite != null)
				{
					return this.m_identifiableSprite.Id;
				}
				return UniqueId.Empty;
			}
		}

		// Token: 0x06002D2E RID: 11566 RVA: 0x0005F5D7 File Offset: 0x0005D7D7
		private void Awake()
		{
			if (this.m_frame)
			{
				this.m_defaultFrameColor = this.m_frame.color;
			}
		}

		// Token: 0x06002D2F RID: 11567 RVA: 0x0014D1F0 File Offset: 0x0014B3F0
		public void Init(IdentifiableSprite idSprite, bool isSelected, ToggleGroup toggleGroup)
		{
			this.m_subscriberOnly = false;
			this.m_toggle.group = null;
			this.m_identifiableSprite = idSprite;
			if (this.m_identifiableSprite != null)
			{
				this.m_portrait.overrideSprite = this.m_identifiableSprite.Obj;
				this.m_portrait.enabled = true;
				this.m_toggle.isOn = isSelected;
				bool flag = GlobalSettings.Values && GlobalSettings.Values.Portraits != null && GlobalSettings.Values.Portraits.BasePortraits != null && GlobalSettings.Values.Portraits.BasePortraits.Contains(this.m_identifiableSprite);
				this.m_toggle.interactable = (flag || (SessionData.User != null && SessionData.User.IsSubscriber()));
				this.m_frame.color = (flag ? this.m_defaultFrameColor : UIManager.SubscriberColor);
				this.m_subscriberOnly = !flag;
			}
			else
			{
				this.m_portrait.enabled = false;
				this.m_toggle.isOn = false;
			}
			this.m_toggle.group = toggleGroup;
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x0005F5F7 File Offset: 0x0005D7F7
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_highlight != null && this.m_toggle && this.m_toggle.interactable)
			{
				this.m_highlight.enabled = true;
			}
		}

		// Token: 0x06002D31 RID: 11569 RVA: 0x0005F62D File Offset: 0x0005D82D
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = false;
			}
		}

		// Token: 0x06002D32 RID: 11570 RVA: 0x0005F649 File Offset: 0x0005D849
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_subscriberOnly || (SessionData.User != null && SessionData.User.IsSubscriber()))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, "This portrait is reserved for subscribers.", true);
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x06002D33 RID: 11571 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x06002D34 RID: 11572 RVA: 0x0005F679 File Offset: 0x0005D879
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x06002D35 RID: 11573 RVA: 0x0005F687 File Offset: 0x0005D887
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06002D37 RID: 11575 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002CC6 RID: 11462
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04002CC7 RID: 11463
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04002CC8 RID: 11464
		[SerializeField]
		private Image m_portrait;

		// Token: 0x04002CC9 RID: 11465
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04002CCA RID: 11466
		[SerializeField]
		private Image m_frame;

		// Token: 0x04002CCB RID: 11467
		private IdentifiableSprite m_identifiableSprite;

		// Token: 0x04002CCC RID: 11468
		private Color m_defaultFrameColor;

		// Token: 0x04002CCD RID: 11469
		private bool m_subscriberOnly;
	}
}
