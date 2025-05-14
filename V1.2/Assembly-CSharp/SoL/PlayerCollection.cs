using System;
using System.Collections.Generic;
using SoL.Networking.Objects;

namespace SoL
{
	// Token: 0x02000222 RID: 546
	public class PlayerCollection : DictionaryList<uint, NetworkEntity>
	{
		// Token: 0x0600125A RID: 4698 RVA: 0x0004F133 File Offset: 0x0004D333
		public PlayerCollection(bool replace = false) : base(replace)
		{
			this.m_peerIdToNetworkId = new Dictionary<uint, uint>();
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x000E6C14 File Offset: 0x000E4E14
		public NetworkEntity GetNetworkEntityForPeerId(uint peerId)
		{
			NetworkEntity result = null;
			uint key;
			if (this.m_peerIdToNetworkId.TryGetValue(peerId, out key) && base.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x000E6C44 File Offset: 0x000E4E44
		public override void Add(uint key, NetworkEntity value)
		{
			base.Add(key, value);
			if (this.m_replaceWhenPresent)
			{
				this.m_peerIdToNetworkId.Remove(value.NetworkId.Peer.ID);
			}
			this.m_peerIdToNetworkId.Add(value.NetworkId.Peer.ID, value.NetworkId.Value);
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x000E6CB4 File Offset: 0x000E4EB4
		public override bool Remove(uint key)
		{
			bool flag = false;
			NetworkEntity networkEntity;
			if (base.TryGetValue(key, out networkEntity))
			{
				uint id = networkEntity.NetworkId.Peer.ID;
				flag = this.m_peerIdToNetworkId.Remove(id);
			}
			return flag && base.Remove(key);
		}

		// Token: 0x04000FF8 RID: 4088
		private readonly Dictionary<uint, uint> m_peerIdToNetworkId;
	}
}
