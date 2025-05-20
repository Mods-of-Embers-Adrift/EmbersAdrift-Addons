using System;
using System.Collections;
using SoL.Game.NPCs.Senses;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DBC RID: 3516
	public class SensorProfileVisuals : MonoBehaviour
	{
		// Token: 0x06006920 RID: 26912 RVA: 0x00063AF0 File Offset: 0x00061CF0
		private IEnumerable GetSensorProfile()
		{
			return SolOdinUtilities.GetDropdownItems<NpcSensorProfile>();
		}

		// Token: 0x04005B7A RID: 23418
		[SerializeField]
		private NpcSensorProfile m_profile;

		// Token: 0x04005B7B RID: 23419
		[SerializeField]
		private bool m_drawOlfactory;

		// Token: 0x04005B7C RID: 23420
		[SerializeField]
		private bool m_drawAuditory;

		// Token: 0x04005B7D RID: 23421
		[SerializeField]
		private bool m_drawPeripheral;

		// Token: 0x04005B7E RID: 23422
		[SerializeField]
		private bool m_drawImmediate;
	}
}
