using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A3E RID: 2622
	[Serializable]
	public class ItemDamage : INetworkSerializable
	{
		// Token: 0x14000110 RID: 272
		// (add) Token: 0x06005126 RID: 20774 RVA: 0x001CEE7C File Offset: 0x001CD07C
		// (remove) Token: 0x06005127 RID: 20775 RVA: 0x001CEEB4 File Offset: 0x001CD0B4
		public event Action DurabilityChanged;

		// Token: 0x17001224 RID: 4644
		// (get) Token: 0x06005128 RID: 20776 RVA: 0x000763CA File Offset: 0x000745CA
		// (set) Token: 0x06005129 RID: 20777 RVA: 0x000763D2 File Offset: 0x000745D2
		public int Absorbed
		{
			get
			{
				return this.m_absorbed;
			}
			set
			{
				this.m_absorbed = value;
				Action durabilityChanged = this.DurabilityChanged;
				if (durabilityChanged == null)
				{
					return;
				}
				durabilityChanged();
			}
		}

		// Token: 0x17001225 RID: 4645
		// (get) Token: 0x0600512A RID: 20778 RVA: 0x000763EB File Offset: 0x000745EB
		// (set) Token: 0x0600512B RID: 20779 RVA: 0x000763F3 File Offset: 0x000745F3
		public int Repaired
		{
			get
			{
				return this.m_repaired;
			}
			set
			{
				this.m_repaired = value;
				Action durabilityChanged = this.DurabilityChanged;
				if (durabilityChanged == null)
				{
					return;
				}
				durabilityChanged();
			}
		}

		// Token: 0x0600512C RID: 20780 RVA: 0x0007640C File Offset: 0x0007460C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddInt(this.Absorbed);
			buffer.AddInt(this.Repaired);
			return buffer;
		}

		// Token: 0x0600512D RID: 20781 RVA: 0x00076429 File Offset: 0x00074629
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Absorbed = buffer.ReadInt();
			this.Repaired = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x0600512E RID: 20782 RVA: 0x00076444 File Offset: 0x00074644
		public void RepairItem()
		{
			this.Repaired += this.Absorbed;
			this.Absorbed = 0;
		}

		// Token: 0x0600512F RID: 20783 RVA: 0x00076460 File Offset: 0x00074660
		public void RepairAmount(int amount)
		{
			if (this.Absorbed < amount)
			{
				amount = this.Absorbed;
			}
			this.Repaired += amount;
			this.Absorbed -= amount;
		}

		// Token: 0x0400489B RID: 18587
		private int m_absorbed;

		// Token: 0x0400489C RID: 18588
		private int m_repaired;
	}
}
