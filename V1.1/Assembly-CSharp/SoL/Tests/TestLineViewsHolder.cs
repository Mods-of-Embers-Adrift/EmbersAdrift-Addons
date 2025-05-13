using System;
using Com.TheFallenGames.OSA.Core;
using TMPro;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DB9 RID: 3513
	[Serializable]
	public class TestLineViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06006911 RID: 26897 RVA: 0x000867D0 File Offset: 0x000849D0
		public bool UpdateChatLine(string msg, int index)
		{
			if (string.Equals(this.m_message, msg))
			{
				return false;
			}
			this.m_message = msg;
			this.m_text.text = msg;
			return true;
		}

		// Token: 0x06006912 RID: 26898 RVA: 0x00216804 File Offset: 0x00214A04
		public float GetHeight()
		{
			return this.m_text.GetPreferredValues(this.m_text.text, this.m_text.rectTransform.rect.width, this.m_text.rectTransform.rect.height).y;
		}

		// Token: 0x06006913 RID: 26899 RVA: 0x000867F6 File Offset: 0x000849F6
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_text = this.root.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x04005B6B RID: 23403
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04005B6C RID: 23404
		private string m_message;
	}
}
