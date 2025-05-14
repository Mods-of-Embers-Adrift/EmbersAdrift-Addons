using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200091E RID: 2334
	public class AbilityGrouping : MonoBehaviour
	{
		// Token: 0x060044BA RID: 17594 RVA: 0x0019D8A0 File Offset: 0x0019BAA0
		private void Awake()
		{
			for (int i = 0; i < this.m_slots.Length; i++)
			{
				this.m_slots[i].Column = this;
			}
		}

		// Token: 0x060044BB RID: 17595 RVA: 0x0019D8D0 File Offset: 0x0019BAD0
		public void TriggerLockRefresh(AbilitySlot sourceSlot)
		{
			for (int i = 0; i < this.m_slots.Length; i++)
			{
				if (this.m_slots[i] != sourceSlot)
				{
					this.m_slots[i].RefreshLock();
				}
			}
		}

		// Token: 0x17000F67 RID: 3943
		// (get) Token: 0x060044BC RID: 17596 RVA: 0x0006E738 File Offset: 0x0006C938
		public IEnumerable<AbilitySlot> Slots
		{
			get
			{
				if (this.m_slotsEnumerable == null)
				{
					this.m_slotsEnumerable = this.GetInstances();
				}
				return this.m_slotsEnumerable;
			}
		}

		// Token: 0x060044BD RID: 17597 RVA: 0x0006E754 File Offset: 0x0006C954
		private IEnumerable<AbilitySlot> GetInstances()
		{
			int num;
			for (int i = 0; i < this.m_slots.Length; i = num + 1)
			{
				yield return this.m_slots[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x04004156 RID: 16726
		[SerializeField]
		protected AbilitySlot[] m_slots;

		// Token: 0x04004157 RID: 16727
		private IEnumerable<AbilitySlot> m_slotsEnumerable;
	}
}
