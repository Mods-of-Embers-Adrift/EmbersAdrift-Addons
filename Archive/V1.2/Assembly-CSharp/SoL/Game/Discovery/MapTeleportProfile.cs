using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CAF RID: 3247
	public class MapTeleportProfile : DiscoveryProfile
	{
		// Token: 0x17001785 RID: 6021
		// (get) Token: 0x06006281 RID: 25217 RVA: 0x000824DA File Offset: 0x000806DA
		public int EssenceCost
		{
			get
			{
				return GlobalSettings.Values.Ashen.EmberRingFromMonolithEssenceCost;
			}
		}

		// Token: 0x040055F4 RID: 22004
		[SerializeField]
		private int m_essenceCost;
	}
}
