using System;
using Cysharp.Text;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Subscription
{
	// Token: 0x020003B0 RID: 944
	[Serializable]
	public class SubscriptionPerkInfo
	{
		// Token: 0x060019A8 RID: 6568 RVA: 0x00106E6C File Offset: 0x0010506C
		public void AddLine(ref Utf16ValueStringBuilder zsb, string indent = null)
		{
			if (this.Invisible)
			{
				zsb.AppendFormat<string>("<color={0}>", "#00000000");
			}
			bool flag = !string.IsNullOrEmpty(indent) && !this.Invisible;
			if (flag)
			{
				zsb.AppendFormat<string>("<indent={0}><size=80%>", indent);
			}
			bool flag2 = !this.Invisible;
			if (string.IsNullOrEmpty(this.Description))
			{
				zsb.Append("");
			}
			else if (string.IsNullOrEmpty(this.Tooltip))
			{
				if (flag2)
				{
					zsb.Append("<sprite=\"SolIcons\" name=\"Circle\" tint=1>");
					zsb.Append(" ");
				}
				zsb.Append(this.Description);
			}
			else
			{
				string arg = "text";
				string text = this.Tooltip;
				if (text.Length > 100)
				{
					arg = "longtext";
					int hashCode = this.Tooltip.GetHashCode();
					SoL.Utilities.Extensions.TextMeshProExtensions.LongTooltips.AddOrReplace(hashCode, text);
					text = hashCode.ToString();
				}
				if (flag2)
				{
					zsb.Append("<sprite=\"SolIcons\" name=\"Circle\" tint=1>");
					zsb.Append(" ");
				}
				zsb.AppendFormat<string, string, string>("<link=\"{0}:{1}\">{2}</link>", arg, text, this.Description);
			}
			if (flag)
			{
				zsb.Append("</size></indent>");
			}
			zsb.Append("\n");
			if (this.Invisible)
			{
				zsb.Append("</color>");
			}
		}

		// Token: 0x0400209B RID: 8347
		public bool Invisible;

		// Token: 0x0400209C RID: 8348
		public string Description;

		// Token: 0x0400209D RID: 8349
		[TextArea]
		public string Tooltip;
	}
}
