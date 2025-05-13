using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D4A RID: 3402
	[CreateAssetMenu(menuName = "SoL/Animation/Ability Animation OPTION")]
	public class AbilityAnimationOptionScriptable : AbilityAnimationScriptable
	{
		// Token: 0x1700187D RID: 6269
		// (get) Token: 0x06006664 RID: 26212 RVA: 0x00084EC7 File Offset: 0x000830C7
		private bool m_showAlternate
		{
			get
			{
				return this.m_type > AbilityAnimationOptionScriptable.OptionType.None;
			}
		}

		// Token: 0x1700187E RID: 6270
		// (get) Token: 0x06006665 RID: 26213 RVA: 0x00084ED2 File Offset: 0x000830D2
		public override AbilityAnimation Animation
		{
			get
			{
				if (this.m_type != AbilityAnimationOptionScriptable.OptionType.Learnables)
				{
					return this.m_animation;
				}
				if (!Options.GameOptions.AlternateLearningAnimation.Value)
				{
					return this.m_animation;
				}
				return this.m_alternateAnimation;
			}
		}

		// Token: 0x04005901 RID: 22785
		private const string kAlternate = "Alternate";

		// Token: 0x04005902 RID: 22786
		[SerializeField]
		private AbilityAnimationOptionScriptable.OptionType m_type;

		// Token: 0x04005903 RID: 22787
		[SerializeField]
		private AbilityAnimation m_alternateAnimation;

		// Token: 0x02000D4B RID: 3403
		private enum OptionType
		{
			// Token: 0x04005905 RID: 22789
			None,
			// Token: 0x04005906 RID: 22790
			Learnables
		}
	}
}
