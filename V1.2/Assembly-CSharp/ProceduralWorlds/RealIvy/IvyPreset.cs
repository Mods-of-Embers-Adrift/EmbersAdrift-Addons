using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000096 RID: 150
	[Serializable]
	public class IvyPreset : ScriptableObject
	{
		// Token: 0x060005D4 RID: 1492 RVA: 0x0004708C File Offset: 0x0004528C
		public void CopyFrom(IvyParametersGUI copyFrom)
		{
			this.ivyParameters.CopyFrom(copyFrom);
		}

		// Token: 0x040006B3 RID: 1715
		public string presetName;

		// Token: 0x040006B4 RID: 1716
		public IvyParameters ivyParameters;
	}
}
