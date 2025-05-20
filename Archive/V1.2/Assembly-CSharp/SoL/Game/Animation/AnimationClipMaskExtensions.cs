using System;
using System.Collections.Generic;

namespace SoL.Game.Animation
{
	// Token: 0x02000D6F RID: 3439
	public static class AnimationClipMaskExtensions
	{
		// Token: 0x06006789 RID: 26505 RVA: 0x002131F0 File Offset: 0x002113F0
		public static string GetMaskName(this AnimationClipMask mask)
		{
			if (AnimationClipMaskExtensions.m_clipMaskToStringDict == null)
			{
				AnimationClipMaskExtensions.m_clipMaskToStringDict = new Dictionary<AnimationClipMask, string>(default(AnimationClipMaskComparer));
				AnimationClipMask[] array = (AnimationClipMask[])Enum.GetValues(typeof(AnimationClipMask));
				for (int i = 0; i < array.Length; i++)
				{
					AnimationClipMaskExtensions.m_clipMaskToStringDict.Add(array[i], array[i].ToString());
				}
			}
			string result;
			AnimationClipMaskExtensions.m_clipMaskToStringDict.TryGetValue(mask, out result);
			return result;
		}

		// Token: 0x0600678A RID: 26506 RVA: 0x00085941 File Offset: 0x00083B41
		public static bool AddedByDefault(this AnimationClipMask mask)
		{
			return mask == AnimationClipMask.None || mask - AnimationClipMask.FullBodyOverride <= 1 || mask - AnimationClipMask.GetHit <= 2;
		}

		// Token: 0x04005A0A RID: 23050
		private static Dictionary<AnimationClipMask, string> m_clipMaskToStringDict;
	}
}
