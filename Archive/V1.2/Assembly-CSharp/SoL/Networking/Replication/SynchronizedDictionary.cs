using System;
using System.Collections;
using System.Collections.Generic;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000485 RID: 1157
	public abstract class SynchronizedDictionary<TType, TKey, TValue> : SynchronizedCollection<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TType : IDictionary<TKey, TValue>
	{
		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06002065 RID: 8293 RVA: 0x00057969 File Offset: 0x00055B69
		protected override bool IsEmpty
		{
			get
			{
				return this.m_objects.Count <= 0;
			}
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x00122068 File Offset: 0x00120268
		protected override BitBuffer WriteChangeSet(BitBuffer buffer, SynchronizedCollection<TKey, TValue>.ChangeSet<TKey, TValue> changeSet)
		{
			switch (changeSet.Op)
			{
			case SynchronizedCollection<TKey, TValue>.Operation.Add:
			case SynchronizedCollection<TKey, TValue>.Operation.Set:
				this.WriteKey(buffer, changeSet.Key);
				this.WriteValue(buffer, changeSet.Value);
				break;
			case SynchronizedCollection<TKey, TValue>.Operation.Insert:
				throw new InvalidOperationException("Cannot insert on  sync dict!");
			case SynchronizedCollection<TKey, TValue>.Operation.RemoveAt:
				this.WriteKey(buffer, changeSet.Key);
				break;
			}
			return buffer;
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x001220D4 File Offset: 0x001202D4
		protected override void ReadIndexValue(BitBuffer buffer, SynchronizedCollection<TKey, TValue>.Operation op, out TKey key, out TValue value)
		{
			key = default(TKey);
			value = default(TValue);
			switch (op)
			{
			case SynchronizedCollection<TKey, TValue>.Operation.Add:
			case SynchronizedCollection<TKey, TValue>.Operation.Insert:
			case SynchronizedCollection<TKey, TValue>.Operation.Set:
			case SynchronizedCollection<TKey, TValue>.Operation.InitialAdd:
				key = this.ReadKey(buffer);
				value = this.ReadValue(buffer);
				return;
			case SynchronizedCollection<TKey, TValue>.Operation.Clear:
				break;
			case SynchronizedCollection<TKey, TValue>.Operation.RemoveAt:
				key = this.ReadKey(buffer);
				break;
			default:
				return;
			}
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x00122138 File Offset: 0x00120338
		protected override void PerformReadAction(SynchronizedCollection<TKey, TValue>.Operation op, TKey key, TValue value, out TValue previous)
		{
			previous = default(TValue);
			switch (op)
			{
			case SynchronizedCollection<TKey, TValue>.Operation.Add:
			case SynchronizedCollection<TKey, TValue>.Operation.InitialAdd:
				if (this.m_objects.ContainsKey(key))
				{
					this.m_objects[key] = value;
					return;
				}
				this.m_objects.Add(key, value);
				return;
			case SynchronizedCollection<TKey, TValue>.Operation.Clear:
				this.m_objects.Clear();
				return;
			case SynchronizedCollection<TKey, TValue>.Operation.Insert:
				break;
			case SynchronizedCollection<TKey, TValue>.Operation.RemoveAt:
				if (this.m_objects.ContainsKey(key))
				{
					previous = this.m_objects[key];
					this.m_objects.Remove(key);
				}
				break;
			case SynchronizedCollection<TKey, TValue>.Operation.Set:
				previous = this.m_objects[key];
				this.m_objects[key] = value;
				return;
			default:
				return;
			}
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x00122228 File Offset: 0x00120428
		public override BitBuffer PackInitialData(BitBuffer buffer)
		{
			buffer.AddInt(this.m_objects.Count);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this.m_objects)
			{
				buffer.AddUShort(5);
				buffer.AddUInt(this.m_revision);
				this.WriteKey(buffer, keyValuePair.Key);
				this.WriteValue(buffer, keyValuePair.Value);
			}
			return buffer;
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x00057982 File Offset: 0x00055B82
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.m_objects.GetEnumerator();
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x00057995 File Offset: 0x00055B95
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x0005799D File Offset: 0x00055B9D
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		// Token: 0x0600206D RID: 8301 RVA: 0x000579B3 File Offset: 0x00055BB3
		public void Clear()
		{
			this.m_objects.Clear();
			base.AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation.Clear);
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x000579CD File Offset: 0x00055BCD
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.m_objects.Contains(item);
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x00048A92 File Offset: 0x00046C92
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x000579E1 File Offset: 0x00055BE1
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.Remove(item.Key);
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06002071 RID: 8305 RVA: 0x000579F0 File Offset: 0x00055BF0
		public int Count
		{
			get
			{
				return this.m_objects.Count;
			}
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06002072 RID: 8306 RVA: 0x0004479C File Offset: 0x0004299C
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x00057A03 File Offset: 0x00055C03
		public void Add(TKey key, TValue value)
		{
			this.m_objects.Add(key, value);
			base.AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation.Add, key, value);
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x00057A21 File Offset: 0x00055C21
		public bool ContainsKey(TKey key)
		{
			return this.m_objects.ContainsKey(key);
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x00057A35 File Offset: 0x00055C35
		public bool Remove(TKey key)
		{
			bool flag = this.m_objects.Remove(key);
			if (flag)
			{
				base.AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation.RemoveAt, key);
			}
			return flag;
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x00057A54 File Offset: 0x00055C54
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.m_objects.TryGetValue(key, out value);
		}

		// Token: 0x170006BD RID: 1725
		public TValue this[TKey key]
		{
			get
			{
				return this.m_objects[key];
			}
			set
			{
				TValue tvalue = this.m_objects[key];
				bool flag = !tvalue.Equals(value);
				this.m_objects[key] = value;
				if (flag)
				{
					base.AddChangeSet(SynchronizedCollection<TKey, TValue>.Operation.Set, key, value);
				}
			}
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06002079 RID: 8313 RVA: 0x00057A7D File Offset: 0x00055C7D
		public ICollection<TKey> Keys
		{
			get
			{
				return this.m_objects.Keys;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x0600207A RID: 8314 RVA: 0x00057A90 File Offset: 0x00055C90
		public ICollection<TValue> Values
		{
			get
			{
				return this.m_objects.Values;
			}
		}

		// Token: 0x040025A3 RID: 9635
		protected TType m_objects;
	}
}
