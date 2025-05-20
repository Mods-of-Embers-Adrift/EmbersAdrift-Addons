using System;
using System.Text;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000919 RID: 2329
	public class PendingFriendsListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x0600449A RID: 17562 RVA: 0x0006E59E File Offset: 0x0006C79E
		public void Init(Mail request)
		{
			this.m_request = request;
			this.Refresh();
		}

		// Token: 0x0600449B RID: 17563 RVA: 0x0019CE08 File Offset: 0x0019B008
		public void Refresh()
		{
			DateTime serverCorrectedDateTime = GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now);
			DateTime dateTime = this.m_request.Created.ToLocalTime();
			string arg;
			if (dateTime.Date == serverCorrectedDateTime.Date)
			{
				arg = "Today";
			}
			else if (dateTime.Date == serverCorrectedDateTime.Date.AddDays(-1.0))
			{
				arg = "Yesterday";
			}
			else if (dateTime.Year == serverCorrectedDateTime.Year)
			{
				arg = dateTime.ToString("m");
			}
			else
			{
				arg = dateTime.ToString("d");
			}
			this.m_date.ZStringSetText(arg);
			if (this.m_request.Sender == LocalPlayer.GameEntity.CharacterData.Name)
			{
				this.m_name.ZStringSetText(this.m_request.Recipient);
				this.m_layoutGroup.padding.right = 35;
				this.m_pendingText.gameObject.SetActive(true);
				this.m_acceptButton.gameObject.SetActive(false);
				this.m_rejectButtonTooltipText.ZStringSetText("Cancel");
				return;
			}
			this.m_name.ZStringSetText(this.m_request.Sender);
			this.m_layoutGroup.padding.right = 60;
			this.m_pendingText.gameObject.SetActive(false);
			this.m_acceptButton.gameObject.SetActive(true);
			this.m_rejectButtonTooltipText.ZStringSetText("Reject");
		}

		// Token: 0x0600449C RID: 17564 RVA: 0x0006E5AD File Offset: 0x0006C7AD
		private void Start()
		{
			this.m_acceptButton.onClick.AddListener(new UnityAction(this.Accept));
			this.m_rejectButton.onClick.AddListener(new UnityAction(this.Reject));
		}

		// Token: 0x0600449D RID: 17565 RVA: 0x0006E5E7 File Offset: 0x0006C7E7
		private void OnDestroy()
		{
			this.m_rejectButton.onClick.RemoveListener(new UnityAction(this.Reject));
			this.m_acceptButton.onClick.RemoveListener(new UnityAction(this.Accept));
		}

		// Token: 0x0600449E RID: 17566 RVA: 0x0006E621 File Offset: 0x0006C821
		private void SendTell()
		{
			UIManager.ActiveChatInput.StartWhisper(this.m_request.Sender);
		}

		// Token: 0x0600449F RID: 17567 RVA: 0x0006E638 File Offset: 0x0006C838
		private void Accept()
		{
			ClientGameManager.SocialManager.AcceptFriendRequest(this.m_request._id);
		}

		// Token: 0x060044A0 RID: 17568 RVA: 0x0006E64F File Offset: 0x0006C84F
		private void Reject()
		{
			ClientGameManager.SocialManager.RejectFriendRequest(this.m_request._id);
		}

		// Token: 0x060044A1 RID: 17569 RVA: 0x0006E666 File Offset: 0x0006C866
		private void Block()
		{
			ClientGameManager.SocialManager.Block(this.m_request.Sender);
		}

		// Token: 0x060044A2 RID: 17570 RVA: 0x0019CF94 File Offset: 0x0019B194
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

		// Token: 0x17000F63 RID: 3939
		// (get) Token: 0x060044A3 RID: 17571 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060044A4 RID: 17572 RVA: 0x0019D058 File Offset: 0x0019B258
		private ITooltipParameter GetTooltipParameter()
		{
			DateTime serverCorrectedDateTime = GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now);
			DateTime dateTime = this.m_request.Created.ToLocalTime();
			string value;
			if (dateTime.Date == serverCorrectedDateTime.Date)
			{
				value = "Today";
			}
			else if (dateTime.Date == serverCorrectedDateTime.Date.AddDays(-1.0))
			{
				value = "Yesterday";
			}
			else if (dateTime.Year == serverCorrectedDateTime.Year)
			{
				value = dateTime.ToString("m");
			}
			else
			{
				value = dateTime.ToString("d");
			}
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			fromPool.AppendLine((this.m_request.Sender == LocalPlayer.GameEntity.CharacterData.Name) ? this.m_request.Recipient : this.m_request.Sender);
			fromPool.Append("Sent: ");
			fromPool.AppendLine(value);
			return new ObjectTextTooltipParameter(this, fromPool.ToString_ReturnToPool(), false);
		}

		// Token: 0x17000F64 RID: 3940
		// (get) Token: 0x060044A5 RID: 17573 RVA: 0x0006E67D File Offset: 0x0006C87D
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F65 RID: 3941
		// (get) Token: 0x060044A6 RID: 17574 RVA: 0x0006E68B File Offset: 0x0006C88B
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060044A8 RID: 17576 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400412B RID: 16683
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x0400412C RID: 16684
		[SerializeField]
		private TextMeshProUGUI m_date;

		// Token: 0x0400412D RID: 16685
		[SerializeField]
		private TextMeshProUGUI m_pendingText;

		// Token: 0x0400412E RID: 16686
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x0400412F RID: 16687
		[SerializeField]
		private SolButton m_rejectButton;

		// Token: 0x04004130 RID: 16688
		[SerializeField]
		private TextMeshProUGUI m_rejectButtonTooltipText;

		// Token: 0x04004131 RID: 16689
		[SerializeField]
		private HorizontalLayoutGroup m_layoutGroup;

		// Token: 0x04004132 RID: 16690
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004133 RID: 16691
		private Mail m_request;
	}
}
