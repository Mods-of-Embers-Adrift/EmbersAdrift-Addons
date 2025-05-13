using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CB1 RID: 3249
	[CreateAssetMenu(menuName = "SoL/Profiles/Discovery (Monolith)")]
	public class MonolithProfile : DiscoveryProfile
	{
		// Token: 0x17001786 RID: 6022
		// (get) Token: 0x06006286 RID: 25222 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_showCategory
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001787 RID: 6023
		// (get) Token: 0x06006287 RID: 25223 RVA: 0x00062532 File Offset: 0x00060732
		protected override DiscoveryCategory DiscoveryCategory
		{
			get
			{
				return DiscoveryCategory.Monolith;
			}
		}

		// Token: 0x17001788 RID: 6024
		// (get) Token: 0x06006288 RID: 25224 RVA: 0x000824FB File Offset: 0x000806FB
		public MonolithFlags MonolithFlag
		{
			get
			{
				return this.m_monolithFlag;
			}
		}

		// Token: 0x06006289 RID: 25225 RVA: 0x00205204 File Offset: 0x00203404
		private string GetActiveTimeInSeconds()
		{
			if (!this.m_requiresActiveRecord)
			{
				return string.Empty;
			}
			return "Active Time In Seconds: " + Mathf.FloorToInt(this.m_activeTimeInDays * 86400f).ToString();
		}

		// Token: 0x17001789 RID: 6025
		// (get) Token: 0x0600628A RID: 25226 RVA: 0x00082503 File Offset: 0x00080703
		public bool RequiresActiveRecord
		{
			get
			{
				return this.m_requiresActiveRecord;
			}
		}

		// Token: 0x1700178A RID: 6026
		// (get) Token: 0x0600628B RID: 25227 RVA: 0x0008250B File Offset: 0x0008070B
		public float ActiveTimeInDays
		{
			get
			{
				return this.m_activeTimeInDays;
			}
		}

		// Token: 0x0600628C RID: 25228 RVA: 0x00082513 File Offset: 0x00080713
		public bool IsAvailable()
		{
			return !this.m_requiresActiveRecord || ActivatedMonolithReplicator.IsAvailable(this);
		}

		// Token: 0x0600628D RID: 25229 RVA: 0x00082525 File Offset: 0x00080725
		public bool TryGetOnlineMessage(out string msg)
		{
			return this.TryGetMessage(true, out msg);
		}

		// Token: 0x0600628E RID: 25230 RVA: 0x0008252F File Offset: 0x0008072F
		public bool TryGetOfflineMessage(out string msg)
		{
			return this.TryGetMessage(false, out msg);
		}

		// Token: 0x0600628F RID: 25231 RVA: 0x00205244 File Offset: 0x00203444
		private bool TryGetMessage(bool isOnline, out string msg)
		{
			msg = (isOnline ? this.m_onlineMessage : this.m_offlineMessage);
			return (!this.m_notifyGmsOnly || !LocalPlayer.GameEntity || LocalPlayer.GameEntity.GM) && this.m_requiresActiveRecord && !string.IsNullOrEmpty(msg);
		}

		// Token: 0x040055F5 RID: 22005
		[SerializeField]
		private MonolithFlags m_monolithFlag;

		// Token: 0x040055F6 RID: 22006
		[SerializeField]
		private bool m_requiresActiveRecord;

		// Token: 0x040055F7 RID: 22007
		[SerializeField]
		private bool m_notifyGmsOnly;

		// Token: 0x040055F8 RID: 22008
		[SerializeField]
		private float m_activeTimeInDays;

		// Token: 0x040055F9 RID: 22009
		[SerializeField]
		private string m_onlineMessage;

		// Token: 0x040055FA RID: 22010
		[SerializeField]
		private string m_offlineMessage;
	}
}
