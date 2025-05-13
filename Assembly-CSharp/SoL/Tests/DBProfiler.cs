using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D99 RID: 3481
	public class DBProfiler : MonoBehaviour
	{
		// Token: 0x06006884 RID: 26756 RVA: 0x00214B74 File Offset: 0x00212D74
		private void Update()
		{
			for (int i = 0; i < this.nCycles; i++)
			{
			}
		}

		// Token: 0x04005AC8 RID: 23240
		[SerializeField]
		private int nCycles = 1;
	}
}
