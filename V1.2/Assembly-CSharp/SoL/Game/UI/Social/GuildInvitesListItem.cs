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
	// Token: 0x020008FE RID: 2302
	public class GuildInvitesListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x0600437D RID: 17277 RVA: 0x0006D88C File Offset: 0x0006BA8C
		public void Init(Mail request)
		{
			this.m_request = request;
			this.Refresh();
		}

		// Token: 0x0600437E RID: 17278 RVA: 0x001967B0 File Offset: 0x001949B0
		public void Refresh()
		{
			this.m_name.text = this.m_request.GuildName + " (invited by " + this.m_request.Sender + ")";
			DateTime serverCorrectedDateTime = GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now);
			DateTime dateTime = this.m_request.Created.ToLocalTime();
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
			this.m_date.text = text;
			this.m_pendingText.gameObject.SetActive(false);
			this.m_acceptButton.gameObject.SetActive(true);
			this.m_rejectButtonTooltipText.text = "Reject";
			this.m_tooltipStr = string.Concat(new string[]
			{
				this.m_request.GuildName,
				"\n",
				this.m_request.GuildDescription,
				"\n\nSent By: ",
				this.m_request.Sender,
				"\nSent: ",
				this.m_request.Created.ToString("g")
			});
		}

		// Token: 0x0600437F RID: 17279 RVA: 0x0006D89B File Offset: 0x0006BA9B
		private void Start()
		{
			this.m_acceptButton.onClick.AddListener(new UnityAction(this.Accept));
			this.m_rejectButton.onClick.AddListener(new UnityAction(this.Reject));
		}

		// Token: 0x06004380 RID: 17280 RVA: 0x0006D8D5 File Offset: 0x0006BAD5
		private void OnDestroy()
		{
			this.m_rejectButton.onClick.RemoveListener(new UnityAction(this.Reject));
			this.m_acceptButton.onClick.RemoveListener(new UnityAction(this.Accept));
		}

		// Token: 0x06004381 RID: 17281 RVA: 0x0006D90F File Offset: 0x0006BB0F
		private void SendTell()
		{
			UIManager.ActiveChatInput.StartWhisper(this.m_request.Sender);
		}

		// Token: 0x06004382 RID: 17282 RVA: 0x0006D926 File Offset: 0x0006BB26
		private void Accept()
		{
			ClientGameManager.SocialManager.AcceptGuildInvite(this.m_request._id);
		}

		// Token: 0x06004383 RID: 17283 RVA: 0x0006D93D File Offset: 0x0006BB3D
		private void Reject()
		{
			ClientGameManager.SocialManager.RejectGuildInvite(this.m_request._id);
		}

		// Token: 0x06004384 RID: 17284 RVA: 0x0006D954 File Offset: 0x0006BB54
		private void Block()
		{
			ClientGameManager.SocialManager.Block(this.m_request.Sender);
		}

		// Token: 0x06004385 RID: 17285 RVA: 0x00196934 File Offset: 0x00194B34
		string IContextMenu.FillActionsGetTitle()
		{
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Send Tell", true, new Action(this.SendTell), null, null);
			if (this.m_request.Sender == LocalPlayer.GameEntity.CharacterData.Name)
			{
				ContextMenuUI.AddContextAction("Cancel", true, new Action(this.Reject), null, null);
			}
			else
			{
				ContextMenuUI.AddContextAction("Accept", true, new Action(this.Accept), null, null);
				ContextMenuUI.AddContextAction("Reject", true, new Action(this.Reject), null, null);
			}
			ContextMenuUI.AddContextAction("Block", true, new Action(this.Block), null, null);
			return this.m_request.Sender;
		}

		// Token: 0x17000F46 RID: 3910
		// (get) Token: 0x06004386 RID: 17286 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004387 RID: 17287 RVA: 0x0006D96B File Offset: 0x0006BB6B
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.m_tooltipStr, false);
		}

		// Token: 0x17000F47 RID: 3911
		// (get) Token: 0x06004388 RID: 17288 RVA: 0x0006D97F File Offset: 0x0006BB7F
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F48 RID: 3912
		// (get) Token: 0x06004389 RID: 17289 RVA: 0x0006D98D File Offset: 0x0006BB8D
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600438B RID: 17291 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400401A RID: 16410
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x0400401B RID: 16411
		[SerializeField]
		private TextMeshProUGUI m_date;

		// Token: 0x0400401C RID: 16412
		[SerializeField]
		private TextMeshProUGUI m_pendingText;

		// Token: 0x0400401D RID: 16413
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x0400401E RID: 16414
		[SerializeField]
		private SolButton m_rejectButton;

		// Token: 0x0400401F RID: 16415
		[SerializeField]
		private TextMeshProUGUI m_rejectButtonTooltipText;

		// Token: 0x04004020 RID: 16416
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004021 RID: 16417
		private Mail m_request;

		// Token: 0x04004022 RID: 16418
		private string m_tooltipStr;
	}
}
