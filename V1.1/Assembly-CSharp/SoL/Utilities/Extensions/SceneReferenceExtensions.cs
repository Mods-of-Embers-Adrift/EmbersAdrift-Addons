using System;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000336 RID: 822
	public static class SceneReferenceExtensions
	{
		// Token: 0x0600168F RID: 5775 RVA: 0x00051C01 File Offset: 0x0004FE01
		public static bool IsValid(this SceneReference reference)
		{
			return reference != null && !string.IsNullOrEmpty(reference.ScenePath);
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00051C16 File Offset: 0x0004FE16
		public static bool AddToBuild(this SceneReference reference)
		{
			return !reference.IsAddressable() && reference.IsValid();
		}
	}
}
