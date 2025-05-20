using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D4C RID: 3404
	[CreateAssetMenu(menuName = "SoL/Animation/Ability Animation")]
	public class AbilityAnimationScriptable : ScriptableObject
	{
		// Token: 0x1700187F RID: 6271
		// (get) Token: 0x06006667 RID: 26215 RVA: 0x00084F05 File Offset: 0x00083105
		public virtual AbilityAnimation Animation
		{
			get
			{
				return this.m_animation;
			}
		}

		// Token: 0x04005907 RID: 22791
		[SerializeField]
		protected AbilityAnimation m_animation;
	}
}
