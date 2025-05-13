using System;
using SoL.Game.Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Notifications
{
	// Token: 0x0200095D RID: 2397
	public class NotificationItem : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x17000FCB RID: 4043
		// (get) Token: 0x0600470B RID: 18187 RVA: 0x0006FDAB File Offset: 0x0006DFAB
		public RectTransform RectTransform
		{
			get
			{
				return this.m_thisRect;
			}
		}

		// Token: 0x17000FCC RID: 4044
		// (get) Token: 0x0600470C RID: 18188 RVA: 0x0006FDB3 File Offset: 0x0006DFB3
		public Notification Notification
		{
			get
			{
				return this.m_notification;
			}
		}

		// Token: 0x17000FCD RID: 4045
		// (get) Token: 0x0600470D RID: 18189 RVA: 0x0006FDBB File Offset: 0x0006DFBB
		// (set) Token: 0x0600470E RID: 18190 RVA: 0x0006FDC3 File Offset: 0x0006DFC3
		public float FlowTimeProgress { get; set; }

		// Token: 0x17000FCE RID: 4046
		// (get) Token: 0x0600470F RID: 18191 RVA: 0x0006FDCC File Offset: 0x0006DFCC
		// (set) Token: 0x06004710 RID: 18192 RVA: 0x0006FDD4 File Offset: 0x0006DFD4
		public int FlowFromIndex { get; set; } = -1;

		// Token: 0x06004711 RID: 18193 RVA: 0x001A5B14 File Offset: 0x001A3D14
		private void Start()
		{
			this.m_maxWidth = this.m_thisRect.rect.width;
			this.m_minWidth = this.m_minWidthRect.rect.width;
		}

		// Token: 0x06004712 RID: 18194 RVA: 0x001A5B54 File Offset: 0x001A3D54
		private void Update()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (!(utcNow >= this.m_expirationTime))
			{
				if (this.m_fadeInTimeProgress < this.m_fadeInDurationSeconds)
				{
					this.m_fadeInTimeProgress = Mathf.Min(this.m_fadeInTimeProgress + Time.deltaTime, this.m_fadeInDurationSeconds);
					this.m_canvasGroup.alpha = this.m_fadeInCurve.Evaluate(this.m_fadeInTimeProgress / this.m_fadeInDurationSeconds);
				}
				if (utcNow >= this.m_shrinkTime)
				{
					float width = this.m_thisRect.rect.width;
					if (this.m_hovered && width != this.m_maxWidth)
					{
						this.m_shrinkTimeProgress = Mathf.Min(this.m_shrinkTimeProgress + Time.deltaTime, this.m_hoverAnimationDurationSeconds);
						float size = this.m_hoverCurve.Evaluate(this.m_shrinkTimeProgress / this.m_hoverAnimationDurationSeconds) * (this.m_maxWidth - this.m_minWidth) + this.m_minWidth;
						this.m_thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
						return;
					}
					if (!this.m_hovered && width != this.m_minWidth)
					{
						this.m_shrinkTimeProgress = Mathf.Max(this.m_shrinkTimeProgress - Time.deltaTime, 0f);
						float size2 = this.m_hoverCurve.Evaluate(this.m_shrinkTimeProgress / this.m_hoverAnimationDurationSeconds) * (this.m_maxWidth - this.m_minWidth) + this.m_minWidth;
						this.m_thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size2);
					}
				}
				return;
			}
			Action<NotificationItem> expireCallback = this.m_expireCallback;
			if (expireCallback == null)
			{
				return;
			}
			expireCallback(this);
		}

		// Token: 0x06004713 RID: 18195 RVA: 0x001A5CD0 File Offset: 0x001A3ED0
		public void Init(Notification notification, Action<NotificationItem> dismissCallback, Action<NotificationItem> expireCallback)
		{
			DateTime utcNow = DateTime.UtcNow;
			this.m_message.text = notification.TitleWithData;
			this.m_icon.sprite = notification.Type.Icon;
			this.m_icon.color = notification.Type.IconTint;
			this.m_hovered = false;
			this.m_notification = notification;
			this.m_dismissCallback = dismissCallback;
			this.m_expireCallback = expireCallback;
			this.m_expirationTime = ((notification.Type.SidePanelExpirationSeconds != 0f) ? utcNow.AddSeconds((double)notification.Type.SidePanelExpirationSeconds) : DateTime.MaxValue);
			this.m_fadeInTimeProgress = 0f;
			this.m_canvasGroup.alpha = 0f;
			this.FlowTimeProgress = 0f;
			this.FlowFromIndex = -1;
			if (!float.IsNaN(this.m_maxWidth))
			{
				this.m_thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.m_maxWidth);
			}
			this.m_shrinkTime = utcNow.AddSeconds((double)this.m_initialFullWidthDelaySeconds);
			this.m_shrinkTimeProgress = this.m_hoverAnimationDurationSeconds;
		}

		// Token: 0x06004714 RID: 18196 RVA: 0x0006FDDD File Offset: 0x0006DFDD
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.m_notification.Type.Open(this.m_notification.OpenData);
			}
			Action<NotificationItem> dismissCallback = this.m_dismissCallback;
			if (dismissCallback == null)
			{
				return;
			}
			dismissCallback(this);
		}

		// Token: 0x06004715 RID: 18197 RVA: 0x0006FE13 File Offset: 0x0006E013
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.m_hovered = true;
		}

		// Token: 0x06004716 RID: 18198 RVA: 0x0006FE1C File Offset: 0x0006E01C
		public void OnPointerExit(PointerEventData eventData)
		{
			this.m_hovered = false;
		}

		// Token: 0x040042F3 RID: 17139
		[SerializeField]
		private RectTransform m_thisRect;

		// Token: 0x040042F4 RID: 17140
		[SerializeField]
		private RectTransform m_minWidthRect;

		// Token: 0x040042F5 RID: 17141
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x040042F6 RID: 17142
		[SerializeField]
		private TextMeshProUGUI m_message;

		// Token: 0x040042F7 RID: 17143
		[SerializeField]
		private Image m_icon;

		// Token: 0x040042F8 RID: 17144
		[SerializeField]
		private AnimationCurve m_hoverCurve;

		// Token: 0x040042F9 RID: 17145
		[Min(0f)]
		[SerializeField]
		private float m_hoverAnimationDurationSeconds = 0.2f;

		// Token: 0x040042FA RID: 17146
		[Min(0f)]
		[Tooltip("The delay between the notification being shown and shrinking width.")]
		[SerializeField]
		private float m_initialFullWidthDelaySeconds = 5f;

		// Token: 0x040042FB RID: 17147
		[SerializeField]
		private AnimationCurve m_fadeInCurve;

		// Token: 0x040042FC RID: 17148
		[Min(0f)]
		[SerializeField]
		private float m_fadeInDurationSeconds = 0.2f;

		// Token: 0x040042FD RID: 17149
		private float m_maxWidth = float.NaN;

		// Token: 0x040042FE RID: 17150
		private float m_minWidth = float.NaN;

		// Token: 0x040042FF RID: 17151
		private bool m_hovered;

		// Token: 0x04004300 RID: 17152
		private Notification m_notification;

		// Token: 0x04004301 RID: 17153
		private Action<NotificationItem> m_dismissCallback;

		// Token: 0x04004302 RID: 17154
		private Action<NotificationItem> m_expireCallback;

		// Token: 0x04004303 RID: 17155
		private DateTime m_expirationTime = DateTime.MaxValue;

		// Token: 0x04004304 RID: 17156
		private DateTime m_shrinkTime = DateTime.MaxValue;

		// Token: 0x04004305 RID: 17157
		private float m_fadeInTimeProgress;

		// Token: 0x04004306 RID: 17158
		private float m_shrinkTimeProgress;
	}
}
