using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x020008E8 RID: 2280
	public interface IContainerUI
	{
		// Token: 0x17000F2C RID: 3884
		// (get) Token: 0x060042CC RID: 17100
		string Id { get; }

		// Token: 0x17000F2D RID: 3885
		// (get) Token: 0x060042CD RID: 17101
		bool Locked { get; }

		// Token: 0x060042CE RID: 17102
		bool IsLockedWithNotification();

		// Token: 0x17000F2E RID: 3886
		// (get) Token: 0x060042CF RID: 17103
		bool Visible { get; }

		// Token: 0x060042D0 RID: 17104
		void AddInstance(ArchetypeInstance instance);

		// Token: 0x060042D1 RID: 17105
		void RemoveInstance(ArchetypeInstance instance);

		// Token: 0x060042D2 RID: 17106
		void ItemsSwapped();

		// Token: 0x060042D3 RID: 17107
		void Initialize(ContainerInstance containerInstance);

		// Token: 0x060042D4 RID: 17108
		void PostInit();

		// Token: 0x060042D5 RID: 17109
		void InstanceClicked(PointerEventData eventData, ArchetypeInstance instance);

		// Token: 0x17000F2F RID: 3887
		// (get) Token: 0x060042D6 RID: 17110
		ContainerInstance ContainerInstance { get; }

		// Token: 0x060042D7 RID: 17111
		void Hide();

		// Token: 0x060042D8 RID: 17112
		void Show();

		// Token: 0x060042D9 RID: 17113
		void CloseButtonPressed();

		// Token: 0x060042DA RID: 17114
		void InteractWithInstance(ArchetypeInstance instance);

		// Token: 0x060042DB RID: 17115
		bool TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI);
	}
}
