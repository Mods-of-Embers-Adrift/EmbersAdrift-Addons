using System;
using System.Text;
using Cysharp.Text;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x0200038D RID: 909
	public class TooltipTextBlock : MonoBehaviour
	{
		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x060018DB RID: 6363 RVA: 0x00053668 File Offset: 0x00051868
		public StringBuilder Sb
		{
			get
			{
				return this.m_sb;
			}
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x060018DC RID: 6364 RVA: 0x00053670 File Offset: 0x00051870
		// (set) Token: 0x060018DD RID: 6365 RVA: 0x0005367D File Offset: 0x0005187D
		public string Title
		{
			get
			{
				return this.m_title.text;
			}
			set
			{
				this.m_title.ZStringSetText(value);
				this.m_title.enabled = !string.IsNullOrEmpty(value);
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x060018DE RID: 6366 RVA: 0x0005369F File Offset: 0x0005189F
		// (set) Token: 0x060018DF RID: 6367 RVA: 0x000536A7 File Offset: 0x000518A7
		public bool ShowSpacer
		{
			get
			{
				return this.m_showSpacer;
			}
			set
			{
				this.m_showSpacer = value;
				this.m_spacer.enabled = this.m_showSpacer;
			}
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x000536C1 File Offset: 0x000518C1
		public void ResetBlock()
		{
			this.m_sb.Clear();
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x00105774 File Offset: 0x00103974
		public void ToggleBlock()
		{
			bool flag = this.m_sb.Length <= 0;
			if (!flag)
			{
				this.m_body.text = this.m_sb.ToString();
			}
			this.m_sb.Clear();
			if (flag && base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			else if (!flag && !base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x000533C4 File Offset: 0x000515C4
		public static string LeftRightText(string left, string right)
		{
			return ZString.Format<string, string>("<align=left>{0}<line-height=0.001>\n<align=right>{1}<line-height=1em></align>", left, right);
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x000536CF File Offset: 0x000518CF
		public void Append(string txt, int indent = 0)
		{
			if (indent > 0)
			{
				txt = txt.Indent(indent);
			}
			this.m_sb.Append(txt);
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x000536EB File Offset: 0x000518EB
		public void AppendLine(string txt, int indent = 0)
		{
			if (indent > 0)
			{
				txt = txt.Indent(indent);
			}
			this.m_sb.AppendLine(txt);
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x00053707 File Offset: 0x00051907
		public void AppendLineAtStart(string txt, int indent = 0)
		{
			if (indent > 0)
			{
				txt = txt.Indent(indent);
			}
			this.m_sb.Insert(0, txt + "\n");
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x0005372E File Offset: 0x0005192E
		public void AppendLine(string left, string right)
		{
			this.m_sb.AppendLine(TooltipTextBlock.LeftRightText(left, right));
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x00053743 File Offset: 0x00051943
		public void AppendLineAtStart(string left, string right)
		{
			this.m_sb.Insert(0, TooltipTextBlock.LeftRightText(left, right) + "\n");
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x001057F4 File Offset: 0x001039F4
		public void AddSpacer(int size = 20)
		{
			if (this.m_sb.Length > 1)
			{
				this.m_sb.Remove(this.m_sb.Length - 1, 1);
			}
			this.m_sb.Append(ZString.Format<int>("<line-height={0}%>\n</line-height>\n", size));
		}

		// Token: 0x04001FE7 RID: 8167
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04001FE8 RID: 8168
		[SerializeField]
		private TextMeshProUGUI m_body;

		// Token: 0x04001FE9 RID: 8169
		[SerializeField]
		private Image m_spacer;

		// Token: 0x04001FEA RID: 8170
		[SerializeField]
		private GameObject m_titlePanel;

		// Token: 0x04001FEB RID: 8171
		private readonly StringBuilder m_sb = new StringBuilder();

		// Token: 0x04001FEC RID: 8172
		private bool m_showSpacer = true;
	}
}
