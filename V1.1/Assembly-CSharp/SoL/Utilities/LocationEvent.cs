using System;
using NetStack.Serialization;
using SoL.Game.Objects;
using SoL.Networking;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000296 RID: 662
	public struct LocationEvent : INetworkSerializable
	{
		// Token: 0x06001419 RID: 5145 RVA: 0x00050173 File Offset: 0x0004E373
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddVector3(this.Location, NetworkManager.Range);
			return buffer;
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x00050195 File Offset: 0x0004E395
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.Location = buffer.ReadVector3(NetworkManager.Range);
			return buffer;
		}

		// Token: 0x0600141B RID: 5147 RVA: 0x000F93C8 File Offset: 0x000F75C8
		public void ExecuteEvent()
		{
			ILocationEvent locationEvent;
			if (InternalGameDatabase.Archetypes.TryGetAsType<ILocationEvent>(this.ArchetypeId, out locationEvent))
			{
				locationEvent.ExecuteLocationEvent(this.Location);
			}
		}

		// Token: 0x04001C6C RID: 7276
		public UniqueId ArchetypeId;

		// Token: 0x04001C6D RID: 7277
		public Vector3 Location;
	}
}
