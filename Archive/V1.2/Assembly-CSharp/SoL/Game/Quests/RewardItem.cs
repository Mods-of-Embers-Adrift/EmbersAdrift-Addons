using System;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000792 RID: 1938
	[Serializable]
	public class RewardItem
	{
		// Token: 0x17000D20 RID: 3360
		// (get) Token: 0x06003939 RID: 14649 RVA: 0x00066BCC File Offset: 0x00064DCC
		public bool ContainsItems
		{
			get
			{
				return this.m_type == RewardItemType.Item || this.m_type == RewardItemType.TieredItem;
			}
		}

		// Token: 0x0600393A RID: 14650 RVA: 0x00172788 File Offset: 0x00170988
		public IMerchantInventory Acquisition(GameEntity entity)
		{
			IMerchantInventory result;
			switch (this.m_type)
			{
			case RewardItemType.Item:
				result = this.m_item;
				break;
			case RewardItemType.Mastery:
				result = this.m_masteryBundle;
				break;
			case RewardItemType.Learnable:
				result = this.m_learnable;
				break;
			case RewardItemType.TieredItem:
				result = this.m_tieredItemProfile.GetItemForLevel(entity.CharacterData.AdventuringLevel);
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x0600393B RID: 14651 RVA: 0x00066BE1 File Offset: 0x00064DE1
		public bool CanBeReissuedToEntity(GameEntity entity, out string errorMessage)
		{
			errorMessage = string.Empty;
			return this.CanBeReissued && this.Acquisition(entity).EntityCanAcquire(entity, out errorMessage);
		}

		// Token: 0x04003800 RID: 14336
		[SerializeField]
		private RewardItemType m_type;

		// Token: 0x04003801 RID: 14337
		[SerializeField]
		private ItemArchetype m_item;

		// Token: 0x04003802 RID: 14338
		[SerializeField]
		private MasteryAbilityBundle m_masteryBundle;

		// Token: 0x04003803 RID: 14339
		[SerializeField]
		private LearnableArchetype m_learnable;

		// Token: 0x04003804 RID: 14340
		[SerializeField]
		private TieredItemProfile m_tieredItemProfile;

		// Token: 0x04003805 RID: 14341
		public uint Amount = 1U;

		// Token: 0x04003806 RID: 14342
		public ProgressionRequirement Requirement;

		// Token: 0x04003807 RID: 14343
		public ItemFlags FlagsToSet;

		// Token: 0x04003808 RID: 14344
		public bool MarkAsSoulbound;

		// Token: 0x04003809 RID: 14345
		public bool CanBeReissued;
	}
}
