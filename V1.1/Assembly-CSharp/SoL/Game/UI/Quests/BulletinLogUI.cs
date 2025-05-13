using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Objects;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200093E RID: 2366
	public class BulletinLogUI : MonoBehaviour
	{
		// Token: 0x17000F9D RID: 3997
		// (get) Token: 0x060045DB RID: 17883 RVA: 0x0006F046 File Offset: 0x0006D246
		public BulletinBoard SelectedBoard
		{
			get
			{
				return this.m_boardsList.SelectedItem;
			}
		}

		// Token: 0x060045DC RID: 17884 RVA: 0x001A17E4 File Offset: 0x0019F9E4
		protected void Start()
		{
			this.m_boardsList.SelectionChanged += this.OnSelectionChanged;
			LootObjective.LootAmountChanged += this.OnLootAmountChanged;
			DiscoveryProgression.DiscoveryFound += this.OnDiscoveriesChanged;
			GameManager.QuestManager.BBTasksUpdated += this.OnTasksUpdated;
			if (LocalPlayer.IsInitialized)
			{
				this.OnLocalPlayerInitialized();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x060045DD RID: 17885 RVA: 0x001A1860 File Offset: 0x0019FA60
		protected void OnDestroy()
		{
			this.m_boardsList.SelectionChanged -= this.OnSelectionChanged;
			LootObjective.LootAmountChanged -= this.OnLootAmountChanged;
			DiscoveryProgression.DiscoveryFound -= this.OnDiscoveriesChanged;
			GameManager.QuestManager.BBTasksUpdated -= this.OnTasksUpdated;
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.OnEmberStoneChanged;
			}
			this.m_boardsList.Initialized -= this.UpdateList;
		}

		// Token: 0x060045DE RID: 17886 RVA: 0x0006F053 File Offset: 0x0006D253
		public void Show()
		{
			this.UpdateListWhenReady();
			this.RefreshVisuals();
		}

		// Token: 0x060045DF RID: 17887 RVA: 0x0006F061 File Offset: 0x0006D261
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.OnEmberStoneChanged;
		}

		// Token: 0x060045E0 RID: 17888 RVA: 0x0006F08F File Offset: 0x0006D28F
		private void OnSelectionChanged(BulletinBoard board)
		{
			this.RefreshVisuals();
		}

		// Token: 0x060045E1 RID: 17889 RVA: 0x0006F08F File Offset: 0x0006D28F
		private void OnLootAmountChanged(LootObjective objective)
		{
			this.RefreshVisuals();
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0006F08F File Offset: 0x0006D28F
		private void OnDiscoveriesChanged(DiscoveryProfile discovery)
		{
			this.RefreshVisuals();
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x0006F08F File Offset: 0x0006D28F
		private void OnEmberStoneChanged()
		{
			this.RefreshVisuals();
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x001A18FC File Offset: 0x0019FAFC
		private void OnTasksUpdated()
		{
			if (LocalPlayer.GameEntity)
			{
				ICollectionController collectionController = LocalPlayer.GameEntity.CollectionController;
				bool flag;
				if (collectionController == null)
				{
					flag = (null != null);
				}
				else
				{
					CharacterRecord record = collectionController.Record;
					if (record == null)
					{
						flag = (null != null);
					}
					else
					{
						PlayerProgressionData progression = record.Progression;
						flag = (((progression != null) ? progression.BBTasks : null) != null);
					}
				}
				if (!flag || LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks.Count == 0)
				{
					this.m_boardsList.DeselectAll(false);
				}
			}
			this.UpdateListWhenReady();
			this.RefreshVisuals();
		}

		// Token: 0x060045E5 RID: 17893 RVA: 0x001A1980 File Offset: 0x0019FB80
		public void RefreshVisuals()
		{
			if (this.SelectedBoard == null)
			{
				this.m_noSelection.gameObject.SetActive(true);
				this.m_boardDetail.SetActive(false);
				return;
			}
			this.m_noSelection.gameObject.SetActive(false);
			this.m_boardDetail.SetActive(true);
			this.m_boardTitle.ZStringSetText(this.SelectedBoard.Title);
			BBTask task = null;
			BBTask task2 = null;
			BBTask task3 = null;
			if (LocalPlayer.GameEntity)
			{
				ICollectionController collectionController = LocalPlayer.GameEntity.CollectionController;
				bool flag;
				if (collectionController == null)
				{
					flag = (null != null);
				}
				else
				{
					CharacterRecord record = collectionController.Record;
					if (record == null)
					{
						flag = (null != null);
					}
					else
					{
						PlayerProgressionData progression = record.Progression;
						flag = (((progression != null) ? progression.BBTasks : null) != null);
					}
				}
				if (flag && LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks.Count > 0)
				{
					foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair.Key, out bbtask) && bbtask.Enabled && bbtask.BulletinBoard.Id == this.SelectedBoard.Id)
						{
							switch (bbtask.Type)
							{
							case BBTaskType.Adventuring:
								task = bbtask;
								break;
							case BBTaskType.Crafting:
								task2 = bbtask;
								break;
							case BBTaskType.Gathering:
								task3 = bbtask;
								break;
							}
						}
					}
				}
			}
			if (QuestManager.TasksAvailable[BBTaskType.Adventuring])
			{
				this.m_adventuringCard.Init(false, true, BBTaskType.Adventuring, task);
			}
			else
			{
				this.m_adventuringCard.Init(false, true, BBTaskType.Invalid, null);
			}
			if (QuestManager.TasksAvailable[BBTaskType.Crafting])
			{
				this.m_craftingCard.Init(false, true, BBTaskType.Crafting, task2);
			}
			else
			{
				this.m_craftingCard.Init(false, true, BBTaskType.Invalid, null);
			}
			if (QuestManager.TasksAvailable[BBTaskType.Gathering])
			{
				this.m_gatheringCard.Init(false, true, BBTaskType.Gathering, task3);
				return;
			}
			this.m_gatheringCard.Init(false, true, BBTaskType.Invalid, null);
		}

		// Token: 0x060045E6 RID: 17894 RVA: 0x0006F097 File Offset: 0x0006D297
		private void UpdateListWhenReady()
		{
			if (this.m_boardsList.IsInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_boardsList.Initialized += this.UpdateList;
		}

		// Token: 0x060045E7 RID: 17895 RVA: 0x001A1B9C File Offset: 0x0019FD9C
		private void UpdateList()
		{
			List<BulletinBoard> fromPool = StaticListPool<BulletinBoard>.GetFromPool();
			if (LocalPlayer.GameEntity)
			{
				PlayerProgressionData progression = LocalPlayer.GameEntity.CollectionController.Record.Progression;
				if (((progression != null) ? progression.BBTasks : null) != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks.Count > 0)
				{
					foreach (UniqueId id in LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks.Keys)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(id, out bbtask) && bbtask.Enabled && !fromPool.Contains(bbtask.BulletinBoard))
						{
							fromPool.Add(bbtask.BulletinBoard);
						}
					}
				}
			}
			fromPool.Sort((BulletinBoard a, BulletinBoard b) => a.Title.TitleCompare(b.Title));
			this.m_boardsList.UpdateItems(fromPool);
			this.m_boardsList.ReindexItems(this.SelectedBoard);
			StaticListPool<BulletinBoard>.ReturnToPool(fromPool);
		}

		// Token: 0x0400421F RID: 16927
		[SerializeField]
		private BoardsList m_boardsList;

		// Token: 0x04004220 RID: 16928
		[SerializeField]
		private TextMeshProUGUI m_noSelection;

		// Token: 0x04004221 RID: 16929
		[SerializeField]
		private GameObject m_boardDetail;

		// Token: 0x04004222 RID: 16930
		[SerializeField]
		private TextMeshProUGUI m_boardTitle;

		// Token: 0x04004223 RID: 16931
		[SerializeField]
		private TaskCard m_adventuringCard;

		// Token: 0x04004224 RID: 16932
		[SerializeField]
		private TaskCard m_craftingCard;

		// Token: 0x04004225 RID: 16933
		[SerializeField]
		private TaskCard m_gatheringCard;

		// Token: 0x04004226 RID: 16934
		public string PlayerPrefsKey = string.Empty;
	}
}
