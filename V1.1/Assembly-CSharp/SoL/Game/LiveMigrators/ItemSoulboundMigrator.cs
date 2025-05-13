using System;
using System.Collections.Generic;
using SoL.Game.Objects.Containers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.LiveMigrators
{
	// Token: 0x02000B75 RID: 2933
	[CreateAssetMenu(menuName = "SoL/Live Migrators/Soulbound Items")]
	public class ItemSoulboundMigrator : BaseLiveItemMigrator
	{
		// Token: 0x06005A44 RID: 23108 RVA: 0x001EC780 File Offset: 0x001EA980
		public override bool LiveMigrate(CharacterRecord record)
		{
			if (record == null || record.Storage == null || this.m_items == null || this.m_items.Length == 0)
			{
				return false;
			}
			bool result = false;
			foreach (KeyValuePair<ContainerType, ContainerRecord> keyValuePair in record.Storage)
			{
				for (int i = 0; i < keyValuePair.Value.Instances.Count; i++)
				{
					if (keyValuePair.Value.Instances[i] != null && base.ContainsItem(keyValuePair.Value.Instances[i].ArchetypeId) && keyValuePair.Value.Instances[i].ItemData != null && !keyValuePair.Value.Instances[i].ItemData.IsSoulbound)
					{
						keyValuePair.Value.Instances[i].ItemData.MarkAsSoulbound(record);
						result = true;
					}
				}
			}
			return result;
		}
	}
}
