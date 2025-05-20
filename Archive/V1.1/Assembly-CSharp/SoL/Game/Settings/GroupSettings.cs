using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200072A RID: 1834
	[Serializable]
	public class GroupSettings
	{
		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x06003707 RID: 14087 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x04003590 RID: 13712
		public float GroupBonusMasteryLeveling = 0.01f;

		// Token: 0x04003591 RID: 13713
		public AnimationCurve MasteryPointGroupBonusCurve;

		// Token: 0x04003592 RID: 13714
		public float LootRollClientTimeout = 30f;

		// Token: 0x04003593 RID: 13715
		public Color LootRollColorNeed = Color.gray;

		// Token: 0x04003594 RID: 13716
		public Color LootRollColorGreed = Color.yellow;

		// Token: 0x04003595 RID: 13717
		public Color LootRollColorPass = Color.red;
	}
}
