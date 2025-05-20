using System;
using UnityEngine;

namespace SoL.Animation.Generator
{
	// Token: 0x02000232 RID: 562
	[CreateAssetMenu(menuName = "SoL/Animation/AnimationSet", order = 0)]
	public class AnimationSet : ScriptableObject
	{
		// Token: 0x060012CD RID: 4813 RVA: 0x0004F653 File Offset: 0x0004D853
		private bool ShowRun()
		{
			return this.AllowRun;
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0004F65B File Offset: 0x0004D85B
		private bool ShowCrouch()
		{
			return this.AllowCrouch;
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x0004F663 File Offset: 0x0004D863
		private bool ShowTransitions()
		{
			return this.HasTransitions;
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x0004479C File Offset: 0x0004299C
		private bool ShowAttacks()
		{
			return true;
		}

		// Token: 0x0400107F RID: 4223
		[Header("Idles")]
		public IdleAnimationSet StandIdle;

		// Token: 0x04001080 RID: 4224
		public IdleAnimationSet CrouchIdle;

		// Token: 0x04001081 RID: 4225
		[Header("Transitions")]
		public bool HasTransitions;

		// Token: 0x04001082 RID: 4226
		public TransitionAnimationSet Transitions;

		// Token: 0x04001083 RID: 4227
		[Header("Locomotion")]
		public LocomotionAnimationSet WalkLocomotion;

		// Token: 0x04001084 RID: 4228
		public bool AllowRun = true;

		// Token: 0x04001085 RID: 4229
		public LocomotionAnimationSet RunLocomotion;

		// Token: 0x04001086 RID: 4230
		public bool AllowCrouch = true;

		// Token: 0x04001087 RID: 4231
		public LocomotionAnimationSet CrouchLocomotion;

		// Token: 0x04001088 RID: 4232
		[Header("CombatSpecific")]
		public Motion[] Hits;

		// Token: 0x04001089 RID: 4233
		public DeathPair[] Death;

		// Token: 0x0400108A RID: 4234
		public Motion[] Attacks;
	}
}
