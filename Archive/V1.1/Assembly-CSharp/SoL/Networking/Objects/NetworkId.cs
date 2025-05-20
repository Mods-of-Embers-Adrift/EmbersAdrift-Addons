using System;
using ENet;

namespace SoL.Networking.Objects
{
	// Token: 0x020004B7 RID: 1207
	public struct NetworkId : IEquatable<NetworkId>
	{
		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x060021DE RID: 8670 RVA: 0x000587B2 File Offset: 0x000569B2
		public uint Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x060021DF RID: 8671 RVA: 0x000587BA File Offset: 0x000569BA
		public Peer Peer
		{
			get
			{
				return this.m_peer;
			}
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x000587C2 File Offset: 0x000569C2
		public NetworkId(uint value)
		{
			this.m_value = value;
			this.m_peer = default(Peer);
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x000587D7 File Offset: 0x000569D7
		public NetworkId(uint value, Peer peer)
		{
			this.m_value = value;
			this.m_peer = peer;
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x060021E2 RID: 8674 RVA: 0x000587E7 File Offset: 0x000569E7
		public bool IsEmpty
		{
			get
			{
				return this.m_value == 0U;
			}
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x060021E3 RID: 8675 RVA: 0x00125904 File Offset: 0x00123B04
		public bool HasPeer
		{
			get
			{
				return this.m_peer.IsSet;
			}
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x060021E4 RID: 8676 RVA: 0x000587F2 File Offset: 0x000569F2
		public bool IsPlayer
		{
			get
			{
				return this.HasPeer;
			}
		}

		// Token: 0x060021E5 RID: 8677 RVA: 0x000587B2 File Offset: 0x000569B2
		public override int GetHashCode()
		{
			return (int)this.m_value;
		}

		// Token: 0x060021E6 RID: 8678 RVA: 0x000587FA File Offset: 0x000569FA
		public override bool Equals(object obj)
		{
			return obj != null && obj is NetworkId && this.Equals((NetworkId)obj);
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x00058817 File Offset: 0x00056A17
		public static bool operator ==(NetworkId c1, NetworkId c2)
		{
			return c1.m_value == c2.m_value;
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x00058827 File Offset: 0x00056A27
		public static bool operator !=(NetworkId c1, NetworkId c2)
		{
			return c1.m_value != c2.m_value;
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x00125920 File Offset: 0x00123B20
		public override string ToString()
		{
			return this.m_value.ToString();
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x00058817 File Offset: 0x00056A17
		public bool Equals(NetworkId other)
		{
			return this.m_value == other.m_value;
		}

		// Token: 0x04002620 RID: 9760
		private readonly uint m_value;

		// Token: 0x04002621 RID: 9761
		private readonly Peer m_peer;

		// Token: 0x04002622 RID: 9762
		public static NetworkId Invalid = new NetworkId(uint.MaxValue);

		// Token: 0x04002623 RID: 9763
		internal static NetworkId Zero = new NetworkId(0U);
	}
}
