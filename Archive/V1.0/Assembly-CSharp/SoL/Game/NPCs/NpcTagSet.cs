using System;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x0200081B RID: 2075
	[Serializable]
	public class NpcTagSet
	{
		// Token: 0x06003C13 RID: 15379 RVA: 0x00044765 File Offset: 0x00042965
		public NpcTagSet()
		{
		}

		// Token: 0x06003C14 RID: 15380 RVA: 0x00068B39 File Offset: 0x00066D39
		public NpcTagSet(NpcTags tagsA, NpcTagsB tagsB)
		{
			this.m_tagsA = tagsA;
			this.m_tagsB = tagsB;
		}

		// Token: 0x06003C15 RID: 15381 RVA: 0x00068B4F File Offset: 0x00066D4F
		public static NpcTagSet Clone(NpcTagSet other)
		{
			if (other == null)
			{
				return new NpcTagSet();
			}
			return new NpcTagSet(other.m_tagsA, other.m_tagsB);
		}

		// Token: 0x06003C16 RID: 15382 RVA: 0x0017E14C File Offset: 0x0017C34C
		public long GetCombinedTags()
		{
			if (this.m_combined == null)
			{
				long num = (long)this.m_tagsA;
				long num2 = (long)this.m_tagsB;
				this.m_combined = new long?(num | num2 << 32);
			}
			return this.m_combined.Value;
		}

		// Token: 0x06003C17 RID: 15383 RVA: 0x0017E194 File Offset: 0x0017C394
		public void AddAggressive()
		{
			if (this.m_defaultTagsA == null)
			{
				this.m_defaultTagsA = new NpcTags?(this.m_tagsA);
			}
			NpcTags tagsA = this.m_tagsA;
			this.m_tagsA |= NpcTags.Aggressive;
			if (tagsA != this.m_tagsA)
			{
				this.m_combined = null;
			}
		}

		// Token: 0x06003C18 RID: 15384 RVA: 0x00068B6B File Offset: 0x00066D6B
		public void RevertAggressive()
		{
			if (this.m_defaultTagsA != null)
			{
				NpcTags tagsA = this.m_tagsA;
				this.m_tagsA = this.m_defaultTagsA.Value;
				if (tagsA != this.m_tagsA)
				{
					this.m_combined = null;
				}
			}
		}

		// Token: 0x06003C19 RID: 15385 RVA: 0x00068BA5 File Offset: 0x00066DA5
		public bool IsGuard()
		{
			return this.m_tagsA.HasBitFlag(NpcTags.Guard);
		}

		// Token: 0x04003AFA RID: 15098
		[SerializeField]
		private NpcTags m_tagsA;

		// Token: 0x04003AFB RID: 15099
		[SerializeField]
		private NpcTagsB m_tagsB;

		// Token: 0x04003AFC RID: 15100
		private NpcTags? m_defaultTagsA;

		// Token: 0x04003AFD RID: 15101
		private long? m_combined;
	}
}
