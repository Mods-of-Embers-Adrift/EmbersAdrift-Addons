using System;
using System.Collections.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D9A RID: 3482
	public class DcaGetDna : MonoBehaviour
	{
		// Token: 0x06006886 RID: 26758 RVA: 0x00214B94 File Offset: 0x00212D94
		private void GetDna()
		{
			foreach (KeyValuePair<string, DnaSetter> keyValuePair in this.m_dca.GetDNA(null))
			{
				Debug.Log(keyValuePair.Key);
			}
		}

		// Token: 0x06006887 RID: 26759 RVA: 0x00214BF4 File Offset: 0x00212DF4
		private void GetPredefinedDna()
		{
			foreach (DnaValue dnaValue in this.m_dca.predefinedDNA.PreloadValues)
			{
				Debug.Log(dnaValue.Name + " " + dnaValue.Value.ToString());
			}
		}

		// Token: 0x04005AC9 RID: 23241
		[SerializeField]
		private DynamicCharacterAvatar m_dca;
	}
}
