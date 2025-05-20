using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200056F RID: 1391
	public class EmberRingUpdater : MonoBehaviour
	{
		// Token: 0x04002B26 RID: 11046
		private const float kRefreshCadence = 1f;

		// Token: 0x04002B27 RID: 11047
		[SerializeField]
		private CampfireEffectApplicator m_applicator;

		// Token: 0x04002B28 RID: 11048
		[SerializeField]
		private InteractiveEmberRing m_interactiveEmberRing;
	}
}
