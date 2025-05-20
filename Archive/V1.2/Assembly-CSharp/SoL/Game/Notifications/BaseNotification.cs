using System;
using SoL.Game.Messages;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Notifications
{
	// Token: 0x02000840 RID: 2112
	[CreateAssetMenu(menuName = "SoL/Notifications/Base")]
	public class BaseNotification : Identifiable
	{
		// Token: 0x17000E0C RID: 3596
		// (get) Token: 0x06003D00 RID: 15616 RVA: 0x0006957B File Offset: 0x0006777B
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000E0D RID: 3597
		// (get) Token: 0x06003D01 RID: 15617 RVA: 0x00069583 File Offset: 0x00067783
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x17000E0E RID: 3598
		// (get) Token: 0x06003D02 RID: 15618 RVA: 0x0006958B File Offset: 0x0006778B
		public Sprite Icon
		{
			get
			{
				return this.m_icon;
			}
		}

		// Token: 0x17000E0F RID: 3599
		// (get) Token: 0x06003D03 RID: 15619 RVA: 0x00069593 File Offset: 0x00067793
		public Color IconTint
		{
			get
			{
				return this.m_iconTint;
			}
		}

		// Token: 0x17000E10 RID: 3600
		// (get) Token: 0x06003D04 RID: 15620 RVA: 0x0006959B File Offset: 0x0006779B
		public NotificationCategory Category
		{
			get
			{
				return this.m_category;
			}
		}

		// Token: 0x17000E11 RID: 3601
		// (get) Token: 0x06003D05 RID: 15621 RVA: 0x000695A3 File Offset: 0x000677A3
		public int Sort
		{
			get
			{
				return this.m_sort;
			}
		}

		// Token: 0x17000E12 RID: 3602
		// (get) Token: 0x06003D06 RID: 15622 RVA: 0x000695AB File Offset: 0x000677AB
		public NotificationPresentationFlags Presentation
		{
			get
			{
				return this.m_presentation;
			}
		}

		// Token: 0x17000E13 RID: 3603
		// (get) Token: 0x06003D07 RID: 15623 RVA: 0x000695B3 File Offset: 0x000677B3
		public float SidePanelExpirationSeconds
		{
			get
			{
				return this.m_sidePanelExpirationSeconds;
			}
		}

		// Token: 0x17000E14 RID: 3604
		// (get) Token: 0x06003D08 RID: 15624 RVA: 0x000695BB File Offset: 0x000677BB
		public float CenterToastDurationSeconds
		{
			get
			{
				return this.m_centerToastDurationSeconds;
			}
		}

		// Token: 0x17000E15 RID: 3605
		// (get) Token: 0x06003D09 RID: 15625 RVA: 0x000695C3 File Offset: 0x000677C3
		public MessageType PopupMessageType
		{
			get
			{
				return this.m_popupMessageType;
			}
		}

		// Token: 0x17000E16 RID: 3606
		// (get) Token: 0x06003D0A RID: 15626 RVA: 0x000695CB File Offset: 0x000677CB
		private bool m_usesSidePanel
		{
			get
			{
				return this.m_presentation.HasBitFlag(NotificationPresentationFlags.SidePanel);
			}
		}

		// Token: 0x17000E17 RID: 3607
		// (get) Token: 0x06003D0B RID: 15627 RVA: 0x000695D9 File Offset: 0x000677D9
		private bool m_usesCenterToast
		{
			get
			{
				return this.m_presentation.HasBitFlag(NotificationPresentationFlags.CenterToast);
			}
		}

		// Token: 0x17000E18 RID: 3608
		// (get) Token: 0x06003D0C RID: 15628 RVA: 0x000695E7 File Offset: 0x000677E7
		private bool m_usesPopup
		{
			get
			{
				return this.m_presentation.HasBitFlag(NotificationPresentationFlags.TutorialPopup);
			}
		}

		// Token: 0x17000E19 RID: 3609
		// (get) Token: 0x06003D0D RID: 15629 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool CanOpen
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003D0E RID: 15630 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Open(object data = null)
		{
		}

		// Token: 0x04003BD8 RID: 15320
		[SerializeField]
		private string m_title;

		// Token: 0x04003BD9 RID: 15321
		[Multiline]
		[SerializeField]
		private string m_description;

		// Token: 0x04003BDA RID: 15322
		[SerializeField]
		private Sprite m_icon;

		// Token: 0x04003BDB RID: 15323
		[SerializeField]
		private Color m_iconTint = Color.white;

		// Token: 0x04003BDC RID: 15324
		[SerializeField]
		private NotificationCategory m_category;

		// Token: 0x04003BDD RID: 15325
		[SerializeField]
		private int m_sort = -1;

		// Token: 0x04003BDE RID: 15326
		[SerializeField]
		private NotificationPresentationFlags m_presentation;

		// Token: 0x04003BDF RID: 15327
		[Tooltip("Number of seconds before this notification will dismiss itself. 0 for no expiration.")]
		[SerializeField]
		private float m_sidePanelExpirationSeconds;

		// Token: 0x04003BE0 RID: 15328
		[SerializeField]
		private float m_centerToastDurationSeconds;

		// Token: 0x04003BE1 RID: 15329
		[SerializeField]
		private MessageType m_popupMessageType = MessageType.Notification;
	}
}
