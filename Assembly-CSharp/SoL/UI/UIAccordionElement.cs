using System;
using System.Collections;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000391 RID: 913
	public class UIAccordionElement : SolToggle
	{
		// Token: 0x060018F9 RID: 6393 RVA: 0x00105BC0 File Offset: 0x00103DC0
		protected override void Awake()
		{
			base.Awake();
			this.m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			this.m_layoutElement = base.gameObject.GetComponent<LayoutElement>();
			this.m_accordion = base.gameObject.GetComponentInParent<UIAccordion>();
			if (Application.isPlaying)
			{
				if (this.m_useGroup)
				{
					base.group = this.m_accordion;
				}
				this.m_layoutElement.preferredHeight = this.m_minHeight;
				this.m_currentState = base.isOn;
				this.SetToggleController();
				this.ToggleContent(false);
				this.Refresh(true);
				this.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			}
		}

		// Token: 0x060018FA RID: 6394 RVA: 0x000537C8 File Offset: 0x000519C8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x000537E7 File Offset: 0x000519E7
		public void DisableElement()
		{
			if (this.m_content != null)
			{
				this.m_content.SetActive(false);
			}
			if (this.m_toggleController != null)
			{
				this.m_toggleController.gameObject.SetActive(false);
			}
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x00053822 File Offset: 0x00051A22
		private void ToggleContent(bool updateCanvas)
		{
			if (this.m_content != null)
			{
				this.m_content.SetActive(this.m_currentState);
				if (updateCanvas)
				{
					Canvas.ForceUpdateCanvases();
				}
			}
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x0005384B File Offset: 0x00051A4B
		private void SetToggleController()
		{
			if (this.m_toggleController != null)
			{
				this.m_toggleController.State = (this.m_currentState ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
			}
		}

		// Token: 0x060018FE RID: 6398 RVA: 0x00053872 File Offset: 0x00051A72
		private void ToggleChanged(bool state)
		{
			if (this.m_currentState != state)
			{
				this.m_currentState = state;
				this.SetToggleController();
				if (this.m_currentState)
				{
					this.ToggleContent(true);
				}
				this.Refresh(false);
			}
		}

		// Token: 0x060018FF RID: 6399 RVA: 0x00105C6C File Offset: 0x00103E6C
		private void Refresh(bool instant)
		{
			if (instant)
			{
				this.m_layoutElement.preferredHeight = (this.m_currentState ? -1f : this.m_minHeight);
				return;
			}
			float a;
			float b;
			if (this.m_currentState)
			{
				a = this.m_minHeight;
				b = this.m_rectTransform.GetExpandedHeight(this.m_layoutElement);
			}
			else
			{
				a = this.m_rectTransform.rect.height;
				b = this.m_minHeight;
			}
			if (this.m_resizeCo != null)
			{
				base.StopCoroutine(this.m_resizeCo);
			}
			this.m_resizeCo = this.ResizeCo(a, b);
			base.StartCoroutine(this.m_resizeCo);
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x000538A0 File Offset: 0x00051AA0
		private IEnumerator ResizeCo(float a, float b)
		{
			float t = 0f;
			while (t < this.m_accordion.Duration)
			{
				float preferredHeight = Mathf.Lerp(a, b, t / this.m_accordion.Duration);
				this.m_layoutElement.preferredHeight = preferredHeight;
				t += Time.deltaTime;
				yield return null;
			}
			this.m_layoutElement.preferredHeight = b;
			this.m_resizeCo = null;
			if (!this.m_currentState)
			{
				this.ToggleContent(false);
			}
			yield break;
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x00105D0C File Offset: 0x00103F0C
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!base.enabled)
			{
				return;
			}
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (eventData.pointerCurrentRaycast.gameObject != base.image.gameObject)
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x04002008 RID: 8200
		private RectTransform m_rectTransform;

		// Token: 0x04002009 RID: 8201
		private LayoutElement m_layoutElement;

		// Token: 0x0400200A RID: 8202
		private UIAccordion m_accordion;

		// Token: 0x0400200B RID: 8203
		private IEnumerator m_resizeCo;

		// Token: 0x0400200C RID: 8204
		private bool m_currentState;

		// Token: 0x0400200D RID: 8205
		[SerializeField]
		private float m_minHeight = 10f;

		// Token: 0x0400200E RID: 8206
		[SerializeField]
		private bool m_useGroup = true;

		// Token: 0x0400200F RID: 8207
		[SerializeField]
		private GameObject m_content;

		// Token: 0x04002010 RID: 8208
		[SerializeField]
		private ToggleController m_toggleController;
	}
}
