using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000251 RID: 593
	[CreateAssetMenu(menuName = "SoL/Collections/Color Collection")]
	public class ColorCollection : ScriptableObject
	{
		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06001346 RID: 4934 RVA: 0x0004F992 File Offset: 0x0004DB92
		public Color[] Colors
		{
			get
			{
				return this.m_colors;
			}
		}

		// Token: 0x04001100 RID: 4352
		[SerializeField]
		private Color[] m_colors;

		// Token: 0x04001101 RID: 4353
		[SerializeField]
		private TextAsset m_colorsToLoad;
	}
}
