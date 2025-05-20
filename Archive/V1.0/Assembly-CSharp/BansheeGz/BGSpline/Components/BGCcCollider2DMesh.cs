using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001A5 RID: 421
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCollider2DMesh")]
	[BGCc.CcDescriptor(Description = "Create a mesh collider inside 2D spline.", Name = "Collider 2D Mesh", Icon = "BGCcCollider2DMesh123")]
	[RequireComponent(typeof(PolygonCollider2D))]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCollider2DMesh")]
	public class BGCcCollider2DMesh : BGCcColliderMeshAbstract<PolygonCollider2D>
	{
		// Token: 0x06000EDA RID: 3802 RVA: 0x000DB4EC File Offset: 0x000D96EC
		protected override void Build(PolygonCollider2D collider, List<Vector3> positions)
		{
			collider.points = this.to2DArray(positions);
			if (base.IsMeshGenerationOn)
			{
				base.Get<MeshRenderer>();
				MeshFilter meshFilter = base.Get<MeshFilter>();
				Mesh mesh = meshFilter.sharedMesh;
				if (mesh == null)
				{
					mesh = new Mesh();
				}
				if (this.triangulator == null)
				{
					this.triangulator = new BGTriangulator2D();
				}
				this.triangulator.Bind(mesh, positions, new BGTriangulator2D.Config
				{
					Closed = base.Curve.Closed,
					Mode2D = base.Curve.Mode2D,
					Flip = false,
					ScaleUV = Vector2.one,
					OffsetUV = Vector2.zero,
					DoubleSided = false,
					ScaleBackUV = Vector2.one,
					OffsetBackUV = Vector2.zero
				});
				meshFilter.sharedMesh = mesh;
			}
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x000DB5BC File Offset: 0x000D97BC
		private Vector2[] to2DArray(List<Vector3> positions)
		{
			Vector2[] array = new Vector2[positions.Count];
			switch (base.Curve.Mode2D)
			{
			case BGCurve.Mode2DEnum.Off:
				throw new ArgumentOutOfRangeException("2D Mode for a curve should be on");
			case BGCurve.Mode2DEnum.XY:
				for (int i = 0; i < positions.Count; i++)
				{
					Vector3 vector = positions[i];
					array[i] = new Vector2(vector.x, vector.y);
				}
				break;
			case BGCurve.Mode2DEnum.XZ:
				for (int j = 0; j < positions.Count; j++)
				{
					Vector3 vector2 = positions[j];
					array[j] = new Vector2(vector2.x, vector2.z);
				}
				break;
			case BGCurve.Mode2DEnum.YZ:
				for (int k = 0; k < positions.Count; k++)
				{
					Vector3 vector3 = positions[k];
					array[k] = new Vector2(vector3.y, vector3.z);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("Curve.Mode2D");
			}
			return array;
		}

		// Token: 0x04000D06 RID: 3334
		private BGTriangulator2D triangulator;
	}
}
