using System;
using NetStack.Serialization;
using SoL.Game.Objects;
using SoL.Networking;

namespace SoL.Game.Quests
{
	// Token: 0x0200077B RID: 1915
	public struct BBTaskDrawCache : INetworkSerializable
	{
		// Token: 0x06003887 RID: 14471 RVA: 0x0006675B File Offset: 0x0006495B
		public BitBuffer PackData(BitBuffer buffer)
		{
			if (this.BulletinBoard != null)
			{
				buffer.AddUniqueId(this.BulletinBoard.Id);
			}
			buffer.AddEnum(this.Type);
			return buffer;
		}

		// Token: 0x06003888 RID: 14472 RVA: 0x0016DF64 File Offset: 0x0016C164
		public BitBuffer ReadData(BitBuffer buffer)
		{
			UniqueId id = buffer.ReadUniqueId();
			InternalGameDatabase.BulletinBoards.TryGetItem(id, out this.BulletinBoard);
			this.Type = buffer.ReadEnum<BBTaskType>();
			return buffer;
		}

		// Token: 0x04003755 RID: 14165
		public BulletinBoard BulletinBoard;

		// Token: 0x04003756 RID: 14166
		public BBTaskType Type;
	}
}
