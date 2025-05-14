using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x0200074F RID: 1871
	public interface ISceneComposition
	{
		// Token: 0x17000CB7 RID: 3255
		// (get) Token: 0x060037D0 RID: 14288
		SceneReference ActiveScene { get; }

		// Token: 0x17000CB8 RID: 3256
		// (get) Token: 0x060037D1 RID: 14289
		SceneReference ImposterScene { get; }

		// Token: 0x060037D2 RID: 14290
		IEnumerable<SceneReference> GetAdditionalScenes(SceneInclusionFlags flags, bool beforeActive);

		// Token: 0x17000CB9 RID: 3257
		// (get) Token: 0x060037D3 RID: 14291
		bool HasZoneSettings { get; }

		// Token: 0x17000CBA RID: 3258
		// (get) Token: 0x060037D4 RID: 14292
		ZoneId ZoneId { get; }

		// Token: 0x060037D5 RID: 14293
		Sprite GetRandomLoadingImage();

		// Token: 0x060037D6 RID: 14294
		string GetLoadingTip();

		// Token: 0x060037D7 RID: 14295
		string GetSceneLoadCompleteMessage();
	}
}
