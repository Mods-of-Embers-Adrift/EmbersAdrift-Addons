using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200032B RID: 811
	public struct ColorComparer : IEqualityComparer<Color>
	{
		// Token: 0x06001650 RID: 5712 RVA: 0x00051937 File Offset: 0x0004FB37
		public bool Equals(Color x, Color y)
		{
			return x.Equals(y);
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x00051941 File Offset: 0x0004FB41
		public int GetHashCode(Color obj)
		{
			return obj.GetHashCode();
		}
	}
}
