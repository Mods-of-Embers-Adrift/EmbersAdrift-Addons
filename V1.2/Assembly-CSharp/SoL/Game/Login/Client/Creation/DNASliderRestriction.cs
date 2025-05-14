using System;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B5E RID: 2910
	[Serializable]
	public class DNASliderRestriction
	{
		// Token: 0x06005987 RID: 22919 RVA: 0x001EA370 File Offset: 0x001E8570
		public float GetNormalizedValue(float value)
		{
			if (value > 0.5f)
			{
				return Mathf.Lerp(0.5f, this.MinMax.y, 2f * (value - 0.5f));
			}
			return Mathf.Lerp(this.MinMax.x, 0.5f, 2f * value);
		}

		// Token: 0x06005988 RID: 22920 RVA: 0x0007BF8A File Offset: 0x0007A18A
		public float GetSliderValue(float value)
		{
			return Mathf.Lerp(0f, 1f, (value - this.MinMax.x) / (this.MinMax.y - this.MinMax.x));
		}

		// Token: 0x04004EBE RID: 20158
		public UMADnaType DnaType;

		// Token: 0x04004EBF RID: 20159
		public Vector2 MinMax;
	}
}
