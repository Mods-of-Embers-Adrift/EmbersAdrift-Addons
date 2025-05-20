using System;

namespace Rewired
{
	// Token: 0x0200005B RID: 91
	public interface IFlightPedalsTemplate : IControllerTemplate
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000298 RID: 664
		IControllerTemplateAxis leftPedal { get; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000299 RID: 665
		IControllerTemplateAxis rightPedal { get; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600029A RID: 666
		IControllerTemplateAxis slide { get; }
	}
}
