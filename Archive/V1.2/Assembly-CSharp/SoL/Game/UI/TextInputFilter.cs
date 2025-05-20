using System;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008E3 RID: 2275
	public class TextInputFilter : MonoBehaviour
	{
		// Token: 0x140000C5 RID: 197
		// (add) Token: 0x06004294 RID: 17044 RVA: 0x001932CC File Offset: 0x001914CC
		// (remove) Token: 0x06004295 RID: 17045 RVA: 0x00193304 File Offset: 0x00191504
		public event Action<string> InputChanged;

		// Token: 0x06004296 RID: 17046 RVA: 0x0006CEE6 File Offset: 0x0006B0E6
		private void Awake()
		{
			this.m_input.onValueChanged.AddListener(new UnityAction<string>(this.OnInputChanged));
			this.m_clear.onClick.AddListener(new UnityAction(this.ClearClicked));
		}

		// Token: 0x06004297 RID: 17047 RVA: 0x0006CF20 File Offset: 0x0006B120
		private void OnDestroy()
		{
			this.m_input.onValueChanged.AddListener(new UnityAction<string>(this.OnInputChanged));
			this.m_clear.onClick.RemoveListener(new UnityAction(this.ClearClicked));
		}

		// Token: 0x06004298 RID: 17048 RVA: 0x0006CF5A File Offset: 0x0006B15A
		private void ClearClicked()
		{
			this.m_input.text = string.Empty;
		}

		// Token: 0x06004299 RID: 17049 RVA: 0x0006CF6C File Offset: 0x0006B16C
		private void OnInputChanged(string arg0)
		{
			Action<string> inputChanged = this.InputChanged;
			if (inputChanged == null)
			{
				return;
			}
			inputChanged(arg0);
		}

		// Token: 0x0600429A RID: 17050 RVA: 0x0006CF7F File Offset: 0x0006B17F
		public string GetText()
		{
			if (!this.m_input)
			{
				return string.Empty;
			}
			return this.m_input.text;
		}

		// Token: 0x0600429B RID: 17051 RVA: 0x0006CF9F File Offset: 0x0006B19F
		public void SetText(string value)
		{
			if (this.m_input)
			{
				this.m_input.text = value;
			}
		}

		// Token: 0x0600429C RID: 17052 RVA: 0x0006CFBA File Offset: 0x0006B1BA
		public void Deactivate()
		{
			if (this.m_input)
			{
				this.m_input.Deactivate();
			}
		}

		// Token: 0x04003F8E RID: 16270
		[SerializeField]
		private SolTMP_InputField m_input;

		// Token: 0x04003F8F RID: 16271
		[SerializeField]
		private SolButton m_clear;
	}
}
