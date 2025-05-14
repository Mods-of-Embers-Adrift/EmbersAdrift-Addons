using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class LandingSpot : MonoBehaviour
{
	// Token: 0x060000F9 RID: 249 RVA: 0x0009660C File Offset: 0x0009480C
	public void Start()
	{
		if (this._transformCache == null)
		{
			this._transformCache = base.transform;
		}
		this._cachePreLandingWaypoint = this._preLandWaypoint;
		if (this._controller == null)
		{
			this._controller = this._transformCache.parent.GetComponent<LandingSpotController>();
		}
		if (this._controller._autoCatchDelay.x > 0f)
		{
			base.StartCoroutine(this.GetFlockChild(this._controller._autoCatchDelay.x, this._controller._autoCatchDelay.y));
		}
		if (this._controller._randomRotate && this._controller._parentBirdToSpot)
		{
			LandingSpotController controller = this._controller;
			Debug.LogWarning(((controller != null) ? controller.ToString() : null) + "\nEnabling random rotate and parent bird to spot is not yet available, disabling random rotate");
			this._controller._randomRotate = false;
		}
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00044F27 File Offset: 0x00043127
	public IEnumerator GetFlockChild(float minDelay, float maxDelay)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay));
		if (this._controller._flock.gameObject.activeInHierarchy && this._landingChild == null)
		{
			FlockChild flockChild = null;
			for (int i = 0; i < this._controller._flock._roamers.Count; i++)
			{
				FlockChild flockChild2 = this._controller._flock._roamers[i];
				if (flockChild2 != null && !flockChild2._landing && flockChild2.gameObject.activeInHierarchy)
				{
					flockChild2._landingSpot = this;
					this._distance = Vector3.Distance(flockChild2._thisT.position, this._transformCache.position);
					if (!this._controller._onlyBirdsAbove)
					{
						if (flockChild == null && this._controller._maxBirdDistance > this._distance && this._controller._minBirdDistance < this._distance)
						{
							flockChild = flockChild2;
							if (!this._controller._takeClosest)
							{
								break;
							}
						}
						else if (flockChild != null && Vector3.Distance(flockChild._thisT.position, this._transformCache.position) > this._distance)
						{
							flockChild = flockChild2;
						}
					}
					else if (flockChild == null && flockChild2._thisT.position.y > this._transformCache.position.y && this._controller._maxBirdDistance > this._distance && this._controller._minBirdDistance < this._distance)
					{
						flockChild = flockChild2;
						if (!this._controller._takeClosest)
						{
							break;
						}
					}
					else if (flockChild != null && flockChild2._thisT.position.y > this._transformCache.position.y && Vector3.Distance(flockChild._thisT.position, this._transformCache.position) > this._distance)
					{
						flockChild = flockChild2;
					}
				}
			}
			if (flockChild != null)
			{
				if (this._controller._abortLanding)
				{
					base.Invoke("ReleaseFlockChild", this._controller._abortLandingTimer);
				}
				this._landingChild = flockChild;
				if (this._controller._parentBirdToSpot)
				{
					this._landingChild.transform.SetParent(base.transform);
				}
				this._landing = true;
				this._landingChild._landing = true;
				if (this._controller._autoDismountDelay.x > 0f)
				{
					base.Invoke("ReleaseFlockChild", UnityEngine.Random.Range(this._controller._autoDismountDelay.x, this._controller._autoDismountDelay.y));
				}
				this._controller._activeLandingSpots++;
				this.RandomRotate();
			}
			else if (this._controller._autoCatchDelay.x > 0f)
			{
				base.StartCoroutine(this.GetFlockChild(this._controller._autoCatchDelay.x, this._controller._autoCatchDelay.y));
			}
		}
		yield break;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x000966F0 File Offset: 0x000948F0
	public void InstantLand()
	{
		if (this._controller._flock.gameObject.activeInHierarchy && this._landingChild == null)
		{
			FlockChild flockChild = null;
			for (int i = 0; i < this._controller._flock._roamers.Count; i++)
			{
				FlockChild flockChild2 = this._controller._flock._roamers[i];
				if (!flockChild2._landing)
				{
					flockChild = flockChild2;
				}
			}
			if (flockChild != null)
			{
				this._landingChild = flockChild;
				if (this._controller._parentBirdToSpot)
				{
					this._landingChild.transform.SetParent(base.transform);
				}
				this._landingChild._move = false;
				flockChild._speed = 0f;
				flockChild._targetSpeed = 0f;
				flockChild._landingSpot = this;
				this._landing = true;
				this._controller._activeLandingSpots++;
				this._landingChild._landing = true;
				this._landingChild._thisT.position = this._landingChild.GetLandingSpotPosition();
				if (this._controller._randomRotate)
				{
					this._landingChild._thisT.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
				}
				else
				{
					this._landingChild._thisT.rotation = this._transformCache.rotation;
				}
				if (!this._landingChild._animationIsBaked)
				{
					if (!this._landingChild._animator)
					{
						this._landingChild._modelAnimation.Play(this._landingChild._spawner._idleAnimation);
					}
					else
					{
						this._landingChild._animator.Play(this._landingChild._spawner._idleAnimation);
					}
				}
				else
				{
					this._landingChild._bakedAnimator.SetAnimation(2, -1);
				}
				if (this._controller._autoDismountDelay.x > 0f)
				{
					base.Invoke("ReleaseFlockChild", UnityEngine.Random.Range(this._controller._autoDismountDelay.x, this._controller._autoDismountDelay.y));
					return;
				}
			}
			else if (this._controller._autoCatchDelay.x > 0f)
			{
				base.StartCoroutine(this.GetFlockChild(this._controller._autoCatchDelay.x, this._controller._autoCatchDelay.y));
			}
		}
	}

	// Token: 0x060000FC RID: 252 RVA: 0x0009695C File Offset: 0x00094B5C
	public void ReleaseFlockChild()
	{
		if (this._controller._flock.gameObject.activeInHierarchy && this._landingChild != null)
		{
			this._preLandWaypoint = this._cachePreLandingWaypoint;
			this.EmitFeathers();
			this._landingChild._modelT.localEulerAngles = new Vector3(0f, 0f, 0f);
			this._landing = false;
			this._landingChild._avoid = true;
			this._landingChild._closeToSpot = false;
			this._landingChild._damping = this._landingChild._spawner._maxDamping;
			this._landingChild._targetSpeed = UnityEngine.Random.Range(this._landingChild._spawner._minSpeed, this._landingChild._spawner._maxSpeed);
			this._landingChild._move = true;
			this._landingChild._landing = false;
			this._landingChild.currentAnim = "";
			this._landingChild.Flap(0.1f);
			if (this._controller._parentBirdToSpot)
			{
				this._landingChild._spawner.AddChildToParent(this._landingChild._thisT);
			}
			this._landingChild._wayPoint = new Vector3(this._landingChild._wayPoint.x + 5f, this._transformCache.position.y + 5f, this._landingChild._wayPoint.z + 5f);
			this._landingChild._damping = 0.1f;
			if (this._controller._autoCatchDelay.x > 0f)
			{
				base.StartCoroutine(this.GetFlockChild(this._controller._autoCatchDelay.x + 0.1f, this._controller._autoCatchDelay.y + 0.1f));
			}
			this._controller._activeLandingSpots--;
			if (this._controller._abortLanding)
			{
				base.CancelInvoke("ReleaseFlockChild");
				if (this._landingChild.currentAnim != "Idle")
				{
					this._landingChild.FindWaypoint();
				}
			}
			this._landingChild = null;
		}
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00096B9C File Offset: 0x00094D9C
	public void RandomRotate()
	{
		if (this._controller._randomRotate)
		{
			Quaternion rotation = this._transformCache.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			eulerAngles.y = (float)UnityEngine.Random.Range(-180, 180);
			rotation.eulerAngles = eulerAngles;
			this._transformCache.rotation = rotation;
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00096BF8 File Offset: 0x00094DF8
	public void EmitFeathers()
	{
		if (this._controller._featherPS == null)
		{
			return;
		}
		this._controller._featherPS.position = this._landingChild._thisT.position;
		this._controller._featherParticles.Emit(UnityEngine.Random.Range(0, 3));
	}

	// Token: 0x04000275 RID: 629
	[HideInInspector]
	public FlockChild _landingChild;

	// Token: 0x04000276 RID: 630
	[HideInInspector]
	public bool _landing;

	// Token: 0x04000277 RID: 631
	public LandingSpotController _controller;

	// Token: 0x04000278 RID: 632
	public Transform _transformCache;

	// Token: 0x04000279 RID: 633
	public Vector3 _preLandWaypoint;

	// Token: 0x0400027A RID: 634
	private Vector3 _cachePreLandingWaypoint;

	// Token: 0x0400027B RID: 635
	private float _distance;
}
