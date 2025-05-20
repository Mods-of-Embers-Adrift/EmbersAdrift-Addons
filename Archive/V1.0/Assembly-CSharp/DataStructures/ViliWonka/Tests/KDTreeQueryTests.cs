using System;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;

namespace DataStructures.ViliWonka.Tests
{
	// Token: 0x02000155 RID: 341
	public class KDTreeQueryTests : MonoBehaviour
	{
		// Token: 0x06000B9E RID: 2974 RVA: 0x000CCC48 File Offset: 0x000CAE48
		private void Awake()
		{
			this.pointCloud = new Vector3[20000];
			this.query = new KDQuery(2048);
			for (int i = 0; i < this.pointCloud.Length; i++)
			{
				this.pointCloud[i] = new Vector3(1f + UnityEngine.Random.value * 0.25f, 1f + UnityEngine.Random.value * 0.25f, 1f + UnityEngine.Random.value * 0.25f);
			}
			for (int j = 0; j < this.pointCloud.Length; j++)
			{
				for (int k = 0; k < j; k++)
				{
					this.pointCloud[j] += this.LorenzStep(this.pointCloud[j]) * 0.01f;
				}
			}
			this.tree = new KDTree(this.pointCloud, 32);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x000CCD38 File Offset: 0x000CAF38
		private Vector3 LorenzStep(Vector3 p)
		{
			float num = 28f;
			float num2 = 10f;
			float num3 = 2.6666667f;
			return new Vector3(num2 * (p.y - p.x), p.x * (num - p.z) - p.y, p.x * p.y - num3 * p.z);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x000CCD98 File Offset: 0x000CAF98
		private void Update()
		{
			for (int i = 0; i < this.tree.Count; i++)
			{
				this.tree.Points[i] += this.LorenzStep(this.tree.Points[i]) * Time.deltaTime * 0.1f;
			}
			this.tree.Rebuild(-1);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x000CCE14 File Offset: 0x000CB014
		private void OnDrawGizmos()
		{
			if (this.query == null)
			{
				return;
			}
			Vector3 vector = 0.2f * Vector3.one;
			for (int i = 0; i < this.pointCloud.Length; i++)
			{
				Gizmos.DrawCube(this.pointCloud[i], vector);
			}
			List<int> list = new List<int>();
			Color red = Color.red;
			red.a = 0.5f;
			Gizmos.color = red;
			switch (this.QueryType)
			{
			case QType.ClosestPoint:
				this.query.ClosestPoint(this.tree, base.transform.position, list, null);
				break;
			case QType.KNearest:
				this.query.KNearest(this.tree, base.transform.position, this.K, list, null);
				break;
			case QType.Radius:
				this.query.Radius(this.tree, base.transform.position, this.Radius, list);
				Gizmos.DrawWireSphere(base.transform.position, this.Radius);
				break;
			case QType.Interval:
				this.query.Interval(this.tree, base.transform.position - this.IntervalSize / 2f, base.transform.position + this.IntervalSize / 2f, list);
				Gizmos.DrawWireCube(base.transform.position, this.IntervalSize);
				break;
			}
			for (int j = 0; j < list.Count; j++)
			{
				Gizmos.DrawCube(this.pointCloud[list[j]], 2f * vector);
			}
			Gizmos.color = Color.green;
			Gizmos.DrawCube(base.transform.position, 4f * vector);
			if (this.DrawQueryNodes)
			{
				this.query.DrawLastQuery();
			}
		}

		// Token: 0x04000B38 RID: 2872
		public QType QueryType;

		// Token: 0x04000B39 RID: 2873
		public int K = 13;

		// Token: 0x04000B3A RID: 2874
		[Range(0f, 100f)]
		public float Radius = 0.1f;

		// Token: 0x04000B3B RID: 2875
		public bool DrawQueryNodes = true;

		// Token: 0x04000B3C RID: 2876
		public Vector3 IntervalSize = new Vector3(0.2f, 0.2f, 0.2f);

		// Token: 0x04000B3D RID: 2877
		private Vector3[] pointCloud;

		// Token: 0x04000B3E RID: 2878
		private KDTree tree;

		// Token: 0x04000B3F RID: 2879
		private KDQuery query;
	}
}
