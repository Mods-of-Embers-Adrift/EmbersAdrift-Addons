using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008B9 RID: 2233
	public class NotificationIndicator : MonoBehaviour
	{
		// Token: 0x06004168 RID: 16744 RVA: 0x0006C2DA File Offset: 0x0006A4DA
		private void Start()
		{
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.LocalPlayerDestroyed += this.LocalPlayerOnLocalPlayerDestroyed;
		}

		// Token: 0x06004169 RID: 16745 RVA: 0x0006C2FE File Offset: 0x0006A4FE
		private void OnDestroy()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.LocalPlayerDestroyed -= this.LocalPlayerOnLocalPlayerDestroyed;
		}

		// Token: 0x0600416A RID: 16746 RVA: 0x0018F380 File Offset: 0x0018D580
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.UnreadMailUpdated += this.SocialManagerOnUnreadMailUpdated;
			}
			if (ClientGameManager.NotificationsManager)
			{
				ClientGameManager.NotificationsManager.ArchiveChanged += this.NotificationsManagerOnArchiveChanged;
			}
			this.RefreshIndicator();
		}

		// Token: 0x0600416B RID: 16747 RVA: 0x0018F3D8 File Offset: 0x0018D5D8
		private void LocalPlayerOnLocalPlayerDestroyed()
		{
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.UnreadMailUpdated -= this.SocialManagerOnUnreadMailUpdated;
			}
			if (ClientGameManager.NotificationsManager)
			{
				ClientGameManager.NotificationsManager.ArchiveChanged -= this.NotificationsManagerOnArchiveChanged;
			}
		}

		// Token: 0x0600416C RID: 16748 RVA: 0x0006C322 File Offset: 0x0006A522
		private void SocialManagerOnUnreadMailUpdated()
		{
			this.RefreshIndicator();
		}

		// Token: 0x0600416D RID: 16749 RVA: 0x0006C322 File Offset: 0x0006A522
		private void NotificationsManagerOnArchiveChanged()
		{
			this.RefreshIndicator();
		}

		// Token: 0x0600416E RID: 16750 RVA: 0x0018F42C File Offset: 0x0018D62C
		private void RefreshIndicator()
		{
			if (this.m_image)
			{
				bool flag = ClientGameManager.SocialManager && ClientGameManager.SocialManager.HasUnreadMail && !ClientGameManager.SocialManager.UnreadMailNotificationDismissed;
				bool flag2 = ClientGameManager.NotificationsManager && ClientGameManager.NotificationsManager.Archive != null && ClientGameManager.NotificationsManager.Archive.Count > 0;
				this.m_image.enabled = (flag || flag2);
			}
		}

		// Token: 0x04003EC3 RID: 16067
		[SerializeField]
		private Image m_image;
	}
}
