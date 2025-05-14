using System;
using System.Collections.Generic;
using SoL.Game.Notifications;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI.Notifications
{
	// Token: 0x02000962 RID: 2402
	public class NotificationsUI : MonoBehaviour
	{
		// Token: 0x0600472E RID: 18222 RVA: 0x0006FFA5 File Offset: 0x0006E1A5
		private void Awake()
		{
			this.m_pool = new Stack<NotificationItem>(32);
			this.m_shown = new List<NotificationItem>(32);
		}

		// Token: 0x0600472F RID: 18223 RVA: 0x001A5FC4 File Offset: 0x001A41C4
		private void Start()
		{
			this.m_maxNumberOfItems = Math.Max(0, (int)(this.m_thisRect.rect.height / this.m_notificationItem.RectTransform.rect.height));
		}

		// Token: 0x06004730 RID: 18224 RVA: 0x001A600C File Offset: 0x001A420C
		private void OnDestroy()
		{
			if (this.m_shown != null && ClientGameManager.NotificationsManager != null)
			{
				foreach (NotificationItem notificationItem in this.m_shown)
				{
					ClientGameManager.NotificationsManager.ArchiveNotification(notificationItem.Notification);
				}
			}
		}

		// Token: 0x06004731 RID: 18225 RVA: 0x001A6080 File Offset: 0x001A4280
		private void Update()
		{
			if (DateTime.UtcNow >= this.m_condenseTime)
			{
				bool flag = true;
				for (int i = 0; i < this.m_shown.Count; i++)
				{
					float num = this.PositionYForIndex(i);
					float num2 = this.PositionYForIndex(this.m_shown[i].FlowFromIndex);
					if (this.m_shown[i].RectTransform.localPosition.y != num)
					{
						flag = false;
						this.m_shown[i].FlowTimeProgress = Mathf.Min(this.m_shown[i].FlowTimeProgress + Time.deltaTime, this.m_flowDurationSeconds);
						float y = this.m_flowCurve.Evaluate(this.m_shown[i].FlowTimeProgress / this.m_flowDurationSeconds) * (num - num2) + num2;
						this.m_shown[i].RectTransform.localPosition = new Vector3(this.m_shown[i].RectTransform.localPosition.x, y, this.m_shown[i].RectTransform.localPosition.z);
					}
					else
					{
						this.m_shown[i].FlowTimeProgress = 0f;
						this.m_shown[i].FlowFromIndex = -1;
					}
				}
				if (flag)
				{
					this.m_condenseTime = DateTime.MaxValue;
					if (ClientGameManager.NotificationsManager != null)
					{
						ClientGameManager.NotificationsManager.HaltNotifications = (this.m_shown.Count == this.m_maxNumberOfItems);
					}
				}
			}
		}

		// Token: 0x06004732 RID: 18226 RVA: 0x001A6218 File Offset: 0x001A4418
		public void Purge()
		{
			if (this.m_shown != null)
			{
				foreach (NotificationItem notificationItem in this.m_shown)
				{
					notificationItem.gameObject.SetActive(false);
					this.m_pool.Push(notificationItem);
				}
				this.m_shown.Clear();
			}
		}

		// Token: 0x06004733 RID: 18227 RVA: 0x001A6290 File Offset: 0x001A4490
		public bool ShowNotification(Notification notification)
		{
			if (this.m_shown == null || this.m_pool == null)
			{
				return false;
			}
			if (this.m_shown.Count == this.m_maxNumberOfItems && this.m_shown.Count > 0)
			{
				this.OnExpire(this.m_shown[0]);
			}
			NotificationItem notificationItem;
			if (this.m_pool.Count == 0)
			{
				notificationItem = UnityEngine.Object.Instantiate<NotificationItem>(this.m_notificationItem, base.transform);
			}
			else
			{
				notificationItem = this.m_pool.Pop();
			}
			notificationItem.gameObject.SetActive(true);
			notificationItem.Init(notification, new Action<NotificationItem>(this.OnDismiss), new Action<NotificationItem>(this.OnExpire));
			notificationItem.RectTransform.localPosition = new Vector3(notificationItem.RectTransform.localPosition.x, this.PositionYForIndex(this.m_shown.Count), notificationItem.RectTransform.localPosition.z);
			this.m_shown.Add(notificationItem);
			return true;
		}

		// Token: 0x06004734 RID: 18228 RVA: 0x001A638C File Offset: 0x001A458C
		private void OnDismiss(NotificationItem item)
		{
			item.gameObject.SetActive(false);
			int num = -1;
			for (int i = 0; i < this.m_shown.Count; i++)
			{
				if (num != -1 && i > num && this.m_shown[i].FlowFromIndex == -1)
				{
					this.m_shown[i].FlowFromIndex = i;
				}
				else if (this.m_shown[i] == item)
				{
					num = i;
				}
			}
			this.m_shown.Remove(item);
			this.m_pool.Push(item);
			if (this.m_shown.Count > 0)
			{
				if (ClientGameManager.NotificationsManager != null)
				{
					ClientGameManager.NotificationsManager.HaltNotifications = true;
				}
				this.m_condenseTime = DateTime.UtcNow.AddSeconds((double)this.m_flowDelaySeconds);
			}
		}

		// Token: 0x06004735 RID: 18229 RVA: 0x0006FFC1 File Offset: 0x0006E1C1
		private void OnExpire(NotificationItem item)
		{
			if (ClientGameManager.NotificationsManager != null)
			{
				ClientGameManager.NotificationsManager.ArchiveNotification(item.Notification);
			}
			this.OnDismiss(item);
		}

		// Token: 0x06004736 RID: 18230 RVA: 0x001A645C File Offset: 0x001A465C
		private float PositionYForIndex(int index)
		{
			float height = this.m_notificationItem.RectTransform.rect.height;
			float num = height / 2f;
			if (this.m_flowDirection != NotificationFlowDirection.TopDown)
			{
				return this.m_thisRect.rect.yMin + height * (float)(index + 1) - num;
			}
			return this.m_thisRect.rect.yMax - height * (float)(index + 1) + num;
		}

		// Token: 0x04004317 RID: 17175
		[SerializeField]
		private RectTransform m_thisRect;

		// Token: 0x04004318 RID: 17176
		[SerializeField]
		private NotificationFlowDirection m_flowDirection;

		// Token: 0x04004319 RID: 17177
		[SerializeField]
		private AnimationCurve m_flowCurve;

		// Token: 0x0400431A RID: 17178
		[Min(0f)]
		[SerializeField]
		private float m_flowDurationSeconds = 0.2f;

		// Token: 0x0400431B RID: 17179
		[Min(0f)]
		[Tooltip("The delay between the last notification dismissal and the start of the flow animation.")]
		[SerializeField]
		private float m_flowDelaySeconds = 2f;

		// Token: 0x0400431C RID: 17180
		[SerializeField]
		private AnimationCurve m_showCurve;

		// Token: 0x0400431D RID: 17181
		[SerializeField]
		private NotificationItem m_notificationItem;

		// Token: 0x0400431E RID: 17182
		private int m_maxNumberOfItems;

		// Token: 0x0400431F RID: 17183
		private Stack<NotificationItem> m_pool;

		// Token: 0x04004320 RID: 17184
		private List<NotificationItem> m_shown;

		// Token: 0x04004321 RID: 17185
		private DateTime m_condenseTime = DateTime.MaxValue;
	}
}
