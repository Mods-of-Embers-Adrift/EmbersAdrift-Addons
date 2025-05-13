using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C06 RID: 3078
	[CreateAssetMenu(menuName = "SoL/Ability VFX")]
	public class AbilityVFXScriptable : ScriptableObject
	{
		// Token: 0x1700167E RID: 5758
		// (get) Token: 0x06005ECF RID: 24271 RVA: 0x0007FC96 File Offset: 0x0007DE96
		public AbilityVFX VFX
		{
			get
			{
				return this.m_vfx;
			}
		}

		// Token: 0x040051F9 RID: 20985
		[SerializeField]
		private AbilityVFX m_vfx;
	}
}
