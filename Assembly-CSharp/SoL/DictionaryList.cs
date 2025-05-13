using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Utilities.Extensions;

namespace SoL
{
	// Token: 0x0200022C RID: 556
	public class DictionaryList<TKey, TValue> : IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, IEnumerable, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
	{
		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001279 RID: 4729 RVA: 0x0004F2F6 File Offset: 0x0004D4F6
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0004F303 File Offset: 0x0004D503
		public DictionaryList(bool replaceWhenPresent = false)
		{
			this.m_list = new List<TValue>(10);
			this.m_dict = new Dictionary<TKey, TValue>(10);
			this.m_replaceWhenPresent = replaceWhenPresent;
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x0004F32C File Offset: 0x0004D52C
		public DictionaryList(IEqualityComparer<TKey> comparer, bool replaceWhenPresent = false)
		{
			this.m_list = new List<TValue>(10);
			this.m_dict = new Dictionary<TKey, TValue>(10, comparer);
			this.m_replaceWhenPresent = replaceWhenPresent;
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x000E75A8 File Offset: 0x000E57A8
		public virtual void Add(TKey key, TValue value)
		{
			if (this.m_replaceWhenPresent)
			{
				this.Remove(key);
				this.m_list.Add(value);
				this.m_dict.AddOrReplace(key, value);
				return;
			}
			this.m_list.Add(value);
			this.m_dict.Add(key, value);
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x000E75F8 File Offset: 0x000E57F8
		public virtual bool Remove(TKey key)
		{
			TValue item;
			return this.m_dict.TryGetValue(key, out item) && this.m_dict.Remove(key) && this.m_list.Remove(item);
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x0004F356 File Offset: 0x0004D556
		protected bool Remove(TKey key, TValue value)
		{
			return this.m_dict.Remove(key) && this.m_list.Remove(value);
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x0004F374 File Offset: 0x0004D574
		public void Clear()
		{
			this.m_list.Clear();
			this.m_dict.Clear();
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x0004F38C File Offset: 0x0004D58C
		public bool ContainsKey(TKey key)
		{
			return this.m_dict.ContainsKey(key);
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x0004F39A File Offset: 0x0004D59A
		public bool Contains(TValue value)
		{
			return this.m_list.Contains(value);
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0004F3A8 File Offset: 0x0004D5A8
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.m_dict.TryGetValue(key, out value);
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x0004F3B7 File Offset: 0x0004D5B7
		public void ShuffleList()
		{
			this.m_list.Shuffle<TValue>();
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00048A92 File Offset: 0x00046C92
		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x0004F3C4 File Offset: 0x0004D5C4
		IEnumerator<TValue> IEnumerable<!1>.GetEnumerator()
		{
			return this.m_list.GetEnumerator();
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x00048A92 File Offset: 0x00046C92
		void ICollection<!1>.Add(TValue item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x0004F3D6 File Offset: 0x0004D5D6
		void ICollection<!1>.Clear()
		{
			this.Clear();
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x0004F39A File Offset: 0x0004D59A
		bool ICollection<!1>.Contains(TValue item)
		{
			return this.m_list.Contains(item);
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x0004F3DE File Offset: 0x0004D5DE
		void ICollection<!1>.CopyTo(TValue[] array, int arrayIndex)
		{
			this.m_list.CopyTo(array, arrayIndex);
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x00048A92 File Offset: 0x00046C92
		bool ICollection<!1>.Remove(TValue item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x0600128B RID: 4747 RVA: 0x0004F2F6 File Offset: 0x0004D4F6
		int ICollection<!1>.Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x0600128C RID: 4748 RVA: 0x0004479C File Offset: 0x0004299C
		bool ICollection<!1>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0004F3ED File Offset: 0x0004D5ED
		int IList<!1>.IndexOf(TValue item)
		{
			return this.m_list.IndexOf(item);
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x00048A92 File Offset: 0x00046C92
		void IList<!1>.Insert(int index, TValue item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x00048A92 File Offset: 0x00046C92
		void IList<!1>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170004D8 RID: 1240
		public TValue this[int index]
		{
			get
			{
				return this.m_list[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x0004F409 File Offset: 0x0004D609
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return this.m_dict.GetEnumerator();
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0004F41B File Offset: 0x0004D61B
		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x0004F3D6 File Offset: 0x0004D5D6
		void ICollection<KeyValuePair<!0, !1>>.Clear()
		{
			this.Clear();
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0004F431 File Offset: 0x0004D631
		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.ContainsKey(item.Key) && this.Contains(item.Value);
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x00048A92 File Offset: 0x00046C92
		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x0004F451 File Offset: 0x0004D651
		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.Remove(item.Key);
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06001298 RID: 4760 RVA: 0x0004F460 File Offset: 0x0004D660
		int ICollection<KeyValuePair<!0, !1>>.Count
		{
			get
			{
				return this.m_dict.Count;
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06001299 RID: 4761 RVA: 0x0004479C File Offset: 0x0004299C
		bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0004F46D File Offset: 0x0004D66D
		void IDictionary<!0, !1>.Add(TKey key, TValue value)
		{
			this.Add(key, value);
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x0004F477 File Offset: 0x0004D677
		bool IDictionary<!0, !1>.ContainsKey(TKey key)
		{
			return this.ContainsKey(key);
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x0004F480 File Offset: 0x0004D680
		bool IDictionary<!0, !1>.Remove(TKey key)
		{
			return this.Remove(key);
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x0004F489 File Offset: 0x0004D689
		bool IDictionary<!0, !1>.TryGetValue(TKey key, out TValue value)
		{
			return this.TryGetValue(key, out value);
		}

		// Token: 0x170004DB RID: 1243
		public TValue this[TKey key]
		{
			get
			{
				return this.m_dict[key];
			}
			set
			{
				TValue item;
				if (this.m_dict.TryGetValue(key, out item))
				{
					this.m_list.Remove(item);
					this.m_dict[key] = value;
					this.m_list.Add(value);
				}
			}
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x060012A0 RID: 4768 RVA: 0x0004F4A1 File Offset: 0x0004D6A1
		ICollection<TKey> IDictionary<!0, !1>.Keys
		{
			get
			{
				return this.m_dict.Keys;
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x060012A1 RID: 4769 RVA: 0x0004F4AE File Offset: 0x0004D6AE
		ICollection<TValue> IDictionary<!0, !1>.Values
		{
			get
			{
				return this.m_dict.Values;
			}
		}

		// Token: 0x04001077 RID: 4215
		private const int kStartingCapacity = 10;

		// Token: 0x04001078 RID: 4216
		private readonly List<TValue> m_list;

		// Token: 0x04001079 RID: 4217
		private readonly Dictionary<TKey, TValue> m_dict;

		// Token: 0x0400107A RID: 4218
		protected readonly bool m_replaceWhenPresent;
	}
}
