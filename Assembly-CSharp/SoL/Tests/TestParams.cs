using System;
using Com.TheFallenGames.OSA.Core;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DB8 RID: 3512
	[Serializable]
	public class TestParams : BaseParams
	{
		// Token: 0x1700191C RID: 6428
		// (get) Token: 0x0600690F RID: 26895 RVA: 0x000867C8 File Offset: 0x000849C8
		public GameObject ItemPrefab
		{
			get
			{
				return this.m_itemPrefab;
			}
		}

		// Token: 0x04005B6A RID: 23402
		[SerializeField]
		private GameObject m_itemPrefab;
	}
}
