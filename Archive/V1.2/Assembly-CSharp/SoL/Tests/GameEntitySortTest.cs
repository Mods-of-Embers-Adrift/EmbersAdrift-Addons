using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Spawning;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DA2 RID: 3490
	public class GameEntitySortTest : MonoBehaviour
	{
		// Token: 0x060068A4 RID: 26788 RVA: 0x002153D8 File Offset: 0x002135D8
		private void Compare()
		{
			List<GameEntity> list = new List<GameEntity>(this.m_entities);
			CallForHelpExtensions.SortListBySqrMagnitudeDistance(this.m_entity, list);
			for (int i = 0; i < list.Count; i++)
			{
				Debug.Log(list[i].gameObject.name ?? "");
			}
		}

		// Token: 0x060068A5 RID: 26789 RVA: 0x00086370 File Offset: 0x00084570
		private void Shuffle()
		{
			this.m_entities.Shuffle<GameEntity>();
		}

		// Token: 0x04005AF5 RID: 23285
		[SerializeField]
		private GameEntity m_entity;

		// Token: 0x04005AF6 RID: 23286
		[SerializeField]
		private List<GameEntity> m_entities = new List<GameEntity>();
	}
}
