using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class LandingSpotController : MonoBehaviour
{
	// Token: 0x06000106 RID: 262 RVA: 0x00096FC0 File Offset: 0x000951C0
	public void Start()
	{
		if (this._transformCache == null)
		{
			this._transformCache = base.transform;
		}
		if (this._flock == null)
		{
			this._flock = (FlockController)UnityEngine.Object.FindObjectOfType(typeof(FlockController));
			Debug.Log(((this != null) ? this.ToString() : null) + " has no assigned FlockController, a random FlockController has been assigned");
		}
		if (this._featherPS)
		{
			this._featherParticles = this._featherPS.GetComponent<ParticleSystem>();
		}
		if (this._landOnStart)
		{
			base.StartCoroutine(this.InstantLandOnStart(0.1f));
		}
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00044F5B File Offset: 0x0004315B
	public void ScareAll()
	{
		this.ScareAll(0f, 1f);
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00097064 File Offset: 0x00095264
	public void ScareAll(float minDelay, float maxDelay)
	{
		for (int i = 0; i < this._transformCache.childCount; i++)
		{
			if (this._transformCache.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				this._transformCache.GetChild(i).GetComponent<LandingSpot>().ReleaseFlockChild();
			}
		}
	}

	// Token: 0x06000109 RID: 265 RVA: 0x000970B8 File Offset: 0x000952B8
	public void LandAll()
	{
		for (int i = 0; i < this._transformCache.childCount; i++)
		{
			if (this._transformCache.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				LandingSpot component = this._transformCache.GetChild(i).GetComponent<LandingSpot>();
				base.StartCoroutine(component.GetFlockChild(0f, 2f));
			}
		}
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00044F6D File Offset: 0x0004316D
	public IEnumerator InstantLandOnStart(float delay)
	{
		yield return new WaitForSeconds(delay);
		for (int i = 0; i < this._transformCache.childCount; i++)
		{
			if (this._transformCache.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				this._transformCache.GetChild(i).GetComponent<LandingSpot>().InstantLand();
			}
		}
		yield break;
	}

	// Token: 0x0600010B RID: 267 RVA: 0x00044F83 File Offset: 0x00043183
	public IEnumerator InstantLand(float delay)
	{
		yield return new WaitForSeconds(delay);
		for (int i = 0; i < this._transformCache.childCount; i++)
		{
			if (this._transformCache.GetChild(i).GetComponent<LandingSpot>() != null)
			{
				this._transformCache.GetChild(i).GetComponent<LandingSpot>().InstantLand();
			}
		}
		yield break;
	}

	// Token: 0x04000281 RID: 641
	public bool _rotateAfterLanding = true;

	// Token: 0x04000282 RID: 642
	public bool _randomRotate = true;

	// Token: 0x04000283 RID: 643
	public Vector2 _autoCatchDelay = new Vector2(10f, 20f);

	// Token: 0x04000284 RID: 644
	public Vector2 _autoDismountDelay = new Vector2(10f, 20f);

	// Token: 0x04000285 RID: 645
	public float _maxBirdDistance = 20f;

	// Token: 0x04000286 RID: 646
	public float _minBirdDistance = 5f;

	// Token: 0x04000287 RID: 647
	public bool _takeClosest;

	// Token: 0x04000288 RID: 648
	public FlockController _flock;

	// Token: 0x04000289 RID: 649
	public bool _landOnStart;

	// Token: 0x0400028A RID: 650
	public bool _soarLand = true;

	// Token: 0x0400028B RID: 651
	public bool _onlyBirdsAbove;

	// Token: 0x0400028C RID: 652
	public float _landingSpeedModifier = 0.5f;

	// Token: 0x0400028D RID: 653
	public float _closeToSpotSpeedModifier = 1f;

	// Token: 0x0400028E RID: 654
	public float _releaseSpeedModifier = 3f;

	// Token: 0x0400028F RID: 655
	public float _landingTurnSpeedModifier = 5f;

	// Token: 0x04000290 RID: 656
	public Transform _featherPS;

	// Token: 0x04000291 RID: 657
	[HideInInspector]
	public ParticleSystem _featherParticles;

	// Token: 0x04000292 RID: 658
	[HideInInspector]
	public Transform _transformCache;

	// Token: 0x04000293 RID: 659
	[HideInInspector]
	public int _activeLandingSpots;

	// Token: 0x04000294 RID: 660
	[Range(0.01f, 1f)]
	public float _snapLandDistance = 0.05f;

	// Token: 0x04000295 RID: 661
	public float _landedRotateSpeed = 2f;

	// Token: 0x04000296 RID: 662
	public bool _drawGizmos = true;

	// Token: 0x04000297 RID: 663
	public float _gizmoSize = 0.2f;

	// Token: 0x04000298 RID: 664
	public bool _parentBirdToSpot;

	// Token: 0x04000299 RID: 665
	public bool _abortLanding;

	// Token: 0x0400029A RID: 666
	[Range(1f, 20f)]
	public float _abortLandingTimer = 10f;

	// Token: 0x0400029B RID: 667
	public float idleAnimationDelayMin = 0.1f;

	// Token: 0x0400029C RID: 668
	public float idleAnimationDelayMax = 0.75f;
}
