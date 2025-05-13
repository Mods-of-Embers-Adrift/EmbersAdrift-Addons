using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002ED RID: 749
	public class ToggleController : MonoBehaviour
	{
		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06001557 RID: 5463 RVA: 0x000FC5D0 File Offset: 0x000FA7D0
		// (remove) Token: 0x06001558 RID: 5464 RVA: 0x000FC608 File Offset: 0x000FA808
		public event Action<ToggleController.ToggleState> ToggleChanged;

		// Token: 0x06001559 RID: 5465 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateTogglesInternal()
		{
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x0600155A RID: 5466 RVA: 0x0005103F File Offset: 0x0004F23F
		// (set) Token: 0x0600155B RID: 5467 RVA: 0x00051047 File Offset: 0x0004F247
		public ToggleController.ToggleState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_initialized = true;
				if (this.m_state == value)
				{
					return;
				}
				this.m_state = value;
				this.UpdateToggles();
				Action<ToggleController.ToggleState> toggleChanged = this.ToggleChanged;
				if (toggleChanged == null)
				{
					return;
				}
				toggleChanged(this.m_state);
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x0005107D File Offset: 0x0004F27D
		private void Start()
		{
			if (this.m_initialized)
			{
				return;
			}
			this.State = this.m_initialState;
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00051094 File Offset: 0x0004F294
		public void Toggle(bool isOn)
		{
			this.State = (isOn ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x000510A3 File Offset: 0x0004F2A3
		public void Toggle()
		{
			this.State = ((this.State == ToggleController.ToggleState.ON) ? ToggleController.ToggleState.OFF : ToggleController.ToggleState.ON);
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000FC640 File Offset: 0x000FA840
		private void UpdateToggles()
		{
			for (int i = 0; i < this.m_gameObjects.Length; i++)
			{
				this.m_gameObjects[i].State = this.State;
			}
			for (int j = 0; j < this.m_images.Length; j++)
			{
				this.m_images[j].State = this.State;
			}
			for (int k = 0; k < this.m_windows.Length; k++)
			{
				this.m_windows[k].State = this.State;
			}
			for (int l = 0; l < this.m_text.Length; l++)
			{
				this.m_text[l].State = this.State;
			}
			this.UpdateTogglesInternal();
		}

		// Token: 0x04001D7F RID: 7551
		[SerializeField]
		private ToggleController.ToggleState m_initialState;

		// Token: 0x04001D80 RID: 7552
		[SerializeField]
		private GameObjectToggle[] m_gameObjects;

		// Token: 0x04001D81 RID: 7553
		[SerializeField]
		private ImageToggle[] m_images;

		// Token: 0x04001D82 RID: 7554
		[SerializeField]
		private WindowToggle[] m_windows;

		// Token: 0x04001D83 RID: 7555
		[SerializeField]
		private TextMeshProUGUIToggle[] m_text;

		// Token: 0x04001D84 RID: 7556
		private ToggleController.ToggleState m_state;

		// Token: 0x04001D85 RID: 7557
		private bool m_initialized;

		// Token: 0x020002EE RID: 750
		public enum ToggleState
		{
			// Token: 0x04001D87 RID: 7559
			DEFAULT,
			// Token: 0x04001D88 RID: 7560
			ON,
			// Token: 0x04001D89 RID: 7561
			OFF
		}
	}
}
