using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class BakedMeshAnimation : MonoBehaviour
{
	// Token: 0x040001C7 RID: 455
	public Mesh[] meshes;

	// Token: 0x040001C8 RID: 456
	public float playSpeed = 30f;

	// Token: 0x040001C9 RID: 457
	[HideInInspector]
	public Renderer rendererComponent;

	// Token: 0x040001CA RID: 458
	public bool randomStartFrame = true;

	// Token: 0x040001CB RID: 459
	public bool loop = true;

	// Token: 0x040001CC RID: 460
	public bool pingPong;

	// Token: 0x040001CD RID: 461
	public bool playOnAwake = true;

	// Token: 0x040001CE RID: 462
	public Transform transformCache;

	// Token: 0x040001CF RID: 463
	public int transitionFrame;

	// Token: 0x040001D0 RID: 464
	public int crossfadeFrame;
}
