using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
[RequireComponent(typeof(Rigidbody))]
public class Vulture : MonoBehaviour
{
	// Token: 0x0600011F RID: 287 RVA: 0x00044FCF File Offset: 0x000431CF
	private void Start()
	{
		this.GenerateWanderPoints();
		this.flapFrequency += (float)UnityEngine.Random.Range(0, 100) * 0.02f;
	}

	// Token: 0x06000120 RID: 288 RVA: 0x000974A0 File Offset: 0x000956A0
	private void GenerateWanderPoints()
	{
		if (!this.enableWandering)
		{
			return;
		}
		this.wanderPoints = new Vector3[this.numberOfWanderPoints];
		for (int i = 0; i < this.numberOfWanderPoints; i++)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * this.wanderRange;
			float y = UnityEngine.Random.Range(0f, this.wanderHeightOffset);
			Vector3 vector2 = new Vector3(vector.x, y, vector.y) + base.transform.position;
			this.wanderPoints[i] = vector2;
		}
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0009752C File Offset: 0x0009572C
	private void Update()
	{
		this.rigidBody.AddForce(base.transform.forward * this.speed, ForceMode.VelocityChange);
		this.HandleWandering();
		this.timeSinceLastFlap += Time.deltaTime;
		if (this.timeSinceLastFlap > this.flapFrequency)
		{
			this.Flap();
		}
		if (this.flap)
		{
			this.timeSpentFlapping += Time.deltaTime;
			if (this.timeSpentFlapping > this.flapTime)
			{
				this.flap = false;
				this.anim.SetBool("Flap", false);
				if (this.debugMode)
				{
					Debug.Log("Vulture exiting flap");
				}
			}
		}
		this.timeLastLookChanged += Time.deltaTime;
		if (this.timeLastLookChanged > this.changelookEveryX)
		{
			this.timeLastLookChanged = 0f;
			this.lookpose = UnityEngine.Random.Range(0, 4);
		}
		this.anim.SetInteger("Look", this.lookpose);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00097628 File Offset: 0x00095828
	private void HandleWandering()
	{
		Vector3 vector = this.wanderPoints[this.wanderIndex];
		Vector3 vector2 = vector - base.transform.position;
		Vector3 b = Quaternion.LookRotation(vector2, Vector3.up) * Vector3.forward;
		Vector3 vector3 = -Vector3.Cross(vector2 - b, Vector3.up).normalized * (this.wanderPointProximity * this.orbitDistance);
		if (this.debugMode)
		{
			Debug.DrawRay(vector, vector3);
		}
		float angle = this.maxBankingAngle * -Vector3.Dot(base.transform.right, vector2.normalized);
		Quaternion localRotation = this.banker.localRotation;
		float maxDegreesDelta = Time.deltaTime * this.turnSpeed;
		this.banker.localRotation = Quaternion.RotateTowards(localRotation, Quaternion.AngleAxis(angle, Vector3.forward), maxDegreesDelta);
		if (Vector3.Distance(base.transform.position, vector) > this.deadzoneRadius)
		{
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector2 + vector3), maxDegreesDelta);
		}
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, 0f);
		if (Vector3.Distance(base.transform.position, vector) < this.wanderPointProximity && UnityEngine.Random.value < this.chanceToMoveOn)
		{
			if (this.wanderIndex < this.numberOfWanderPoints - 1)
			{
				this.wanderIndex++;
				return;
			}
			this.wanderIndex = 0;
		}
	}

	// Token: 0x06000123 RID: 291 RVA: 0x000977E4 File Offset: 0x000959E4
	private void Flap()
	{
		this.timeSinceLastFlap = 0f;
		this.timeSpentFlapping = 0f;
		this.rigidBody.AddForce(base.transform.up * this.flapForce, ForceMode.Impulse);
		this.flap = true;
		this.anim.SetBool("Flap", true);
		if (this.debugMode)
		{
			Debug.Log("Vulture entering flap");
		}
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00097854 File Offset: 0x00095A54
	private void OnDrawGizmos()
	{
		if (!Application.isPlaying || !this.enableWandering || !this.debugMode)
		{
			return;
		}
		for (int i = 0; i < this.numberOfWanderPoints; i++)
		{
			Vector3 vector = this.wanderPoints[i];
			bool flag = Vector3.Distance(base.transform.position, vector) < this.wanderPointProximity;
			if (i == this.wanderIndex)
			{
				Gizmos.color = Color.yellow;
				if (flag)
				{
					Gizmos.color = Color.green;
				}
			}
			else if (flag)
			{
				Gizmos.color = Color.blue;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawSphere(vector, 0.3f);
			Gizmos.DrawWireSphere(vector, this.wanderPointProximity);
		}
		Gizmos.color = Color.red;
		for (int j = 0; j < this.numberOfWanderPoints - 1; j++)
		{
			Gizmos.DrawLine(this.wanderPoints[j], this.wanderPoints[j + 1]);
		}
		Gizmos.DrawLine(this.wanderPoints[this.numberOfWanderPoints - 1], this.wanderPoints[0]);
	}

	// Token: 0x040002AE RID: 686
	[Header("Vulture general movement settings.")]
	[Tooltip("Speed of the vulture.")]
	public float speed = 3f;

	// Token: 0x040002AF RID: 687
	[Tooltip("Maximum banking angle.")]
	public float maxBankingAngle = 45f;

	// Token: 0x040002B0 RID: 688
	[Tooltip("Turn speed")]
	public float turnSpeed = 30f;

	// Token: 0x040002B1 RID: 689
	[Header("Flapping")]
	[Tooltip("Flap force power")]
	public float flapForce;

	// Token: 0x040002B2 RID: 690
	[Tooltip("How often to flap.")]
	public float flapFrequency = 3f;

	// Token: 0x040002B3 RID: 691
	[Tooltip("How long to flap for.")]
	public float flapTime = 1f;

	// Token: 0x040002B4 RID: 692
	[Header("Vulture wandering settings")]
	public bool enableWandering = true;

	// Token: 0x040002B5 RID: 693
	[Tooltip("How far from starting Pos to wander off to")]
	public float wanderRange = 50f;

	// Token: 0x040002B6 RID: 694
	[Tooltip("How far much to offset in height")]
	public float wanderHeightOffset = 10f;

	// Token: 0x040002B7 RID: 695
	[Tooltip("How many wander points")]
	[Range(2f, 10f)]
	public int numberOfWanderPoints = 4;

	// Token: 0x040002B8 RID: 696
	[Tooltip("How near to get to wander point")]
	public float wanderPointProximity = 20f;

	// Token: 0x040002B9 RID: 697
	[Tooltip("Preferred orbit distance")]
	[Range(0f, 1f)]
	public float orbitDistance = 1f;

	// Token: 0x040002BA RID: 698
	[Tooltip("Deadzone radius")]
	public float deadzoneRadius = 1f;

	// Token: 0x040002BB RID: 699
	[Tooltip("Percent chance to keep circling before moving on")]
	[Range(0f, 1f)]
	public float chanceToMoveOn = 0.002f;

	// Token: 0x040002BC RID: 700
	[Header("Vulture Head look settings")]
	[Tooltip("How often to change look")]
	public float changelookEveryX = 2f;

	// Token: 0x040002BD RID: 701
	public bool debugMode;

	// Token: 0x040002BE RID: 702
	private int lookpose;

	// Token: 0x040002BF RID: 703
	private float timeSinceLastFlap;

	// Token: 0x040002C0 RID: 704
	private float timeSpentFlapping;

	// Token: 0x040002C1 RID: 705
	private bool flap;

	// Token: 0x040002C2 RID: 706
	private Vector3[] wanderPoints;

	// Token: 0x040002C3 RID: 707
	private int wanderIndex;

	// Token: 0x040002C4 RID: 708
	private float timeLastLookChanged;

	// Token: 0x040002C5 RID: 709
	public Rigidbody rigidBody;

	// Token: 0x040002C6 RID: 710
	public Animator anim;

	// Token: 0x040002C7 RID: 711
	public Transform banker;
}
