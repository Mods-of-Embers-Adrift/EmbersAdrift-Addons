using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200008E RID: 142
	[Serializable]
	public class InfoPool : ScriptableObject
	{
		// Token: 0x04000646 RID: 1606
		public IvyContainer ivyContainer;

		// Token: 0x04000647 RID: 1607
		public EditorMeshBuilder meshBuilder;

		// Token: 0x04000648 RID: 1608
		public IvyParameters ivyParameters;

		// Token: 0x04000649 RID: 1609
		public EditorIvyGrowth growth;
	}
}
