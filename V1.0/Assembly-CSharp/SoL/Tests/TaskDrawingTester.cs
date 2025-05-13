using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Quests;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DBF RID: 3519
	[CreateAssetMenu(menuName = "SoL/Tests/Task Drawing")]
	public class TaskDrawingTester : ScriptableObject
	{
		// Token: 0x06006925 RID: 26917 RVA: 0x00216B78 File Offset: 0x00214D78
		private void InitBulletinBoardTasks()
		{
			foreach (BBTask bbtask in InternalGameDatabase.BBTasks.GetAllItems())
			{
				if (bbtask != null && bbtask.BulletinBoard != null)
				{
					bbtask.BulletinBoard.Tasks.Clear();
				}
			}
			foreach (BBTask bbtask2 in InternalGameDatabase.BBTasks.GetAllItems())
			{
				if (bbtask2 != null && bbtask2.BulletinBoard != null)
				{
					bbtask2.BulletinBoard.Tasks.Add(bbtask2);
				}
			}
		}

		// Token: 0x06006926 RID: 26918 RVA: 0x0004475B File Offset: 0x0004295B
		private void SampleTask()
		{
		}

		// Token: 0x06006927 RID: 26919 RVA: 0x00216C4C File Offset: 0x00214E4C
		private void RandomlyFillDiscard()
		{
			if (this.m_discardFill <= 0)
			{
				return;
			}
			if (this.m_board == null)
			{
				return;
			}
			this.InitBulletinBoardTasks();
			this.m_playerDiscard.Clear();
			List<BBTask> list = new List<BBTask>(this.m_board.Tasks);
			list.Shuffle<BBTask>();
			for (int i = 0; i < list.Count; i++)
			{
				if (i < this.m_discardFill)
				{
					this.m_playerDiscard.Add(list[i]);
				}
			}
		}

		// Token: 0x1700191F RID: 6431
		// (get) Token: 0x06006928 RID: 26920 RVA: 0x000822E7 File Offset: 0x000804E7
		private IEnumerable GetBoards
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BulletinBoard>();
			}
		}

		// Token: 0x17001920 RID: 6432
		// (get) Token: 0x06006929 RID: 26921 RVA: 0x00086835 File Offset: 0x00084A35
		private IEnumerable GetTasks
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BBTask>();
			}
		}

		// Token: 0x04005B80 RID: 23424
		private const string kSample = "Sample";

		// Token: 0x04005B81 RID: 23425
		[SerializeField]
		private BulletinBoard m_board;

		// Token: 0x04005B82 RID: 23426
		[Range(1f, 50f)]
		[SerializeField]
		private int m_playerLevel = 1;

		// Token: 0x04005B83 RID: 23427
		private const string kDiscard = "Discard";

		// Token: 0x04005B84 RID: 23428
		[SerializeField]
		private int m_discardFill = 1;

		// Token: 0x04005B85 RID: 23429
		[SerializeField]
		private List<BBTask> m_playerDiscard;
	}
}
