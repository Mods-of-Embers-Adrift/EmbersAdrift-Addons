using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CD2 RID: 3282
	public static class TreeImposterCullingDistanceExt
	{
		// Token: 0x170017BD RID: 6077
		// (get) Token: 0x06006366 RID: 25446 RVA: 0x00082F8E File Offset: 0x0008118E
		public static TreeImposterCullingDistance[] TreeImposterCullingDistances
		{
			get
			{
				if (TreeImposterCullingDistanceExt.m_treeImposterCullingDistances == null)
				{
					TreeImposterCullingDistanceExt.m_treeImposterCullingDistances = (TreeImposterCullingDistance[])Enum.GetValues(typeof(TreeImposterCullingDistance));
				}
				return TreeImposterCullingDistanceExt.m_treeImposterCullingDistances;
			}
		}

		// Token: 0x06006367 RID: 25447 RVA: 0x00206B88 File Offset: 0x00204D88
		public static float GetDistance(this TreeImposterCullingDistance d)
		{
			switch (d)
			{
			case TreeImposterCullingDistance.Low:
				return 100f;
			case TreeImposterCullingDistance.High:
				return 400f;
			case TreeImposterCullingDistance.Ultra:
				return 1000f;
			case TreeImposterCullingDistance.FarCull:
				return 3000f;
			case TreeImposterCullingDistance.NoCull:
				return 10000f;
			}
			return 250f;
		}

		// Token: 0x0400567C RID: 22140
		private static TreeImposterCullingDistance[] m_treeImposterCullingDistances;
	}
}
