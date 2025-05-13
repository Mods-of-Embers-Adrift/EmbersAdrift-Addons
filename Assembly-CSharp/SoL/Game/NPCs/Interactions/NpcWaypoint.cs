using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs.Interactions
{
	// Token: 0x0200083F RID: 2111
	public class NpcWaypoint : NpcInteractive
	{
		// Token: 0x17000E0A RID: 3594
		// (get) Token: 0x06003CF8 RID: 15608 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool ResetInitialPosition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000E0B RID: 3595
		// (get) Token: 0x06003CF9 RID: 15609 RVA: 0x000694E2 File Offset: 0x000676E2
		public override bool UseLightItemAtNight
		{
			get
			{
				return this.m_controller != null && this.m_controller.UseLightItemAtNight;
			}
		}

		// Token: 0x06003CFA RID: 15610 RVA: 0x000694FF File Offset: 0x000676FF
		public override bool EntityCanInteract(GameEntity entity)
		{
			return this.m_controller != null && this.m_controller.EntityCanInteract(entity);
		}

		// Token: 0x06003CFB RID: 15611 RVA: 0x0006951D File Offset: 0x0006771D
		public override void RegisterOccupant(GameEntity entity)
		{
			if (this.m_controller)
			{
				this.m_controller.RegisterOccupant(entity, this);
			}
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x00069539 File Offset: 0x00067739
		public override void DeregisterOccupant(GameEntity entity)
		{
			if (this.m_controller)
			{
				this.m_controller.DeregisterOccupant(entity);
			}
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x00069554 File Offset: 0x00067754
		public override NpcInteractive GetNextInteractive(GameEntity entity)
		{
			if (!(this.m_controller != null))
			{
				return null;
			}
			return this.m_controller.GetNextWaypoint(entity, this);
		}

		// Token: 0x06003CFE RID: 15614 RVA: 0x00181940 File Offset: 0x0017FB40
		public void Init(NpcPath path)
		{
			UnityEngine.Object controller = this.m_controller;
			AlignWithGround alignWithGround = this.m_alignWithGround;
			this.m_controller = path;
			base.SetupCollider();
			if (!(controller != this.m_controller))
			{
				alignWithGround != this.m_alignWithGround;
			}
		}

		// Token: 0x04003BD6 RID: 15318
		[SerializeField]
		private AlignWithGround m_alignWithGround;

		// Token: 0x04003BD7 RID: 15319
		[SerializeField]
		private NpcPath m_controller;
	}
}
