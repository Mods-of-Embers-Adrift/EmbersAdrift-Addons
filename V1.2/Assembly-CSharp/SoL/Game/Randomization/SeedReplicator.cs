using System;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000775 RID: 1909
	public class SeedReplicator : SyncVarReplicator
	{
		// Token: 0x17000CDE RID: 3294
		// (get) Token: 0x0600385E RID: 14430 RVA: 0x000665BC File Offset: 0x000647BC
		// (set) Token: 0x0600385F RID: 14431 RVA: 0x000665C9 File Offset: 0x000647C9
		public int Seed
		{
			get
			{
				return this.m_seed.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_seed.Value = value;
				}
			}
		}

		// Token: 0x17000CDF RID: 3295
		// (get) Token: 0x06003860 RID: 14432 RVA: 0x000665DE File Offset: 0x000647DE
		// (set) Token: 0x06003861 RID: 14433 RVA: 0x000665EB File Offset: 0x000647EB
		public byte? VisualIndexOverride
		{
			get
			{
				return this.m_visualIndexOverride.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_visualIndexOverride.Value = value;
				}
			}
		}

		// Token: 0x06003862 RID: 14434 RVA: 0x00066600 File Offset: 0x00064800
		private void Awake()
		{
			if (base.GameEntity)
			{
				base.GameEntity.SeedReplicator = this;
			}
			if (GameManager.IsServer)
			{
				this.m_seed.Value = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
		}

		// Token: 0x06003863 RID: 14435 RVA: 0x0016D7BC File Offset: 0x0016B9BC
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_seed);
			this.m_seed.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_visualIndexOverride);
			this.m_visualIndexOverride.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x0400373A RID: 14138
		private readonly SynchronizedInt m_seed = new SynchronizedInt();

		// Token: 0x0400373B RID: 14139
		private readonly SynchronizedNullableByte m_visualIndexOverride = new SynchronizedNullableByte(null);
	}
}
