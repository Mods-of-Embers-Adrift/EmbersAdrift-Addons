using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000269 RID: 617
	[CreateAssetMenu(menuName = "SoL/Level Curve")]
	public class ScriptableLevelCurve : ScriptableObject
	{
		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x0600138D RID: 5005 RVA: 0x0004FC7C File Offset: 0x0004DE7C
		public ParameterizedLevelCurve Curve
		{
			get
			{
				return this.m_curve;
			}
		}

		// Token: 0x04001BE2 RID: 7138
		[SerializeField]
		private ParameterizedLevelCurve m_curve;
	}
}
