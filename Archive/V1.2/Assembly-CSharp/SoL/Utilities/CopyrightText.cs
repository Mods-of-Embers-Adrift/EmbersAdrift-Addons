using System;
using TMPro;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200025A RID: 602
	public class CopyrightText : MonoBehaviour
	{
		// Token: 0x0600135D RID: 4957 RVA: 0x000F6854 File Offset: 0x000F4A54
		private void Awake()
		{
			this.m_label.SetText(string.Format("Copyright © 2014-{0} Stormhaven Studios, LLC.\nStormhaven Studios ® and Embers Adrift ® are registered trademarks of Stormhaven Studios, LLC.  All Rights Reserved", DateTime.Now.Year.ToString()));
		}

		// Token: 0x04001B8F RID: 7055
		private const string kFormatText = "Copyright © 2014-{0} Stormhaven Studios, LLC.\nStormhaven Studios ® and Embers Adrift ® are registered trademarks of Stormhaven Studios, LLC.  All Rights Reserved";

		// Token: 0x04001B90 RID: 7056
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}
