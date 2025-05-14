using System;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x0200090B RID: 2315
	public class LfgLfmListItem : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x06004401 RID: 17409 RVA: 0x0006DF28 File Offset: 0x0006C128
		public void Init(LookingFor lookingFor)
		{
			if (this.m_defaultFrameColor == null)
			{
				this.m_defaultFrameColor = new Color?(this.m_frame.color);
			}
			this.m_lookingEntry = lookingFor;
			this.Refresh();
		}

		// Token: 0x06004402 RID: 17410 RVA: 0x00199064 File Offset: 0x00197264
		public void Refresh()
		{
			string text = (this.m_lookingEntry.Type == LookingType.Lfm) ? (this.m_lookingEntry.ContactName + "'s Group") : this.m_lookingEntry.ContactName;
			if ((this.m_lookingEntry.MinLevel == 0 && this.m_lookingEntry.MaxLevel == 0) || (this.m_lookingEntry.MinLevel == 1 && this.m_lookingEntry.MaxLevel == 50))
			{
				this.m_topLine.text = text + " (Any Level)";
			}
			else if (this.m_lookingEntry.MinLevel == this.m_lookingEntry.MaxLevel)
			{
				this.m_topLine.text = string.Format("{0} ({1})", text, this.m_lookingEntry.MaxLevel);
			}
			else
			{
				this.m_topLine.text = string.Format("{0} ({1}-{2})", text, this.m_lookingEntry.MinLevel, this.m_lookingEntry.MaxLevel);
			}
			string text2 = this.m_lookingEntry.Tags.ToStringWithSpaces();
			this.m_bottomLine.text = ((text2 == "None") ? "No tags provided" : text2);
			this.m_backgroundImage.color = ((this.m_lookingEntry.Type == LookingType.Lfm) ? this.m_groupedPanelColor : this.m_ungroupedPanelColor);
			this.m_borderImage.color = ((this.m_lookingEntry.Type == LookingType.Lfm) ? Colors.Aqua : this.m_ungroupedBorderColor);
			this.m_groupIcon.gameObject.SetActive(this.m_lookingEntry.Type == LookingType.Lfm);
			this.m_individualIcon.gameObject.SetActive(this.m_lookingEntry.Type == LookingType.Lfg);
			this.m_noteIcon.Text = this.m_lookingEntry.Note;
			GameObject gameObject = this.m_noteIcon.gameObject;
			string note = this.m_lookingEntry.Note;
			gameObject.SetActive(!string.IsNullOrWhiteSpace((note != null) ? note.Replace("<noparse>", "").Replace("</noparse>", "") : null));
			this.m_frame.color = ((this.m_lookingEntry.Type == LookingType.Lfm) ? this.m_groupedBorderColor : this.m_defaultFrameColor.Value);
		}

		// Token: 0x06004403 RID: 17411 RVA: 0x0006DF5A File Offset: 0x0006C15A
		private void SendTell()
		{
			UIManager.ActiveChatInput.StartWhisper(this.m_lookingEntry.ContactName);
		}

		// Token: 0x06004404 RID: 17412 RVA: 0x001992B0 File Offset: 0x001974B0
		private void InviteToGroup()
		{
			if (this.m_lookingEntry.Type != LookingType.Lfg)
			{
				return;
			}
			if (ClientGameManager.GroupManager.IsGrouped && (!ClientGameManager.GroupManager.IsLeader || ClientGameManager.GroupManager.GroupIsFull))
			{
				return;
			}
			ClientGameManager.GroupManager.InviteNewMember(this.m_lookingEntry.ContactName);
		}

		// Token: 0x06004405 RID: 17413 RVA: 0x00199308 File Offset: 0x00197508
		string IContextMenu.FillActionsGetTitle()
		{
			if (this.m_lookingEntry.ContactName == LocalPlayer.GameEntity.CharacterData.Name)
			{
				return null;
			}
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Send Tell", true, new Action(this.SendTell), null, null);
			if (this.m_lookingEntry.Type == LookingType.Lfg)
			{
				bool flag = false;
				string text = "Invite to group";
				if (ClientGameManager.GroupManager.IsGrouped)
				{
					if (ClientGameManager.GroupManager.IsLeader)
					{
						flag = !ClientGameManager.GroupManager.GroupIsFull;
						if (!flag)
						{
							text += " (Full)";
						}
					}
					else
					{
						text += " (Not Leader)";
					}
				}
				else
				{
					flag = true;
				}
				ContextMenuUI.AddContextAction(text, flag, new Action(this.InviteToGroup), null, null);
			}
			return this.m_lookingEntry.ContactName;
		}

		// Token: 0x17000F55 RID: 3925
		// (get) Token: 0x06004406 RID: 17414 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004407 RID: 17415 RVA: 0x0006DF71 File Offset: 0x0006C171
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.m_lookingEntry.Tags.ToStringWithSpaces(), false);
		}

		// Token: 0x17000F56 RID: 3926
		// (get) Token: 0x06004408 RID: 17416 RVA: 0x0006DF94 File Offset: 0x0006C194
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F57 RID: 3927
		// (get) Token: 0x06004409 RID: 17417 RVA: 0x0006DFA2 File Offset: 0x0006C1A2
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600440B RID: 17419 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400407C RID: 16508
		[SerializeField]
		private TextMeshProUGUI m_topLine;

		// Token: 0x0400407D RID: 16509
		[SerializeField]
		private TextMeshProUGUI m_bottomLine;

		// Token: 0x0400407E RID: 16510
		[SerializeField]
		private Image m_backgroundImage;

		// Token: 0x0400407F RID: 16511
		[SerializeField]
		private Image m_borderImage;

		// Token: 0x04004080 RID: 16512
		[SerializeField]
		private Image m_groupIcon;

		// Token: 0x04004081 RID: 16513
		[SerializeField]
		private Image m_individualIcon;

		// Token: 0x04004082 RID: 16514
		[SerializeField]
		private TextTooltipTrigger m_noteIcon;

		// Token: 0x04004083 RID: 16515
		[SerializeField]
		private Image m_frame;

		// Token: 0x04004084 RID: 16516
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004085 RID: 16517
		private readonly Color m_ungroupedBorderColor = new Color32(43, 43, 43, byte.MaxValue);

		// Token: 0x04004086 RID: 16518
		private readonly Color m_groupedBorderColor = Colors.Aqua;

		// Token: 0x04004087 RID: 16519
		private readonly Color m_ungroupedPanelColor = new Color32(byte.MaxValue, 141, 81, byte.MaxValue);

		// Token: 0x04004088 RID: 16520
		private readonly Color m_groupedPanelColor = new Color32(33, 98, byte.MaxValue, byte.MaxValue);

		// Token: 0x04004089 RID: 16521
		private LookingFor m_lookingEntry;

		// Token: 0x0400408A RID: 16522
		private Color? m_defaultFrameColor;
	}
}
