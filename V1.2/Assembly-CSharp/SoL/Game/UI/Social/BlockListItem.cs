using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F4 RID: 2292
	public class BlockListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x06004332 RID: 17202 RVA: 0x0006D532 File Offset: 0x0006B732
		public void Init(Relation relation)
		{
			this.m_relation = relation;
			this.Refresh();
		}

		// Token: 0x06004333 RID: 17203 RVA: 0x00195330 File Offset: 0x00193530
		public void Refresh()
		{
			this.m_topLine.text = this.m_relation.Target;
			DateTime serverCorrectedDateTime = GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now);
			DateTime dateTime = this.m_relation.Created.ToLocalTime();
			string text;
			if (dateTime.Date == serverCorrectedDateTime.Date)
			{
				text = "Today";
			}
			else if (dateTime.Date == serverCorrectedDateTime.Date.AddDays(-1.0))
			{
				text = "Yesterday";
			}
			else if (dateTime.Year == serverCorrectedDateTime.Year)
			{
				text = dateTime.ToString("m");
			}
			else
			{
				text = dateTime.ToString("d");
			}
			this.m_bottomLine.text = text;
		}

		// Token: 0x06004334 RID: 17204 RVA: 0x0006D541 File Offset: 0x0006B741
		private void Unblock()
		{
			ClientGameManager.SocialManager.DeleteRelation(this.m_relation._id);
		}

		// Token: 0x06004335 RID: 17205 RVA: 0x0006D558 File Offset: 0x0006B758
		string IContextMenu.FillActionsGetTitle()
		{
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Unblock", true, new Action(this.Unblock), null, null);
			return this.m_relation.Target;
		}

		// Token: 0x17000F3E RID: 3902
		// (get) Token: 0x06004336 RID: 17206 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004337 RID: 17207 RVA: 0x00049FFA File Offset: 0x000481FA
		private ITooltipParameter GetTooltipParameter()
		{
			return null;
		}

		// Token: 0x17000F3F RID: 3903
		// (get) Token: 0x06004338 RID: 17208 RVA: 0x0006D583 File Offset: 0x0006B783
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F40 RID: 3904
		// (get) Token: 0x06004339 RID: 17209 RVA: 0x0006D591 File Offset: 0x0006B791
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600433B RID: 17211 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003FDD RID: 16349
		[SerializeField]
		private TextMeshProUGUI m_topLine;

		// Token: 0x04003FDE RID: 16350
		[SerializeField]
		private TextMeshProUGUI m_bottomLine;

		// Token: 0x04003FDF RID: 16351
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003FE0 RID: 16352
		private Relation m_relation;
	}
}
