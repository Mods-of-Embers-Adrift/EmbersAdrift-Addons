using System;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007DF RID: 2015
	public static class PooledObjectExtensions
	{
		// Token: 0x06003ABB RID: 15035 RVA: 0x00067CF7 File Offset: 0x00065EF7
		public static void ReturnToPool(this PooledObject obj)
		{
			if (obj.Pool)
			{
				obj.Pool.ReturnToPool(obj);
				return;
			}
			if (obj && obj.gameObject)
			{
				UnityEngine.Object.Destroy(obj.gameObject);
			}
		}
	}
}
