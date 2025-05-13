using System;

namespace SoL.Networking
{
	// Token: 0x020003C7 RID: 967
	[Serializable]
	public class NetworkedAnimatorParamSetting
	{
		// Token: 0x0400211F RID: 8479
		public string ParamName;

		// Token: 0x04002120 RID: 8480
		public bool LocallyInterpolated;

		// Token: 0x04002121 RID: 8481
		public float Acceleration = 5f;

		// Token: 0x04002122 RID: 8482
		public float Deceleration = 2f;
	}
}
