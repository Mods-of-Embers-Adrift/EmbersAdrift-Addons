using System;

namespace SoL.Utilities
{
	// Token: 0x02000268 RID: 616
	[Serializable]
	public class ParameterizedLevelCurve : ParameterizedCurve
	{
		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x0600138B RID: 5003 RVA: 0x0004FC6D File Offset: 0x0004DE6D
		protected override float MaxValue
		{
			get
			{
				return 50f;
			}
		}
	}
}
