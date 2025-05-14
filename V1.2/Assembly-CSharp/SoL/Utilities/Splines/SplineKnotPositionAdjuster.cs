using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

namespace SoL.Utilities.Splines
{
	// Token: 0x0200030A RID: 778
	public class SplineKnotPositionAdjuster : MonoBehaviour
	{
		// Token: 0x060015B3 RID: 5555 RVA: 0x000FDA04 File Offset: 0x000FBC04
		private void AdjustPosition()
		{
			if (this.m_containers == null)
			{
				return;
			}
			for (int i = 0; i < this.m_containers.Length; i++)
			{
				if (!(this.m_containers[i] == null))
				{
					int num = 0;
					int num2 = 0;
					Transform transform = this.m_containers[i].gameObject.transform;
					foreach (Spline spline in this.m_containers[i].Splines)
					{
						List<BezierKnot> list = new List<BezierKnot>();
						list.AddRange(spline.Knots);
						for (int j = 0; j < list.Count; j++)
						{
							bool flag = false;
							BezierKnot bezierKnot = list[j];
							if (this.m_zeroOutRotation)
							{
								bezierKnot.Rotation = quaternion.identity;
								flag = true;
							}
							SplineKnotPositionAdjuster.GroundAlignmentType groundAlignmentType = this.m_groundAlignmentType;
							if (groundAlignmentType != SplineKnotPositionAdjuster.GroundAlignmentType.Terrain)
							{
								if (groundAlignmentType == SplineKnotPositionAdjuster.GroundAlignmentType.NavMesh)
								{
									Vector3 sourcePosition = transform.TransformPoint(bezierKnot.Position);
									NavMeshHit navMeshHit;
									if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, 5f, -1))
									{
										Vector3 position = navMeshHit.position + Vector3.up * this.m_positionOffset;
										bezierKnot.Position = transform.InverseTransformPoint(position);
										flag = true;
									}
									else
									{
										Debug.LogWarning("Could not locate nav at " + sourcePosition.ToString() + "!");
									}
								}
							}
							else
							{
								Vector3 a = transform.TransformPoint(bezierKnot.Position);
								Vector3 b = Vector3.up * 5f;
								Vector3 a2;
								if (this.TryLocateTerrain(a + b, Vector3.down, out a2) || this.TryLocateTerrain(a - b, Vector3.up, out a2))
								{
									Vector3 position2 = a2 + Vector3.up * this.m_positionOffset;
									bezierKnot.Position = transform.InverseTransformPoint(position2);
									flag = true;
								}
							}
							if (!this.m_dryRun && flag)
							{
								spline.SetKnot(j, bezierKnot, BezierTangent.Out);
							}
							if (flag)
							{
								num2++;
							}
							num++;
						}
					}
					Debug.Log(string.Concat(new string[]
					{
						"Modified ",
						num2.ToString(),
						"/",
						num.ToString(),
						" on ",
						this.m_containers[i].gameObject.name
					}));
				}
			}
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x000FDCA4 File Offset: 0x000FBEA4
		private bool TryLocateTerrain(Vector3 worldPos, Vector3 direction, out Vector3 point)
		{
			point = Vector3.zero;
			RaycastHit[] hits = Hits.Hits100;
			int num = Physics.RaycastNonAlloc(worldPos, direction, hits, 20f);
			for (int i = 0; i < num; i++)
			{
				if (hits[i].collider && hits[i].collider is TerrainCollider)
				{
					point = hits[i].point;
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001DC4 RID: 7620
		[SerializeField]
		private SplineContainer[] m_containers;

		// Token: 0x04001DC5 RID: 7621
		[SerializeField]
		private bool m_dryRun;

		// Token: 0x04001DC6 RID: 7622
		[SerializeField]
		private bool m_zeroOutRotation;

		// Token: 0x04001DC7 RID: 7623
		[SerializeField]
		private SplineKnotPositionAdjuster.GroundAlignmentType m_groundAlignmentType;

		// Token: 0x04001DC8 RID: 7624
		[SerializeField]
		private float m_positionOffset;

		// Token: 0x0200030B RID: 779
		private enum GroundAlignmentType
		{
			// Token: 0x04001DCA RID: 7626
			None,
			// Token: 0x04001DCB RID: 7627
			Terrain,
			// Token: 0x04001DCC RID: 7628
			NavMesh
		}
	}
}
