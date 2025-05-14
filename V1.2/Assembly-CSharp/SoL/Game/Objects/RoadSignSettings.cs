using System;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x02000A03 RID: 2563
	[Serializable]
	internal class RoadSignSettings
	{
		// Token: 0x1700112F RID: 4399
		// (get) Token: 0x06004DD4 RID: 19924 RVA: 0x00074ACE File Offset: 0x00072CCE
		private bool m_showTooltipTextOverride
		{
			get
			{
				return this.m_type == RoadSignSettings.LabelType.Text && this.m_overrideTooltipText;
			}
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x001C0F48 File Offset: 0x001BF148
		public string GetSignLabel()
		{
			RoadSignSettings.LabelType type = this.m_type;
			if (type == RoadSignSettings.LabelType.Text)
			{
				return this.m_text;
			}
			if (type != RoadSignSettings.LabelType.Zone)
			{
				return string.Empty;
			}
			ZoneRecord zoneRecord = SessionData.GetZoneRecord(this.m_zoneId);
			if (zoneRecord != null)
			{
				return zoneRecord.DisplayName;
			}
			return this.m_zoneId.ToString();
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x00074AE0 File Offset: 0x00072CE0
		public string GetTooltipLabel()
		{
			if (this.m_type != RoadSignSettings.LabelType.Text || !this.m_overrideTooltipText)
			{
				return this.GetSignLabel();
			}
			return this.m_tooltipTextOverride;
		}

		// Token: 0x06004DD7 RID: 19927 RVA: 0x00074AFF File Offset: 0x00072CFF
		public void CopyFromOtherSetting(RoadSignSettings incoming)
		{
			this.m_type = incoming.m_type;
			this.m_text = incoming.m_text;
			this.m_overrideTooltipText = incoming.m_overrideTooltipText;
			this.m_tooltipTextOverride = incoming.m_tooltipTextOverride;
			this.m_zoneId = incoming.m_zoneId;
		}

		// Token: 0x04004748 RID: 18248
		[SerializeField]
		private RoadSignSettings.LabelType m_type;

		// Token: 0x04004749 RID: 18249
		[SerializeField]
		private string m_text;

		// Token: 0x0400474A RID: 18250
		[SerializeField]
		private bool m_overrideTooltipText;

		// Token: 0x0400474B RID: 18251
		[SerializeField]
		private string m_tooltipTextOverride;

		// Token: 0x0400474C RID: 18252
		[SerializeField]
		private ZoneId m_zoneId;

		// Token: 0x02000A04 RID: 2564
		private enum LabelType
		{
			// Token: 0x0400474E RID: 18254
			Text,
			// Token: 0x0400474F RID: 18255
			Zone
		}
	}
}
