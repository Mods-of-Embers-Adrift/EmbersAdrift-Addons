using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000865 RID: 2149
	public class MasteryAbilityContainerUI : ContainerSlotUI
	{
		// Token: 0x06003E18 RID: 15896 RVA: 0x00069F98 File Offset: 0x00068198
		private void Awake()
		{
			if (ClientGameManager.UIManager != null)
			{
				this.m_containerUI = ClientGameManager.UIManager.MasteryPanel;
			}
		}

		// Token: 0x06003E19 RID: 15897 RVA: 0x001843F4 File Offset: 0x001825F4
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
			this.m_masterDetailsUi.RegisterMastery(instance);
			this.RefreshAbilities();
		}

		// Token: 0x06003E1A RID: 15898 RVA: 0x00069FB7 File Offset: 0x000681B7
		public override void InstanceRemoved(ArchetypeInstance instance)
		{
			this.m_masterDetailsUi.UnregisterMastery();
			base.InstanceRemoved(instance);
		}

		// Token: 0x06003E1B RID: 15899 RVA: 0x00069FCB File Offset: 0x000681CB
		public void AbilityAdded(ArchetypeInstance instance)
		{
			if (!this.m_abilityInstances.Contains(instance))
			{
				this.m_abilityInstances.Add(instance);
			}
			this.RefreshAbilities();
		}

		// Token: 0x06003E1C RID: 15900 RVA: 0x00069FED File Offset: 0x000681ED
		public void AbilityRemoved(ArchetypeInstance instance)
		{
			this.m_abilityInstances.Remove(instance);
			this.RefreshAbilities();
		}

		// Token: 0x06003E1D RID: 15901 RVA: 0x0018444C File Offset: 0x0018264C
		public void RefreshAbilities()
		{
			if (this.m_abilityInstances.Count > 1)
			{
				this.m_abilityInstances.Sort((ArchetypeInstance a, ArchetypeInstance b) => a.Ability.MinimumLevel.CompareTo(b.Ability.MinimumLevel));
			}
			int count = this.m_abilityInstances.Count;
			for (int i = 0; i < count; i++)
			{
				if (i > this.m_abilities.Length)
				{
					Debug.LogWarning("TOO MANY ABILITIES! " + i.ToString() + " > " + this.m_abilities.Length.ToString());
				}
				else
				{
					this.m_abilities[i].gameObject.SetActive(true);
					this.m_abilities[i].InstanceAdded(this.m_abilityInstances[i]);
				}
			}
			if (this.m_abilities.Length > count)
			{
				for (int j = count; j < this.m_abilities.Length; j++)
				{
					this.m_abilities[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06003E1E RID: 15902 RVA: 0x001820F0 File Offset: 0x001802F0
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

		// Token: 0x04003C6F RID: 15471
		[SerializeField]
		private MasteryDetailsUI m_masterDetailsUi;

		// Token: 0x04003C70 RID: 15472
		[SerializeField]
		private AbilityDetailsUI[] m_abilities;

		// Token: 0x04003C71 RID: 15473
		private readonly List<ArchetypeInstance> m_abilityInstances = new List<ArchetypeInstance>(10);
	}
}
