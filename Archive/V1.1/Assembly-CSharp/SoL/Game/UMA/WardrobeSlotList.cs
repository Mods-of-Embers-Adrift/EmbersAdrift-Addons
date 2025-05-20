using System;
using System.Collections.Generic;
using UMA;
using UnityEngine;

namespace SoL.Game.UMA
{
	// Token: 0x02000627 RID: 1575
	[CreateAssetMenu(menuName = "SoL/Wardrobe Slot List")]
	public class WardrobeSlotList : ScriptableObject
	{
		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x060031B9 RID: 12729 RVA: 0x00062476 File Offset: 0x00060676
		private bool m_showUpdate
		{
			get
			{
				return this.m_wardrobeSlots != null && this.m_wardrobeSlots.Count > 0 && this.m_races != null && this.m_races.Length != 0;
			}
		}

		// Token: 0x060031BA RID: 12730 RVA: 0x0015DA94 File Offset: 0x0015BC94
		private void UpdateRaces()
		{
			for (int i = 0; i < this.m_races.Length; i++)
			{
				this.m_races[i].wardrobeSlots = new List<string>(this.m_wardrobeSlots);
			}
		}

		// Token: 0x04003025 RID: 12325
		[SerializeField]
		private List<string> m_wardrobeSlots;

		// Token: 0x04003026 RID: 12326
		[SerializeField]
		private RaceData[] m_races;
	}
}
