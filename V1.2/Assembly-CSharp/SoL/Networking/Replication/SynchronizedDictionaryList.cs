using System;
using System.Collections;
using System.Collections.Generic;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000488 RID: 1160
	public class SynchronizedDictionaryList<TKey, TValue> : SynchronizedDictionary<DictionaryList<TKey, TValue>, TKey, TValue>, IList<!1>, ICollection<!1>, IEnumerable<!1>, IEnumerable where TKey : INetworkSerializable, new() where TValue : INetworkSerializable, new()
	{
		// Token: 0x06002086 RID: 8326 RVA: 0x00057B09 File Offset: 0x00055D09
		public SynchronizedDictionaryList(IEqualityComparer<TKey> comparer)
		{
			this.m_objects = new DictionaryList<TKey, TValue>(comparer, false);
		}

		// Token: 0x06002087 RID: 8327 RVA: 0x00057AC9 File Offset: 0x00055CC9
		protected override BitBuffer WriteKey(BitBuffer buffer, TKey key)
		{
			key.PackData(buffer);
			return buffer;
		}

		// Token: 0x06002088 RID: 8328 RVA: 0x00057B1E File Offset: 0x00055D1E
		protected override BitBuffer WriteValue(BitBuffer buffer, TValue value)
		{
			value.PackData(buffer);
			return buffer;
		}

		// Token: 0x06002089 RID: 8329 RVA: 0x00122314 File Offset: 0x00120514
		protected override TKey ReadKey(BitBuffer buffer)
		{
			TKey result = Activator.CreateInstance<TKey>();
			result.ReadData(buffer);
			return result;
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x00122338 File Offset: 0x00120538
		protected override TValue ReadValue(BitBuffer buffer)
		{
			TValue result = Activator.CreateInstance<TValue>();
			result.ReadData(buffer);
			return result;
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x00057B30 File Offset: 0x00055D30
		IEnumerator<TValue> IEnumerable<!1>.GetEnumerator()
		{
			return ((IEnumerable<TValue>)this.m_objects).GetEnumerator();
		}

		// Token: 0x0600208C RID: 8332 RVA: 0x00048A92 File Offset: 0x00046C92
		void ICollection<!1>.Add(TValue item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x0004475B File Offset: 0x0004295B
		void ICollection<!1>.Clear()
		{
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x00057B3D File Offset: 0x00055D3D
		bool ICollection<!1>.Contains(TValue item)
		{
			return this.m_objects.Contains(item);
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x0004475B File Offset: 0x0004295B
		void ICollection<!1>.CopyTo(TValue[] array, int arrayIndex)
		{
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x00048A92 File Offset: 0x00046C92
		bool ICollection<!1>.Remove(TValue item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06002091 RID: 8337 RVA: 0x00057B4B File Offset: 0x00055D4B
		int ICollection<!1>.Count
		{
			get
			{
				return this.m_objects.Count;
			}
		}

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06002092 RID: 8338 RVA: 0x0004479C File Offset: 0x0004299C
		bool ICollection<!1>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002093 RID: 8339 RVA: 0x00057B58 File Offset: 0x00055D58
		int IList<!1>.IndexOf(TValue item)
		{
			return ((IList<TValue>)this.m_objects).IndexOf(item);
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x00048A92 File Offset: 0x00046C92
		void IList<!1>.Insert(int index, TValue item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002095 RID: 8341 RVA: 0x00048A92 File Offset: 0x00046C92
		void IList<!1>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170006C2 RID: 1730
		public TValue this[int index]
		{
			get
			{
				return this.m_objects[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
