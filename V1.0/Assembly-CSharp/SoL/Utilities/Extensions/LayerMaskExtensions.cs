using System;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000333 RID: 819
	public static class LayerMaskExtensions
	{
		// Token: 0x06001665 RID: 5733 RVA: 0x00051A1B File Offset: 0x0004FC1B
		public static bool Contains(this LayerMask mask, int layer)
		{
			return mask == (mask | 1 << layer);
		}
	}
}
