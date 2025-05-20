using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C56 RID: 3158
	[Flags]
	public enum EffectCategoryFlags
	{
		// Token: 0x0400541B RID: 21531
		None = 0,
		// Token: 0x0400541C RID: 21532
		OutgoingDamage = 1,
		// Token: 0x0400541D RID: 21533
		OutgoingHit = 2,
		// Token: 0x0400541E RID: 21534
		OutgoingPenetration = 4,
		// Token: 0x0400541F RID: 21535
		OutgoingFlanking = 8,
		// Token: 0x04005420 RID: 21536
		OutgoingHealing = 16,
		// Token: 0x04005421 RID: 21537
		IncomingDamageResist = 32,
		// Token: 0x04005422 RID: 21538
		IncomingStatusEffectResist = 64,
		// Token: 0x04005423 RID: 21539
		IncomingBehaviorEffectResist = 128,
		// Token: 0x04005424 RID: 21540
		Avoid = 256,
		// Token: 0x04005425 RID: 21541
		Block = 512,
		// Token: 0x04005426 RID: 21542
		Parry = 1024,
		// Token: 0x04005427 RID: 21543
		Riposte = 2048,
		// Token: 0x04005428 RID: 21544
		Resilience = 4096,
		// Token: 0x04005429 RID: 21545
		Movement = 8192,
		// Token: 0x0400542A RID: 21546
		CombatMovement = 16384,
		// Token: 0x0400542B RID: 21547
		Haste = 32768,
		// Token: 0x0400542C RID: 21548
		SafeFall = 65536,
		// Token: 0x0400542D RID: 21549
		HealthRegen = 131072,
		// Token: 0x0400542E RID: 21550
		StaminaRegen = 262144,
		// Token: 0x0400542F RID: 21551
		HealthOverTime = 524288,
		// Token: 0x04005430 RID: 21552
		StaminaOverTime = 1048576,
		// Token: 0x04005431 RID: 21553
		Flatworm = 2097152,
		// Token: 0x04005432 RID: 21554
		OpenWound = 4194304,
		// Token: 0x04005433 RID: 21555
		Serration = 8388608,
		// Token: 0x04005434 RID: 21556
		OutgoingAdvantage = 16777216,
		// Token: 0x04005435 RID: 21557
		Supplies = 33554432,
		// Token: 0x04005436 RID: 21558
		BulletinBoard = 67108864,
		// Token: 0x04005437 RID: 21559
		ViperidToxins = 134217728,
		// Token: 0x04005438 RID: 21560
		GlobalSpeedDebuff = 1073741824,
		// Token: 0x04005439 RID: 21561
		IncomingActiveDefense = -2147483648
	}
}
