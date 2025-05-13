using System;
using Com.TheFallenGames.OSA.Core;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009B8 RID: 2488
	[Serializable]
	public class ChatLineViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004B2F RID: 19247 RVA: 0x00072DA1 File Offset: 0x00070FA1
		public void UpdateChatLine(string msg)
		{
			this.UpdateFontSize();
			if (this.m_text.text.Equals(msg))
			{
				return;
			}
			this.m_text.ZStringSetText(msg);
		}

		// Token: 0x06004B30 RID: 19248 RVA: 0x00072DC9 File Offset: 0x00070FC9
		public void UpdateFontSize()
		{
			if ((int)this.m_text.fontSize != Options.GameOptions.ChatFontSize.Value)
			{
				this.m_text.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
			}
		}

		// Token: 0x06004B31 RID: 19249 RVA: 0x00072DF9 File Offset: 0x00070FF9
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_text = this.root.GetComponent<TextMeshProUGUI>();
			this.UpdateFontSize();
		}

		// Token: 0x040045C6 RID: 17862
		[SerializeField]
		private TextMeshProUGUI m_text;
	}
}
