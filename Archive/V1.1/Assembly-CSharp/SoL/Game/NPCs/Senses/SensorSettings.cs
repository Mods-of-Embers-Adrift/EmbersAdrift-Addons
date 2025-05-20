using System;

namespace SoL.Game.NPCs.Senses
{
	// Token: 0x0200082F RID: 2095
	[Serializable]
	public struct SensorSettings
	{
		// Token: 0x06003CBC RID: 15548 RVA: 0x00069251 File Offset: 0x00067451
		public bool WithinDistanceAndArc(float angle, float sqrDistance)
		{
			return this.WithinArc(angle) && this.WithinDistance(sqrDistance);
		}

		// Token: 0x06003CBD RID: 15549 RVA: 0x00069265 File Offset: 0x00067465
		public bool WithinDistance(float sqrDistance)
		{
			return sqrDistance <= this.Distance * this.Distance;
		}

		// Token: 0x06003CBE RID: 15550 RVA: 0x0006927A File Offset: 0x0006747A
		public bool WithinArc(float angle)
		{
			return angle <= this.Angle * 0.5f;
		}

		// Token: 0x04003B94 RID: 15252
		public float Distance;

		// Token: 0x04003B95 RID: 15253
		public float Angle;

		// Token: 0x04003B96 RID: 15254
		public float Threshold;
	}
}
