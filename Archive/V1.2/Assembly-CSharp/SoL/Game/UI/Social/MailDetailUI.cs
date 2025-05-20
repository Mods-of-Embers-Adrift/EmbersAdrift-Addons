using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000913 RID: 2323
	public class MailDetailUI : MonoBehaviour
	{
		// Token: 0x17000F5F RID: 3935
		// (get) Token: 0x06004468 RID: 17512 RVA: 0x0006E304 File Offset: 0x0006C504
		public UniversalContainerUI Attachments
		{
			get
			{
				return this.m_attachments;
			}
		}

		// Token: 0x140000C9 RID: 201
		// (add) Token: 0x06004469 RID: 17513 RVA: 0x0019BC44 File Offset: 0x00199E44
		// (remove) Token: 0x0600446A RID: 17514 RVA: 0x0019BC7C File Offset: 0x00199E7C
		public event Action<string, string> ReplyClicked;

		// Token: 0x0600446B RID: 17515 RVA: 0x0019BCB4 File Offset: 0x00199EB4
		private void Start()
		{
			this.m_backButton.onClick.AddListener(new UnityAction(this.OnBackButtonClicked));
			this.m_deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteButtonClicked));
			this.m_replyButton.onClick.AddListener(new UnityAction(this.OnReplyButtonClicked));
			this.m_acceptButton.onClick.AddListener(new UnityAction(this.OnAcceptButtonClicked));
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.ExpiredPost += this.OnMailExpired;
			}
			if (this.m_attachments.ContainerInstance != null)
			{
				this.m_attachments.ContainerInstance.LockFlags |= ContainerLockFlags.PostIncoming;
				return;
			}
			Debug.LogError("Failed to lock PostIncoming container, no container instance found.");
		}

		// Token: 0x0600446C RID: 17516 RVA: 0x0019BD84 File Offset: 0x00199F84
		private void OnDestroy()
		{
			this.m_backButton.onClick.RemoveListener(new UnityAction(this.OnBackButtonClicked));
			this.m_deleteButton.onClick.RemoveListener(new UnityAction(this.OnDeleteButtonClicked));
			this.m_replyButton.onClick.RemoveListener(new UnityAction(this.OnReplyButtonClicked));
			this.m_acceptButton.onClick.RemoveListener(new UnityAction(this.OnAcceptButtonClicked));
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.ExpiredPost -= this.OnMailExpired;
			}
		}

		// Token: 0x0600446D RID: 17517 RVA: 0x0019BE24 File Offset: 0x0019A024
		public void Init(Mail mail)
		{
			this.m_mail = mail;
			base.gameObject.SetActive(true);
			this.UnlockDetailUI();
			this.RefreshVisuals();
			if (this.m_attachments.ContainerInstance != null)
			{
				this.m_attachments.ContainerInstance.LockFlags |= ContainerLockFlags.PostIncoming;
			}
			else
			{
				Debug.LogError("Failed to lock PostIncoming container, no container instance found.");
			}
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.MarkAsRead(this.m_mail._id, true);
			}
		}

		// Token: 0x0600446E RID: 17518 RVA: 0x0019BEA4 File Offset: 0x0019A0A4
		private void RemoveAttachmentContents()
		{
			if (!this.m_attachments || this.m_attachments.ContainerInstance == null)
			{
				return;
			}
			List<ArchetypeInstance> list = this.m_attachments.ContainerInstance.RemoveAllInstances();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && list[i].InstanceUI)
				{
					list[i].InstanceUI.ExternalOnDestroy();
					UnityEngine.Object.Destroy(list[i].InstanceUI.gameObject);
				}
			}
			StaticListPool<ArchetypeInstance>.ReturnToPool(list);
			ulong currency = this.m_attachments.ContainerInstance.Currency;
			if (currency > 0UL)
			{
				this.m_attachments.ContainerInstance.RemoveCurrency(currency);
			}
		}

		// Token: 0x0600446F RID: 17519 RVA: 0x0019BF60 File Offset: 0x0019A160
		private void RefreshVisuals()
		{
			if (!LocalPlayer.GameEntity || this.m_mail == null || this.m_mail.Type != MailType.Post)
			{
				return;
			}
			DateTime serverCorrectedDateTime = GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now);
			DateTime dateTime = this.m_mail.Created.ToLocalTime();
			TimeSpan t = serverCorrectedDateTime - dateTime;
			string arg;
			if (t < TimeSpan.FromDays(1.0))
			{
				arg = dateTime.ToString("t");
			}
			else if (t < TimeSpan.FromDays(365.0))
			{
				arg = dateTime.ToString("M");
			}
			else
			{
				arg = dateTime.ToString("d");
			}
			string arg2;
			if (this.m_mail.Expires != null)
			{
				dateTime = this.m_mail.Expires.Value.ToLocalTime();
				TimeSpan t2 = dateTime - serverCorrectedDateTime;
				if (t2 < TimeSpan.FromDays(1.0))
				{
					arg2 = dateTime.ToString("t");
				}
				else if (t2 < TimeSpan.FromDays(365.0))
				{
					arg2 = dateTime.ToString("M");
				}
				else
				{
					arg2 = dateTime.ToString("d");
				}
			}
			else
			{
				arg2 = "Never";
			}
			this.m_sent.ZStringSetText(arg);
			this.m_expires.ZStringSetText(arg2);
			bool flag = this.m_mail.Sender == LocalPlayer.GameEntity.CharacterData.Name.Value;
			if (flag)
			{
				this.m_otherNameLabel.ZStringSetText("To:");
				this.m_otherName.text = this.m_mail.Recipient;
				this.m_deleteButton.text = "Delete";
			}
			else
			{
				this.m_otherNameLabel.ZStringSetText("From:");
				this.m_otherName.text = this.m_mail.Sender;
				this.m_deleteButton.text = ((this.m_mail.CanBeReturned && !this.m_mail.Returned.GetValueOrDefault()) ? "Return" : "Delete");
			}
			this.m_deleteButton.gameObject.SetActive(!flag);
			this.m_replyButton.gameObject.SetActive(!flag);
			this.m_subject.text = this.m_mail.Subject;
			this.m_body.text = this.m_mail.Message;
			this.RemoveAttachmentContents();
			if (this.m_mail.ItemAttachments != null && this.m_mail.ItemAttachments.Count > 0)
			{
				for (int i = 0; i < this.m_mail.ItemAttachments.Count; i++)
				{
					ArchetypeInstance archetypeInstance = this.m_mail.ItemAttachments[i];
					archetypeInstance.CreateItemInstanceUI();
					archetypeInstance.InstanceUI.ToggleLock(true);
					this.m_attachments.ContainerInstance.Add(archetypeInstance, true);
				}
			}
			ulong? num;
			ulong num2;
			bool flag2;
			if (this.m_attachments.ContainerInstance.Count <= 0)
			{
				num = this.m_mail.CurrencyAttachment;
				num2 = 0UL;
				flag2 = (num.GetValueOrDefault() > num2 & num != null);
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			if ((this.m_mail.Returned != null && this.m_mail.Returned.Value && flag3) || (this.m_mail.IsSystemMail && flag3) || flag)
			{
				this.m_deleteButton.interactable = false;
				this.m_deleteButtonTooltip.Text = (flag ? "You may not delete sent mail." : "Please claim any remaining attachments and/or money.");
			}
			else
			{
				this.m_deleteButton.interactable = true;
				this.m_deleteButtonTooltip.Text = null;
			}
			num = this.m_mail.CashOnDelivery;
			num2 = 0UL;
			bool flag4 = num.GetValueOrDefault() > num2 & num != null;
			if (flag4)
			{
				this.m_monelLabel.ZStringSetText("COD:");
				this.m_attachments.ContainerInstance.AddCurrency(this.m_mail.CashOnDelivery.GetValueOrDefault());
			}
			else
			{
				this.m_monelLabel.ZStringSetText("Receiving:");
				this.m_attachments.ContainerInstance.AddCurrency(this.m_mail.CurrencyAttachment.GetValueOrDefault());
			}
			if (!this.m_detailUILocked)
			{
				if (!flag && (this.m_attachments.ContainerInstance.Count > 0 || (this.m_attachments.ContainerInstance.Currency > 0UL && !flag4)))
				{
					this.m_acceptButton.interactable = true;
					this.m_acceptButtonTooltip.Text = null;
					return;
				}
				this.m_acceptButton.interactable = false;
				this.m_acceptButtonTooltip.Text = (flag ? "Cannot accept content from outgoing mail." : "Nothing to accept.");
			}
		}

		// Token: 0x06004470 RID: 17520 RVA: 0x0006E30C File Offset: 0x0006C50C
		private void OnBackButtonClicked()
		{
			base.gameObject.SetActive(false);
			this.RemoveAttachmentContents();
		}

		// Token: 0x06004471 RID: 17521 RVA: 0x0019C434 File Offset: 0x0019A634
		private void OnDeleteButtonClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Delete Mail",
				Text = ((this.m_mail.Sender == LocalPlayer.GameEntity.CharacterData.Name.Value) ? "Are you sure you want to recind this mail item? You will be able to reclaim attachments and/or money when the message is returned to your inbox." : ((this.m_mail.CanBeReturned && !this.m_mail.Returned.GetValueOrDefault()) ? "Are you sure you want to return this mail item? All attachments and/or money will be returned to sender." : "Are you sure you want to delete this mail item?")),
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnDeleteConfirmed)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06004472 RID: 17522 RVA: 0x0006E320 File Offset: 0x0006C520
		private void OnDeleteConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.DeletePost(this.m_mail._id);
				base.gameObject.SetActive(false);
				this.m_attachments.ContainerInstance.DestroyContents();
			}
		}

		// Token: 0x06004473 RID: 17523 RVA: 0x0006E356 File Offset: 0x0006C556
		private void OnReplyButtonClicked()
		{
			Action<string, string> replyClicked = this.ReplyClicked;
			if (replyClicked == null)
			{
				return;
			}
			replyClicked(this.m_mail.Sender, this.m_mail.Subject);
		}

		// Token: 0x06004474 RID: 17524 RVA: 0x0006E37E File Offset: 0x0006C57E
		private void OnAcceptButtonClicked()
		{
			if (LocalPlayer.NetworkEntity)
			{
				this.LockDetailUI();
				LocalPlayer.NetworkEntity.PlayerRpcHandler.AcceptMailRequest(this.m_mail._id);
			}
		}

		// Token: 0x06004475 RID: 17525 RVA: 0x0019C4FC File Offset: 0x0019A6FC
		private void OnMailExpired(Mail mail)
		{
			if (mail._id == this.m_mail._id)
			{
				this.m_deleteButton.interactable = false;
				this.m_deleteButtonTooltip.Text = "This message has now expired.";
				this.m_acceptButton.interactable = false;
				this.m_acceptButtonTooltip.Text = "This message has now expired.";
			}
		}

		// Token: 0x06004476 RID: 17526 RVA: 0x0019C55C File Offset: 0x0019A75C
		private void LockDetailUI()
		{
			this.m_backButton.interactable = false;
			this.m_deleteButton.interactable = false;
			this.m_deleteButtonTooltip.Text = "Busy";
			this.m_acceptButton.interactable = false;
			this.m_acceptButtonTooltip.Text = "Busy";
			this.m_detailUILocked = true;
		}

		// Token: 0x06004477 RID: 17527 RVA: 0x0019C5B4 File Offset: 0x0019A7B4
		private void UnlockDetailUI()
		{
			this.m_backButton.interactable = true;
			this.m_deleteButton.interactable = true;
			this.m_deleteButtonTooltip.Text = null;
			this.m_acceptButton.interactable = true;
			this.m_acceptButtonTooltip.Text = null;
			this.m_detailUILocked = false;
		}

		// Token: 0x040040EA RID: 16618
		private const string kMsgNothingToAccept = "Nothing to accept.";

		// Token: 0x040040EB RID: 16619
		private const string kMsgCantAcceptOutgoing = "Cannot accept content from outgoing mail.";

		// Token: 0x040040EC RID: 16620
		private const string kMsgDeleteConfirmationTitle = "Delete Mail";

		// Token: 0x040040ED RID: 16621
		private const string kMsgRecindConfirmationBody = "Are you sure you want to recind this mail item? You will be able to reclaim attachments and/or money when the message is returned to your inbox.";

		// Token: 0x040040EE RID: 16622
		private const string kMsgReturnConfirmationBody = "Are you sure you want to return this mail item? All attachments and/or money will be returned to sender.";

		// Token: 0x040040EF RID: 16623
		private const string kMsgDeleteConfirmationBody = "Are you sure you want to delete this mail item?";

		// Token: 0x040040F0 RID: 16624
		private const string kMsgDeleteMustClaimItems = "Please claim any remaining attachments and/or money.";

		// Token: 0x040040F1 RID: 16625
		private const string kMsgCannotRescind = "You may not delete sent mail.";

		// Token: 0x040040F2 RID: 16626
		private const string kMsgCannotRescindReturned = "You may not rescind returned mail.";

		// Token: 0x040040F3 RID: 16627
		private const string kMsgAcceptButtonUILocked = "Busy";

		// Token: 0x040040F4 RID: 16628
		private const string kMsgExpired = "This message has now expired.";

		// Token: 0x040040F5 RID: 16629
		[SerializeField]
		private TextMeshProUGUI m_sent;

		// Token: 0x040040F6 RID: 16630
		[SerializeField]
		private TextMeshProUGUI m_expires;

		// Token: 0x040040F7 RID: 16631
		[SerializeField]
		private TextMeshProUGUI m_otherNameLabel;

		// Token: 0x040040F8 RID: 16632
		[SerializeField]
		private TMP_InputField m_otherName;

		// Token: 0x040040F9 RID: 16633
		[SerializeField]
		private TMP_InputField m_subject;

		// Token: 0x040040FA RID: 16634
		[SerializeField]
		private TMP_InputField m_body;

		// Token: 0x040040FB RID: 16635
		[SerializeField]
		private TextMeshProUGUI m_monelLabel;

		// Token: 0x040040FC RID: 16636
		[SerializeField]
		private UniversalContainerUI m_attachments;

		// Token: 0x040040FD RID: 16637
		[SerializeField]
		private SolButton m_backButton;

		// Token: 0x040040FE RID: 16638
		[SerializeField]
		private SolButton m_deleteButton;

		// Token: 0x040040FF RID: 16639
		[SerializeField]
		private TextTooltipTrigger m_deleteButtonTooltip;

		// Token: 0x04004100 RID: 16640
		[SerializeField]
		private SolButton m_replyButton;

		// Token: 0x04004101 RID: 16641
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x04004102 RID: 16642
		[SerializeField]
		private TextTooltipTrigger m_acceptButtonTooltip;

		// Token: 0x04004104 RID: 16644
		private Mail m_mail;

		// Token: 0x04004105 RID: 16645
		private bool m_detailUILocked;
	}
}
