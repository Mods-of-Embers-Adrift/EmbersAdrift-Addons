using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C45 RID: 3141
	[CreateAssetMenu(menuName = "SoL/NEW_EFFECTS/Effects")]
	public class ScriptableEffects : ScriptableObject
	{
		// Token: 0x17001748 RID: 5960
		// (get) Token: 0x060060E7 RID: 24807 RVA: 0x00081401 File Offset: 0x0007F601
		public Effects Effects
		{
			get
			{
				return this.m_effects;
			}
		}

		// Token: 0x04005385 RID: 21381
		[SerializeField]
		protected Effects m_effects;
	}
}
