using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200073F RID: 1855
	[Serializable]
	public class RoleSettings
	{
		// Token: 0x17000C7C RID: 3196
		// (get) Token: 0x06003771 RID: 14193 RVA: 0x00065EE9 File Offset: 0x000640E9
		private IEnumerable GetBaseRoles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BaseRole>();
			}
		}

		// Token: 0x17000C7D RID: 3197
		// (get) Token: 0x06003772 RID: 14194 RVA: 0x00065EF0 File Offset: 0x000640F0
		private IEnumerable GetSpecializedRoles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<SpecializedRole>();
			}
		}

		// Token: 0x06003773 RID: 14195 RVA: 0x0016BA9C File Offset: 0x00169C9C
		public BaseRoleFlags GetBaseRoleFlag(UniqueId baseRoleId)
		{
			if (this.m_baseRoleFlagDict == null)
			{
				this.m_baseRoleFlagDict = new Dictionary<UniqueId, BaseRoleFlags>(default(UniqueIdComparer));
				if (this.m_defender)
				{
					this.m_baseRoleFlagDict.Add(this.m_defender.Id, BaseRoleFlags.Defender);
				}
				if (this.m_striker)
				{
					this.m_baseRoleFlagDict.Add(this.m_striker.Id, BaseRoleFlags.Striker);
				}
				if (this.m_supporter)
				{
					this.m_baseRoleFlagDict.Add(this.m_supporter.Id, BaseRoleFlags.Supporter);
				}
			}
			BaseRoleFlags result;
			if (!this.m_baseRoleFlagDict.TryGetValue(baseRoleId, out result))
			{
				return BaseRoleFlags.None;
			}
			return result;
		}

		// Token: 0x06003774 RID: 14196 RVA: 0x0016BB4C File Offset: 0x00169D4C
		public SpecializedRoleFlags GetSpecializedRoleFlag(UniqueId specializedRoleId)
		{
			if (this.m_specializationRoleFlagDict == null)
			{
				this.m_specializationRoleFlagDict = new Dictionary<UniqueId, SpecializedRoleFlags>(default(UniqueIdComparer));
				if (this.m_juggernaut)
				{
					this.m_specializationRoleFlagDict.Add(this.m_juggernaut.Id, SpecializedRoleFlags.Juggernaut);
				}
				if (this.m_knight)
				{
					this.m_specializationRoleFlagDict.Add(this.m_knight.Id, SpecializedRoleFlags.Knight);
				}
				if (this.m_marshal)
				{
					this.m_specializationRoleFlagDict.Add(this.m_marshal.Id, SpecializedRoleFlags.Marshal);
				}
				if (this.m_berserker)
				{
					this.m_specializationRoleFlagDict.Add(this.m_berserker.Id, SpecializedRoleFlags.Berserker);
				}
				if (this.m_brigand)
				{
					this.m_specializationRoleFlagDict.Add(this.m_brigand.Id, SpecializedRoleFlags.Brigand);
				}
				if (this.m_warden)
				{
					this.m_specializationRoleFlagDict.Add(this.m_warden.Id, SpecializedRoleFlags.Warden);
				}
				if (this.m_duelist)
				{
					this.m_specializationRoleFlagDict.Add(this.m_duelist.Id, SpecializedRoleFlags.Duelist);
				}
				if (this.m_sentinel)
				{
					this.m_specializationRoleFlagDict.Add(this.m_sentinel.Id, SpecializedRoleFlags.Sentinel);
				}
				if (this.m_warlord)
				{
					this.m_specializationRoleFlagDict.Add(this.m_warlord.Id, SpecializedRoleFlags.Warlord);
				}
			}
			SpecializedRoleFlags result;
			if (!this.m_specializationRoleFlagDict.TryGetValue(specializedRoleId, out result))
			{
				return SpecializedRoleFlags.None;
			}
			return result;
		}

		// Token: 0x06003775 RID: 14197 RVA: 0x0016BCE0 File Offset: 0x00169EE0
		public BaseArchetype GetRoleFromPacked(RolePacked packed)
		{
			switch (packed)
			{
			case RolePacked.Defender:
				return this.m_defender;
			case RolePacked.Striker:
				return this.m_striker;
			case RolePacked.Supporter:
				return this.m_supporter;
			case RolePacked.Juggernaut:
				return this.m_juggernaut;
			case RolePacked.Knight:
				return this.m_knight;
			case RolePacked.Marshal:
				return this.m_marshal;
			case RolePacked.Berserker:
				return this.m_berserker;
			case RolePacked.Brigand:
				return this.m_brigand;
			case RolePacked.Warden:
				return this.m_warden;
			case RolePacked.Duelist:
				return this.m_duelist;
			case RolePacked.Sentinel:
				return this.m_sentinel;
			case RolePacked.Warlord:
				return this.m_warlord;
			default:
				return null;
			}
		}

		// Token: 0x06003776 RID: 14198 RVA: 0x00065EF7 File Offset: 0x000640F7
		public bool CanDualWield(UniqueId baseRoleId)
		{
			return this.m_striker && this.m_striker.Id == baseRoleId;
		}

		// Token: 0x0400363D RID: 13885
		private const string kDefender = "Defender";

		// Token: 0x0400363E RID: 13886
		private const string kStriker = "Striker";

		// Token: 0x0400363F RID: 13887
		private const string kSupporter = "Supporter";

		// Token: 0x04003640 RID: 13888
		[SerializeField]
		private BaseRole m_defender;

		// Token: 0x04003641 RID: 13889
		[SerializeField]
		private SpecializedRole m_juggernaut;

		// Token: 0x04003642 RID: 13890
		[SerializeField]
		private SpecializedRole m_knight;

		// Token: 0x04003643 RID: 13891
		[SerializeField]
		private SpecializedRole m_marshal;

		// Token: 0x04003644 RID: 13892
		[SerializeField]
		private BaseRole m_striker;

		// Token: 0x04003645 RID: 13893
		[SerializeField]
		private SpecializedRole m_berserker;

		// Token: 0x04003646 RID: 13894
		[SerializeField]
		private SpecializedRole m_brigand;

		// Token: 0x04003647 RID: 13895
		[SerializeField]
		private SpecializedRole m_warden;

		// Token: 0x04003648 RID: 13896
		[SerializeField]
		private BaseRole m_supporter;

		// Token: 0x04003649 RID: 13897
		[SerializeField]
		private SpecializedRole m_duelist;

		// Token: 0x0400364A RID: 13898
		[SerializeField]
		private SpecializedRole m_sentinel;

		// Token: 0x0400364B RID: 13899
		[SerializeField]
		private SpecializedRole m_warlord;

		// Token: 0x0400364C RID: 13900
		private Dictionary<UniqueId, BaseRoleFlags> m_baseRoleFlagDict;

		// Token: 0x0400364D RID: 13901
		private Dictionary<UniqueId, SpecializedRoleFlags> m_specializationRoleFlagDict;
	}
}
