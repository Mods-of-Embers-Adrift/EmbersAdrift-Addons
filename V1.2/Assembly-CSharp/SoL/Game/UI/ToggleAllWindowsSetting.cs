using System;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008C7 RID: 2247
	[Serializable]
	public class ToggleAllWindowsSetting
	{
		// Token: 0x060041B9 RID: 16825 RVA: 0x00190500 File Offset: 0x0018E700
		public void Init()
		{
			if (this.m_window != ToggleAllWindowType.None && this.m_toggle)
			{
				ToggleAllWindowFlags value = (ToggleAllWindowFlags)Options.GameOptions.ToggleAllWindows.Value;
				ToggleAllWindowFlags flag = this.m_window.GetFlag();
				this.m_toggle.isOn = value.HasBitFlag(flag);
				this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			}
		}

		// Token: 0x060041BA RID: 16826 RVA: 0x0006C68A File Offset: 0x0006A88A
		public void Destroy()
		{
			if (this.m_window != ToggleAllWindowType.None && this.m_toggle)
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			}
		}

		// Token: 0x060041BB RID: 16827 RVA: 0x00190568 File Offset: 0x0018E768
		private void ToggleChanged(bool arg0)
		{
			ToggleAllWindowFlags flag = this.m_window.GetFlag();
			if (flag != ToggleAllWindowFlags.None)
			{
				ToggleAllWindowFlags toggleAllWindowFlags = (ToggleAllWindowFlags)Options.GameOptions.ToggleAllWindows.Value;
				if (this.m_toggle.isOn)
				{
					toggleAllWindowFlags |= flag;
				}
				else
				{
					toggleAllWindowFlags &= ~flag;
				}
				Options.GameOptions.ToggleAllWindows.Value = (int)toggleAllWindowFlags;
			}
		}

		// Token: 0x04003EFC RID: 16124
		[SerializeField]
		private ToggleAllWindowType m_window;

		// Token: 0x04003EFD RID: 16125
		[SerializeField]
		private SolToggle m_toggle;
	}
}
