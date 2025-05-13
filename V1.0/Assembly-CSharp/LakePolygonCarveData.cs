using System;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class LakePolygonCarveData
{
	// Token: 0x04000096 RID: 150
	public float distSmooth;

	// Token: 0x04000097 RID: 151
	public float minX = float.MaxValue;

	// Token: 0x04000098 RID: 152
	public float maxX = float.MinValue;

	// Token: 0x04000099 RID: 153
	public float minZ = float.MaxValue;

	// Token: 0x0400009A RID: 154
	public float maxZ = float.MinValue;

	// Token: 0x0400009B RID: 155
	public Vector4[,] distances;
}
