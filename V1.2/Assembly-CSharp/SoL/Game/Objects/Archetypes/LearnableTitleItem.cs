using System;
using System.Collections;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A8A RID: 2698
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Learnable Title Consumable")]
	public class LearnableTitleItem : ConsumableItem
	{
		// Token: 0x06005390 RID: 21392 RVA: 0x00077C5D File Offset: 0x00075E5D
		private IEnumerable GetTitles()
		{
			return SolOdinUtilities.GetDropdownItems<LearnableTitle>();
		}

		// Token: 0x17001309 RID: 4873
		// (get) Token: 0x06005391 RID: 21393 RVA: 0x00077C64 File Offset: 0x00075E64
		public override string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(base.DisplayName))
				{
					return base.DisplayName;
				}
				if (!(this.m_title != null))
				{
					return "Title: ???";
				}
				return "Title: " + this.m_title.DisplayName;
			}
		}

		// Token: 0x1700130A RID: 4874
		// (get) Token: 0x06005392 RID: 21394 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700130B RID: 4875
		// (get) Token: 0x06005393 RID: 21395 RVA: 0x00062532 File Offset: 0x00060732
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Consume;
			}
		}

		// Token: 0x1700130C RID: 4876
		// (get) Token: 0x06005394 RID: 21396 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsLearning
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005395 RID: 21397 RVA: 0x001D8524 File Offset: 0x001D6724
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			LearnableArchetype learnableArchetype;
			if (executionCache.SourceEntity.CollectionController.Titles != null && executionCache.SourceEntity.CollectionController.Titles.TryGetLearnableForId(this.m_title.Id, out learnableArchetype))
			{
				executionCache.Message = "You already have this title.";
				executionCache.MsgType = MessageType.Skills;
				return false;
			}
			return true;
		}

		// Token: 0x06005396 RID: 21398 RVA: 0x00077CA3 File Offset: 0x00075EA3
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			executionCache.SourceEntity.CollectionController.Titles.Add(this.m_title, true);
			TitleManager.InvokeTitlesChangedEvent();
		}

		// Token: 0x06005397 RID: 21399 RVA: 0x001D857C File Offset: 0x001D677C
		public bool CanUseAuctionHouse(GameEntity entity)
		{
			LearnableArchetype learnableArchetype;
			return this.m_title && entity && entity.CollectionController != null && entity.CollectionController.Titles != null && !entity.CollectionController.Titles.TryGetLearnableForId(this.m_title.Id, out learnableArchetype);
		}

		// Token: 0x06005398 RID: 21400 RVA: 0x001D85D8 File Offset: 0x001D67D8
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			LearnableArchetype learnableArchetype;
			if (this.m_title && entity && entity.CollectionController != null && entity.CollectionController.Titles != null && entity.CollectionController.Titles.TryGetLearnableForId(this.m_title.Id, out learnableArchetype))
			{
				tooltip.RequirementsBlock.AppendLine("Already known".Color(UIManager.RequirementsNotMetColor), 0);
			}
		}

		// Token: 0x04004A93 RID: 19091
		[SerializeField]
		private LearnableTitle m_title;
	}
}
