using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Selection
{
	// Token: 0x02000B3F RID: 2879
	public class CharacterSelectButton : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x170014BD RID: 5309
		// (get) Token: 0x06005877 RID: 22647 RVA: 0x0007B1E4 File Offset: 0x000793E4
		// (set) Token: 0x06005878 RID: 22648 RVA: 0x0007B1EC File Offset: 0x000793EC
		public IHighlight SelectableCharHighlight { get; set; }

		// Token: 0x06005879 RID: 22649 RVA: 0x001E60F0 File Offset: 0x001E42F0
		private void Awake()
		{
			this.m_selectButton.onClick.AddListener(new UnityAction(this.OnCharacterClicked));
			if (this.m_deleteButton)
			{
				this.m_deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteClicked));
			}
			if (this.m_leftButton)
			{
				this.m_leftButton.onClick.AddListener(new UnityAction(this.LeftButtonClicked));
			}
			if (this.m_rightButton)
			{
				this.m_rightButton.onClick.AddListener(new UnityAction(this.RightButtonClicked));
			}
			if (this.m_highlight)
			{
				this.m_highlight.enabled = false;
				this.m_highlight.gameObject.SetActive(true);
			}
			if (this.m_buttonPanel)
			{
				this.m_buttonPanel.SetActive(false);
			}
			if (this.m_renameButton)
			{
				this.m_renameButton.interactable = false;
				this.m_renameButton.onClick.AddListener(new UnityAction(this.RenameButtonClicked));
			}
			if (this.m_activateCharacterButton)
			{
				this.m_activateCharacterButton.onClick.AddListener(new UnityAction(this.ActivateCharacterClicked));
			}
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x001E6238 File Offset: 0x001E4438
		private void OnDestroy()
		{
			this.m_selectButton.onClick.RemoveAllListeners();
			if (this.m_deleteButton)
			{
				this.m_deleteButton.onClick.RemoveAllListeners();
			}
			if (this.m_leftButton)
			{
				this.m_leftButton.onClick.RemoveAllListeners();
			}
			if (this.m_rightButton)
			{
				this.m_rightButton.onClick.RemoveAllListeners();
			}
			if (this.m_renameButton)
			{
				this.m_renameButton.onClick.RemoveListener(new UnityAction(this.RenameButtonClicked));
			}
			if (this.m_activateCharacterButton)
			{
				this.m_activateCharacterButton.onClick.RemoveListener(new UnityAction(this.ActivateCharacterClicked));
			}
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x0007B1F5 File Offset: 0x000793F5
		private void ToggleObject(bool isActive)
		{
			if (this.m_toToggle)
			{
				this.m_toToggle.SetActive(isActive);
				return;
			}
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600587C RID: 22652 RVA: 0x0007B21D File Offset: 0x0007941D
		public void Init(SelectionDirector director, int index, GameObject parent)
		{
			if (this.m_uiToWorldSpace)
			{
				this.m_uiToWorldSpace.Parent = parent;
			}
			this.m_director = director;
			this.m_positionIndex = index;
			this.ToggleObject(false);
			this.RefreshPortrait();
			this.RefreshRoleIconAndLevel();
		}

		// Token: 0x0600587D RID: 22653 RVA: 0x001E6300 File Offset: 0x001E4500
		public void SetCharacter(CharacterRecord record)
		{
			this.m_record = record;
			this.RefreshPortrait();
			this.RefreshRoleIconAndLevel();
			this.RefreshTooltipString();
			if (this.m_record == null)
			{
				this.SelectableCharHighlight = null;
				this.ToggleObject(false);
				return;
			}
			this.ConfigUIForText(this.m_title, record.Title);
			this.RefreshName();
			this.ConfigUIForText(this.m_zone, record.Location.Zone.DisplayName);
			this.m_corpseIcon.SetActive(record.Corpse != null);
			this.ToggleObject(true);
			this.RefreshInactiveRenamePanel();
			this.RefreshVisibleCharacterLowerLabel();
			this.RefreshUpperSeparator();
		}

		// Token: 0x0600587E RID: 22654 RVA: 0x001E63A0 File Offset: 0x001E45A0
		private void RefreshInactiveRenamePanel()
		{
			if (this.m_record == null || !this.m_inactivePanel)
			{
				return;
			}
			bool flag = SessionData.CharacterIsActive(this.m_record);
			bool flag2 = this.m_record.SelectionPositionIndex >= 0;
			bool flag3 = this.m_record.RequiresRenaming != null && this.m_record.RequiresRenaming.Value;
			if (this.m_canvasGroup)
			{
				this.m_canvasGroup.alpha = (flag ? 1f : 0.5f);
			}
			if (this.m_notVisibleIcon)
			{
				this.m_notVisibleIcon.enabled = !flag2;
			}
			if (!flag || !flag2 || flag3)
			{
				if (this.m_layoutElement)
				{
					this.m_layoutElement.preferredHeight = 92f;
				}
				if (this.m_inactivePanelLabel)
				{
					this.m_inactivePanelLabel.enabled = !flag;
				}
				if (flag3 && !SessionData.CharacterCanBeSetAsActive(this.m_record))
				{
					this.m_renameButton.interactable = true;
					this.m_renameButton.gameObject.SetActive(true);
				}
				else
				{
					this.m_renameButton.interactable = false;
					this.m_renameButton.gameObject.SetActive(false);
				}
				this.m_inactivePanel.SetActive(true);
				return;
			}
			if (this.m_layoutElement)
			{
				this.m_layoutElement.preferredHeight = 72f;
			}
			this.m_inactivePanel.SetActive(false);
		}

		// Token: 0x0600587F RID: 22655 RVA: 0x001E6524 File Offset: 0x001E4724
		private void RefreshUpperSeparator()
		{
			if (!this.m_upperSeparator)
			{
				return;
			}
			if (this.m_record == null || SessionData.Characters == null || (this.m_inactivePanel && !this.m_inactivePanel.activeSelf))
			{
				this.m_upperSeparator.SetActive(false);
				return;
			}
			for (int i = 0; i < SessionData.Characters.Length; i++)
			{
				if (SessionData.Characters[i].SelectionPositionIndex < 0)
				{
					this.m_upperSeparator.SetActive(SessionData.Characters[i].Id == this.m_record.Id);
					return;
				}
			}
		}

		// Token: 0x06005880 RID: 22656 RVA: 0x001E65C0 File Offset: 0x001E47C0
		private void RefreshVisibleCharacterLowerLabel()
		{
			if (this.m_record == null)
			{
				return;
			}
			if (this.m_visibleInactivePanel)
			{
				this.m_visibleInactivePanel.SetActive(!SessionData.CharacterIsActive(this.m_record));
			}
			if (this.m_activateCharacterButton)
			{
				this.m_activateCharacterButton.gameObject.SetActive(SessionData.CharacterCanBeSetAsActive(this.m_record));
			}
		}

		// Token: 0x06005881 RID: 22657 RVA: 0x001E6624 File Offset: 0x001E4824
		public void RefreshPortrait()
		{
			Sprite overrideSprite;
			if (this.m_record != null && !this.m_record.Settings.PortraitId.IsEmpty && GlobalSettings.Values && GlobalSettings.Values.Portraits != null && GlobalSettings.Values.Portraits.AllPortraits && GlobalSettings.Values.Portraits.AllPortraits.TryGetObject(this.m_record.Settings.PortraitId, out overrideSprite))
			{
				this.m_portraitImage.overrideSprite = overrideSprite;
				this.m_portraitImage.enabled = true;
				this.m_emptyUserIcon.enabled = ((GlobalSettings.Values.Portraits.PlayerPortraits && this.m_record.Settings.PortraitId.IsEmpty) || this.m_record.Settings.PortraitId == GlobalSettings.Values.Portraits.PlayerPortraits.GetEntryIdByIndex(0));
				return;
			}
			this.m_portraitImage.enabled = false;
			this.m_portraitImage.overrideSprite = null;
			this.m_emptyUserIcon.enabled = true;
		}

		// Token: 0x06005882 RID: 22658 RVA: 0x0007B259 File Offset: 0x00079459
		public void RefreshName()
		{
			if (this.m_record != null)
			{
				this.ConfigUIForText(this.m_name, this.m_record.Name);
			}
			this.RefreshRenamePanel();
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x001E6758 File Offset: 0x001E4958
		private void ActivateCharacterClicked()
		{
			if (this.m_record != null && this.m_director != null && this.m_activateCharacterButton.gameObject.activeSelf && SessionData.CharacterCanBeSetAsActive(this.m_record))
			{
				this.m_director.OpenSetPrimaryDialog();
			}
		}

		// Token: 0x06005884 RID: 22660 RVA: 0x0007B280 File Offset: 0x00079480
		private void RenameButtonClicked()
		{
			if (this.m_record != null && SelectionDirector.Instance)
			{
				SelectionDirector.Instance.RenameClicked(this.m_record);
			}
		}

		// Token: 0x06005885 RID: 22661 RVA: 0x0007B2A6 File Offset: 0x000794A6
		private void RefreshRenamePanel()
		{
			this.RefreshInactiveRenamePanel();
			this.RefreshVisibleCharacterLowerLabel();
			this.RefreshUpperSeparator();
		}

		// Token: 0x06005886 RID: 22662 RVA: 0x001E67A8 File Offset: 0x001E49A8
		private void RefreshRoleIconAndLevel()
		{
			CharacterRecordExtensions.LoginRoleData loginRoleData;
			if (this.m_record != null && this.m_record.TryGetLoginRoleData(out loginRoleData))
			{
				this.m_loginRoleData = new CharacterRecordExtensions.LoginRoleData?(loginRoleData);
				this.m_roleIcon.overrideSprite = loginRoleData.Icon;
				this.m_roleIcon.color = loginRoleData.IconTint;
				if (this.m_abbreviatedRoleLevel)
				{
					this.m_levelLabel.SetTextFormat("{0}", loginRoleData.Level);
				}
				else
				{
					this.m_levelLabel.SetTextFormat("Level {0} {1}", loginRoleData.Level, loginRoleData.Name);
				}
				this.m_levelLabel.enabled = true;
				this.m_roleIcon.enabled = true;
				return;
			}
			this.m_loginRoleData = null;
			this.m_roleIcon.enabled = false;
			this.m_levelLabel.enabled = false;
		}

		// Token: 0x06005887 RID: 22663 RVA: 0x001E687C File Offset: 0x001E4A7C
		private void RefreshTooltipString()
		{
			this.m_tooltipString = string.Empty;
			ContainerRecord containerRecord;
			if (this.m_record != null && this.m_record.Storage != null && this.m_record.Storage.TryGetValue(ContainerType.Masteries, out containerRecord) && containerRecord.Instances != null && containerRecord.Instances.Count > 0)
			{
				if (this.m_roleInstanceData == null)
				{
					this.m_roleInstanceData = new List<CharacterSelectButton.RoleInstanceData>(5);
				}
				else
				{
					this.m_roleInstanceData.Clear();
				}
				for (int i = 0; i < containerRecord.Instances.Count; i++)
				{
					ArchetypeInstance archetypeInstance = containerRecord.Instances[i];
					BaseRole baseRole;
					if (archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out baseRole) && (baseRole.Type == MasteryType.Trade || baseRole.Type == MasteryType.Harvesting))
					{
						this.m_roleInstanceData.Add(new CharacterSelectButton.RoleInstanceData
						{
							Name = baseRole.DisplayName,
							Level = Mathf.FloorToInt(archetypeInstance.MasteryData.BaseLevel),
							Type = baseRole.Type
						});
					}
				}
				if (this.m_roleInstanceData.Count > 0)
				{
					this.m_roleInstanceData.Sort(delegate(CharacterSelectButton.RoleInstanceData a, CharacterSelectButton.RoleInstanceData b)
					{
						if (a.Type == MasteryType.Combat && b.Type == MasteryType.Combat)
						{
							return b.Level.CompareTo(a.Level);
						}
						if (a.Type == MasteryType.Combat)
						{
							return -1;
						}
						if (b.Type == MasteryType.Combat)
						{
							return 1;
						}
						return b.Level.CompareTo(a.Level);
					});
					using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
					{
						for (int j = 0; j < this.m_roleInstanceData.Count; j++)
						{
							utf16ValueStringBuilder.AppendFormat<int, string>("{0} {1}", this.m_roleInstanceData[j].Level, this.m_roleInstanceData[j].Name);
							if (j < this.m_roleInstanceData.Count - 1)
							{
								utf16ValueStringBuilder.AppendLine();
							}
						}
						this.m_tooltipString = utf16ValueStringBuilder.ToString();
					}
				}
			}
		}

		// Token: 0x06005888 RID: 22664 RVA: 0x0007B2BA File Offset: 0x000794BA
		private void ConfigUIForText(TextMeshProUGUI ui, string txt)
		{
			if (ui == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(txt))
			{
				ui.gameObject.SetActive(false);
				return;
			}
			ui.text = txt;
			ui.gameObject.SetActive(true);
		}

		// Token: 0x06005889 RID: 22665 RVA: 0x0007B2EE File Offset: 0x000794EE
		private void OnDeleteClicked()
		{
			if (this.m_record == null)
			{
				return;
			}
			this.m_director.DeleteCharacterRequest(this.m_record);
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x0007B30A File Offset: 0x0007950A
		private void OnCharacterClicked()
		{
			this.m_director.SelectCharacterByCharacterRecord(this.m_record);
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x001E6A7C File Offset: 0x001E4C7C
		public void SetSelected(bool selected)
		{
			this.m_selectButton.interactable = !selected;
			if (this.m_buttonBackground)
			{
				if (this.m_buttonBackgroundAlpha == null)
				{
					this.m_buttonBackgroundAlpha = new float?(this.m_buttonBackground.color.a);
				}
				Color color = this.m_buttonBackground.color;
				color.a = (selected ? 1f : this.m_buttonBackgroundAlpha.Value);
				this.m_buttonBackground.color = color;
			}
			if (this.m_highlight)
			{
				this.m_highlight.enabled = selected;
			}
			if (selected && this.m_director && this.m_director.SelectionStage)
			{
				this.m_director.SelectionStage.ParentEnterWorldButton(this.m_enterWorldParent, this.m_record);
			}
			this.RefreshRenamePanel();
		}

		// Token: 0x0600588C RID: 22668 RVA: 0x0007B31E File Offset: 0x0007951E
		private void LeftButtonClicked()
		{
			this.m_director.MoveCharacterLeft(this.m_record);
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x0007B331 File Offset: 0x00079531
		private void RightButtonClicked()
		{
			this.m_director.MoveCharacterRight(this.m_record);
		}

		// Token: 0x0600588E RID: 22670 RVA: 0x0007B344 File Offset: 0x00079544
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.SelectableCharHighlight != null)
			{
				this.SelectableCharHighlight.HighlightEnabled = true;
			}
		}

		// Token: 0x0600588F RID: 22671 RVA: 0x0007B35A File Offset: 0x0007955A
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.SelectableCharHighlight != null)
			{
				this.SelectableCharHighlight.HighlightEnabled = false;
			}
		}

		// Token: 0x06005890 RID: 22672 RVA: 0x0007B370 File Offset: 0x00079570
		public bool MatchesId(string id)
		{
			return this.m_record != null && this.m_record.Id.Equals(id);
		}

		// Token: 0x06005891 RID: 22673 RVA: 0x0007B38D File Offset: 0x0007958D
		public bool MatchesName(string characterName)
		{
			return this.m_record != null && this.m_record.Name.Equals(characterName);
		}

		// Token: 0x170014BE RID: 5310
		// (get) Token: 0x06005892 RID: 22674 RVA: 0x0007B3AA File Offset: 0x000795AA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x0007B3B2 File Offset: 0x000795B2
		string IContextMenu.FillActionsGetTitle()
		{
			if (this.m_record == null || this.m_director == null)
			{
				return null;
			}
			return this.m_director.ContextMenuForCharacter(this.m_record, this.m_uiToWorldSpace != null);
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x0007B3E9 File Offset: 0x000795E9
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_record != null && !string.IsNullOrEmpty(this.m_tooltipString))
			{
				return new ObjectTextTooltipParameter(this, this.m_tooltipString, true);
			}
			return null;
		}

		// Token: 0x170014BF RID: 5311
		// (get) Token: 0x06005895 RID: 22677 RVA: 0x0007B414 File Offset: 0x00079614
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170014C0 RID: 5312
		// (get) Token: 0x06005896 RID: 22678 RVA: 0x0007B422 File Offset: 0x00079622
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004DDA RID: 19930
		[SerializeField]
		private Vector3 m_positionOffset = Vector3.zero;

		// Token: 0x04004DDB RID: 19931
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x04004DDC RID: 19932
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04004DDD RID: 19933
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04004DDE RID: 19934
		[SerializeField]
		private TextMeshProUGUI m_zone;

		// Token: 0x04004DDF RID: 19935
		[SerializeField]
		private Button m_selectButton;

		// Token: 0x04004DE0 RID: 19936
		[SerializeField]
		private Button m_deleteButton;

		// Token: 0x04004DE1 RID: 19937
		[SerializeField]
		private GameObject m_enterWorldParent;

		// Token: 0x04004DE2 RID: 19938
		[SerializeField]
		private GameObject m_inactivePanel;

		// Token: 0x04004DE3 RID: 19939
		[SerializeField]
		private TextMeshProUGUI m_inactivePanelLabel;

		// Token: 0x04004DE4 RID: 19940
		[SerializeField]
		private GameObject m_corpseIcon;

		// Token: 0x04004DE5 RID: 19941
		[SerializeField]
		private Image m_portraitImage;

		// Token: 0x04004DE6 RID: 19942
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04004DE7 RID: 19943
		[SerializeField]
		private SolButton m_leftButton;

		// Token: 0x04004DE8 RID: 19944
		[SerializeField]
		private SolButton m_rightButton;

		// Token: 0x04004DE9 RID: 19945
		[SerializeField]
		private UIToWorldSpace m_uiToWorldSpace;

		// Token: 0x04004DEA RID: 19946
		[SerializeField]
		private Image m_roleIcon;

		// Token: 0x04004DEB RID: 19947
		[SerializeField]
		private TextMeshProUGUI m_levelLabel;

		// Token: 0x04004DEC RID: 19948
		[SerializeField]
		private GameObject m_buttonPanel;

		// Token: 0x04004DED RID: 19949
		[SerializeField]
		private SolButton m_renameButton;

		// Token: 0x04004DEE RID: 19950
		[SerializeField]
		private Image m_emptyUserIcon;

		// Token: 0x04004DEF RID: 19951
		[SerializeField]
		private Image m_notVisibleIcon;

		// Token: 0x04004DF0 RID: 19952
		[SerializeField]
		private Image m_buttonBackground;

		// Token: 0x04004DF1 RID: 19953
		[SerializeField]
		private GameObject m_toToggle;

		// Token: 0x04004DF2 RID: 19954
		[SerializeField]
		private LayoutElement m_layoutElement;

		// Token: 0x04004DF3 RID: 19955
		[SerializeField]
		private bool m_abbreviatedRoleLevel;

		// Token: 0x04004DF4 RID: 19956
		[SerializeField]
		private GameObject m_visibleInactivePanel;

		// Token: 0x04004DF5 RID: 19957
		[SerializeField]
		private SolButton m_activateCharacterButton;

		// Token: 0x04004DF6 RID: 19958
		[SerializeField]
		private GameObject m_upperSeparator;

		// Token: 0x04004DF8 RID: 19960
		private int m_positionIndex = -1;

		// Token: 0x04004DF9 RID: 19961
		private SelectionDirector m_director;

		// Token: 0x04004DFA RID: 19962
		private CharacterRecord m_record;

		// Token: 0x04004DFB RID: 19963
		private const float kDefaultPreferredHeight = 72f;

		// Token: 0x04004DFC RID: 19964
		private const float kExpandedPreferredHeight = 92f;

		// Token: 0x04004DFD RID: 19965
		private float? m_buttonBackgroundAlpha;

		// Token: 0x04004DFE RID: 19966
		private List<CharacterSelectButton.RoleInstanceData> m_roleInstanceData;

		// Token: 0x04004DFF RID: 19967
		private CharacterRecordExtensions.LoginRoleData? m_loginRoleData;

		// Token: 0x04004E00 RID: 19968
		private string m_tooltipString = string.Empty;

		// Token: 0x04004E01 RID: 19969
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04004E02 RID: 19970
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x02000B40 RID: 2880
		private struct RoleInstanceData
		{
			// Token: 0x04004E03 RID: 19971
			public string Name;

			// Token: 0x04004E04 RID: 19972
			public int Level;

			// Token: 0x04004E05 RID: 19973
			public MasteryType Type;
		}
	}
}
