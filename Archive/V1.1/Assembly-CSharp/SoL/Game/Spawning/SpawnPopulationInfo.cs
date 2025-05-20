using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006D2 RID: 1746
	public class SpawnPopulationInfo : MonoBehaviour
	{
		// Token: 0x060034FF RID: 13567 RVA: 0x0006449E File Offset: 0x0006269E
		private void CollectChildren()
		{
			this.m_controllers = base.gameObject.GetComponentsInChildren<SpawnController>();
		}

		// Token: 0x06003500 RID: 13568 RVA: 0x001668B8 File Offset: 0x00164AB8
		private string GetLongDescription()
		{
			int num = 0;
			if (this.m_controllers != null)
			{
				for (int i = 0; i < this.m_controllers.Length; i++)
				{
					if (this.m_controllers[i] != null && this.m_controllers[i].gameObject != null && this.m_controllers[i].gameObject.activeInHierarchy)
					{
						num += this.m_controllers[i].TargetPopulation;
					}
				}
			}
			return num.ToString() + " total population";
		}

		// Token: 0x06003501 RID: 13569 RVA: 0x000644B1 File Offset: 0x000626B1
		private string GetShortDescription()
		{
			return "Population Stats";
		}

		// Token: 0x0400332D RID: 13101
		[SerializeField]
		private SpawnController[] m_controllers;

		// Token: 0x0400332E RID: 13102
		[SerializeField]
		private DummyClass m_dummy;
	}
}
