using System;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001F7 RID: 503
	[RequireComponent(typeof(BGCcMath))]
	public class BGTestPerformance : MonoBehaviour
	{
		// Token: 0x06001160 RID: 4448 RVA: 0x000E36C8 File Offset: 0x000E18C8
		private void Start()
		{
			this.curve = base.GetComponent<BGCurve>();
			BGCcMath component = base.GetComponent<BGCcMath>();
			this.math = component.Math;
			this.curve = component.Curve;
			this.oldPos = new Vector3[this.PointsCount];
			this.newPos = new Vector3[this.PointsCount];
			this.speed = new float[this.ObjectsCount];
			this.distances = new float[this.ObjectsCount];
			this.objects = new GameObject[this.ObjectsCount];
			for (int i = 0; i < this.PointsCount; i++)
			{
				BGCurvePoint.ControlTypeEnum controlType = BGCurvePoint.ControlTypeEnum.BezierIndependant;
				BGTestPerformance.ControlTypeForNewPoints controlType2 = this.ControlType;
				if (controlType2 != BGTestPerformance.ControlTypeForNewPoints.Random)
				{
					if (controlType2 == BGTestPerformance.ControlTypeForNewPoints.Absent)
					{
						controlType = BGCurvePoint.ControlTypeEnum.Absent;
					}
				}
				else
				{
					controlType = (BGCurvePoint.ControlTypeEnum)UnityEngine.Random.Range(0, 3);
				}
				this.curve.AddPoint(new BGCurvePoint(this.curve, this.RandomVector(), controlType, this.RandomVector(), this.RandomVector(), false));
			}
			this.math.Recalculate(false);
			if (this.ObjectToMove != null)
			{
				float maxInclusive = this.oldDistance = this.math.GetDistance(-1);
				for (int j = 0; j < this.ObjectsCount; j++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ObjectToMove, Vector3.zero, Quaternion.identity);
					gameObject.transform.parent = base.transform;
					this.objects[j] = gameObject;
					this.distances[j] = UnityEngine.Random.Range(0f, maxInclusive);
				}
				this.ObjectToMove.SetActive(false);
				for (int k = 0; k < this.ObjectsCount; k++)
				{
					this.speed[k] = ((UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(-5f, -1.5f) : UnityEngine.Random.Range(1.5f, 5f));
				}
			}
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x000E3894 File Offset: 0x000E1A94
		private void Update()
		{
			if (Time.time - this.startTime > 10f)
			{
				this.startTime = Time.time;
				for (int i = 0; i < this.PointsCount; i++)
				{
					this.oldPos[i] = this.newPos[i];
					this.newPos[i] = this.RandomVector();
				}
			}
			float t = (Time.time - this.startTime) / 10f;
			BGCurvePointI[] points = this.curve.Points;
			for (int j = 0; j < this.PointsCount; j++)
			{
				points[j].PositionLocal = Vector3.Lerp(this.oldPos[j], this.newPos[j], t);
			}
			float distance = this.math.GetDistance(-1);
			if (this.ObjectToMove != null)
			{
				float num = distance / this.oldDistance;
				for (int k = 0; k < this.ObjectsCount; k++)
				{
					float num2 = this.distances[k];
					num2 *= num;
					num2 += this.speed[k] * Time.deltaTime;
					if (num2 < 0f)
					{
						this.speed[k] = -this.speed[k];
						num2 = 0f;
					}
					else if (num2 > distance)
					{
						this.speed[k] = -this.speed[k];
						num2 = distance;
					}
					this.distances[k] = num2;
					this.objects[k].transform.position = this.math.CalcByDistance(BGCurveBaseMath.Field.Position, num2, false);
				}
			}
			this.oldDistance = distance;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0004E66F File Offset: 0x0004C86F
		private Vector3 RandomVector()
		{
			return new Vector3(this.Range(0), this.Range(1), this.Range(2));
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x000E3A34 File Offset: 0x000E1C34
		private float Range(int index)
		{
			return UnityEngine.Random.Range(this.Bounds.min[index], this.Bounds.max[index]);
		}

		// Token: 0x04000EC4 RID: 3780
		private const float SpeedRange = 5f;

		// Token: 0x04000EC5 RID: 3781
		private const int Period = 10;

		// Token: 0x04000EC6 RID: 3782
		[Tooltip("Object's prefab")]
		public GameObject ObjectToMove;

		// Token: 0x04000EC7 RID: 3783
		[Tooltip("Limits for points positions and transitions")]
		public Bounds Bounds = new Bounds(Vector3.zero, Vector3.one);

		// Token: 0x04000EC8 RID: 3784
		[Tooltip("Number of points to spawn")]
		[Range(2f, 2000f)]
		public int PointsCount = 100;

		// Token: 0x04000EC9 RID: 3785
		[Tooltip("Number of objects to spawn")]
		[Range(1f, 500f)]
		public int ObjectsCount = 100;

		// Token: 0x04000ECA RID: 3786
		[Tooltip("Control Type")]
		public BGTestPerformance.ControlTypeForNewPoints ControlType;

		// Token: 0x04000ECB RID: 3787
		private float startTime = -1000f;

		// Token: 0x04000ECC RID: 3788
		private BGCurve curve;

		// Token: 0x04000ECD RID: 3789
		private BGCurveBaseMath math;

		// Token: 0x04000ECE RID: 3790
		private Vector3[] oldPos;

		// Token: 0x04000ECF RID: 3791
		private Vector3[] newPos;

		// Token: 0x04000ED0 RID: 3792
		private GameObject[] objects;

		// Token: 0x04000ED1 RID: 3793
		private float[] speed;

		// Token: 0x04000ED2 RID: 3794
		private float[] distances;

		// Token: 0x04000ED3 RID: 3795
		private float oldDistance;

		// Token: 0x020001F8 RID: 504
		public enum ControlTypeForNewPoints
		{
			// Token: 0x04000ED5 RID: 3797
			Random,
			// Token: 0x04000ED6 RID: 3798
			Absent,
			// Token: 0x04000ED7 RID: 3799
			Bezier
		}
	}
}
