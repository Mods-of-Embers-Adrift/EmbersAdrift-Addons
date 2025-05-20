using System;
using SoL.Game.Culling;
using TheVisualEngine;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005DB RID: 1499
	public class TVETrailGenerator : GameEntityComponent, ITrailGenerator
	{
		// Token: 0x06002F95 RID: 12181 RVA: 0x00060D1C File Offset: 0x0005EF1C
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.TrailGenerator = this;
			}
		}

		// Token: 0x06002F96 RID: 12182 RVA: 0x00060D6A File Offset: 0x0005EF6A
		private void Start()
		{
			this.m_allowEnable = (TVEManager.Instance != null);
			this.RefreshTrail();
		}

		// Token: 0x06002F97 RID: 12183 RVA: 0x00157720 File Offset: 0x00155920
		private void RefreshTrail()
		{
			bool emitting = this.m_allowEnable && this.m_cullingFlags == CullingFlags.None;
			this.m_trailRenderer.emitting = emitting;
		}

		// Token: 0x06002F98 RID: 12184 RVA: 0x00157750 File Offset: 0x00155950
		void ITrailGenerator.SetCullingDistance(CullingDistance cullingDistance)
		{
			this.m_cullingFlags = ((cullingDistance.GetDistance() <= CullingDistance.VeryNear.GetDistance()) ? this.m_cullingFlags.UnsetBitFlag(CullingFlags.Distance) : this.m_cullingFlags.SetBitFlag(CullingFlags.Distance));
			this.RefreshTrail();
		}

		// Token: 0x04002E9C RID: 11932
		[SerializeField]
		private TrailRenderer m_trailRenderer;

		// Token: 0x04002E9D RID: 11933
		private bool m_allowEnable;

		// Token: 0x04002E9E RID: 11934
		private CullingFlags m_cullingFlags;
	}
}
