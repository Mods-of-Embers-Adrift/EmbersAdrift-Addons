using System;
using System.Text;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Penalties
{
	// Token: 0x0200095B RID: 2395
	public class PenaltiesListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x060046FF RID: 18175 RVA: 0x001A58E0 File Offset: 0x001A3AE0
		public void Init(PenaltiesList parent, int index, Penalty data)
		{
			this.m_parent = parent;
			this.m_index = index;
			this.m_penalty = data;
			this.m_nameLabel.ZStringSetText(parent.CurrentUserData.Username);
			string str;
			if (data.Expiration == null)
			{
				str = "Forever";
			}
			else if (data.Expiration.Value < DateTime.UtcNow)
			{
				str = "Expired".Color(UIManager.RequirementsMetColor);
			}
			else
			{
				str = (data.Expiration.Value - DateTime.UtcNow).GetFormatted();
			}
			this.m_penaltyLabel.ZStringSetText(data.Type.ToString() + " (" + str + ")");
			this.m_descriptionLabel.ZStringSetText(data.InfractionDescription);
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			fromPool.AppendLine(parent.CurrentUserData.Username + " (" + data.UserId + ")");
			fromPool.AppendLine("Characters:");
			foreach (string str2 in this.m_parent.CurrentUserData.CharacterNames)
			{
				fromPool.AppendLine(" - " + str2);
			}
			this.m_tooltipStr = fromPool.ToString_ReturnToPool();
		}

		// Token: 0x17000FC8 RID: 4040
		// (get) Token: 0x06004700 RID: 18176 RVA: 0x0006FD58 File Offset: 0x0006DF58
		public InteractionSettings Settings
		{
			get
			{
				return this.m_interationSettings;
			}
		}

		// Token: 0x06004701 RID: 18177 RVA: 0x0006FD60 File Offset: 0x0006DF60
		private void CopyUserIdToClipboard()
		{
			GUIUtility.systemCopyBuffer = this.m_penalty.UserId;
		}

		// Token: 0x06004702 RID: 18178 RVA: 0x001A5A60 File Offset: 0x001A3C60
		private void DeletePenalty()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Delete Penalty",
				Text = "Are you sure you want to delete this penalty?",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnDeletePenaltyConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06004703 RID: 18179 RVA: 0x001A5AD4 File Offset: 0x001A3CD4
		private void OnDeletePenaltyConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				SolServerCommand solServerCommand = CommandClass.penalties.NewCommand(CommandType.delete);
				solServerCommand.Args.Add("penaltyid", this.m_penalty._id);
				solServerCommand.Send();
				this.m_parent.MarkDirty();
			}
		}

		// Token: 0x06004704 RID: 18180 RVA: 0x001A5B1C File Offset: 0x001A3D1C
		public string FillActionsGetTitle()
		{
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Copy UserID to Clipboard", true, new Action(this.CopyUserIdToClipboard), null, null);
			ContextMenuUI.AddContextAction("Delete Penalty", true, new Action(this.DeletePenalty), null, null);
			return this.m_parent.CurrentUserData.Username;
		}

		// Token: 0x06004705 RID: 18181 RVA: 0x0006FD72 File Offset: 0x0006DF72
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.m_tooltipStr, false);
		}

		// Token: 0x17000FC9 RID: 4041
		// (get) Token: 0x06004706 RID: 18182 RVA: 0x0006FD86 File Offset: 0x0006DF86
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000FCA RID: 4042
		// (get) Token: 0x06004707 RID: 18183 RVA: 0x0006FD94 File Offset: 0x0006DF94
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004709 RID: 18185 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040042D8 RID: 17112
		private const string kCopyUID = "Copy UserID to Clipboard";

		// Token: 0x040042D9 RID: 17113
		private const string kDelete = "Delete Penalty";

		// Token: 0x040042DA RID: 17114
		[SerializeField]
		private TextMeshProUGUI m_nameLabel;

		// Token: 0x040042DB RID: 17115
		[SerializeField]
		private TextMeshProUGUI m_penaltyLabel;

		// Token: 0x040042DC RID: 17116
		[SerializeField]
		private TextMeshProUGUI m_descriptionLabel;

		// Token: 0x040042DD RID: 17117
		[SerializeField]
		private InteractionSettings m_interationSettings;

		// Token: 0x040042DE RID: 17118
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040042DF RID: 17119
		private PenaltiesList m_parent;

		// Token: 0x040042E0 RID: 17120
		private int m_index = -1;

		// Token: 0x040042E1 RID: 17121
		private Penalty m_penalty;

		// Token: 0x040042E2 RID: 17122
		private string m_tooltipStr;
	}
}
