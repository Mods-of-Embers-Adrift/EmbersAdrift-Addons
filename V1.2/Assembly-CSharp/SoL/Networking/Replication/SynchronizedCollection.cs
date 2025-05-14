using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Networking.Managers;

namespace SoL.Networking.Replication
{
	// Token: 0x02000482 RID: 1154
	public abstract class SynchronizedCollection<TKey, TValue> : ISynchronizedVariable
	{
		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06002046 RID: 8262 RVA: 0x00121C80 File Offset: 0x0011FE80
		// (remove) Token: 0x06002047 RID: 8263 RVA: 0x00121CB8 File Offset: 0x0011FEB8
		public event Action<SynchronizedCollection<TKey, TValue>.Operation, TKey, TValue, TValue> Changed;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06002048 RID: 8264 RVA: 0x00121CF0 File Offset: 0x0011FEF0
		// (remove) Token: 0x06002049 RID: 8265 RVA: 0x00121D28 File Offset: 0x0011FF28
		public event Action ReadComplete;

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x0600204A RID: 8266
		protected abstract bool IsEmpty { get; }

		// Token: 0x0600204B RID: 8267
		protected abstract BitBuffer WriteKey(BitBuffer buffer, TKey key);

		// Token: 0x0600204C RID: 8268
		protected abstract BitBuffer WriteValue(BitBuffer buffer, TValue value);

		// Token: 0x0600204D RID: 8269
		protected abstract TKey ReadKey(BitBuffer buffer);

		// Token: 0x0600204E RID: 8270
		protected abstract TValue ReadValue(BitBuffer buffer);

		// Token: 0x0600204F RID: 8271
		protected abstract BitBuffer WriteChangeSet(BitBuffer buffer, SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue> changeSet);

		// Token: 0x06002050 RID: 8272
		protected abstract void ReadIndexValue(BitBuffer buffer, SynchronizedCollection<TKey, TValue>.Operation op, out TKey key, out TValue value);

		// Token: 0x06002051 RID: 8273
		protected abstract void PerformReadAction(SynchronizedCollection<TKey, TValue>.Operation op, TKey key, TValue value, out TValue previous);

		// Token: 0x06002052 RID: 8274 RVA: 0x000578CF File Offset: 0x00055ACF
		public void ResetDirty()
		{
			this.m_changes.Clear();
			this.m_changed = false;
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x000578E3 File Offset: 0x00055AE3
		private void AddChangeSet(SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue> set)
		{
			this.m_revision += 1U;
			set.Revision = this.m_revision;
			set.Timestamp = DateTime.UtcNow;
			this.m_changes.Add(set);
			this.m_changed = true;
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x00121D60 File Offset: 0x0011FF60
		protected void AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation op, TKey key, TValue value)
		{
			if (!NetworkManager.IsServer)
			{
				return;
			}
			this.AddChangeSet(new SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue>
			{
				Op = op,
				Key = key,
				Value = value
			});
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x00121D9C File Offset: 0x0011FF9C
		protected void AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation op, TKey key)
		{
			if (!NetworkManager.IsServer)
			{
				return;
			}
			this.AddChangeSet(new SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue>
			{
				Op = op,
				Key = key
			});
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x00121DD0 File Offset: 0x0011FFD0
		protected void AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation op, TValue value)
		{
			if (!NetworkManager.IsServer)
			{
				return;
			}
			this.AddChangeSet(new SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue>
			{
				Op = op,
				Value = value
			});
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x00121E04 File Offset: 0x00120004
		protected void AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation op)
		{
			if (!NetworkManager.IsServer)
			{
				return;
			}
			this.AddChangeSet(new SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue>
			{
				Op = op
			});
		}

		// Token: 0x06002058 RID: 8280
		public abstract BitBuffer PackInitialData(BitBuffer buffer);

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06002059 RID: 8281 RVA: 0x0005791F File Offset: 0x00055B1F
		public bool IsDefault
		{
			get
			{
				return this.IsEmpty;
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x0600205A RID: 8282 RVA: 0x00057927 File Offset: 0x00055B27
		// (set) Token: 0x0600205B RID: 8283 RVA: 0x0005792F File Offset: 0x00055B2F
		public bool Dirty { get; private set; }

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x0600205C RID: 8284 RVA: 0x00057938 File Offset: 0x00055B38
		// (set) Token: 0x0600205D RID: 8285 RVA: 0x00057940 File Offset: 0x00055B40
		public int BitFlag { get; set; }

		// Token: 0x0600205E RID: 8286 RVA: 0x00121E30 File Offset: 0x00120030
		public void SetDirtyFlags(DateTime lastUpdate)
		{
			DateTime utcNow = DateTime.UtcNow;
			this.m_dirtyTime = lastUpdate;
			this.m_dirtyCount = 0;
			for (int i = 0; i < this.m_changes.Count; i++)
			{
				if ((utcNow - this.m_changes[i].Timestamp).TotalSeconds > 60.0)
				{
					this.m_changes.RemoveAt(i);
					i--;
				}
				else if (this.m_changes[i].Timestamp >= this.m_dirtyTime)
				{
					this.m_dirtyCount++;
				}
			}
			this.Dirty = (this.m_changed && this.m_dirtyCount > 0);
		}

		// Token: 0x0600205F RID: 8287 RVA: 0x00121EEC File Offset: 0x001200EC
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddInt(this.m_dirtyCount);
			for (int i = 0; i < this.m_changes.Count; i++)
			{
				SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue> changeSet = this.m_changes[i];
				if (changeSet.Timestamp >= this.m_dirtyTime)
				{
					buffer.AddUShort((ushort)changeSet.Op);
					buffer.AddUInt(changeSet.Revision);
					this.WriteChangeSet(buffer, changeSet);
				}
			}
			return buffer;
		}

		// Token: 0x06002060 RID: 8288 RVA: 0x00121F64 File Offset: 0x00120164
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				SynchronizedCollection<TKey, TValue>.Operation operation = (SynchronizedCollection<TKey, TValue>.Operation)buffer.ReadUShort();
				uint num3 = buffer.ReadUInt();
				TKey tkey;
				TValue tvalue;
				this.ReadIndexValue(buffer, operation, out tkey, out tvalue);
				if (operation == SynchronizedCollection<TKey, TValue>.Operation.InitialAdd || this.m_revision < num3)
				{
					this.m_revision = num3;
					TValue arg;
					this.PerformReadAction(operation, tkey, tvalue, out arg);
					if (operation == SynchronizedCollection<TKey, TValue>.Operation.InitialAdd && i == num - 1)
					{
						operation = SynchronizedCollection<TKey, TValue>.Operation.InitialAddFinal;
					}
					Action<SynchronizedCollection<TKey, TValue>.Operation, TKey, TValue, TValue> changed = this.Changed;
					if (changed != null)
					{
						changed(operation, tkey, arg, tvalue);
					}
					num2++;
				}
			}
			if (num2 > 0)
			{
				Action readComplete = this.ReadComplete;
				if (readComplete != null)
				{
					readComplete();
				}
			}
			return buffer;
		}

		// Token: 0x06002061 RID: 8289 RVA: 0x00048A92 File Offset: 0x00046C92
		public BitBuffer ReadDataFromClient(BitBuffer buffer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x0004475B File Offset: 0x0004295B
		void ISynchronizedVariable.ClearMonoReferences()
		{
		}

		// Token: 0x0400258C RID: 9612
		private const float kMaxSecondsToKeepChanges = 60f;

		// Token: 0x0400258F RID: 9615
		protected readonly List<SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue>> m_changes = new List<SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue>>(10);

		// Token: 0x04002590 RID: 9616
		protected uint m_revision;

		// Token: 0x04002591 RID: 9617
		private bool m_changed;

		// Token: 0x04002592 RID: 9618
		private int m_dirtyCount;

		// Token: 0x04002593 RID: 9619
		private DateTime m_dirtyTime = DateTime.MinValue;

		// Token: 0x02000483 RID: 1155
		public enum Operation
		{
			// Token: 0x04002597 RID: 9623
			Add,
			// Token: 0x04002598 RID: 9624
			Clear,
			// Token: 0x04002599 RID: 9625
			Insert,
			// Token: 0x0400259A RID: 9626
			RemoveAt,
			// Token: 0x0400259B RID: 9627
			Set,
			// Token: 0x0400259C RID: 9628
			InitialAdd,
			// Token: 0x0400259D RID: 9629
			InitialAddFinal
		}

		// Token: 0x02000484 RID: 1156
		protected struct ChangeSet<TCSKey, TCSValue>
		{
			// Token: 0x06002064 RID: 8292 RVA: 0x00122004 File Offset: 0x00120204
			public override string ToString()
			{
				return string.Format("ChangeSet: {0}, Op: {1}, Index: {2}, Item: {3}", new object[]
				{
					this.Revision.ToString(),
					this.Op.ToString(),
					this.Key,
					this.Value.ToString()
				});
			}

			// Token: 0x0400259E RID: 9630
			public DateTime Timestamp;

			// Token: 0x0400259F RID: 9631
			public uint Revision;

			// Token: 0x040025A0 RID: 9632
			public SynchronizedCollection<TKey, TValue>.Operation Op;

			// Token: 0x040025A1 RID: 9633
			public TCSKey Key;

			// Token: 0x040025A2 RID: 9634
			public TCSValue Value;
		}
	}
}
