using System;
using System.Collections.Generic;
using SoL.Game.Login.Client.Selection;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x020005A4 RID: 1444
	public class SelectPortraitWindow : MonoBehaviour
	{
		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x06002D41 RID: 11585 RVA: 0x0005F6F0 File Offset: 0x0005D8F0
		public bool Visible
		{
			get
			{
				return this.m_window != null && this.m_window.Visible;
			}
		}

		// Token: 0x06002D42 RID: 11586 RVA: 0x0014D4C4 File Offset: 0x0014B6C4
		private void Awake()
		{
			this.m_toggleGroup.allowSwitchOff = false;
			this.m_confirmButton.onClick.AddListener(new UnityAction(this.ConfirmClicked));
			this.m_window.WindowClosed += this.WindowHidden;
		}

		// Token: 0x06002D43 RID: 11587 RVA: 0x0005F70D File Offset: 0x0005D90D
		private void OnDestroy()
		{
			this.m_confirmButton.onClick.RemoveListener(new UnityAction(this.ConfirmClicked));
			this.m_window.WindowClosed -= this.WindowHidden;
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x0005F742 File Offset: 0x0005D942
		public void Init(CharacterRecord record)
		{
			this.Init(record, record.Name);
		}

		// Token: 0x06002D45 RID: 11589 RVA: 0x0014D510 File Offset: 0x0014B710
		public void Init(CharacterRecord record, string characterName)
		{
			if (record == null || GlobalSettings.Values == null || GlobalSettings.Values.Portraits == null || GlobalSettings.Values.Portraits.PlayerPortraits == null)
			{
				this.WindowHidden();
				return;
			}
			this.m_recordToAlter = record;
			this.m_label.SetText("Select Portrait for " + characterName);
			this.m_toggleGroup.SetAllTogglesOff(true);
			int num = 0;
			foreach (IdentifiableSprite identifiableSprite in GlobalSettings.Values.Portraits.PlayerPortraits.GetObjects())
			{
				bool isSelected = identifiableSprite.Id == record.Settings.PortraitId.Value;
				if (num < this.m_items.Count)
				{
					this.m_items[num].Init(identifiableSprite, isSelected, this.m_toggleGroup);
					this.m_items[num].gameObject.SetActive(true);
				}
				else
				{
					PortraitSelectionItem component = UnityEngine.Object.Instantiate<GameObject>(this.m_prefab, this.m_parent).GetComponent<PortraitSelectionItem>();
					component.Init(identifiableSprite, isSelected, this.m_toggleGroup);
					this.m_items.Add(component);
				}
				num++;
			}
			if (num < this.m_items.Count)
			{
				for (int i = num; i < this.m_items.Count; i++)
				{
					this.m_items[i].gameObject.SetActive(false);
				}
			}
			this.m_confirmButton.interactable = true;
			this.m_window.Show(false);
		}

		// Token: 0x06002D46 RID: 11590 RVA: 0x0014D6C0 File Offset: 0x0014B8C0
		private void ConfirmClicked()
		{
			if (this.m_recordToAlter == null)
			{
				this.m_window.Hide(false);
				return;
			}
			this.m_confirmButton.interactable = false;
			int i = 0;
			while (i < this.m_items.Count)
			{
				if (this.m_items[i].IsOn)
				{
					if (this.m_items[i].SpriteId.IsEmpty || !(this.m_items[i].SpriteId != this.m_recordToAlter.Settings.PortraitId.Value))
					{
						break;
					}
					if (LocalPlayer.GameEntity != null)
					{
						LocalPlayer.NetworkEntity.PlayerRpcHandler.ChangePortraitRequest(this.m_items[i].SpriteId);
						break;
					}
					if (SelectionDirector.Instance != null && SelectionDirector.Instance.IsStageActive)
					{
						SelectionDirector.Instance.SetCharacterPortraitId(this.m_recordToAlter, this.m_items[i].SpriteId);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.m_window.Hide(false);
		}

		// Token: 0x06002D47 RID: 11591 RVA: 0x0005F751 File Offset: 0x0005D951
		private void WindowHidden()
		{
			this.m_recordToAlter = null;
			this.m_label.SetText("Select Portrait");
		}

		// Token: 0x06002D48 RID: 11592 RVA: 0x0005F76A File Offset: 0x0005D96A
		public void HideWindow()
		{
			this.m_window.Hide(false);
		}

		// Token: 0x04002CD3 RID: 11475
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04002CD4 RID: 11476
		[SerializeField]
		private GameObject m_prefab;

		// Token: 0x04002CD5 RID: 11477
		[SerializeField]
		private RectTransform m_parent;

		// Token: 0x04002CD6 RID: 11478
		[SerializeField]
		private SolButton m_confirmButton;

		// Token: 0x04002CD7 RID: 11479
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x04002CD8 RID: 11480
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04002CD9 RID: 11481
		private readonly List<PortraitSelectionItem> m_items = new List<PortraitSelectionItem>();

		// Token: 0x04002CDA RID: 11482
		private CharacterRecord m_recordToAlter;

		// Token: 0x04002CDB RID: 11483
		private const string kBaseLabel = "Select Portrait";
	}
}
