using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C24 RID: 3108
	[Serializable]
	public class KinematicParameters
	{
		// Token: 0x06005FE6 RID: 24550 RVA: 0x000808BB File Offset: 0x0007EABB
		private void ValidateNumbers()
		{
			this.m_maxKinematicDistance = Mathf.Clamp(this.m_maxKinematicDistance, 0f, float.MaxValue);
		}

		// Token: 0x040052B2 RID: 21170
		private const string kKinematicGroupName = "Kinematic";

		// Token: 0x040052B3 RID: 21171
		[SerializeField]
		private KinematicEffectTypes m_kinematicType;

		// Token: 0x040052B4 RID: 21172
		[SerializeField]
		private float m_maxKinematicDistance = 5f;
	}
}
