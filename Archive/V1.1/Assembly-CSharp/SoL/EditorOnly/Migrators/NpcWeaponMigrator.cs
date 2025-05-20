using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.EditorOnly.Migrators
{
	// Token: 0x02000DDA RID: 3546
	[CreateAssetMenu(menuName = "SoL/Migrators/Weapon Migrator")]
	public class NpcWeaponMigrator : ScriptableObject
	{
		// Token: 0x1700192D RID: 6445
		// (get) Token: 0x0600698D RID: 27021 RVA: 0x00063AE9 File Offset: 0x00061CE9
		private IEnumerable GetProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<WeaponItem>();
			}
		}

		// Token: 0x0600698E RID: 27022 RVA: 0x00217DBC File Offset: 0x00215FBC
		private NpcWeaponMigrator.WeaponFilterData GetWeaponFilter(WeaponItem weapon)
		{
			foreach (NpcWeaponMigrator.WeaponFilterData weaponFilterData in this.m_weaponFilters)
			{
				if (weapon.name.Contains(weaponFilterData.Filter))
				{
					return weaponFilterData;
				}
			}
			return null;
		}

		// Token: 0x04005BF5 RID: 23541
		[SerializeField]
		private NpcWeaponMigrator.WeaponFilterData[] m_weaponFilters;

		// Token: 0x04005BF6 RID: 23542
		[SerializeField]
		private WeaponItem[] m_weapons;

		// Token: 0x04005BF7 RID: 23543
		[SerializeField]
		private NpcWeaponMigrator.LevelSetting m_minimumLevel;

		// Token: 0x04005BF8 RID: 23544
		[SerializeField]
		private NpcWeaponMigrator.LevelSetting m_maximumLevel;

		// Token: 0x02000DDB RID: 3547
		[Serializable]
		private class WeaponFilterData
		{
			// Token: 0x04005BF9 RID: 23545
			public string Filter = string.Empty;

			// Token: 0x04005BFA RID: 23546
			public float Variance = 0.2f;

			// Token: 0x04005BFB RID: 23547
			public int Delay = 10;
		}

		// Token: 0x02000DDC RID: 3548
		[Serializable]
		private class LevelSetting
		{
			// Token: 0x1700192E RID: 6446
			// (get) Token: 0x06006991 RID: 27025 RVA: 0x00086CFC File Offset: 0x00084EFC
			// (set) Token: 0x06006992 RID: 27026 RVA: 0x00086D04 File Offset: 0x00084F04
			public float MinDamage { get; private set; }

			// Token: 0x1700192F RID: 6447
			// (get) Token: 0x06006993 RID: 27027 RVA: 0x00086D0D File Offset: 0x00084F0D
			// (set) Token: 0x06006994 RID: 27028 RVA: 0x00086D15 File Offset: 0x00084F15
			public float MaxDamage { get; private set; }

			// Token: 0x17001930 RID: 6448
			// (get) Token: 0x06006995 RID: 27029 RVA: 0x00086D1E File Offset: 0x00084F1E
			// (set) Token: 0x06006996 RID: 27030 RVA: 0x00086D26 File Offset: 0x00084F26
			public int SidesMod { get; private set; }

			// Token: 0x17001931 RID: 6449
			// (get) Token: 0x06006997 RID: 27031 RVA: 0x00086D2F File Offset: 0x00084F2F
			// (set) Token: 0x06006998 RID: 27032 RVA: 0x00086D37 File Offset: 0x00084F37
			public int Modifer { get; private set; }

			// Token: 0x06006999 RID: 27033 RVA: 0x00217DF8 File Offset: 0x00215FF8
			public void CalculateValues(NpcWeaponMigrator.WeaponFilterData weaponFilter)
			{
				this.MinDamage = (this.AverageDps - this.AverageDps * weaponFilter.Variance) * (float)weaponFilter.Delay;
				this.MaxDamage = (this.AverageDps + this.AverageDps * weaponFilter.Variance) * (float)weaponFilter.Delay;
				this.SidesMod = Mathf.CeilToInt(this.MinDamage - this.MaxDamage);
				this.Modifer = Mathf.FloorToInt(this.MinDamage);
			}

			// Token: 0x04005BFC RID: 23548
			public float AverageDps = 1f;

			// Token: 0x04005BFD RID: 23549
			public int NDice = 1;
		}
	}
}
