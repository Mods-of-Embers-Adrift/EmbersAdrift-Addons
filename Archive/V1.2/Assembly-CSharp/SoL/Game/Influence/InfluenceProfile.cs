using System;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BC3 RID: 3011
	[CreateAssetMenu(menuName = "SoL/Profiles/Influence")]
	public class InfluenceProfile : ScriptableObject
	{
		// Token: 0x06005D21 RID: 23841 RVA: 0x001F2D04 File Offset: 0x001F0F04
		public float SampleInfluenceAtPointFromSource(Vector3 sourcePosition, Vector3 samplePosition, InfluenceFlags flags)
		{
			float num = 0f;
			if (this.m_influenceSettings == null || this.m_influenceSettings.Length == 0)
			{
				return num;
			}
			float sqrMagnitude = (samplePosition - sourcePosition).sqrMagnitude;
			for (int i = 0; i < this.m_influenceSettings.Length; i++)
			{
				if (flags.HasBitFlag(this.m_influenceSettings[i].Flags))
				{
					float num2 = this.m_influenceSettings[i].Radius * this.m_influenceSettings[i].Radius;
					if (sqrMagnitude <= num2)
					{
						float num3 = sqrMagnitude / num2;
						if (num3 > 0f && num3 <= 1f)
						{
							num += Mathf.Clamp01(this.m_influenceSettings[i].Curve.Evaluate(num3));
						}
					}
				}
			}
			return num;
		}

		// Token: 0x0400508D RID: 20621
		[SerializeField]
		private InfluenceProfile.InfluenceSettings[] m_influenceSettings;

		// Token: 0x02000BC4 RID: 3012
		[Serializable]
		private class InfluenceSettings
		{
			// Token: 0x0400508E RID: 20622
			public InfluenceFlags Flags;

			// Token: 0x0400508F RID: 20623
			public AnimationCurve Curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			// Token: 0x04005090 RID: 20624
			public float Radius = 5f;
		}
	}
}
