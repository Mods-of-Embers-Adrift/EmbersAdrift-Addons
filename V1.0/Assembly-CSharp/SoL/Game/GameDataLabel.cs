using System;
using SoL.Game.Settings;
using TMPro;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000578 RID: 1400
	public class GameDataLabel : MonoBehaviour
	{
		// Token: 0x06002B30 RID: 11056 RVA: 0x0005DFA0 File Offset: 0x0005C1A0
		private void Start()
		{
			this.UpdateLabel();
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x00146474 File Offset: 0x00144674
		private void UpdateLabel()
		{
			if (GlobalSettings.Values != null && GlobalSettings.Values.Configs != null && GlobalSettings.Values.Configs.Data != null)
			{
				string dataString = GlobalSettings.Values.Configs.Data.GetDataString();
				Debug.Log("Starting Client: " + dataString);
				if (this.m_label != null)
				{
					this.m_label.text = dataString;
				}
			}
		}

		// Token: 0x04002B65 RID: 11109
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}
