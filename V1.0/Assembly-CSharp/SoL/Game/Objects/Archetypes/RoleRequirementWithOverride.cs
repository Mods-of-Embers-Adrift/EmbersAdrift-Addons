using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A9E RID: 2718
	[Serializable]
	public class RoleRequirementWithOverride : BaseRoleRequirement
	{
		// Token: 0x06005416 RID: 21526 RVA: 0x00078361 File Offset: 0x00076561
		public override bool MeetsAllRequirements(GameEntity entity)
		{
			if (!this.m_override)
			{
				return base.MeetsAllRequirements(entity);
			}
			return this.m_override.MeetsAllRequirements(entity);
		}

		// Token: 0x06005417 RID: 21527 RVA: 0x00078384 File Offset: 0x00076584
		public override bool ShowRoleRequirement(GameEntity entity, out bool meetsRoleRequirement, out string roleString)
		{
			meetsRoleRequirement = false;
			roleString = string.Empty;
			if (!this.m_override)
			{
				return base.ShowRoleRequirement(entity, out meetsRoleRequirement, out roleString);
			}
			return this.m_override.ShowRoleRequirement(entity, out meetsRoleRequirement, out roleString);
		}

		// Token: 0x06005418 RID: 21528 RVA: 0x000783B5 File Offset: 0x000765B5
		public override bool MatchesNameFilter(string filter)
		{
			if (!this.m_override)
			{
				return base.MatchesNameFilter(filter);
			}
			return this.m_override.MatchesNameFilter(filter);
		}

		// Token: 0x1700134D RID: 4941
		// (get) Token: 0x06005419 RID: 21529 RVA: 0x000783D8 File Offset: 0x000765D8
		protected override bool m_hasOverride
		{
			get
			{
				return this.m_override != null;
			}
		}

		// Token: 0x1700134E RID: 4942
		// (get) Token: 0x0600541A RID: 21530 RVA: 0x000783E6 File Offset: 0x000765E6
		private IEnumerable GetRoleRequirementProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<RoleRequirementProfile>();
			}
		}

		// Token: 0x04004AF3 RID: 19187
		[SerializeField]
		private RoleRequirementProfile m_override;
	}
}
