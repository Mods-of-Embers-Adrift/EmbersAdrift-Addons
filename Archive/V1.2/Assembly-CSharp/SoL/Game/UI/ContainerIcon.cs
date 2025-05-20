using System;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200086B RID: 2155
	public class ContainerIcon : MonoBehaviour
	{
		// Token: 0x06003E58 RID: 15960 RVA: 0x0006A2BC File Offset: 0x000684BC
		private void Start()
		{
			if (this.m_autoInit)
			{
				this.Init(this.m_containerType);
			}
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x0018518C File Offset: 0x0018338C
		public void Init(ContainerType containerType)
		{
			this.m_containerType = containerType;
			if (this.m_icon)
			{
				Sprite overrideSprite;
				if (GlobalSettings.Values && GlobalSettings.Values.UI != null && GlobalSettings.Values.UI.TryGetContainerIcon(this.m_containerType, out overrideSprite))
				{
					this.m_icon.enabled = true;
					this.m_icon.overrideSprite = overrideSprite;
					return;
				}
				this.m_icon.enabled = false;
			}
		}

		// Token: 0x04003C8F RID: 15503
		[SerializeField]
		private bool m_autoInit;

		// Token: 0x04003C90 RID: 15504
		[SerializeField]
		private ContainerType m_containerType;

		// Token: 0x04003C91 RID: 15505
		[SerializeField]
		private Image m_icon;
	}
}
