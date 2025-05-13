using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA3 RID: 2723
	public static class RuneSourceTypeExtensions
	{
		// Token: 0x17001351 RID: 4945
		// (get) Token: 0x0600542F RID: 21551 RVA: 0x000784FC File Offset: 0x000766FC
		public static RuneSourceType[] RuneSourceTypes
		{
			get
			{
				if (RuneSourceTypeExtensions.m_runeSourceTypes == null)
				{
					RuneSourceTypeExtensions.m_runeSourceTypes = (RuneSourceType[])Enum.GetValues(typeof(RuneSourceType));
				}
				return RuneSourceTypeExtensions.m_runeSourceTypes;
			}
		}

		// Token: 0x06005430 RID: 21552 RVA: 0x00078523 File Offset: 0x00076723
		public static bool OfferForExchange(this RuneSourceType type)
		{
			return type - RuneSourceType.Death > 1;
		}

		// Token: 0x04004B01 RID: 19201
		private static RuneSourceType[] m_runeSourceTypes;
	}
}
