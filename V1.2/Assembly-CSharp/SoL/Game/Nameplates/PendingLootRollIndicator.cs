using System;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Nameplates
{
	// Token: 0x020009DB RID: 2523
	public class PendingLootRollIndicator : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004CBD RID: 19645 RVA: 0x001BDA00 File Offset: 0x001BBC00
		public void RefreshIndicator(NameplateControllerUI controller)
		{
			if (!controller.AllowPendingLootRollIndicator() || controller.Targetable == null || controller.Targetable.Entity == null || controller.Targetable.Entity.Interactive == null)
			{
				this.m_parent.SetActive(false);
				return;
			}
			ILootRollSource lootRollSource = controller.Targetable.Entity.Interactive as ILootRollSource;
			if (lootRollSource != null)
			{
				this.m_parent.SetActive(lootRollSource.LootRollIsPending);
				return;
			}
			this.m_parent.SetActive(false);
		}

		// Token: 0x06004CBE RID: 19646 RVA: 0x00073ED2 File Offset: 0x000720D2
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Loot roll pending", false);
		}

		// Token: 0x170010F1 RID: 4337
		// (get) Token: 0x06004CBF RID: 19647 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170010F2 RID: 4338
		// (get) Token: 0x06004CC0 RID: 19648 RVA: 0x00073EE5 File Offset: 0x000720E5
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170010F3 RID: 4339
		// (get) Token: 0x06004CC1 RID: 19649 RVA: 0x00073EF3 File Offset: 0x000720F3
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004CC3 RID: 19651 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004691 RID: 18065
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004692 RID: 18066
		[SerializeField]
		private GameObject m_parent;
	}
}
