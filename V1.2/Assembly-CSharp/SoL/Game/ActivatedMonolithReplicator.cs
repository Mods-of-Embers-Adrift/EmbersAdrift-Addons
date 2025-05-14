using System;
using System.Collections.Generic;
using SoL.Game.Discovery;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000550 RID: 1360
	public class ActivatedMonolithReplicator : SyncVarReplicator
	{
		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06002953 RID: 10579 RVA: 0x001411B8 File Offset: 0x0013F3B8
		// (remove) Token: 0x06002954 RID: 10580 RVA: 0x001411EC File Offset: 0x0013F3EC
		public static event Action ActiveMonolithListChanged;

		// Token: 0x06002955 RID: 10581 RVA: 0x00141220 File Offset: 0x0013F420
		public static bool IsAvailable(MonolithProfile profile)
		{
			return profile && (!profile.RequiresActiveRecord || (ActivatedMonolithReplicator.Instance && ActivatedMonolithReplicator.Instance.m_available != null && ActivatedMonolithReplicator.Instance.m_available.Contains(profile.Id)));
		}

		// Token: 0x06002956 RID: 10582 RVA: 0x0005C96F File Offset: 0x0005AB6F
		private void Awake()
		{
			if (ActivatedMonolithReplicator.Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			ActivatedMonolithReplicator.Instance = this;
		}

		// Token: 0x06002957 RID: 10583 RVA: 0x0005C98B File Offset: 0x0005AB8B
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				this.m_available.Changed += this.AvailableOnChanged;
			}
		}

		// Token: 0x06002958 RID: 10584 RVA: 0x0005C9AB File Offset: 0x0005ABAB
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!GameManager.IsServer)
			{
				this.m_available.Changed -= this.AvailableOnChanged;
				ActivatedMonolithReplicator.Instance = null;
				Action activeMonolithListChanged = ActivatedMonolithReplicator.ActiveMonolithListChanged;
				if (activeMonolithListChanged == null)
				{
					return;
				}
				activeMonolithListChanged();
			}
		}

		// Token: 0x06002959 RID: 10585 RVA: 0x00141270 File Offset: 0x0013F470
		private void AvailableOnChanged(SynchronizedCollection<int, UniqueId>.Operation arg1, int arg2, UniqueId arg3, UniqueId arg4)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			Action activeMonolithListChanged = ActivatedMonolithReplicator.ActiveMonolithListChanged;
			if (activeMonolithListChanged != null)
			{
				activeMonolithListChanged();
			}
			string text = null;
			MonolithProfile monolithProfile2;
			string text3;
			if (arg1 != SynchronizedCollection<int, UniqueId>.Operation.Add)
			{
				if (arg1 == SynchronizedCollection<int, UniqueId>.Operation.RemoveAt)
				{
					MonolithProfile monolithProfile;
					string text2;
					if (InternalGameDatabase.Archetypes.TryGetAsType<MonolithProfile>(arg3, out monolithProfile) && monolithProfile.TryGetOfflineMessage(out text2))
					{
						text = text2;
					}
				}
			}
			else if (InternalGameDatabase.Archetypes.TryGetAsType<MonolithProfile>(arg4, out monolithProfile2) && monolithProfile2.TryGetOnlineMessage(out text3))
			{
				text = text3;
			}
			if (!string.IsNullOrEmpty(text))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Emote, text);
			}
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x001412F4 File Offset: 0x0013F4F4
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_available);
			this.m_available.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002A48 RID: 10824
		private static ActivatedMonolithReplicator Instance;

		// Token: 0x04002A4A RID: 10826
		private readonly SynchronizedList<UniqueId> m_available = new SynchronizedListStruct<UniqueId>();

		// Token: 0x04002A4B RID: 10827
		private List<UniqueId> m_currentActive;
	}
}
