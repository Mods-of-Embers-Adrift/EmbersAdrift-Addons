using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000580 RID: 1408
	public abstract class GameEntityComponent : MonoBehaviour
	{
		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x06002BF4 RID: 11252 RVA: 0x00147464 File Offset: 0x00145664
		public GameEntity GameEntity
		{
			get
			{
				if (!this.m_cached && !this.m_entity)
				{
					if (!base.gameObject.TryGetComponent<GameEntity>(out this.m_entity))
					{
						Transform parent = base.gameObject.transform.parent;
						while (parent)
						{
							if (parent.TryGetComponent<GameEntity>(out this.m_entity))
							{
								break;
							}
							parent = parent.transform.parent;
						}
					}
					if (!this.m_entity)
					{
						Debug.LogWarning("GameEntity is null on " + base.gameObject.name + "!  " + base.gameObject.transform.GetPath());
					}
					this.m_cached = true;
				}
				return this.m_entity;
			}
		}

		// Token: 0x04002BC1 RID: 11201
		private GameEntity m_entity;

		// Token: 0x04002BC2 RID: 11202
		private bool m_cached;
	}
}
