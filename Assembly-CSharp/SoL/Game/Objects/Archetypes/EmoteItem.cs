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
	// Token: 0x02000A6F RID: 2671
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Emote Item")]
	public class EmoteItem : ConsumableItem
	{
		// Token: 0x060052A5 RID: 21157 RVA: 0x000772FF File Offset: 0x000754FF
		private IEnumerable GetEmotes()
		{
			return SolOdinUtilities.GetDropdownItems<Emote>();
		}

		// Token: 0x170012C1 RID: 4801
		// (get) Token: 0x060052A6 RID: 21158 RVA: 0x00077306 File Offset: 0x00075506
		public override string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(base.DisplayName))
				{
					return base.DisplayName;
				}
				if (!(this.m_emote != null))
				{
					return "Emote: ???";
				}
				return "Emote: " + this.m_emote.DisplayName;
			}
		}

		// Token: 0x170012C2 RID: 4802
		// (get) Token: 0x060052A7 RID: 21159 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170012C3 RID: 4803
		// (get) Token: 0x060052A8 RID: 21160 RVA: 0x00062532 File Offset: 0x00060732
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Consume;
			}
		}

		// Token: 0x170012C4 RID: 4804
		// (get) Token: 0x060052A9 RID: 21161 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsLearning
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060052AA RID: 21162 RVA: 0x001D4668 File Offset: 0x001D2868
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			LearnableArchetype learnableArchetype;
			if (executionCache.SourceEntity.CollectionController.Emotes != null && executionCache.SourceEntity.CollectionController.Emotes.TryGetLearnableForId(this.m_emote.Id, out learnableArchetype))
			{
				executionCache.Message = "You already know this.";
				executionCache.MsgType = MessageType.Skills;
				return false;
			}
			return true;
		}

		// Token: 0x060052AB RID: 21163 RVA: 0x00077345 File Offset: 0x00075545
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			executionCache.SourceEntity.CollectionController.Emotes.Add(this.m_emote, true);
		}

		// Token: 0x060052AC RID: 21164 RVA: 0x001D46C0 File Offset: 0x001D28C0
		public bool CanUseAuctionHouse(GameEntity entity)
		{
			LearnableArchetype learnableArchetype;
			return this.m_emote && entity && entity.CollectionController != null && entity.CollectionController.Emotes != null && !entity.CollectionController.Emotes.TryGetLearnableForId(this.m_emote.Id, out learnableArchetype);
		}

		// Token: 0x060052AD RID: 21165 RVA: 0x001D471C File Offset: 0x001D291C
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			LearnableArchetype learnableArchetype;
			if (this.m_emote && entity && entity.CollectionController != null && entity.CollectionController.Emotes != null && entity.CollectionController.Emotes.TryGetLearnableForId(this.m_emote.Id, out learnableArchetype))
			{
				tooltip.RequirementsBlock.AppendLine("Already known".Color(UIManager.RequirementsNotMetColor), 0);
			}
		}

		// Token: 0x040049DF RID: 18911
		[SerializeField]
		private Emote m_emote;
	}
}
