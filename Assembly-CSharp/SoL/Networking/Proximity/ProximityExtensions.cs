using System;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004B0 RID: 1200
	public static class ProximityExtensions
	{
		// Token: 0x06002179 RID: 8569 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static SensorBand SetFlag(this SensorBand a, SensorBand b)
		{
			return a | b;
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x000578BA File Offset: 0x00055ABA
		public static SensorBand UnsetFlag(this SensorBand a, SensorBand b)
		{
			return a & ~b;
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this SensorBand a, SensorBand b)
		{
			return (a & b) == b;
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x00123F28 File Offset: 0x00122128
		public static float GetUpdateTime(this SensorBand band)
		{
			switch (band)
			{
			case SensorBand.A:
				return 0.1f;
			case SensorBand.B:
				return 0.2f;
			case SensorBand.A | SensorBand.B:
				break;
			case SensorBand.C:
				return 0.5f;
			default:
				if (band == SensorBand.D)
				{
					return 1f;
				}
				if (band == SensorBand.E)
				{
					return 2f;
				}
				break;
			}
			throw new ArgumentException("no time set for band " + band.ToString() + "!");
		}
	}
}
