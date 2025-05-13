using System;
using UnityEngine;

namespace SoL.Game.EffectSystem.Scriptables
{
	// Token: 0x02000C86 RID: 3206
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Ability Parameters/Targeting")]
	public class ScriptableTargetingParams : ScriptableObject
	{
		// Token: 0x1700175D RID: 5981
		// (get) Token: 0x0600618A RID: 24970 RVA: 0x00081C09 File Offset: 0x0007FE09
		internal TargetingParamsSpatial Params
		{
			get
			{
				return this.m_targetingParams;
			}
		}

		// Token: 0x04005501 RID: 21761
		[SerializeField]
		private TargetingParamsSpatial m_targetingParams;
	}
}
