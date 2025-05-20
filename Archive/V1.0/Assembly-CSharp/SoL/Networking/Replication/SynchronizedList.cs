using System;
using System.Collections;
using System.Collections.Generic;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000489 RID: 1161
	public abstract class SynchronizedList<TValue> : SynchronizedCollection<int, TValue>, IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, IEnumerable
	{
		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06002098 RID: 8344 RVA: 0x00057B74 File Offset: 0x00055D74
		protected override bool IsEmpty
		{
			get
			{
				return this.m_objects.Count <= 0;
			}
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x00057ABE File Offset: 0x00055CBE
		protected override BitBuffer WriteKey(BitBuffer buffer, int key)
		{
			buffer.AddInt(key);
			return buffer;
		}

		// Token: 0x0600209A RID: 8346 RVA: 0x00057ADB File Offset: 0x00055CDB
		protected override int ReadKey(BitBuffer buffer)
		{
			return buffer.ReadInt();
		}

		// Token: 0x0600209B RID: 8347 RVA: 0x0012233C File Offset: 0x0012053C
		protected override BitBuffer WriteChangeSet(BitBuffer buffer, SynchronizedCollection<int, TValue>.ChangeSet<int, TValue> changeSet)
		{
			switch (changeSet.Op)
			{
			case SynchronizedCollection<int, TValue>.Operation.Add:
				this.WriteValue(buffer, changeSet.Value);
				break;
			case SynchronizedCollection<int, TValue>.Operation.Insert:
			case SynchronizedCollection<int, TValue>.Operation.Set:
				this.WriteKey(buffer, changeSet.Key);
				this.WriteValue(buffer, changeSet.Value);
				break;
			case SynchronizedCollection<int, TValue>.Operation.RemoveAt:
				this.WriteKey(buffer, changeSet.Key);
				break;
			}
			return buffer;
		}

		// Token: 0x0600209C RID: 8348 RVA: 0x001223AC File Offset: 0x001205AC
		protected override void ReadIndexValue(BitBuffer buffer, SynchronizedCollection<int, TValue>.Operation op, out int key, out TValue value)
		{
			key = -1;
			value = default(TValue);
			switch (op)
			{
			case SynchronizedCollection<int, TValue>.Operation.Add:
			case SynchronizedCollection<int, TValue>.Operation.InitialAdd:
				value = this.ReadValue(buffer);
				return;
			case SynchronizedCollection<int, TValue>.Operation.Clear:
				break;
			case SynchronizedCollection<int, TValue>.Operation.Insert:
				key = this.ReadKey(buffer);
				value = this.ReadValue(buffer);
				return;
			case SynchronizedCollection<int, TValue>.Operation.RemoveAt:
				key = this.ReadKey(buffer);
				break;
			case SynchronizedCollection<int, TValue>.Operation.Set:
				key = this.ReadKey(buffer);
				value = this.ReadValue(buffer);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600209D RID: 8349 RVA: 0x0012242C File Offset: 0x0012062C
		protected override void PerformReadAction(SynchronizedCollection<int, TValue>.Operation op, int key, TValue value, out TValue previous)
		{
			previous = default(TValue);
			switch (op)
			{
			case SynchronizedCollection<int, TValue>.Operation.Add:
			case SynchronizedCollection<int, TValue>.Operation.InitialAdd:
				this.m_objects.Add(value);
				key = this.m_objects.Count - 1;
				return;
			case SynchronizedCollection<int, TValue>.Operation.Clear:
				this.m_objects.Clear();
				return;
			case SynchronizedCollection<int, TValue>.Operation.Insert:
				this.m_objects.Insert(key, value);
				return;
			case SynchronizedCollection<int, TValue>.Operation.RemoveAt:
				if (key < this.m_objects.Count)
				{
					previous = this.m_objects[key];
					this.m_objects.RemoveAt(key);
					return;
				}
				throw new ArgumentOutOfRangeException(string.Format("SYNCLIST: Cannot remove index {0} from the SyncList of count {1}", key, this.m_objects.Count));
			case SynchronizedCollection<int, TValue>.Operation.Set:
				previous = this.m_objects[key];
				this.m_objects[key] = value;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600209E RID: 8350 RVA: 0x0012250C File Offset: 0x0012070C
		public override BitBuffer PackInitialData(BitBuffer buffer)
		{
			buffer.AddInt(this.m_objects.Count);
			for (int i = 0; i < this.m_objects.Count; i++)
			{
				buffer.AddUShort(5);
				buffer.AddUInt(this.m_revision);
				this.WriteValue(buffer, this.m_objects[i]);
			}
			return buffer;
		}

		// Token: 0x0600209F RID: 8351 RVA: 0x00057B87 File Offset: 0x00055D87
		public IEnumerator<TValue> GetEnumerator()
		{
			return this.m_objects.GetEnumerator();
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x00057B99 File Offset: 0x00055D99
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x00057BA1 File Offset: 0x00055DA1
		public void Add(TValue item)
		{
			this.m_objects.Add(item);
			base.AddChangeSet(SynchronizedCollection<int, TValue>.Operation.Add, item);
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x00057BB7 File Offset: 0x00055DB7
		public void Clear()
		{
			this.m_objects.Clear();
			base.AddChangeSet(SynchronizedCollection<int, TValue>.Operation.Clear);
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x00057BCB File Offset: 0x00055DCB
		public bool Contains(TValue item)
		{
			return this.m_objects.Contains(item);
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x00057BD9 File Offset: 0x00055DD9
		public void CopyTo(TValue[] array, int arrayIndex)
		{
			this.m_objects.CopyTo(array, arrayIndex);
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x0012256C File Offset: 0x0012076C
		public bool Remove(TValue item)
		{
			int num = this.IndexOf(item);
			if (num != -1)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x060020A6 RID: 8358 RVA: 0x00057BE8 File Offset: 0x00055DE8
		public int Count
		{
			get
			{
				return this.m_objects.Count;
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x060020A7 RID: 8359 RVA: 0x0004479C File Offset: 0x0004299C
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x00057BF5 File Offset: 0x00055DF5
		public int IndexOf(TValue item)
		{
			return this.m_objects.IndexOf(item);
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x00057C03 File Offset: 0x00055E03
		public void Insert(int index, TValue item)
		{
			this.m_objects.Insert(index, item);
			base.AddChangeSet(SynchronizedCollection<int, TValue>.Operation.Insert, index, item);
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x00057C1B File Offset: 0x00055E1B
		public void RemoveAt(int index)
		{
			this.m_objects.RemoveAt(index);
			base.AddChangeSet(SynchronizedCollection<int, TValue>.Operation.RemoveAt, index);
		}

		// Token: 0x170006C6 RID: 1734
		public TValue this[int index]
		{
			get
			{
				return this.m_objects[index];
			}
			set
			{
				TValue tvalue = this.m_objects[index];
				bool flag = !tvalue.Equals(value);
				this.m_objects[index] = value;
				if (flag)
				{
					base.AddChangeSet(SynchronizedCollection<int, TValue>.Operation.Set, index, value);
				}
			}
		}

		// Token: 0x040025A4 RID: 9636
		private readonly List<TValue> m_objects = new List<TValue>();
	}
}
