using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009FB RID: 2555
	[CreateAssetMenu(menuName = "SoL/Profiles/MinMaxFloatRange")]
	public class MinMaxFloatRangeScriptable : ScriptableObject
	{
		// Token: 0x17001122 RID: 4386
		// (get) Token: 0x06004D9D RID: 19869 RVA: 0x00074761 File Offset: 0x00072961
		public MinMaxFloatRange Range
		{
			get
			{
				return this.m_range;
			}
		}

		// Token: 0x04004735 RID: 18229
		[SerializeField]
		private MinMaxFloatRange m_range = new MinMaxFloatRange(0f, 1f);
	}
}
