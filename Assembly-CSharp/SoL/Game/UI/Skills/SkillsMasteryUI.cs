using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000930 RID: 2352
	public class SkillsMasteryUI : MonoBehaviour, IContainerUI
	{
		// Token: 0x140000CB RID: 203
		// (add) Token: 0x0600453E RID: 17726 RVA: 0x0019F0B4 File Offset: 0x0019D2B4
		// (remove) Token: 0x0600453F RID: 17727 RVA: 0x0019F0EC File Offset: 0x0019D2EC
		public event Action<ArchetypeInstance> SelectedMasteryUpdated;

		// Token: 0x17000F87 RID: 3975
		// (get) Token: 0x06004540 RID: 17728 RVA: 0x0006EB0C File Offset: 0x0006CD0C
		// (set) Token: 0x06004541 RID: 17729 RVA: 0x0019F124 File Offset: 0x0019D324
		public ArchetypeInstance CurrentInstance
		{
			get
			{
				return this.m_currentInstance;
			}
			private set
			{
				if (this.m_initialized && this.m_currentInstance == value)
				{
					return;
				}
				if (this.m_currentInstance != null)
				{
					this.m_masteryDetails.UnregisterMastery();
					this.m_currentInstance.MasteryData.LevelDataChanged -= this.RefreshLevelData;
					this.m_currentInstance.MasteryData.MasteryDataChanged -= this.RefreshAbilityPoints;
				}
				this.m_currentInstance = value;
				if (this.m_currentInstance != null)
				{
					this.m_masteryDetails.RegisterMastery(this.m_currentInstance);
					this.m_masteryDetails.gameObject.SetActive(true);
					this.m_currentInstance.MasteryData.LevelDataChanged += this.RefreshLevelData;
					this.m_currentInstance.MasteryData.MasteryDataChanged += this.RefreshAbilityPoints;
				}
				else
				{
					this.m_masteryDetails.gameObject.SetActive(false);
					this.m_masteryDetails.UnregisterMastery();
				}
				this.RefreshLevelData();
				this.RefreshPauseAdventuringExperienceToggle();
				Action<ArchetypeInstance> selectedMasteryUpdated = this.SelectedMasteryUpdated;
				if (selectedMasteryUpdated != null)
				{
					selectedMasteryUpdated(this.m_currentInstance);
				}
				this.m_initialized = true;
			}
		}

		// Token: 0x06004542 RID: 17730 RVA: 0x0019F240 File Offset: 0x0019D440
		private void Awake()
		{
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			this.m_tabController.TabChanged += this.TabControllerOnTabChanged;
			if (this.m_specializationLines != null && this.m_specializationLines.Length != 0)
			{
				this.m_defaultSpecializationLineColor = this.m_specializationLines[0].color;
			}
			for (int i = 0; i < this.m_masteryTabs.Length; i++)
			{
				if (this.m_masteryTabs[i].LowerTab)
				{
					this.m_lowerTabs.Add(this.m_masteryTabs[i]);
				}
				else
				{
					this.m_upperTabs.Add(this.m_masteryTabs[i]);
				}
			}
			if (this.m_pauseAdventuringExperienceToggle)
			{
				this.m_pauseAdventuringExperienceToggle.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004543 RID: 17731 RVA: 0x0019F304 File Offset: 0x0019D504
		private void OnDestroy()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.m_tabController.TabChanged -= this.TabControllerOnTabChanged;
			if (this.m_container != null)
			{
				this.m_container.ContentsChanged -= this.MasteriesOnContentsChanged;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed -= this.AdventuringLevelSyncOnChanged;
			}
		}

		// Token: 0x06004544 RID: 17732 RVA: 0x0019F398 File Offset: 0x0019D598
		public void SwitchToMastery(UniqueId archetypeId)
		{
			int num = -1;
			for (int i = 0; i < this.m_masteryTabs.Length; i++)
			{
				if (this.m_masteryTabs[i].Instance != null && this.m_masteryTabs[i].Instance.ArchetypeId == archetypeId)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				this.m_tabController.SwitchToTab(num);
			}
		}

		// Token: 0x06004545 RID: 17733 RVA: 0x0019F3F8 File Offset: 0x0019D5F8
		private ArchetypeInstance GetActiveInstance()
		{
			for (int i = 0; i < this.m_masteryTabs.Length; i++)
			{
				if (this.m_masteryTabs[i].Instance != null && this.m_masteryTabs[i].ToggleIsOn)
				{
					return this.m_masteryTabs[i].Instance;
				}
			}
			return null;
		}

		// Token: 0x06004546 RID: 17734 RVA: 0x0006EB14 File Offset: 0x0006CD14
		private void TabControllerOnTabChanged()
		{
			if (!this.m_preventDropdownEvent)
			{
				this.CurrentInstance = this.GetActiveInstance();
			}
		}

		// Token: 0x06004547 RID: 17735 RVA: 0x0019F448 File Offset: 0x0019D648
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			if (this.m_pauseAdventuringExperienceToggle)
			{
				this.m_pauseAdventuringExperienceToggle.Init();
			}
			this.RefreshAvailableMasteries(false);
			this.RefreshLevelData();
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed += this.AdventuringLevelSyncOnChanged;
			}
		}

		// Token: 0x06004548 RID: 17736 RVA: 0x0006EB2A File Offset: 0x0006CD2A
		private void RefreshAbilityPoints()
		{
			this.m_abilityUi.RefreshLocks();
		}

		// Token: 0x06004549 RID: 17737 RVA: 0x0019F4B8 File Offset: 0x0019D6B8
		private void RefreshLevelData()
		{
			this.RefreshAbilityPoints();
			bool active = false;
			int? num = null;
			int? num2 = null;
			if (this.CurrentInstance != null && this.CurrentInstance.MasteryData != null)
			{
				float currentInstanceMaxLevel = this.GetCurrentInstanceMaxLevel();
				num = new int?(Mathf.FloorToInt(this.CurrentInstance.MasteryData.BaseLevel));
				this.m_baseProgress.SetFills(this.CurrentInstance.MasteryData.BaseLevel, currentInstanceMaxLevel);
				if (this.CurrentInstance.MasteryData.Specialization != null)
				{
					num2 = new int?(Mathf.FloorToInt(this.CurrentInstance.MasteryData.SpecializationLevel));
					this.m_specProgress.SetFills(this.CurrentInstance.MasteryData.SpecializationLevel, currentInstanceMaxLevel);
					active = !Mathf.Approximately(this.CurrentInstance.MasteryData.BaseLevel, this.CurrentInstance.MasteryData.SpecializationLevel);
				}
				else
				{
					this.m_specProgress.Reset();
				}
			}
			else
			{
				this.m_baseProgress.Reset();
				this.m_specProgress.Reset();
			}
			this.m_specPanelBlocker.gameObject.SetActive(false);
			this.m_specProgressBar.gameObject.SetActive(active);
			if (this.m_levelLabel)
			{
				if (num != null && num2 != null && num.Value != num2.Value)
				{
					this.m_levelLabel.SetTextFormat("{0}\n<size={1}%>{2}</size>", num2.Value, 60, num.Value);
				}
				else if (num != null)
				{
					this.m_levelLabel.SetTextFormat("{0}", num.Value);
				}
				else
				{
					this.m_levelLabel.text = "";
				}
			}
			this.m_abilityUi.RefreshLocks();
			this.RefreshMaxLevelBlocker();
		}

		// Token: 0x0600454A RID: 17738 RVA: 0x0019F690 File Offset: 0x0019D890
		private void RefreshPauseAdventuringExperienceToggle()
		{
			if (!this.m_pauseAdventuringExperienceToggle)
			{
				return;
			}
			bool active = this.CurrentInstance != null && this.CurrentInstance.MasteryData != null && this.CurrentInstance.Mastery && this.CurrentInstance.Mastery.Type == MasteryType.Combat;
			this.m_pauseAdventuringExperienceToggle.gameObject.SetActive(active);
		}

		// Token: 0x0600454B RID: 17739 RVA: 0x0004475B File Offset: 0x0004295B
		private void RefreshMaxLevelBlocker()
		{
		}

		// Token: 0x0600454C RID: 17740 RVA: 0x0006EB37 File Offset: 0x0006CD37
		private void AdventuringLevelSyncOnChanged(byte obj)
		{
			this.RefreshMaxLevelBlocker();
		}

		// Token: 0x0600454D RID: 17741 RVA: 0x0019F6FC File Offset: 0x0019D8FC
		private float GetCurrentInstanceMaxLevel()
		{
			if (this.CurrentInstance != null && this.CurrentInstance.Mastery != null)
			{
				MasteryType type = this.CurrentInstance.Mastery.Type;
				if (type == MasteryType.Combat)
				{
					return 50f;
				}
				if (type - MasteryType.Trade <= 1)
				{
					return (float)GlobalSettings.Values.Crafting.GetMaxTradeLevel(LocalPlayer.GameEntity.CharacterData.AdventuringLevel);
				}
			}
			return 0f;
		}

		// Token: 0x0600454E RID: 17742 RVA: 0x0019F76C File Offset: 0x0019D96C
		private void RefreshAvailableMasteries(bool initial)
		{
			this.m_preventDropdownEvent = true;
			ArchetypeInstance activeInstance = this.GetActiveInstance();
			MasteryTabUI masteryTabUI = this.m_upperTabs[0];
			this.InitializeTabGroup(false);
			this.InitializeTabGroup(true);
			for (int i = 0; i < this.m_masteryTabs.Length; i++)
			{
				if (this.m_masteryTabs[i].Instance != null && this.m_masteryTabs[i].Instance == activeInstance)
				{
					masteryTabUI = this.m_masteryTabs[i];
				}
			}
			this.CurrentInstance = masteryTabUI.Instance;
			if (masteryTabUI.Instance != null)
			{
				masteryTabUI.ToggleIsOn = true;
			}
			this.m_preventDropdownEvent = false;
		}

		// Token: 0x0600454F RID: 17743 RVA: 0x0019F800 File Offset: 0x0019DA00
		private void InitializeTabGroup(bool lowerTabs)
		{
			List<MasteryTabUI> list = lowerTabs ? this.m_lowerTabs : this.m_upperTabs;
			int num = 0;
			foreach (ArchetypeInstance archetypeInstance in this.m_container.Instances)
			{
				BaseRole baseRole;
				if (archetypeInstance.Archetype.TryGetAsType(out baseRole))
				{
					if (baseRole.Type == MasteryType.Combat)
					{
						if (lowerTabs)
						{
							continue;
						}
					}
					else if (!lowerTabs)
					{
						continue;
					}
					if (num < list.Count)
					{
						list[num].Instance = archetypeInstance;
					}
					num++;
				}
			}
			for (int i = num; i < list.Count; i++)
			{
				list[i].Instance = null;
				list[i].ToggleIsOn = false;
			}
		}

		// Token: 0x06004550 RID: 17744 RVA: 0x0006EB3F File Offset: 0x0006CD3F
		private void MasteriesOnContentsChanged()
		{
			this.RefreshAvailableMasteries(false);
			this.RefreshLevelData();
		}

		// Token: 0x17000F88 RID: 3976
		// (get) Token: 0x06004551 RID: 17745 RVA: 0x0006EB4E File Offset: 0x0006CD4E
		public string Id
		{
			get
			{
				return this.m_container.Id;
			}
		}

		// Token: 0x17000F89 RID: 3977
		// (get) Token: 0x06004552 RID: 17746 RVA: 0x0004479C File Offset: 0x0004299C
		public bool Locked
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000F8A RID: 3978
		// (get) Token: 0x06004553 RID: 17747 RVA: 0x0006EB5B File Offset: 0x0006CD5B
		public bool Visible { get; }

		// Token: 0x06004554 RID: 17748 RVA: 0x0004475B File Offset: 0x0004295B
		void IContainerUI.CloseButtonPressed()
		{
		}

		// Token: 0x06004555 RID: 17749 RVA: 0x0006EB63 File Offset: 0x0006CD63
		public void AddInstance(ArchetypeInstance instance)
		{
			this.RefreshAvailableMasteries(false);
		}

		// Token: 0x06004556 RID: 17750 RVA: 0x0006EB63 File Offset: 0x0006CD63
		public void RemoveInstance(ArchetypeInstance instance)
		{
			this.RefreshAvailableMasteries(false);
		}

		// Token: 0x06004557 RID: 17751 RVA: 0x0004475B File Offset: 0x0004295B
		public void ItemsSwapped()
		{
		}

		// Token: 0x06004558 RID: 17752 RVA: 0x0006EB6C File Offset: 0x0006CD6C
		public void Initialize(ContainerInstance containerInstance)
		{
			this.m_container = containerInstance;
			this.m_container.ContentsChanged += this.MasteriesOnContentsChanged;
		}

		// Token: 0x06004559 RID: 17753 RVA: 0x0004475B File Offset: 0x0004295B
		public void PostInit()
		{
		}

		// Token: 0x0600455A RID: 17754 RVA: 0x0004475B File Offset: 0x0004295B
		public void InstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x0600455B RID: 17755 RVA: 0x0004475B File Offset: 0x0004295B
		public void InteractWithInstance(ArchetypeInstance instance)
		{
		}

		// Token: 0x17000F8B RID: 3979
		// (get) Token: 0x0600455C RID: 17756 RVA: 0x0006EB8C File Offset: 0x0006CD8C
		public ContainerInstance ContainerInstance
		{
			get
			{
				return this.m_container;
			}
		}

		// Token: 0x0600455D RID: 17757 RVA: 0x0004475B File Offset: 0x0004295B
		public void Show()
		{
		}

		// Token: 0x0600455E RID: 17758 RVA: 0x0004475B File Offset: 0x0004295B
		public void Hide()
		{
		}

		// Token: 0x0600455F RID: 17759 RVA: 0x0006EB94 File Offset: 0x0006CD94
		public bool IsLockedWithNotification()
		{
			return this.Locked;
		}

		// Token: 0x06004560 RID: 17760 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		bool IContainerUI.TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI)
		{
			slotUI = null;
			return false;
		}

		// Token: 0x040041A5 RID: 16805
		[SerializeField]
		private SkillsAbilityUI m_abilityUi;

		// Token: 0x040041A6 RID: 16806
		[SerializeField]
		private TMP_Dropdown m_masteryDropdown;

		// Token: 0x040041A7 RID: 16807
		[SerializeField]
		private MasteryDetailsUI m_masteryDetails;

		// Token: 0x040041A8 RID: 16808
		[SerializeField]
		private TextMeshProUGUI m_roleLevelLabel;

		// Token: 0x040041A9 RID: 16809
		[SerializeField]
		private TextMeshProUGUI m_specLevelLabel;

		// Token: 0x040041AA RID: 16810
		[SerializeField]
		private MasteryTabUI[] m_masteryTabs;

		// Token: 0x040041AB RID: 16811
		[SerializeField]
		private TabController m_tabController;

		// Token: 0x040041AC RID: 16812
		[SerializeField]
		private Image[] m_specializationLines;

		// Token: 0x040041AD RID: 16813
		private Color m_defaultSpecializationLineColor = Color.white;

		// Token: 0x040041AE RID: 16814
		private const string kBaseGroup = "Base";

		// Token: 0x040041AF RID: 16815
		private const string kSpecGroup = "Spec";

		// Token: 0x040041B0 RID: 16816
		[SerializeField]
		private SkillsMasteryUI.ProgressBar m_baseProgress;

		// Token: 0x040041B1 RID: 16817
		[SerializeField]
		private SkillsMasteryUI.ProgressBar m_specProgress;

		// Token: 0x040041B2 RID: 16818
		[SerializeField]
		private FilledImage m_baseLevelFill;

		// Token: 0x040041B3 RID: 16819
		[SerializeField]
		private FilledImage m_baseLevelFractionFill;

		// Token: 0x040041B4 RID: 16820
		[SerializeField]
		private FilledImage m_specLevelFill;

		// Token: 0x040041B5 RID: 16821
		[SerializeField]
		private FilledImage m_specLevelFractionFill;

		// Token: 0x040041B6 RID: 16822
		[SerializeField]
		private Image m_maxLevelBlocker;

		// Token: 0x040041B7 RID: 16823
		[SerializeField]
		private RectTransform m_specProgressBar;

		// Token: 0x040041B8 RID: 16824
		[SerializeField]
		private RectTransform m_specPanelBlocker;

		// Token: 0x040041B9 RID: 16825
		[SerializeField]
		private TextMeshProUGUI m_levelLabel;

		// Token: 0x040041BA RID: 16826
		[SerializeField]
		private PauseAdventuringExperienceToggle m_pauseAdventuringExperienceToggle;

		// Token: 0x040041BB RID: 16827
		private ContainerInstance m_container;

		// Token: 0x040041BC RID: 16828
		private bool m_initialized;

		// Token: 0x040041BD RID: 16829
		private bool m_preventDropdownEvent;

		// Token: 0x040041BE RID: 16830
		private readonly List<MasteryTabUI> m_upperTabs = new List<MasteryTabUI>();

		// Token: 0x040041BF RID: 16831
		private readonly List<MasteryTabUI> m_lowerTabs = new List<MasteryTabUI>();

		// Token: 0x040041C0 RID: 16832
		private ArchetypeInstance m_currentInstance;

		// Token: 0x02000931 RID: 2353
		[Serializable]
		private class ProgressBar
		{
			// Token: 0x06004562 RID: 17762 RVA: 0x0019F8CC File Offset: 0x0019DACC
			private void Init()
			{
				if (!this.m_initialized)
				{
					if (this.m_ticks != null && this.m_ticks.Length != 0 && this.m_ticks[0])
					{
						this.m_defaultTickColor = this.m_ticks[0].color;
					}
					if (this.m_tickLabels != null && this.m_tickLabels.Length != 0 && this.m_tickLabels[0])
					{
						this.m_defaultTickLabelColor = this.m_tickLabels[0].color;
					}
					if (this.m_levelFractionFill != null)
					{
						this.m_selectedTickColor = this.m_levelFractionFill.GetFillColor();
					}
				}
				this.m_initialized = true;
			}

			// Token: 0x06004563 RID: 17763 RVA: 0x0019F968 File Offset: 0x0019DB68
			public void SetFills(float level, float levelCap)
			{
				this.Init();
				float fillAmount = 1f;
				float fillAmount2 = 1f;
				float arg = levelCap - 1f;
				float arg2 = levelCap;
				int tickColors = -1;
				if (level < levelCap)
				{
					int num = Mathf.FloorToInt(level);
					float num2 = (fillAmount = level - (float)num) * 5f;
					int num3 = Mathf.FloorToInt(num2);
					tickColors = num3;
					fillAmount2 = num2 - (float)num3;
					arg = (float)num;
					arg2 = (float)(num + 1);
				}
				this.m_levelFill.fillAmount = fillAmount;
				this.m_levelFractionFill.fillAmount = fillAmount2;
				this.m_leftLabel.SetTextFormat("{0}", arg);
				this.m_rightLabel.SetTextFormat("{0}", arg2);
				this.SetTickColors(tickColors);
			}

			// Token: 0x06004564 RID: 17764 RVA: 0x0019FA0C File Offset: 0x0019DC0C
			public void Reset()
			{
				this.m_levelFill.fillAmount = 0f;
				this.m_levelFractionFill.fillAmount = 0f;
				this.m_leftLabel.text = "";
				this.m_rightLabel.text = "";
			}

			// Token: 0x06004565 RID: 17765 RVA: 0x0019FA5C File Offset: 0x0019DC5C
			private void SetTickColors(int index)
			{
				if (this.m_ticks != null)
				{
					for (int i = 0; i < this.m_ticks.Length; i++)
					{
						if (this.m_ticks[i])
						{
							this.m_ticks[i].color = ((i == index) ? this.m_selectedTickColor : this.m_defaultTickColor);
						}
					}
				}
				if (this.m_tickLabels != null)
				{
					for (int j = 0; j < this.m_tickLabels.Length; j++)
					{
						if (this.m_tickLabels[j])
						{
							this.m_tickLabels[j].color = ((j == index) ? this.m_selectedTickLabelColor : this.m_defaultTickLabelColor);
						}
					}
				}
			}

			// Token: 0x040041C2 RID: 16834
			private const int kNumberOfSegments = 5;

			// Token: 0x040041C3 RID: 16835
			[SerializeField]
			private FilledImage m_levelFill;

			// Token: 0x040041C4 RID: 16836
			[SerializeField]
			private FilledImage m_levelFractionFill;

			// Token: 0x040041C5 RID: 16837
			[SerializeField]
			private TextMeshProUGUI m_leftLabel;

			// Token: 0x040041C6 RID: 16838
			[SerializeField]
			private TextMeshProUGUI m_rightLabel;

			// Token: 0x040041C7 RID: 16839
			[SerializeField]
			private Image[] m_ticks;

			// Token: 0x040041C8 RID: 16840
			[SerializeField]
			private TextMeshProUGUI[] m_tickLabels;

			// Token: 0x040041C9 RID: 16841
			[SerializeField]
			private Color m_selectedTickLabelColor = new Color(0.7921569f, 0.77254903f, 0.76862746f, 1f);

			// Token: 0x040041CA RID: 16842
			private bool m_initialized;

			// Token: 0x040041CB RID: 16843
			private Color m_defaultTickColor = new Color(0.41960785f, 0.39607844f, 0.4117647f, 0.7058824f);

			// Token: 0x040041CC RID: 16844
			private Color m_defaultTickLabelColor = new Color(0.4117647f, 0.38431373f, 0.39215687f, 1f);

			// Token: 0x040041CD RID: 16845
			private Color m_selectedTickColor = new Color(0.8627451f, 0.078431375f, 0.23529412f, 1f);
		}
	}
}
