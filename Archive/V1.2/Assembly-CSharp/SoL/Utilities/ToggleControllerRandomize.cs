using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002EF RID: 751
	public class ToggleControllerRandomize : MonoBehaviour
	{
		// Token: 0x06001561 RID: 5473 RVA: 0x000FC70C File Offset: 0x000FA90C
		[ContextMenu("Randomize")]
		public void Randomize()
		{
			if (this.m_controllers != null && this.m_controllers.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, this.m_controllers.Length);
				for (int i = 0; i < this.m_controllers.Length; i++)
				{
					this.m_controllers[i].Toggle(i == num);
				}
			}
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x000FC760 File Offset: 0x000FA960
		[ContextMenu("Next")]
		public void Next()
		{
			if (this.m_controllers != null && this.m_controllers.Length != 0)
			{
				int num = 0;
				int num2 = -1;
				for (int i = 0; i < this.m_controllers.Length; i++)
				{
					if (this.m_controllers[i].State == ToggleController.ToggleState.ON)
					{
						num++;
						num2 = i;
					}
				}
				if (num > 1 || num2 == -1)
				{
					this.Randomize();
					this.Next();
					return;
				}
				int num3;
				if (num2 == this.m_controllers.Length - 1)
				{
					num3 = 0;
				}
				else
				{
					num3 = num2 + 1;
				}
				if (num3 != -1)
				{
					for (int j = 0; j < this.m_controllers.Length; j++)
					{
						this.m_controllers[j].Toggle(j == num3);
					}
				}
			}
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x000510B8 File Offset: 0x0004F2B8
		[ContextMenu("Enable All")]
		private void EnableAll()
		{
			this.ToggleAll(true);
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x000510C1 File Offset: 0x0004F2C1
		[ContextMenu("Disable All")]
		private void DisableAll()
		{
			this.ToggleAll(false);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x000FC80C File Offset: 0x000FAA0C
		private void ToggleAll(bool isOn)
		{
			if (this.m_controllers != null && this.m_controllers.Length != 0)
			{
				for (int i = 0; i < this.m_controllers.Length; i++)
				{
					this.m_controllers[i].Toggle(isOn);
				}
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x000510CA File Offset: 0x0004F2CA
		[ContextMenu("Get Toggles")]
		private void GetToggleControllers()
		{
			this.m_controllers = base.gameObject.GetComponentsInChildren<ToggleController>();
		}

		// Token: 0x04001D8A RID: 7562
		[SerializeField]
		private ToggleController[] m_controllers;
	}
}
