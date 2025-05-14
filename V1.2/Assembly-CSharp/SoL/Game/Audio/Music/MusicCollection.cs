using System;
using UnityEngine;

namespace SoL.Game.Audio.Music
{
	// Token: 0x02000D1F RID: 3359
	[CreateAssetMenu(menuName = "SoL/Collections/Music", order = 8)]
	public class MusicCollection : ScriptableObject
	{
		// Token: 0x17001840 RID: 6208
		// (get) Token: 0x06006518 RID: 25880 RVA: 0x000840C8 File Offset: 0x000822C8
		public MusicSetList SetList
		{
			get
			{
				return this.m_setList;
			}
		}

		// Token: 0x040057E3 RID: 22499
		[SerializeField]
		private MusicSetList m_setList;
	}
}
