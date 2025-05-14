using System;
using SoL.Game.Messages;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A69 RID: 2665
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Quest Starter Item")]
	public class QuestStarterItem : ConsumableItem
	{
		// Token: 0x170012B8 RID: 4792
		// (get) Token: 0x06005289 RID: 21129 RVA: 0x000771C8 File Offset: 0x000753C8
		public Quest Quest
		{
			get
			{
				return this.m_quest;
			}
		}

		// Token: 0x170012B9 RID: 4793
		// (get) Token: 0x0600528A RID: 21130 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170012BA RID: 4794
		// (get) Token: 0x0600528B RID: 21131 RVA: 0x00062532 File Offset: 0x00060732
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Consume;
			}
		}

		// Token: 0x0600528C RID: 21132 RVA: 0x001D4280 File Offset: 0x001D2480
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			if (!this.m_quest.Enabled)
			{
				executionCache.Message = "That quest is currently disabled!";
				executionCache.MsgType = MessageType.Quest;
				return false;
			}
			if (this.m_quest.IsOnQuest(executionCache.SourceEntity))
			{
				executionCache.Message = "You are already on that quest!";
				executionCache.MsgType = MessageType.Quest;
				return false;
			}
			if (!this.m_quest.Requirements.Role.MeetsAllRequirements(executionCache.SourceEntity))
			{
				executionCache.Message = "That quest is for a different role!";
				executionCache.MsgType = MessageType.Quest;
				return false;
			}
			if (!this.m_quest.Requirements.Level.MeetsAllRequirements(executionCache.SourceEntity))
			{
				executionCache.Message = "You are not of a high enough level for that quest!";
				executionCache.MsgType = MessageType.Quest;
				return false;
			}
			return true;
		}

		// Token: 0x0600528D RID: 21133 RVA: 0x000771D0 File Offset: 0x000753D0
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			bool isServer = GameManager.IsServer;
		}

		// Token: 0x0600528E RID: 21134 RVA: 0x001D4348 File Offset: 0x001D2548
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			this.m_quest.Requirements.FillTooltipBlocks(tooltip, instance, entity);
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			if (this.m_quest.IsOnQuest(entity))
			{
				dataBlock.AppendLine("\nRight-click to start a quest".Italicize(), "Already on Quest".Color(UIManager.RequirementsNotMetColor));
				return;
			}
			dataBlock.AppendLine("\nRight-click to start a quest".Italicize(), 0);
		}

		// Token: 0x040049CC RID: 18892
		[SerializeField]
		private Quest m_quest;

		// Token: 0x040049CD RID: 18893
		[Tooltip("Note that when indicating an alternate start, one must be able to pass validation for the indicated objective.")]
		[SerializeField]
		private bool m_alternateStart;

		// Token: 0x040049CE RID: 18894
		[SerializeField]
		private QuestObjective m_starterObjective;
	}
}
