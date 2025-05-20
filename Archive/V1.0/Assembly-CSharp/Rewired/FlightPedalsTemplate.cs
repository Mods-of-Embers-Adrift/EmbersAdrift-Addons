using System;

namespace Rewired
{
	// Token: 0x02000061 RID: 97
	public sealed class FlightPedalsTemplate : ControllerTemplate, IFlightPedalsTemplate, IControllerTemplate
	{
		// Token: 0x170001FA RID: 506
		// (get) Token: 0x0600039C RID: 924 RVA: 0x000455CE File Offset: 0x000437CE
		IControllerTemplateAxis IFlightPedalsTemplate.leftPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(0);
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x0600039D RID: 925 RVA: 0x000455D7 File Offset: 0x000437D7
		IControllerTemplateAxis IFlightPedalsTemplate.rightPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(1);
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600039E RID: 926 RVA: 0x000455E0 File Offset: 0x000437E0
		IControllerTemplateAxis IFlightPedalsTemplate.slide
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(2);
			}
		}

		// Token: 0x0600039F RID: 927 RVA: 0x000455B4 File Offset: 0x000437B4
		public FlightPedalsTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x04000501 RID: 1281
		public static readonly Guid typeGuid = new Guid("f6fe76f8-be2a-4db2-b853-9e3652075913");

		// Token: 0x04000502 RID: 1282
		public const int elementId_leftPedal = 0;

		// Token: 0x04000503 RID: 1283
		public const int elementId_rightPedal = 1;

		// Token: 0x04000504 RID: 1284
		public const int elementId_slide = 2;
	}
}
