using System;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CCF RID: 3279
	public static class FlickerTypeExt
	{
		// Token: 0x06006353 RID: 25427 RVA: 0x00082DDE File Offset: 0x00080FDE
		public static float GetFlickerSpeed(this FlickerType ft)
		{
			switch (ft)
			{
			case FlickerType.Candle:
				return 0.5f;
			case FlickerType.Torch:
				return 0.75f;
			case FlickerType.Lantern:
				return 0.75f;
			default:
				return 1f;
			}
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x00082E0D File Offset: 0x0008100D
		public static float GetFlickerRangeDelta(this FlickerType ft)
		{
			switch (ft)
			{
			case FlickerType.Candle:
				return 0.1f;
			case FlickerType.Torch:
				return 0.75f;
			case FlickerType.Lantern:
				return 0.5f;
			default:
				return 0f;
			}
		}

		// Token: 0x06006355 RID: 25429 RVA: 0x00082E3C File Offset: 0x0008103C
		public static float GetFlickerIntensityDelta(this FlickerType ft)
		{
			switch (ft)
			{
			case FlickerType.Candle:
				return 0.25f;
			case FlickerType.Torch:
				return 0.75f;
			case FlickerType.Lantern:
				return 0.5f;
			default:
				return 0f;
			}
		}

		// Token: 0x06006356 RID: 25430 RVA: 0x00206AA4 File Offset: 0x00204CA4
		public static Vector3 GetFlickerMoveDistance(this FlickerType ft)
		{
			float d = 0f;
			switch (ft)
			{
			case FlickerType.Candle:
				d = 0.05f;
				break;
			case FlickerType.Torch:
				d = 0.1f;
				break;
			case FlickerType.Lantern:
				d = 0f;
				break;
			}
			return Vector3.one * d;
		}
	}
}
