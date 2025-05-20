using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008AB RID: 2219
	[Obsolete]
	public class MasteryPanelSlotUI : ContainerSlotUI
	{
		// Token: 0x17000EDF RID: 3807
		// (get) Token: 0x060040E5 RID: 16613 RVA: 0x0006BEC5 File Offset: 0x0006A0C5
		public RectTransform Content
		{
			get
			{
				return this.m_content;
			}
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x0006BECD File Offset: 0x0006A0CD
		private void Awake()
		{
			this.m_containerUI = ClientGameManager.UIManager.MasteryPanel;
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x0006BEDF File Offset: 0x0006A0DF
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x0006BEE7 File Offset: 0x0006A0E7
		private void Subscribe()
		{
			if (this.m_instance != null && this.m_instance.MasteryData != null)
			{
				this.m_instance.MasteryData.LevelDataChanged += this.MasteryDataOnMasteryDataChanged;
			}
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x0006BF1A File Offset: 0x0006A11A
		private void Unsubscribe()
		{
			if (this.m_instance != null && this.m_instance.MasteryData != null)
			{
				this.m_instance.MasteryData.LevelDataChanged -= this.MasteryDataOnMasteryDataChanged;
			}
		}

		// Token: 0x060040EA RID: 16618 RVA: 0x0018DCBC File Offset: 0x0018BEBC
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			if (instance.InstanceUI != null)
			{
				UnityEngine.Object.Destroy(instance.InstanceUI.gameObject);
			}
			base.InstanceAdded(instance);
			this.m_image.overrideSprite = instance.Archetype.Icon;
			this.m_name.text = instance.Archetype.DisplayName;
			this.MasteryDataOnMasteryDataChanged();
			this.Subscribe();
		}

		// Token: 0x060040EB RID: 16619 RVA: 0x0006BF4D File Offset: 0x0006A14D
		public override void InstanceRemoved(ArchetypeInstance instance)
		{
			this.Unsubscribe();
			base.InstanceRemoved(instance);
			this.m_image.overrideSprite = null;
			this.m_name.text = null;
		}

		// Token: 0x060040EC RID: 16620 RVA: 0x0018DD28 File Offset: 0x0018BF28
		private void MasteryDataOnMasteryDataChanged()
		{
			float associatedLevel = this.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity);
			int num = Mathf.FloorToInt(associatedLevel);
			this.m_level.text = num.ToString();
			this.m_progress.fillAmount = associatedLevel - (float)num;
		}

		// Token: 0x060040ED RID: 16621 RVA: 0x0006BF74 File Offset: 0x0006A174
		public void AbilityAdded(ArchetypeInstance instance)
		{
			if (!this.m_abilityInstances.Contains(instance))
			{
				this.m_abilityInstances.Add(instance);
			}
			this.RefreshAbilities();
		}

		// Token: 0x060040EE RID: 16622 RVA: 0x0018DD70 File Offset: 0x0018BF70
		public void RefreshAbilities()
		{
			this.m_abilityInstances.Sort((ArchetypeInstance a, ArchetypeInstance b) => a.Ability.MinimumLevel.CompareTo(b.Ability.MinimumLevel));
			int count = this.m_abilityInstances.Count;
			for (int i = 0; i < count; i++)
			{
				if (i > this.m_abilityPanelSlots.Count - 1)
				{
					AbilityPanelSlotUI component = UnityEngine.Object.Instantiate<GameObject>(this.m_abilityPrefab, this.m_content).GetComponent<AbilityPanelSlotUI>();
					this.m_abilityPanelSlots.Add(component);
				}
				this.m_abilityPanelSlots[i].gameObject.SetActive(true);
				this.m_abilityPanelSlots[i].InstanceAdded(this.m_abilityInstances[i]);
			}
			if (this.m_abilityPanelSlots.Count > count)
			{
				for (int j = count; j < this.m_abilityPanelSlots.Count; j++)
				{
					this.m_abilityPanelSlots[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060040EF RID: 16623 RVA: 0x001820F0 File Offset: 0x001802F0
		protected override ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instance
			};
		}

		// Token: 0x04003E7D RID: 15997
		[SerializeField]
		private GameObject m_abilityPrefab;

		// Token: 0x04003E7E RID: 15998
		[SerializeField]
		private Image m_image;

		// Token: 0x04003E7F RID: 15999
		[SerializeField]
		private TextMeshProUGUI m_name;

		// Token: 0x04003E80 RID: 16000
		[SerializeField]
		private TextMeshProUGUI m_level;

		// Token: 0x04003E81 RID: 16001
		[SerializeField]
		private RectTransform m_content;

		// Token: 0x04003E82 RID: 16002
		[SerializeField]
		private Image m_progress;

		// Token: 0x04003E83 RID: 16003
		private readonly List<ArchetypeInstance> m_abilityInstances = new List<ArchetypeInstance>();

		// Token: 0x04003E84 RID: 16004
		private readonly List<AbilityPanelSlotUI> m_abilityPanelSlots = new List<AbilityPanelSlotUI>();
	}
}
