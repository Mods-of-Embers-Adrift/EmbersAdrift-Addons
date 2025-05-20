using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CCD RID: 3277
	public static class ICulleeExtensions
	{
		// Token: 0x06006351 RID: 25425 RVA: 0x00082DD0 File Offset: 0x00080FD0
		public static bool IsNull(this ICullee cullee)
		{
			return cullee == null || cullee.Equals(null);
		}

		// Token: 0x06006352 RID: 25426 RVA: 0x00051849 File Offset: 0x0004FA49
		public static bool TryGetAsType<T>(this ICullee cullee, out T outType) where T : class, ICullee
		{
			outType = (cullee as T);
			return outType != null;
		}
	}
}
