using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F7 RID: 2295
	public class FriendsListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x06004345 RID: 17221 RVA: 0x0006D625 File Offset: 0x0006B825
		public void Init(Relation relation)
		{
			this.m_relation = relation;
			this.Refresh();
		}

		// Token: 0x06004346 RID: 17222 RVA: 0x0006D634 File Offset: 0x0006B834
		public void Refresh()
		{
			this.m_topLine.text = this.m_relation.OtherName;
			this.OnStatusUpdate();
		}

		// Token: 0x06004347 RID: 17223 RVA: 0x0006D652 File Offset: 0x0006B852
		private void Start()
		{
			if (this.m_icon)
			{
				this.m_defaultRoleIconColor = this.m_icon.color;
			}
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x0019549C File Offset: 0x0019369C
		private void OnStatusUpdate()
		{
			PlayerStatus playerStatus;
			if (ClientGameManager.SocialManager.TryGetLatestStatus(this.m_relation.OtherName, out playerStatus))
			{
				this.m_level.SetTextFormat("Lvl {0}", playerStatus.Level);
				BaseArchetype roleFromPacked = GlobalSettings.Values.Roles.GetRoleFromPacked(playerStatus.RolePacked);
				Color color = roleFromPacked ? roleFromPacked.IconTint : this.m_defaultRoleIconColor;
				if (roleFromPacked)
				{
					this.m_icon.sprite = roleFromPacked.Icon;
					this.m_icon.color = color;
					this.m_iconTooltip.Text = roleFromPacked.DisplayName;
				}
				else
				{
					this.m_icon.sprite = this.m_unknownRoleIcon;
					this.m_icon.color = color;
					this.m_iconTooltip.Text = "Unknown Role";
				}
				if (playerStatus.ZoneId > 0)
				{
					if ((playerStatus.PresenceFlags & PresenceFlags.Invisible) == PresenceFlags.Invisible)
					{
						this.m_icon.color = color * 0.5f;
						this.m_level.color = this.m_offlineNameTextColor;
						this.m_topLine.color = this.m_offlineNameTextColor;
						this.m_bottomLine.color = this.m_offlineZoneTextColor;
						this.m_borderImage.color = this.m_offlineBorderColor;
						this.m_bottomLine.ZStringSetText(this.GetMessageForTimePeriod(this.TimePeriodToDisplay(playerStatus.LastOnline), playerStatus.LastOnline));
					}
					if ((playerStatus.PresenceFlags & PresenceFlags.Anonymous) == PresenceFlags.Anonymous)
					{
						this.m_icon.color = color;
						this.m_level.color = this.m_normalNameTextColor;
						this.m_topLine.color = this.m_normalNameTextColor;
						this.m_bottomLine.color = this.m_normalZoneTextColor;
						this.m_borderImage.color = this.m_normalBorderColor;
						this.m_bottomLine.ZStringSetText("Anonymous");
						return;
					}
					ZoneRecord zoneRecord = SessionData.GetZoneRecord(playerStatus.ZoneIdEnum);
					this.m_icon.color = color;
					this.m_level.color = this.m_normalNameTextColor;
					this.m_topLine.color = this.m_normalNameTextColor;
					this.m_bottomLine.color = this.m_normalZoneTextColor;
					this.m_borderImage.color = this.m_normalBorderColor;
					this.m_bottomLine.ZStringSetText(LocalZoneManager.GetFormattedZoneName(zoneRecord.DisplayName, playerStatus.SubZoneIdEnum));
					return;
				}
				else
				{
					if (playerStatus.ZoneId == 0)
					{
						this.m_icon.color = color * 0.5f;
						this.m_level.color = this.m_normalNameTextColor;
						this.m_topLine.color = this.m_normalNameTextColor;
						this.m_bottomLine.color = this.m_normalZoneTextColor;
						this.m_borderImage.color = this.m_normalBorderColor;
						this.m_bottomLine.ZStringSetText("Unknown");
						return;
					}
					if (playerStatus.ZoneId == -1)
					{
						this.m_icon.color = color * 0.5f;
						this.m_level.color = this.m_offlineNameTextColor;
						this.m_topLine.color = this.m_offlineNameTextColor;
						this.m_bottomLine.color = this.m_offlineZoneTextColor;
						this.m_borderImage.color = this.m_offlineBorderColor;
						this.m_bottomLine.ZStringSetText(this.GetMessageForTimePeriod(this.TimePeriodToDisplay(playerStatus.LastOnline), playerStatus.LastOnline));
						return;
					}
					if (playerStatus.ZoneId == -2)
					{
						if ((playerStatus.PresenceFlags & PresenceFlags.Anonymous) == PresenceFlags.Anonymous)
						{
							this.m_icon.color = color;
							this.m_level.color = this.m_normalNameTextColor;
							this.m_topLine.color = this.m_normalNameTextColor;
							this.m_bottomLine.color = this.m_normalZoneTextColor;
							this.m_borderImage.color = this.m_normalBorderColor;
							this.m_bottomLine.ZStringSetText("Anonymous");
							return;
						}
						this.m_icon.color = color * 0.5f;
						this.m_level.color = this.m_offlineNameTextColor;
						this.m_topLine.color = this.m_offlineNameTextColor;
						this.m_bottomLine.color = this.m_offlineZoneTextColor;
						this.m_borderImage.color = this.m_offlineBorderColor;
						this.m_bottomLine.ZStringSetText(this.GetMessageForTimePeriod(this.TimePeriodToDisplay(playerStatus.LastOnline), playerStatus.LastOnline));
						return;
					}
				}
			}
			else
			{
				this.m_icon.color = this.m_offlineNameTextColor;
				this.m_level.color = this.m_offlineNameTextColor;
				this.m_topLine.color = this.m_offlineNameTextColor;
				this.m_bottomLine.color = this.m_offlineZoneTextColor;
				this.m_borderImage.color = this.m_offlineBorderColor;
				this.m_bottomLine.ZStringSetText(this.GetMessageForTimePeriod(SocialUI.TimePeriod.Invalid, null));
			}
		}

		// Token: 0x06004349 RID: 17225 RVA: 0x0006D672 File Offset: 0x0006B872
		private void SendTell()
		{
			UIManager.ActiveChatInput.StartWhisper(this.m_relation.OtherName);
		}

		// Token: 0x0600434A RID: 17226 RVA: 0x0006D689 File Offset: 0x0006B889
		private void InviteToGroup()
		{
			ClientGameManager.GroupManager.InviteNewMember(this.m_relation.OtherName);
		}

		// Token: 0x0600434B RID: 17227 RVA: 0x0006D6A0 File Offset: 0x0006B8A0
		private void RemoveFriend()
		{
			ClientGameManager.SocialManager.DeleteRelation(this.m_relation._id);
		}

		// Token: 0x0600434C RID: 17228 RVA: 0x0006D6B7 File Offset: 0x0006B8B7
		private void Block()
		{
			ClientGameManager.SocialManager.Block(this.m_relation.OtherName);
		}

		// Token: 0x0600434D RID: 17229 RVA: 0x00195950 File Offset: 0x00193B50
		string IContextMenu.FillActionsGetTitle()
		{
			bool flag = !ClientGameManager.GroupManager.IsGrouped || (ClientGameManager.GroupManager.IsLeader && !ClientGameManager.GroupManager.GroupIsFull);
			bool valueOrDefault = ClientGameManager.SocialManager.IsOnline(this.m_relation.OtherName).GetValueOrDefault();
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Send Tell", valueOrDefault, new Action(this.SendTell), null, null);
			ContextMenuUI.AddContextAction("Invite to Group", valueOrDefault && flag, new Action(this.InviteToGroup), null, null);
			ContextMenuUI.AddContextAction("Remove Friend", true, new Action(this.RemoveFriend), null, null);
			ContextMenuUI.AddContextAction("Remove Friend & Block", true, new Action(this.Block), null, null);
			return this.m_relation.OtherName;
		}

		// Token: 0x17000F41 RID: 3905
		// (get) Token: 0x0600434E RID: 17230 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600434F RID: 17231 RVA: 0x00049FFA File Offset: 0x000481FA
		private ITooltipParameter GetTooltipParameter()
		{
			return null;
		}

		// Token: 0x17000F42 RID: 3906
		// (get) Token: 0x06004350 RID: 17232 RVA: 0x0006D6CE File Offset: 0x0006B8CE
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F43 RID: 3907
		// (get) Token: 0x06004351 RID: 17233 RVA: 0x0006D6DC File Offset: 0x0006B8DC
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004352 RID: 17234 RVA: 0x00195A20 File Offset: 0x00193C20
		private SocialUI.TimePeriod TimePeriodToDisplay(DateTime? lastOnlineUtc)
		{
			if (lastOnlineUtc == null)
			{
				return SocialUI.TimePeriod.Never;
			}
			DateTime d = lastOnlineUtc.Value.ToLocalTime();
			DateTime serverCorrectedDateTime = GameTimeReplicator.GetServerCorrectedDateTime(DateTime.Now);
			TimeSpan t = serverCorrectedDateTime - d;
			if (t < TimeSpan.FromMinutes(1.0))
			{
				return SocialUI.TimePeriod.LessThanAMinute;
			}
			if (t < TimeSpan.FromMinutes(10.0))
			{
				return SocialUI.TimePeriod.FewMinutes;
			}
			if (t < TimeSpan.FromMinutes(60.0))
			{
				return SocialUI.TimePeriod.LessThanAnHour;
			}
			if (t < TimeSpan.FromHours(5.0))
			{
				return SocialUI.TimePeriod.FewHours;
			}
			if (d.Date == serverCorrectedDateTime.Date)
			{
				return SocialUI.TimePeriod.EarlierToday;
			}
			if (d.Date == serverCorrectedDateTime.Date.AddDays(-1.0))
			{
				return SocialUI.TimePeriod.Yesterday;
			}
			if (t < TimeSpan.FromDays(5.0))
			{
				return SocialUI.TimePeriod.FewDays;
			}
			if (d.Year == serverCorrectedDateTime.Year)
			{
				return SocialUI.TimePeriod.ThisYear;
			}
			return SocialUI.TimePeriod.Absolute;
		}

		// Token: 0x06004353 RID: 17235 RVA: 0x00195B28 File Offset: 0x00193D28
		private string GetMessageForTimePeriod(SocialUI.TimePeriod period, DateTime? lastOnlineUtc = null)
		{
			DateTime? dateTime = (lastOnlineUtc != null) ? new DateTime?(lastOnlineUtc.GetValueOrDefault().ToLocalTime()) : null;
			switch (period)
			{
			case SocialUI.TimePeriod.Never:
				return "Offline (Last online: Never)";
			case SocialUI.TimePeriod.LessThanAMinute:
				return "Offline (Last online: Less than a minute ago)";
			case SocialUI.TimePeriod.FewMinutes:
				return "Offline (Last online: A few minutes ago)";
			case SocialUI.TimePeriod.LessThanAnHour:
				return "Offline (Last online: Less than an hour ago)";
			case SocialUI.TimePeriod.FewHours:
				return "Offline (Last online: A few hours ago)";
			case SocialUI.TimePeriod.EarlierToday:
				return "Offline (Last online: Earlier today)";
			case SocialUI.TimePeriod.Yesterday:
				return "Offline (Last online: Yesterday)";
			case SocialUI.TimePeriod.FewDays:
				return "Offline (Last online: A few days ago)";
			case SocialUI.TimePeriod.ThisYear:
				return "Offline (Last online: " + ((dateTime != null) ? dateTime.GetValueOrDefault().ToString("m") : null) + ")";
			case SocialUI.TimePeriod.Absolute:
				return "Offline (Last online: " + ((dateTime != null) ? dateTime.GetValueOrDefault().ToString("d") : null) + ")";
			default:
				return "Offline";
			}
		}

		// Token: 0x06004355 RID: 17237 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003FE4 RID: 16356
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003FE5 RID: 16357
		[SerializeField]
		private TextMeshProUGUI m_level;

		// Token: 0x04003FE6 RID: 16358
		[SerializeField]
		private TextMeshProUGUI m_topLine;

		// Token: 0x04003FE7 RID: 16359
		[SerializeField]
		private TextMeshProUGUI m_bottomLine;

		// Token: 0x04003FE8 RID: 16360
		[SerializeField]
		private TextTooltipTrigger m_iconTooltip;

		// Token: 0x04003FE9 RID: 16361
		[SerializeField]
		private Image m_borderImage;

		// Token: 0x04003FEA RID: 16362
		[SerializeField]
		private Sprite m_unknownRoleIcon;

		// Token: 0x04003FEB RID: 16363
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003FEC RID: 16364
		private readonly Color m_normalNameTextColor = Colors.LightGray;

		// Token: 0x04003FED RID: 16365
		private readonly Color m_normalZoneTextColor = Colors.GrayHTMLCSSGray;

		// Token: 0x04003FEE RID: 16366
		private readonly Color m_offlineNameTextColor = Colors.GraniteGray;

		// Token: 0x04003FEF RID: 16367
		private readonly Color m_offlineZoneTextColor = Colors.GraniteGray;

		// Token: 0x04003FF0 RID: 16368
		private readonly Color m_normalBorderColor = new Color32(100, 104, 106, byte.MaxValue);

		// Token: 0x04003FF1 RID: 16369
		private readonly Color m_offlineBorderColor = new Color32(48, 48, 48, byte.MaxValue);

		// Token: 0x04003FF2 RID: 16370
		private Relation m_relation;

		// Token: 0x04003FF3 RID: 16371
		private Color m_defaultRoleIconColor;

		// Token: 0x04003FF4 RID: 16372
		private const string kUnknownRole = "Unknown Role";
	}
}
