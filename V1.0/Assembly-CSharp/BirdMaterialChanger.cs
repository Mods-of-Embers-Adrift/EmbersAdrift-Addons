using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class BirdMaterialChanger : MonoBehaviour
{
	// Token: 0x060000B3 RID: 179 RVA: 0x00044CFD File Offset: 0x00042EFD
	private void Start()
	{
		this.ChangeMaterial();
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00044D05 File Offset: 0x00042F05
	private void ChangeMaterial()
	{
		base.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = this.materials[BirdMaterialChanger.counter];
		BirdMaterialChanger.counter++;
	}

	// Token: 0x040001F6 RID: 502
	public Material[] materials;

	// Token: 0x040001F7 RID: 503
	private static int counter;
}
