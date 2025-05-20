using System;
using TMPro;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000286 RID: 646
	public class FontAssigner : MonoBehaviour
	{
		// Token: 0x04001C24 RID: 7204
		[SerializeField]
		private TMP_Text m_text;

		// Token: 0x04001C25 RID: 7205
		[SerializeField]
		private FontAssigner.FontType m_fontType;

		// Token: 0x02000287 RID: 647
		public enum FontType
		{
			// Token: 0x04001C27 RID: 7207
			None,
			// Token: 0x04001C28 RID: 7208
			Normal,
			// Token: 0x04001C29 RID: 7209
			Button,
			// Token: 0x04001C2A RID: 7210
			Header,
			// Token: 0x04001C2B RID: 7211
			Paragraph,
			// Token: 0x04001C2C RID: 7212
			InputField
		}
	}
}
