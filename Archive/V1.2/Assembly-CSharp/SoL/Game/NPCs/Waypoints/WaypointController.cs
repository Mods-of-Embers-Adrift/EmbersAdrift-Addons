using System;
using UnityEngine;

namespace SoL.Game.NPCs.Waypoints
{
	// Token: 0x0200082C RID: 2092
	public class WaypointController : MonoBehaviour
	{
		// Token: 0x17000DFA RID: 3578
		// (get) Token: 0x06003CB8 RID: 15544 RVA: 0x00053500 File Offset: 0x00051700
		internal NpcInteractiveType InteractiveType
		{
			get
			{
				return NpcInteractiveType.Path;
			}
		}

		// Token: 0x17000DFB RID: 3579
		// (get) Token: 0x06003CB9 RID: 15545 RVA: 0x0006922F File Offset: 0x0006742F
		internal NpcTypeFlags NpcTypes
		{
			get
			{
				return this.m_npcTypes;
			}
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x00180FB8 File Offset: 0x0017F1B8
		private void Awake()
		{
			for (int i = 0; i < this.m_waypoints.Length; i++)
			{
				this.m_waypoints[i].SetController(this);
			}
		}

		// Token: 0x04003B81 RID: 15233
		private const string kGizmoGroup = "Gizmos";

		// Token: 0x04003B82 RID: 15234
		[SerializeField]
		private bool m_showHandles = true;

		// Token: 0x04003B83 RID: 15235
		[SerializeField]
		private Color m_gizmoColor = Color.yellow;

		// Token: 0x04003B84 RID: 15236
		[Space]
		[SerializeField]
		private NpcTypeFlags m_npcTypes;

		// Token: 0x04003B85 RID: 15237
		[SerializeField]
		private bool m_loop;

		// Token: 0x04003B86 RID: 15238
		[SerializeField]
		private Waypoint[] m_waypoints;
	}
}
