using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C35 RID: 3125
	[Flags]
	public enum EffectApplicationFlags
	{
		// Token: 0x04005304 RID: 21252
		None = 0,
		// Token: 0x04005305 RID: 21253
		Miss = 1,
		// Token: 0x04005306 RID: 21254
		Glancing = 2,
		// Token: 0x04005307 RID: 21255
		Normal = 4,
		// Token: 0x04005308 RID: 21256
		Heavy = 8,
		// Token: 0x04005309 RID: 21257
		Critical = 16,
		// Token: 0x0400530A RID: 21258
		Avoid = 32,
		// Token: 0x0400530B RID: 21259
		Block = 64,
		// Token: 0x0400530C RID: 21260
		Parry = 128,
		// Token: 0x0400530D RID: 21261
		Riposte = 256,
		// Token: 0x0400530E RID: 21262
		Resist = 512,
		// Token: 0x0400530F RID: 21263
		PartialResist = 1024,
		// Token: 0x04005310 RID: 21264
		Absorb = 2048,
		// Token: 0x04005311 RID: 21265
		Applied = 4096,
		// Token: 0x04005312 RID: 21266
		OverTime = 8192,
		// Token: 0x04005313 RID: 21267
		Killed = 16384,
		// Token: 0x04005314 RID: 21268
		Tagger = 32768,
		// Token: 0x04005315 RID: 21269
		MeleePhysical = 65536,
		// Token: 0x04005316 RID: 21270
		RangedPhysical = 131072,
		// Token: 0x04005317 RID: 21271
		Advantage = 262144,
		// Token: 0x04005318 RID: 21272
		Disadvantage = 524288,
		// Token: 0x04005319 RID: 21273
		MentalDamage = 1048576,
		// Token: 0x0400531A RID: 21274
		ChemicalDamage = 2097152,
		// Token: 0x0400531B RID: 21275
		EmberDamage = 4194304,
		// Token: 0x0400531C RID: 21276
		InitialApplication = 8388608,
		// Token: 0x0400531D RID: 21277
		CannotReach = 33554432,
		// Token: 0x0400531E RID: 21278
		RiposteAbility = 67108864,
		// Token: 0x0400531F RID: 21279
		Ability = 134217728,
		// Token: 0x04005320 RID: 21280
		AutoAttack = 268435456,
		// Token: 0x04005321 RID: 21281
		Diminished = 536870912,
		// Token: 0x04005322 RID: 21282
		NegativePolaritySelfAoe = 1073741824,
		// Token: 0x04005323 RID: 21283
		Positive = -2147483648,
		// Token: 0x04005324 RID: 21284
		ValidHit = 30,
		// Token: 0x04005325 RID: 21285
		ValidHitAndOverTime = 8222
	}
}
