using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001A RID: 26
[RequireComponent(typeof(Rigidbody))]
public class RamBuoyancy : MonoBehaviour
{
	// Token: 0x0600005C RID: 92 RVA: 0x0008D4D0 File Offset: 0x0008B6D0
	private void Start()
	{
		this.rigidbody = base.GetComponent<Rigidbody>();
		if (RamBuoyancy.ramSplines == null)
		{
			RamBuoyancy.ramSplines = UnityEngine.Object.FindObjectsOfType<RamSpline>();
		}
		if (RamBuoyancy.lakePolygons == null)
		{
			RamBuoyancy.lakePolygons = UnityEngine.Object.FindObjectsOfType<LakePolygon>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.collider == null)
		{
			Debug.LogError("Buoyancy doesn't have collider");
			base.enabled = false;
			return;
		}
		if (this.autoGenerateVolumePoints)
		{
			Vector3 size = this.collider.bounds.size;
			Vector3 min = this.collider.bounds.min;
			Vector3 vector = new Vector3(size.x / (float)this.pointsInAxis, size.y / (float)this.pointsInAxis, size.z / (float)this.pointsInAxis);
			for (int i = 0; i <= this.pointsInAxis; i++)
			{
				for (int j = 0; j <= this.pointsInAxis; j++)
				{
					for (int k = 0; k <= this.pointsInAxis; k++)
					{
						Vector3 vector2 = new Vector3(min.x + (float)i * vector.x, min.y + (float)j * vector.y, min.z + (float)k * vector.z);
						if (Vector3.Distance(this.collider.ClosestPoint(vector2), vector2) < 1E-45f)
						{
							this.volumePoints.Add(base.transform.InverseTransformPoint(vector2));
						}
					}
				}
			}
		}
		this.volumePointsMatrix = new Vector3[this.volumePoints.Count];
	}

	// Token: 0x0600005D RID: 93 RVA: 0x000449F7 File Offset: 0x00042BF7
	private void FixedUpdate()
	{
		this.WaterPhysics();
	}

	// Token: 0x0600005E RID: 94 RVA: 0x0008D678 File Offset: 0x0008B878
	public void WaterPhysics()
	{
		if (this.volumePoints.Count == 0)
		{
			Debug.Log("Not initiated Buoyancy");
			return;
		}
		Ray ray = default(Ray);
		ray.direction = Vector3.up;
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		this.lowestPoint = this.volumePoints[0];
		float num = float.MaxValue;
		for (int i = 0; i < this.volumePoints.Count; i++)
		{
			this.volumePointsMatrix[i] = localToWorldMatrix.MultiplyPoint3x4(this.volumePoints[i]);
			if (num > this.volumePointsMatrix[i].y)
			{
				this.lowestPoint = this.volumePointsMatrix[i];
				num = this.lowestPoint.y;
			}
		}
		ray.origin = this.lowestPoint;
		this.center = Vector3.zero;
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 100f, this.layer))
		{
			Mathf.Max(this.collider.bounds.size.x, this.collider.bounds.size.z);
			int num2 = 0;
			Vector3 velocity = this.rigidbody.velocity;
			Vector3 normalized = velocity.normalized;
			num = raycastHit.point.y;
			for (int j = 0; j < this.volumePointsMatrix.Length; j++)
			{
				if (this.volumePointsMatrix[j].y <= num)
				{
					this.center += this.volumePointsMatrix[j];
					num2++;
				}
			}
			this.center /= (float)num2;
			this.rigidbody.AddForceAtPosition(Vector3.up * this.buoyancy * (num - this.center.y), this.center);
			this.rigidbody.AddForce(velocity * -1f * this.viscosity);
			if (velocity.magnitude > 0.01f)
			{
				Vector3 normalized2 = Vector3.Cross(velocity, new Vector3(1f, 1f, 1f)).normalized;
				Vector3 normalized3 = Vector3.Cross(velocity, normalized2).normalized;
				Vector3 a = velocity.normalized * 10f;
				foreach (Vector3 b in this.volumePointsMatrix)
				{
					Vector3 origin = a + b;
					Ray ray2 = new Ray(origin, -normalized);
					RaycastHit raycastHit2;
					if (this.collider.Raycast(ray2, out raycastHit2, 50f))
					{
						Vector3 pointVelocity = this.rigidbody.GetPointVelocity(raycastHit2.point);
						this.rigidbody.AddForceAtPosition(-pointVelocity * this.viscosityAngular, raycastHit2.point);
						if (this.debug)
						{
							Debug.DrawRay(raycastHit2.point, -pointVelocity * this.viscosityAngular, Color.red, 0.1f);
						}
					}
				}
			}
			RamSpline component = raycastHit.collider.GetComponent<RamSpline>();
			LakePolygon component2 = raycastHit.collider.GetComponent<LakePolygon>();
			if (component != null)
			{
				Mesh sharedMesh = component.meshfilter.sharedMesh;
				int num3 = sharedMesh.triangles[raycastHit.triangleIndex * 3];
				Vector3 vector = component.verticeDirection[num3];
				Vector2 vector2 = sharedMesh.uv4[num3];
				vector = vector * vector2.y - new Vector3(vector.z, vector.y, -vector.x) * vector2.x;
				this.rigidbody.AddForce(new Vector3(vector.x, 0f, vector.z) * component.floatSpeed);
				if (this.debug)
				{
					Debug.DrawRay(this.center, Vector3.up * this.buoyancy * (num - this.center.y) * 5f, Color.blue);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * -1f * this.viscosity * 5f, Color.magenta);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * 5f, Color.grey);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, this.rigidbody.angularVelocity * 5f, Color.black);
				}
			}
			if (component2 != null)
			{
				Mesh sharedMesh2 = component2.meshfilter.sharedMesh;
				int num4 = sharedMesh2.triangles[raycastHit.triangleIndex * 3];
				Vector2 vector3 = -sharedMesh2.uv4[num4];
				Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.y);
				this.rigidbody.AddForce(new Vector3(vector4.x, 0f, vector4.z) * component2.floatSpeed);
				float maxRadiansDelta = 1f * Time.deltaTime;
				Vector3 forward = Vector3.RotateTowards(base.transform.forward, vector4, maxRadiansDelta, 0f);
				base.transform.rotation = Quaternion.LookRotation(forward);
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position + Vector3.up, vector4 * 5f, Color.red);
				}
				if (this.debug)
				{
					Debug.DrawRay(this.center, Vector3.up * this.buoyancy * (num - this.center.y) * 5f, Color.blue);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * -1f * this.viscosity * 5f, Color.magenta);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * 5f, Color.grey);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, this.rigidbody.angularVelocity * 5f, Color.black);
				}
			}
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x0008DD4C File Offset: 0x0008BF4C
	private void OnDrawGizmosSelected()
	{
		if (!this.debug)
		{
			return;
		}
		if (this.collider != null && this.volumePointsMatrix != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			foreach (Vector3 vector in this.volumePointsMatrix)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(vector, 0.08f);
			}
		}
		Vector3 vector2 = this.lowestPoint;
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(this.lowestPoint, 0.08f);
		Vector3 vector3 = this.center;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.center, 0.08f);
	}

	// Token: 0x040000D1 RID: 209
	public float buoyancy = 30f;

	// Token: 0x040000D2 RID: 210
	public float viscosity = 2f;

	// Token: 0x040000D3 RID: 211
	public float viscosityAngular = 0.4f;

	// Token: 0x040000D4 RID: 212
	public LayerMask layer = 16;

	// Token: 0x040000D5 RID: 213
	public Collider collider;

	// Token: 0x040000D6 RID: 214
	[Range(2f, 10f)]
	public int pointsInAxis = 2;

	// Token: 0x040000D7 RID: 215
	private Rigidbody rigidbody;

	// Token: 0x040000D8 RID: 216
	private static RamSpline[] ramSplines;

	// Token: 0x040000D9 RID: 217
	private static LakePolygon[] lakePolygons;

	// Token: 0x040000DA RID: 218
	public List<Vector3> volumePoints = new List<Vector3>();

	// Token: 0x040000DB RID: 219
	public bool autoGenerateVolumePoints = true;

	// Token: 0x040000DC RID: 220
	private Vector3[] volumePointsMatrix;

	// Token: 0x040000DD RID: 221
	private Vector3 lowestPoint;

	// Token: 0x040000DE RID: 222
	private Vector3 center = Vector3.zero;

	// Token: 0x040000DF RID: 223
	public bool debug;
}
