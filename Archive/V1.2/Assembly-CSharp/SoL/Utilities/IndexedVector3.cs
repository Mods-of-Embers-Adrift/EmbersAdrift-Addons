using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000290 RID: 656
	public readonly struct IndexedVector3
	{
		// Token: 0x0600140C RID: 5132 RVA: 0x000F90E0 File Offset: 0x000F72E0
		public IndexedVector3(Vector3 v3, int size, float cellSize)
		{
			float num = 1f / cellSize;
			int num2 = Mathf.FloorToInt(v3.x * num);
			int num3 = Mathf.FloorToInt(v3.y * num);
			int num4 = Mathf.FloorToInt(v3.z * num);
			this.Pos = new Vector3((float)num2 / num, (float)num3 / num, (float)num4 / num);
			this.Index = num2 * size * size + num3 * size + num4;
		}

		// Token: 0x04001C5B RID: 7259
		public readonly Vector3 Pos;

		// Token: 0x04001C5C RID: 7260
		public readonly int Index;
	}
}
