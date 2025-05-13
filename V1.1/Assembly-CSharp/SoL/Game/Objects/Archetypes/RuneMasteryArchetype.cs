using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ADA RID: 2778
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Masteries/CombatMastery")]
	public class RuneMasteryArchetype : MasteryArchetype
	{
		// Token: 0x170013D2 RID: 5074
		// (get) Token: 0x060055A8 RID: 21928 RVA: 0x000792C0 File Offset: 0x000774C0
		public RuneSourceType RuneSource
		{
			get
			{
				return this.m_runeSource;
			}
		}

		// Token: 0x04004C06 RID: 19462
		[SerializeField]
		private RuneSourceType m_runeSource;
	}
}
