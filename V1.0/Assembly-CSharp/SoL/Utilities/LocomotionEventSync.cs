using System;
using SoL.Game.Animation;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000297 RID: 663
	[CreateAssetMenu(menuName = "SoL/Animation/LocoEventSync")]
	public class LocomotionEventSync : ScriptableObject
	{
		// Token: 0x04001C6E RID: 7278
		private const string kCopyGroup = "Copy";

		// Token: 0x04001C6F RID: 7279
		[SerializeField]
		private AnimancerAnimationSet m_set;

		// Token: 0x04001C70 RID: 7280
		[SerializeField]
		private string m_targetDirectory;

		// Token: 0x04001C71 RID: 7281
		[SerializeField]
		private bool m_useManualTimes;

		// Token: 0x04001C72 RID: 7282
		[SerializeField]
		private float[] m_manualTimes;

		// Token: 0x04001C73 RID: 7283
		[SerializeField]
		private AnimationClip m_sourceClip;

		// Token: 0x04001C74 RID: 7284
		[SerializeField]
		private AnimationClip[] m_walkClips;

		// Token: 0x04001C75 RID: 7285
		[SerializeField]
		private AnimationClip[] m_runClips;
	}
}
