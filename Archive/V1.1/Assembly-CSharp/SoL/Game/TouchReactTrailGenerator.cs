using System;
using AwesomeTechnologies.TouchReact;
using AwesomeTechnologies.VegetationStudio;
using SoL.Game.Culling;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005D9 RID: 1497
	public class TouchReactTrailGenerator : GameEntityComponent, ITrailGenerator
	{
		// Token: 0x06002F8B RID: 12171 RVA: 0x00060D1C File Offset: 0x0005EF1C
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.TrailGenerator = this;
			}
		}

		// Token: 0x06002F8C RID: 12172 RVA: 0x00060D38 File Offset: 0x0005EF38
		private void Start()
		{
			this.m_allowEnable = (VegetationStudioManager.Instance != null);
			this.RefreshTrail();
		}

		// Token: 0x06002F8D RID: 12173 RVA: 0x00157624 File Offset: 0x00155824
		public void SetCullingDistance(CullingDistance cullingDistance)
		{
			this.m_cullingFlags = ((cullingDistance.GetDistance() <= CullingDistance.VeryNear.GetDistance()) ? this.m_cullingFlags.UnsetBitFlag(CullingFlags.Distance) : this.m_cullingFlags.SetBitFlag(CullingFlags.Distance));
			this.RefreshTrail();
		}

		// Token: 0x06002F8E RID: 12174 RVA: 0x0015766C File Offset: 0x0015586C
		private void RefreshTrail()
		{
			bool flag = this.m_allowEnable && this.m_cullingFlags == CullingFlags.None;
			this.m_touchReactMesh.enabled = flag;
			this.m_trailRenderer.emitting = flag;
		}

		// Token: 0x04002E96 RID: 11926
		[SerializeField]
		private TrailRenderer m_trailRenderer;

		// Token: 0x04002E97 RID: 11927
		[SerializeField]
		private TouchReactMesh m_touchReactMesh;

		// Token: 0x04002E98 RID: 11928
		private bool m_allowEnable;

		// Token: 0x04002E99 RID: 11929
		private CullingFlags m_cullingFlags;
	}
}
