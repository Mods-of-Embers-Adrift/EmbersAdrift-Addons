using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE5 RID: 3301
	public struct ItemUsageSerializable : INetworkSerializable
	{
		// Token: 0x170017F5 RID: 6133
		// (get) Token: 0x060063FC RID: 25596 RVA: 0x00083521 File Offset: 0x00081721
		// (set) Token: 0x060063FD RID: 25597 RVA: 0x00083529 File Offset: 0x00081729
		public UniqueId Instance { readonly get; set; }

		// Token: 0x170017F6 RID: 6134
		// (get) Token: 0x060063FE RID: 25598 RVA: 0x00083532 File Offset: 0x00081732
		// (set) Token: 0x060063FF RID: 25599 RVA: 0x0008353A File Offset: 0x0008173A
		public string UsedFor { readonly get; set; }

		// Token: 0x170017F7 RID: 6135
		// (get) Token: 0x06006400 RID: 25600 RVA: 0x00083543 File Offset: 0x00081743
		// (set) Token: 0x06006401 RID: 25601 RVA: 0x0008354B File Offset: 0x0008174B
		public int AmountUsed { readonly get; set; }

		// Token: 0x06006402 RID: 25602 RVA: 0x00083554 File Offset: 0x00081754
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.Instance);
			buffer.AddString(this.UsedFor);
			buffer.AddInt(this.AmountUsed);
			return buffer;
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x0008357E File Offset: 0x0008177E
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Instance = buffer.ReadUniqueId();
			this.UsedFor = buffer.ReadString();
			this.AmountUsed = buffer.ReadInt();
			return buffer;
		}
	}
}
