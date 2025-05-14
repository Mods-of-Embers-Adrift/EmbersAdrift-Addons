using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x020003A3 RID: 931
	public class WindowComponentStylizer : MonoBehaviour
	{
		// Token: 0x06001978 RID: 6520 RVA: 0x00053EBE File Offset: 0x000520BE
		private void SetDefaultFrameColor()
		{
			if (this.m_defaultFrameColor == null && this.m_frame)
			{
				this.m_defaultFrameColor = new Color?(this.m_frame.color);
			}
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x00053EF0 File Offset: 0x000520F0
		private void Awake()
		{
			this.DisableHighlight();
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x00053EF8 File Offset: 0x000520F8
		public void EnableHighlight()
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = true;
			}
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x00053F14 File Offset: 0x00052114
		public void DisableHighlight()
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = false;
			}
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x00053F30 File Offset: 0x00052130
		private void SetColors()
		{
			this.OnFrameColorChanged();
			this.OnHighlightColorChanged();
			this.OnOrnamentColorChanged();
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x00053F44 File Offset: 0x00052144
		private void OnFrameColorChanged()
		{
			if (this.m_frame)
			{
				this.SetDefaultFrameColor();
				this.m_frame.color = this.m_frameColor;
			}
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x00053F6A File Offset: 0x0005216A
		private void OnHighlightColorChanged()
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.color = this.m_highlightColor;
			}
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x00053F8B File Offset: 0x0005218B
		private void OnOrnamentColorChanged()
		{
			if (this.m_ornamentController != null)
			{
				this.m_ornamentController.SetColor(this.m_ornamentColor);
			}
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x00053FAC File Offset: 0x000521AC
		private void OnOrnamentTypeChanged()
		{
			if (this.m_ornamentController != null)
			{
				this.m_ornamentController.Type = this.m_ornamentType;
			}
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x00053FCD File Offset: 0x000521CD
		public void ResetFrameColor()
		{
			this.SetDefaultFrameColor();
			if (this.m_defaultFrameColor != null)
			{
				this.m_frameColor = this.m_defaultFrameColor.Value;
			}
			this.OnFrameColorChanged();
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x00053FF9 File Offset: 0x000521F9
		public void SetFrameColor(Color color)
		{
			this.m_frameColor = color;
			this.OnFrameColorChanged();
		}

		// Token: 0x0400206C RID: 8300
		[SerializeField]
		private Image m_frame;

		// Token: 0x0400206D RID: 8301
		[SerializeField]
		private Image m_highlight;

		// Token: 0x0400206E RID: 8302
		[SerializeField]
		private WindowOrnamentController m_ornamentController;

		// Token: 0x0400206F RID: 8303
		[SerializeField]
		private Color m_frameColor = Color.white;

		// Token: 0x04002070 RID: 8304
		[SerializeField]
		private Color m_highlightColor = Color.white;

		// Token: 0x04002071 RID: 8305
		[SerializeField]
		private Color m_ornamentColor = Color.white;

		// Token: 0x04002072 RID: 8306
		[SerializeField]
		private OrnamentType m_ornamentType;

		// Token: 0x04002073 RID: 8307
		private Color? m_defaultFrameColor;
	}
}
