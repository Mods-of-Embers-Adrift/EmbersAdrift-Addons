using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class UnluckDistanceDisabler : MonoBehaviour
{
	// Token: 0x060000B9 RID: 185 RVA: 0x00094098 File Offset: 0x00092298
	public void Start()
	{
		if (this._distanceFromMainCam)
		{
			this._distanceFrom = Camera.main.transform;
		}
		base.InvokeRepeating("CheckDisable", this._disableCheckInterval + UnityEngine.Random.value * this._disableCheckInterval, this._disableCheckInterval);
		base.InvokeRepeating("CheckEnable", this._enableCheckInterval + UnityEngine.Random.value * this._enableCheckInterval, this._enableCheckInterval);
		base.Invoke("DisableOnStart", 0.01f);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00044D54 File Offset: 0x00042F54
	public void DisableOnStart()
	{
		if (this._disableOnStart)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00094118 File Offset: 0x00092318
	public void CheckDisable()
	{
		if (base.gameObject.activeInHierarchy && (base.transform.position - this._distanceFrom.position).sqrMagnitude > (float)(this._distanceDisable * this._distanceDisable))
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00094174 File Offset: 0x00092374
	public void CheckEnable()
	{
		if (!base.gameObject.activeInHierarchy && (base.transform.position - this._distanceFrom.position).sqrMagnitude < (float)(this._distanceDisable * this._distanceDisable))
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x040001FA RID: 506
	public int _distanceDisable = 1000;

	// Token: 0x040001FB RID: 507
	public Transform _distanceFrom;

	// Token: 0x040001FC RID: 508
	public bool _distanceFromMainCam;

	// Token: 0x040001FD RID: 509
	public float _disableCheckInterval = 10f;

	// Token: 0x040001FE RID: 510
	public float _enableCheckInterval = 1f;

	// Token: 0x040001FF RID: 511
	public bool _disableOnStart;
}
