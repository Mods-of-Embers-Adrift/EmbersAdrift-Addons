using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200036D RID: 877
	public static class ResizableWindowTriggerTypeExtensions
	{
		// Token: 0x060017FE RID: 6142 RVA: 0x00103458 File Offset: 0x00101658
		public static Vector2 GetPivot(this ResizableWindowTrigger.ResizableWindowTriggerType triggerType)
		{
			switch (triggerType)
			{
			case ResizableWindowTrigger.ResizableWindowTriggerType.Left:
				return new Vector2(1f, 0.5f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.Right:
				return new Vector2(0f, 0.5f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.Top:
				return new Vector2(0.5f, 0f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.Bottom:
				return new Vector2(0.5f, 1f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.LowerLeft:
				return new Vector2(1f, 1f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.LowerRight:
				return new Vector2(0f, 1f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.UpperLeft:
				return new Vector2(1f, 0f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.UpperRight:
				return new Vector2(0f, 0f);
			default:
				throw new ArgumentException("Unknown ResizableWindowTriggerType! " + triggerType.ToString());
			}
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x0010352C File Offset: 0x0010172C
		public static Vector2 GetSign(this ResizableWindowTrigger.ResizableWindowTriggerType triggerType)
		{
			switch (triggerType)
			{
			case ResizableWindowTrigger.ResizableWindowTriggerType.Left:
			case ResizableWindowTrigger.ResizableWindowTriggerType.Top:
			case ResizableWindowTrigger.ResizableWindowTriggerType.UpperLeft:
				return new Vector2(-1f, 1f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.Right:
			case ResizableWindowTrigger.ResizableWindowTriggerType.Bottom:
			case ResizableWindowTrigger.ResizableWindowTriggerType.LowerRight:
				return new Vector2(1f, -1f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.LowerLeft:
				return new Vector2(-1f, -1f);
			case ResizableWindowTrigger.ResizableWindowTriggerType.UpperRight:
				return new Vector2(1f, 1f);
			default:
				throw new ArgumentException("Unknown ResizableWindowTriggerType! " + triggerType.ToString());
			}
		}
	}
}
