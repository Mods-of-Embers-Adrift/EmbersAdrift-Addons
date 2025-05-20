using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A8B RID: 2699
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Light Source")]
	public class LightItem : EquipableItem
	{
		// Token: 0x1700130D RID: 4877
		// (get) Token: 0x0600539A RID: 21402 RVA: 0x000447AA File Offset: 0x000429AA
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.LightSource;
			}
		}
	}
}
