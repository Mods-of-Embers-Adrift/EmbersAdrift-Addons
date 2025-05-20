using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007BF RID: 1983
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/CurrencyAction")]
	public class CurrencyAction : QuestAction
	{
		// Token: 0x06003A31 RID: 14897 RVA: 0x00067709 File Offset: 0x00065909
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.Execute(cache, sourceEntity);
			bool isServer = GameManager.IsServer;
		}

		// Token: 0x040038A2 RID: 14498
		[SerializeField]
		private AddRemove m_addRemove;

		// Token: 0x040038A3 RID: 14499
		[SerializeField]
		private CurrencyValue m_amount;

		// Token: 0x040038A4 RID: 14500
		[Tooltip("Will display as: \"x added to Inventory via [message]\"")]
		[SerializeField]
		private string m_message;
	}
}
