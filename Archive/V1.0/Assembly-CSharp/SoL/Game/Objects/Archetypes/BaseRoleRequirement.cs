using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A9C RID: 2716
	[Serializable]
	public class BaseRoleRequirement : IRequirement
	{
		// Token: 0x1700134A RID: 4938
		// (get) Token: 0x0600540D RID: 21517 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_hasOverride
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700134B RID: 4939
		// (get) Token: 0x0600540E RID: 21518 RVA: 0x00078321 File Offset: 0x00076521
		private bool m_hasBaseRoleRequirement
		{
			get
			{
				return !this.m_hasOverride && (this.m_type == BaseRoleRequirement.RoleRequirementType.Base || this.m_type == BaseRoleRequirement.RoleRequirementType.Either);
			}
		}

		// Token: 0x1700134C RID: 4940
		// (get) Token: 0x0600540F RID: 21519 RVA: 0x00078341 File Offset: 0x00076541
		private bool m_hasSpecializedRoleRequirement
		{
			get
			{
				return !this.m_hasOverride && (this.m_type == BaseRoleRequirement.RoleRequirementType.Spec || this.m_type == BaseRoleRequirement.RoleRequirementType.Either);
			}
		}

		// Token: 0x06005410 RID: 21520 RVA: 0x001D9C24 File Offset: 0x001D7E24
		public virtual bool MeetsAllRequirements(GameEntity entity)
		{
			if (!entity || !entity.CharacterData)
			{
				return false;
			}
			switch (this.m_type)
			{
			case BaseRoleRequirement.RoleRequirementType.None:
				return true;
			case BaseRoleRequirement.RoleRequirementType.Base:
				return this.BaseRoleIsCompatible(entity.CharacterData.BaseRoleId);
			case BaseRoleRequirement.RoleRequirementType.Spec:
				return this.SpecializedRoleIsCompatible(entity.CharacterData.SpecializedRoleId);
			case BaseRoleRequirement.RoleRequirementType.Either:
				return this.BaseRoleIsCompatible(entity.CharacterData.BaseRoleId) || this.SpecializedRoleIsCompatible(entity.CharacterData.SpecializedRoleId);
			default:
				return false;
			}
		}

		// Token: 0x06005411 RID: 21521 RVA: 0x001D9CB8 File Offset: 0x001D7EB8
		private bool BaseRoleIsCompatible(UniqueId baseRoleId)
		{
			if (this.m_baseRoles == BaseRoleFlags.None)
			{
				return true;
			}
			if (baseRoleId.IsEmpty)
			{
				return false;
			}
			BaseRoleFlags baseRoleFlag = GlobalSettings.Values.Roles.GetBaseRoleFlag(baseRoleId);
			return baseRoleFlag != BaseRoleFlags.None && this.m_baseRoles.HasBitFlag(baseRoleFlag);
		}

		// Token: 0x06005412 RID: 21522 RVA: 0x001D9CFC File Offset: 0x001D7EFC
		private bool SpecializedRoleIsCompatible(UniqueId specializedRoleId)
		{
			if (this.m_specializedRoles == SpecializedRoleFlags.None)
			{
				return true;
			}
			if (specializedRoleId.IsEmpty)
			{
				return false;
			}
			SpecializedRoleFlags specializedRoleFlag = GlobalSettings.Values.Roles.GetSpecializedRoleFlag(specializedRoleId);
			return specializedRoleFlag != SpecializedRoleFlags.None && this.m_specializedRoles.HasBitFlag(specializedRoleFlag);
		}

		// Token: 0x06005413 RID: 21523 RVA: 0x001D9D40 File Offset: 0x001D7F40
		public virtual bool ShowRoleRequirement(GameEntity entity, out bool meetsRoleRequirement, out string roleString)
		{
			meetsRoleRequirement = false;
			roleString = string.Empty;
			if (this.m_type != BaseRoleRequirement.RoleRequirementType.None)
			{
				if (this.m_hasSpecializedRoleRequirement)
				{
					bool flag = entity && entity.CharacterData && this.SpecializedRoleIsCompatible(entity.CharacterData.SpecializedRoleId);
					SpecializedRoleFlags selectedFlag = flag ? GlobalSettings.Values.Roles.GetSpecializedRoleFlag(entity.CharacterData.SpecializedRoleId) : SpecializedRoleFlags.None;
					meetsRoleRequirement = flag;
					roleString = this.m_specializedRoles.GetSpecializationRoleDescription(selectedFlag, flag);
				}
				else if (this.m_hasBaseRoleRequirement)
				{
					bool flag2 = entity && entity.CharacterData && this.BaseRoleIsCompatible(entity.CharacterData.BaseRoleId);
					BaseRoleFlags selectedFlag2 = flag2 ? GlobalSettings.Values.Roles.GetBaseRoleFlag(entity.CharacterData.BaseRoleId) : BaseRoleFlags.None;
					meetsRoleRequirement = flag2;
					roleString = this.m_baseRoles.GetBaseRoleDescription(selectedFlag2, flag2);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005414 RID: 21524 RVA: 0x001D9E34 File Offset: 0x001D8034
		public virtual bool MatchesNameFilter(string filter)
		{
			switch (this.m_type)
			{
			case BaseRoleRequirement.RoleRequirementType.Base:
				return this.m_baseRoles.MatchesNameFilter(filter);
			case BaseRoleRequirement.RoleRequirementType.Spec:
				return this.m_specializedRoles.MatchesNameFilter(filter);
			case BaseRoleRequirement.RoleRequirementType.Either:
				return this.m_baseRoles.MatchesNameFilter(filter) || this.m_specializedRoles.MatchesNameFilter(filter);
			default:
				return false;
			}
		}

		// Token: 0x04004AEB RID: 19179
		[SerializeField]
		private BaseRoleRequirement.RoleRequirementType m_type;

		// Token: 0x04004AEC RID: 19180
		[SerializeField]
		private BaseRoleFlags m_baseRoles;

		// Token: 0x04004AED RID: 19181
		[SerializeField]
		private SpecializedRoleFlags m_specializedRoles;

		// Token: 0x02000A9D RID: 2717
		private enum RoleRequirementType
		{
			// Token: 0x04004AEF RID: 19183
			None,
			// Token: 0x04004AF0 RID: 19184
			Base,
			// Token: 0x04004AF1 RID: 19185
			Spec,
			// Token: 0x04004AF2 RID: 19186
			Either
		}
	}
}
