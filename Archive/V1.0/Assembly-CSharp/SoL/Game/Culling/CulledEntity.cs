using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CBE RID: 3262
	public class CulledEntity : CulledObject
	{
		// Token: 0x1700179A RID: 6042
		// (get) Token: 0x060062D4 RID: 25300 RVA: 0x00082971 File Offset: 0x00080B71
		public CullingFlags LimitFlags
		{
			get
			{
				return this.m_limitFlags;
			}
		}

		// Token: 0x1700179B RID: 6043
		// (get) Token: 0x060062D5 RID: 25301 RVA: 0x00082979 File Offset: 0x00080B79
		public CullingDistance CurrentBand
		{
			get
			{
				return this.m_currentBand;
			}
		}

		// Token: 0x1700179C RID: 6044
		// (get) Token: 0x060062D6 RID: 25302 RVA: 0x00082981 File Offset: 0x00080B81
		public CullingFlags CullingFlags
		{
			get
			{
				return this.m_cullingFlags;
			}
		}

		// Token: 0x1700179D RID: 6045
		// (get) Token: 0x060062D7 RID: 25303 RVA: 0x00082989 File Offset: 0x00080B89
		public int? Index
		{
			get
			{
				return base.InternalIndex;
			}
		}

		// Token: 0x1700179E RID: 6046
		// (get) Token: 0x060062D8 RID: 25304 RVA: 0x00082991 File Offset: 0x00080B91
		public bool Initialized
		{
			get
			{
				return base.InternalInitialized;
			}
		}

		// Token: 0x060062D9 RID: 25305 RVA: 0x00082999 File Offset: 0x00080B99
		protected void Awake()
		{
			this.m_gameEntity = base.gameObject.transform.parent.GetComponent<GameEntity>();
			if (this.m_gameEntity != null)
			{
				this.m_gameEntity.CulledEntity = this;
			}
		}

		// Token: 0x060062DA RID: 25306 RVA: 0x000829D0 File Offset: 0x00080BD0
		private void Start()
		{
			this.RefreshLimitFlags();
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x00205BCC File Offset: 0x00203DCC
		public void RefreshLimitFlags()
		{
			if (!this.m_gameEntity)
			{
				return;
			}
			if (this.m_gameEntity.DCAController)
			{
				this.m_limitFlags |= CullingFlags.UmaFeatureLimit;
			}
			else if (this.m_gameEntity.EntityVisuals != null && this.m_gameEntity.EntityVisuals.ObjectCastsShadows)
			{
				this.m_limitFlags |= CullingFlags.ObjectShadowLimit;
			}
			if (this.m_gameEntity.IKController)
			{
				this.m_limitFlags |= CullingFlags.IKLimit;
			}
		}

		// Token: 0x060062DC RID: 25308 RVA: 0x00205C5C File Offset: 0x00203E5C
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (!this.m_gameEntity)
			{
				return;
			}
			ICulledShadowCastingObject entityVisuals = this.m_gameEntity.EntityVisuals;
			if (entityVisuals != null)
			{
				entityVisuals.SetCullingDistance(this.m_currentBand, this.m_cullingFlags);
			}
			if (this.m_gameEntity.IKController)
			{
				this.m_gameEntity.IKController.RefreshCullee(!this.m_currentBand.IsLowestBand() || base.IsCulled() || this.m_cullingFlags.HasBitFlag(CullingFlags.IKLimit), this.m_sqrMagnitudeDistance);
			}
			ITrailGenerator trailGenerator = this.m_gameEntity.TrailGenerator;
			if (trailGenerator != null)
			{
				trailGenerator.SetCullingDistance(this.m_currentBand);
			}
			if (this.m_gameEntity.AudioEventController)
			{
				this.m_gameEntity.AudioEventController.SetCullingDistance(this.m_currentBand);
			}
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x000829D8 File Offset: 0x00080BD8
		public void SetAtlasScale(float value)
		{
			if (this.m_gameEntity && this.m_gameEntity.EntityVisuals != null)
			{
				this.m_gameEntity.EntityVisuals.SetAtlasResolution(value);
			}
		}

		// Token: 0x060062DE RID: 25310 RVA: 0x00082A05 File Offset: 0x00080C05
		public void RefreshCulling()
		{
			this.RefreshCullee();
		}

		// Token: 0x060062DF RID: 25311 RVA: 0x00205D30 File Offset: 0x00203F30
		public void RefreshTargetCulling()
		{
			if (CullingManager.Instance == null || !this.Initialized || this.Index == null)
			{
				return;
			}
			int distanceBand = CullingManager.Instance.GetDistanceBand(this.Index.Value);
			((ICullee)this).OnDistanceBandChanged((int)this.CurrentBand, distanceBand, true);
		}

		// Token: 0x04005626 RID: 22054
		private GameEntity m_gameEntity;
	}
}
