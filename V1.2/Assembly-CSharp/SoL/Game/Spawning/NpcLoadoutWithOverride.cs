using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000681 RID: 1665
	[Serializable]
	public class NpcLoadoutWithOverride
	{
		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06003377 RID: 13175 RVA: 0x0006365D File Offset: 0x0006185D
		private bool m_showLoadout
		{
			get
			{
				return this.m_override == null;
			}
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x0006366B File Offset: 0x0006186B
		private IEnumerable GetLoadoutProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<NpcLoadoutProfile>();
		}

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06003379 RID: 13177 RVA: 0x00063672 File Offset: 0x00061872
		public NpcLoadout Loadout
		{
			get
			{
				if (!(this.m_override != null))
				{
					return this.m_loadout;
				}
				return this.m_override.Loadout;
			}
		}

		// Token: 0x0400318A RID: 12682
		[SerializeField]
		private NpcLoadoutProfile m_override;

		// Token: 0x0400318B RID: 12683
		[SerializeField]
		private NpcLoadout m_loadout;
	}
}
