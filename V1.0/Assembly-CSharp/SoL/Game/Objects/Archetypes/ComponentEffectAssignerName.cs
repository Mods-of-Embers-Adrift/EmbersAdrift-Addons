using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A53 RID: 2643
	public enum ComponentEffectAssignerName
	{
		// Token: 0x04004917 RID: 18711
		None,
		// Token: 0x04004918 RID: 18712
		Weight,
		// Token: 0x04004919 RID: 18713
		BaseWorth,
		// Token: 0x0400491A RID: 18714
		BaseArmorClass,
		// Token: 0x0400491B RID: 18715
		MaxDamageAbsorption,
		// Token: 0x0400491C RID: 18716
		MinimumToolEffectiveness,
		// Token: 0x0400491D RID: 18717
		MaxStackCount,
		// Token: 0x0400491E RID: 18718
		ExecutionTime,
		// Token: 0x0400491F RID: 18719
		MovementModifier,
		// Token: 0x04004920 RID: 18720
		Cooldown,
		// Token: 0x04004921 RID: 18721
		FireDuration,
		// Token: 0x04004922 RID: 18722
		ProfileFraction,
		// Token: 0x04004923 RID: 18723
		Angle = 14,
		// Token: 0x04004924 RID: 18724
		Delay,
		// Token: 0x04004925 RID: 18725
		DiceCount,
		// Token: 0x04004926 RID: 18726
		DiceSides,
		// Token: 0x04004927 RID: 18727
		MinimumDistance,
		// Token: 0x04004928 RID: 18728
		MaximumDistance,
		// Token: 0x04004929 RID: 18729
		HitModifier,
		// Token: 0x0400492A RID: 18730
		ArmorModifier,
		// Token: 0x0400492B RID: 18731
		ProficiencyRequirement,
		// Token: 0x0400492C RID: 18732
		ArmorCostMultiplier,
		// Token: 0x0400492D RID: 18733
		Resilience,
		// Token: 0x0400492E RID: 18734
		SafeFall,
		// Token: 0x0400492F RID: 18735
		Haste,
		// Token: 0x04004930 RID: 18736
		Movement,
		// Token: 0x04004931 RID: 18737
		CombatMovement,
		// Token: 0x04004932 RID: 18738
		RegenHealth = 40,
		// Token: 0x04004933 RID: 18739
		RegenStamina,
		// Token: 0x04004934 RID: 18740
		MaxDuration,
		// Token: 0x04004935 RID: 18741
		DiceModifier,
		// Token: 0x04004936 RID: 18742
		ArmorWeightOverride,
		// Token: 0x04004937 RID: 18743
		ArmorWeightInterpolator,
		// Token: 0x04004938 RID: 18744
		Hit,
		// Token: 0x04004939 RID: 18745
		Penetration,
		// Token: 0x0400493A RID: 18746
		Flanking,
		// Token: 0x0400493B RID: 18747
		Damage1H,
		// Token: 0x0400493C RID: 18748
		Damage2H,
		// Token: 0x0400493D RID: 18749
		DamageRanged,
		// Token: 0x0400493E RID: 18750
		DamageMental,
		// Token: 0x0400493F RID: 18751
		DamageChemical,
		// Token: 0x04004940 RID: 18752
		DamageEmber,
		// Token: 0x04004941 RID: 18753
		Healing,
		// Token: 0x04004942 RID: 18754
		StunStatusResist = 100,
		// Token: 0x04004943 RID: 18755
		FearStatusResist,
		// Token: 0x04004944 RID: 18756
		DazeStatusResist,
		// Token: 0x04004945 RID: 18757
		EnrageStatusResist,
		// Token: 0x04004946 RID: 18758
		ConfuseStatusResist,
		// Token: 0x04004947 RID: 18759
		LullStatusResist,
		// Token: 0x04004948 RID: 18760
		ResistDamagePhysical,
		// Token: 0x04004949 RID: 18761
		ResistDamageMental,
		// Token: 0x0400494A RID: 18762
		ResistDamageChemical,
		// Token: 0x0400494B RID: 18763
		ResistDamageEmber,
		// Token: 0x0400494C RID: 18764
		ResistDebuffPhysical,
		// Token: 0x0400494D RID: 18765
		ResistDebuffMental,
		// Token: 0x0400494E RID: 18766
		ResistDebuffChemical,
		// Token: 0x0400494F RID: 18767
		ResistDebuffEmber,
		// Token: 0x04004950 RID: 18768
		ResistDebuffMovement,
		// Token: 0x04004951 RID: 18769
		Avoidance,
		// Token: 0x04004952 RID: 18770
		Block,
		// Token: 0x04004953 RID: 18771
		Parry,
		// Token: 0x04004954 RID: 18772
		Riposte,
		// Token: 0x04004955 RID: 18773
		DamageResist,
		// Token: 0x04004956 RID: 18774
		DamageResistEmber,
		// Token: 0x04004957 RID: 18775
		DamageReduction,
		// Token: 0x04004958 RID: 18776
		DamageReductionEmber,
		// Token: 0x04004959 RID: 18777
		DebuffResist,
		// Token: 0x0400495A RID: 18778
		DebuffResistEmber,
		// Token: 0x0400495B RID: 18779
		AllDamage = 1000,
		// Token: 0x0400495C RID: 18780
		AllPhysicalDamage,
		// Token: 0x0400495D RID: 18781
		AllNonPhysicalDamage,
		// Token: 0x0400495E RID: 18782
		AllMeleeDamage,
		// Token: 0x0400495F RID: 18783
		AllActiveDefense,
		// Token: 0x04004960 RID: 18784
		AllDamageResist,
		// Token: 0x04004961 RID: 18785
		AllDebuffResist,
		// Token: 0x04004962 RID: 18786
		AllStatusResist,
		// Token: 0x04004963 RID: 18787
		ExecutionTimeMultiplier
	}
}
