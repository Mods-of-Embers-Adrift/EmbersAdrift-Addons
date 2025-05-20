using System;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C2 RID: 2498
	public class ArchetypeInstanceUIDragShadow : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004C00 RID: 19456 RVA: 0x001BBA34 File Offset: 0x001B9C34
		private void Awake()
		{
			this.m_canvasGroup.alpha = 0f;
			this.m_canvasGroup.blocksRaycasts = false;
			this.m_originalParent = this.m_rectTransform.parent;
			this.m_originalPivot = this.m_rectTransform.pivot;
			this.m_originalPosition = this.m_rectTransform.anchoredPosition;
		}

		// Token: 0x06004C01 RID: 19457 RVA: 0x00073662 File Offset: 0x00071862
		private void Update()
		{
			if (this.m_isEnabled && !this.m_instanceUi)
			{
				this.DisableInternal();
			}
		}

		// Token: 0x06004C02 RID: 19458 RVA: 0x001BBA90 File Offset: 0x001B9C90
		public void Enable(ArchetypeInstanceUI instanceUi)
		{
			if (instanceUi == null)
			{
				this.DisableInternal();
				return;
			}
			this.m_instanceUi = instanceUi;
			this.m_image.overrideSprite = instanceUi.Instance.Archetype.Icon;
			this.m_image.color = instanceUi.Instance.Archetype.IconTint;
			this.m_rectTransform.SetPivot(instanceUi.RectTransform.pivot);
			this.m_rectTransform.SetParent(instanceUi.RectTransform.parent);
			this.m_rectTransform.anchoredPosition = instanceUi.RectTransform.anchoredPosition;
			this.m_canvasGroup.alpha = 1f;
			this.m_canvasGroup.blocksRaycasts = true;
			this.m_isEnabled = true;
		}

		// Token: 0x06004C03 RID: 19459 RVA: 0x0007367F File Offset: 0x0007187F
		public void Disable(ArchetypeInstanceUI instanceUi)
		{
			if (this.m_instanceUi != instanceUi)
			{
				return;
			}
			this.DisableInternal();
		}

		// Token: 0x06004C04 RID: 19460 RVA: 0x001BBB50 File Offset: 0x001B9D50
		private void DisableInternal()
		{
			this.m_instanceUi = null;
			this.m_canvasGroup.alpha = 0f;
			this.m_canvasGroup.blocksRaycasts = false;
			this.m_image.overrideSprite = null;
			this.m_rectTransform.SetParent(this.m_originalParent);
			this.m_rectTransform.SetPivot(this.m_originalPivot);
			this.m_rectTransform.anchoredPosition = this.m_originalPosition;
			this.m_isEnabled = false;
		}

		// Token: 0x06004C05 RID: 19461 RVA: 0x001BBBC8 File Offset: 0x001B9DC8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instanceUi == null || this.m_instanceUi.Instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instanceUi.Instance,
				AdditionalText = "Currently Dragging"
			};
		}

		// Token: 0x170010C5 RID: 4293
		// (get) Token: 0x06004C06 RID: 19462 RVA: 0x00073696 File Offset: 0x00071896
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170010C6 RID: 4294
		// (get) Token: 0x06004C07 RID: 19463 RVA: 0x001BA874 File Offset: 0x001B8A74
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return default(TooltipSettings);
			}
		}

		// Token: 0x170010C7 RID: 4295
		// (get) Token: 0x06004C08 RID: 19464 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004C0A RID: 19466 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004631 RID: 17969
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04004632 RID: 17970
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x04004633 RID: 17971
		[SerializeField]
		private Image m_image;

		// Token: 0x04004634 RID: 17972
		private bool m_isEnabled;

		// Token: 0x04004635 RID: 17973
		private Transform m_originalParent;

		// Token: 0x04004636 RID: 17974
		private Vector2 m_originalPivot;

		// Token: 0x04004637 RID: 17975
		private Vector2 m_originalPosition;

		// Token: 0x04004638 RID: 17976
		private ArchetypeInstanceUI m_instanceUi;
	}
}
