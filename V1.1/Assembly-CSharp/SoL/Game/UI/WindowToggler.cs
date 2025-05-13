using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008EE RID: 2286
	public class WindowToggler : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600430E RID: 17166 RVA: 0x00194E68 File Offset: 0x00193068
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
			WindowToggler.WindowType windowType = this.m_windowType;
			if (windowType - WindowToggler.WindowType.Inventory <= 1 || windowType == WindowToggler.WindowType.Gathering)
			{
				UIManager.ItemAddedToContainer += this.ContainerInstanceOnInstanceAddedToContainer;
			}
			UIManager.TriggerControlPanelUsageHighlight += this.OnTriggerControlPanelUsageHighlight;
		}

		// Token: 0x0600430F RID: 17167 RVA: 0x00194EC8 File Offset: 0x001930C8
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
			WindowToggler.WindowType windowType = this.m_windowType;
			if (windowType - WindowToggler.WindowType.Inventory <= 1 || windowType == WindowToggler.WindowType.Gathering)
			{
				UIManager.ItemAddedToContainer -= this.ContainerInstanceOnInstanceAddedToContainer;
			}
			UIManager.TriggerControlPanelUsageHighlight -= this.OnTriggerControlPanelUsageHighlight;
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x0006D386 File Offset: 0x0006B586
		private void OnTriggerControlPanelUsageHighlight(WindowToggler.WindowType obj)
		{
			if (obj == this.m_windowType)
			{
				this.CrossFadeUsageHighlight();
			}
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x00194F28 File Offset: 0x00193128
		private void ContainerInstanceOnInstanceAddedToContainer(ContainerType obj)
		{
			bool flag = false;
			if (obj != ContainerType.Equipment)
			{
				if (obj != ContainerType.Inventory)
				{
					if (obj == ContainerType.Gathering)
					{
						flag = (this.m_windowType == WindowToggler.WindowType.Gathering);
					}
				}
				else
				{
					flag = (this.m_windowType == WindowToggler.WindowType.Inventory);
				}
			}
			else
			{
				flag = (this.m_windowType == WindowToggler.WindowType.Equipment);
			}
			if (flag)
			{
				this.CrossFadeUsageHighlight();
			}
		}

		// Token: 0x06004312 RID: 17170 RVA: 0x0006D397 File Offset: 0x0006B597
		private void CrossFadeUsageHighlight()
		{
			if (this.m_usageHighlight)
			{
				this.m_usageHighlight.gameObject.SetActive(true);
				this.m_usageHighlight.CustomCrossFadeAlpha(1f, 0f, 1.5f);
			}
		}

		// Token: 0x06004313 RID: 17171 RVA: 0x00194F74 File Offset: 0x00193174
		private void ButtonClicked()
		{
			switch (this.m_windowType)
			{
			case WindowToggler.WindowType.Settings:
				ClientGameManager.UIManager.InGameUiMenu.ToggleWindow();
				return;
			case WindowToggler.WindowType.Inventory:
				ClientGameManager.UIManager.Inventory.ToggleWindow();
				return;
			case WindowToggler.WindowType.Equipment:
				ClientGameManager.UIManager.EquipmentStats.ToggleWindow();
				return;
			case WindowToggler.WindowType.Codex:
				ClientGameManager.UIManager.SkillsUI.ToggleWindow();
				return;
			case WindowToggler.WindowType.Help:
				ClientGameManager.UIManager.GameHelpPanel.ToggleWindow();
				return;
			case WindowToggler.WindowType.Custom:
				if (this.m_customWindow != null)
				{
					this.m_customWindow.ToggleWindow();
				}
				return;
			case WindowToggler.WindowType.Recipes:
				ClientGameManager.UIManager.CraftingUI.ToggleWindow();
				return;
			case WindowToggler.WindowType.Map:
				ClientGameManager.UIManager.MapUI.ToggleWindow();
				return;
			case WindowToggler.WindowType.Social:
				ClientGameManager.UIManager.SocialUI.ToggleWindow();
				return;
			case WindowToggler.WindowType.Notifications:
				ClientGameManager.UIManager.NotificationsWindow.ToggleWindow();
				return;
			case WindowToggler.WindowType.Gathering:
				ClientGameManager.UIManager.Gathering.ToggleWindow();
				return;
			case WindowToggler.WindowType.Log:
				ClientGameManager.UIManager.LogUI.ToggleWindow();
				return;
			case WindowToggler.WindowType.Time:
				ClientGameManager.UIManager.TimeUI.ToggleWindow();
				return;
			default:
				return;
			}
		}

		// Token: 0x06004314 RID: 17172 RVA: 0x001950A0 File Offset: 0x001932A0
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_disableTooltip)
			{
				return null;
			}
			WindowToggler.WindowType windowType = this.m_windowType;
			string text;
			if (windowType != WindowToggler.WindowType.Custom)
			{
				if (windowType != WindowToggler.WindowType.Time)
				{
					text = WindowToggler.GetTooltipTextForWindowType(this.m_windowType);
				}
				else
				{
					string keybindDescription = WindowToggler.GetKeybindDescription(this.m_windowType);
					string fullTimeDisplay = SkyDomeManager.GetFullTimeDisplay();
					text = (string.IsNullOrEmpty(keybindDescription) ? fullTimeDisplay : ZString.Format<string, string>("{0}\n\n{1}", WindowToggler.GetTooltipTextForWindowType(this.m_windowType), fullTimeDisplay));
				}
			}
			else
			{
				text = this.m_customWindowDescription;
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, text, false);
		}

		// Token: 0x17000F37 RID: 3895
		// (get) Token: 0x06004315 RID: 17173 RVA: 0x0006D3D1 File Offset: 0x0006B5D1
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F38 RID: 3896
		// (get) Token: 0x06004316 RID: 17174 RVA: 0x0006D3DF File Offset: 0x0006B5DF
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000F39 RID: 3897
		// (get) Token: 0x06004317 RID: 17175 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004318 RID: 17176 RVA: 0x00195130 File Offset: 0x00193330
		private static string GetTooltipTextForWindowType(WindowToggler.WindowType type)
		{
			string arg = string.Empty;
			switch (type)
			{
			case WindowToggler.WindowType.Inventory:
				arg = "Bag";
				break;
			case WindowToggler.WindowType.Equipment:
				arg = "Inventory";
				break;
			case WindowToggler.WindowType.Codex:
				arg = "Skills";
				break;
			default:
				arg = type.ToString();
				break;
			}
			return ZString.Format<string, string>("{0}{1}", arg, WindowToggler.GetKeybindDescription(type));
		}

		// Token: 0x06004319 RID: 17177 RVA: 0x00195190 File Offset: 0x00193390
		private static string GetKeybindDescription(WindowToggler.WindowType type)
		{
			int num = -1;
			switch (type)
			{
			case WindowToggler.WindowType.Inventory:
				num = 15;
				break;
			case WindowToggler.WindowType.Equipment:
				num = 17;
				break;
			case WindowToggler.WindowType.Codex:
				num = 16;
				break;
			case WindowToggler.WindowType.Recipes:
				num = 64;
				break;
			case WindowToggler.WindowType.Map:
				num = 67;
				break;
			case WindowToggler.WindowType.Social:
				num = 71;
				break;
			case WindowToggler.WindowType.Gathering:
				num = 96;
				break;
			case WindowToggler.WindowType.Log:
				num = 97;
				break;
			case WindowToggler.WindowType.Time:
				num = 112;
				break;
			}
			if (num == -1)
			{
				return null;
			}
			string primaryBindingNameForAction = SolInput.Mapper.GetPrimaryBindingNameForAction(num);
			if (!string.IsNullOrEmpty(primaryBindingNameForAction))
			{
				return ZString.Format<string>(" ({0})", BindingLabels.GetAbbreviatedBindingLabel(primaryBindingNameForAction));
			}
			return null;
		}

		// Token: 0x0600431B RID: 17179 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003FBF RID: 16319
		[SerializeField]
		private WindowToggler.WindowType m_windowType;

		// Token: 0x04003FC0 RID: 16320
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003FC1 RID: 16321
		[SerializeField]
		private Image m_usageHighlight;

		// Token: 0x04003FC2 RID: 16322
		[SerializeField]
		private bool m_disableTooltip;

		// Token: 0x04003FC3 RID: 16323
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003FC4 RID: 16324
		[SerializeField]
		private UIWindow m_customWindow;

		// Token: 0x04003FC5 RID: 16325
		[SerializeField]
		private string m_customWindowDescription;

		// Token: 0x020008EF RID: 2287
		public enum WindowType
		{
			// Token: 0x04003FC7 RID: 16327
			None,
			// Token: 0x04003FC8 RID: 16328
			Settings,
			// Token: 0x04003FC9 RID: 16329
			Inventory,
			// Token: 0x04003FCA RID: 16330
			Equipment,
			// Token: 0x04003FCB RID: 16331
			Codex,
			// Token: 0x04003FCC RID: 16332
			Help,
			// Token: 0x04003FCD RID: 16333
			Custom,
			// Token: 0x04003FCE RID: 16334
			Recipes,
			// Token: 0x04003FCF RID: 16335
			Map,
			// Token: 0x04003FD0 RID: 16336
			Social,
			// Token: 0x04003FD1 RID: 16337
			Notifications,
			// Token: 0x04003FD2 RID: 16338
			Gathering,
			// Token: 0x04003FD3 RID: 16339
			Log,
			// Token: 0x04003FD4 RID: 16340
			Time
		}
	}
}
