using System;
using BansheeGz.BGSpline.Components;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D16 RID: 3350
	public class SplineAudioAdjustments : MonoBehaviour
	{
		// Token: 0x060064FA RID: 25850 RVA: 0x00083FA2 File Offset: 0x000821A2
		private void Start()
		{
			if (!this.m_source || !this.m_cursor)
			{
				base.enabled = false;
			}
		}

		// Token: 0x060064FB RID: 25851 RVA: 0x0020B49C File Offset: 0x0020969C
		private void LateUpdate()
		{
			if (this.m_source && this.m_cursor)
			{
				float a;
				float b;
				float adjacentFieldValues = this.m_cursor.GetAdjacentFieldValues("MaxRange", out a, out b);
				this.m_source.maxDistance = Mathf.Clamp(Mathf.Lerp(a, b, adjacentFieldValues), this.m_source.minDistance, 500f);
			}
		}

		// Token: 0x040057BC RID: 22460
		private const float kMaxAudioRange = 500f;

		// Token: 0x040057BD RID: 22461
		private const string kRangeKey = "MaxRange";

		// Token: 0x040057BE RID: 22462
		[SerializeField]
		private AudioSource m_source;

		// Token: 0x040057BF RID: 22463
		[SerializeField]
		private BGCcCursor m_cursor;
	}
}
