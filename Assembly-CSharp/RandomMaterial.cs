using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class RandomMaterial : MonoBehaviour
{
	// Token: 0x060000B6 RID: 182 RVA: 0x00044D2A File Offset: 0x00042F2A
	public void Start()
	{
		this.ChangeMaterial();
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00044D32 File Offset: 0x00042F32
	public void ChangeMaterial()
	{
		this.targetRenderer.sharedMaterial = this.materials[UnityEngine.Random.Range(0, this.materials.Length)];
	}

	// Token: 0x040001F8 RID: 504
	public Renderer targetRenderer;

	// Token: 0x040001F9 RID: 505
	public Material[] materials;
}
