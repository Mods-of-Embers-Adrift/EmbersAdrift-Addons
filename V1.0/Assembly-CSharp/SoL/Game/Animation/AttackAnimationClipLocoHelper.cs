using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D79 RID: 3449
	[CreateAssetMenu(menuName = "SoL/Animation/AnimationClipDescriptions")]
	public class AttackAnimationClipLocoHelper : ScriptableObject
	{
		// Token: 0x04005A4E RID: 23118
		[SerializeField]
		private AttackAnimationClipLocoHelper.AnimationClipSet[] m_sets;

		// Token: 0x02000D7A RID: 3450
		[Serializable]
		private class AnimationClipSet
		{
			// Token: 0x170018F3 RID: 6387
			// (get) Token: 0x060067F7 RID: 26615 RVA: 0x00085BE4 File Offset: 0x00083DE4
			private IEnumerable GetAnimSets
			{
				get
				{
					return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>(null, new string[]
					{
						"Assets/SolData/Animation/Locomotion"
					}, null);
				}
			}

			// Token: 0x04005A4F RID: 23119
			[SerializeField]
			private AnimancerAnimationSet m_locoSet;

			// Token: 0x04005A50 RID: 23120
			public AttackAnimationClipLocoHelper.AnimationClipDescription[] ClipDescriptions;

			// Token: 0x04005A51 RID: 23121
			[SerializeField]
			private AnimationClip[] m_toPopulate;

			// Token: 0x04005A52 RID: 23122
			[SerializeField]
			private AttackAnimationClipLocoHelper.AnimationClipSet.UsedIndex[] m_usedIndexes;

			// Token: 0x02000D7B RID: 3451
			[Serializable]
			private class UsedIndex
			{
				// Token: 0x04005A53 RID: 23123
				private const string kGroup = "GRP";

				// Token: 0x04005A54 RID: 23124
				public AnimationExecutionTime ExecutionTime;

				// Token: 0x04005A55 RID: 23125
				public int Index;

				// Token: 0x04005A56 RID: 23126
				public string Description;

				// Token: 0x04005A57 RID: 23127
				public List<AppliableEffectAbility> Abilities;
			}
		}

		// Token: 0x02000D7C RID: 3452
		[Serializable]
		internal class AnimationClipDescription
		{
			// Token: 0x170018F4 RID: 6388
			// (get) Token: 0x060067FA RID: 26618 RVA: 0x00085E92 File Offset: 0x00084092
			public string IndexName
			{
				get
				{
					if (!(this.Clip != null))
					{
						return "None";
					}
					return this.Clip.name + " (" + this.Description + ")";
				}
			}

			// Token: 0x04005A58 RID: 23128
			public AnimationClip Clip;

			// Token: 0x04005A59 RID: 23129
			public string Description;

			// Token: 0x04005A5A RID: 23130
			public AnimationFlags Flags;

			// Token: 0x04005A5B RID: 23131
			public bool AllowAsAutoAttack;
		}
	}
}
