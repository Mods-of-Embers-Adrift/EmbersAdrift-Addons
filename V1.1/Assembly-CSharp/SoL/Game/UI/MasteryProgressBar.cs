using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008AE RID: 2222
	public class MasteryProgressBar : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000EE5 RID: 3813
		// (get) Token: 0x0600410A RID: 16650 RVA: 0x0006C02D File Offset: 0x0006A22D
		// (set) Token: 0x0600410B RID: 16651 RVA: 0x0018DF1C File Offset: 0x0018C11C
		private ArchetypeInstance SelectedInstance
		{
			get
			{
				return this.m_selectedInstance;
			}
			set
			{
				if (this.m_selectedInstance == value)
				{
					return;
				}
				if (this.m_selectedInstance != null && this.m_selectedInstance.MasteryData != null)
				{
					this.m_selectedInstance.MasteryData.LevelDataChanged -= this.LevelDataChanged;
					this.m_selectedInstance.MasteryData.MasteryDataChanged -= this.MasteryDataChanged;
				}
				this.m_selectedInstance = value;
				if (this.m_selectedInstance != null && this.m_selectedInstance.MasteryData != null)
				{
					this.m_selectedInstance.MasteryData.LevelDataChanged += this.LevelDataChanged;
					this.m_selectedInstance.MasteryData.MasteryDataChanged += this.MasteryDataChanged;
				}
				this.m_playerCharacterData.TrackedMastery = ((this.m_selectedInstance == null) ? UniqueId.Empty : this.m_selectedInstance.ArchetypeId);
				this.RefreshIcon();
				this.LevelDataChanged(true);
			}
		}

		// Token: 0x0600410C RID: 16652 RVA: 0x0018E008 File Offset: 0x0018C208
		private void Awake()
		{
			this.m_bubble.Init(this);
			this.m_progressFill.fillAmount = 0f;
			this.m_fractionalProgressFill.fillAmount = 0f;
			if (this.m_ticks != null && this.m_ticks.Length != 0 && this.m_ticks[0])
			{
				this.m_defaultTickColor = this.m_ticks[0].color;
			}
			this.m_selectedTickColor = this.m_fractionalProgressFill.GetFillColor();
			if (this.m_pausePanel)
			{
				this.m_pausePanel.SetActive(false);
			}
		}

		// Token: 0x0600410D RID: 16653 RVA: 0x0006C035 File Offset: 0x0006A235
		private void Start()
		{
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x0600410E RID: 16654 RVA: 0x0018E0A0 File Offset: 0x0018C2A0
		private void OnDestroy()
		{
			if (this.SelectedInstance != null && this.SelectedInstance.MasteryData != null)
			{
				this.SelectedInstance.MasteryData.LevelDataChanged -= this.LevelDataChanged;
				this.SelectedInstance.MasteryData.MasteryDataChanged -= this.MasteryDataChanged;
			}
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
				{
					LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
				}
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.PauseAdventuringExperienceChanged -= this.OnPauseAdventuringExperienceChanged;
				}
			}
		}

		// Token: 0x0600410F RID: 16655 RVA: 0x0006C056 File Offset: 0x0006A256
		private void Update()
		{
			this.UpdateFill(this.m_progressFill, this.m_targetFillAmount);
			this.UpdateFill(this.m_fractionalProgressFill, this.m_targetFractionalFillAmount);
		}

		// Token: 0x06004110 RID: 16656 RVA: 0x0006C07C File Offset: 0x0006A27C
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x06004111 RID: 16657 RVA: 0x0018E170 File Offset: 0x0018C370
		private void Init()
		{
			if (LocalPlayer.GameEntity)
			{
				if (!LocalPlayer.GameEntity.CharacterData || !LocalPlayer.GameEntity.CharacterData.TryGetAsType(out this.m_playerCharacterData))
				{
					throw new ArgumentException("Invalid PlayerCharacterData!");
				}
				LocalPlayer.GameEntity.CharacterData.PauseAdventuringExperienceChanged += this.OnPauseAdventuringExperienceChanged;
				if (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
				{
					this.m_nMasteries = LocalPlayer.GameEntity.CollectionController.Masteries.Count;
					LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
					ArchetypeInstance selectedInstance;
					if (LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_playerCharacterData.TrackedMastery, out selectedInstance))
					{
						this.SelectedInstance = selectedInstance;
					}
				}
			}
			this.LevelDataChanged(true);
		}

		// Token: 0x06004112 RID: 16658 RVA: 0x0006C095 File Offset: 0x0006A295
		private void OnPauseAdventuringExperienceChanged()
		{
			this.RefreshIcon();
		}

		// Token: 0x06004113 RID: 16659 RVA: 0x0018E264 File Offset: 0x0018C464
		private void UpdateFill(FilledImage filledImage, float targetFillAmount)
		{
			if (filledImage.fillAmount > targetFillAmount || filledImage.fillAmount < targetFillAmount)
			{
				if (filledImage.fillAmount >= 1f)
				{
					filledImage.fillAmount = 0f;
					return;
				}
				float target = (filledImage.fillAmount > targetFillAmount) ? 1f : targetFillAmount;
				filledImage.fillAmount = Mathf.MoveTowards(filledImage.fillAmount, target, Time.deltaTime * this.m_fillSpeed);
			}
		}

		// Token: 0x06004114 RID: 16660 RVA: 0x0018E2CC File Offset: 0x0018C4CC
		private void MasteriesOnContentsChanged()
		{
			ArchetypeInstance archetypeInstance;
			if (this.SelectedInstance == null)
			{
				this.SelectAdventuringThenTradeInstance();
			}
			else if (this.SelectedInstance != null && !LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForInstanceId(this.SelectedInstance.InstanceId, out archetypeInstance))
			{
				this.SelectAdventuringThenTradeInstance();
			}
			this.m_nMasteries = ((LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null) ? LocalPlayer.GameEntity.CollectionController.Masteries.Count : 0);
		}

		// Token: 0x06004115 RID: 16661 RVA: 0x0018E360 File Offset: 0x0018C560
		private void SelectAdventuringThenTradeInstance()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				ArchetypeInstance archetypeInstance = null;
				foreach (ArchetypeInstance archetypeInstance2 in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
				{
					if (archetypeInstance2 != null && archetypeInstance2.Mastery != null)
					{
						MasteryType type = archetypeInstance2.Mastery.Type;
						if (type == MasteryType.Combat)
						{
							this.SelectedInstance = archetypeInstance2;
							return;
						}
						if (type - MasteryType.Trade <= 1)
						{
							if (archetypeInstance == null)
							{
								archetypeInstance = archetypeInstance2;
							}
							else if (archetypeInstance.MasteryData != null && archetypeInstance2.MasteryData != null && archetypeInstance.MasteryData.BaseLevel < archetypeInstance2.MasteryData.BaseLevel)
							{
								archetypeInstance = archetypeInstance2;
							}
						}
					}
				}
				if (archetypeInstance != null)
				{
					this.SelectedInstance = archetypeInstance;
				}
			}
		}

		// Token: 0x06004116 RID: 16662 RVA: 0x0006C09D File Offset: 0x0006A29D
		public void SelectMastery(ArchetypeInstance masteryInstance)
		{
			this.SelectedInstance = masteryInstance;
		}

		// Token: 0x06004117 RID: 16663 RVA: 0x0006C095 File Offset: 0x0006A295
		private void MasteryDataChanged()
		{
			this.RefreshIcon();
		}

		// Token: 0x06004118 RID: 16664 RVA: 0x0018E458 File Offset: 0x0018C658
		private void RefreshIcon()
		{
			if (this.m_bubble && this.m_bubble.Icon)
			{
				Sprite overrideSprite = null;
				if (this.m_selectedInstance != null && this.m_selectedInstance.Archetype != null)
				{
					overrideSprite = this.m_selectedInstance.Archetype.Icon;
					SpecializedRole specializedRole;
					if (this.m_selectedInstance.MasteryData != null && this.m_selectedInstance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(this.m_selectedInstance.MasteryData.Specialization.Value, out specializedRole))
					{
						overrideSprite = specializedRole.Icon;
					}
				}
				this.m_bubble.Icon.overrideSprite = overrideSprite;
			}
			if (this.m_pausePanel)
			{
				this.m_pausePanel.SetActive(this.ShowPauseIcon());
			}
		}

		// Token: 0x06004119 RID: 16665 RVA: 0x0018E53C File Offset: 0x0018C73C
		private bool ShowPauseIcon()
		{
			return this.SelectedInstance != null && this.SelectedInstance.Mastery && this.SelectedInstance.Mastery.Type == MasteryType.Combat && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.PauseAdventuringExperience;
		}

		// Token: 0x0600411A RID: 16666 RVA: 0x0006C0A6 File Offset: 0x0006A2A6
		private void LevelDataChanged()
		{
			this.LevelDataChanged(false);
		}

		// Token: 0x0600411B RID: 16667 RVA: 0x0018E5A4 File Offset: 0x0018C7A4
		private void LevelDataChanged(bool instant)
		{
			if (this.SelectedInstance != null)
			{
				float associatedLevel = this.SelectedInstance.GetAssociatedLevel(LocalPlayer.GameEntity);
				int num = Mathf.FloorToInt(associatedLevel);
				float num2 = (associatedLevel >= 50f) ? 1f : (associatedLevel - (float)num);
				float num3 = num2 * 5f;
				int num4 = Mathf.FloorToInt(num3);
				float num5 = (associatedLevel >= 50f) ? 1f : (num3 - (float)num4);
				this.m_leftText.SetTextFormat("{0}", num);
				this.m_rightText.SetTextFormat("{0}%", num2.GetAsPercentage());
				this.SetTickColors(num4);
				if (instant)
				{
					this.m_progressFill.fillAmount = num2;
					this.m_fractionalProgressFill.fillAmount = num5;
				}
				this.m_targetFillAmount = num2;
				this.m_targetFractionalFillAmount = num5;
				return;
			}
			this.m_leftText.text = "";
			this.m_rightText.text = "";
			this.SetTickColors(-1);
			if (instant)
			{
				this.m_progressFill.fillAmount = 0f;
				this.m_fractionalProgressFill.fillAmount = 0f;
			}
			this.m_targetFillAmount = 0f;
			this.m_targetFractionalFillAmount = 0f;
		}

		// Token: 0x0600411C RID: 16668 RVA: 0x0018E6CC File Offset: 0x0018C8CC
		private void SetTickColors(int index)
		{
			if (this.m_ticks != null && this.m_ticks.Length != 0)
			{
				for (int i = 0; i < this.m_ticks.Length; i++)
				{
					if (this.m_ticks[i])
					{
						this.m_ticks[i].color = ((i == index) ? this.m_selectedTickColor : this.m_defaultTickColor);
					}
				}
			}
		}

		// Token: 0x0600411D RID: 16669 RVA: 0x0018E72C File Offset: 0x0018C92C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.SelectedInstance == null)
			{
				return new ObjectTextTooltipParameter(this, "Right click to select and track a mastery's progress", false);
			}
			string additionalText = this.ShowPauseIcon() ? ZString.Format<string>("<b><color={0}>ADVENTURING XP PAUSED</color></b>", UIManager.RequirementsNotMetColor.ToHex()) : null;
			return new ArchetypeTooltipParameter
			{
				Instance = this.SelectedInstance,
				AdditionalText = additionalText
			};
		}

		// Token: 0x17000EE6 RID: 3814
		// (get) Token: 0x0600411E RID: 16670 RVA: 0x0006C0AF File Offset: 0x0006A2AF
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EE7 RID: 3815
		// (get) Token: 0x0600411F RID: 16671 RVA: 0x0006C0BD File Offset: 0x0006A2BD
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000EE8 RID: 3816
		// (get) Token: 0x06004120 RID: 16672 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004122 RID: 16674 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E8D RID: 16013
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003E8E RID: 16014
		[SerializeField]
		private FilledImage m_progressFill;

		// Token: 0x04003E8F RID: 16015
		[SerializeField]
		private FilledImage m_fractionalProgressFill;

		// Token: 0x04003E90 RID: 16016
		[SerializeField]
		private TextMeshProUGUI m_leftText;

		// Token: 0x04003E91 RID: 16017
		[SerializeField]
		private TextMeshProUGUI m_rightText;

		// Token: 0x04003E92 RID: 16018
		[SerializeField]
		private MasteryProgressBarBubble m_bubble;

		// Token: 0x04003E93 RID: 16019
		[SerializeField]
		private float m_fillSpeed = 0.1f;

		// Token: 0x04003E94 RID: 16020
		[SerializeField]
		private Image[] m_ticks;

		// Token: 0x04003E95 RID: 16021
		[SerializeField]
		private GameObject m_pausePanel;

		// Token: 0x04003E96 RID: 16022
		private int m_nMasteries;

		// Token: 0x04003E97 RID: 16023
		private float m_targetFillAmount;

		// Token: 0x04003E98 RID: 16024
		private float m_targetFractionalFillAmount;

		// Token: 0x04003E99 RID: 16025
		private ArchetypeInstance m_selectedInstance;

		// Token: 0x04003E9A RID: 16026
		private PlayerCharacterData m_playerCharacterData;

		// Token: 0x04003E9B RID: 16027
		private Color m_defaultTickColor = new Color(0.41960785f, 0.41960785f, 0.42352942f, 1f);

		// Token: 0x04003E9C RID: 16028
		private Color m_selectedTickColor = new Color(0.8627451f, 0.078431375f, 0.23529412f, 1f);
	}
}
