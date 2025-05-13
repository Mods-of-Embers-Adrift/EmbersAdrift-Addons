using System;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200035C RID: 860
	public class TextEntryDialog : BaseDialog<DialogOptions>
	{
		// Token: 0x0600177D RID: 6013 RVA: 0x001025F0 File Offset: 0x001007F0
		protected override void Start()
		{
			base.Start();
			this.m_defaultContentType = this.m_input.contentType;
			this.m_defaultLineType = this.m_input.lineType;
			SolTMP_InputField input = this.m_input;
			input.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(input.onValidateInput, new TMP_InputField.OnValidateInput(this.OnValidateInput));
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x000526A5 File Offset: 0x000508A5
		protected override void OnDestroy()
		{
			base.OnDestroy();
			SolTMP_InputField input = this.m_input;
			input.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Remove(input.onValidateInput, new TMP_InputField.OnValidateInput(this.OnValidateInput));
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x000526D4 File Offset: 0x000508D4
		private char OnValidateInput(string text, int pos, char ch)
		{
			if (!this.m_currentOptions.AsciiOnly)
			{
				return ch;
			}
			if (!ch.IsAscii())
			{
				return '\0';
			}
			return ch;
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06001780 RID: 6016 RVA: 0x000526F0 File Offset: 0x000508F0
		protected override object Result
		{
			get
			{
				return this.m_input.text;
			}
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0010264C File Offset: 0x0010084C
		protected override void InitInternal()
		{
			this.m_input.text = this.m_currentOptions.Text;
			this.m_input.richText = this.m_currentOptions.AllowRichText;
			this.m_input.characterLimit = this.m_currentOptions.CharacterLimit;
			this.m_input.lineLimit = this.m_currentOptions.LineLimit;
			this.m_input.contentType = ((this.m_currentOptions.ContentType != null) ? this.m_currentOptions.ContentType.Value : this.m_defaultContentType);
			this.m_input.lineType = ((this.m_currentOptions.LineType != null) ? this.m_currentOptions.LineType.Value : this.m_defaultLineType);
			this.m_input.Activate();
		}

		// Token: 0x04001F36 RID: 7990
		[SerializeField]
		private SolTMP_InputField m_input;

		// Token: 0x04001F37 RID: 7991
		private TMP_InputField.ContentType m_defaultContentType;

		// Token: 0x04001F38 RID: 7992
		private TMP_InputField.LineType m_defaultLineType = TMP_InputField.LineType.MultiLineNewline;
	}
}
