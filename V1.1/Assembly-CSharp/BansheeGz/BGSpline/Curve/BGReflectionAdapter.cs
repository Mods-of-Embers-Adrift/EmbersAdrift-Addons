using System;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x020001A2 RID: 418
	public static class BGReflectionAdapter
	{
		// Token: 0x06000EAF RID: 3759 RVA: 0x0004C80E File Offset: 0x0004AA0E
		public static object[] GetCustomAttributes(Type type, Type attributeType, bool inherit)
		{
			return type.GetCustomAttributes(attributeType, inherit);
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x0004C818 File Offset: 0x0004AA18
		public static bool IsAbstract(Type type)
		{
			return type.IsAbstract;
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0004C820 File Offset: 0x0004AA20
		public static bool IsClass(Type type)
		{
			return type.IsClass;
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0004C828 File Offset: 0x0004AA28
		public static bool IsSubclassOf(Type type, Type typeToCheck)
		{
			return type.IsSubclassOf(typeToCheck);
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0004C831 File Offset: 0x0004AA31
		public static bool IsValueType(Type type)
		{
			return type.IsValueType;
		}
	}
}
