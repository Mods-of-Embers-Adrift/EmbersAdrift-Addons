using System;
using System.Collections.Generic;

namespace SoL.Utilities.NodeGraph
{
	// Token: 0x02000310 RID: 784
	[Serializable]
	public class CommentBlockData
	{
		// Token: 0x04001DE4 RID: 7652
		public string Title = "Comment Block";

		// Token: 0x04001DE5 RID: 7653
		public float PositionX;

		// Token: 0x04001DE6 RID: 7654
		public float PositionY;

		// Token: 0x04001DE7 RID: 7655
		public List<UniqueId> ChildNodes = new List<UniqueId>();
	}
}
