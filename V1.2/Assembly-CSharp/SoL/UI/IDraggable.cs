using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000366 RID: 870
	public interface IDraggable
	{
		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x060017CA RID: 6090
		RectTransform RectTransform { get; }

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x060017CB RID: 6091
		bool ExternallyHandlePositionUpdate { get; }

		// Token: 0x060017CC RID: 6092
		void CompleteDrag(bool canceled);
	}
}
