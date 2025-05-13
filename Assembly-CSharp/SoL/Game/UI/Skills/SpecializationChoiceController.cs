using System;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000933 RID: 2355
	public class SpecializationChoiceController : MonoBehaviour
	{
		// Token: 0x0600456A RID: 17770 RVA: 0x0019FB8C File Offset: 0x0019DD8C
		public void InitializeMasteryChoice(ArchetypeInstance masteryInstance)
		{
			if (masteryInstance == null || masteryInstance.Mastery == null || !masteryInstance.Mastery.HasSpecializations)
			{
				base.gameObject.SetActive(false);
				return;
			}
			SpecializedRole specializedRole = null;
			if (masteryInstance.MasteryData.Specialization != null)
			{
				InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(masteryInstance.MasteryData.Specialization.Value, out specializedRole);
			}
			BaseRole baseRole = (specializedRole != null) ? specializedRole.GeneralRole : (masteryInstance.Mastery as BaseRole);
			if (baseRole == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			SpecializedRole[] specializations = baseRole.Specializations;
			int num = 0;
			for (int i = 0; i < specializations.Length; i++)
			{
				if (i < this.m_specializationChoices.Length)
				{
					this.m_specializationChoices[num].SetArchetype(masteryInstance, baseRole, specializations[i]);
					num++;
				}
			}
			for (int j = num; j < this.m_specializationChoices.Length; j++)
			{
				this.m_specializationChoices[j].SetArchetype(null, null, null);
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x040041D0 RID: 16848
		[SerializeField]
		private SpecializationChoiceIcon[] m_specializationChoices;
	}
}
