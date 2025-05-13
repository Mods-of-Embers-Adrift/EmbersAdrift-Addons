using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DAB RID: 3499
	public class JaggedArray : MonoBehaviour
	{
		// Token: 0x060068CD RID: 26829 RVA: 0x00215A9C File Offset: 0x00213C9C
		private void JaggedArrayIndexes()
		{
			int num = (int)this.m_size.x;
			int num2 = (int)this.m_size.y;
			int num3 = (int)this.m_size.z;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					for (int k = 0; k < num3; k++)
					{
						int num4 = i * num2 * num3 + j * num3 + k;
						Debug.Log(string.Format("[{0}, {1}, {2}] --> {3}", new object[]
						{
							i,
							j,
							k,
							num4
						}));
					}
				}
			}
		}

		// Token: 0x04005B34 RID: 23348
		[SerializeField]
		private Vector3 m_size = Vector3.zero;
	}
}
