using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Quests;
using SoL.Managers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200093D RID: 2365
	public class BulletinBoardUI : DraggableUIWindow
	{
		// Token: 0x17000F9C RID: 3996
		// (get) Token: 0x060045CF RID: 17871 RVA: 0x0006EFE0 File Offset: 0x0006D1E0
		// (set) Token: 0x060045D0 RID: 17872 RVA: 0x0006EFE8 File Offset: 0x0006D1E8
		public InteractiveBulletinBoard Interactive
		{
			get
			{
				return this.m_interactive;
			}
			set
			{
				this.m_interactive = value;
				this.m_board = ((this.m_interactive != null) ? this.m_interactive.Board : null);
			}
		}

		// Token: 0x060045D1 RID: 17873 RVA: 0x001A1234 File Offset: 0x0019F434
		protected override void Start()
		{
			base.Start();
			this.m_adventuringCard.CoverClicked += this.OnCoverClicked;
			this.m_adventuringCard.TurnInClicked += this.OnTurnInClicked;
			this.m_craftingCard.CoverClicked += this.OnCoverClicked;
			this.m_craftingCard.TurnInClicked += this.OnTurnInClicked;
			this.m_gatheringCard.CoverClicked += this.OnCoverClicked;
			this.m_gatheringCard.TurnInClicked += this.OnTurnInClicked;
			GameManager.QuestManager.BBTasksUpdated += this.OnTasksUpdated;
		}

		// Token: 0x060045D2 RID: 17874 RVA: 0x001A12E8 File Offset: 0x0019F4E8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_adventuringCard.CoverClicked -= this.OnCoverClicked;
			this.m_adventuringCard.TurnInClicked -= this.OnTurnInClicked;
			this.m_craftingCard.CoverClicked -= this.OnCoverClicked;
			this.m_craftingCard.TurnInClicked -= this.OnTurnInClicked;
			this.m_gatheringCard.CoverClicked -= this.OnCoverClicked;
			this.m_gatheringCard.TurnInClicked -= this.OnTurnInClicked;
			GameManager.QuestManager.BBTasksUpdated -= this.OnTasksUpdated;
		}

		// Token: 0x060045D3 RID: 17875 RVA: 0x001A139C File Offset: 0x0019F59C
		private void Update()
		{
			if (LocalPlayer.GameEntity && base.Visible && this.Interactive != null && !this.Interactive.CanInteract(LocalPlayer.GameEntity))
			{
				this.Hide(false);
				this.Interactive = null;
			}
		}

		// Token: 0x060045D4 RID: 17876 RVA: 0x0006F013 File Offset: 0x0006D213
		public override void Show(bool skipTransition = false)
		{
			base.Show(skipTransition);
			this.RefreshTasks();
			this.RefreshVisuals();
			ClientGameManager.NotificationsManager.TryShowTutorial(TutorialProgress.BulletinBoards);
		}

		// Token: 0x060045D5 RID: 17877 RVA: 0x001A13EC File Offset: 0x0019F5EC
		private void OnCoverClicked(BBTaskType type)
		{
			if (LocalPlayer.GameEntity && QuestManager.TasksAvailable[type])
			{
				GameManager.QuestManager.DrawBBTask(new BBTaskDrawCache
				{
					BulletinBoard = this.m_board,
					Type = type
				}, null);
			}
		}

		// Token: 0x060045D6 RID: 17878 RVA: 0x001A143C File Offset: 0x0019F63C
		private void OnTurnInClicked(BBTask task)
		{
			if (LocalPlayer.GameEntity && task != null)
			{
				GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
				{
					QuestId = task.Id
				}, null, false);
			}
		}

		// Token: 0x060045D7 RID: 17879 RVA: 0x0006F038 File Offset: 0x0006D238
		private void OnTasksUpdated()
		{
			this.RefreshTasks();
			this.RefreshVisuals();
		}

		// Token: 0x060045D8 RID: 17880 RVA: 0x001A1480 File Offset: 0x0019F680
		private void RefreshTasks()
		{
			this.m_adventuringTask = null;
			this.m_craftingTask = null;
			this.m_gatheringTask = null;
			if (this.m_board != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (UniqueId id in LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks.Keys)
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(id, out bbtask) && bbtask.Enabled && bbtask.BulletinBoard.Id == this.m_board.Id)
					{
						switch (bbtask.Type)
						{
						case BBTaskType.Adventuring:
							this.m_adventuringTask = bbtask;
							break;
						case BBTaskType.Crafting:
							this.m_craftingTask = bbtask;
							break;
						case BBTaskType.Gathering:
							this.m_gatheringTask = bbtask;
							break;
						default:
							Debug.LogError("Unknown task type encountered!");
							break;
						}
					}
				}
			}
		}

		// Token: 0x060045D9 RID: 17881 RVA: 0x001A15EC File Offset: 0x0019F7EC
		private void RefreshVisuals()
		{
			this.m_adventuringLabel.ZStringSetText(QuestManager.TasksAvailable[BBTaskType.Adventuring] ? "Adventuring" : "Coming Soon...");
			this.m_craftingLabel.ZStringSetText(QuestManager.TasksAvailable[BBTaskType.Crafting] ? "Crafting" : "Coming Soon...");
			this.m_gatheringLabel.ZStringSetText(QuestManager.TasksAvailable[BBTaskType.Gathering] ? "Gathering" : "Coming Soon...");
			this.m_adventuringCard.gameObject.SetActive(QuestManager.TasksAvailable[BBTaskType.Adventuring]);
			this.m_craftingCard.gameObject.SetActive(QuestManager.TasksAvailable[BBTaskType.Crafting]);
			this.m_gatheringCard.gameObject.SetActive(QuestManager.TasksAvailable[BBTaskType.Gathering]);
			if (this.m_board == null)
			{
				if (this.m_adventuringCard.gameObject.activeInHierarchy)
				{
					this.m_adventuringCard.Init(true, false, BBTaskType.Adventuring, this.m_adventuringTask);
				}
				if (this.m_craftingCard.gameObject.activeInHierarchy)
				{
					this.m_craftingCard.Init(true, false, BBTaskType.Crafting, this.m_craftingTask);
				}
				if (this.m_gatheringCard.gameObject.activeInHierarchy)
				{
					this.m_gatheringCard.Init(true, false, BBTaskType.Gathering, this.m_gatheringTask);
					return;
				}
			}
			else
			{
				this.m_boardTitle.ZStringSetText(this.m_board.Title);
				if (this.m_adventuringCard.gameObject.activeInHierarchy)
				{
					this.m_adventuringCard.Init(true, this.m_adventuringTask != null, BBTaskType.Adventuring, this.m_adventuringTask);
				}
				if (this.m_craftingCard.gameObject.activeInHierarchy)
				{
					this.m_craftingCard.Init(true, this.m_craftingTask != null, BBTaskType.Crafting, this.m_craftingTask);
				}
				if (this.m_gatheringCard.gameObject.activeInHierarchy)
				{
					this.m_gatheringCard.Init(true, this.m_gatheringTask != null, BBTaskType.Gathering, this.m_gatheringTask);
				}
			}
		}

		// Token: 0x04004211 RID: 16913
		[SerializeField]
		private TextMeshProUGUI m_boardTitle;

		// Token: 0x04004212 RID: 16914
		[SerializeField]
		private TextMeshProUGUI m_adventuringLabel;

		// Token: 0x04004213 RID: 16915
		[SerializeField]
		private TextMeshProUGUI m_craftingLabel;

		// Token: 0x04004214 RID: 16916
		[SerializeField]
		private TextMeshProUGUI m_gatheringLabel;

		// Token: 0x04004215 RID: 16917
		[SerializeField]
		private TaskCard m_adventuringCard;

		// Token: 0x04004216 RID: 16918
		[SerializeField]
		private TaskCard m_craftingCard;

		// Token: 0x04004217 RID: 16919
		[SerializeField]
		private TaskCard m_gatheringCard;

		// Token: 0x04004218 RID: 16920
		[SerializeField]
		private Image m_gmTray;

		// Token: 0x04004219 RID: 16921
		[SerializeField]
		private TaskCardList m_gmTaskList;

		// Token: 0x0400421A RID: 16922
		private BBTask m_adventuringTask;

		// Token: 0x0400421B RID: 16923
		private BBTask m_craftingTask;

		// Token: 0x0400421C RID: 16924
		private BBTask m_gatheringTask;

		// Token: 0x0400421D RID: 16925
		private BulletinBoard m_board;

		// Token: 0x0400421E RID: 16926
		private InteractiveBulletinBoard m_interactive;
	}
}
