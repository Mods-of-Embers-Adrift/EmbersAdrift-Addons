using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000686 RID: 1670
	[CreateAssetMenu(menuName = "SoL/Profiles/Npc Weapon Loadout")]
	public class NpcWeaponProfile : ScriptableObject
	{
		// Token: 0x06003396 RID: 13206 RVA: 0x00063789 File Offset: 0x00061989
		public NpcWeaponLoadout GetLoadout()
		{
			NpcWeaponLoadoutProbabilityCollection weapons = this.m_weapons;
			NpcWeaponLoadoutProbabilityEntry npcWeaponLoadoutProbabilityEntry = (weapons != null) ? weapons.GetEntry(null, false) : null;
			if (npcWeaponLoadoutProbabilityEntry == null)
			{
				return null;
			}
			return npcWeaponLoadoutProbabilityEntry.Obj;
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x000637AA File Offset: 0x000619AA
		private void OnValidate()
		{
			this.m_weapons.Normalize();
		}

		// Token: 0x0400319C RID: 12700
		[SerializeField]
		private NpcWeaponLoadoutProbabilityCollection m_weapons;
	}
}
