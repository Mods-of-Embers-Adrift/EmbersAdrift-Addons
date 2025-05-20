using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C48 RID: 3144
	public enum StatType
	{
		// Token: 0x04005389 RID: 21385
		None,
		// Token: 0x0400538A RID: 21386
		Health,
		// Token: 0x0400538B RID: 21387
		Stamina,
		// Token: 0x0400538C RID: 21388
		ArmorWeight,
		// Token: 0x0400538D RID: 21389
		RegenHealth = 10,
		// Token: 0x0400538E RID: 21390
		RegenStamina,
		// Token: 0x0400538F RID: 21391
		Healing,
		// Token: 0x04005390 RID: 21392
		Movement = 20,
		// Token: 0x04005391 RID: 21393
		CombatMovement,
		// Token: 0x04005392 RID: 21394
		Haste,
		// Token: 0x04005393 RID: 21395
		SafeFall,
		// Token: 0x04005394 RID: 21396
		Resilience,
		// Token: 0x04005395 RID: 21397
		Avoid = 30,
		// Token: 0x04005396 RID: 21398
		Block,
		// Token: 0x04005397 RID: 21399
		Parry,
		// Token: 0x04005398 RID: 21400
		Riposte,
		// Token: 0x04005399 RID: 21401
		Hit = 40,
		// Token: 0x0400539A RID: 21402
		Penetration,
		// Token: 0x0400539B RID: 21403
		[InspectorName("Positional")]
		Flanking,
		// Token: 0x0400539C RID: 21404
		Damage1H = 50,
		// Token: 0x0400539D RID: 21405
		Damage2H,
		// Token: 0x0400539E RID: 21406
		DamageRanged,
		// Token: 0x0400539F RID: 21407
		DamageMental,
		// Token: 0x040053A0 RID: 21408
		DamageChemical,
		// Token: 0x040053A1 RID: 21409
		DamageEmber,
		// Token: 0x040053A2 RID: 21410
		DamagePhysical,
		// Token: 0x040053A3 RID: 21411
		DamageRaw,
		// Token: 0x040053A4 RID: 21412
		ResistDamagePhysical = 60,
		// Token: 0x040053A5 RID: 21413
		ResistDamageMental,
		// Token: 0x040053A6 RID: 21414
		ResistDamageChemical,
		// Token: 0x040053A7 RID: 21415
		ResistDamageEmber,
		// Token: 0x040053A8 RID: 21416
		ResistDebuffPhysical = 70,
		// Token: 0x040053A9 RID: 21417
		ResistDebuffMental,
		// Token: 0x040053AA RID: 21418
		ResistDebuffChemical,
		// Token: 0x040053AB RID: 21419
		ResistDebuffEmber,
		// Token: 0x040053AC RID: 21420
		ResistDebuffMovement,
		// Token: 0x040053AD RID: 21421
		ResistStun = 90,
		// Token: 0x040053AE RID: 21422
		ResistFear,
		// Token: 0x040053AF RID: 21423
		ResistDaze = 93,
		// Token: 0x040053B0 RID: 21424
		ResistEnrage,
		// Token: 0x040053B1 RID: 21425
		ResistConfuse,
		// Token: 0x040053B2 RID: 21426
		ResistLull,
		// Token: 0x040053B3 RID: 21427
		[InspectorName("[UNUSED] Damage Resist")]
		DamageResist = 100,
		// Token: 0x040053B4 RID: 21428
		[InspectorName("[UNUSED] Damage Resist Ember")]
		DamageResistEmber,
		// Token: 0x040053B5 RID: 21429
		[InspectorName("[UNUSED] Damage Reduction")]
		DamageReduction,
		// Token: 0x040053B6 RID: 21430
		[InspectorName("[UNUSED] Damage Reduction Ember")]
		DamageReductionEmber,
		// Token: 0x040053B7 RID: 21431
		[InspectorName("[UNUSED] Debuff Resist")]
		DebuffResist,
		// Token: 0x040053B8 RID: 21432
		[InspectorName("[UNUSED] Debuff Resist Ember")]
		DebuffResistEmber
	}
}
