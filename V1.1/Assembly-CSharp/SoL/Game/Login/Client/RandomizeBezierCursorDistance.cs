using System;
using BansheeGz.BGSpline.Components;
using UnityEngine;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B3D RID: 2877
	public class RandomizeBezierCursorDistance : MonoBehaviour
	{
		// Token: 0x170014B9 RID: 5305
		// (get) Token: 0x0600586A RID: 22634 RVA: 0x001E5E2C File Offset: 0x001E402C
		private static System.Random m_random
		{
			get
			{
				if (RandomizeBezierCursorDistance._random == null)
				{
					RandomizeBezierCursorDistance._random = new System.Random((int)DateTime.UtcNow.Ticks);
				}
				return RandomizeBezierCursorDistance._random;
			}
		}

		// Token: 0x0600586B RID: 22635 RVA: 0x001E5E60 File Offset: 0x001E4060
		private void Start()
		{
			if (this.m_cursor)
			{
				float distanceRatio = (float)RandomizeBezierCursorDistance.m_random.NextDouble();
				this.m_cursor.DistanceRatio = distanceRatio;
			}
		}

		// Token: 0x04004DCF RID: 19919
		[SerializeField]
		private BGCcCursor m_cursor;

		// Token: 0x04004DD0 RID: 19920
		private static System.Random _random;
	}
}
