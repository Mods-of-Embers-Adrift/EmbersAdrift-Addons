using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DA3 RID: 3491
	public class GaussianTest : MonoBehaviour
	{
		// Token: 0x060068A7 RID: 26791 RVA: 0x00215430 File Offset: 0x00213630
		private void RunIt()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < this.m_calls; i++)
			{
				float num4 = SolMath.Gaussian();
				if (num4 < num)
				{
					num = num4;
				}
				if (num4 > num2)
				{
					num2 = num4;
				}
				num3 += num4;
			}
			float num5 = num3 / (float)this.m_calls;
			Debug.Log(string.Format("{0}->{1} average {2}", num, num2, num5));
		}

		// Token: 0x060068A8 RID: 26792 RVA: 0x002154AC File Offset: 0x002136AC
		private void HitRoll()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < this.m_calls; i++)
			{
				float num4 = SolMath.Gaussian() + 1f * this.m_bonus;
				if (num4 < num)
				{
					num = num4;
				}
				if (num4 > num2)
				{
					num2 = num4;
				}
				num3 += num4;
			}
			float num5 = num3 / (float)this.m_calls;
			Debug.Log(string.Format("{0}->{1} average {2}", num, num2, num5));
		}

		// Token: 0x04005AF7 RID: 23287
		[SerializeField]
		private int m_calls = 10;

		// Token: 0x04005AF8 RID: 23288
		[Range(0f, 1f)]
		[SerializeField]
		private float m_bonus;
	}
}
