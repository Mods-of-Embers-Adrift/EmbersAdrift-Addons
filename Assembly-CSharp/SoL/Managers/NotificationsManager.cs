using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Discovery;
using SoL.Game.Interactives;
using SoL.Game.Notifications;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Game.UI.Notifications;
using SoL.Networking.Database;
using SoL.UI;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x0200050A RID: 1290
	public class NotificationsManager : MonoBehaviour
	{
		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x060024D5 RID: 9429 RVA: 0x0005A5DF File Offset: 0x000587DF
		public List<Notification> Archive
		{
			get
			{
				return this.m_archive;
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x060024D6 RID: 9430 RVA: 0x0005A5E7 File Offset: 0x000587E7
		// (set) Token: 0x060024D7 RID: 9431 RVA: 0x0005A5EF File Offset: 0x000587EF
		public bool HaltNotifications { get; set; }

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x060024D8 RID: 9432 RVA: 0x0012ECE0 File Offset: 0x0012CEE0
		// (remove) Token: 0x060024D9 RID: 9433 RVA: 0x0012ED18 File Offset: 0x0012CF18
		public event Action ArchiveChanged;

		// Token: 0x060024DA RID: 9434 RVA: 0x0012ED50 File Offset: 0x0012CF50
		public void Reset()
		{
			UIManager uimanager = ClientGameManager.UIManager;
			if (uimanager != null)
			{
				NotificationsUI notificationsUI = uimanager.NotificationsUI;
				if (notificationsUI != null)
				{
					notificationsUI.Purge();
				}
			}
			this.m_pendingQueue.Clear();
			this.m_archive.Clear();
			this.m_nextNotificationShowTime = null;
			this.HaltNotifications = false;
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x0012EDA4 File Offset: 0x0012CFA4
		private void Start()
		{
			ClientGameManager.SocialManager.NewFriendRequestReceived += this.OnNewFriendRequest;
			ClientGameManager.SocialManager.NewGuildInviteReceived += this.OnNewGuildInvite;
			ClientGameManager.SocialManager.UnreadMailUpdated += this.OnUnreadMailUpdated;
			Options.GameOptions.HideTutorialPopups.Changed += this.OnHideTutorialsChanged;
			this.OnHideTutorialsChanged();
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x0012EE10 File Offset: 0x0012D010
		private void OnDestroy()
		{
			ClientGameManager.SocialManager.NewFriendRequestReceived -= this.OnNewFriendRequest;
			ClientGameManager.SocialManager.NewGuildInviteReceived -= this.OnNewGuildInvite;
			ClientGameManager.SocialManager.UnreadMailUpdated -= this.OnUnreadMailUpdated;
			Options.GameOptions.HideTutorialPopups.Changed -= this.OnHideTutorialsChanged;
			LocalPlayer.LocalPlayerOffensiveTargetChanged -= this.OnSelectAggressiveTarget;
			LocalPlayer.LocalPlayerHealthStateUpdated -= this.OnHealthStateChanged;
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.OnInventoryChanged;
			}
			DiscoveryProgression.DiscoveryFound -= this.OnDiscoveryFound;
		}

		// Token: 0x060024DD RID: 9437 RVA: 0x0012EED4 File Offset: 0x0012D0D4
		private void Update()
		{
			if (LocalPlayer.GameEntity != null && this.m_nextNotificationShowTime != null && !this.HaltNotifications)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (this.m_nextNotificationShowTime <= utcNow)
				{
					this.ShowNotification(utcNow);
				}
			}
		}

		// Token: 0x060024DE RID: 9438 RVA: 0x0012EF34 File Offset: 0x0012D134
		public void EnqueueNotification(Notification notification)
		{
			this.m_pendingQueue.Enqueue(notification);
			if (this.m_nextNotificationShowTime == null)
			{
				this.m_nextNotificationShowTime = new DateTime?(DateTime.UtcNow.AddSeconds(2.0));
			}
		}

		// Token: 0x060024DF RID: 9439 RVA: 0x0005A5F8 File Offset: 0x000587F8
		public void ArchiveNotification(Notification notification)
		{
			this.m_archive.Add(notification);
			Action archiveChanged = this.ArchiveChanged;
			if (archiveChanged == null)
			{
				return;
			}
			archiveChanged();
		}

		// Token: 0x060024E0 RID: 9440 RVA: 0x0005A616 File Offset: 0x00058816
		public void DismissNotification(Notification notification)
		{
			this.m_archive.Remove(notification);
			Action archiveChanged = this.ArchiveChanged;
			if (archiveChanged == null)
			{
				return;
			}
			archiveChanged();
		}

		// Token: 0x060024E1 RID: 9441 RVA: 0x0005A635 File Offset: 0x00058835
		public void ClearArchived()
		{
			this.m_archive.Clear();
			Action archiveChanged = this.ArchiveChanged;
			if (archiveChanged == null)
			{
				return;
			}
			archiveChanged();
		}

		// Token: 0x060024E2 RID: 9442 RVA: 0x0005A652 File Offset: 0x00058852
		public bool HasSeenTutorial(TutorialProgress progress)
		{
			if (this.m_cachedTutProgress == null)
			{
				this.m_cachedTutProgress = new TutorialProgress?((TutorialProgress)PlayerPrefs.GetInt("TutorialProgress", 0));
			}
			return (this.m_cachedTutProgress.Value & progress) == progress;
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x0012EF7C File Offset: 0x0012D17C
		public void MarkTutorialSeen(TutorialProgress progress)
		{
			if (this.m_cachedTutProgress == null)
			{
				this.m_cachedTutProgress = new TutorialProgress?((TutorialProgress)PlayerPrefs.GetInt("TutorialProgress", 0));
			}
			this.m_cachedTutProgress |= progress;
			PlayerPrefs.SetInt("TutorialProgress", (int)this.m_cachedTutProgress.Value);
		}

		// Token: 0x060024E4 RID: 9444 RVA: 0x0005A687 File Offset: 0x00058887
		public void ResetTutorialProgress()
		{
			this.m_cachedTutProgress = new TutorialProgress?(TutorialProgress.None);
			PlayerPrefs.DeleteKey("TutorialProgress");
			this.OnHideTutorialsChanged();
		}

		// Token: 0x060024E5 RID: 9445 RVA: 0x0012EFF4 File Offset: 0x0012D1F4
		public bool TryShowTutorial(TutorialProgress progress)
		{
			if (!Options.GameOptions.HideTutorialPopups.Value)
			{
				bool flag = false;
				foreach (TutorialNotification tutorialNotification in GlobalSettings.Values.Notifications.TutorialNotifications)
				{
					if (progress.HasFlag(tutorialNotification.Progress) && !this.HasSeenTutorial(tutorialNotification.Progress))
					{
						this.EnqueueNotification(new Notification
						{
							Type = tutorialNotification.Notification,
							Created = DateTime.UtcNow
						});
						flag = true;
					}
				}
				if (flag)
				{
					UIManager.InvokeTriggerControlPanelUsageHighlight(WindowToggler.WindowType.Log);
				}
				this.MarkTutorialSeen(progress);
				return true;
			}
			return false;
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x0012F09C File Offset: 0x0012D29C
		private void ShowNotification(DateTime now)
		{
			if (this.m_pendingQueue.Count == 0)
			{
				this.m_nextNotificationShowTime = null;
				return;
			}
			Notification notification = this.m_pendingQueue.Dequeue();
			notification.Shown = now;
			if (notification.Type.Presentation.HasBitFlag(NotificationPresentationFlags.SidePanel) && !ClientGameManager.UIManager.NotificationsUI.ShowNotification(notification))
			{
				this.EnqueueNotification(notification);
				return;
			}
			if (notification.Type.Presentation.HasBitFlag(NotificationPresentationFlags.CenterToast))
			{
				ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
				{
					Title = notification.TitleWithData,
					Text = notification.DescriptionWithData,
					TimeShown = notification.Type.CenterToastDurationSeconds
				});
			}
			if (notification.Type.Presentation.HasBitFlag(NotificationPresentationFlags.TutorialPopup))
			{
				ClientGameManager.UIManager.InitTutorialPopup(new TutorialPopupOptions
				{
					Title = notification.TitleWithData,
					Text = notification.DescriptionWithData,
					MessageType = notification.Type.PopupMessageType,
					ConfirmationText = "Ok",
					PreventDragging = true,
					Tutorial = notification.Type
				});
			}
			this.m_nextNotificationShowTime = new DateTime?(now.AddSeconds(2.0));
		}

		// Token: 0x060024E7 RID: 9447 RVA: 0x0012F1E8 File Offset: 0x0012D3E8
		private void OnNewFriendRequest(Mail request)
		{
			this.EnqueueNotification(new Notification
			{
				Type = GlobalSettings.Values.Notifications.NewFriendRequest,
				TextData = request.Sender,
				Created = DateTime.UtcNow
			});
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x0012F234 File Offset: 0x0012D434
		private void OnNewGuildInvite(Mail request)
		{
			this.EnqueueNotification(new Notification
			{
				Type = GlobalSettings.Values.Notifications.NewGuildInvite,
				TextData = request.GuildName,
				Created = DateTime.UtcNow
			});
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x0012F280 File Offset: 0x0012D480
		private void OnUnreadMailUpdated()
		{
			if (ClientGameManager.SocialManager.HasUnreadMail && !this.m_hadUnreadMail)
			{
				this.EnqueueNotification(new Notification
				{
					Type = GlobalSettings.Values.Notifications.UnreadMail,
					TextData = string.Empty,
					Created = DateTime.UtcNow
				});
				this.m_hadUnreadMail = true;
				return;
			}
			if (!ClientGameManager.SocialManager.HasUnreadMail && this.m_hadUnreadMail)
			{
				this.m_hadUnreadMail = false;
			}
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x0012F304 File Offset: 0x0012D504
		private void OnHideTutorialsChanged()
		{
			LocalPlayer.LocalPlayerOffensiveTargetChanged -= this.OnSelectAggressiveTarget;
			LocalPlayer.LocalPlayerHealthStateUpdated -= this.OnHealthStateChanged;
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.OnInventoryChanged;
			}
			DiscoveryProgression.DiscoveryFound -= this.OnDiscoveryFound;
			if (!Options.GameOptions.HideTutorialPopups.Value)
			{
				if (!this.HasSeenTutorial(TutorialProgress.MobDifficulty1))
				{
					LocalPlayer.LocalPlayerOffensiveTargetChanged += this.OnSelectAggressiveTarget;
				}
				if (!this.HasSeenTutorial(TutorialProgress.KnockedSenseless) || !this.HasSeenTutorial(TutorialProgress.LiveToPlayAnotherDay))
				{
					LocalPlayer.LocalPlayerHealthStateUpdated += this.OnHealthStateChanged;
				}
				if (LocalPlayer.IsInitialized && !this.HasSeenTutorial(TutorialProgress.ArmorWeight))
				{
					LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.OnInventoryChanged;
				}
				if (!this.HasSeenTutorial(TutorialProgress.FindingAGroup))
				{
					DiscoveryProgression.DiscoveryFound += this.OnDiscoveryFound;
				}
				if (LocalPlayer.IsInitialized)
				{
					this.MarkNewAsSeen();
				}
				if (!LocalPlayer.IsInitialized)
				{
					LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
				}
			}
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x0012F434 File Offset: 0x0012D634
		private void MarkNewAsSeen()
		{
			ArchetypeInstance archetypeInstance;
			if (LocalPlayer.IsInitialized && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(LocalPlayer.GameEntity.CharacterData.BaseRoleId, out archetypeInstance))
			{
				MasteryInstanceData masteryData = archetypeInstance.MasteryData;
				if (masteryData != null && masteryData.BaseLevel > 1f)
				{
					this.MarkTutorialSeen(TutorialProgress.SheatheWeapon);
				}
			}
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x0005A6A5 File Offset: 0x000588A5
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.OnInventoryChanged;
			this.MarkNewAsSeen();
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x0012F498 File Offset: 0x0012D698
		private void OnSelectAggressiveTarget(GameEntity target)
		{
			if (!Options.GameOptions.HideTutorialPopups.Value)
			{
				if (target && target.gameObject.GetComponent<InteractiveTargetDummy>() == null && this.TryShowTutorial(TutorialProgress.MobDifficulty1 | TutorialProgress.MobDifficulty2))
				{
					LocalPlayer.LocalPlayerOffensiveTargetChanged -= this.OnSelectAggressiveTarget;
					return;
				}
			}
			else
			{
				LocalPlayer.LocalPlayerOffensiveTargetChanged -= this.OnSelectAggressiveTarget;
			}
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x0012F4F8 File Offset: 0x0012D6F8
		private void OnHealthStateChanged(HealthState state)
		{
			if (!Options.GameOptions.HideTutorialPopups.Value)
			{
				if (state == HealthState.Unconscious)
				{
					this.TryShowTutorial(TutorialProgress.KnockedSenseless);
					return;
				}
				if (state != HealthState.Dead)
				{
					return;
				}
				if (this.TryShowTutorial(TutorialProgress.LiveToPlayAnotherDay | TutorialProgress.ALostAdventuringBag))
				{
					LocalPlayer.LocalPlayerHealthStateUpdated -= this.OnHealthStateChanged;
					return;
				}
			}
			else
			{
				LocalPlayer.LocalPlayerHealthStateUpdated -= this.OnHealthStateChanged;
			}
		}

		// Token: 0x060024EF RID: 9455 RVA: 0x0012F550 File Offset: 0x0012D750
		private void OnInventoryChanged()
		{
			if (!Options.GameOptions.HideTutorialPopups.Value)
			{
				if (LocalPlayer.GameEntity.Vitals.ArmorCost > LocalPlayer.GameEntity.Vitals.ArmorWeightCapacity && this.TryShowTutorial(TutorialProgress.ArmorWeight))
				{
					LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.OnInventoryChanged;
					return;
				}
			}
			else
			{
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.OnInventoryChanged;
			}
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0012F5D8 File Offset: 0x0012D7D8
		private void OnDiscoveryFound(DiscoveryProfile profile)
		{
			if (!Options.GameOptions.HideTutorialPopups.Value)
			{
				if (profile.Id == GlobalSettings.Values.Notifications.TutorialDiscovery1.Id && this.TryShowTutorial(TutorialProgress.FindingAGroup))
				{
					DiscoveryProgression.DiscoveryFound -= this.OnDiscoveryFound;
					return;
				}
			}
			else
			{
				DiscoveryProgression.DiscoveryFound -= this.OnDiscoveryFound;
			}
		}

		// Token: 0x040027A4 RID: 10148
		private const int kNextNotificationDelaySeconds = 2;

		// Token: 0x040027A5 RID: 10149
		private Queue<Notification> m_pendingQueue = new Queue<Notification>();

		// Token: 0x040027A6 RID: 10150
		private List<Notification> m_archive = new List<Notification>();

		// Token: 0x040027A7 RID: 10151
		private DateTime? m_nextNotificationShowTime;

		// Token: 0x040027AA RID: 10154
		private const string kTutorialProgressKey = "TutorialProgress";

		// Token: 0x040027AB RID: 10155
		private TutorialProgress? m_cachedTutProgress;

		// Token: 0x040027AC RID: 10156
		private bool m_hadUnreadMail;
	}
}
