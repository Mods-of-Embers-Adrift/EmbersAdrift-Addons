using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000780 RID: 1920
	[CreateAssetMenu(menuName = "SoL/BulletinBoards/BulletinBoard")]
	public class BulletinBoard : Identifiable
	{
		// Token: 0x17000CEE RID: 3310
		// (get) Token: 0x0600388F RID: 14479 RVA: 0x0006678B File Offset: 0x0006498B
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000CEF RID: 3311
		// (get) Token: 0x06003890 RID: 14480 RVA: 0x00066793 File Offset: 0x00064993
		public ZoneId ZoneId
		{
			get
			{
				return this.m_zoneId;
			}
		}

		// Token: 0x17000CF0 RID: 3312
		// (get) Token: 0x06003891 RID: 14481 RVA: 0x0006679B File Offset: 0x0006499B
		public SubZoneId SubZoneId
		{
			get
			{
				return this.m_subZoneId;
			}
		}

		// Token: 0x17000CF1 RID: 3313
		// (get) Token: 0x06003892 RID: 14482 RVA: 0x00049FFA File Offset: 0x000481FA
		public List<BBTask> Tasks
		{
			get
			{
				return null;
			}
		}

		// Token: 0x04003770 RID: 14192
		[SerializeField]
		private string m_title;

		// Token: 0x04003771 RID: 14193
		[SerializeField]
		private ZoneId m_zoneId;

		// Token: 0x04003772 RID: 14194
		[SerializeField]
		private SubZoneId m_subZoneId;
	}
}
