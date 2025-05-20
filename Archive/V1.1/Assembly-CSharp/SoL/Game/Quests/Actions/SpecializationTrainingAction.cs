using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C6 RID: 1990
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/SpecializationTrainingAction")]
	public class SpecializationTrainingAction : QuestAction
	{
		// Token: 0x06003A40 RID: 14912 RVA: 0x0004475B File Offset: 0x0004295B
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x040038A8 RID: 14504
		[SerializeField]
		private SpecializedRole m_specialization;
	}
}
