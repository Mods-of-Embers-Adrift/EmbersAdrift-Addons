using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.LiveMigrators
{
	// Token: 0x02000B74 RID: 2932
	public abstract class BaseLiveItemMigrator : ScriptableObject
	{
		// Token: 0x06005A40 RID: 23104 RVA: 0x001EC6E4 File Offset: 0x001EA8E4
		protected bool ContainsItem(UniqueId id)
		{
			if (this.m_items == null || this.m_items.Length == 0 || id.IsEmpty)
			{
				return false;
			}
			if (this.m_itemIds == null)
			{
				this.m_itemIds = new HashSet<UniqueId>(this.m_items.Length, default(UniqueIdComparer));
				for (int i = 0; i < this.m_items.Length; i++)
				{
					if (this.m_items[i])
					{
						this.m_itemIds.Add(this.m_items[i].Id);
					}
				}
			}
			return this.m_itemIds.Contains(id);
		}

		// Token: 0x06005A41 RID: 23105
		public abstract bool LiveMigrate(CharacterRecord record);

		// Token: 0x06005A42 RID: 23106 RVA: 0x0007C99B File Offset: 0x0007AB9B
		private void ResetCollection()
		{
			this.m_itemIds = null;
		}

		// Token: 0x04004F5D RID: 20317
		[SerializeField]
		protected ItemArchetype[] m_items;

		// Token: 0x04004F5E RID: 20318
		private HashSet<UniqueId> m_itemIds;
	}
}
