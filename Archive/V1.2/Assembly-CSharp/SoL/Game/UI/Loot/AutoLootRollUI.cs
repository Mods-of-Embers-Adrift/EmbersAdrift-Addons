using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Loot
{
	// Token: 0x0200097E RID: 2430
	public class AutoLootRollUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600485F RID: 18527 RVA: 0x001A9888 File Offset: 0x001A7A88
		private void Awake()
		{
			this.m_autoRollImageDefaultColor = this.m_autoRollImage.color;
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].Init(this);
			}
			this.RefreshAutoRollImage();
			this.m_optionsButton.onClick.AddListener(new UnityAction(this.OptionsButtonClicked));
			ClientGroupManager.AutoLootRollChanged += this.SelectActiveToggle;
			this.m_optionsMenu.Hide(true);
		}

		// Token: 0x06004860 RID: 18528 RVA: 0x001A9908 File Offset: 0x001A7B08
		private void Start()
		{
			Transform[] componentsInChildren = this.m_optionsMenu.gameObject.GetComponentsInChildren<Transform>();
			this.m_nestedMenuGameObjects = new HashSet<GameObject>
			{
				this.m_optionsMenu.gameObject
			};
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.m_nestedMenuGameObjects.Add(componentsInChildren[i].gameObject);
			}
		}

		// Token: 0x06004861 RID: 18529 RVA: 0x001A9968 File Offset: 0x001A7B68
		private void Update()
		{
			if (this.m_optionsMenu.Visible && !this.m_optionsMenu.CursorInside && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && (InteractionManager.HoveredUIElement == null || !this.m_nestedMenuGameObjects.Contains(InteractionManager.HoveredUIElement)))
			{
				this.m_optionsMenu.Hide(false);
			}
		}

		// Token: 0x06004862 RID: 18530 RVA: 0x001A99CC File Offset: 0x001A7BCC
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].OnDestroy();
			}
			this.m_optionsButton.onClick.RemoveListener(new UnityAction(this.OptionsButtonClicked));
			ClientGroupManager.AutoLootRollChanged -= this.SelectActiveToggle;
		}

		// Token: 0x06004863 RID: 18531 RVA: 0x00070B10 File Offset: 0x0006ED10
		private void OptionsButtonClicked()
		{
			this.m_optionsMenu.ToggleWindow();
		}

		// Token: 0x06004864 RID: 18532 RVA: 0x001A9A28 File Offset: 0x001A7C28
		private void ToggleChanged()
		{
			this.m_preventEventTriggers = true;
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].IsActive)
				{
					ClientGroupManager.AutoLootRoll = this.m_toggles[i].ToggleType;
					break;
				}
			}
			this.m_preventEventTriggers = false;
			this.RefreshToggleInteractivity();
			this.RefreshAutoRollImage();
			if (this.m_optionsMenu.Visible)
			{
				this.m_optionsMenu.Hide(false);
			}
		}

		// Token: 0x06004865 RID: 18533 RVA: 0x001A9AA0 File Offset: 0x001A7CA0
		private void SelectActiveToggle()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].ToggleType == ClientGroupManager.AutoLootRoll)
				{
					this.m_activeToggle = this.m_toggles[i];
					this.m_toggles[i].Toggle.isOn = true;
					return;
				}
			}
		}

		// Token: 0x06004866 RID: 18534 RVA: 0x001A9AF8 File Offset: 0x001A7CF8
		private void RefreshToggleInteractivity()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].Toggle.interactable = (this.m_toggles[i] != this.m_activeToggle);
			}
		}

		// Token: 0x06004867 RID: 18535 RVA: 0x001A9B40 File Offset: 0x001A7D40
		private void RefreshAutoRollImage()
		{
			if (ClientGroupManager.AutoLootRoll == LootRollChoice.Unanswered)
			{
				this.m_autoRollImage.overrideSprite = null;
				this.m_autoRollImage.color = this.m_autoRollImageDefaultColor;
				return;
			}
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].ToggleType == ClientGroupManager.AutoLootRoll)
				{
					this.m_autoRollImage.overrideSprite = this.m_toggles[i].Icon.sprite;
					this.m_autoRollImage.color = this.m_toggles[i].Icon.color;
					return;
				}
			}
		}

		// Token: 0x06004868 RID: 18536 RVA: 0x001A9BD4 File Offset: 0x001A7DD4
		private ITooltipParameter GetTooltipParameter()
		{
			string arg = (ClientGroupManager.AutoLootRoll == LootRollChoice.Unanswered) ? "None" : ClientGroupManager.AutoLootRoll.ToString();
			return new ObjectTextTooltipParameter(this, ZString.Format<string>("Auto Loot Roll: {0}", arg), false);
		}

		// Token: 0x17001013 RID: 4115
		// (get) Token: 0x06004869 RID: 18537 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001014 RID: 4116
		// (get) Token: 0x0600486A RID: 18538 RVA: 0x00070B1D File Offset: 0x0006ED1D
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001015 RID: 4117
		// (get) Token: 0x0600486B RID: 18539 RVA: 0x00070B2B File Offset: 0x0006ED2B
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600486D RID: 18541 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040043AF RID: 17327
		[SerializeField]
		private AutoLootRollUI.LootRollToggle[] m_toggles;

		// Token: 0x040043B0 RID: 17328
		[SerializeField]
		private UIWindow m_optionsMenu;

		// Token: 0x040043B1 RID: 17329
		[SerializeField]
		private SolButton m_optionsButton;

		// Token: 0x040043B2 RID: 17330
		[SerializeField]
		private Image m_autoRollImage;

		// Token: 0x040043B3 RID: 17331
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040043B4 RID: 17332
		private AutoLootRollUI.LootRollToggle m_activeToggle;

		// Token: 0x040043B5 RID: 17333
		private Color m_autoRollImageDefaultColor;

		// Token: 0x040043B6 RID: 17334
		private bool m_preventEventTriggers;

		// Token: 0x040043B7 RID: 17335
		private HashSet<GameObject> m_nestedMenuGameObjects;

		// Token: 0x0200097F RID: 2431
		[Serializable]
		private class LootRollToggle
		{
			// Token: 0x17001016 RID: 4118
			// (get) Token: 0x0600486E RID: 18542 RVA: 0x00070B33 File Offset: 0x0006ED33
			public LootRollChoice ToggleType
			{
				get
				{
					return this.m_choice;
				}
			}

			// Token: 0x17001017 RID: 4119
			// (get) Token: 0x0600486F RID: 18543 RVA: 0x00070B3B File Offset: 0x0006ED3B
			public SolToggle Toggle
			{
				get
				{
					return this.m_toggle;
				}
			}

			// Token: 0x17001018 RID: 4120
			// (get) Token: 0x06004870 RID: 18544 RVA: 0x00070B43 File Offset: 0x0006ED43
			public bool IsActive
			{
				get
				{
					return this.m_toggle.isOn;
				}
			}

			// Token: 0x17001019 RID: 4121
			// (get) Token: 0x06004871 RID: 18545 RVA: 0x00070B50 File Offset: 0x0006ED50
			public Image Icon
			{
				get
				{
					return this.m_icon;
				}
			}

			// Token: 0x06004872 RID: 18546 RVA: 0x00070B58 File Offset: 0x0006ED58
			public void Init(AutoLootRollUI controller)
			{
				this.m_controller = controller;
				this.m_toggle.isOn = (this.m_choice == LootRollChoice.Unanswered);
				this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			}

			// Token: 0x06004873 RID: 18547 RVA: 0x00070B91 File Offset: 0x0006ED91
			public void OnDestroy()
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			}

			// Token: 0x06004874 RID: 18548 RVA: 0x00070BAF File Offset: 0x0006EDAF
			private void ToggleChanged(bool value)
			{
				if (this.m_controller.m_preventEventTriggers)
				{
					return;
				}
				this.m_controller.ToggleChanged();
			}

			// Token: 0x040043B8 RID: 17336
			[SerializeField]
			private LootRollChoice m_choice;

			// Token: 0x040043B9 RID: 17337
			[SerializeField]
			private SolToggle m_toggle;

			// Token: 0x040043BA RID: 17338
			[SerializeField]
			private Image m_icon;

			// Token: 0x040043BB RID: 17339
			private AutoLootRollUI m_controller;
		}
	}
}
