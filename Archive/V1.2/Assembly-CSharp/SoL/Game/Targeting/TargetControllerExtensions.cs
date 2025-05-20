using System;

namespace SoL.Game.Targeting
{
	// Token: 0x02000650 RID: 1616
	public static class TargetControllerExtensions
	{
		// Token: 0x06003263 RID: 12899 RVA: 0x00051849 File Offset: 0x0004FA49
		public static bool TryGetAsType<T>(this BaseTargetController controller, out T asType) where T : class
		{
			asType = (controller as T);
			return asType != null;
		}
	}
}
