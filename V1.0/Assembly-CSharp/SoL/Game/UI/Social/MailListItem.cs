using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000916 RID: 2326
	public class MailListItem : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004484 RID: 17540 RVA: 0x0006E445 File Offset: 0x0006C645
		private void Start()
		{
			this.m_viewButton.onClick.AddListener(new UnityAction(this.OnViewButtonClicked));
			this.m_deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteButtonClicked));
		}

		// Token: 0x06004485 RID: 17541 RVA: 0x0006E47F File Offset: 0x0006C67F
		private void OnDestroy()
		{
			this.m_viewButton.onClick.RemoveListener(new UnityAction(this.OnViewButtonClicked));
			this.m_deleteButton.onClick.RemoveListener(new UnityAction(this.OnDeleteButtonClicked));
		}

		// Token: 0x06004486 RID: 17542 RVA: 0x0006E4B9 File Offset: 0x0006C6B9
		public void Init(Mail data, MailList parent)
		{
			this.m_mail = data;
			this.m_parent = parent;
			this.RefreshVisuals();
		}

		// Token: 0x06004487 RID: 17543 RVA: 0x0019C6BC File Offset: 0x0019A8BC
		private void RefreshVisuals()
		{
			if (!LocalPlayer.GameEntity)
			{
				return;
			}
			bool flag = this.m_mail.Sender == LocalPlayer.GameEntity.CharacterData.Name.Value;
			if (flag)
			{
				this.m_authorLabel.ZStringSetText("To:");
				this.m_author.ZStringSetText(this.m_mail.Recipient);
			}
			else
			{
				this.m_authorLabel.ZStringSetText("From:");
				this.m_author.ZStringSetText(this.m_mail.Sender);
			}
			this.m_subject.ZStringSetText(this.m_mail.Subject);
			if (this.m_mail.ItemAttachments != null && this.m_mail.ItemAttachments.Count > 0 && this.m_mail.ItemAttachments[0] != null && this.m_mail.ItemAttachments[0].Archetype)
			{
				this.m_icon.sprite = this.m_mail.ItemAttachments[0].Archetype.Icon;
				this.m_icon.color = this.m_mail.ItemAttachments[0].Archetype.IconTint;
			}
			else
			{
				this.m_icon.sprite = this.m_defaultIcon;
				this.m_icon.color = Color.white;
			}
			List<ArchetypeInstance> itemAttachments = this.m_mail.ItemAttachments;
			bool flag2;
			if (itemAttachments == null || itemAttachments.Count <= 0)
			{
				ulong? currencyAttachment = this.m_mail.CurrencyAttachment;
				ulong num = 0UL;
				flag2 = (currencyAttachment.GetValueOrDefault() > num & currencyAttachment != null);
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
				this.m_deleteButtonIcon.sprite = this.m_deleteIcon;
			}
			else
			{
				this.m_deleteButton.interactable = true;
				this.m_deleteButtonTooltip.Text = ((this.m_mail.CanBeReturned && !this.m_mail.Returned.GetValueOrDefault()) ? "Return" : "Delete");
				this.m_deleteButtonIcon.sprite = ((this.m_mail.CanBeReturned && !this.m_mail.Returned.GetValueOrDefault()) ? this.m_returnIcon : this.m_deleteIcon);
			}
			this.m_auctionIcon.enabled = this.m_mail.Auction.GetValueOrDefault();
			this.m_returnedIcon.enabled = (!this.m_auctionIcon.enabled && this.m_mail.Returned.GetValueOrDefault());
			this.m_alertIcon.enabled = (this.m_mail.Expires - DateTime.UtcNow < this.m_expiresSoonWindow);
			if (ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsUnread(this.m_mail._id))
			{
				this.m_author.SetTextColor(this.m_unreadTextColor);
				this.m_subject.SetTextColor(this.m_unreadTextColor);
				this.m_border.color = this.m_unreadBorderColor;
				return;
			}
			this.m_author.SetTextColor(this.m_readTextColor);
			this.m_subject.SetTextColor(this.m_readTextColor);
			this.m_border.color = this.m_readBorderColor;
		}

		// Token: 0x06004488 RID: 17544 RVA: 0x0006E4CF File Offset: 0x0006C6CF
		private void OnViewButtonClicked()
		{
			this.m_parent.View(this.m_mail);
		}

		// Token: 0x06004489 RID: 17545 RVA: 0x0019CAAC File Offset: 0x0019ACAC
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

		// Token: 0x0600448A RID: 17546 RVA: 0x0006E4E2 File Offset: 0x0006C6E2
		private void OnDeleteConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				ClientGameManager.SocialManager.DeletePost(this.m_mail._id);
			}
		}

		// Token: 0x17000F60 RID: 3936
		// (get) Token: 0x0600448B RID: 17547 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600448C RID: 17548 RVA: 0x0019CB74 File Offset: 0x0019AD74
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_mail == null)
			{
				return null;
			}
			bool flag = this.m_mail.Sender == LocalPlayer.GameEntity.CharacterData.Name.Value;
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			if (flag)
			{
				fromPool.AppendLine(string.Format("To: {0}", this.m_mail.Recipient));
			}
			else
			{
				fromPool.AppendLine(string.Format("From: {0}", this.m_mail.Sender));
			}
			fromPool.AppendLine(string.Format("Subject: {0}", this.m_mail.Subject));
			fromPool.AppendLine(string.Format("Sent: {0}", this.m_mail.Created.ToLocalTime().ToString("g")));
			DateTime? dateTime;
			fromPool.AppendLine(string.Format("Expires: {0}", ((this.m_mail.Expires != null) ? dateTime.GetValueOrDefault().ToLocalTime().ToString("g") : null) ?? "Never"));
			return new ObjectTextTooltipParameter(this, fromPool.ToString_ReturnToPool(), false);
		}

		// Token: 0x17000F61 RID: 3937
		// (get) Token: 0x0600448D RID: 17549 RVA: 0x0006E4FC File Offset: 0x0006C6FC
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F62 RID: 3938
		// (get) Token: 0x0600448E RID: 17550 RVA: 0x0006E50A File Offset: 0x0006C70A
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004490 RID: 17552 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400410A RID: 16650
		private const string kMsgDeleteConfirmationTitle = "Delete Mail";

		// Token: 0x0400410B RID: 16651
		private const string kMsgRecindConfirmationBody = "Are you sure you want to recind this mail item? You will be able to reclaim attachments and/or money when the message is returned to your inbox.";

		// Token: 0x0400410C RID: 16652
		private const string kMsgReturnConfirmationBody = "Are you sure you want to return this mail item? All attachments and/or money will be returned to sender.";

		// Token: 0x0400410D RID: 16653
		private const string kMsgDeleteConfirmationBody = "Are you sure you want to delete this mail item?";

		// Token: 0x0400410E RID: 16654
		private const string kMsgDeleteMustClaimItems = "Please claim any remaining attachments and/or money.";

		// Token: 0x0400410F RID: 16655
		private const string kMsgCannotRescind = "You may not delete sent mail.";

		// Token: 0x04004110 RID: 16656
		[SerializeField]
		private TextMeshProUGUI m_authorLabel;

		// Token: 0x04004111 RID: 16657
		[SerializeField]
		private TextMeshProUGUI m_author;

		// Token: 0x04004112 RID: 16658
		[SerializeField]
		private TextMeshProUGUI m_subject;

		// Token: 0x04004113 RID: 16659
		[SerializeField]
		private Image m_icon;

		// Token: 0x04004114 RID: 16660
		[SerializeField]
		private SolButton m_viewButton;

		// Token: 0x04004115 RID: 16661
		[SerializeField]
		private SolButton m_deleteButton;

		// Token: 0x04004116 RID: 16662
		[SerializeField]
		private Image m_deleteButtonIcon;

		// Token: 0x04004117 RID: 16663
		[SerializeField]
		private TextTooltipTrigger m_deleteButtonTooltip;

		// Token: 0x04004118 RID: 16664
		[SerializeField]
		private Image m_returnedIcon;

		// Token: 0x04004119 RID: 16665
		[SerializeField]
		private Image m_auctionIcon;

		// Token: 0x0400411A RID: 16666
		[SerializeField]
		private Image m_alertIcon;

		// Token: 0x0400411B RID: 16667
		[SerializeField]
		private Image m_border;

		// Token: 0x0400411C RID: 16668
		[SerializeField]
		private Sprite m_defaultIcon;

		// Token: 0x0400411D RID: 16669
		[SerializeField]
		private Sprite m_deleteIcon;

		// Token: 0x0400411E RID: 16670
		[SerializeField]
		private Sprite m_returnIcon;

		// Token: 0x0400411F RID: 16671
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004120 RID: 16672
		public Action ViewClicked;

		// Token: 0x04004121 RID: 16673
		private Mail m_mail;

		// Token: 0x04004122 RID: 16674
		private MailList m_parent;

		// Token: 0x04004123 RID: 16675
		private readonly Color m_unreadTextColor = UISettings.StandardTextColor;

		// Token: 0x04004124 RID: 16676
		private readonly Color m_readTextColor = Colors.GraniteGray;

		// Token: 0x04004125 RID: 16677
		private readonly Color m_unreadBorderColor = ColorExtensions.FromHexLiteral(6580330U);

		// Token: 0x04004126 RID: 16678
		private readonly Color m_readBorderColor = new Color32(48, 48, 48, byte.MaxValue);

		// Token: 0x04004127 RID: 16679
		private readonly TimeSpan m_expiresSoonWindow = TimeSpan.FromDays(1.0);
	}
}
