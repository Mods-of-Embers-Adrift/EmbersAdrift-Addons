using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200028B RID: 651
	public class GridChildren : MonoBehaviour
	{
		// Token: 0x060013FB RID: 5115 RVA: 0x000F89C8 File Offset: 0x000F6BC8
		private void DoIt()
		{
			List<Transform> fromPool = StaticListPool<Transform>.GetFromPool();
			foreach (object obj in base.gameObject.transform)
			{
				Transform item = (Transform)obj;
				fromPool.Add(item);
			}
			float num = 0f;
			int num2 = this.m_x;
			for (int i = 0; i < fromPool.Count; i++)
			{
				float x = (float)(i % this.m_x) * this.m_spacing;
				fromPool[i].localPosition = new Vector3(x, 0f, num);
				if (i >= num2 - 1)
				{
					num += this.m_spacing;
					num2 += this.m_x;
				}
			}
			StaticListPool<Transform>.ReturnToPool(fromPool);
		}

		// Token: 0x04001C42 RID: 7234
		[SerializeField]
		private int m_x = 10;

		// Token: 0x04001C43 RID: 7235
		[SerializeField]
		private float m_spacing = 2f;
	}
}
