using System;
using UnityEngine;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009D8 RID: 2520
	public class NameplateOffsetUpdater : GameEntityComponent
	{
		// Token: 0x06004CB0 RID: 19632 RVA: 0x00073DEA File Offset: 0x00071FEA
		private void Start()
		{
			if (base.GameEntity == null)
			{
				base.enabled = false;
				return;
			}
			base.GameEntity.UseFullNameplateHeightOffset = true;
		}

		// Token: 0x06004CB1 RID: 19633 RVA: 0x001BD864 File Offset: 0x001BBA64
		private void Update()
		{
			if (base.GameEntity)
			{
				Vector3 value = base.gameObject.transform.position - base.GameEntity.gameObject.transform.position;
				base.GameEntity.NameplateHeightOffset = new Vector3?(value);
			}
		}
	}
}
