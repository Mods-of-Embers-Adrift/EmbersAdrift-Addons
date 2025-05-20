using System;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000867 RID: 2151
	[Obsolete]
	public class MasteryContainerUI : ContainerSlotUI
	{
		// Token: 0x06003E23 RID: 15907 RVA: 0x0018456C File Offset: 0x0018276C
		private void Awake()
		{
			if (ClientGameManager.UIManager != null)
			{
				this.m_containerUI = ClientGameManager.UIManager.MasteryPanel;
			}
			if (this.m_button != null && this.m_tab != null)
			{
				this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
			}
		}

		// Token: 0x06003E24 RID: 15908 RVA: 0x0006A023 File Offset: 0x00068223
		private void OnDestroy()
		{
			this.Unsubscribe();
			if (this.m_button != null && this.m_tab != null)
			{
				this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
			}
		}

		// Token: 0x06003E25 RID: 15909 RVA: 0x001845D0 File Offset: 0x001827D0
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			if (this.m_instance != null)
			{
				this.InstanceRemoved(this.m_instance);
			}
			if (instance.InstanceUI != null)
			{
				UnityEngine.Object.Destroy(instance.InstanceUI.gameObject);
			}
			base.InstanceAdded(instance);
			this.m_icon.overrideSprite = instance.Archetype.Icon;
			this.m_name.text = instance.Archetype.DisplayName;
			if (this.m_tab != null)
			{
				this.m_tab.gameObject.SetActive(true);
			}
			this.MasteryDataOnMasteryDataChanged();
			this.Subscribe();
		}

		// Token: 0x06003E26 RID: 15910 RVA: 0x0006A063 File Offset: 0x00068263
		public override void InstanceRemoved(ArchetypeInstance instance)
		{
			this.Unsubscribe();
			base.InstanceRemoved(instance);
			this.m_icon.overrideSprite = null;
			this.m_name.text = null;
		}

		// Token: 0x06003E27 RID: 15911 RVA: 0x0006A08A File Offset: 0x0006828A
		private void ButtonClicked()
		{
			this.m_tab.isOn = true;
		}

		// Token: 0x06003E28 RID: 15912 RVA: 0x00184670 File Offset: 0x00182870
		private void MasteryDataOnMasteryDataChanged()
		{
			float associatedLevel = this.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity);
			int num = Mathf.FloorToInt(associatedLevel);
			this.m_level.text = num.ToString();
			this.m_progress.fillAmount = associatedLevel - (float)num;
		}

		// Token: 0x06003E29 RID: 15913 RVA: 0x0006A098 File Offset: 0x00068298
		private void Subscribe()
		{
			if (this.m_instance != null && this.m_instance.MasteryData != null)
			{
				this.m_instance.MasteryData.LevelDataChanged += this.MasteryDataOnMasteryDataChanged;
			}
		}

		// Token: 0x06003E2A RID: 15914 RVA: 0x0006A0CB File Offset: 0x000682CB
		private void Unsubscribe()
		{
			if (this.m_instance != null && this.m_instance.MasteryData != null)
			{
				this.m_instance.MasteryData.LevelDataChanged -= this.MasteryDataOnMasteryDataChanged;
			}
		}

		// Token: 0x04003C74 RID: 15476
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04003C75 RID: 15477
		[SerializeField]
		private TextMeshProUGUI m_level;

		// Token: 0x04003C76 RID: 15478
		[SerializeField]
		private Image m_progress;

		// Token: 0x04003C77 RID: 15479
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003C78 RID: 15480
		[SerializeField]
		private SolToggle m_tab;
	}
}
