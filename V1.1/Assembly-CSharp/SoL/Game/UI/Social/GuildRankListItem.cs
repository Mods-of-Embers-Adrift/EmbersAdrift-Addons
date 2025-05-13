using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000904 RID: 2308
	public class GuildRankListItem : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060043BF RID: 17343 RVA: 0x0019753C File Offset: 0x0019573C
		private void Start()
		{
			this.m_editButton.onClick.AddListener(new UnityAction(this.OnEditClicked));
			this.m_sortUpButton.onClick.AddListener(new UnityAction(this.OnMoveUpClicked));
			this.m_sortDownButton.onClick.AddListener(new UnityAction(this.OnMoveDownClicked));
			this.m_deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteClicked));
		}

		// Token: 0x060043C0 RID: 17344 RVA: 0x001975BC File Offset: 0x001957BC
		private void OnDestroy()
		{
			this.m_editButton.onClick.RemoveListener(new UnityAction(this.OnEditClicked));
			this.m_sortUpButton.onClick.RemoveListener(new UnityAction(this.OnMoveUpClicked));
			this.m_sortDownButton.onClick.RemoveListener(new UnityAction(this.OnMoveDownClicked));
			this.m_deleteButton.onClick.RemoveListener(new UnityAction(this.OnDeleteClicked));
		}

		// Token: 0x060043C1 RID: 17345 RVA: 0x0006DC95 File Offset: 0x0006BE95
		public void Init(GuildRankList parent, int index, GuildRank rank)
		{
			this.m_parent = parent;
			this.m_index = index;
			this.m_rank = rank;
			this.RefreshVisuals();
		}

		// Token: 0x060043C2 RID: 17346 RVA: 0x0019763C File Offset: 0x0019583C
		private void RefreshVisuals()
		{
			GuildRank ownGuildRank = ClientGameManager.SocialManager.OwnGuildRank;
			bool flag = ownGuildRank.Permissions.HasBitFlag(GuildPermissions.EditRanks);
			GuildRank guildRank = null;
			foreach (GuildRank guildRank2 in ClientGameManager.SocialManager.Guild.Ranks)
			{
				if (guildRank2.Sort == this.m_rank.Sort + 1)
				{
					guildRank = guildRank2;
					break;
				}
			}
			this.m_name.text = this.m_rank.Name;
			this.m_editButton.interactable = (flag && (ownGuildRank.Sort == int.MaxValue || this.m_rank.Sort < ownGuildRank.Sort));
			this.m_sortUpButton.interactable = (flag && this.m_rank.Sort + 1 < ownGuildRank.Sort && this.m_rank.Sort != int.MaxValue && guildRank != null);
			this.m_sortDownButton.interactable = (flag && this.m_rank.Sort < ownGuildRank.Sort && this.m_rank.Sort > 0);
			this.m_deleteButton.interactable = (flag && this.m_rank.Sort < ownGuildRank.Sort && this.m_rank.Sort != int.MaxValue);
		}

		// Token: 0x060043C3 RID: 17347 RVA: 0x0006DCB2 File Offset: 0x0006BEB2
		private void OnEditClicked()
		{
			this.m_parent.OpenRankEdit(this.m_rank._id);
		}

		// Token: 0x060043C4 RID: 17348 RVA: 0x001977B8 File Offset: 0x001959B8
		private void OnMoveUpClicked()
		{
			GuildRank rank = this.m_rank;
			int sort = rank.Sort + 1;
			rank.Sort = sort;
			ClientGameManager.SocialManager.UpdateGuildRank(this.m_rank._id, this.m_rank);
		}

		// Token: 0x060043C5 RID: 17349 RVA: 0x001977F8 File Offset: 0x001959F8
		private void OnMoveDownClicked()
		{
			GuildRank rank = this.m_rank;
			int sort = rank.Sort - 1;
			rank.Sort = sort;
			ClientGameManager.SocialManager.UpdateGuildRank(this.m_rank._id, this.m_rank);
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x00197838 File Offset: 0x00195A38
		private void OnDeleteClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Delete Rank",
				Text = "Are you sure you want to delete the \"" + this.m_rank.Name + "\" rank?",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnDeleteConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060043C7 RID: 17351 RVA: 0x0006DCCA File Offset: 0x0006BECA
		private void OnDeleteConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.RemoveGuildRank(this.m_rank._id);
			}
		}

		// Token: 0x17000F50 RID: 3920
		// (get) Token: 0x060043C8 RID: 17352 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060043C9 RID: 17353 RVA: 0x00049FFA File Offset: 0x000481FA
		private ITooltipParameter GetTooltipParameter()
		{
			return null;
		}

		// Token: 0x17000F51 RID: 3921
		// (get) Token: 0x060043CA RID: 17354 RVA: 0x0006DCE4 File Offset: 0x0006BEE4
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F52 RID: 3922
		// (get) Token: 0x060043CB RID: 17355 RVA: 0x0006DCF2 File Offset: 0x0006BEF2
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060043CD RID: 17357 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004038 RID: 16440
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04004039 RID: 16441
		[SerializeField]
		private SolButton m_editButton;

		// Token: 0x0400403A RID: 16442
		[SerializeField]
		private SolButton m_sortUpButton;

		// Token: 0x0400403B RID: 16443
		[SerializeField]
		private SolButton m_sortDownButton;

		// Token: 0x0400403C RID: 16444
		[SerializeField]
		private SolButton m_deleteButton;

		// Token: 0x0400403D RID: 16445
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400403E RID: 16446
		private GuildRankList m_parent;

		// Token: 0x0400403F RID: 16447
		private int m_index = -1;

		// Token: 0x04004040 RID: 16448
		private GuildRank m_rank;

		// Token: 0x04004041 RID: 16449
		private string m_tooltipStr;
	}
}
