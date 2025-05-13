using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000790 RID: 1936
	[CreateAssetMenu(menuName = "SoL/Quests/Reward")]
	public class Reward : ScriptableObject
	{
		// Token: 0x17000D1C RID: 3356
		// (get) Token: 0x06003931 RID: 14641 RVA: 0x00066BA1 File Offset: 0x00064DA1
		public CurrencyValue Currency
		{
			get
			{
				return this.m_currency;
			}
		}

		// Token: 0x17000D1D RID: 3357
		// (get) Token: 0x06003932 RID: 14642 RVA: 0x00066BA9 File Offset: 0x00064DA9
		public RewardItem[] Granted
		{
			get
			{
				return this.m_granted;
			}
		}

		// Token: 0x17000D1E RID: 3358
		// (get) Token: 0x06003933 RID: 14643 RVA: 0x00066BB1 File Offset: 0x00064DB1
		public RewardItem[] Choices
		{
			get
			{
				return this.m_choices;
			}
		}

		// Token: 0x17000D1F RID: 3359
		// (get) Token: 0x06003934 RID: 14644 RVA: 0x001724A0 File Offset: 0x001706A0
		public bool ContainsItems
		{
			get
			{
				RewardItem[] array = this.m_granted;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ContainsItems)
					{
						return true;
					}
				}
				array = this.m_choices;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ContainsItems)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x06003935 RID: 14645 RVA: 0x001724F0 File Offset: 0x001706F0
		public bool HasReissuableRewardsForEntity(GameEntity entity)
		{
			RewardItem[] array = this.m_granted;
			for (int i = 0; i < array.Length; i++)
			{
				string text;
				if (array[i].CanBeReissuedToEntity(entity, out text))
				{
					return true;
				}
			}
			array = this.m_choices;
			for (int i = 0; i < array.Length; i++)
			{
				string text;
				if (array[i].CanBeReissuedToEntity(entity, out text))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003936 RID: 14646 RVA: 0x00172548 File Offset: 0x00170748
		public bool TryGetRewards(GameEntity entity, UniqueId rewardId, out List<RewardItem> rewards, bool reissue = false)
		{
			this.m_tempRewards.Clear();
			foreach (RewardItem rewardItem in this.m_granted)
			{
				string text;
				if ((rewardItem.Requirement == null || rewardItem.Requirement.MeetsAllRequirements(entity)) && (!reissue || rewardItem.CanBeReissuedToEntity(entity, out text)))
				{
					this.m_tempRewards.Add(rewardItem);
				}
			}
			if (!rewardId.IsEmpty)
			{
				foreach (RewardItem rewardItem2 in this.m_choices)
				{
					string text;
					if (rewardItem2.Acquisition(entity).Archetype.Id == rewardId && (!reissue || rewardItem2.CanBeReissuedToEntity(entity, out text)))
					{
						this.m_tempRewards.Add(rewardItem2);
						rewards = this.m_tempRewards;
						return rewardItem2.Requirement == null || rewardItem2.Requirement.MeetsAllRequirements(entity);
					}
				}
			}
			else if (this.m_tempRewards.Count > 0 || (this.m_currency.GetCurrency() > 0UL && !reissue))
			{
				rewards = this.m_tempRewards;
				return true;
			}
			rewards = this.m_tempRewards;
			return false;
		}

		// Token: 0x06003937 RID: 14647 RVA: 0x0017265C File Offset: 0x0017085C
		public void GrantReward(GameEntity entity, UniqueId rewardChoiceId, bool reissue = false)
		{
			List<RewardItem> list;
			string text;
			if (this.TryGetRewards(entity, rewardChoiceId, out list, reissue) && list.EntityCanAcquire(entity, out text))
			{
				foreach (RewardItem rewardItem in list)
				{
					ArchetypeInstance archetypeInstance;
					rewardItem.Acquisition(entity).AddToPlayer(entity, ItemAddContext.Quest, rewardItem.Amount, rewardItem.FlagsToSet, rewardItem.MarkAsSoulbound, out archetypeInstance);
				}
			}
			if (!reissue)
			{
				ulong currency = this.m_currency.GetCurrency();
				if (currency > 0UL)
				{
					CurrencyTransaction currencyTransaction = new CurrencyTransaction
					{
						Add = true,
						Amount = currency,
						TargetContainer = ContainerType.Inventory.ToString(),
						Message = "a quest reward",
						Context = CurrencyContext.Quest
					};
					entity.CollectionController.Inventory.AddCurrency(currencyTransaction.Amount);
					entity.NetworkEntity.PlayerRpcHandler.ProcessCurrencyTransaction(currencyTransaction);
				}
			}
		}

		// Token: 0x040037F7 RID: 14327
		[SerializeField]
		private CurrencyValue m_currency;

		// Token: 0x040037F8 RID: 14328
		[SerializeField]
		private RewardItem[] m_granted;

		// Token: 0x040037F9 RID: 14329
		[SerializeField]
		private RewardItem[] m_choices;

		// Token: 0x040037FA RID: 14330
		private List<RewardItem> m_tempRewards = new List<RewardItem>();
	}
}
