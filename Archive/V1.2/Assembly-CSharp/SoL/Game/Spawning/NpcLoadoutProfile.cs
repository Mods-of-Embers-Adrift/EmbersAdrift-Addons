using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000685 RID: 1669
	[CreateAssetMenu(menuName = "SoL/Profiles/Npc Loadout")]
	public class NpcLoadoutProfile : ScriptableObject
	{
		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x06003394 RID: 13204 RVA: 0x00063781 File Offset: 0x00061981
		public NpcLoadout Loadout
		{
			get
			{
				return this.m_loadout;
			}
		}

		// Token: 0x0400319B RID: 12699
		[SerializeField]
		private NpcLoadout m_loadout;
	}
}
