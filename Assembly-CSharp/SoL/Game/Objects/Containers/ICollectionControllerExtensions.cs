using System;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A14 RID: 2580
	public static class ICollectionControllerExtensions
	{
		// Token: 0x06004EF6 RID: 20214 RVA: 0x00051849 File Offset: 0x0004FA49
		public static bool TryGetAsType<T>(this ICollectionController controller, out T outType) where T : class, ICollectionController
		{
			outType = (controller as T);
			return outType != null;
		}
	}
}
