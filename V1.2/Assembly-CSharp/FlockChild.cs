using System;
using SoL.Game.Pooling;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class FlockChild : PooledObject
{
	// Token: 0x060000BE RID: 190 RVA: 0x00044D93 File Offset: 0x00042F93
	private void PeriodicFindWaypoint()
	{
		this.FindWaypoint();
		this.m_nextWaypointUpdate = Time.time + 20f;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000941D0 File Offset: 0x000923D0
	public void Start()
	{
		this.FindRequiredComponents();
		if (this._spawner == null)
		{
			base.enabled = false;
			return;
		}
		this.Wander();
		this.SetRandomScale();
		this.FindWaypoint();
		this._thisT.position = this._wayPoint;
		this.RandomizeStartAnimationFrame();
		this.InitAvoidanceValues();
		this._speed = this._spawner._minSpeed;
		this._spawner._activeChildren += 1f;
		this._instantiated = true;
		if (this._spawner._skipFrame)
		{
			FlockChild._updateNextSeed++;
			this._updateSeed = FlockChild._updateNextSeed;
			FlockChild._updateNextSeed %= 1;
		}
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00094288 File Offset: 0x00092488
	public void BirdUpdate()
	{
		if (this == null || !this._instantiated)
		{
			return;
		}
		if (Time.time > this.m_nextWaypointUpdate)
		{
			this.PeriodicFindWaypoint();
		}
		if (!this._spawner._skipFrame || this._spawner._updateCounter == this._updateSeed)
		{
			if (this._spawner.LimitPitchRotation || this._landing)
			{
				this.LimitRotationOfModel();
			}
			if (!this._landing)
			{
				this.SoarTimeLimit();
				this.CheckForDistanceToWaypoint();
				this.RotationBasedOnWaypointOrAvoidance();
				this.Move();
			}
			else if (!this._move)
			{
				this.RotateBird();
				if (!this._bakedAnimator)
				{
					return;
				}
				if (this._bakedAnimator.isActiveAndEnabled)
				{
					this._bakedAnimator.AnimateUpdate();
				}
				return;
			}
			else
			{
				if (this.distance > 5f)
				{
					this._wayPoint = this.GetLandingSpotPosition() + this._landingSpot._preLandWaypoint;
					if (this._landingSpot._preLandWaypoint.sqrMagnitude > 0f && Vector3.Distance(this._wayPoint, this._thisT.position) < 1f)
					{
						this._wayPoint = this.GetLandingSpotPosition();
						this._landingSpot._preLandWaypoint = Vector3.zero;
					}
				}
				else
				{
					this._wayPoint = this._landingSpot._transformCache.position + this._landingPosOffset + this._landingOffsetFix * this._thisT.localScale.y;
				}
				this._damping = this._landingSpot._controller._landingTurnSpeedModifier;
				this.RotationBasedOnWaypointOrAvoidance();
				this.Move();
				this.Landing();
			}
		}
		if (!this._bakedAnimator)
		{
			return;
		}
		if (this._bakedAnimator.isActiveAndEnabled)
		{
			this._bakedAnimator.AnimateUpdate();
		}
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0009445C File Offset: 0x0009265C
	private void Landing()
	{
		if (this.currentAnim == "Idle")
		{
			return;
		}
		this.distance = Vector3.Distance(this.GetLandingSpotPosition(), this._thisT.position);
		if (this.distance < 8f && this.distance >= 1f)
		{
			if (this.distance < 4f)
			{
				if (this.currentAnim != "Flap")
				{
					if (!this._animationIsBaked)
					{
						if (!this._animator)
						{
							this._modelAnimation.CrossFade(this._spawner._flapAnimation, 0.5f);
						}
						else
						{
							this._animator.CrossFade(this._spawner._flapAnimation, 0.5f);
						}
					}
					else
					{
						this._bakedAnimator.SetAnimation(0, -1);
						this._bakedAnimator.SetSpeedMultiplier(this._spawner._maxAnimationSpeed);
					}
					this.currentAnim = "Flap";
				}
			}
			else if (this._landingSpot._controller._soarLand)
			{
				if (this.currentAnim != "Soar")
				{
					if (!this._animationIsBaked)
					{
						if (!this._animator)
						{
							this._modelAnimation.CrossFade(this._spawner._soarAnimation, 0.5f);
						}
						else
						{
							this._animator.CrossFade(this._spawner._soarAnimation, 0.5f);
						}
					}
					else
					{
						this._bakedAnimator.SetAnimation(1);
						this._bakedAnimator.SetSpeedMultiplier(this._spawner._minAnimationSpeed);
					}
					this.currentAnim = "Soar";
				}
			}
			else if (this.currentAnim != "Flap")
			{
				if (!this._animationIsBaked)
				{
					if (!this._animator)
					{
						this._modelAnimation.CrossFade(this._spawner._flapAnimation, 0.5f);
					}
					else
					{
						this._animator.CrossFade(this._spawner._flapAnimation, 0.5f);
					}
				}
				else
				{
					this._bakedAnimator.SetAnimation(0, -1);
					this._bakedAnimator.SetSpeedMultiplier(this._spawner._maxAnimationSpeed);
				}
				this.currentAnim = "Flap";
			}
			this._targetSpeed = this._spawner._maxSpeed * this._landingSpot._controller._landingSpeedModifier;
			return;
		}
		if (this.distance < 1f)
		{
			this._thisT.position += (this.GetLandingSpotPosition() - base.transform.position).normalized * this._spawner._newDelta * this._landingSpot._controller._closeToSpotSpeedModifier * this._targetSpeed;
			this._closeToSpot = true;
			if (this.distance < this._landingSpot._controller._snapLandDistance)
			{
				if (this.currentAnim != "Idle")
				{
					base.Invoke("Idle", UnityEngine.Random.Range(this._landingSpot._controller.idleAnimationDelayMin, this._landingSpot._controller.idleAnimationDelayMax));
					this.currentAnim = "Idle";
				}
				this._move = false;
				this._thisT.position = this.GetLandingSpotPosition();
				this._modelT.localRotation = Quaternion.identity;
				this._thisT.eulerAngles = new Vector3(0f, this._thisT.rotation.eulerAngles.y, 0f);
				this._damping = 0.75f;
				return;
			}
			this._speed = this._spawner._minSpeed * 0.2f;
		}
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x0009481C File Offset: 0x00092A1C
	private void Idle()
	{
		if (this._animationIsBaked)
		{
			this._bakedAnimator.SetAnimation(2, -1);
			return;
		}
		if (!this._animator)
		{
			this._modelAnimation.CrossFade(this._spawner._idleAnimation, 0.5f);
			return;
		}
		this._animator.CrossFade(this._spawner._idleAnimation, 0.5f);
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00044DAC File Offset: 0x00042FAC
	public Vector3 GetLandingSpotPosition()
	{
		return this._landingSpot._transformCache.position + this._landingPosOffset + this._landingOffsetFix * this._thisT.localScale.y;
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00094884 File Offset: 0x00092A84
	public void RotateBird()
	{
		if (!this._landingSpot._controller._rotateAfterLanding || this._thisT.rotation.eulerAngles == this._landingSpot._transformCache.rotation.eulerAngles)
		{
			return;
		}
		Quaternion rotation = this._landingSpot._landingChild._thisT.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		eulerAngles.x = Mathf.LerpAngle(this._thisT.rotation.eulerAngles.x, this._landingSpot._transformCache.rotation.eulerAngles.x, this._landingSpot._controller._landedRotateSpeed * this._spawner._newDelta);
		eulerAngles.z = Mathf.LerpAngle(this._thisT.rotation.eulerAngles.z, this._landingSpot._transformCache.rotation.eulerAngles.z, this._landingSpot._controller._landedRotateSpeed * this._spawner._newDelta);
		eulerAngles.y = Mathf.LerpAngle(this._thisT.rotation.eulerAngles.y, this._landingSpot._transformCache.rotation.eulerAngles.y, this._landingSpot._controller._landedRotateSpeed * this._spawner._newDelta);
		rotation.eulerAngles = eulerAngles;
		this._thisT.rotation = rotation;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00044DE9 File Offset: 0x00042FE9
	public void OnDisable()
	{
		base.CancelInvoke();
		if (this._spawner)
		{
			this._spawner._activeChildren -= 1f;
		}
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00094A20 File Offset: 0x00092C20
	public void OnEnable()
	{
		if (this._instantiated)
		{
			this._spawner._activeChildren += 1f;
			if (this._animationIsBaked)
			{
				if (this._landing)
				{
					this._bakedAnimator.SetAnimation(2);
					return;
				}
				this._bakedAnimator.SetAnimation(0);
				return;
			}
			else
			{
				if (this._landing)
				{
					this._modelAnimation.Play(this._spawner._idleAnimation);
				}
				else
				{
					this._modelAnimation.Play(this._spawner._flapAnimation);
				}
				this.PeriodicFindWaypoint();
				this._thisT.position = this._wayPoint;
				this.FindWaypoint();
			}
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00094AD0 File Offset: 0x00092CD0
	public void FindRequiredComponents()
	{
		if (this._thisT == null)
		{
			this._thisT = base.transform;
		}
		if (this._model == null)
		{
			this._model = this._thisT.Find("Model").gameObject;
		}
		if (this._modelT == null)
		{
			this._modelT = this._model.transform;
		}
		if (this._bakedAnimator != null)
		{
			this._animationIsBaked = true;
			return;
		}
		if (this._modelAnimation != null)
		{
			return;
		}
		if (this._animator != null)
		{
			return;
		}
		this._modelAnimation = this._model.GetComponent<Animation>();
		if (!this._modelAnimation)
		{
			this._animator = this._model.GetComponent<Animator>();
		}
		if (!this._modelAnimation && !this._animator)
		{
			this._animationIsBaked = true;
		}
		else
		{
			this._animationIsBaked = false;
		}
		if (this._bakedAnimator == null)
		{
			this._bakedAnimator = base.GetComponent<BakedMeshAnimator>();
		}
		if (this._bakedAnimator == null)
		{
			this._bakedAnimator = this._model.GetComponent<BakedMeshAnimator>();
		}
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00094C08 File Offset: 0x00092E08
	public void RandomizeStartAnimationFrame()
	{
		if (this._animationIsBaked || this._animator)
		{
			return;
		}
		foreach (object obj in this._modelAnimation)
		{
			AnimationState animationState = (AnimationState)obj;
			animationState.time = UnityEngine.Random.value * animationState.length;
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00094C84 File Offset: 0x00092E84
	public void InitAvoidanceValues()
	{
		this._avoidValue = UnityEngine.Random.Range(0.3f, 0.1f);
		if (this._spawner._birdAvoidDistanceMax != this._spawner._birdAvoidDistanceMin)
		{
			this._avoidDistance = UnityEngine.Random.Range(this._spawner._birdAvoidDistanceMax, this._spawner._birdAvoidDistanceMin);
			return;
		}
		this._avoidDistance = this._spawner._birdAvoidDistanceMin;
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00094CF4 File Offset: 0x00092EF4
	public void SetRandomScale()
	{
		float num = UnityEngine.Random.Range(this._spawner._minScale, this._spawner._maxScale);
		this._thisT.localScale = new Vector3(num, num, num);
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00094D30 File Offset: 0x00092F30
	public void SoarTimeLimit()
	{
		if (this._soar && this._spawner._soarMaxTime > 0f)
		{
			if (this._soarTimer > this._spawner._soarMaxTime)
			{
				this.Flap(0.5f);
				this._soarTimer = 0f;
				return;
			}
			this._soarTimer += this._spawner._newDelta;
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00094D9C File Offset: 0x00092F9C
	public void CheckForDistanceToWaypoint()
	{
		if (!this._landing && (this._thisT.position - this._wayPoint).magnitude < this._spawner._waypointDistance)
		{
			this.Wander();
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00094DE4 File Offset: 0x00092FE4
	public void RotationBasedOnWaypointOrAvoidance()
	{
		if (this._avoiding)
		{
			return;
		}
		Vector3 vector = this._wayPoint - this._thisT.position;
		if (this._targetSpeed > -1f && vector != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(vector);
			this._thisT.rotation = Quaternion.Slerp(this._thisT.rotation, b, this._spawner._newDelta * this._damping);
		}
		if (this._spawner._childTriggerPos && (this._thisT.position - this._spawner._posBuffer).magnitude < 1f)
		{
			this._spawner.SetFlockRandomPosition();
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00094EA4 File Offset: 0x000930A4
	private void Move()
	{
		if (this._move)
		{
			this.ChangeSpeed();
			if (!this._closeToSpot)
			{
				this._thisT.position += this._thisT.forward * this._speed * this._spawner._newDelta;
			}
			if (this.Avoidance() && !this._avoiding)
			{
				this._avoiding = true;
				base.Invoke("AvoidNoLonger", UnityEngine.Random.Range(0.25f, 0.5f));
			}
		}
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00044E15 File Offset: 0x00043015
	private void AvoidNoLonger()
	{
		this._avoiding = false;
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00094F34 File Offset: 0x00093134
	private void ChangeSpeed()
	{
		if (this._speed < this._targetSpeed)
		{
			this._speed += this._acceleration * this._spawner._newDelta;
			return;
		}
		if (this._speed > this._targetSpeed)
		{
			this._speed -= this._acceleration * this._spawner._newDelta;
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00094F9C File Offset: 0x0009319C
	public bool Avoidance()
	{
		if (!this._avoid || !this._spawner._birdAvoid)
		{
			return false;
		}
		Vector3 forward = this._modelT.forward;
		bool result = false;
		Vector3 position = this._thisT.position;
		Quaternion rotation = this._thisT.rotation;
		Vector3 eulerAngles = this._thisT.rotation.eulerAngles;
		RaycastHit raycastHit;
		if (Physics.Raycast(this._thisT.position, forward + this._modelT.right * this._avoidValue, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			if (this._landing)
			{
				this._damping = this._spawner._minDamping;
			}
			eulerAngles.y -= (float)this._spawner._birdAvoidHorizontalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			result = true;
		}
		else if (Physics.Raycast(this._thisT.position, forward + this._modelT.right * -this._avoidValue, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			if (this._landing)
			{
				this._damping = this._spawner._minDamping;
			}
			eulerAngles.y += (float)this._spawner._birdAvoidHorizontalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			result = true;
		}
		if (this._spawner._birdAvoidDown && !this._landing && Physics.Raycast(this._thisT.position, -Vector3.up, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			eulerAngles.x -= (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			position.y += (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * 0.01f;
			this._thisT.position = position;
			result = true;
		}
		if (this._spawner._birdAvoidUp && !this._landing && Physics.Raycast(this._thisT.position, Vector3.up, out raycastHit, this._avoidDistance, this._spawner._avoidanceMask))
		{
			eulerAngles.x += (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * this._damping;
			rotation.eulerAngles = eulerAngles;
			this._thisT.rotation = rotation;
			position.y -= (float)this._spawner._birdAvoidVerticalForce * this._spawner._newDelta * 0.01f;
			this._thisT.position = position;
			result = true;
		}
		return result;
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x000952D0 File Offset: 0x000934D0
	public void LimitRotationOfModel()
	{
		Quaternion localRotation = this._modelT.localRotation;
		Vector3 eulerAngles = localRotation.eulerAngles;
		if ((((this._soar && this._spawner._flatSoar) || (this._spawner._flatFly && !this._soar)) && this._wayPoint.y > this._thisT.position.y) || this._landing)
		{
			eulerAngles.x = Mathf.LerpAngle(this._modelT.localEulerAngles.x, -this._thisT.localEulerAngles.x, this._spawner._newDelta * 1.75f);
			localRotation.eulerAngles = eulerAngles;
			this._modelT.localRotation = localRotation;
			return;
		}
		eulerAngles.x = Mathf.LerpAngle(this._modelT.localEulerAngles.x, 0f, this._spawner._newDelta * 1.75f);
		localRotation.eulerAngles = eulerAngles;
		this._modelT.localRotation = localRotation;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x000953DC File Offset: 0x000935DC
	public void Wander()
	{
		if (!this._landing)
		{
			this._damping = UnityEngine.Random.Range(this._spawner._minDamping, this._spawner._maxDamping);
			this._targetSpeed = UnityEngine.Random.Range(this._spawner._minSpeed, this._spawner._maxSpeed);
			if (this != null)
			{
				this.SetRandomMode();
			}
		}
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00095444 File Offset: 0x00093644
	public void Wander(float delay)
	{
		if (!this._landing)
		{
			this._damping = UnityEngine.Random.Range(this._spawner._minDamping, this._spawner._maxDamping);
			this._targetSpeed = UnityEngine.Random.Range(this._spawner._minSpeed, this._spawner._maxSpeed);
			if (this != null)
			{
				base.Invoke("SetRandomMode", delay);
			}
		}
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x000954B0 File Offset: 0x000936B0
	public void SetRandomMode()
	{
		base.CancelInvoke("SetRandomMode");
		if (!this._dived && UnityEngine.Random.value < this._spawner._soarFrequency)
		{
			this.Soar(0.75f);
			return;
		}
		if (!this._dived && UnityEngine.Random.value < this._spawner._diveFrequency)
		{
			this.Dive();
			return;
		}
		this.Flap(0.5f);
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x0009551C File Offset: 0x0009371C
	public void Flap(float crossfadeSeconds = 0.5f)
	{
		this.FindWaypoint();
		if (this.currentAnim == "Flap" || !this._move)
		{
			return;
		}
		if (!this._animationIsBaked)
		{
			if (!this._animator)
			{
				this._modelAnimation.CrossFade(this._spawner._flapAnimation, crossfadeSeconds);
			}
			else
			{
				this._animator.CrossFade(this._spawner._flapAnimation, crossfadeSeconds);
			}
		}
		else
		{
			this.CachedAnimationHandler("flap");
		}
		this._soar = false;
		this._dived = false;
		this.currentAnim = "Flap";
		this.AnimationSpeed();
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x000955BC File Offset: 0x000937BC
	public void FindWaypoint()
	{
		Vector3 zero = Vector3.zero;
		zero.x = UnityEngine.Random.Range(-this._spawner._spawnSphere, this._spawner._spawnSphere) + this._spawner._posBuffer.x;
		zero.z = UnityEngine.Random.Range(-this._spawner._spawnSphereDepth, this._spawner._spawnSphereDepth) + this._spawner._posBuffer.z;
		zero.y = UnityEngine.Random.Range(-this._spawner._spawnSphereHeight, this._spawner._spawnSphereHeight) + this._spawner._posBuffer.y;
		this._wayPoint = zero;
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00095674 File Offset: 0x00093874
	public void Soar(float crossfadeSeconds = 0.75f)
	{
		this.FindWaypoint();
		if (this.currentAnim == "Soar" || !this._move)
		{
			return;
		}
		if (!this._animationIsBaked)
		{
			if (!this._animator)
			{
				this._modelAnimation.CrossFade(this._spawner._soarAnimation, crossfadeSeconds);
			}
			else
			{
				this._animator.CrossFade(this._spawner._soarAnimation, crossfadeSeconds);
			}
		}
		else
		{
			this.CachedAnimationHandler("soar");
		}
		this._soar = true;
		this.currentAnim = "Soar";
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00095708 File Offset: 0x00093908
	public void CachedAnimationHandler(string type)
	{
		if (!this._bakedAnimator)
		{
			return;
		}
		if (type == "flap")
		{
			this._bakedAnimator.SetAnimation(0);
			return;
		}
		if (type == "soar")
		{
			this._bakedAnimator.SetAnimation(1);
			return;
		}
		if (type == "idle")
		{
			this._bakedAnimator.SetAnimation(2);
			return;
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00095774 File Offset: 0x00093974
	public void Dive()
	{
		if (!this._animationIsBaked)
		{
			if (!this._animator)
			{
				this._modelAnimation.CrossFade(this._spawner._soarAnimation, 0.2f);
			}
			else
			{
				this._animator.CrossFade(this._spawner._soarAnimation, 0.2f);
			}
		}
		else
		{
			this.CachedAnimationHandler("soar");
		}
		this.currentAnim = "Soar";
		this._wayPoint = this._thisT.position;
		this._wayPoint.y = this._wayPoint.y - this._spawner._diveValue;
		this._dived = true;
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00095818 File Offset: 0x00093A18
	public void AnimationSpeed()
	{
		if (!this._animationIsBaked)
		{
			if (!this._animator)
			{
				if (this.flapState == null)
				{
					this.flapState = this._modelAnimation[this._spawner._flapAnimation];
					this.idleState = this._modelAnimation[this._spawner._idleAnimation];
					this.soarState = this._modelAnimation[this._spawner._soarAnimation];
				}
				float speed;
				if (!this._dived && !this._landing)
				{
					speed = UnityEngine.Random.Range(this._spawner._minAnimationSpeed, this._spawner._maxAnimationSpeed);
				}
				else
				{
					speed = this._spawner._maxAnimationSpeed;
				}
				if (this.currentAnim == "Flap")
				{
					this.flapState.speed = speed;
					return;
				}
				if (this.currentAnim == "Soar")
				{
					this.soarState.speed = speed;
					return;
				}
				if (this.currentAnim == "Soar")
				{
					this.idleState.speed = speed;
					return;
				}
			}
			else
			{
				if (!this._dived && !this._landing)
				{
					this._animator.speed = UnityEngine.Random.Range(this._spawner._minAnimationSpeed, this._spawner._maxAnimationSpeed);
					return;
				}
				this._animator.speed = this._spawner._maxAnimationSpeed;
			}
			return;
		}
		if (!this._dived && !this._landing)
		{
			this._bakedAnimator.SetSpeedMultiplier(UnityEngine.Random.Range(this._spawner._minAnimationSpeed, this._spawner._maxAnimationSpeed));
			return;
		}
		this._bakedAnimator.SetSpeedMultiplier(this._spawner._maxAnimationSpeed);
	}

	// Token: 0x04000200 RID: 512
	private const float kForcedWaypointUpdateRate = 20f;

	// Token: 0x04000201 RID: 513
	[NonSerialized]
	private float m_nextWaypointUpdate = 20f;

	// Token: 0x04000202 RID: 514
	[HideInInspector]
	public FlockController _spawner;

	// Token: 0x04000203 RID: 515
	[HideInInspector]
	public Vector3 _wayPoint;

	// Token: 0x04000204 RID: 516
	[HideInInspector]
	public float _speed;

	// Token: 0x04000205 RID: 517
	[HideInInspector]
	public bool _dived = true;

	// Token: 0x04000206 RID: 518
	[HideInInspector]
	public float _damping;

	// Token: 0x04000207 RID: 519
	[HideInInspector]
	public bool _soar = true;

	// Token: 0x04000208 RID: 520
	[HideInInspector]
	public bool _landing;

	// Token: 0x04000209 RID: 521
	[HideInInspector]
	public bool _landed;

	// Token: 0x0400020A RID: 522
	[HideInInspector]
	public LandingSpot _landingSpot;

	// Token: 0x0400020B RID: 523
	[HideInInspector]
	public float _targetSpeed;

	// Token: 0x0400020C RID: 524
	[HideInInspector]
	public bool _move = true;

	// Token: 0x0400020D RID: 525
	public GameObject _model;

	// Token: 0x0400020E RID: 526
	public Transform _modelT;

	// Token: 0x0400020F RID: 527
	[HideInInspector]
	public float _avoidValue;

	// Token: 0x04000210 RID: 528
	[HideInInspector]
	public float _avoidDistance;

	// Token: 0x04000211 RID: 529
	private float _soarTimer;

	// Token: 0x04000212 RID: 530
	private bool _instantiated;

	// Token: 0x04000213 RID: 531
	private static int _updateNextSeed;

	// Token: 0x04000214 RID: 532
	private int _updateSeed = -1;

	// Token: 0x04000215 RID: 533
	[HideInInspector]
	public bool _avoid = true;

	// Token: 0x04000216 RID: 534
	public Transform _thisT;

	// Token: 0x04000217 RID: 535
	public Vector3 _landingPosOffset;

	// Token: 0x04000218 RID: 536
	[HideInInspector]
	public bool _animationIsBaked;

	// Token: 0x04000219 RID: 537
	public BakedMeshAnimator _bakedAnimator;

	// Token: 0x0400021A RID: 538
	public Animation _modelAnimation;

	// Token: 0x0400021B RID: 539
	public Animator _animator;

	// Token: 0x0400021C RID: 540
	private float _acceleration = 20f;

	// Token: 0x0400021D RID: 541
	[HideInInspector]
	public string currentAnim;

	// Token: 0x0400021E RID: 542
	[HideInInspector]
	public bool _closeToSpot;

	// Token: 0x0400021F RID: 543
	private float distance;

	// Token: 0x04000220 RID: 544
	private bool _avoiding;

	// Token: 0x04000221 RID: 545
	private AnimationState flapState;

	// Token: 0x04000222 RID: 546
	private AnimationState soarState;

	// Token: 0x04000223 RID: 547
	private AnimationState idleState;

	// Token: 0x04000224 RID: 548
	public Vector3 _landingOffsetFix = new Vector3(0f, 0.1f, 0f);
}
