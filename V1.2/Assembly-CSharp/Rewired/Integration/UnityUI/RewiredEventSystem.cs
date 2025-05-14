using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UnityUI
{
	// Token: 0x02000070 RID: 112
	[AddComponentMenu("Rewired/Rewired Event System")]
	public class RewiredEventSystem : EventSystem
	{
		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x0004619B File Offset: 0x0004439B
		// (set) Token: 0x06000475 RID: 1141 RVA: 0x000461A3 File Offset: 0x000443A3
		public bool alwaysUpdate
		{
			get
			{
				return this._alwaysUpdate;
			}
			set
			{
				this._alwaysUpdate = value;
			}
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x0009C7AC File Offset: 0x0009A9AC
		protected override void Update()
		{
			if (this.alwaysUpdate)
			{
				EventSystem current = EventSystem.current;
				if (current != this)
				{
					EventSystem.current = this;
				}
				try
				{
					base.Update();
					return;
				}
				finally
				{
					if (current != this)
					{
						EventSystem.current = current;
					}
				}
			}
			base.Update();
		}

		// Token: 0x0400057A RID: 1402
		[Tooltip("If enabled, the Event System will be updated every frame even if other Event Systems are enabled. Otherwise, only EventSystem.current will be updated.")]
		[SerializeField]
		private bool _alwaysUpdate;
	}
}
