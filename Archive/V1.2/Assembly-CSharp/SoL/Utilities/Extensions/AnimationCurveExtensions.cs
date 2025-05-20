using System;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000321 RID: 801
	public static class AnimationCurveExtensions
	{
		// Token: 0x06001623 RID: 5667 RVA: 0x000FEB9C File Offset: 0x000FCD9C
		public static bool TryGetMinMaxValues(this AnimationCurve curve, out MinMaxAnimationCurveValues values)
		{
			values = default(MinMaxAnimationCurveValues);
			if (curve.keys.Length <= 1)
			{
				return false;
			}
			Keyframe keyframe = curve.keys[0];
			Keyframe keyframe2 = curve.keys[curve.keys.Length - 1];
			values.LevelMin = Mathf.FloorToInt(keyframe.time);
			values.ValueMin = Mathf.FloorToInt(keyframe.value);
			values.LevelMax = Mathf.FloorToInt(keyframe2.time);
			values.ValueMax = Mathf.FloorToInt(keyframe2.value);
			return true;
		}
	}
}
