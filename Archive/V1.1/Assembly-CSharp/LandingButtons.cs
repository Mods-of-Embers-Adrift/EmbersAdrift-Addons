using System;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class LandingButtons : MonoBehaviour
{
	// Token: 0x060000F7 RID: 247 RVA: 0x00096458 File Offset: 0x00094658
	public void OnGUI()
	{
		GUI.Label(new Rect(20f, 20f, 125f, 18f), "Landing Spots: " + this._landingSpotController.transform.childCount.ToString());
		if (GUI.Button(new Rect(20f, 40f, 125f, 18f), "Scare All"))
		{
			this._landingSpotController.ScareAll();
		}
		if (GUI.Button(new Rect(20f, 60f, 125f, 18f), "Land In Reach"))
		{
			this._landingSpotController.LandAll();
		}
		if (GUI.Button(new Rect(20f, 80f, 125f, 18f), "Land Instant"))
		{
			base.StartCoroutine(this._landingSpotController.InstantLand(0.01f));
		}
		if (GUI.Button(new Rect(20f, 100f, 125f, 18f), "Destroy"))
		{
			this._flockController.destroyBirds();
		}
		GUI.Label(new Rect(20f, 120f, 125f, 18f), "Bird Amount: " + this._flockController._childAmount.ToString());
		this._flockController._childAmount = (int)GUI.HorizontalSlider(new Rect(20f, 140f, 125f, 18f), (float)this._flockController._childAmount, 0f, 250f);
	}

	// Token: 0x04000272 RID: 626
	public LandingSpotController _landingSpotController;

	// Token: 0x04000273 RID: 627
	public FlockController _flockController;

	// Token: 0x04000274 RID: 628
	public float hSliderValue = 250f;
}
