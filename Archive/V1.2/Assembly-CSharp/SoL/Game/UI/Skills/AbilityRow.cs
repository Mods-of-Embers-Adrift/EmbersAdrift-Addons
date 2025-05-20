using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000920 RID: 2336
	public class AbilityRow : AbilityGrouping
	{
		// Token: 0x17000F6A RID: 3946
		// (get) Token: 0x060044C7 RID: 17607 RVA: 0x0006E78E File Offset: 0x0006C98E
		private static AbilityRow.AbilityLevel[] AbilityLevelList
		{
			get
			{
				if (AbilityRow.m_abilityLevelList == null)
				{
					AbilityRow.m_abilityLevelList = (AbilityRow.AbilityLevel[])Enum.GetValues(typeof(AbilityRow.AbilityLevel));
				}
				return AbilityRow.m_abilityLevelList;
			}
		}

		// Token: 0x060044C8 RID: 17608 RVA: 0x0019D9CC File Offset: 0x0019BBCC
		public bool UpdateAbilitiesForMastery(ArchetypeInstance masteryInstance, Dictionary<UniqueId, AbilitySlot> localSlots)
		{
			if (masteryInstance == null || masteryInstance.Mastery == null)
			{
				for (int i = 0; i < this.m_slots.Length; i++)
				{
					this.m_slots[i].AssignAbility(null);
				}
				return false;
			}
			MasteryArchetype mastery = masteryInstance.Mastery;
			int num = 0;
			List<AbilityArchetype> abilitiesSortedByLevel = mastery.GetAbilitiesSortedByLevel();
			if (abilitiesSortedByLevel != null)
			{
				for (int j = 0; j < abilitiesSortedByLevel.Count; j++)
				{
					AbilityArchetype abilityArchetype = abilitiesSortedByLevel[j];
					if (!(abilityArchetype == null) && !localSlots.ContainsKey(abilityArchetype.Id) && (!(abilityArchetype.Specialization != null) || (masteryInstance.MasteryData.Specialization != null && !(masteryInstance.MasteryData.Specialization.Value != abilityArchetype.Specialization.Id))))
					{
						bool flag;
						if (this.m_useManualLevel)
						{
							flag = (abilityArchetype.MinimumLevel <= this.m_manualLevelMax);
						}
						else
						{
							flag = (this.GetAbilityLevel(abilityArchetype) == this.m_level);
						}
						if (flag && num < this.m_slots.Length)
						{
							this.m_slots[num].AssignAbility(abilityArchetype);
							localSlots.Add(abilityArchetype.Id, this.m_slots[num]);
							num++;
						}
					}
				}
			}
			for (int k = num; k < this.m_slots.Length; k++)
			{
				this.m_slots[k].AssignAbility(null);
			}
			return num != 0;
		}

		// Token: 0x060044C9 RID: 17609 RVA: 0x0019DB44 File Offset: 0x0019BD44
		private AbilityRow.AbilityLevel GetAbilityLevel(AbilityArchetype abil)
		{
			for (int i = AbilityRow.AbilityLevelList.Length - 1; i >= 0; i--)
			{
				if (abil.LevelRange.Min >= (int)AbilityRow.AbilityLevelList[i])
				{
					return AbilityRow.AbilityLevelList[i];
				}
			}
			return AbilityRow.AbilityLevel.lvl100;
		}

		// Token: 0x060044CA RID: 17610 RVA: 0x0019DB88 File Offset: 0x0019BD88
		public bool HasAbilities()
		{
			bool flag = false;
			for (int i = 0; i < this.m_slots.Length; i++)
			{
				flag = (flag || this.m_slots[i].Ability != null);
			}
			return flag;
		}

		// Token: 0x0400415D RID: 16733
		[SerializeField]
		private bool m_useManualLevel;

		// Token: 0x0400415E RID: 16734
		[SerializeField]
		private AbilityRow.AbilityLevel m_level;

		// Token: 0x0400415F RID: 16735
		[SerializeField]
		private int m_manualLevelMax = 1;

		// Token: 0x04004160 RID: 16736
		private static AbilityRow.AbilityLevel[] m_abilityLevelList;

		// Token: 0x02000921 RID: 2337
		private enum AbilityLevel
		{
			// Token: 0x04004162 RID: 16738
			None,
			// Token: 0x04004163 RID: 16739
			lvl1,
			// Token: 0x04004164 RID: 16740
			lvl5 = 5,
			// Token: 0x04004165 RID: 16741
			lvl10 = 10,
			// Token: 0x04004166 RID: 16742
			lvl15 = 15,
			// Token: 0x04004167 RID: 16743
			lvl20 = 20,
			// Token: 0x04004168 RID: 16744
			lvl30 = 30,
			// Token: 0x04004169 RID: 16745
			lvl40 = 40,
			// Token: 0x0400416A RID: 16746
			lvl50 = 50,
			// Token: 0x0400416B RID: 16747
			lvl60 = 60,
			// Token: 0x0400416C RID: 16748
			lvl70 = 70,
			// Token: 0x0400416D RID: 16749
			lvl80 = 80,
			// Token: 0x0400416E RID: 16750
			lvl90 = 90,
			// Token: 0x0400416F RID: 16751
			lvl100 = 100
		}
	}
}
