using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC2 RID: 2754
	[Serializable]
	public class ComponentSelection
	{
		// Token: 0x04004B7E RID: 19326
		public bool Selected;

		// Token: 0x04004B7F RID: 19327
		public UniqueId ComponentId;

		// Token: 0x04004B80 RID: 19328
		[NonSerialized]
		public string ComponentName;
	}
}
