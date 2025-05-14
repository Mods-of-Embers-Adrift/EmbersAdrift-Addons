using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008DB RID: 2267
	public class TabbedStatPanelElementUI : MonoBehaviour
	{
		// Token: 0x06004245 RID: 16965 RVA: 0x0006CB88 File Offset: 0x0006AD88
		private void Awake()
		{
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
		}

		// Token: 0x06004246 RID: 16966 RVA: 0x0006CBA6 File Offset: 0x0006ADA6
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
		}

		// Token: 0x06004247 RID: 16967 RVA: 0x00192128 File Offset: 0x00190328
		public void Init(ToggleGroup toggleGroup, TextMeshProUGUI tmp, StatPanel statPanel, int index, int selectedIndex)
		{
			this.m_index = index;
			this.m_toggle.isOn = (index == selectedIndex);
			this.m_toggle.group = toggleGroup;
			this.m_tmp = tmp;
			this.m_title.text = statPanel.ToString();
			this.m_statPanel = statPanel;
		}

		// Token: 0x06004248 RID: 16968 RVA: 0x0006CBC4 File Offset: 0x0006ADC4
		private void ToggleChanged(bool active)
		{
			this.UpdateContents();
			if (this.m_toggle.isOn && this.m_index >= 0)
			{
				TabbedStatPanelUI.SaveSelectedTab(this.m_index);
			}
		}

		// Token: 0x06004249 RID: 16969 RVA: 0x0006CBED File Offset: 0x0006ADED
		public void UpdateContents()
		{
			this.m_selectionBorder.enabled = this.m_toggle.isOn;
			if (!this.m_toggle.isOn || this.m_tmp == null)
			{
				return;
			}
			this.PopulateStatPanelTab();
		}

		// Token: 0x0600424A RID: 16970 RVA: 0x00192180 File Offset: 0x00190380
		private void PopulateStatPanelTab()
		{
			using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder())
			{
				Dictionary<string, List<StatType>> categories = this.m_statPanel.GetCategories();
				bool flag = false;
				List<StatType> list;
				if (categories.TryGetValue("NONE", out list) && list.Count > 0)
				{
					stringBuilder.AppendLine("<u><i>Misc</i></u>");
					foreach (StatType statType in list)
					{
						if (!statType.IsInvalid())
						{
							string arg;
							string arg2;
							this.GetDisplayValues(statType, out arg, out arg2);
							stringBuilder.AppendFormat<string, string>("<align=left>{0}:<line-height=0.001>\n<align=right>{1}<line-height=1em></align>\n", arg, arg2);
						}
					}
					flag = true;
				}
				foreach (KeyValuePair<string, List<StatType>> keyValuePair in categories)
				{
					if (!(keyValuePair.Key == "NONE") && keyValuePair.Value.Count > 0)
					{
						if (flag)
						{
							stringBuilder.AppendLine();
						}
						stringBuilder.AppendFormat<string>("<u><i>{0}</i></u>\n", keyValuePair.Key);
						foreach (StatType statType2 in keyValuePair.Value)
						{
							if (!statType2.IsInvalid())
							{
								string arg3;
								string arg4;
								this.GetDisplayValues(statType2, out arg3, out arg4);
								stringBuilder.AppendFormat<string, string>("<align=left>{0}:<line-height=0.001>\n<align=right>{1}<line-height=1em></align>\n", arg3, arg4);
							}
						}
						flag = true;
					}
				}
				this.m_tmp.SetText(stringBuilder);
			}
		}

		// Token: 0x0600424B RID: 16971 RVA: 0x00192368 File Offset: 0x00190568
		private void GetDisplayValues(StatType statType, out string left, out string right)
		{
			int statValue = -1;
			string statPanelDisplay = statType.GetStatPanelDisplay();
			string tooltipDescription = statType.GetTooltipDescription(statValue);
			string arg = string.IsNullOrEmpty(tooltipDescription) ? statPanelDisplay : SoL.Utilities.Extensions.TextMeshProExtensions.CreateLongTextTooltipLink(tooltipDescription, statPanelDisplay, new int?(statType.GetHashCode()));
			string statusEffectDisplayValue = LocalPlayer.GameEntity.Vitals.GetStatusEffectDisplayValue(statType);
			left = ZString.Format<string, string>("{0}{1}", "<color=#00000000><size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size></color>", arg);
			right = statusEffectDisplayValue;
		}

		// Token: 0x04003F56 RID: 16214
		private const string kStatLineFormat = "<align=left>{0}:<line-height=0.001>\n<align=right>{1}<line-height=1em></align>\n";

		// Token: 0x04003F57 RID: 16215
		[SerializeField]
		private Image m_selectionBorder;

		// Token: 0x04003F58 RID: 16216
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04003F59 RID: 16217
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04003F5A RID: 16218
		private int m_index = -1;

		// Token: 0x04003F5B RID: 16219
		private TextMeshProUGUI m_tmp;

		// Token: 0x04003F5C RID: 16220
		private StatPanel m_statPanel;
	}
}
