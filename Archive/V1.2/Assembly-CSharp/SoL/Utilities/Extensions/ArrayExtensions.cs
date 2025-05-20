using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000325 RID: 805
	public static class ArrayExtensions
	{
		// Token: 0x0600163F RID: 5695 RVA: 0x000FF66C File Offset: 0x000FD86C
		public static void Shuffle<T>(this IList<T> ts)
		{
			int count = ts.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int index = UnityEngine.Random.Range(i, count);
				T value = ts[i];
				ts[i] = ts[index];
				ts[index] = value;
			}
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x000FF6B8 File Offset: 0x000FD8B8
		public static void Shuffle<T>(this T[] ts)
		{
			int num = ts.Length;
			int num2 = num - 1;
			for (int i = 0; i < num2; i++)
			{
				int num3 = UnityEngine.Random.Range(i, num);
				T t = ts[i];
				ts[i] = ts[num3];
				ts[num3] = t;
			}
		}
	}
}
