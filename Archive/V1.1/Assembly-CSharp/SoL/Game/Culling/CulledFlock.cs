using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CB5 RID: 3253
	public class CulledFlock : CulledObject
	{
		// Token: 0x17001791 RID: 6033
		// (get) Token: 0x060062A9 RID: 25257 RVA: 0x00082699 File Offset: 0x00080899
		protected override float Radius
		{
			get
			{
				if (!this.m_controller)
				{
					return base.Radius;
				}
				return this.m_controller.GetSpawnSize().Max() * 0.5f;
			}
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x00205584 File Offset: 0x00203784
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			bool flag = this.IsCulled();
			if (this.m_controller && this.m_controller.gameObject && this.m_controller.gameObject.activeSelf == flag)
			{
				this.m_controller.gameObject.SetActive(!flag);
			}
		}

		// Token: 0x0400560A RID: 22026
		[SerializeField]
		private FlockController m_controller;
	}
}
