using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class FlockScare : MonoBehaviour
{
	// Token: 0x060000ED RID: 237 RVA: 0x00096284 File Offset: 0x00094484
	private void CheckProximityToLandingSpots()
	{
		this.IterateLandingSpots();
		if (this.currentController._activeLandingSpots > 0 && this.CheckDistanceToLandingSpot(this.landingSpotControllers[this.lsc]))
		{
			this.landingSpotControllers[this.lsc].ScareAll();
		}
		base.Invoke("CheckProximityToLandingSpots", this.scareInterval);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000962E0 File Offset: 0x000944E0
	private void IterateLandingSpots()
	{
		this.ls += this.checkEveryNthLandingSpot;
		this.currentController = this.landingSpotControllers[this.lsc];
		int childCount = this.currentController.transform.childCount;
		if (this.ls > childCount - 1)
		{
			this.ls -= childCount;
			if (this.lsc < this.landingSpotControllers.Length - 1)
			{
				this.lsc++;
				return;
			}
			this.lsc = 0;
		}
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00096368 File Offset: 0x00094568
	private bool CheckDistanceToLandingSpot(LandingSpotController lc)
	{
		Transform child = lc.transform.GetChild(this.ls);
		LandingSpot component = child.GetComponent<LandingSpot>();
		if (component._landingChild != null && (child.position - base.transform.position).sqrMagnitude < this.distanceToScare * this.distanceToScare)
		{
			if (this.scareAll)
			{
				return true;
			}
			component.ReleaseFlockChild();
		}
		return false;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x000963DC File Offset: 0x000945DC
	private void Invoker()
	{
		for (int i = 0; i < this.InvokeAmounts; i++)
		{
			float num = this.scareInterval / (float)this.InvokeAmounts * (float)i;
			base.Invoke("CheckProximityToLandingSpots", this.scareInterval + num);
		}
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00044E93 File Offset: 0x00043093
	private void OnEnable()
	{
		base.CancelInvoke("CheckProximityToLandingSpots");
		if (this.landingSpotControllers.Length != 0)
		{
			this.Invoker();
		}
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00044EAF File Offset: 0x000430AF
	private void OnDisable()
	{
		base.CancelInvoke("CheckProximityToLandingSpots");
	}

	// Token: 0x04000267 RID: 615
	public LandingSpotController[] landingSpotControllers;

	// Token: 0x04000268 RID: 616
	public float scareInterval = 0.1f;

	// Token: 0x04000269 RID: 617
	public float distanceToScare = 2f;

	// Token: 0x0400026A RID: 618
	public int checkEveryNthLandingSpot = 1;

	// Token: 0x0400026B RID: 619
	public int InvokeAmounts = 1;

	// Token: 0x0400026C RID: 620
	private int lsc;

	// Token: 0x0400026D RID: 621
	private int ls;

	// Token: 0x0400026E RID: 622
	private LandingSpotController currentController;

	// Token: 0x0400026F RID: 623
	public bool scareAll = true;
}
