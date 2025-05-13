using System;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;

namespace SoL.Game
{
	// Token: 0x02000567 RID: 1383
	public class NpcCharacterData : CharacterData
	{
		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x06002A44 RID: 10820 RVA: 0x0005D1CD File Offset: 0x0005B3CD
		// (set) Token: 0x06002A45 RID: 10821 RVA: 0x0005D1DA File Offset: 0x0005B3DA
		public override ChallengeRating ChallengeRating
		{
			get
			{
				return this.m_challengeRating.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_challengeRating.Value = value;
				}
			}
		}

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x06002A46 RID: 10822 RVA: 0x0005D1EF File Offset: 0x0005B3EF
		public override int GroupedLevel
		{
			get
			{
				return base.AdventuringLevel;
			}
		}

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x06002A47 RID: 10823 RVA: 0x0005D1F7 File Offset: 0x0005B3F7
		// (set) Token: 0x06002A48 RID: 10824 RVA: 0x0005D204 File Offset: 0x0005B404
		public override int ResourceLevel
		{
			get
			{
				return (int)this.m_resourceLevel.Value;
			}
			set
			{
				this.m_resourceLevel.Value = (byte)value;
			}
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x06002A49 RID: 10825 RVA: 0x0005D213 File Offset: 0x0005B413
		// (set) Token: 0x06002A4A RID: 10826 RVA: 0x0005D220 File Offset: 0x0005B420
		public override Faction Faction
		{
			get
			{
				return this.m_faction;
			}
			set
			{
				this.m_faction.Value = value;
			}
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06002A4B RID: 10827 RVA: 0x0005D22E File Offset: 0x0005B42E
		// (set) Token: 0x06002A4C RID: 10828 RVA: 0x0005D23B File Offset: 0x0005B43B
		public override NpcInitData NpcInitData
		{
			get
			{
				return this.m_npcInitData.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_npcInitData.Value = value;
				}
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x06002A4D RID: 10829 RVA: 0x0005D250 File Offset: 0x0005B450
		// (set) Token: 0x06002A4E RID: 10830 RVA: 0x0005D25D File Offset: 0x0005B45D
		public override float? TransformScale
		{
			get
			{
				return this.m_transformScale.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_transformScale.Value = value;
				}
			}
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x06002A4F RID: 10831 RVA: 0x0005D272 File Offset: 0x0005B472
		// (set) Token: 0x06002A50 RID: 10832 RVA: 0x0005D289 File Offset: 0x0005B489
		public override CharacterSex Sex
		{
			get
			{
				if (this.m_sex == CharacterSex.None)
				{
					return base.Sex;
				}
				return this.m_sex;
			}
			set
			{
				this.m_sex = value;
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x06002A51 RID: 10833 RVA: 0x0005D292 File Offset: 0x0005B492
		// (set) Token: 0x06002A52 RID: 10834 RVA: 0x0005D29F File Offset: 0x0005B49F
		public override bool IsSwimming
		{
			get
			{
				return this.m_isSwimming.Value;
			}
			set
			{
				if (GameManager.IsServer)
				{
					this.m_isSwimming.Value = value;
				}
			}
		}

		// Token: 0x06002A53 RID: 10835 RVA: 0x0005D2B4 File Offset: 0x0005B4B4
		public override void InitNpcLevel(int level)
		{
			base.InitNpcLevel(level);
			base.AdventuringLevel = level;
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x06002A54 RID: 10836 RVA: 0x0005D2C4 File Offset: 0x0005B4C4
		// (set) Token: 0x06002A55 RID: 10837 RVA: 0x0004475B File Offset: 0x0004295B
		public override EmberStoneFillLevels EmberStoneFillLevel
		{
			get
			{
				return EmberStoneFillLevels.Full;
			}
			set
			{
			}
		}

		// Token: 0x06002A56 RID: 10838 RVA: 0x00142C7C File Offset: 0x00140E7C
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_challengeRating);
			this.m_challengeRating.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_faction);
			this.m_faction.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_isSwimming);
			this.m_isSwimming.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_npcInitData);
			this.m_npcInitData.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_resourceLevel);
			this.m_resourceLevel.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_transformScale);
			this.m_transformScale.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002AEE RID: 10990
		private readonly SynchronizedNullableFloat m_transformScale = new SynchronizedNullableFloat(null);

		// Token: 0x04002AEF RID: 10991
		private readonly SynchronizedStruct<NpcInitData> m_npcInitData = new SynchronizedStruct<NpcInitData>();

		// Token: 0x04002AF0 RID: 10992
		private readonly SynchronizedByte m_resourceLevel = new SynchronizedByte();

		// Token: 0x04002AF1 RID: 10993
		private readonly SynchronizedEnum<Faction> m_faction = new SynchronizedEnum<Faction>(Faction.Ignore);

		// Token: 0x04002AF2 RID: 10994
		private readonly SynchronizedEnum<ChallengeRating> m_challengeRating = new SynchronizedEnum<ChallengeRating>(ChallengeRating.CR1);

		// Token: 0x04002AF3 RID: 10995
		private readonly SynchronizedBool m_isSwimming = new SynchronizedBool(false);

		// Token: 0x04002AF4 RID: 10996
		private CharacterSex m_sex;
	}
}
