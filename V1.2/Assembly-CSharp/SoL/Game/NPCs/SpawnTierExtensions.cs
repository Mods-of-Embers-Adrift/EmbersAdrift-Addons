using System;

namespace SoL.Game.NPCs
{
	// Token: 0x0200082A RID: 2090
	public static class SpawnTierExtensions
	{
		// Token: 0x17000DF7 RID: 3575
		// (get) Token: 0x06003CB0 RID: 15536 RVA: 0x000691D0 File Offset: 0x000673D0
		public static SpawnTierFlags[] AllSpawnTierFlags
		{
			get
			{
				if (SpawnTierExtensions.m_spawnTierFlags == null)
				{
					SpawnTierExtensions.m_spawnTierFlags = (SpawnTierFlags[])Enum.GetValues(typeof(SpawnTierFlags));
				}
				return SpawnTierExtensions.m_spawnTierFlags;
			}
		}

		// Token: 0x06003CB1 RID: 15537 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this SpawnTierFlags a, SpawnTierFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06003CB2 RID: 15538 RVA: 0x00180EB4 File Offset: 0x0017F0B4
		public static SpawnTier GetSpawnTier(this SpawnTierFlags flags)
		{
			if (flags <= SpawnTierFlags.Champion)
			{
				switch (flags)
				{
				case SpawnTierFlags.Weak:
					return SpawnTier.Weak;
				case SpawnTierFlags.Normal:
					return SpawnTier.Normal;
				case SpawnTierFlags.Weak | SpawnTierFlags.Normal:
					break;
				case SpawnTierFlags.Strong:
					return SpawnTier.Strong;
				default:
					if (flags == SpawnTierFlags.Champion)
					{
						return SpawnTier.Champion;
					}
					break;
				}
			}
			else
			{
				if (flags == SpawnTierFlags.Elite)
				{
					return SpawnTier.Elite;
				}
				if (flags == SpawnTierFlags.Boss)
				{
					return SpawnTier.Boss;
				}
				if (flags == SpawnTierFlags.Epic)
				{
					return SpawnTier.Epic;
				}
			}
			return SpawnTier.Normal;
		}

		// Token: 0x06003CB3 RID: 15539 RVA: 0x00180F10 File Offset: 0x0017F110
		public static SpawnTierFlags GetSpawnTierFlags(this SpawnTier tier)
		{
			if (tier <= SpawnTier.Strong)
			{
				if (tier == SpawnTier.Weak)
				{
					return SpawnTierFlags.Weak;
				}
				if (tier == SpawnTier.Normal)
				{
					return SpawnTierFlags.Normal;
				}
				if (tier == SpawnTier.Strong)
				{
					return SpawnTierFlags.Strong;
				}
			}
			else if (tier <= SpawnTier.Elite)
			{
				if (tier == SpawnTier.Champion)
				{
					return SpawnTierFlags.Champion;
				}
				if (tier == SpawnTier.Elite)
				{
					return SpawnTierFlags.Elite;
				}
			}
			else
			{
				if (tier == SpawnTier.Boss)
				{
					return SpawnTierFlags.Boss;
				}
				if (tier == SpawnTier.Epic)
				{
					return SpawnTierFlags.Epic;
				}
			}
			return SpawnTierFlags.None;
		}

		// Token: 0x04003B7E RID: 15230
		private static SpawnTierFlags[] m_spawnTierFlags;
	}
}
