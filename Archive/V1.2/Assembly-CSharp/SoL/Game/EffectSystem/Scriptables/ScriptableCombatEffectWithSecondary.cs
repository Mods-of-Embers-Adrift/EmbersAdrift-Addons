using System;
using UnityEngine;

namespace SoL.Game.EffectSystem.Scriptables
{
	// Token: 0x02000C84 RID: 3204
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Ability Parameters/Effect")]
	public class ScriptableCombatEffectWithSecondary : ScriptableObject
	{
		// Token: 0x1700175B RID: 5979
		// (get) Token: 0x06006186 RID: 24966 RVA: 0x00081BF9 File Offset: 0x0007FDF9
		internal CombatEffectWithSecondary Params
		{
			get
			{
				return this.m_combatEffect;
			}
		}

		// Token: 0x040054FF RID: 21759
		[SerializeField]
		private CombatEffectWithSecondary m_combatEffect;
	}
}
