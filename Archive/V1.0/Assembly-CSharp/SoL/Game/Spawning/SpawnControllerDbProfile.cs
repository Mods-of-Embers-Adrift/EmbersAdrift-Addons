using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006B4 RID: 1716
	[CreateAssetMenu(menuName = "SoL/Profiles/Spawn Controller DB")]
	public class SpawnControllerDbProfile : Identifiable
	{
		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x0600343C RID: 13372 RVA: 0x00063D2A File Offset: 0x00061F2A
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x04003260 RID: 12896
		[SerializeField]
		private string m_description;
	}
}
