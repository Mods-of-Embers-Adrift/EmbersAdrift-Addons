using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA0 RID: 2720
	[CreateAssetMenu(menuName = "SoL/Profiles/Role Restriction")]
	public class RoleRequirementProfile : ScriptableObject, IRequirement
	{
		// Token: 0x06005429 RID: 21545 RVA: 0x000784A8 File Offset: 0x000766A8
		public bool MeetsAllRequirements(GameEntity entity)
		{
			return this.m_baseRoleRequirement != null && this.m_baseRoleRequirement.MeetsAllRequirements(entity);
		}

		// Token: 0x0600542A RID: 21546 RVA: 0x000784C0 File Offset: 0x000766C0
		public bool ShowRoleRequirement(GameEntity entity, out bool meetsRoleRequirement, out string roleString)
		{
			meetsRoleRequirement = false;
			roleString = string.Empty;
			return this.m_baseRoleRequirement != null && this.m_baseRoleRequirement.ShowRoleRequirement(entity, out meetsRoleRequirement, out roleString);
		}

		// Token: 0x0600542B RID: 21547 RVA: 0x000784E4 File Offset: 0x000766E4
		public bool MatchesNameFilter(string filter)
		{
			return this.m_baseRoleRequirement != null && this.m_baseRoleRequirement.MatchesNameFilter(filter);
		}

		// Token: 0x04004AF6 RID: 19190
		[SerializeField]
		private BaseRoleRequirement m_baseRoleRequirement;
	}
}
