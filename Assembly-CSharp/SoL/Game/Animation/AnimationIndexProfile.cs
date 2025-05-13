using System;
using System.Collections;
using Cysharp.Text;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D74 RID: 3444
	[CreateAssetMenu(menuName = "SoL/Animation/Animation Index Profile")]
	public class AnimationIndexProfile : ScriptableObject
	{
		// Token: 0x060067B4 RID: 26548 RVA: 0x00213394 File Offset: 0x00211594
		internal string GetAnimIndexDescription()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				for (int i = 0; i < this.m_indexes.Length; i++)
				{
					if (this.m_indexes[i].Set)
					{
						utf16ValueStringBuilder.Append(AnimancerUtilities.GetAnimIndexDescription(this.m_indexes[i].Set, this.m_exeTime, this.m_indexes[i].Index, true, false));
						if (i < this.m_indexes.Length - 1)
						{
							utf16ValueStringBuilder.AppendLine();
						}
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060067B5 RID: 26549 RVA: 0x00213448 File Offset: 0x00211648
		public AbilityAnimation GetAbilityAnimation(UniqueId setId)
		{
			if (this.m_indexes == null || this.m_indexes.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < this.m_indexes.Length; i++)
			{
				if (this.m_indexes[i].Set && this.m_indexes[i].Set.Id == setId)
				{
					return this.m_indexes[i].Set.GetAbilityAnimation(this.m_exeTime, this.m_indexes[i].Index);
				}
			}
			return null;
		}

		// Token: 0x04005A24 RID: 23076
		[SerializeField]
		private AnimationExecutionTime m_exeTime;

		// Token: 0x04005A25 RID: 23077
		[SerializeField]
		private AnimationIndexProfile.AnimationIndex[] m_indexes;

		// Token: 0x04005A26 RID: 23078
		[SerializeField]
		private DummyClass m_animDescriptionDummy;

		// Token: 0x02000D75 RID: 3445
		[Serializable]
		private class AnimationIndex
		{
			// Token: 0x170018D5 RID: 6357
			// (get) Token: 0x060067B7 RID: 26551 RVA: 0x00085B9E File Offset: 0x00083D9E
			public AnimancerAnimationSet Set
			{
				get
				{
					return this.m_locomotion;
				}
			}

			// Token: 0x170018D6 RID: 6358
			// (get) Token: 0x060067B8 RID: 26552 RVA: 0x00085BA6 File Offset: 0x00083DA6
			public int Index
			{
				get
				{
					return this.m_index;
				}
			}

			// Token: 0x170018D7 RID: 6359
			// (get) Token: 0x060067B9 RID: 26553 RVA: 0x00085BAE File Offset: 0x00083DAE
			// (set) Token: 0x060067BA RID: 26554 RVA: 0x00085BB6 File Offset: 0x00083DB6
			internal AnimationExecutionTime ParentExeTime { get; set; }

			// Token: 0x060067BB RID: 26555 RVA: 0x00085BBF File Offset: 0x00083DBF
			public string GetAnimIndexDescription(AnimationExecutionTime exeTime)
			{
				if (!this.m_locomotion)
				{
					return null;
				}
				return AnimancerUtilities.GetAnimIndexDescription(this.m_locomotion, exeTime, this.m_index, true, false);
			}

			// Token: 0x060067BC RID: 26556 RVA: 0x00049FFA File Offset: 0x000481FA
			private IEnumerable GetDropdownItems()
			{
				return null;
			}

			// Token: 0x170018D8 RID: 6360
			// (get) Token: 0x060067BD RID: 26557 RVA: 0x00085BE4 File Offset: 0x00083DE4
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

			// Token: 0x04005A27 RID: 23079
			[SerializeField]
			private AnimancerAnimationSet m_locomotion;

			// Token: 0x04005A28 RID: 23080
			[SerializeField]
			private int m_index;
		}
	}
}
