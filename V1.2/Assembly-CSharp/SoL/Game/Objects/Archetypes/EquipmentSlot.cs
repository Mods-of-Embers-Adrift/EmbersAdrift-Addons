using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A76 RID: 2678
	[Flags]
	public enum EquipmentSlot
	{
		// Token: 0x040049FD RID: 18941
		None = 0,
		// Token: 0x040049FE RID: 18942
		PrimaryWeapon_MainHand = 1,
		// Token: 0x040049FF RID: 18943
		PrimaryWeapon_OffHand = 2,
		// Token: 0x04004A00 RID: 18944
		SecondaryWeapon_MainHand = 4,
		// Token: 0x04004A01 RID: 18945
		SecondaryWeapon_OffHand = 8,
		// Token: 0x04004A02 RID: 18946
		Tool1 = 16,
		// Token: 0x04004A03 RID: 18947
		Tool2 = 32,
		// Token: 0x04004A04 RID: 18948
		Tool3 = 64,
		// Token: 0x04004A05 RID: 18949
		Tool4 = 128,
		// Token: 0x04004A06 RID: 18950
		LightSource = 256,
		// Token: 0x04004A07 RID: 18951
		EmberStone = 512,
		// Token: 0x04004A08 RID: 18952
		Neck = 1024,
		// Token: 0x04004A09 RID: 18953
		Ear_Left = 2048,
		// Token: 0x04004A0A RID: 18954
		Ear_Right = 4096,
		// Token: 0x04004A0B RID: 18955
		Finger_Left = 8192,
		// Token: 0x04004A0C RID: 18956
		Finger_Right = 16384,
		// Token: 0x04004A0D RID: 18957
		Head = 32768,
		// Token: 0x04004A0E RID: 18958
		Cosmetic = 65536,
		// Token: 0x04004A0F RID: 18959
		[InspectorName("Utility")]
		Back = 131072,
		// Token: 0x04004A10 RID: 18960
		Waist = 262144,
		// Token: 0x04004A11 RID: 18961
		Clothing_Chest = 524288,
		// Token: 0x04004A12 RID: 18962
		Clothing_Hands = 1048576,
		// Token: 0x04004A13 RID: 18963
		Clothing_Legs = 2097152,
		// Token: 0x04004A14 RID: 18964
		Clothing_Feet = 4194304,
		// Token: 0x04004A15 RID: 18965
		Armor_Shoulder_L = 8388608,
		// Token: 0x04004A16 RID: 18966
		Armor_Shoulder_R = 16777216,
		// Token: 0x04004A17 RID: 18967
		Armor_Chest = 33554432,
		// Token: 0x04004A18 RID: 18968
		Armor_Hand_L = 67108864,
		// Token: 0x04004A19 RID: 18969
		Armor_Hand_R = 134217728,
		// Token: 0x04004A1A RID: 18970
		Armor_Legs = 268435456,
		// Token: 0x04004A1B RID: 18971
		Armor_Feet_L = 536870912,
		// Token: 0x04004A1C RID: 18972
		Armor_Feet_R = 1073741824
	}
}
