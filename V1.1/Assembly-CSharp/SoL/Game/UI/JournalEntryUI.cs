using System;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Quests;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000864 RID: 2148
	public class JournalEntryUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000E57 RID: 3671
		// (get) Token: 0x06003E0F RID: 15887 RVA: 0x00069F59 File Offset: 0x00068159
		public UniqueId QuestId
		{
			get
			{
				if (!(this.m_quest == null))
				{
					return this.m_quest.Id;
				}
				return UniqueId.Empty;
			}
		}

		// Token: 0x06003E10 RID: 15888 RVA: 0x00184300 File Offset: 0x00182500
		public bool Init(UniqueId questId)
		{
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(questId, out quest) && !string.IsNullOrEmpty(quest.Title))
			{
				this.m_quest = quest;
			}
			this.RefreshEntry();
			return this.m_quest != null;
		}

		// Token: 0x06003E11 RID: 15889 RVA: 0x00184344 File Offset: 0x00182544
		public void RefreshEntry()
		{
			QuestProgressionData questProgressionData;
			if (this.m_quest == null || !this.m_quest.TryGetProgress(out questProgressionData))
			{
				this.m_text.text = null;
				return;
			}
			this.m_text.text = this.m_quest.Title;
			BaseTooltip.Sb.Clear();
			QuestStep questStep;
			if (this.m_quest.TryGetStep(questProgressionData.CurrentNodeId, out questStep))
			{
				BaseTooltip.Sb.AppendLine(questStep.LogEntry);
			}
			if (BaseTooltip.Sb.Length > 0)
			{
				this.m_parameter = new ObjectTextTooltipParameter(this, BaseTooltip.Sb.ToString(), false);
				return;
			}
			this.m_parameter = null;
		}

		// Token: 0x06003E12 RID: 15890 RVA: 0x00069F7A File Offset: 0x0006817A
		private ITooltipParameter GetTooltipParameter()
		{
			return this.m_parameter;
		}

		// Token: 0x17000E58 RID: 3672
		// (get) Token: 0x06003E13 RID: 15891 RVA: 0x00069F82 File Offset: 0x00068182
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E59 RID: 3673
		// (get) Token: 0x06003E14 RID: 15892 RVA: 0x00069F90 File Offset: 0x00068190
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E5A RID: 3674
		// (get) Token: 0x06003E15 RID: 15893 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003E17 RID: 15895 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C6B RID: 15467
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04003C6C RID: 15468
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C6D RID: 15469
		private Quest m_quest;

		// Token: 0x04003C6E RID: 15470
		private ITooltipParameter m_parameter;
	}
}
