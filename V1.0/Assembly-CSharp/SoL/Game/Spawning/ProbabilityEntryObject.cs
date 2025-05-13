using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x0200069F RID: 1695
	[Serializable]
	public abstract class ProbabilityEntryObject<T> : ProbabilityEntry<T> where T : UnityEngine.Object
	{
		// Token: 0x060033D7 RID: 13271 RVA: 0x00063978 File Offset: 0x00061B78
		protected override IEnumerable GetDropdownEntries()
		{
			return SolOdinUtilities.GetDropdownItems<T>();
		}
	}
}
