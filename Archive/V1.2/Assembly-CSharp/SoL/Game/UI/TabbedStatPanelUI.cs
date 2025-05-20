using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008DC RID: 2268
	public class TabbedStatPanelUI : MonoBehaviour
	{
		// Token: 0x0600424D RID: 16973 RVA: 0x001923D4 File Offset: 0x001905D4
		internal static string GetSelectedTabPlayerPrefsKey()
		{
			if (!LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null)
			{
				return "StatPanelTab";
			}
			return "StatPanelTab_" + LocalPlayer.GameEntity.CollectionController.Record.Id;
		}

		// Token: 0x0600424E RID: 16974 RVA: 0x0006CC36 File Offset: 0x0006AE36
		internal static void SaveSelectedTab(int index)
		{
			PlayerPrefs.SetInt(TabbedStatPanelUI.GetSelectedTabPlayerPrefsKey(), index);
		}

		// Token: 0x0600424F RID: 16975 RVA: 0x00192430 File Offset: 0x00190630
		public void Init()
		{
			this.m_elements = new List<TabbedStatPanelElementUI>();
			int num = PlayerPrefs.GetInt(TabbedStatPanelUI.GetSelectedTabPlayerPrefsKey(), 0);
			if (num >= StatTypeExtensions.StatPanels.Length)
			{
				num = 0;
				TabbedStatPanelUI.SaveSelectedTab(num);
			}
			for (int i = 0; i < StatTypeExtensions.StatPanels.Length; i++)
			{
				TabbedStatPanelElementUI component = UnityEngine.Object.Instantiate<GameObject>(this.m_elementPrefab, this.m_tabPanel, false).GetComponent<TabbedStatPanelElementUI>();
				component.Init(this.m_toggleGroup, this.m_textContent, StatTypeExtensions.StatPanels[i], i, num);
				this.m_elements.Add(component);
			}
			this.UpdateArmorClass();
			this.UpdateArmorBudget();
			this.UpdatePanels();
			this.UpdateEventCurrency();
			if (this.m_inspectToggle)
			{
				this.m_inspectToggle.Init();
			}
		}

		// Token: 0x06004250 RID: 16976 RVA: 0x001924E8 File Offset: 0x001906E8
		public void UpdatePanels()
		{
			if (this.m_elements == null)
			{
				return;
			}
			for (int i = 0; i < this.m_elements.Count; i++)
			{
				this.m_elements[i].UpdateContents();
			}
		}

		// Token: 0x06004251 RID: 16977 RVA: 0x0006CC43 File Offset: 0x0006AE43
		public void UpdateArmorClass()
		{
			this.m_armorClassPanel.UpdateData();
		}

		// Token: 0x06004252 RID: 16978 RVA: 0x0006CC50 File Offset: 0x0006AE50
		public void UpdateArmorBudget()
		{
			this.m_armorBudgetPanel.UpdateData();
		}

		// Token: 0x06004253 RID: 16979 RVA: 0x00192528 File Offset: 0x00190728
		public void UpdateEventCurrency()
		{
			if (this.m_eventCurrency && LocalPlayer.GameEntity && LocalPlayer.GameEntity.User != null)
			{
				ulong value = (LocalPlayer.GameEntity.User.EventCurrency != null) ? LocalPlayer.GameEntity.User.EventCurrency.Value : 0UL;
				this.m_eventCurrency.UpdateEventCurrency(value);
			}
		}

		// Token: 0x04003F5D RID: 16221
		[SerializeField]
		private TextMeshProUGUI m_textContent;

		// Token: 0x04003F5E RID: 16222
		[SerializeField]
		private GameObject m_elementPrefab;

		// Token: 0x04003F5F RID: 16223
		[SerializeField]
		private RectTransform m_tabPanel;

		// Token: 0x04003F60 RID: 16224
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x04003F61 RID: 16225
		[SerializeField]
		private ArmorClassPanel m_armorClassPanel;

		// Token: 0x04003F62 RID: 16226
		[SerializeField]
		private ArmorBudgetPanel m_armorBudgetPanel;

		// Token: 0x04003F63 RID: 16227
		[SerializeField]
		private InspectToggle m_inspectToggle;

		// Token: 0x04003F64 RID: 16228
		[SerializeField]
		private CurrencyDisplayPanelUI m_eventCurrency;

		// Token: 0x04003F65 RID: 16229
		private List<TabbedStatPanelElementUI> m_elements;

		// Token: 0x04003F66 RID: 16230
		private const string kKeyPrefix = "StatPanelTab";
	}
}
