using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000975 RID: 2421
	public class RuneCollectorUI : BaseMerchantUI<InteractiveRuneCollector>
	{
		// Token: 0x17000FF8 RID: 4088
		// (get) Token: 0x06004807 RID: 18439 RVA: 0x000701E6 File Offset: 0x0006E3E6
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.RuneCollector;
			}
		}

		// Token: 0x06004808 RID: 18440 RVA: 0x001A8788 File Offset: 0x001A6988
		protected override void Awake()
		{
			base.Awake();
			this.m_runeSourceTypes = new Dictionary<int, RuneSourceType>();
			this.m_choiceDropdown.ClearOptions();
			List<string> list = new List<string>(RuneSourceTypeExtensions.RuneSourceTypes.Length);
			for (int i = 0; i < RuneSourceTypeExtensions.RuneSourceTypes.Length; i++)
			{
				RuneSourceType runeSourceType = RuneSourceTypeExtensions.RuneSourceTypes[i];
				if (runeSourceType.OfferForExchange())
				{
					string item = runeSourceType.ToString();
					list.Add(item);
					this.m_runeSourceTypes.Add(list.IndexOf(item), runeSourceType);
				}
			}
			this.m_originalColor = this.m_costText.color;
			this.m_choiceDropdown.AddOptions(list);
			this.m_choiceDropdown.value = 0;
			this.m_choiceDropdown.onValueChanged.AddListener(new UnityAction<int>(this.ChoiceDropdownChanged));
			base.UIWindow.ShowCallback = new Action(this.WindowShown);
		}

		// Token: 0x06004809 RID: 18441 RVA: 0x000707FB File Offset: 0x0006E9FB
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_choiceDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.ChoiceDropdownChanged));
			base.UIWindow.ShowCallback = null;
		}

		// Token: 0x0600480A RID: 18442 RVA: 0x0007082B File Offset: 0x0006EA2B
		private void WindowShown()
		{
			this.m_choiceDropdown.value = 0;
		}

		// Token: 0x0600480B RID: 18443 RVA: 0x00070839 File Offset: 0x0006EA39
		private void ChoiceDropdownChanged(int arg0)
		{
			this.ContainerInstanceOnContentsChanged();
		}

		// Token: 0x0600480C RID: 18444 RVA: 0x001A8864 File Offset: 0x001A6A64
		protected override bool ButtonClickedInternal()
		{
			RuneSourceType runeSourceType;
			if (this.m_containerInstance.Count > 0 && this.m_runeSourceTypes.TryGetValue(this.m_choiceDropdown.value, out runeSourceType))
			{
				this.m_button.interactable = false;
				return true;
			}
			return false;
		}

		// Token: 0x0600480D RID: 18445 RVA: 0x001A88A8 File Offset: 0x001A6AA8
		protected override void ContainerInstanceOnContentsChanged()
		{
			base.ContainerInstanceOnContentsChanged();
			RuneSourceType runeSourceType;
			this.m_runeSourceTypes.TryGetValue(this.m_choiceDropdown.value, out runeSourceType);
			ArchetypeInstance archetypeInstance;
			RunicBattery runicBattery;
			if (this.m_containerInstance != null && this.m_containerInstance.TryGetInstanceForIndex(0, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out runicBattery) && archetypeInstance.IsItem && archetypeInstance.ItemData.Charges != null && archetypeInstance.ItemData.Charges.Value > 0)
			{
				bool flag = LocalPlayer.GameEntity.CollectionController.Inventory.Currency >= this.m_containerInstance.Currency;
				this.m_costText.color = (flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				this.m_button.interactable = (runeSourceType != RuneSourceType.None && runeSourceType != runicBattery.Mastery.RuneSource && flag);
				return;
			}
			this.m_costText.color = this.m_originalColor;
			this.m_button.interactable = false;
		}

		// Token: 0x0600480E RID: 18446 RVA: 0x00070841 File Offset: 0x0006EA41
		public void ExchangeResponse(OpCodes op)
		{
			if (op == OpCodes.Ok)
			{
				this.m_choiceDropdown.value = 0;
			}
			base.InteractionComplete();
		}

		// Token: 0x0400437E RID: 17278
		[SerializeField]
		private TMP_Dropdown m_choiceDropdown;

		// Token: 0x0400437F RID: 17279
		[SerializeField]
		private TextMeshProUGUI m_costText;

		// Token: 0x04004380 RID: 17280
		private Dictionary<int, RuneSourceType> m_runeSourceTypes;

		// Token: 0x04004381 RID: 17281
		private Color m_originalColor;
	}
}
