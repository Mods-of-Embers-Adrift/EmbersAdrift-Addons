using System;
using SoL.Networking.Replication;

namespace SoL.Game
{
	// Token: 0x020005FD RID: 1533
	public class VitalsReplicatorPlayer : VitalsReplicator
	{
		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x060030FF RID: 12543 RVA: 0x00061C2A File Offset: 0x0005FE2A
		public override SynchronizedByte StaminaSyncVar
		{
			get
			{
				return this.Stamina;
			}
		}

		// Token: 0x06003100 RID: 12544 RVA: 0x0015BEE8 File Offset: 0x0015A0E8
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.ArmorClass);
			this.ArmorClass.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Health);
			this.Health.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.HealthWound);
			this.HealthWound.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.MaxArmorClass);
			this.MaxArmorClass.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.MaxHealth);
			this.MaxHealth.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Stamina);
			this.Stamina.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.StaminaWound);
			this.StaminaWound.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002F44 RID: 12100
		public readonly SynchronizedInt Health = new SynchronizedInt();

		// Token: 0x04002F45 RID: 12101
		public readonly SynchronizedByte HealthWound = new SynchronizedByte();

		// Token: 0x04002F46 RID: 12102
		public readonly SynchronizedInt MaxHealth = new SynchronizedInt();

		// Token: 0x04002F47 RID: 12103
		public readonly SynchronizedByte Stamina = new SynchronizedByte();

		// Token: 0x04002F48 RID: 12104
		public readonly SynchronizedByte StaminaWound = new SynchronizedByte();

		// Token: 0x04002F49 RID: 12105
		public readonly SynchronizedInt ArmorClass = new SynchronizedInt();

		// Token: 0x04002F4A RID: 12106
		public readonly SynchronizedInt MaxArmorClass = new SynchronizedInt();
	}
}
