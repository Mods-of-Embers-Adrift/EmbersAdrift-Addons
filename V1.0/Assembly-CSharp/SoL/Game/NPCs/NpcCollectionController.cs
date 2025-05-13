using System;
using SoL.Game.Objects.Containers;
using SoL.Networking.Database;

namespace SoL.Game.NPCs
{
	// Token: 0x02000801 RID: 2049
	public class NpcCollectionController : BaseCollectionController
	{
		// Token: 0x17000D9C RID: 3484
		// (get) Token: 0x06003B71 RID: 15217 RVA: 0x00053500 File Offset: 0x00051700
		protected override GameEntityType EntityType
		{
			get
			{
				return GameEntityType.Npc;
			}
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x0006835A File Offset: 0x0006655A
		protected override void OnDestroy()
		{
			base.OnDestroy();
			CharacterRecord record = this.m_record;
			if (record == null)
			{
				return;
			}
			record.CleanupReferences();
		}
	}
}
