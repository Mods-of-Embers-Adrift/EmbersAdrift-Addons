using System;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007F5 RID: 2037
	[CreateAssetMenu(menuName = "SoL/Profiles/Area Cost Overrides")]
	public class AreaCostOverrideProfile : ScriptableObject
	{
		// Token: 0x17000D8B RID: 3467
		// (get) Token: 0x06003B36 RID: 15158 RVA: 0x00068180 File Offset: 0x00066380
		internal NpcMotor.NavMeshAreaCost[] AreaCostOverrides
		{
			get
			{
				return this.m_areaCostOverrides;
			}
		}

		// Token: 0x040039A6 RID: 14758
		[SerializeField]
		private NpcMotor.NavMeshAreaCost[] m_areaCostOverrides;
	}
}
