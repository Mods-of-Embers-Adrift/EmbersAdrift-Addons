using System;
using SoL.Game.EffectSystem;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D88 RID: 3464
	public class AngleToTest : MonoBehaviour
	{
		// Token: 0x0600684F RID: 26703 RVA: 0x00214198 File Offset: 0x00212398
		private void Update()
		{
			if (this.m_target == null)
			{
				return;
			}
			this.m_angle = Mathf.Abs(base.gameObject.AngleTo(this.m_target, true));
			this.m_withinAngle = DistanceAngleChecks.EDITOR_MeetsAngleRequirements(base.gameObject, this.m_target, this.m_angleRequirement, new float?(this.m_angle), this.m_conalType);
		}

		// Token: 0x04005A81 RID: 23169
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04005A82 RID: 23170
		[SerializeField]
		private float m_angle;

		// Token: 0x04005A83 RID: 23171
		[SerializeField]
		private ConalTypes m_conalType = ConalTypes.Frontal;

		// Token: 0x04005A84 RID: 23172
		[SerializeField]
		private float m_angleRequirement;

		// Token: 0x04005A85 RID: 23173
		[SerializeField]
		private bool m_withinAngle;
	}
}
