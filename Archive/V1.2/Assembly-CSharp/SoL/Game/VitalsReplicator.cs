using System;
using SoL.Game.EffectSystem;
using SoL.Networking.Replication;

namespace SoL.Game
{
	// Token: 0x020005FB RID: 1531
	public abstract class VitalsReplicator : SyncVarReplicator
	{
		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x060030F9 RID: 12537 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual SynchronizedByte StaminaSyncVar
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060030FA RID: 12538 RVA: 0x00061BEE File Offset: 0x0005FDEE
		private void Awake()
		{
			base.GameEntity.VitalsReplicator = this;
			this.CurrentStance.PermitClientToModify(this);
		}

		// Token: 0x060030FB RID: 12539 RVA: 0x0015BDA4 File Offset: 0x00159FA4
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.BehaviorFlags);
			this.BehaviorFlags.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.CurrentHealthState);
			this.CurrentHealthState.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.CurrentStance);
			this.CurrentStance.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Effects);
			this.Effects.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002F3E RID: 12094
		public readonly SynchronizedEnum<Stance> CurrentStance = new SynchronizedEnum<Stance>(Stance.Idle);

		// Token: 0x04002F3F RID: 12095
		public readonly SynchronizedEnum<HealthState> CurrentHealthState = new SynchronizedEnum<HealthState>(HealthState.Alive);

		// Token: 0x04002F40 RID: 12096
		public readonly SynchronizedEnum<BehaviorEffectTypeFlags> BehaviorFlags = new SynchronizedEnum<BehaviorEffectTypeFlags>(BehaviorEffectTypeFlags.None);

		// Token: 0x04002F41 RID: 12097
		public readonly SynchronizedDictionaryList<UniqueId, EffectSyncData> Effects = new SynchronizedDictionaryList<UniqueId, EffectSyncData>(default(UniqueIdComparer));
	}
}
