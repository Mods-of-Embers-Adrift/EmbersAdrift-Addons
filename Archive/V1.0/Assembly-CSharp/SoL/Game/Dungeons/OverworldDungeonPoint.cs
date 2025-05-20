using System;
using UnityEngine;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C9A RID: 3226
	public class OverworldDungeonPoint : MonoBehaviour
	{
		// Token: 0x060061F2 RID: 25074 RVA: 0x00081FF3 File Offset: 0x000801F3
		private void OnDrawGizmosSelected()
		{
			if (this.m_controller)
			{
				this.m_controller.DrawGizmosForSelected(base.gameObject);
			}
		}

		// Token: 0x04005574 RID: 21876
		[SerializeField]
		private OverworldDungeonEntranceAlwaysOn m_controller;
	}
}
