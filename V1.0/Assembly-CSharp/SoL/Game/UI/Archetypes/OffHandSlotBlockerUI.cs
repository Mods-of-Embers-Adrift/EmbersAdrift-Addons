using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009D1 RID: 2513
	public class OffHandSlotBlockerUI : MonoBehaviour, IArchetypeDropZone, ITooltip, IInteractiveBase
	{
		// Token: 0x06004C7D RID: 19581 RVA: 0x001BCEC8 File Offset: 0x001BB0C8
		public void Refresh(ArchetypeInstance instance, bool isEnabled)
		{
			if (isEnabled)
			{
				this.m_instance = instance;
				this.m_icon.overrideSprite = instance.Archetype.Icon;
				base.gameObject.SetActive(true);
				return;
			}
			this.m_instance = null;
			base.gameObject.SetActive(false);
			this.m_icon.overrideSprite = null;
		}

		// Token: 0x170010E3 RID: 4323
		// (get) Token: 0x06004C7E RID: 19582 RVA: 0x00073BF0 File Offset: 0x00071DF0
		GameObject IArchetypeDropZone.GO
		{
			get
			{
				ArchetypeInstance instance = this.m_instance;
				if (instance == null)
				{
					return null;
				}
				return instance.InstanceUI.gameObject;
			}
		}

		// Token: 0x170010E4 RID: 4324
		// (get) Token: 0x06004C7F RID: 19583 RVA: 0x00073C08 File Offset: 0x00071E08
		int IArchetypeDropZone.Index
		{
			get
			{
				if (this.m_instance == null)
				{
					return -1;
				}
				return this.m_instance.Index;
			}
		}

		// Token: 0x170010E5 RID: 4325
		// (get) Token: 0x06004C80 RID: 19584 RVA: 0x00073C1F File Offset: 0x00071E1F
		ArchetypeInstance IArchetypeDropZone.CurrentOccupant
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x170010E6 RID: 4326
		// (get) Token: 0x06004C81 RID: 19585 RVA: 0x00073C27 File Offset: 0x00071E27
		IContainerUI IArchetypeDropZone.ContainerUI
		{
			get
			{
				ArchetypeInstance instance = this.m_instance;
				if (instance == null)
				{
					return null;
				}
				ContainerInstance containerInstance = instance.ContainerInstance;
				if (containerInstance == null)
				{
					return null;
				}
				return containerInstance.ContainerUI;
			}
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IArchetypeDropZone.CanPlace(ArchetypeInstance instance, int targetIndex)
		{
			return false;
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x001BCF24 File Offset: 0x001BB124
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instance == null)
			{
				return null;
			}
			string additionalText = "<i>In Main Hand</i>";
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instance,
				AdditionalText = additionalText
			};
		}

		// Token: 0x170010E7 RID: 4327
		// (get) Token: 0x06004C84 RID: 19588 RVA: 0x00073C45 File Offset: 0x00071E45
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170010E8 RID: 4328
		// (get) Token: 0x06004C85 RID: 19589 RVA: 0x001BA818 File Offset: 0x001B8A18
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return default(TooltipSettings);
			}
		}

		// Token: 0x170010E9 RID: 4329
		// (get) Token: 0x06004C86 RID: 19590 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004C88 RID: 19592 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004669 RID: 18025
		[SerializeField]
		private Image m_icon;

		// Token: 0x0400466A RID: 18026
		private ArchetypeInstance m_instance;
	}
}
