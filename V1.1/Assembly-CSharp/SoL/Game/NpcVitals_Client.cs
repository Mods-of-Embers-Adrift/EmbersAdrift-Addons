using System;

namespace SoL.Game
{
	// Token: 0x020005ED RID: 1517
	public class NpcVitals_Client : Vitals
	{
		// Token: 0x06002FED RID: 12269 RVA: 0x0006105D File Offset: 0x0005F25D
		public override float GetHealthPercent()
		{
			return this.Health * 0.01f;
		}

		// Token: 0x06002FEE RID: 12270 RVA: 0x0006106B File Offset: 0x0005F26B
		public override float GetArmorClassPercent()
		{
			return (float)this.ArmorClass * 0.01f;
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06002FEF RID: 12271 RVA: 0x0006107A File Offset: 0x0005F27A
		public override float Health
		{
			get
			{
				return (float)this.NpcReplicator.HealthPercent.Value;
			}
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06002FF0 RID: 12272 RVA: 0x0006108D File Offset: 0x0005F28D
		public override float HealthWound
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06002FF1 RID: 12273 RVA: 0x00061094 File Offset: 0x0005F294
		public override int MaxHealth { get; } = 100;

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06002FF2 RID: 12274 RVA: 0x0006109C File Offset: 0x0005F29C
		public override float Stamina
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06002FF3 RID: 12275 RVA: 0x0006108D File Offset: 0x0005F28D
		public override float StaminaWound
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06002FF4 RID: 12276 RVA: 0x000610A3 File Offset: 0x0005F2A3
		public override int ArmorClass
		{
			get
			{
				return (int)this.NpcReplicator.ArmorClassPercent.Value;
			}
		}

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06002FF5 RID: 12277 RVA: 0x000610B5 File Offset: 0x0005F2B5
		public override int MaxArmorClass { get; }

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06002FF6 RID: 12278 RVA: 0x000610BD File Offset: 0x0005F2BD
		private VitalsReplicatorNpc NpcReplicator
		{
			get
			{
				if (this.m_npcReplicator == null)
				{
					this.m_npcReplicator = (base.m_replicator as VitalsReplicatorNpc);
				}
				return this.m_npcReplicator;
			}
		}

		// Token: 0x04002EDA RID: 11994
		private VitalsReplicatorNpc m_npcReplicator;
	}
}
