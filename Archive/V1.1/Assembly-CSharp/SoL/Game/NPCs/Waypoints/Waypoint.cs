using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs.Waypoints
{
	// Token: 0x0200082B RID: 2091
	public class Waypoint : MonoBehaviour, INpcInteractive
	{
		// Token: 0x06003CB4 RID: 15540 RVA: 0x00180F0C File Offset: 0x0017F10C
		public void SetController(WaypointController controller)
		{
			this.m_controller = controller;
			if (this.m_collider == null)
			{
				this.m_collider = base.gameObject.AddComponent<SphereCollider>();
			}
			base.gameObject.layer = LayerMap.Interaction.Layer;
		}

		// Token: 0x17000DF8 RID: 3576
		// (get) Token: 0x06003CB5 RID: 15541 RVA: 0x000691F7 File Offset: 0x000673F7
		NpcInteractiveType INpcInteractive.ObjectType
		{
			get
			{
				if (!this.m_controller)
				{
					return NpcInteractiveType.None;
				}
				return this.m_controller.InteractiveType;
			}
		}

		// Token: 0x17000DF9 RID: 3577
		// (get) Token: 0x06003CB6 RID: 15542 RVA: 0x00069213 File Offset: 0x00067413
		NpcTypeFlags INpcInteractive.NpcTypes
		{
			get
			{
				if (!this.m_controller)
				{
					return NpcTypeFlags.None;
				}
				return this.m_controller.NpcTypes;
			}
		}

		// Token: 0x04003B7F RID: 15231
		[SerializeField]
		private WaypointController m_controller;

		// Token: 0x04003B80 RID: 15232
		[SerializeField]
		private SphereCollider m_collider;
	}
}
