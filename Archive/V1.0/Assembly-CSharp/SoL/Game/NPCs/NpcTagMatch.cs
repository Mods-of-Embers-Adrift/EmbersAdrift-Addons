using System;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x0200081C RID: 2076
	[Serializable]
	public class NpcTagMatch
	{
		// Token: 0x06003C1A RID: 15386 RVA: 0x0017E1E8 File Offset: 0x0017C3E8
		public bool Matches(NpcTags query)
		{
			if (query == NpcTags.None)
			{
				return false;
			}
			if (this.m_required == NpcTags.None && this.m_optional == NpcTags.None)
			{
				return false;
			}
			bool flag = this.m_required == NpcTags.None || query.HasBitFlag(this.m_required);
			bool flag2 = this.m_optional == NpcTags.None || query.HasAnyFlags(this.m_optional);
			return flag && flag2;
		}

		// Token: 0x06003C1B RID: 15387 RVA: 0x0017E240 File Offset: 0x0017C440
		public bool Matches(NpcTagSet query)
		{
			long num = (query != null) ? query.GetCombinedTags() : 0L;
			if (num == 0L)
			{
				return false;
			}
			long num2 = (this.m_requiredSet != null) ? this.m_requiredSet.GetCombinedTags() : 0L;
			long num3 = (this.m_optionalSet != null) ? this.m_optionalSet.GetCombinedTags() : 0L;
			if (num2 == 0L && num3 == 0L)
			{
				return false;
			}
			bool flag = num2 == 0L || NpcTagExtensions.HasBitFlag(num, num2);
			bool flag2 = num3 == 0L || NpcTagExtensions.HasAnyFlags(num, num3);
			return flag && flag2;
		}

		// Token: 0x04003AFE RID: 15102
		private const int kLabelWidth = 64;

		// Token: 0x04003AFF RID: 15103
		private const string kGroupName = "NpcTagMatch";

		// Token: 0x04003B00 RID: 15104
		[HideInInspector]
		[SerializeField]
		private NpcTags m_required;

		// Token: 0x04003B01 RID: 15105
		[SerializeField]
		private NpcTagSet m_requiredSet;

		// Token: 0x04003B02 RID: 15106
		[HideInInspector]
		[SerializeField]
		private NpcTags m_optional;

		// Token: 0x04003B03 RID: 15107
		[SerializeField]
		private NpcTagSet m_optionalSet;
	}
}
