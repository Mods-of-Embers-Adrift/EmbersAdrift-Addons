using System;
using Cysharp.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000868 RID: 2152
	public class MasteryDetailsUI : MonoBehaviour
	{
		// Token: 0x17000E5B RID: 3675
		// (get) Token: 0x06003E2C RID: 15916 RVA: 0x0006A0FE File Offset: 0x000682FE
		// (set) Token: 0x06003E2D RID: 15917 RVA: 0x0006A106 File Offset: 0x00068306
		public ArchetypeInstance Instance { get; private set; }

		// Token: 0x17000E5C RID: 3676
		// (get) Token: 0x06003E2E RID: 15918 RVA: 0x0006A10F File Offset: 0x0006830F
		public SolToggle Tab
		{
			get
			{
				return this.m_tab;
			}
		}

		// Token: 0x06003E2F RID: 15919 RVA: 0x00184714 File Offset: 0x00182914
		private void Awake()
		{
			if (this.m_button != null && this.m_tab != null)
			{
				this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
			}
			if (this.m_trainingToggle != null)
			{
				this.m_trainingToggle.gameObject.SetActive(false);
			}
		}

		// Token: 0x06003E30 RID: 15920 RVA: 0x0006A117 File Offset: 0x00068317
		private void OnDestroy()
		{
			if (this.m_button != null && this.m_tab != null)
			{
				this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
			}
		}

		// Token: 0x06003E31 RID: 15921 RVA: 0x0006A151 File Offset: 0x00068351
		private void ButtonClicked()
		{
			this.m_tab.isOn = true;
		}

		// Token: 0x06003E32 RID: 15922 RVA: 0x00184778 File Offset: 0x00182978
		public void RegisterMastery(ArchetypeInstance instance)
		{
			if (this.Instance != null)
			{
				this.UnregisterMastery();
			}
			this.Instance = instance;
			this.Instance.MasteryData.MasteryDataChanged += this.RefreshMasteryName;
			this.Instance.MasteryData.LevelDataChanged += this.OnLevelChanged;
			LocalPlayer.GameEntity.Vitals.StatsChanged += this.OnLevelChanged;
			this.m_icon.overrideSprite = instance.Archetype.Icon;
			this.m_icon.color = instance.Archetype.IconTint;
			this.m_frame.color = instance.Archetype.FrameColor;
			if (this.m_tabIcon != null)
			{
				this.m_tabIcon.overrideSprite = instance.Archetype.Icon;
			}
			this.RefreshMasteryName();
			MasteryArchetype masteryArchetype;
			if (instance.Archetype.TryGetAsType(out masteryArchetype))
			{
				this.m_details.text = "";
			}
			this.m_masteryIconUi.AssignInstance(instance);
			this.OnLevelChanged();
			if (this.m_button != null)
			{
				this.m_button.interactable = true;
			}
			if (this.m_tab != null)
			{
				this.m_tab.gameObject.SetActive(true);
			}
		}

		// Token: 0x06003E33 RID: 15923 RVA: 0x001848C4 File Offset: 0x00182AC4
		public void UnregisterMastery()
		{
			if (this.Instance != null)
			{
				this.Instance.MasteryData.MasteryDataChanged -= this.RefreshMasteryName;
				this.Instance.MasteryData.LevelDataChanged -= this.OnLevelChanged;
				LocalPlayer.GameEntity.Vitals.StatsChanged -= this.OnLevelChanged;
				this.Instance = null;
			}
			if (this.m_button != null)
			{
				this.m_button.interactable = false;
			}
			if (this.m_tab != null)
			{
				this.m_tab.gameObject.SetActive(false);
			}
			this.m_masteryIconUi.AssignInstance(null);
			this.m_icon.overrideSprite = null;
			this.m_icon.color = Color.white;
			if (this.m_tabIcon != null)
			{
				this.m_tabIcon.overrideSprite = null;
			}
			this.m_name.text = null;
			this.m_details.text = null;
			this.m_progress.fillAmount = 0f;
			this.Instance = null;
		}

		// Token: 0x06003E34 RID: 15924 RVA: 0x001849E0 File Offset: 0x00182BE0
		private void OnLevelChanged()
		{
			float baseLevel = this.Instance.MasteryData.BaseLevel;
			int num = Mathf.FloorToInt(baseLevel);
			this.m_level.text = num.ToString() + "/100";
			this.m_progress.fillAmount = ((baseLevel >= 100f) ? 1f : (baseLevel - (float)num));
		}

		// Token: 0x06003E35 RID: 15925 RVA: 0x00184A40 File Offset: 0x00182C40
		private void RefreshMasteryName()
		{
			if (this.Instance == null || this.Instance.Mastery == null)
			{
				this.m_name.text = string.Empty;
				return;
			}
			SpecializedRole specializedRole;
			if (this.Instance.Mastery.HasSpecializations && this.Instance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(this.Instance.MasteryData.Specialization.Value, out specializedRole))
			{
				this.m_name.SetTextFormat("{0}\n<size={1}%>{2}", specializedRole.DisplayName, 60, this.Instance.Mastery.DisplayName);
				return;
			}
			this.m_name.SetTextFormat("{0}", this.Instance.Mastery.DisplayName);
		}

		// Token: 0x04003C79 RID: 15481
		public const int kBaseSizeWithSpec = 60;

		// Token: 0x04003C7A RID: 15482
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04003C7B RID: 15483
		[SerializeField]
		private TextMeshProUGUI m_details;

		// Token: 0x04003C7C RID: 15484
		[SerializeField]
		private TextMeshProUGUI m_level;

		// Token: 0x04003C7D RID: 15485
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003C7E RID: 15486
		[SerializeField]
		private Image m_progress;

		// Token: 0x04003C7F RID: 15487
		[SerializeField]
		private Image m_frame;

		// Token: 0x04003C80 RID: 15488
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003C81 RID: 15489
		[SerializeField]
		private SolToggle m_tab;

		// Token: 0x04003C82 RID: 15490
		[SerializeField]
		private Image m_tabIcon;

		// Token: 0x04003C83 RID: 15491
		[SerializeField]
		private MasteryIconUI m_masteryIconUi;

		// Token: 0x04003C84 RID: 15492
		[SerializeField]
		private SolToggle m_trainingToggle;
	}
}
