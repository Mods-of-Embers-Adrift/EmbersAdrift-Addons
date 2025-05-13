using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A52 RID: 2642
	public static class OperationTypeExtensions
	{
		// Token: 0x060051EB RID: 20971 RVA: 0x00076AE7 File Offset: 0x00074CE7
		public static bool AffectsInstanceOnly(this OperationType opType)
		{
			return opType - OperationType.CustomVisualIndex <= 1 || opType - OperationType.AddFlags <= 1;
		}
	}
}
