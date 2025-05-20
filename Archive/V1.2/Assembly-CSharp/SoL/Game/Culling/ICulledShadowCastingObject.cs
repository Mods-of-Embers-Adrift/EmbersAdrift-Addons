using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CCB RID: 3275
	public interface ICulledShadowCastingObject
	{
		// Token: 0x170017B0 RID: 6064
		// (get) Token: 0x0600633E RID: 25406
		bool ObjectCastsShadows { get; }

		// Token: 0x0600633F RID: 25407
		void SetCullingDistance(CullingDistance cullingDistance, CullingFlags flags);

		// Token: 0x06006340 RID: 25408
		void SetAtlasResolution(float value);

		// Token: 0x06006341 RID: 25409
		void ToggleShadows(bool isEnabled);
	}
}
