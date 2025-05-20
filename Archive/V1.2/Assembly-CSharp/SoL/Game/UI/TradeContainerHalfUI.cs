using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x020008E6 RID: 2278
	public class TradeContainerHalfUI : UniversalContainerUI
	{
		// Token: 0x140000C6 RID: 198
		// (add) Token: 0x060042AA RID: 17066 RVA: 0x0019333C File Offset: 0x0019153C
		// (remove) Token: 0x060042AB RID: 17067 RVA: 0x00193374 File Offset: 0x00191574
		public event Action ContentsChanged;

		// Token: 0x140000C7 RID: 199
		// (add) Token: 0x060042AC RID: 17068 RVA: 0x001933AC File Offset: 0x001915AC
		// (remove) Token: 0x060042AD RID: 17069 RVA: 0x001933E4 File Offset: 0x001915E4
		public event Action Initialized;

		// Token: 0x060042AE RID: 17070 RVA: 0x0006D044 File Offset: 0x0006B244
		protected override void InitializeSlots()
		{
			base.InitializeSlots();
			Action initialized = this.Initialized;
			if (initialized == null)
			{
				return;
			}
			initialized();
		}

		// Token: 0x060042AF RID: 17071 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void RightInstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x060042B0 RID: 17072 RVA: 0x0006D05C File Offset: 0x0006B25C
		public override void AddInstance(ArchetypeInstance instance)
		{
			base.AddInstance(instance);
			Action contentsChanged = this.ContentsChanged;
			if (contentsChanged == null)
			{
				return;
			}
			contentsChanged();
		}

		// Token: 0x060042B1 RID: 17073 RVA: 0x0006D075 File Offset: 0x0006B275
		public override void RemoveInstance(ArchetypeInstance instance)
		{
			base.RemoveInstance(instance);
			Action contentsChanged = this.ContentsChanged;
			if (contentsChanged == null)
			{
				return;
			}
			contentsChanged();
		}

		// Token: 0x060042B2 RID: 17074 RVA: 0x0006D08E File Offset: 0x0006B28E
		public override void ItemsSwapped()
		{
			base.ItemsSwapped();
			Action contentsChanged = this.ContentsChanged;
			if (contentsChanged == null)
			{
				return;
			}
			contentsChanged();
		}

		// Token: 0x060042B3 RID: 17075 RVA: 0x0006D0A6 File Offset: 0x0006B2A6
		protected override void ContainerOnCurrencyChanged(ulong obj)
		{
			base.ContainerOnCurrencyChanged(obj);
			Action contentsChanged = this.ContentsChanged;
			if (contentsChanged == null)
			{
				return;
			}
			contentsChanged();
		}

		// Token: 0x060042B4 RID: 17076 RVA: 0x0006D0BF File Offset: 0x0006B2BF
		public void UpdateHeaderText()
		{
			base.SetHeaderText();
		}
	}
}
