using System;
using SoL.Game.Objects.Containers;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA4 RID: 2980
	public class InteractiveRuneCollector : BaseNetworkInteractiveStation
	{
		// Token: 0x170015C5 RID: 5573
		// (get) Token: 0x06005C51 RID: 23633 RVA: 0x0007DF7D File Offset: 0x0007C17D
		protected override string m_tooltipText
		{
			get
			{
				return "Rune Collector";
			}
		}

		// Token: 0x170015C6 RID: 5574
		// (get) Token: 0x06005C52 RID: 23634 RVA: 0x000701E6 File Offset: 0x0006E3E6
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.RuneCollector;
			}
		}
	}
}
