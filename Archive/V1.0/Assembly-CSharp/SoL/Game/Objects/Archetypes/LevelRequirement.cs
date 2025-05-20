using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A95 RID: 2709
	[Serializable]
	public class LevelRequirement : IRequirement
	{
		// Token: 0x17001345 RID: 4933
		// (get) Token: 0x060053F9 RID: 21497 RVA: 0x00078226 File Offset: 0x00076426
		private bool m_showRequiredTrade
		{
			get
			{
				return this.m_type > LevelRequirement.RequirementType.Adventuring;
			}
		}

		// Token: 0x17001346 RID: 4934
		// (get) Token: 0x060053FA RID: 21498 RVA: 0x00078231 File Offset: 0x00076431
		private IEnumerable GetTradeRoles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BaseRole>(delegate(BaseRole x)
				{
					LevelRequirement.RequirementType type = this.m_type;
					if (type != LevelRequirement.RequirementType.Gathering)
					{
						return type == LevelRequirement.RequirementType.Crafting && x.Type == MasteryType.Trade;
					}
					return x.Type == MasteryType.Harvesting;
				});
			}
		}

		// Token: 0x17001347 RID: 4935
		// (get) Token: 0x060053FB RID: 21499 RVA: 0x00078244 File Offset: 0x00076444
		public int Level
		{
			get
			{
				return this.m_level;
			}
		}

		// Token: 0x17001348 RID: 4936
		// (get) Token: 0x060053FC RID: 21500 RVA: 0x0007824C File Offset: 0x0007644C
		public LevelRequirement.RequirementType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x060053FD RID: 21501 RVA: 0x00078254 File Offset: 0x00076454
		public LevelRequirement(int level, LevelRequirement.RequirementType reqType = LevelRequirement.RequirementType.Adventuring)
		{
			this.m_type = reqType;
			this.m_level = level;
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x001D993C File Offset: 0x001D7B3C
		public bool MeetsAllRequirements(GameEntity entity)
		{
			if (!entity || !entity.CharacterData)
			{
				return false;
			}
			switch (this.m_type)
			{
			case LevelRequirement.RequirementType.Adventuring:
				return entity.CharacterData.AdventuringLevel >= this.m_level;
			case LevelRequirement.RequirementType.Gathering:
				if (!this.m_requiredTrade)
				{
					return entity.CharacterData.GatheringLevel >= this.m_level;
				}
				return this.HasRequiredTradeAtLevel(entity);
			case LevelRequirement.RequirementType.Crafting:
				if (!this.m_requiredTrade)
				{
					return entity.CharacterData.CraftingLevel >= this.m_level;
				}
				return this.HasRequiredTradeAtLevel(entity);
			default:
				return false;
			}
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x001D99EC File Offset: 0x001D7BEC
		public bool HasRequiredTrade(GameEntity entity)
		{
			if (!entity || !entity.CharacterData)
			{
				return false;
			}
			LevelRequirement.RequirementType type = this.m_type;
			return type - LevelRequirement.RequirementType.Gathering > 1 || !this.m_requiredTrade || this.HasRequiredTrade_Internal(entity);
		}

		// Token: 0x06005400 RID: 21504 RVA: 0x001D9A38 File Offset: 0x001D7C38
		private bool HasRequiredTrade_Internal(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			return entity && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_requiredTrade.Id, out archetypeInstance);
		}

		// Token: 0x06005401 RID: 21505 RVA: 0x001D9A84 File Offset: 0x001D7C84
		private bool HasRequiredTradeAtLevel(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			return entity && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_requiredTrade.Id, out archetypeInstance) && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.BaseLevel >= (float)this.m_level;
		}

		// Token: 0x06005402 RID: 21506 RVA: 0x001D9AF0 File Offset: 0x001D7CF0
		public bool ShowLevelRequirementForTooltip(GameEntity entity, out bool meetsLevelRequirement)
		{
			meetsLevelRequirement = false;
			if (this.m_level > 1)
			{
				switch (this.m_type)
				{
				case LevelRequirement.RequirementType.Adventuring:
					meetsLevelRequirement = (entity && entity.CharacterData && entity.CharacterData.AdventuringLevel >= this.m_level);
					break;
				case LevelRequirement.RequirementType.Gathering:
					meetsLevelRequirement = (this.m_requiredTrade ? this.HasRequiredTradeAtLevel(entity) : (entity && entity.CharacterData && entity.CharacterData.GatheringLevel >= this.m_level));
					break;
				case LevelRequirement.RequirementType.Crafting:
					meetsLevelRequirement = (this.m_requiredTrade ? this.HasRequiredTradeAtLevel(entity) : (entity && entity.CharacterData && entity.CharacterData.CraftingLevel >= this.m_level));
					break;
				}
				return true;
			}
			return false;
		}

		// Token: 0x04004AC1 RID: 19137
		[SerializeField]
		private LevelRequirement.RequirementType m_type;

		// Token: 0x04004AC2 RID: 19138
		[Range(1f, 50f)]
		[SerializeField]
		private int m_level = 1;

		// Token: 0x04004AC3 RID: 19139
		[SerializeField]
		private BaseRole m_requiredTrade;

		// Token: 0x02000A96 RID: 2710
		public enum RequirementType
		{
			// Token: 0x04004AC5 RID: 19141
			Adventuring,
			// Token: 0x04004AC6 RID: 19142
			Gathering,
			// Token: 0x04004AC7 RID: 19143
			Crafting
		}
	}
}
