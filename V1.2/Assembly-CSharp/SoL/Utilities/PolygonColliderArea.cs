using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeTechnologies;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.VegetationSystem.Biomes;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002AC RID: 684
	[ExecuteInEditMode]
	public class PolygonColliderArea : MonoBehaviour
	{
		// Token: 0x06001450 RID: 5200 RVA: 0x000F9918 File Offset: 0x000F7B18
		private void CopyNodes()
		{
			if (this.m_biomeToCopyFrom != null)
			{
				base.gameObject.transform.position = this.m_biomeToCopyFrom.gameObject.transform.position;
				base.gameObject.transform.rotation = this.m_biomeToCopyFrom.gameObject.transform.rotation;
				this.Nodes = new List<AwesomeTechnologies.VegetationSystem.Biomes.Node>(this.m_biomeToCopyFrom.Nodes);
				this.m_biomeToCopyFrom = null;
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06001451 RID: 5201 RVA: 0x00045BCA File Offset: 0x00043DCA
		private bool m_showPostProcessingData
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x000503DF File Offset: 0x0004E5DF
		private bool m_showCopyNodes
		{
			get
			{
				return this.m_biomeToCopyFrom != null;
			}
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x000F999C File Offset: 0x000F7B9C
		private void OnEnable()
		{
			if (this.Id == "")
			{
				this.Id = Guid.NewGuid().ToString();
			}
			this._needInit = true;
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x000503ED File Offset: 0x0004E5ED
		private void Awake()
		{
			if (Application.isPlaying)
			{
				this.Init();
				this.ToggleVisualization(false);
			}
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x000F99DC File Offset: 0x000F7BDC
		private void Start()
		{
			this._lastPosition = base.transform.position;
			this._lastRotation = base.transform.rotation;
			this._lastLossyScale = base.transform.lossyScale;
			if (this.Nodes.Count == 0)
			{
				this.CreateDefaultNodes();
			}
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x00050403 File Offset: 0x0004E603
		private void Update()
		{
			if (!Application.isPlaying && this._needInit)
			{
				this.Init();
			}
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x000F9A30 File Offset: 0x000F7C30
		public virtual void Reset()
		{
			if (this.Id == "")
			{
				this.Id = Guid.NewGuid().ToString();
			}
			this.ClearNodes();
			this.CreateDefaultNodes();
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x000F9A74 File Offset: 0x000F7C74
		private void Init()
		{
			if (this._lastPosition != base.transform.position || this._lastRotation != base.transform.rotation || this._needInit || this._lastLossyScale != base.transform.lossyScale)
			{
				this.PositionNodes();
				this._lastPosition = base.transform.position;
				this._lastRotation = base.transform.rotation;
				this._lastLossyScale = base.transform.lossyScale;
				this._needInit = false;
			}
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x0005041A File Offset: 0x0004E61A
		public void ClearNodes()
		{
			this.Nodes.Clear();
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x000F9B14 File Offset: 0x000F7D14
		private void CreateDefaultNodes()
		{
			Bounds bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(6f, 1f, 6f));
			this.ClearNodes();
			for (int i = 0; i <= 3; i++)
			{
				AwesomeTechnologies.VegetationSystem.Biomes.Node node = new AwesomeTechnologies.VegetationSystem.Biomes.Node();
				switch (i)
				{
				case 0:
					node.Position = new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
					break;
				case 1:
					node.Position = new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
					break;
				case 2:
					node.Position = new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
					break;
				case 3:
					node.Position = new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
					break;
				}
				this.Nodes.Add(node);
			}
			this.PositionNodes();
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x000F9C64 File Offset: 0x000F7E64
		public void PositionNodes()
		{
			for (int i = 0; i <= this.Nodes.Count - 1; i++)
			{
				RaycastHit[] array = (from h in Physics.RaycastAll(new Ray(base.transform.TransformPoint(this.Nodes[i].Position) + new Vector3(0f, 2000f, 0f), Vector3.down))
				orderby h.distance
				select h).ToArray<RaycastHit>();
				for (int j = 0; j <= array.Length - 1; j++)
				{
					if (array[j].collider is TerrainCollider || this.GroundLayerMask.Contains(array[j].collider.gameObject.layer))
					{
						this.Nodes[i].Position = base.transform.InverseTransformPoint(array[j].point);
						break;
					}
				}
			}
			this.UpdateBiomeMask();
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x000F9D74 File Offset: 0x000F7F74
		public void AddNode(Vector3 worldPosition)
		{
			if (this.Nodes.Count == 0)
			{
				this.AddNodeToEnd(worldPosition);
				return;
			}
			AwesomeTechnologies.VegetationSystem.Biomes.Node node = this.FindClosestNode(worldPosition);
			AwesomeTechnologies.VegetationSystem.Biomes.Node nextNode = this.GetNextNode(node);
			AwesomeTechnologies.VegetationSystem.Biomes.Node previousNode = this.GetPreviousNode(node);
			LineSegment3D lineSegment3D = new LineSegment3D(base.transform.TransformPoint(node.Position), base.transform.TransformPoint(nextNode.Position));
			LineSegment3D lineSegment3D2 = new LineSegment3D(base.transform.TransformPoint(node.Position), base.transform.TransformPoint(previousNode.Position));
			float num = lineSegment3D.DistanceTo(worldPosition);
			float num2 = lineSegment3D2.DistanceTo(worldPosition);
			AwesomeTechnologies.VegetationSystem.Biomes.Node item = new AwesomeTechnologies.VegetationSystem.Biomes.Node
			{
				Position = base.transform.InverseTransformPoint(worldPosition)
			};
			int nodeIndex = this.GetNodeIndex(node);
			if (num >= num2)
			{
				this.Nodes.Insert(nodeIndex, item);
				return;
			}
			if (nodeIndex == this.Nodes.Count - 1)
			{
				this.Nodes.Add(item);
				return;
			}
			this.Nodes.Insert(nodeIndex + 1, item);
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x00050427 File Offset: 0x0004E627
		public void DeleteNode(AwesomeTechnologies.VegetationSystem.Biomes.Node node)
		{
			this.Nodes.Remove(node);
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x000F9E78 File Offset: 0x000F8078
		public void AddNodesToEnd(Vector3[] worldPositions)
		{
			for (int i = 0; i <= worldPositions.Length - 1; i++)
			{
				this.AddNodeToEnd(worldPositions[i]);
			}
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x000F9EA4 File Offset: 0x000F80A4
		public void AddNodesToEnd(Vector3[] worldPositions, bool[] disableEdges)
		{
			for (int i = 0; i <= worldPositions.Length - 1; i++)
			{
				this.AddNodeToEnd(worldPositions[i], disableEdges[i]);
			}
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x000F9ED4 File Offset: 0x000F80D4
		public void AddNodesToEnd(Vector3[] worldPositions, float[] customWidth, bool[] active)
		{
			for (int i = 0; i <= worldPositions.Length - 1; i++)
			{
				this.AddNodeToEnd(worldPositions[i], customWidth[i], active[i]);
			}
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x000F9F04 File Offset: 0x000F8104
		public void AddNodesToEnd(Vector3[] worldPositions, float[] customWidth, bool[] active, bool[] disableEdges)
		{
			for (int i = 0; i <= worldPositions.Length - 1; i++)
			{
				this.AddNodeToEnd(worldPositions[i], customWidth[i], active[i], disableEdges[i]);
			}
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x000F9F38 File Offset: 0x000F8138
		public void AddNodeToEnd(Vector3 worldPosition)
		{
			AwesomeTechnologies.VegetationSystem.Biomes.Node item = new AwesomeTechnologies.VegetationSystem.Biomes.Node
			{
				Position = base.transform.InverseTransformPoint(worldPosition)
			};
			this.Nodes.Add(item);
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x000F9F6C File Offset: 0x000F816C
		public void AddNodeToEnd(Vector3 worldPosition, bool disableEdge)
		{
			AwesomeTechnologies.VegetationSystem.Biomes.Node item = new AwesomeTechnologies.VegetationSystem.Biomes.Node
			{
				Position = base.transform.InverseTransformPoint(worldPosition),
				DisableEdge = disableEdge
			};
			this.Nodes.Add(item);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x000F9FA4 File Offset: 0x000F81A4
		public void AddNodeToEnd(Vector3 worldPosition, float customWidth, bool active)
		{
			AwesomeTechnologies.VegetationSystem.Biomes.Node item = new AwesomeTechnologies.VegetationSystem.Biomes.Node
			{
				Position = base.transform.InverseTransformPoint(worldPosition),
				CustomWidth = customWidth,
				OverrideWidth = true,
				Active = active
			};
			this.Nodes.Add(item);
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x000F9FEC File Offset: 0x000F81EC
		public void AddNodeToEnd(Vector3 worldPosition, float customWidth, bool active, bool disableEdge)
		{
			AwesomeTechnologies.VegetationSystem.Biomes.Node item = new AwesomeTechnologies.VegetationSystem.Biomes.Node
			{
				Position = base.transform.InverseTransformPoint(worldPosition),
				CustomWidth = customWidth,
				OverrideWidth = true,
				Active = active,
				DisableEdge = disableEdge
			};
			this.Nodes.Add(item);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x000FA03C File Offset: 0x000F823C
		private Vector3 GetMaskCenter()
		{
			List<Vector3> worldSpaceNodePositions = this.GetWorldSpaceNodePositions();
			return this.GetMeanVector(worldSpaceNodePositions.ToArray());
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000FA05C File Offset: 0x000F825C
		private Vector3 GetMeanVector(Vector3[] positions)
		{
			if (positions.Length == 0)
			{
				return Vector3.zero;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			foreach (Vector3 vector in positions)
			{
				num += vector.x;
				num2 += vector.y;
				num3 += vector.z;
			}
			return new Vector3(num / (float)positions.Length, num2 / (float)positions.Length, num3 / (float)positions.Length);
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x000FA0D8 File Offset: 0x000F82D8
		public int GetNodeIndex(AwesomeTechnologies.VegetationSystem.Biomes.Node node)
		{
			int result = 0;
			for (int i = 0; i <= this.Nodes.Count - 1; i++)
			{
				if (this.Nodes[i] == node)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x000FA114 File Offset: 0x000F8314
		public List<Vector3> GetWorldSpaceNodePositions()
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i <= this.Nodes.Count - 1; i++)
			{
				list.Add(base.transform.TransformPoint(this.Nodes[i].Position));
			}
			return list;
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x000FA164 File Offset: 0x000F8364
		public AwesomeTechnologies.VegetationSystem.Biomes.Node GetNextNode(AwesomeTechnologies.VegetationSystem.Biomes.Node node)
		{
			int num = 0;
			for (int i = 0; i <= this.Nodes.Count - 1; i++)
			{
				if (this.Nodes[i] == node)
				{
					num = i;
					break;
				}
			}
			if (num == this.Nodes.Count - 1)
			{
				return this.Nodes[0];
			}
			return this.Nodes[num + 1];
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x000FA1CC File Offset: 0x000F83CC
		public AwesomeTechnologies.VegetationSystem.Biomes.Node GetPreviousNode(AwesomeTechnologies.VegetationSystem.Biomes.Node node)
		{
			int num = 0;
			for (int i = 0; i <= this.Nodes.Count - 1; i++)
			{
				if (this.Nodes[i] == node)
				{
					num = i;
					break;
				}
			}
			if (num == 0)
			{
				return this.Nodes[this.Nodes.Count - 1];
			}
			return this.Nodes[num - 1];
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x000FA230 File Offset: 0x000F8430
		public AwesomeTechnologies.VegetationSystem.Biomes.Node FindClosestNode(Vector3 worldPosition)
		{
			AwesomeTechnologies.VegetationSystem.Biomes.Node result = null;
			float num = float.MaxValue;
			for (int i = 0; i <= this.Nodes.Count - 1; i++)
			{
				float num2 = Vector3.Distance(worldPosition, base.transform.TransformPoint(this.Nodes[i].Position));
				if (num2 < num)
				{
					num = num2;
					result = this.Nodes[i];
				}
			}
			return result;
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00050436 File Offset: 0x0004E636
		private void DrawGizmos()
		{
			bool showArea = this.ShowArea;
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0005043F File Offset: 0x0004E63F
		public virtual void OnDrawGizmosSelected()
		{
			this.DrawGizmos();
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x0005043F File Offset: 0x0004E63F
		public virtual void OnDrawGizmos()
		{
			this.DrawGizmos();
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x0004475B File Offset: 0x0004295B
		private void UpdateEnviro()
		{
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x000FA294 File Offset: 0x000F8494
		public void UpdateBiomeMask()
		{
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				return;
			}
			if (!this.m_isConvex && this.m_isTrigger)
			{
				this.m_isTrigger = false;
			}
			this.RefreshPostProcessVolume(this.m_layer.LayerIndex);
			this.UpdateEnviro();
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x000FA2EC File Offset: 0x000F84EC
		public void RefreshPostProcessVolume(LayerMask postProcessLayer)
		{
			base.gameObject.layer = postProcessLayer;
			if (!this.m_enabled)
			{
				MeshCollider component = base.gameObject.GetComponent<MeshCollider>();
				if (component)
				{
					this.DestroyComponent(component);
					return;
				}
			}
			else
			{
				MeshCollider meshCollider = base.gameObject.GetComponent<MeshCollider>();
				if (!meshCollider)
				{
					meshCollider = base.gameObject.AddComponent<MeshCollider>();
				}
				meshCollider.convex = this.m_isConvex;
				meshCollider.enabled = this.m_enabled;
				meshCollider.isTrigger = this.m_isTrigger;
				Vector3[] array = new Vector3[this.Nodes.Count];
				for (int i = 0; i <= this.Nodes.Count - 1; i++)
				{
					array[i] = this.Nodes[i].Position;
				}
				this.m_mesh = MeshUtils.ExtrudeMeshFromPolygon(array, this.m_volumeHeight);
				if (this.m_flipNormals)
				{
					Vector3[] normals = this.m_mesh.normals;
					for (int j = 0; j < normals.Length; j++)
					{
						normals[j] *= -1f;
					}
					this.m_mesh.normals = normals;
					for (int k = 0; k < this.m_mesh.subMeshCount; k++)
					{
						int[] triangles = this.m_mesh.GetTriangles(k);
						for (int l = 0; l < triangles.Length; l += 3)
						{
							int num = triangles[l];
							triangles[l] = triangles[l + 1];
							triangles[l + 1] = num;
						}
						this.m_mesh.SetTriangles(triangles, k);
					}
				}
				meshCollider.sharedMesh = this.m_mesh;
				this.m_meshCollider = meshCollider;
				if (this.m_visEnabled)
				{
					this.ToggleVisualization(this.m_visEnabled);
				}
			}
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x00050447 File Offset: 0x0004E647
		private void EnableVisualization()
		{
			this.ToggleVisualization(true);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x00050450 File Offset: 0x0004E650
		private void DisableVisualization()
		{
			this.ToggleVisualization(false);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x000FA4AC File Offset: 0x000F86AC
		public void ToggleVisualization(bool isVisible)
		{
			MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
			MeshFilter component2 = base.gameObject.GetComponent<MeshFilter>();
			if (this.m_mesh == null || !isVisible)
			{
				if (component != null)
				{
					component.enabled = false;
				}
				if (component2 != null)
				{
					component2.sharedMesh = null;
				}
				this.m_visEnabled = false;
				return;
			}
			if (component2 != null)
			{
				component2.sharedMesh = this.m_mesh;
				if (component != null)
				{
					component.enabled = true;
				}
				this.m_visEnabled = true;
			}
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x00050459 File Offset: 0x0004E659
		private void DestroyComponent(Component component)
		{
			if (!component)
			{
				return;
			}
			UnityEngine.Object.Destroy(component);
		}

		// Token: 0x04001CA4 RID: 7332
		private const string kParamsEditor = "Editor Data";

		// Token: 0x04001CA5 RID: 7333
		private const string kParamsCollider = "Collider Data";

		// Token: 0x04001CA6 RID: 7334
		private const string kParamsPostProcessing = "Post Processing";

		// Token: 0x04001CA7 RID: 7335
		private const string kParamsEnviro = "Enviro";

		// Token: 0x04001CA8 RID: 7336
		private const string kCopyBox = "Copy Data";

		// Token: 0x04001CA9 RID: 7337
		[HideInInspector]
		public List<AwesomeTechnologies.VegetationSystem.Biomes.Node> Nodes = new List<AwesomeTechnologies.VegetationSystem.Biomes.Node>();

		// Token: 0x04001CAA RID: 7338
		[HideInInspector]
		public string Id;

		// Token: 0x04001CAB RID: 7339
		public string MaskName = "";

		// Token: 0x04001CAC RID: 7340
		public bool ClosedArea = true;

		// Token: 0x04001CAD RID: 7341
		public bool ShowArea = true;

		// Token: 0x04001CAE RID: 7342
		public bool ShowHandles = true;

		// Token: 0x04001CAF RID: 7343
		public LayerMask GroundLayerMask;

		// Token: 0x04001CB0 RID: 7344
		[SerializeField]
		private SingleUnityLayer m_layer = new SingleUnityLayer();

		// Token: 0x04001CB1 RID: 7345
		[SerializeField]
		private bool m_enabled = true;

		// Token: 0x04001CB2 RID: 7346
		[SerializeField]
		private bool m_flipNormals;

		// Token: 0x04001CB3 RID: 7347
		[SerializeField]
		private bool m_isConvex = true;

		// Token: 0x04001CB4 RID: 7348
		[SerializeField]
		private bool m_isTrigger = true;

		// Token: 0x04001CB5 RID: 7349
		[SerializeField]
		private float m_volumeHeight = 20f;

		// Token: 0x04001CB6 RID: 7350
		[SerializeField]
		private float m_blendDistance;

		// Token: 0x04001CB7 RID: 7351
		[SerializeField]
		private float m_weight = 1f;

		// Token: 0x04001CB8 RID: 7352
		[SerializeField]
		private float m_priority = 1f;

		// Token: 0x04001CB9 RID: 7353
		[SerializeField]
		private BiomeMaskArea m_biomeToCopyFrom;

		// Token: 0x04001CBA RID: 7354
		private bool _needInit;

		// Token: 0x04001CBB RID: 7355
		private Vector3 _lastPosition = Vector3.zero;

		// Token: 0x04001CBC RID: 7356
		private Quaternion _lastRotation = Quaternion.identity;

		// Token: 0x04001CBD RID: 7357
		private Vector3 _lastLossyScale = Vector3.zero;

		// Token: 0x04001CBE RID: 7358
		private MeshCollider m_meshCollider;

		// Token: 0x04001CBF RID: 7359
		private Mesh m_mesh;

		// Token: 0x04001CC0 RID: 7360
		private bool m_visEnabled;
	}
}
