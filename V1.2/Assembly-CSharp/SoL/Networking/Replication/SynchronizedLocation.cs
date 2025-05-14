using System;
using NetStack.Serialization;
using SoL.Networking.Database;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A5 RID: 1189
	public class SynchronizedLocation : SynchronizedVariable<CharacterLocation>
	{
		// Token: 0x06002128 RID: 8488 RVA: 0x000580AE File Offset: 0x000562AE
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			base.Value.PackData(buffer);
			return buffer;
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x000580BE File Offset: 0x000562BE
		protected override CharacterLocation ReadDataInternal(BitBuffer buffer)
		{
			CharacterLocation characterLocation = new CharacterLocation();
			characterLocation.ReadData(buffer);
			return characterLocation;
		}
	}
}
