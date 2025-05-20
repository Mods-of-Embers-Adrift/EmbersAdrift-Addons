using System;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x02000A00 RID: 2560
	public class RoadSign : MonoBehaviour, ITooltip, IInteractiveBase, ICursor
	{
		// Token: 0x06004DC4 RID: 19908 RVA: 0x00074A68 File Offset: 0x00072C68
		private void Start()
		{
			this.UpdateLabels();
		}

		// Token: 0x06004DC5 RID: 19909 RVA: 0x001C0E2C File Offset: 0x001BF02C
		private void UpdateLabels()
		{
			string signLabel = this.m_setting.GetSignLabel();
			this.SetText(this.m_frontLabel, signLabel);
			this.SetText(this.m_rearLabel, signLabel);
		}

		// Token: 0x06004DC6 RID: 19910 RVA: 0x00074A70 File Offset: 0x00072C70
		private void SetText(TextMeshPro tmp, string value)
		{
			if (tmp)
			{
				tmp.SetText(value);
			}
		}

		// Token: 0x06004DC7 RID: 19911 RVA: 0x00074A81 File Offset: 0x00072C81
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.m_setting.GetTooltipLabel(), false);
		}

		// Token: 0x1700112B RID: 4395
		// (get) Token: 0x06004DC8 RID: 19912 RVA: 0x00074A9A File Offset: 0x00072C9A
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x1700112C RID: 4396
		// (get) Token: 0x06004DC9 RID: 19913 RVA: 0x00074AA2 File Offset: 0x00072CA2
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700112D RID: 4397
		// (get) Token: 0x06004DCA RID: 19914 RVA: 0x00074AB0 File Offset: 0x00072CB0
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x1700112E RID: 4398
		// (get) Token: 0x06004DCB RID: 19915 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		CursorType ICursor.Type
		{
			get
			{
				return CursorType.IdentifyingGlassCursor;
			}
		}

		// Token: 0x06004DCC RID: 19916 RVA: 0x00074AB8 File Offset: 0x00072CB8
		internal void InitializeFromController(RoadSignSettings settings)
		{
			this.m_setting.CopyFromOtherSetting(settings);
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004740 RID: 18240
		[SerializeField]
		private TextMeshPro m_frontLabel;

		// Token: 0x04004741 RID: 18241
		[SerializeField]
		private TextMeshPro m_rearLabel;

		// Token: 0x04004742 RID: 18242
		[SerializeField]
		private RoadSignSettings m_setting;

		// Token: 0x04004743 RID: 18243
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04004744 RID: 18244
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
