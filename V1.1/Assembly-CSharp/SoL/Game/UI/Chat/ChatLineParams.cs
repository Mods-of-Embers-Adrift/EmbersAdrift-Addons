using System;
using Com.TheFallenGames.OSA.Core;
using UnityEngine;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009B7 RID: 2487
	[Serializable]
	public class ChatLineParams : BaseParams
	{
		// Token: 0x17001086 RID: 4230
		// (get) Token: 0x06004B2D RID: 19245 RVA: 0x00072D91 File Offset: 0x00070F91
		public GameObject ItemPrefab
		{
			get
			{
				return this.m_chatLinePrefab;
			}
		}

		// Token: 0x040045C5 RID: 17861
		[SerializeField]
		private GameObject m_chatLinePrefab;
	}
}
