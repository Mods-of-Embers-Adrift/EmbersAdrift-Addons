using System;
using SoL.Networking.Objects;
using SoL.Utilities;

namespace SoL.Game.Trading
{
	// Token: 0x0200064C RID: 1612
	public class Trade : IPoolable
	{
		// Token: 0x06003222 RID: 12834 RVA: 0x00062A31 File Offset: 0x00060C31
		public void Init(UniqueId id, NetworkEntity source, NetworkEntity target)
		{
			this.Id = id;
			this.Source = source;
			this.Target = target;
			this.InitInternal();
		}

		// Token: 0x06003223 RID: 12835 RVA: 0x00062A4E File Offset: 0x00060C4E
		public void Init(NetworkEntity source, NetworkEntity target)
		{
			this.Id = UniqueId.GenerateFromGuid();
			this.Source = source;
			this.Target = target;
			this.InitInternal();
		}

		// Token: 0x06003224 RID: 12836 RVA: 0x00062A6F File Offset: 0x00060C6F
		private void InitInternal()
		{
			this.Timestamp = DateTime.UtcNow;
			this.SourceStatus = TradeStatus.None;
			this.TargetStatus = TradeStatus.None;
		}

		// Token: 0x06003225 RID: 12837 RVA: 0x00062A8A File Offset: 0x00060C8A
		public void Reset()
		{
			this.Id = UniqueId.Empty;
			this.Timestamp = DateTime.MinValue;
			this.Source = null;
			this.SourceStatus = TradeStatus.None;
			this.Target = null;
			this.TargetStatus = TradeStatus.None;
			this.Complete = false;
		}

		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x06003226 RID: 12838 RVA: 0x00062AC5 File Offset: 0x00060CC5
		// (set) Token: 0x06003227 RID: 12839 RVA: 0x00062ACD File Offset: 0x00060CCD
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x040030BF RID: 12479
		public UniqueId Id;

		// Token: 0x040030C0 RID: 12480
		public DateTime Timestamp;

		// Token: 0x040030C1 RID: 12481
		public NetworkEntity Source;

		// Token: 0x040030C2 RID: 12482
		public TradeStatus SourceStatus;

		// Token: 0x040030C3 RID: 12483
		public NetworkEntity Target;

		// Token: 0x040030C4 RID: 12484
		public TradeStatus TargetStatus;

		// Token: 0x040030C5 RID: 12485
		public bool Complete;

		// Token: 0x040030C6 RID: 12486
		private bool m_inPool;
	}
}
