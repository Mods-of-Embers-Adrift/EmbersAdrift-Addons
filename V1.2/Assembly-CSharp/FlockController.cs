using System;
using System.Collections.Generic;
using SoL.Game.Pooling;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class FlockController : MonoBehaviour
{
	// Token: 0x060000E1 RID: 225 RVA: 0x00044E1E File Offset: 0x0004301E
	public void Start()
	{
		this._roamers.Clear();
		this.LateStart();
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00095CE8 File Offset: 0x00093EE8
	public void LateStart()
	{
		if (!this._childPrefab)
		{
			Debug.LogWarning("Bird Flock does not have a bird prefab assigned, aborting.");
			base.enabled = false;
			return;
		}
		this._thisT = base.transform;
		if (this._positionSphereDepth == -1f)
		{
			this._positionSphereDepth = this._positionSphere;
		}
		if (this._spawnSphereDepth == -1f)
		{
			this._spawnSphereDepth = this._spawnSphere;
		}
		this._posBuffer = this._thisT.position + this._startPosOffset;
		if (!this._slowSpawn)
		{
			this.AddChild(this._childAmount);
			this._childCountCache = this._roamers.Count;
		}
		if (this._randomPositionTimer > 0f)
		{
			base.InvokeRepeating("SetFlockRandomPosition", this._randomPositionTimer, this._randomPositionTimer);
		}
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00095DB8 File Offset: 0x00093FB8
	public void AddChild(int amount)
	{
		if (this._groupChildToNewTransform)
		{
			this.InstantiateGroup();
		}
		for (int i = 0; i < amount; i++)
		{
			FlockChild pooledInstance = this._childPrefab.GetPooledInstance<FlockChild>();
			pooledInstance._spawner = this;
			this._roamers.Add(pooledInstance);
			this.AddChildToParent(pooledInstance.transform);
		}
		this._childCountCache += amount;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00044E31 File Offset: 0x00043031
	public void AddChildToParent(Transform obj)
	{
		if (this._groupChildToFlock)
		{
			this._groupTransform = this._thisT;
		}
		obj.parent = this._groupTransform;
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00095E18 File Offset: 0x00094018
	public void RemoveChild(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			PooledObject obj = this._roamers[this._roamers.Count - 1];
			this._roamers.RemoveAt(this._roamers.Count - 1);
			obj.ReturnToPool();
		}
		this._childCountCache -= amount;
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00095E74 File Offset: 0x00094074
	public void Update()
	{
		if (this._activeChildren > 0f)
		{
			if (this._skipFrame)
			{
				this._updateCounter++;
				this._updateCounter %= 2;
				this._newDelta = Time.deltaTime * 2f % 0.5f;
			}
			else
			{
				this._newDelta = Time.deltaTime;
			}
		}
		this.UpdateChildAmount();
		for (int i = 0; i < this._roamers.Count; i++)
		{
			this._roamers[i].BirdUpdate();
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00095F04 File Offset: 0x00094104
	public void InstantiateGroup()
	{
		if (this._groupTransform != null)
		{
			return;
		}
		GameObject gameObject = new GameObject();
		this._groupTransform = gameObject.transform;
		this._groupTransform.position = this._thisT.position;
		if (this._groupName != "")
		{
			gameObject.name = this._groupName;
			return;
		}
		gameObject.name = this._thisT.name + " Bird Container";
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00044E53 File Offset: 0x00043053
	public void UpdateChildAmount()
	{
		if (this._childAmount >= 0 && this._childAmount < this._roamers.Count)
		{
			this.RemoveChild(1);
			return;
		}
		if (this._childAmount > this._roamers.Count)
		{
			this.AddChild(1);
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00095F84 File Offset: 0x00094184
	public Vector3 GetSpawnSize()
	{
		return new Vector3(this._positionSphere * 2f + this._spawnSphere * 2f, this._positionSphereHeight * 2f + this._spawnSphereHeight * 2f, this._positionSphereDepth * 2f + this._spawnSphereDepth * 2f);
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00095FE4 File Offset: 0x000941E4
	public void SetFlockRandomPosition()
	{
		Vector3 zero = Vector3.zero;
		zero.x = UnityEngine.Random.Range(-this._positionSphere, this._positionSphere) + this._thisT.position.x;
		zero.z = UnityEngine.Random.Range(-this._positionSphereDepth, this._positionSphereDepth) + this._thisT.position.z;
		zero.y = UnityEngine.Random.Range(-this._positionSphereHeight, this._positionSphereHeight) + this._thisT.position.y;
		this._posBuffer = zero;
		if (this._forceChildWaypoints)
		{
			for (int i = 0; i < this._roamers.Count; i++)
			{
				this._roamers[i].Wander(UnityEngine.Random.value * this._forcedRandomDelay);
			}
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x000960B8 File Offset: 0x000942B8
	public void destroyBirds()
	{
		for (int i = 0; i < this._roamers.Count; i++)
		{
			if (this._roamers[i] != null)
			{
				UnityEngine.Object.Destroy(this._roamers[i].gameObject);
			}
		}
		this._childCountCache = 0;
		this._childAmount = 0;
		this._roamers.Clear();
	}

	// Token: 0x04000231 RID: 561
	public FlockChild _childPrefab;

	// Token: 0x04000232 RID: 562
	public int _childAmount = 250;

	// Token: 0x04000233 RID: 563
	public bool _slowSpawn;

	// Token: 0x04000234 RID: 564
	public float _spawnSphere = 3f;

	// Token: 0x04000235 RID: 565
	public float _spawnSphereHeight = 3f;

	// Token: 0x04000236 RID: 566
	public float _spawnSphereDepth = -1f;

	// Token: 0x04000237 RID: 567
	public float _minSpeed = 6f;

	// Token: 0x04000238 RID: 568
	public float _maxSpeed = 10f;

	// Token: 0x04000239 RID: 569
	public float _minScale = 0.7f;

	// Token: 0x0400023A RID: 570
	public float _maxScale = 1f;

	// Token: 0x0400023B RID: 571
	public float _soarFrequency;

	// Token: 0x0400023C RID: 572
	public string _soarAnimation = "Soar";

	// Token: 0x0400023D RID: 573
	public string _flapAnimation = "Flap";

	// Token: 0x0400023E RID: 574
	public string _idleAnimation = "Idle";

	// Token: 0x0400023F RID: 575
	public float _diveValue = 7f;

	// Token: 0x04000240 RID: 576
	public float _diveFrequency = 0.5f;

	// Token: 0x04000241 RID: 577
	public float _minDamping = 1f;

	// Token: 0x04000242 RID: 578
	public float _maxDamping = 2f;

	// Token: 0x04000243 RID: 579
	public float _waypointDistance = 1f;

	// Token: 0x04000244 RID: 580
	public float _minAnimationSpeed = 2f;

	// Token: 0x04000245 RID: 581
	public float _maxAnimationSpeed = 4f;

	// Token: 0x04000246 RID: 582
	public float _randomPositionTimer = 10f;

	// Token: 0x04000247 RID: 583
	public float _positionSphere = 25f;

	// Token: 0x04000248 RID: 584
	public float _positionSphereHeight = 25f;

	// Token: 0x04000249 RID: 585
	public float _positionSphereDepth = -1f;

	// Token: 0x0400024A RID: 586
	public bool _childTriggerPos;

	// Token: 0x0400024B RID: 587
	public bool _forceChildWaypoints;

	// Token: 0x0400024C RID: 588
	public float _forcedRandomDelay = 1.5f;

	// Token: 0x0400024D RID: 589
	public bool _flatFly;

	// Token: 0x0400024E RID: 590
	public bool _flatSoar;

	// Token: 0x0400024F RID: 591
	public bool _birdAvoid;

	// Token: 0x04000250 RID: 592
	public int _birdAvoidHorizontalForce = 1000;

	// Token: 0x04000251 RID: 593
	public bool _birdAvoidDown;

	// Token: 0x04000252 RID: 594
	public bool _birdAvoidUp;

	// Token: 0x04000253 RID: 595
	public int _birdAvoidVerticalForce = 300;

	// Token: 0x04000254 RID: 596
	public float _birdAvoidDistanceMax = 4.5f;

	// Token: 0x04000255 RID: 597
	public float _birdAvoidDistanceMin = 5f;

	// Token: 0x04000256 RID: 598
	public float _soarMaxTime;

	// Token: 0x04000257 RID: 599
	public LayerMask _avoidanceMask = -1;

	// Token: 0x04000258 RID: 600
	public List<FlockChild> _roamers;

	// Token: 0x04000259 RID: 601
	public Vector3 _posBuffer;

	// Token: 0x0400025A RID: 602
	public bool _skipFrame;

	// Token: 0x0400025B RID: 603
	public int _updateDivisor = 2;

	// Token: 0x0400025C RID: 604
	public float _newDelta;

	// Token: 0x0400025D RID: 605
	public int _updateCounter;

	// Token: 0x0400025E RID: 606
	public float _activeChildren;

	// Token: 0x0400025F RID: 607
	public bool _groupChildToNewTransform;

	// Token: 0x04000260 RID: 608
	public Transform _groupTransform;

	// Token: 0x04000261 RID: 609
	public string _groupName = "";

	// Token: 0x04000262 RID: 610
	public bool _groupChildToFlock;

	// Token: 0x04000263 RID: 611
	public Vector3 _startPosOffset;

	// Token: 0x04000264 RID: 612
	public Transform _thisT;

	// Token: 0x04000265 RID: 613
	public bool LimitPitchRotation = true;

	// Token: 0x04000266 RID: 614
	public int _childCountCache;
}
