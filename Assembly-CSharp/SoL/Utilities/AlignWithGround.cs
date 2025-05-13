using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace SoL.Utilities
{
	// Token: 0x0200023E RID: 574
	public class AlignWithGround : MonoBehaviour
	{
		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x0004F806 File Offset: 0x0004DA06
		private bool m_showLayerMask
		{
			get
			{
				return !this.m_terrainOnly;
			}
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0004F811 File Offset: 0x0004DA11
		public void PositionAndAlign()
		{
			this.DoAlignment(true, true);
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0004F81B File Offset: 0x0004DA1B
		public void Position()
		{
			this.DoAlignment(true, false);
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x0004F825 File Offset: 0x0004DA25
		public void Align()
		{
			this.DoAlignment(false, true);
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x000E883C File Offset: 0x000E6A3C
		public void RandomizeRotation()
		{
			base.gameObject.transform.RotateAround(base.gameObject.transform.position, base.gameObject.transform.up, UnityEngine.Random.Range(0f, 360f));
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x000E8888 File Offset: 0x000E6A88
		public void SnapToNavMesh()
		{
			NavMeshHit navMeshHit;
			if (NavMeshUtilities.SamplePosition(base.gameObject.transform.position, out navMeshHit, 10f, -1))
			{
				base.gameObject.transform.position = navMeshHit.position;
				return;
			}
			Debug.LogWarning("Unable to find a valid nav mesh for " + base.gameObject.name + "!");
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x000E88EC File Offset: 0x000E6AEC
		private void DoAlignment(bool position, bool rotation)
		{
			int layerMask = this.m_terrainOnly ? -1 : this.m_layerMask.value;
			RaycastHit[] hits = Hits.Hits25;
			int num = Physics.RaycastNonAlloc(base.gameObject.transform.position + Vector3.up * 0.2f, Vector3.down, hits, 10f, layerMask);
			for (int i = 0; i < num; i++)
			{
				if (!this.m_terrainOnly || hits[i].collider is TerrainCollider)
				{
					if (position)
					{
						base.gameObject.transform.position = hits[i].point + this.m_positionOffset;
					}
					if (rotation)
					{
						base.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hits[i].normal) * this.m_rotationOffset;
					}
					return;
				}
			}
			string str = this.m_terrainOnly ? "Terrain" : "Collider";
			Debug.LogWarning("No " + str + "!");
		}

		// Token: 0x040010D1 RID: 4305
		private const string kAdjustmentGroup = "Adjustment";

		// Token: 0x040010D2 RID: 4306
		private const string kButtonGroupName = "Adjustment/Actions";

		// Token: 0x040010D3 RID: 4307
		[SerializeField]
		private Vector3 m_positionOffset = Vector3.zero;

		// Token: 0x040010D4 RID: 4308
		[FormerlySerializedAs("m_rotationoffset")]
		[SerializeField]
		private Quaternion m_rotationOffset = Quaternion.identity;

		// Token: 0x040010D5 RID: 4309
		[SerializeField]
		private bool m_terrainOnly;

		// Token: 0x040010D6 RID: 4310
		[SerializeField]
		private LayerMask m_layerMask = 1;
	}
}
