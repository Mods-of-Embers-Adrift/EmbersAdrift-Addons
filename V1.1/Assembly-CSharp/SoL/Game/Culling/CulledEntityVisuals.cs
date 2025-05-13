using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SoL.Game.Culling
{
	// Token: 0x02000CBF RID: 3263
	public class CulledEntityVisuals : GameEntityComponent, ICulledShadowCastingObject
	{
		// Token: 0x1700179F RID: 6047
		// (get) Token: 0x060062E1 RID: 25313 RVA: 0x00082A0D File Offset: 0x00080C0D
		public bool ObjectCastsShadows
		{
			get
			{
				return this.m_anyRenderersCastShadows;
			}
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x00205D8C File Offset: 0x00203F8C
		private void Awake()
		{
			if (base.GameEntity != null && base.GameEntity.EntityVisuals == null)
			{
				base.GameEntity.EntityVisuals = this;
			}
			if (this.m_renderers != null)
			{
				for (int i = 0; i < this.m_renderers.Length; i++)
				{
					if (this.m_renderers[i].ObjectCastsShadows)
					{
						this.m_anyRenderersCastShadows = true;
						return;
					}
				}
			}
		}

		// Token: 0x060062E3 RID: 25315 RVA: 0x00205DF4 File Offset: 0x00203FF4
		public void SetCullingDistance(CullingDistance cullingDistance, CullingFlags flags)
		{
			bool isEnabled = cullingDistance.GetDistance() <= CullingDistance.VeryNear.GetDistance();
			this.ToggleShadows(isEnabled);
		}

		// Token: 0x060062E4 RID: 25316 RVA: 0x0004475B File Offset: 0x0004295B
		public void SetAtlasResolution(float value)
		{
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x00205E1C File Offset: 0x0020401C
		public void ToggleShadows(bool isEnabled)
		{
			if (this.m_renderers != null)
			{
				for (int i = 0; i < this.m_renderers.Length; i++)
				{
					this.m_renderers[i].ToggleShadows(isEnabled);
				}
			}
		}

		// Token: 0x04005627 RID: 22055
		[SerializeField]
		private CulledEntityVisuals.RendererShadowSettings[] m_renderers;

		// Token: 0x04005628 RID: 22056
		private bool m_anyRenderersCastShadows;

		// Token: 0x02000CC0 RID: 3264
		[Serializable]
		private class RendererShadowSettings
		{
			// Token: 0x170017A0 RID: 6048
			// (get) Token: 0x060062E7 RID: 25319 RVA: 0x00082A15 File Offset: 0x00080C15
			public bool ObjectCastsShadows
			{
				get
				{
					return this.m_defaultShadowCastingMode > ShadowCastingMode.Off;
				}
			}

			// Token: 0x060062E8 RID: 25320 RVA: 0x00082A20 File Offset: 0x00080C20
			private void UpdateInternalShadows()
			{
				this.m_defaultShadowCastingMode = ((this.m_renderer == null) ? ShadowCastingMode.Off : this.m_renderer.shadowCastingMode);
			}

			// Token: 0x060062E9 RID: 25321 RVA: 0x00082A44 File Offset: 0x00080C44
			public void ToggleShadows(bool isEnabled)
			{
				if (this.m_renderer != null)
				{
					this.m_renderer.shadowCastingMode = (isEnabled ? this.m_defaultShadowCastingMode : ShadowCastingMode.Off);
				}
			}

			// Token: 0x04005629 RID: 22057
			[SerializeField]
			private Renderer m_renderer;

			// Token: 0x0400562A RID: 22058
			[SerializeField]
			private ShadowCastingMode m_defaultShadowCastingMode;
		}
	}
}
