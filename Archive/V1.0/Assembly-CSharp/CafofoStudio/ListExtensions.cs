using System;
using System.Collections.Generic;
using UnityEngine;

namespace CafofoStudio
{
	// Token: 0x02000053 RID: 83
	public static class ListExtensions
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x0009A5D4 File Offset: 0x000987D4
		public static void Shuffle<T>(this List<T> list)
		{
			int count = list.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int index = UnityEngine.Random.Range(i, count);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
		}
	}
}
