using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000587 RID: 1415
	[Serializable]
	public class IdentifiableObject<T> where T : UnityEngine.Object
	{
		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x06002C3B RID: 11323 RVA: 0x0005EB7A File Offset: 0x0005CD7A
		public UniqueId Id
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x06002C3C RID: 11324 RVA: 0x0005EB82 File Offset: 0x0005CD82
		public T Obj
		{
			get
			{
				return this.m_object;
			}
		}

		// Token: 0x06002C3D RID: 11325 RVA: 0x0004475B File Offset: 0x0004295B
		private void UpdateId()
		{
		}

		// Token: 0x04002C01 RID: 11265
		[SerializeField]
		private T m_object;

		// Token: 0x04002C02 RID: 11266
		[SerializeField]
		private UniqueId m_id = UniqueId.Empty;
	}
}
