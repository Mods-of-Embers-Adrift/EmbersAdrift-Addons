using System;
using NetStack.Serialization;
using SoL.Game;
using SoL.Networking.Managers;
using SoL.Networking.Objects;

namespace SoL.Networking.Replication
{
	// Token: 0x0200047F RID: 1151
	public abstract class Replicator : GameEntityComponent, IReplicator
	{
		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06002033 RID: 8243 RVA: 0x00057851 File Offset: 0x00055A51
		// (set) Token: 0x06002034 RID: 8244 RVA: 0x00057859 File Offset: 0x00055A59
		private int SyncCount { get; set; }

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06002035 RID: 8245
		public abstract ReplicatorTypes Type { get; }

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06002036 RID: 8246 RVA: 0x00057862 File Offset: 0x00055A62
		// (set) Token: 0x06002037 RID: 8247 RVA: 0x0005786A File Offset: 0x00055A6A
		public bool Dirty { get; protected set; }

		// Token: 0x06002038 RID: 8248 RVA: 0x00057873 File Offset: 0x00055A73
		public void Init(int index, INetworkManager network, NetworkEntity netEntity)
		{
			this.m_index = index;
			this.m_network = network;
			this.m_netEntity = netEntity;
			this.SyncCount = this.RegisterSyncs();
			this.PostInit();
		}

		// Token: 0x06002039 RID: 8249 RVA: 0x0005789C File Offset: 0x00055A9C
		protected virtual void OnDestroy()
		{
			this.m_network = null;
			this.m_netEntity = null;
		}

		// Token: 0x0600203A RID: 8250 RVA: 0x000578AC File Offset: 0x00055AAC
		public virtual void SetDirtyFlags(DateTime timestamp)
		{
			this.Dirty = false;
		}

		// Token: 0x0600203B RID: 8251 RVA: 0x00049A92 File Offset: 0x00047C92
		public virtual BitBuffer PackData(BitBuffer outBuffer)
		{
			return outBuffer;
		}

		// Token: 0x0600203C RID: 8252 RVA: 0x00049A92 File Offset: 0x00047C92
		public virtual BitBuffer ReadData(BitBuffer inBuffer)
		{
			return inBuffer;
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x00049A92 File Offset: 0x00047C92
		public virtual BitBuffer PackInitialData(BitBuffer outBuffer)
		{
			return outBuffer;
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x00049A92 File Offset: 0x00047C92
		public virtual BitBuffer ReadInitialData(BitBuffer inBuffer)
		{
			return inBuffer;
		}

		// Token: 0x0600203F RID: 8255 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual int RegisterSyncs()
		{
			return 0;
		}

		// Token: 0x06002040 RID: 8256 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void PostInit()
		{
		}

		// Token: 0x04002581 RID: 9601
		protected INetworkManager m_network;

		// Token: 0x04002582 RID: 9602
		protected NetworkEntity m_netEntity;

		// Token: 0x04002583 RID: 9603
		protected int m_index;
	}
}
