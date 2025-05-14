using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002D5 RID: 725
	public static class MaterialPropertyBlockCache
	{
		// Token: 0x060014F3 RID: 5363 RVA: 0x000FBC84 File Offset: 0x000F9E84
		static MaterialPropertyBlockCache()
		{
			SceneCompositionManager.LoadingStartupScene += MaterialPropertyBlockCache.SceneCompositionManagerOnLoadingStartupScene;
			SceneCompositionManager.ZoneLoadStarted += MaterialPropertyBlockCache.SceneCompositionManagerOnZoneLoadStarted;
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x00050A17 File Offset: 0x0004EC17
		private static void SceneCompositionManagerOnLoadingStartupScene()
		{
			MaterialPropertyBlockCache.ClearMaterialPropertyBlocks();
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x00050A17 File Offset: 0x0004EC17
		private static void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			MaterialPropertyBlockCache.ClearMaterialPropertyBlocks();
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x00050A1E File Offset: 0x0004EC1E
		private static void ClearMaterialPropertyBlocks()
		{
			MaterialPropertyBlockCache.m_materialPropertyBlocks.Clear();
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x000FBCCC File Offset: 0x000F9ECC
		public static MaterialPropertyBlock GetMaterialPropertyBlock(Renderer renderer)
		{
			MaterialPropertyBlock materialPropertyBlock;
			if (renderer && MaterialPropertyBlockCache.m_materialPropertyBlocks.TryGetValue(renderer, out materialPropertyBlock))
			{
				return materialPropertyBlock;
			}
			materialPropertyBlock = new MaterialPropertyBlock();
			if (renderer)
			{
				MaterialPropertyBlockCache.m_materialPropertyBlocks.Add(renderer, materialPropertyBlock);
			}
			return materialPropertyBlock;
		}

		// Token: 0x04001D41 RID: 7489
		private static readonly Dictionary<Renderer, MaterialPropertyBlock> m_materialPropertyBlocks = new Dictionary<Renderer, MaterialPropertyBlock>(default(MaterialPropertyBlockCache.RendererComparer));

		// Token: 0x020002D6 RID: 726
		private struct RendererComparer : IEqualityComparer<Renderer>
		{
			// Token: 0x060014F8 RID: 5368 RVA: 0x00050A2A File Offset: 0x0004EC2A
			public bool Equals(Renderer x, Renderer y)
			{
				return x == y || (x != null && y != null && !(x.GetType() != y.GetType()) && x.GetHashCode() == y.GetHashCode());
			}

			// Token: 0x060014F9 RID: 5369 RVA: 0x00050A5F File Offset: 0x0004EC5F
			public int GetHashCode(Renderer obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}
