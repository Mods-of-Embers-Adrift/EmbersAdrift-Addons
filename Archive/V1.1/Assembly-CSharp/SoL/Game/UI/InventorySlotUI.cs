using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200089E RID: 2206
	public class InventorySlotUI : ContainerSlotUI, IBindingLabel
	{
		// Token: 0x06004043 RID: 16451 RVA: 0x0018BCD8 File Offset: 0x00189ED8
		private void OnDestroy()
		{
			if (this.m_bindingType != null)
			{
				BindingLabels.DeregisterBinding(this);
				this.m_bindingType = null;
			}
			if (this.m_toggle)
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			}
		}

		// Token: 0x06004044 RID: 16452 RVA: 0x0018BD30 File Offset: 0x00189F30
		public override void Initialize(IContainerUI containerUI, int index)
		{
			base.Initialize(containerUI, index);
			if (containerUI != null && containerUI.ContainerInstance != null)
			{
				ContainerType containerType = containerUI.ContainerInstance.ContainerType;
				if (containerType - ContainerType.Pouch <= 1)
				{
					this.m_actionBarIndex = index;
					this.m_bindingType = new BindingType?(this.GetBindingType());
					BindingLabels.RegisterBinding(this, false);
				}
			}
			if (this.m_toggle)
			{
				if (containerUI != null && containerUI.ContainerInstance != null && containerUI.ContainerInstance.ContainerType == ContainerType.ReagentPouch)
				{
					this.m_toggle.isOn = containerUI.ContainerInstance.GetToggle(this.m_index);
					this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
					this.m_toggle.gameObject.SetActive(true);
					return;
				}
				this.m_toggle.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004045 RID: 16453 RVA: 0x0006B85F File Offset: 0x00069A5F
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			base.InstanceAdded(instance);
			this.m_emblem.CrossFadeAlpha(this.m_alphaRange.x, 0.1f, true);
		}

		// Token: 0x06004046 RID: 16454 RVA: 0x0006B884 File Offset: 0x00069A84
		public override void InstanceRemoved(ArchetypeInstance instance)
		{
			base.InstanceRemoved(instance);
			this.m_emblem.CrossFadeAlpha(this.m_alphaRange.y, 0.1f, true);
		}

		// Token: 0x06004047 RID: 16455 RVA: 0x0006B8A9 File Offset: 0x00069AA9
		public void ToggleReagentToggle()
		{
			if (this.m_toggle)
			{
				this.m_toggle.isOn = !this.m_toggle.isOn;
			}
		}

		// Token: 0x06004048 RID: 16456 RVA: 0x0018BE04 File Offset: 0x0018A004
		private void ToggleChanged(bool value)
		{
			if (this.m_containerUI != null && this.m_containerUI.ContainerInstance != null && this.m_containerUI.ContainerInstance.ContainerType == ContainerType.ReagentPouch && LocalPlayer.NetworkEntity != null && LocalPlayer.NetworkEntity.PlayerRpcHandler != null)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.ToggleReagent(this.m_index, this.m_toggle.isOn);
				this.m_containerUI.ContainerInstance.SetToggle(this.m_index, this.m_toggle.isOn);
			}
		}

		// Token: 0x06004049 RID: 16457 RVA: 0x0018BE9C File Offset: 0x0018A09C
		private BindingType GetBindingType()
		{
			if (this.m_containerUI != null && this.m_containerUI.ContainerInstance != null)
			{
				ContainerType containerType = this.m_containerUI.ContainerInstance.ContainerType;
				if (containerType == ContainerType.Pouch)
				{
					return BindingType.Consumable;
				}
				if (containerType == ContainerType.ReagentPouch)
				{
					return BindingType.Reagent;
				}
			}
			return BindingType.None;
		}

		// Token: 0x17000EC3 RID: 3779
		// (get) Token: 0x0600404A RID: 16458 RVA: 0x0006B8D1 File Offset: 0x00069AD1
		BindingType IBindingLabel.Type
		{
			get
			{
				return this.GetBindingType();
			}
		}

		// Token: 0x17000EC4 RID: 3780
		// (get) Token: 0x0600404B RID: 16459 RVA: 0x0006B8D9 File Offset: 0x00069AD9
		int IBindingLabel.Index
		{
			get
			{
				return this.m_actionBarIndex;
			}
		}

		// Token: 0x17000EC5 RID: 3781
		// (get) Token: 0x0600404C RID: 16460 RVA: 0x0006B8E1 File Offset: 0x00069AE1
		TextMeshProUGUI IBindingLabel.Label
		{
			get
			{
				return this.m_label;
			}
		}

		// Token: 0x17000EC6 RID: 3782
		// (get) Token: 0x0600404D RID: 16461 RVA: 0x00049FFA File Offset: 0x000481FA
		string IBindingLabel.FormattedString
		{
			get
			{
				return null;
			}
		}

		// Token: 0x04003E20 RID: 15904
		[SerializeField]
		private Image m_emblem;

		// Token: 0x04003E21 RID: 15905
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003E22 RID: 15906
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04003E23 RID: 15907
		private int m_actionBarIndex = -1;

		// Token: 0x04003E24 RID: 15908
		private BindingType? m_bindingType;
	}
}
