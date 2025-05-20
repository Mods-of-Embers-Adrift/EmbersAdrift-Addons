using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F1 RID: 753
	[Serializable]
	public abstract class ObjectToggle<TObject> : ITogglable where TObject : UnityEngine.Object
	{
		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x0600156A RID: 5482 RVA: 0x000510DD File Offset: 0x0004F2DD
		// (set) Token: 0x0600156B RID: 5483 RVA: 0x000510E5 File Offset: 0x0004F2E5
		public ToggleController.ToggleState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				if (this.m_state == value)
				{
					return;
				}
				this.m_state = value;
				this.UpdateObject();
			}
		}

		// Token: 0x0600156C RID: 5484
		protected abstract void UpdateObject();

		// Token: 0x04001D8B RID: 7563
		[SerializeField]
		protected TObject m_object;

		// Token: 0x04001D8C RID: 7564
		private ToggleController.ToggleState m_state;
	}
}
