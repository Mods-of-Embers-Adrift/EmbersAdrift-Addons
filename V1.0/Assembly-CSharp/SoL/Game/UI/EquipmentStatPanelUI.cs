using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200087D RID: 2173
	public class EquipmentStatPanelUI : DraggableUIWindow
	{
		// Token: 0x17000EA0 RID: 3744
		// (get) Token: 0x06003F43 RID: 16195 RVA: 0x0006AC9C File Offset: 0x00068E9C
		public EquipmentUI Equipment
		{
			get
			{
				return this.m_equipment;
			}
		}

		// Token: 0x17000EA1 RID: 3745
		// (get) Token: 0x06003F44 RID: 16196 RVA: 0x0006ACA4 File Offset: 0x00068EA4
		public TabbedStatPanelUI Stats
		{
			get
			{
				return this.m_stats;
			}
		}

		// Token: 0x17000EA2 RID: 3746
		// (get) Token: 0x06003F45 RID: 16197 RVA: 0x0006ACAC File Offset: 0x00068EAC
		public new bool Locked
		{
			get
			{
				return this.m_equipment && this.m_equipment.Locked;
			}
		}

		// Token: 0x04003D26 RID: 15654
		[SerializeField]
		private EquipmentUI m_equipment;

		// Token: 0x04003D27 RID: 15655
		[SerializeField]
		private TabbedStatPanelUI m_stats;
	}
}
