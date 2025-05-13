using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD9 RID: 3033
	public class HuntingLogTierIndicatorUI : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu, ICursor
	{
		// Token: 0x17001623 RID: 5667
		// (get) Token: 0x06005DBE RID: 23998 RVA: 0x0007F06C File Offset: 0x0007D26C
		public RectTransform Rect
		{
			get
			{
				return this.m_rect;
			}
		}

		// Token: 0x06005DBF RID: 23999 RVA: 0x0007F074 File Offset: 0x0007D274
		private void Awake()
		{
			if (this.m_activateButton)
			{
				this.m_activateButton.onClick.AddListener(new UnityAction(this.OnSelectClicked));
			}
		}

		// Token: 0x06005DC0 RID: 24000 RVA: 0x0007F09F File Offset: 0x0007D29F
		private void OnDestroy()
		{
			if (this.m_activateButton)
			{
				this.m_activateButton.onClick.RemoveListener(new UnityAction(this.OnSelectClicked));
			}
		}

		// Token: 0x06005DC1 RID: 24001 RVA: 0x0007F0CA File Offset: 0x0007D2CA
		public void Init(HuntingLogUI controller, HuntingLogProfile profile, HuntingLogTier tier)
		{
			this.m_isActive = false;
			this.m_canBeActivated = false;
			this.m_controller = controller;
			this.m_profile = profile;
			this.m_tier = tier;
			this.Refresh();
		}

		// Token: 0x06005DC2 RID: 24002 RVA: 0x0007F0F5 File Offset: 0x0007D2F5
		public void InitCount(int cnt)
		{
			this.m_count.ZStringSetText(cnt.ToString());
		}

		// Token: 0x06005DC3 RID: 24003 RVA: 0x0007F109 File Offset: 0x0007D309
		public void SetCountFontSize(float targetFontSize)
		{
			this.m_count.fontSize = targetFontSize;
		}

		// Token: 0x06005DC4 RID: 24004 RVA: 0x0007F117 File Offset: 0x0007D317
		public void SetLabel(string txt)
		{
			this.m_label.ZStringSetText(txt);
		}

		// Token: 0x06005DC5 RID: 24005 RVA: 0x0007F125 File Offset: 0x0007D325
		private void OnSelectClicked()
		{
			if (this.m_canBeActivated && this.m_controller)
			{
				this.m_controller.ActivateClicked();
			}
		}

		// Token: 0x06005DC6 RID: 24006 RVA: 0x001F4B14 File Offset: 0x001F2D14
		private void Refresh()
		{
			if (this.m_profile == null || this.m_tier == null || this.m_controller == null || this.m_controller.SelectedEntry == null)
			{
				this.m_activateButton.gameObject.SetActive(false);
				return;
			}
			string arg = string.Empty;
			string arg2 = "<alpha=#FF>";
			string arg3 = "<font=\"Font Awesome 5 Free-Regular-400 SDF\"></font>";
			HuntingLogPerkType huntingLogPerkType;
			if (this.m_controller.SelectedEntry.ActivePerks != null && this.m_controller.SelectedEntry.ActivePerks.TryGetValue(this.m_tier.Count, out huntingLogPerkType))
			{
				this.m_isActive = true;
				this.m_canBeActivated = false;
				string arg4 = string.Empty;
				if (huntingLogPerkType.IsTitle())
				{
					this.m_activeDescription = this.m_profile.GetSelectedTitle(this.m_tier.Count, huntingLogPerkType);
					arg4 = "Title";
				}
				else
				{
					this.m_activeDescription = huntingLogPerkType.GetPerkDescription(this.m_profile.Settings);
					arg4 = " Stat";
				}
				arg = ZString.Format<string, string>("<i><size=80%>{0}:</size></i> {1}", arg4, this.m_activeDescription);
				arg2 = "<alpha=#AA>";
				arg3 = "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
			}
			else
			{
				this.m_isActive = false;
				this.m_canBeActivated = this.CanBeActivated();
				if (this.m_canBeActivated)
				{
					this.m_activateButton.text = "Select " + this.m_tier.Type.ToString();
				}
				arg = ZString.Format<string, string>("<size=60%>{0}</size> <i>{1}</i>", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>", this.m_tier.Type.ToString());
			}
			if (this.m_label)
			{
				this.m_label.SetTextFormat("{0}{1}", arg2, arg);
			}
			if (this.m_count)
			{
				this.m_count.SetTextFormat("{0}{1}", arg2, this.m_tier.Count);
			}
			if (this.m_checkboxLabel)
			{
				this.m_checkboxLabel.ZStringSetText(arg3);
			}
			if (this.m_activateButton)
			{
				this.m_activateButton.gameObject.SetActive(this.m_canBeActivated);
			}
		}

		// Token: 0x06005DC7 RID: 24007 RVA: 0x001F4D2C File Offset: 0x001F2F2C
		private bool CanBeActivated()
		{
			if (this.m_controller == null || this.m_controller.SelectedEntry == null || this.m_tier == null || this.m_profile == null)
			{
				return false;
			}
			int perkCount = this.m_controller.SelectedEntry.PerkCount;
			if (perkCount < this.m_tier.Count)
			{
				return false;
			}
			HuntingLogEntry selectedEntry = this.m_controller.SelectedEntry;
			if (selectedEntry.ActivePerks != null && selectedEntry.ActivePerks.ContainsKey(this.m_tier.Count))
			{
				return false;
			}
			int num = (selectedEntry.ActivePerks == null) ? 0 : selectedEntry.ActivePerks.Count;
			HuntingLogUI.InternalUITier[] uitiers = this.m_controller.UITiers;
			HuntingLogTier huntingLogTier;
			return num < uitiers.Length && uitiers[num].Count <= perkCount && this.m_profile.TryGetTier(uitiers[num].Count, out huntingLogTier) && huntingLogTier.Count == this.m_tier.Count;
		}

		// Token: 0x06005DC8 RID: 24008 RVA: 0x001F4E1C File Offset: 0x001F301C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_tier != null)
			{
				string txt = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					if (this.m_tier.HasDefaultTitle)
					{
						utf16ValueStringBuilder.AppendFormat<string>("<u>Granted Title:</u>\n{0}\n", this.m_tier.DefaultTitle);
						utf16ValueStringBuilder.AppendLine();
					}
					string arg = (this.m_tier.Type == HuntingLogTier.PerkType.Title) ? "Title" : "Stat";
					if (this.m_isActive)
					{
						utf16ValueStringBuilder.AppendFormat<string, string>("<u>Selected {0}:</u>\n{1}\n", arg, this.m_activeDescription);
						utf16ValueStringBuilder.AppendLine();
					}
					utf16ValueStringBuilder.AppendFormat<string, string>("<u>{0} Options:</u>\n{1}", arg, this.m_tier.GetPerks());
					txt = utf16ValueStringBuilder.ToString();
				}
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17001624 RID: 5668
		// (get) Token: 0x06005DC9 RID: 24009 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001625 RID: 5669
		// (get) Token: 0x06005DCA RID: 24010 RVA: 0x0007F147 File Offset: 0x0007D347
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001626 RID: 5670
		// (get) Token: 0x06005DCB RID: 24011 RVA: 0x0007F155 File Offset: 0x0007D355
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005DCC RID: 24012 RVA: 0x001F4EFC File Offset: 0x001F30FC
		string IContextMenu.FillActionsGetTitle()
		{
			if (!this.m_isActive || this.m_profile == null || this.m_tier == null || this.m_controller == null || this.m_controller.RespecConfirmationActive)
			{
				return null;
			}
			ContextMenuUI.AddContextAction("Reset Perk", true, new Action(this.RespecConfirmationCallback), null, null);
			return "Reset Perk";
		}

		// Token: 0x06005DCD RID: 24013 RVA: 0x001F4F64 File Offset: 0x001F3164
		private void RespecConfirmationCallback()
		{
			if (this.m_profile && this.m_tier != null && this.m_controller)
			{
				int nextLowerTierCount = this.m_profile.GetNextLowerTierCount(this.m_tier.Count);
				DialogOptions opts = new DialogOptions
				{
					Title = "Reset Hunting Log Perk",
					Text = ZString.Format<string, int>("Are you sure you want to reset this <b>{0}</b> Hunting Log Perk?\n\nYour Perk Tier will be reset to <b>{1}</b> and all higher tiers will also be reset; you will need to earn these perks again.\n\n<size=80%>(This will NOT reset your total kill count for this creature.)</size>", this.m_profile.TitlePrefix, nextLowerTierCount),
					ConfirmationText = "Yes",
					CancelText = "NO",
					Callback = delegate(bool answer, object result)
					{
						if (answer)
						{
							this.ExecuteRespec();
						}
						if (this.m_controller)
						{
							this.m_controller.RespecConfirmationActive = false;
						}
					}
				};
				ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
				this.m_controller.RespecConfirmationActive = true;
			}
		}

		// Token: 0x06005DCE RID: 24014 RVA: 0x001F5030 File Offset: 0x001F3230
		private void ExecuteRespec()
		{
			if (this.m_profile != null && this.m_tier != null && this.m_controller && this.m_controller.SelectedEntry != null && this.m_controller.SelectedEntry.GetProfile() == this.m_profile)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RespecHuntingLogRequest(this.m_profile.Id, this.m_tier.Count);
			}
		}

		// Token: 0x17001627 RID: 5671
		// (get) Token: 0x06005DCF RID: 24015 RVA: 0x0007F15D File Offset: 0x0007D35D
		CursorType ICursor.Type
		{
			get
			{
				if (!this.m_isActive && !this.m_canBeActivated)
				{
					return CursorType.GloveCursorInactive;
				}
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040050FF RID: 20735
		private const string kRegularAlpha = "<alpha=#FF>";

		// Token: 0x04005100 RID: 20736
		private const string kFadedAlpha = "<alpha=#AA>";

		// Token: 0x04005101 RID: 20737
		public const float kMaxFontSize = 18f;

		// Token: 0x04005102 RID: 20738
		public const float kMinFontSize = 6f;

		// Token: 0x04005103 RID: 20739
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04005104 RID: 20740
		[SerializeField]
		private RectTransform m_rect;

		// Token: 0x04005105 RID: 20741
		[SerializeField]
		private TextMeshProUGUI m_count;

		// Token: 0x04005106 RID: 20742
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04005107 RID: 20743
		[SerializeField]
		private TextMeshProUGUI m_checkboxLabel;

		// Token: 0x04005108 RID: 20744
		[SerializeField]
		private SolButton m_activateButton;

		// Token: 0x04005109 RID: 20745
		private bool m_isActive;

		// Token: 0x0400510A RID: 20746
		private bool m_canBeActivated;

		// Token: 0x0400510B RID: 20747
		private HuntingLogUI m_controller;

		// Token: 0x0400510C RID: 20748
		private HuntingLogProfile m_profile;

		// Token: 0x0400510D RID: 20749
		private HuntingLogTier m_tier;

		// Token: 0x0400510E RID: 20750
		private string m_activeDescription;
	}
}
