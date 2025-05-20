using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class Rotator : MonoBehaviour
{
	// Token: 0x06000159 RID: 345 RVA: 0x00045188 File Offset: 0x00043388
	private void OnEnable()
	{
		base.InvokeRepeating("Rotate", 0f, 0.0167f);
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0004519F File Offset: 0x0004339F
	private void OnDisable()
	{
		base.CancelInvoke();
	}

	// Token: 0x0600015B RID: 347 RVA: 0x000451A7 File Offset: 0x000433A7
	private void Rotate()
	{
		base.transform.localEulerAngles += new Vector3(this.x, this.y, this.z);
	}

	// Token: 0x04000337 RID: 823
	public float x;

	// Token: 0x04000338 RID: 824
	public float y;

	// Token: 0x04000339 RID: 825
	public float z;
}
