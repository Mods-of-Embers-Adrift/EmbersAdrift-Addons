using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD8 RID: 3032
	public class HuntingLogPerkSelectionUI : MonoBehaviour
	{
		// Token: 0x17001622 RID: 5666
		// (get) Token: 0x06005DB4 RID: 23988 RVA: 0x0007F046 File Offset: 0x0007D246
		// (set) Token: 0x06005DB5 RID: 23989 RVA: 0x0007F04E File Offset: 0x0007D24E
		public HuntingLogUI Controller { get; internal set; }

		// Token: 0x06005DB6 RID: 23990 RVA: 0x001F463C File Offset: 0x001F283C
		private void Awake()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].isOn = false;
				this.m_toggles[i].group = this.m_toggleGroup;
				this.m_toggles[i].onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			}
			this.m_okButton.onClick.AddListener(new UnityAction(this.OkClicked));
			this.m_cancelButton.onClick.AddListener(new UnityAction(this.CancelClicked));
		}

		// Token: 0x06005DB7 RID: 23991 RVA: 0x001F46D4 File Offset: 0x001F28D4
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			}
			this.m_okButton.onClick.RemoveListener(new UnityAction(this.OkClicked));
			this.m_cancelButton.onClick.RemoveListener(new UnityAction(this.CancelClicked));
		}

		// Token: 0x06005DB8 RID: 23992 RVA: 0x001F474C File Offset: 0x001F294C
		private void ToggleChanged(bool arg0)
		{
			int num = 0;
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].isOn)
				{
					num++;
				}
			}
			this.m_okButton.interactable = (num > 0);
		}

		// Token: 0x06005DB9 RID: 23993 RVA: 0x001F4790 File Offset: 0x001F2990
		private void OkClicked()
		{
			int num = -1;
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].isOn)
				{
					num = i;
				}
				this.m_toggles[i].interactable = false;
			}
			this.m_okButton.interactable = false;
			HuntingLogPerkType perkType = this.m_perkTypes[num];
			if (this.Controller.SelectedEntry != null)
			{
				HuntingLogProfile profile = this.Controller.SelectedEntry.GetProfile();
				if (profile != null && profile.IsValidPerk(this.Controller.SelectedEntry, perkType, this.m_currentTier.Count))
				{
					Debug.Log(string.Format("Index {0} selected!  {1}  {2}", num, this.m_toggles[num].text, perkType.ToString()));
					LocalPlayer.NetworkEntity.PlayerRpcHandler.SelectHuntingLogPerk(profile.Id, perkType, this.m_currentTier.Count);
					base.gameObject.SetActive(false);
				}
				else
				{
					this.ResetInteractives();
				}
			}
			else
			{
				this.ResetInteractives();
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005DBA RID: 23994 RVA: 0x0004FE03 File Offset: 0x0004E003
		private void CancelClicked()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005DBB RID: 23995 RVA: 0x001F48AC File Offset: 0x001F2AAC
		private void ResetInteractives()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].isOn = false;
				this.m_toggles[i].interactable = true;
			}
			this.m_okButton.interactable = false;
		}

		// Token: 0x06005DBC RID: 23996 RVA: 0x001F48F4 File Offset: 0x001F2AF4
		internal void RefreshPerks(HuntingLogUI controller, HuntingLogEntry entry)
		{
			if (controller == null || controller.UITiers == null || entry == null)
			{
				return;
			}
			HuntingLogProfile profile = entry.GetProfile();
			if (profile == null)
			{
				return;
			}
			HuntingLogUI.InternalUITier[] uitiers = controller.UITiers;
			if (entry.PerkCount < uitiers[0].Count)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.ResetInteractives();
			this.m_currentTier = null;
			this.m_perkTypes.Clear();
			int num = (entry.ActivePerks == null) ? 0 : entry.ActivePerks.Count;
			HuntingLogTier huntingLogTier;
			if (num < uitiers.Length && uitiers[num].Count <= entry.PerkCount && profile.TryGetTier(uitiers[num].Count, out huntingLogTier))
			{
				this.m_currentTier = huntingLogTier;
				this.m_killLabel.SetTextFormat("{0} Kills", uitiers[num].Count);
				HuntingLogTier.PerkType type = huntingLogTier.Type;
				if (type != HuntingLogTier.PerkType.Title)
				{
					if (type == HuntingLogTier.PerkType.Stat)
					{
						for (int i = 0; i < this.m_toggles.Length; i++)
						{
							if (i < profile.StatPerks.Length)
							{
								this.m_toggles[i].text = ZString.Format<string>("Stat: {0}", profile.StatPerks[i].GetPerkDescription(profile.Settings));
								this.m_toggles[i].gameObject.SetActive(true);
								this.m_perkTypes.Add(profile.StatPerks[i]);
							}
							else
							{
								this.m_toggles[i].gameObject.SetActive(false);
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < this.m_toggles.Length; j++)
					{
						if (j < huntingLogTier.PerkTitles.Length)
						{
							this.m_toggles[j].text = ZString.Format<string>("Title: {0}", huntingLogTier.PerkTitles[j]);
							this.m_toggles[j].gameObject.SetActive(true);
							this.m_perkTypes.Add(HuntingLogExtensions.GetTitlePerkType(j));
						}
						else
						{
							this.m_toggles[j].gameObject.SetActive(false);
						}
					}
				}
				base.gameObject.SetActive(true);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x040050F6 RID: 20726
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x040050F7 RID: 20727
		[SerializeField]
		private TextMeshProUGUI m_killLabel;

		// Token: 0x040050F8 RID: 20728
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x040050F9 RID: 20729
		[SerializeField]
		private SolToggle[] m_toggles;

		// Token: 0x040050FA RID: 20730
		[SerializeField]
		private SolButton m_okButton;

		// Token: 0x040050FB RID: 20731
		[SerializeField]
		private SolButton m_cancelButton;

		// Token: 0x040050FD RID: 20733
		private readonly List<HuntingLogPerkType> m_perkTypes = new List<HuntingLogPerkType>(10);

		// Token: 0x040050FE RID: 20734
		private HuntingLogTier m_currentTier;
	}
}
