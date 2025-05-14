using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000209 RID: 521
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/BoxSlider")]
	public class BoxSlider : Selectable, IDragHandler, IEventSystemHandler, IInitializePotentialDragHandler, ICanvasElement
	{
		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06001184 RID: 4484 RVA: 0x0004E7E5 File Offset: 0x0004C9E5
		// (set) Token: 0x06001185 RID: 4485 RVA: 0x0004E7ED File Offset: 0x0004C9ED
		public RectTransform HandleRect
		{
			get
			{
				return this.m_HandleRect;
			}
			set
			{
				if (BoxSlider.SetClass<RectTransform>(ref this.m_HandleRect, value))
				{
					this.UpdateCachedReferences();
					this.UpdateVisuals();
				}
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001186 RID: 4486 RVA: 0x0004E809 File Offset: 0x0004CA09
		// (set) Token: 0x06001187 RID: 4487 RVA: 0x0004E811 File Offset: 0x0004CA11
		public float MinValue
		{
			get
			{
				return this.m_MinValue;
			}
			set
			{
				if (BoxSlider.SetStruct<float>(ref this.m_MinValue, value))
				{
					this.SetX(this.m_ValueX);
					this.SetY(this.m_ValueY);
					this.UpdateVisuals();
				}
			}
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06001188 RID: 4488 RVA: 0x0004E83F File Offset: 0x0004CA3F
		// (set) Token: 0x06001189 RID: 4489 RVA: 0x0004E847 File Offset: 0x0004CA47
		public float MaxValue
		{
			get
			{
				return this.m_MaxValue;
			}
			set
			{
				if (BoxSlider.SetStruct<float>(ref this.m_MaxValue, value))
				{
					this.SetX(this.m_ValueX);
					this.SetY(this.m_ValueY);
					this.UpdateVisuals();
				}
			}
		}

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x0600118A RID: 4490 RVA: 0x0004E875 File Offset: 0x0004CA75
		// (set) Token: 0x0600118B RID: 4491 RVA: 0x0004E87D File Offset: 0x0004CA7D
		public bool WholeNumbers
		{
			get
			{
				return this.m_WholeNumbers;
			}
			set
			{
				if (BoxSlider.SetStruct<bool>(ref this.m_WholeNumbers, value))
				{
					this.SetX(this.m_ValueX);
					this.SetY(this.m_ValueY);
					this.UpdateVisuals();
				}
			}
		}

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x0600118C RID: 4492 RVA: 0x0004E8AB File Offset: 0x0004CAAB
		// (set) Token: 0x0600118D RID: 4493 RVA: 0x0004E8C7 File Offset: 0x0004CAC7
		public float ValueX
		{
			get
			{
				if (this.WholeNumbers)
				{
					return Mathf.Round(this.m_ValueX);
				}
				return this.m_ValueX;
			}
			set
			{
				this.SetX(value);
			}
		}

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x0600118E RID: 4494 RVA: 0x0004E8D0 File Offset: 0x0004CAD0
		// (set) Token: 0x0600118F RID: 4495 RVA: 0x0004E902 File Offset: 0x0004CB02
		public float NormalizedValueX
		{
			get
			{
				if (Mathf.Approximately(this.MinValue, this.MaxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(this.MinValue, this.MaxValue, this.ValueX);
			}
			set
			{
				this.ValueX = Mathf.Lerp(this.MinValue, this.MaxValue, value);
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06001190 RID: 4496 RVA: 0x0004E91C File Offset: 0x0004CB1C
		// (set) Token: 0x06001191 RID: 4497 RVA: 0x0004E938 File Offset: 0x0004CB38
		public float ValueY
		{
			get
			{
				if (this.WholeNumbers)
				{
					return Mathf.Round(this.m_ValueY);
				}
				return this.m_ValueY;
			}
			set
			{
				this.SetY(value);
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06001192 RID: 4498 RVA: 0x0004E941 File Offset: 0x0004CB41
		// (set) Token: 0x06001193 RID: 4499 RVA: 0x0004E973 File Offset: 0x0004CB73
		public float NormalizedValueY
		{
			get
			{
				if (Mathf.Approximately(this.MinValue, this.MaxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(this.MinValue, this.MaxValue, this.ValueY);
			}
			set
			{
				this.ValueY = Mathf.Lerp(this.MinValue, this.MaxValue, value);
			}
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001194 RID: 4500 RVA: 0x0004E98D File Offset: 0x0004CB8D
		// (set) Token: 0x06001195 RID: 4501 RVA: 0x0004E995 File Offset: 0x0004CB95
		public BoxSlider.BoxSliderEvent OnValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06001196 RID: 4502 RVA: 0x0004E99E File Offset: 0x0004CB9E
		private float StepSize
		{
			get
			{
				if (!this.WholeNumbers)
				{
					return (this.MaxValue - this.MinValue) * 0.1f;
				}
				return 1f;
			}
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x0004E9C1 File Offset: 0x0004CBC1
		protected BoxSlider()
		{
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x0004475B File Offset: 0x0004295B
		public void LayoutComplete()
		{
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0004475B File Offset: 0x0004295B
		public void GraphicUpdateComplete()
		{
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x000E3C68 File Offset: 0x000E1E68
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x0004EA00 File Offset: 0x0004CC00
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x0004EA20 File Offset: 0x0004CC20
		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateCachedReferences();
			this.SetX(this.m_ValueX, false);
			this.SetY(this.m_ValueY, false);
			this.UpdateVisuals();
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x0004EA4E File Offset: 0x0004CC4E
		protected override void OnDisable()
		{
			this.m_Tracker.Clear();
			base.OnDisable();
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x000E3CB8 File Offset: 0x000E1EB8
		private void UpdateCachedReferences()
		{
			if (this.m_HandleRect)
			{
				this.m_HandleTransform = this.m_HandleRect.transform;
				if (this.m_HandleTransform.parent != null)
				{
					this.m_HandleContainerRect = this.m_HandleTransform.parent.GetComponent<RectTransform>();
					return;
				}
			}
			else
			{
				this.m_HandleContainerRect = null;
			}
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x0004EA61 File Offset: 0x0004CC61
		private void SetX(float input)
		{
			this.SetX(input, true);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x000E3D14 File Offset: 0x000E1F14
		private void SetX(float input, bool sendCallback)
		{
			float num = Mathf.Clamp(input, this.MinValue, this.MaxValue);
			if (this.WholeNumbers)
			{
				num = Mathf.Round(num);
			}
			if (this.m_ValueX == num)
			{
				return;
			}
			this.m_ValueX = num;
			this.UpdateVisuals();
			if (sendCallback)
			{
				this.m_OnValueChanged.Invoke(num, this.ValueY);
			}
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x0004EA6B File Offset: 0x0004CC6B
		private void SetY(float input)
		{
			this.SetY(input, true);
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x000E3D70 File Offset: 0x000E1F70
		private void SetY(float input, bool sendCallback)
		{
			float num = Mathf.Clamp(input, this.MinValue, this.MaxValue);
			if (this.WholeNumbers)
			{
				num = Mathf.Round(num);
			}
			if (this.m_ValueY == num)
			{
				return;
			}
			this.m_ValueY = num;
			this.UpdateVisuals();
			if (sendCallback)
			{
				this.m_OnValueChanged.Invoke(this.ValueX, num);
			}
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x0004EA75 File Offset: 0x0004CC75
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			this.UpdateVisuals();
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x000E3DCC File Offset: 0x000E1FCC
		private void UpdateVisuals()
		{
			this.m_Tracker.Clear();
			if (this.m_HandleContainerRect != null)
			{
				this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 zero = Vector2.zero;
				Vector2 one = Vector2.one;
				zero[0] = (one[0] = this.NormalizedValueX);
				zero[1] = (one[1] = this.NormalizedValueY);
				if (Application.isPlaying)
				{
					this.m_HandleRect.anchorMin = zero;
					this.m_HandleRect.anchorMax = one;
				}
			}
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x000E3E68 File Offset: 0x000E2068
		private void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			RectTransform handleContainerRect = this.m_HandleContainerRect;
			if (handleContainerRect != null && handleContainerRect.rect.size[0] > 0f)
			{
				Vector2 a;
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(handleContainerRect, eventData.position, cam, out a))
				{
					return;
				}
				a -= handleContainerRect.rect.position;
				float normalizedValueX = Mathf.Clamp01((a - this.m_Offset)[0] / handleContainerRect.rect.size[0]);
				this.NormalizedValueX = normalizedValueX;
				float normalizedValueY = Mathf.Clamp01((a - this.m_Offset)[1] / handleContainerRect.rect.size[1]);
				this.NormalizedValueY = normalizedValueY;
			}
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0004EA83 File Offset: 0x0004CC83
		private bool CanDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x000E3F48 File Offset: 0x000E2148
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.CanDrag(eventData))
			{
				return;
			}
			base.OnPointerDown(eventData);
			this.m_Offset = Vector2.zero;
			if (this.m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.position, eventData.enterEventCamera))
			{
				Vector2 offset;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.position, eventData.pressEventCamera, out offset))
				{
					this.m_Offset = offset;
				}
				this.m_Offset.y = -this.m_Offset.y;
				return;
			}
			this.UpdateDrag(eventData, eventData.pressEventCamera);
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x0004EAA0 File Offset: 0x0004CCA0
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.CanDrag(eventData))
			{
				return;
			}
			this.UpdateDrag(eventData, eventData.pressEventCamera);
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x0004EAB9 File Offset: 0x0004CCB9
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x0004EAC2 File Offset: 0x0004CCC2
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x04000F5C RID: 3932
		[SerializeField]
		private RectTransform m_HandleRect;

		// Token: 0x04000F5D RID: 3933
		[Space(6f)]
		[SerializeField]
		private float m_MinValue;

		// Token: 0x04000F5E RID: 3934
		[SerializeField]
		private float m_MaxValue = 1f;

		// Token: 0x04000F5F RID: 3935
		[SerializeField]
		private bool m_WholeNumbers;

		// Token: 0x04000F60 RID: 3936
		[SerializeField]
		private float m_ValueX = 1f;

		// Token: 0x04000F61 RID: 3937
		[SerializeField]
		private float m_ValueY = 1f;

		// Token: 0x04000F62 RID: 3938
		[Space(6f)]
		[SerializeField]
		private BoxSlider.BoxSliderEvent m_OnValueChanged = new BoxSlider.BoxSliderEvent();

		// Token: 0x04000F63 RID: 3939
		private Transform m_HandleTransform;

		// Token: 0x04000F64 RID: 3940
		private RectTransform m_HandleContainerRect;

		// Token: 0x04000F65 RID: 3941
		private Vector2 m_Offset = Vector2.zero;

		// Token: 0x04000F66 RID: 3942
		private DrivenRectTransformTracker m_Tracker;

		// Token: 0x0200020A RID: 522
		public enum Direction
		{
			// Token: 0x04000F68 RID: 3944
			LeftToRight,
			// Token: 0x04000F69 RID: 3945
			RightToLeft,
			// Token: 0x04000F6A RID: 3946
			BottomToTop,
			// Token: 0x04000F6B RID: 3947
			TopToBottom
		}

		// Token: 0x0200020B RID: 523
		[Serializable]
		public class BoxSliderEvent : UnityEvent<float, float>
		{
		}

		// Token: 0x0200020C RID: 524
		private enum Axis
		{
			// Token: 0x04000F6D RID: 3949
			Horizontal,
			// Token: 0x04000F6E RID: 3950
			Vertical
		}
	}
}
