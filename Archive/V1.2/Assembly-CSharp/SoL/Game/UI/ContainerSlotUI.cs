using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200086C RID: 2156
	public class ContainerSlotUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IArchetypeDropZone, ITooltip, IInteractiveBase
	{
		// Token: 0x17000E63 RID: 3683
		// (get) Token: 0x06003E5B RID: 15963 RVA: 0x0006A2D2 File Offset: 0x000684D2
		protected Color DefaultFrameColor
		{
			get
			{
				if (this.m_defaultFrameColor == null)
				{
					return ContainerSlotUI.kFallbackFrameColor;
				}
				return this.m_defaultFrameColor.Value;
			}
		}

		// Token: 0x17000E64 RID: 3684
		// (get) Token: 0x06003E5C RID: 15964 RVA: 0x0006A2F2 File Offset: 0x000684F2
		// (set) Token: 0x06003E5D RID: 15965 RVA: 0x00185204 File Offset: 0x00183404
		private Color? FrameColorOverride
		{
			get
			{
				return this.m_frameColorOverride;
			}
			set
			{
				if (value == this.m_frameColorOverride || !this.m_frame)
				{
					return;
				}
				if (this.m_defaultFrameColor == null)
				{
					this.m_defaultFrameColor = new Color?(this.m_frame.color);
				}
				this.m_frameColorOverride = value;
				Color value2;
				if (this.m_frameColorOverride != null)
				{
					value2 = this.m_frameColorOverride.Value;
					value2.a = this.m_defaultFrameColor.Value.a;
				}
				else
				{
					value2 = this.m_defaultFrameColor.Value;
				}
				this.m_frame.color = value2;
			}
		}

		// Token: 0x17000E65 RID: 3685
		// (get) Token: 0x06003E5E RID: 15966 RVA: 0x0006A2FA File Offset: 0x000684FA
		public ArchetypeInstance Instance
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x17000E66 RID: 3686
		// (get) Token: 0x06003E5F RID: 15967 RVA: 0x0006A302 File Offset: 0x00068502
		public RectTransform RectTransform
		{
			get
			{
				return this.m_itemSlot;
			}
		}

		// Token: 0x06003E60 RID: 15968 RVA: 0x001852D0 File Offset: 0x001834D0
		public virtual void Initialize(IContainerUI containerUI, int index)
		{
			this.m_containerUI = containerUI;
			this.m_index = index;
			this.InitHighlight();
			this.m_subscriberOnly = (this.m_containerUI != null && this.m_containerUI.ContainerInstance != null && this.m_containerUI.ContainerInstance.IsSubscriberOnlySlot(index));
			if (this.m_frame && this.m_defaultFrameColor == null)
			{
				this.m_defaultFrameColor = new Color?(this.m_frame.color);
			}
			if (this.m_bgHighlight)
			{
				this.m_bgHighlight.enabled = this.m_subscriberOnly;
				if (this.m_subscriberOnly)
				{
					Color color = UIManager.SubscriberColor;
					float h;
					float s;
					float num;
					Color.RGBToHSV(color, out h, out s, out num);
					num *= ((SessionData.User != null && SessionData.User.IsSubscriber()) ? GlobalSettings.Values.Subscribers.SubscriberBankBorderValueMultiplier : GlobalSettings.Values.Subscribers.RegularBankBorderValueMultiplier);
					color = Color.HSVToRGB(h, s, num);
					this.m_bgHighlight.color = color;
				}
			}
		}

		// Token: 0x06003E61 RID: 15969 RVA: 0x001853D4 File Offset: 0x001835D4
		public virtual void InstanceAdded(ArchetypeInstance instance)
		{
			this.m_instance = instance;
			if (this.m_instance != null)
			{
				if (this.m_instance.InstanceUI)
				{
					this.m_instance.InstanceUI.AssignSlotUI(this);
				}
				Color value;
				if (this.m_instance.Archetype && this.m_instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.IconBorder, out value))
				{
					this.FrameColorOverride = new Color?(value);
				}
			}
		}

		// Token: 0x06003E62 RID: 15970 RVA: 0x00185448 File Offset: 0x00183648
		public virtual void InstanceRemoved(ArchetypeInstance instance)
		{
			this.m_instance = null;
			this.FrameColorOverride = null;
		}

		// Token: 0x06003E63 RID: 15971 RVA: 0x0006A30A File Offset: 0x0006850A
		protected void InitHighlight()
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = true;
				this.ToggleHighlight(false, true);
			}
		}

		// Token: 0x06003E64 RID: 15972 RVA: 0x0006A32E File Offset: 0x0006852E
		public void ToggleHighlight(bool isEnabled, bool instant = false)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.CrossFadeAlpha(isEnabled ? 1f : 0f, instant ? 0f : 0.1f, true);
			}
		}

		// Token: 0x06003E65 RID: 15973 RVA: 0x0006A368 File Offset: 0x00068568
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_containerUI != null && !this.m_containerUI.Locked)
			{
				this.ToggleHighlight(true, false);
			}
		}

		// Token: 0x06003E66 RID: 15974 RVA: 0x0006A387 File Offset: 0x00068587
		public void OnPointerExit(PointerEventData eventData)
		{
			this.ToggleHighlight(false, false);
		}

		// Token: 0x17000E67 RID: 3687
		// (get) Token: 0x06003E67 RID: 15975 RVA: 0x0006A391 File Offset: 0x00068591
		public int Index
		{
			get
			{
				return this.m_index;
			}
		}

		// Token: 0x17000E68 RID: 3688
		// (get) Token: 0x06003E68 RID: 15976 RVA: 0x0006A2FA File Offset: 0x000684FA
		public ArchetypeInstance CurrentOccupant
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x17000E69 RID: 3689
		// (get) Token: 0x06003E69 RID: 15977 RVA: 0x0006A399 File Offset: 0x00068599
		public IContainerUI ContainerUI
		{
			get
			{
				return this.m_containerUI;
			}
		}

		// Token: 0x06003E6A RID: 15978 RVA: 0x0006A3A1 File Offset: 0x000685A1
		public bool CanPlace(ArchetypeInstance instance, int targetIndex)
		{
			return this.m_containerUI.ContainerInstance.CanPlace(instance, targetIndex);
		}

		// Token: 0x17000E6A RID: 3690
		// (get) Token: 0x06003E6B RID: 15979 RVA: 0x00052028 File Offset: 0x00050228
		public GameObject GO
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06003E6C RID: 15980 RVA: 0x0006A3B5 File Offset: 0x000685B5
		protected virtual ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_subscriberOnly || (SessionData.User != null && SessionData.User.IsSubscriber()))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, "This slot is reserved for subscribers.", false);
		}

		// Token: 0x17000E6B RID: 3691
		// (get) Token: 0x06003E6D RID: 15981 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E6C RID: 3692
		// (get) Token: 0x06003E6E RID: 15982 RVA: 0x0006A3E5 File Offset: 0x000685E5
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E6D RID: 3693
		// (get) Token: 0x06003E6F RID: 15983 RVA: 0x0006A3F4 File Offset: 0x000685F4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003E72 RID: 15986 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C92 RID: 15506
		protected const float kAlphaFade = 0.1f;

		// Token: 0x04003C93 RID: 15507
		protected ArchetypeInstance m_instance;

		// Token: 0x04003C94 RID: 15508
		protected IContainerUI m_containerUI;

		// Token: 0x04003C95 RID: 15509
		protected int m_index = -1;

		// Token: 0x04003C96 RID: 15510
		private static Color kFallbackFrameColor = new Color(0.22745098f, 0.22745098f, 0.22745098f);

		// Token: 0x04003C97 RID: 15511
		protected bool m_subscriberOnly;

		// Token: 0x04003C98 RID: 15512
		private Color? m_defaultFrameColor;

		// Token: 0x04003C99 RID: 15513
		private Color? m_frameColorOverride;

		// Token: 0x04003C9A RID: 15514
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C9B RID: 15515
		[SerializeField]
		protected RectTransform m_itemSlot;

		// Token: 0x04003C9C RID: 15516
		[SerializeField]
		protected Image m_background;

		// Token: 0x04003C9D RID: 15517
		[SerializeField]
		protected Image m_icon;

		// Token: 0x04003C9E RID: 15518
		[SerializeField]
		protected Image m_frame;

		// Token: 0x04003C9F RID: 15519
		[SerializeField]
		protected Image m_highlight;

		// Token: 0x04003CA0 RID: 15520
		[SerializeField]
		protected Image m_bgHighlight;

		// Token: 0x04003CA1 RID: 15521
		[SerializeField]
		protected Vector2 m_alphaRange = new Vector2(0f, 1f);
	}
}
