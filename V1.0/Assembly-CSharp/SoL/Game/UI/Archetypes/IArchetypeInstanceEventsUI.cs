using System;
using UnityEngine.EventSystems;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009CE RID: 2510
	internal interface IArchetypeInstanceEventsUI
	{
		// Token: 0x170010DD RID: 4317
		// (get) Token: 0x06004C64 RID: 19556
		bool CanPlace { get; }

		// Token: 0x170010DE RID: 4318
		// (get) Token: 0x06004C65 RID: 19557
		bool CanModify { get; }

		// Token: 0x170010DF RID: 4319
		// (get) Token: 0x06004C66 RID: 19558
		bool AuraIsActive { get; }

		// Token: 0x170010E0 RID: 4320
		// (get) Token: 0x06004C67 RID: 19559
		bool ContextualDisabledPanel { get; }

		// Token: 0x06004C68 RID: 19560
		void Init(ArchetypeInstanceUI instanceUI);

		// Token: 0x06004C69 RID: 19561
		void ExternalOnDestroy();

		// Token: 0x06004C6A RID: 19562
		void RefreshDisabledPanel();

		// Token: 0x06004C6B RID: 19563
		bool CooldownsActive();

		// Token: 0x06004C6C RID: 19564
		bool OverrideOnPointerUp(PointerEventData eventData);

		// Token: 0x06004C6D RID: 19565
		bool OverrideFillActionsGetTitle(out string result);
	}
}
