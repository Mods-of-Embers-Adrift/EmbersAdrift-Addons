using System;
using Cysharp.Text;
using UnityEngine;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD4 RID: 3028
	[Serializable]
	public class HuntingLogTitle
	{
		// Token: 0x06005D87 RID: 23943 RVA: 0x00044765 File Offset: 0x00042965
		public HuntingLogTitle()
		{
		}

		// Token: 0x06005D88 RID: 23944 RVA: 0x0007ED8A File Offset: 0x0007CF8A
		public HuntingLogTitle(HuntingLogTitle other)
		{
			this.m_title = other.m_title;
			this.m_bypassPrefix = other.m_bypassPrefix;
		}

		// Token: 0x06005D89 RID: 23945 RVA: 0x0007EDAA File Offset: 0x0007CFAA
		private string GetInfoBox()
		{
			if (this.IsNullOrEmpty)
			{
				return "NONE";
			}
			if (!this.m_bypassPrefix)
			{
				return "<PROFILE PREFIX> " + this.m_title;
			}
			return this.m_title;
		}

		// Token: 0x06005D8A RID: 23946 RVA: 0x0007EDD9 File Offset: 0x0007CFD9
		public string GetTitle(HuntingLogProfile profile)
		{
			if (!profile || this.m_bypassPrefix)
			{
				return this.m_title;
			}
			return ZString.Format<string, string>("{0} {1}", profile.TitlePrefix, this.m_title);
		}

		// Token: 0x17001618 RID: 5656
		// (get) Token: 0x06005D8B RID: 23947 RVA: 0x0007EE08 File Offset: 0x0007D008
		public bool IsNullOrEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.m_title);
			}
		}

		// Token: 0x17001619 RID: 5657
		// (get) Token: 0x06005D8C RID: 23948 RVA: 0x0007EE15 File Offset: 0x0007D015
		public bool HasValue
		{
			get
			{
				return !this.IsNullOrEmpty;
			}
		}

		// Token: 0x040050E1 RID: 20705
		[SerializeField]
		private string m_title;

		// Token: 0x040050E2 RID: 20706
		[SerializeField]
		private bool m_bypassPrefix;
	}
}
