using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Game.Loot;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Loot
{
	// Token: 0x02000983 RID: 2435
	public class LootRollWindow : DraggableUIWindow
	{
		// Token: 0x06004898 RID: 18584 RVA: 0x001AA8E4 File Offset: 0x001A8AE4
		protected override void Awake()
		{
			base.Awake();
			for (int i = 0; i < this.m_lootRollitems.Length; i++)
			{
				this.m_lootRollitems[i].Controller = this;
			}
		}

		// Token: 0x06004899 RID: 18585 RVA: 0x001AA918 File Offset: 0x001A8B18
		private void Update()
		{
			if (!base.Visible || ClientGameManager.InputManager.PreventInput)
			{
				return;
			}
			if (SolInput.GetButtonDown(102))
			{
				this.TriggerExternalChoice(LootRollChoice.Pass);
				return;
			}
			if (SolInput.GetButtonDown(101))
			{
				this.TriggerExternalChoice(LootRollChoice.Greed);
				return;
			}
			if (SolInput.GetButtonDown(100))
			{
				this.TriggerExternalChoice(LootRollChoice.Need);
			}
		}

		// Token: 0x0600489A RID: 18586 RVA: 0x001AA96C File Offset: 0x001A8B6C
		private void TriggerExternalChoice(LootRollChoice choice)
		{
			LootRollItemUI firstItem = this.GetFirstItem(false);
			if (firstItem)
			{
				firstItem.ExternalChoice(choice);
			}
		}

		// Token: 0x0600489B RID: 18587 RVA: 0x001AA990 File Offset: 0x001A8B90
		public void UpdateLootRoll(BitBuffer buffer)
		{
			LootRollItem fromPool = StaticPool<LootRollItem>.GetFromPool();
			fromPool.ReadData(buffer);
			LootRollStatus status = fromPool.Status;
			if (status != LootRollStatus.Pending)
			{
				if (status == LootRollStatus.Complete)
				{
					for (int i = 0; i < this.m_lootRollitems.Length; i++)
					{
						if (this.m_lootRollitems[i].Occupied && this.m_lootRollitems[i].Id == fromPool.Id)
						{
							this.m_lootRollitems[i].Init(null);
							break;
						}
					}
					fromPool.PrintResults();
					StaticPool<LootRollItem>.ReturnToPool(fromPool);
				}
			}
			else
			{
				for (int j = 0; j < this.m_lootRollitems.Length; j++)
				{
					if (!this.m_lootRollitems[j].Occupied)
					{
						this.m_lootRollitems[j].Init(fromPool);
						this.UpdateWindowVisibility();
						this.RefreshIsFirst();
						return;
					}
				}
				this.m_queuedItems.Enqueue(fromPool);
				this.UpdateQueueLabel();
			}
			this.UpdateWindowVisibility();
			this.RefreshIsFirst();
		}

		// Token: 0x0600489C RID: 18588 RVA: 0x001AAA74 File Offset: 0x001A8C74
		public void RefreshFromQueue()
		{
			if (this.m_queuedItems.Count > 0)
			{
				for (int i = 0; i < this.m_lootRollitems.Length; i++)
				{
					if (!this.m_lootRollitems[i].Occupied)
					{
						this.m_lootRollitems[i].Init(this.m_queuedItems.Dequeue());
						break;
					}
				}
			}
			this.UpdateWindowVisibility();
			this.UpdateQueueLabel();
			this.RefreshIsFirst();
		}

		// Token: 0x0600489D RID: 18589 RVA: 0x001AAAE0 File Offset: 0x001A8CE0
		private void UpdateQueueLabel()
		{
			string text = (this.m_queuedItems.Count > 0) ? (this.m_queuedItems.Count.ToString() + " queued") : "";
			this.m_queueLabel.text = text;
		}

		// Token: 0x0600489E RID: 18590 RVA: 0x00070CE3 File Offset: 0x0006EEE3
		private void UpdateWindowVisibility()
		{
			if (this.RollIsActive() || this.m_queuedItems.Count > 0)
			{
				if (!base.Visible)
				{
					this.Show(false);
					return;
				}
			}
			else if (base.Visible)
			{
				this.Hide(false);
			}
		}

		// Token: 0x0600489F RID: 18591 RVA: 0x001AAB2C File Offset: 0x001A8D2C
		private bool RollIsActive()
		{
			for (int i = 0; i < this.m_lootRollitems.Length; i++)
			{
				if (this.m_lootRollitems[i].Occupied)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060048A0 RID: 18592 RVA: 0x001AAB60 File Offset: 0x001A8D60
		private void RefreshIsFirst()
		{
			LootRollItemUI firstItem = this.GetFirstItem(true);
			if (firstItem)
			{
				firstItem.IsFirst = true;
			}
		}

		// Token: 0x060048A1 RID: 18593 RVA: 0x001AAB84 File Offset: 0x001A8D84
		private LootRollItemUI GetFirstItem(bool resetIsFirst)
		{
			int num = int.MaxValue;
			LootRollItemUI result = null;
			for (int i = 0; i < this.m_lootRollitems.Length; i++)
			{
				if (resetIsFirst)
				{
					this.m_lootRollitems[i].IsFirst = false;
				}
				if (this.m_lootRollitems[i].Occupied && this.m_lootRollitems[i].transform)
				{
					int siblingIndex = this.m_lootRollitems[i].transform.GetSiblingIndex();
					if (siblingIndex < num)
					{
						num = siblingIndex;
						result = this.m_lootRollitems[i];
					}
				}
			}
			return result;
		}

		// Token: 0x040043D4 RID: 17364
		[SerializeField]
		private TextMeshProUGUI m_queueLabel;

		// Token: 0x040043D5 RID: 17365
		[SerializeField]
		private LootRollItemUI[] m_lootRollitems;

		// Token: 0x040043D6 RID: 17366
		private readonly Queue<LootRollItem> m_queuedItems = new Queue<LootRollItem>();
	}
}
