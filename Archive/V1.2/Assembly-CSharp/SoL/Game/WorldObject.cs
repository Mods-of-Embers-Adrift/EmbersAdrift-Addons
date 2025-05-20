using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000605 RID: 1541
	public abstract class WorldObject : MonoBehaviour, IWorldObject
	{
		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x0600312B RID: 12587 RVA: 0x00061DED File Offset: 0x0005FFED
		public UniqueId WorldId
		{
			get
			{
				return this.m_worldId;
			}
		}

		// Token: 0x0600312C RID: 12588 RVA: 0x00061DF5 File Offset: 0x0005FFF5
		protected virtual void Awake()
		{
			LocalZoneManager.RegisterWorldObject(this);
		}

		// Token: 0x0600312D RID: 12589 RVA: 0x00061DFD File Offset: 0x0005FFFD
		protected virtual void OnDestroy()
		{
			LocalZoneManager.DeregisterWorldObject(this);
		}

		// Token: 0x0600312E RID: 12590 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool Validate(GameEntity entity)
		{
			return true;
		}

		// Token: 0x0600312F RID: 12591 RVA: 0x00061E05 File Offset: 0x00060005
		bool IWorldObject.Validate(GameEntity entity)
		{
			return this.Validate(entity);
		}

		// Token: 0x06003131 RID: 12593 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IWorldObject.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002F5F RID: 12127
		[SerializeField]
		private UniqueId m_worldId = UniqueId.Empty;
	}
}
