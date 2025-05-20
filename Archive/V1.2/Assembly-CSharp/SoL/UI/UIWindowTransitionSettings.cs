using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x020003A1 RID: 929
	[CreateAssetMenu(menuName = "SoL/UI/Transition Settings")]
	public class UIWindowTransitionSettings : ScriptableObject
	{
		// Token: 0x04002064 RID: 8292
		public UIWindowTransitionSettings.TransitionCurves InCurves;

		// Token: 0x04002065 RID: 8293
		public UIWindowTransitionSettings.TransitionCurves OutCurves;

		// Token: 0x04002066 RID: 8294
		public bool Scale = true;

		// Token: 0x04002067 RID: 8295
		public Vector3 DefaultScale = Vector3.one;

		// Token: 0x04002068 RID: 8296
		public Vector3 ScaleInStart = new Vector3(0.9f, 0.9f, 0.9f);

		// Token: 0x04002069 RID: 8297
		public Vector3 ScaleOutComplete = new Vector3(1.04f, 1.04f, 1.04f);

		// Token: 0x020003A2 RID: 930
		[Serializable]
		public class TransitionCurves
		{
			// Token: 0x0400206A RID: 8298
			public AnimationCurve Alpha;

			// Token: 0x0400206B RID: 8299
			public AnimationCurve Scale;
		}
	}
}
