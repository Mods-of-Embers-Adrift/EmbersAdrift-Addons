using System;
using System.Text;
using Cysharp.Text;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x0200037A RID: 890
	public abstract class BaseTooltip : DynamicUIWindow
	{
		// Token: 0x06001889 RID: 6281
		protected abstract void SetData();

		// Token: 0x0600188A RID: 6282 RVA: 0x00104F74 File Offset: 0x00103174
		protected override void Awake()
		{
			base.Awake();
			if (this.m_backgroundAlpha && GlobalSettings.Values && GlobalSettings.Values.UI != null)
			{
				Color color = this.m_backgroundAlpha.color;
				color.a = GlobalSettings.Values.UI.TooltipBackgroundAlpha / 255f;
				this.m_backgroundAlpha.color = color;
			}
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x00053389 File Offset: 0x00051589
		public void ActivateTooltip(BaseTooltip.GetTooltipParameter paramGetter, bool skipTransition = false)
		{
			this.m_parameterGetter = paramGetter;
			this.SetData();
			this.LateUpdateInternal();
			this.Show(skipTransition);
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x000533A5 File Offset: 0x000515A5
		public virtual void DeactivateTooltip()
		{
			if (base.Visible)
			{
				this.Hide(false);
			}
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x000533B6 File Offset: 0x000515B6
		protected override void LateUpdateInternal()
		{
			this.SetData();
			base.LateUpdateInternal();
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x000533C4 File Offset: 0x000515C4
		public static string LeftRightText(string left, string right)
		{
			return ZString.Format<string, string>("<align=left>{0}<line-height=0.001>\n<align=right>{1}<line-height=1em></align>", left, right);
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000533D2 File Offset: 0x000515D2
		public static void AppendLine(string txt, int indent = 0)
		{
			if (indent > 0)
			{
				txt = txt.Indent(indent);
			}
			BaseTooltip.Sb.AppendLine(txt);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x000533ED File Offset: 0x000515ED
		public static void AppendLine(string left, string right)
		{
			BaseTooltip.Sb.AppendLine(BaseTooltip.LeftRightText(left, right));
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x00053401 File Offset: 0x00051601
		public static void AppendHalfLine()
		{
			BaseTooltip.Sb.AppendLine("<line-height=50%>  </line-height>");
		}

		// Token: 0x04001FB6 RID: 8118
		[SerializeField]
		private Image m_backgroundAlpha;

		// Token: 0x04001FB7 RID: 8119
		public static StringBuilder Sb = new StringBuilder();

		// Token: 0x04001FB8 RID: 8120
		protected BaseTooltip.GetTooltipParameter m_parameterGetter;

		// Token: 0x0200037B RID: 891
		// (Invoke) Token: 0x06001895 RID: 6293
		public delegate ITooltipParameter GetTooltipParameter();
	}
}
