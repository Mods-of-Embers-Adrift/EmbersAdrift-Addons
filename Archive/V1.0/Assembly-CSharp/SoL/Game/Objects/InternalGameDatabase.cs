using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F6 RID: 2550
	public static class InternalGameDatabase
	{
		// Token: 0x1700111A RID: 4378
		// (get) Token: 0x06004D8B RID: 19851 RVA: 0x00074634 File Offset: 0x00072834
		public static ArchetypeCollection Archetypes
		{
			get
			{
				if (InternalGameDatabase.m_archetypes == null)
				{
					InternalGameDatabase.m_archetypes = Resources.Load<ArchetypeCollection>("ArchetypeCollection");
				}
				return InternalGameDatabase.m_archetypes;
			}
		}

		// Token: 0x1700111B RID: 4379
		// (get) Token: 0x06004D8C RID: 19852 RVA: 0x00074657 File Offset: 0x00072857
		public static QuestCollection Quests
		{
			get
			{
				if (InternalGameDatabase.m_quests == null)
				{
					InternalGameDatabase.m_quests = Resources.Load<QuestCollection>("QuestCollection");
				}
				return InternalGameDatabase.m_quests;
			}
		}

		// Token: 0x1700111C RID: 4380
		// (get) Token: 0x06004D8D RID: 19853 RVA: 0x0007467A File Offset: 0x0007287A
		public static BBTaskCollection BBTasks
		{
			get
			{
				if (InternalGameDatabase.m_bbTasks == null)
				{
					InternalGameDatabase.m_bbTasks = Resources.Load<BBTaskCollection>("BBTaskCollection");
				}
				return InternalGameDatabase.m_bbTasks;
			}
		}

		// Token: 0x1700111D RID: 4381
		// (get) Token: 0x06004D8E RID: 19854 RVA: 0x0007469D File Offset: 0x0007289D
		public static BulletinBoardCollection BulletinBoards
		{
			get
			{
				if (InternalGameDatabase.m_bulletinBoards == null)
				{
					InternalGameDatabase.m_bulletinBoards = Resources.Load<BulletinBoardCollection>("BulletinBoardCollection");
				}
				return InternalGameDatabase.m_bulletinBoards;
			}
		}

		// Token: 0x1700111E RID: 4382
		// (get) Token: 0x06004D8F RID: 19855 RVA: 0x000746C0 File Offset: 0x000728C0
		public static NotificationsCollection Notifications
		{
			get
			{
				if (InternalGameDatabase.m_notifications == null)
				{
					InternalGameDatabase.m_notifications = Resources.Load<NotificationsCollection>("NotificationsCollection");
				}
				return InternalGameDatabase.m_notifications;
			}
		}

		// Token: 0x1700111F RID: 4383
		// (get) Token: 0x06004D90 RID: 19856 RVA: 0x000746E3 File Offset: 0x000728E3
		public static GlobalSettings GlobalSettings
		{
			get
			{
				if (InternalGameDatabase.m_globalSettings == null)
				{
					InternalGameDatabase.m_globalSettings = Resources.Load<GlobalSettings>("GlobalSettings");
				}
				return InternalGameDatabase.m_globalSettings;
			}
		}

		// Token: 0x04004728 RID: 18216
		private static ArchetypeCollection m_archetypes;

		// Token: 0x04004729 RID: 18217
		private static QuestCollection m_quests;

		// Token: 0x0400472A RID: 18218
		private static BBTaskCollection m_bbTasks;

		// Token: 0x0400472B RID: 18219
		private static BulletinBoardCollection m_bulletinBoards;

		// Token: 0x0400472C RID: 18220
		private static NotificationsCollection m_notifications;

		// Token: 0x0400472D RID: 18221
		private static GlobalSettings m_globalSettings;
	}
}
