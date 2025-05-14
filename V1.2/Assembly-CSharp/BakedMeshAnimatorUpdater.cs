using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class BakedMeshAnimatorUpdater : MonoBehaviour
{
	// Token: 0x060000B0 RID: 176 RVA: 0x00093FC4 File Offset: 0x000921C4
	private void Start()
	{
		if (this.updateChildren)
		{
			this.children = base.transform.GetComponentsInChildren<BakedMeshAnimator>();
			for (int i = 0; i < this.children.Length; i++)
			{
				if (this.randomizeSpeed)
				{
					this.children[i].SetSpeedMultiplier(UnityEngine.Random.Range(this.minSpeedMultiplier, this.maxSpeedMultiplier));
				}
			}
			return;
		}
		this.animatedMesh = base.GetComponent<BakedMeshAnimator>();
		if (this.randomizeSpeed)
		{
			this.animatedMesh.SetSpeedMultiplier(UnityEngine.Random.Range(this.minSpeedMultiplier, this.maxSpeedMultiplier));
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00094054 File Offset: 0x00092254
	private void Update()
	{
		if (this.updateChildren)
		{
			for (int i = 0; i < this.children.Length; i++)
			{
				this.children[i].AnimateUpdate();
			}
			return;
		}
		this.animatedMesh.AnimateUpdate();
	}

	// Token: 0x040001F0 RID: 496
	private BakedMeshAnimator animatedMesh;

	// Token: 0x040001F1 RID: 497
	private BakedMeshAnimator[] children;

	// Token: 0x040001F2 RID: 498
	public bool updateChildren;

	// Token: 0x040001F3 RID: 499
	public bool randomizeSpeed;

	// Token: 0x040001F4 RID: 500
	public float minSpeedMultiplier = 1f;

	// Token: 0x040001F5 RID: 501
	public float maxSpeedMultiplier = 1f;
}
