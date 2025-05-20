using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F4 RID: 2548
	public abstract class IdentifiableScriptableObjectCollection<T> : ScriptableObject where T : Identifiable
	{
		// Token: 0x06004D7B RID: 19835 RVA: 0x001C0B40 File Offset: 0x001BED40
		protected virtual void Initialize()
		{
			if (this.m_dict != null)
			{
				return;
			}
			this.m_dict = new Dictionary<UniqueId, T>(default(UniqueIdComparer));
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (this.ShouldAddToCollection(this.m_items[i]))
				{
					this.m_dict.Add(this.m_items[i].Id, this.m_items[i]);
				}
			}
		}

		// Token: 0x06004D7C RID: 19836 RVA: 0x00074594 File Offset: 0x00072794
		protected virtual bool ShouldAddToCollection(T item)
		{
			return item != null && item.Id != UniqueId.Empty;
		}

		// Token: 0x06004D7D RID: 19837 RVA: 0x001C0BC4 File Offset: 0x001BEDC4
		public T GetItem(UniqueId id)
		{
			this.Initialize();
			T result = default(T);
			this.m_dict.TryGetValue(id, out result);
			return result;
		}

		// Token: 0x06004D7E RID: 19838 RVA: 0x000745BB File Offset: 0x000727BB
		public T GetItem(string id)
		{
			return this.GetItem(new UniqueId(id));
		}

		// Token: 0x06004D7F RID: 19839 RVA: 0x000745C9 File Offset: 0x000727C9
		public bool TryGetItem(UniqueId id, out T item)
		{
			this.Initialize();
			return this.m_dict.TryGetValue(id, out item);
		}

		// Token: 0x06004D80 RID: 19840 RVA: 0x000745DE File Offset: 0x000727DE
		public bool TryGetItem(string id, out T item)
		{
			return this.TryGetItem(new UniqueId(id), out item);
		}

		// Token: 0x06004D81 RID: 19841 RVA: 0x000745ED File Offset: 0x000727ED
		public IEnumerable<T> GetAllItems()
		{
			int num;
			for (int i = 0; i < this.m_items.Length; i = num + 1)
			{
				yield return this.m_items[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x04004721 RID: 18209
		[SerializeField]
		protected T[] m_items;

		// Token: 0x04004722 RID: 18210
		protected Dictionary<UniqueId, T> m_dict;
	}
}
