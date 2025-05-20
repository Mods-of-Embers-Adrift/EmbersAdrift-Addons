using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class FlockWaypointTrigger : MonoBehaviour
{
	// Token: 0x060000F4 RID: 244 RVA: 0x00096400 File Offset: 0x00094600
	public void Start()
	{
		if (this._flockChild == null)
		{
			this._flockChild = base.transform.parent.GetComponent<FlockChild>();
		}
		float num = UnityEngine.Random.Range(this._timer, this._timer * 3f);
		base.InvokeRepeating("Trigger", num, num);
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00044EEF File Offset: 0x000430EF
	public void Trigger()
	{
		this._flockChild.Wander(0f);
	}

	// Token: 0x04000270 RID: 624
	public float _timer = 1f;

	// Token: 0x04000271 RID: 625
	public FlockChild _flockChild;
}
