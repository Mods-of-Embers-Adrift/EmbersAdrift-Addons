using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Managers;

namespace SoL.Networking.Replication
{
	// Token: 0x02000490 RID: 1168
	public abstract class SynchronizedVariable<T> : ISynchronizedVariable
	{
		// Token: 0x14000032 RID: 50
		// (add) Token: 0x060020C0 RID: 8384 RVA: 0x001225D8 File Offset: 0x001207D8
		// (remove) Token: 0x060020C1 RID: 8385 RVA: 0x00122610 File Offset: 0x00120810
		public event Action<T> Changed;

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x060020C2 RID: 8386 RVA: 0x00057CDE File Offset: 0x00055EDE
		private bool m_serverIsAuthority
		{
			get
			{
				return this.m_syncVarReplicator == null;
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x060020C3 RID: 8387 RVA: 0x00057CEC File Offset: 0x00055EEC
		private bool m_localPlayerIsAuthority
		{
			get
			{
				return this.m_syncVarReplicator != null && this.m_syncVarReplicator.GameEntity.NetworkEntity.IsLocal;
			}
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x060020C4 RID: 8388 RVA: 0x00057D13 File Offset: 0x00055F13
		// (set) Token: 0x060020C5 RID: 8389 RVA: 0x00122648 File Offset: 0x00120848
		public T Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (EqualityComparer<T>.Default.Equals(this.m_value, value))
				{
					return;
				}
				this.m_value = value;
				this.m_lastUpdate = DateTime.UtcNow;
				this.m_changed = true;
				this.InvokeChanged();
				if (!GameManager.IsServer && this.m_localPlayerIsAuthority)
				{
					this.m_syncVarReplicator.SetClientVariable(this);
				}
			}
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x00057D1B File Offset: 0x00055F1B
		protected SynchronizedVariable()
		{
			this.m_value = default(T);
			this.m_defaultValue = default(T);
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x00057D3B File Offset: 0x00055F3B
		protected SynchronizedVariable(T initial)
		{
			this.m_value = initial;
			this.m_defaultValue = initial;
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x00057D51 File Offset: 0x00055F51
		public void SetInitialValue(T initial)
		{
			this.Value = initial;
			this.ResetDirty();
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x00057D60 File Offset: 0x00055F60
		public void ResetDirty()
		{
			this.m_lastUpdate = DateTime.UtcNow;
			this.m_changed = false;
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x00057D74 File Offset: 0x00055F74
		private void InvokeChanged()
		{
			Action<T> changed = this.Changed;
			if (changed == null)
			{
				return;
			}
			changed(this.m_value);
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x00057D8C File Offset: 0x00055F8C
		public void PermitClientToModify(SyncVarReplicator replicator)
		{
			this.m_syncVarReplicator = replicator;
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x00057D95 File Offset: 0x00055F95
		public BitBuffer ReadDataFromClient(BitBuffer buffer)
		{
			if (this.m_syncVarReplicator)
			{
				this.Value = this.ReadDataInternal(buffer);
			}
			return buffer;
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x060020CD RID: 8397 RVA: 0x00057DB2 File Offset: 0x00055FB2
		public bool IsDefault
		{
			get
			{
				return EqualityComparer<T>.Default.Equals(this.m_value, this.m_defaultValue);
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060020CE RID: 8398 RVA: 0x00057DCA File Offset: 0x00055FCA
		// (set) Token: 0x060020CF RID: 8399 RVA: 0x00057DD2 File Offset: 0x00055FD2
		public bool Dirty { get; private set; }

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060020D0 RID: 8400 RVA: 0x00057DDB File Offset: 0x00055FDB
		// (set) Token: 0x060020D1 RID: 8401 RVA: 0x00057DE3 File Offset: 0x00055FE3
		public int BitFlag { get; set; }

		// Token: 0x060020D2 RID: 8402 RVA: 0x00057DEC File Offset: 0x00055FEC
		public void SetDirtyFlags(DateTime timestamp)
		{
			this.Dirty = (this.m_changed && timestamp <= this.m_lastUpdate);
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x00057E0B File Offset: 0x0005600B
		public BitBuffer PackData(BitBuffer buffer)
		{
			this.PackDataInternal(buffer);
			return buffer;
		}

		// Token: 0x060020D4 RID: 8404 RVA: 0x00057E16 File Offset: 0x00056016
		public BitBuffer PackInitialData(BitBuffer buffer)
		{
			return this.PackDataInternal(buffer);
		}

		// Token: 0x060020D5 RID: 8405 RVA: 0x001226A4 File Offset: 0x001208A4
		public BitBuffer ReadData(BitBuffer buffer)
		{
			T value = this.ReadDataInternal(buffer);
			if (this.m_serverIsAuthority || !this.m_localPlayerIsAuthority)
			{
				this.Value = value;
			}
			return buffer;
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x00057E1F File Offset: 0x0005601F
		void ISynchronizedVariable.ClearMonoReferences()
		{
			this.m_syncVarReplicator = null;
		}

		// Token: 0x060020D7 RID: 8407
		protected abstract BitBuffer PackDataInternal(BitBuffer buffer);

		// Token: 0x060020D8 RID: 8408
		protected abstract T ReadDataInternal(BitBuffer buffer);

		// Token: 0x060020D9 RID: 8409 RVA: 0x00057E28 File Offset: 0x00056028
		public static implicit operator T(SynchronizedVariable<T> sv)
		{
			return sv.Value;
		}

		// Token: 0x040025A6 RID: 9638
		private SyncVarReplicator m_syncVarReplicator;

		// Token: 0x040025A7 RID: 9639
		private DateTime m_lastUpdate;

		// Token: 0x040025A8 RID: 9640
		private bool m_changed;

		// Token: 0x040025A9 RID: 9641
		private T m_value;

		// Token: 0x040025AA RID: 9642
		private readonly T m_defaultValue;
	}
}
