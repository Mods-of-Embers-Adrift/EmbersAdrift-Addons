using System;
using SoL.Game.Culling;
using TheVisualEngine;

namespace SoL.Game
{
	// Token: 0x020005DA RID: 1498
	public class TVEMotion : GameEntityComponent, ITrailGenerator
	{
		// Token: 0x06002F90 RID: 12176 RVA: 0x00060D1C File Offset: 0x0005EF1C
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.TrailGenerator = this;
			}
		}

		// Token: 0x06002F91 RID: 12177 RVA: 0x00060D51 File Offset: 0x0005EF51
		private void Start()
		{
			this.m_allowEnable = (TVEManager.Instance != null);
			this.RefreshMotion();
		}

		// Token: 0x06002F92 RID: 12178 RVA: 0x001576C8 File Offset: 0x001558C8
		private void RefreshMotion()
		{
			bool active = this.m_allowEnable && this.m_cullingFlags == CullingFlags.None;
			base.gameObject.SetActive(active);
		}

		// Token: 0x06002F93 RID: 12179 RVA: 0x001576F8 File Offset: 0x001558F8
		void ITrailGenerator.SetCullingDistance(CullingDistance cullingDistance)
		{
			this.m_cullingFlags = ((cullingDistance.GetDistance() <= CullingDistance.VeryNear.GetDistance()) ? this.m_cullingFlags.UnsetBitFlag(CullingFlags.Distance) : this.m_cullingFlags.SetBitFlag(CullingFlags.Distance));
			this.RefreshMotion();
		}

		// Token: 0x04002E9A RID: 11930
		private bool m_allowEnable;

		// Token: 0x04002E9B RID: 11931
		private CullingFlags m_cullingFlags;
	}
}
