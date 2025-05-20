using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F5 RID: 757
	[Serializable]
	public class TextMeshProUGUIToggle : ObjectToggle<TextMeshProUGUI>
	{
		// Token: 0x06001574 RID: 5492 RVA: 0x000FC8A0 File Offset: 0x000FAAA0
		protected override void UpdateObject()
		{
			string text = (base.State == ToggleController.ToggleState.ON) ? this.m_onString : this.m_offString;
			if (this.m_isUnicode)
			{
				text = Convert.ToChar(int.Parse(text, NumberStyles.HexNumber)).ToString();
			}
			this.m_object.text = text;
		}

		// Token: 0x04001D90 RID: 7568
		[SerializeField]
		private bool m_isUnicode;

		// Token: 0x04001D91 RID: 7569
		[SerializeField]
		private string m_onString;

		// Token: 0x04001D92 RID: 7570
		[SerializeField]
		private string m_offString;
	}
}
