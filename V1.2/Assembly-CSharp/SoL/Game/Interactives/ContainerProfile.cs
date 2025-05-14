using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B7A RID: 2938
	public class ContainerProfile : BaseArchetype
	{
		// Token: 0x06005A88 RID: 23176 RVA: 0x0007C9DA File Offset: 0x0007ABDA
		public virtual int GetMaxCapacity(ContainerRecord containerRecord)
		{
			return this.m_maxCapacity;
		}

		// Token: 0x04004F74 RID: 20340
		[SerializeField]
		protected int m_maxCapacity = 24;
	}
}
