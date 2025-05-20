using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class NavMeshCleaner : MonoBehaviour
{
	// Token: 0x040001A8 RID: 424
	public List<Vector3> m_WalkablePoint = new List<Vector3>();

	// Token: 0x040001A9 RID: 425
	public float m_Height = 1f;

	// Token: 0x040001AA RID: 426
	public float m_Offset;

	// Token: 0x040001AB RID: 427
	public int m_MidLayerCount = 3;

	// Token: 0x040001AC RID: 428
	public Material m_material;

	// Token: 0x040001AD RID: 429
	[SerializeField]
	private NavMeshCleaner m_copyPointsFrom;
}
