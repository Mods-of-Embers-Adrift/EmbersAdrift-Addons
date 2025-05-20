using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009BF RID: 2495
	public class ArchetypeInstanceSymbolicLinkUI : MonoBehaviour, ITooltip, IInteractiveBase, IArchetypeDropZone
	{
		// Token: 0x06004B96 RID: 19350 RVA: 0x001B931C File Offset: 0x001B751C
		public void Initialize(ArchetypeInstance instance)
		{
			if (this.m_instance != null && this.m_countSubscribed && this.m_instance.ItemData != null)
			{
				this.m_instance.ItemData.CountChanged -= this.RefreshCountLabel;
			}
			this.m_instance = instance;
			if (!this.m_rectTransform)
			{
				return;
			}
			if (this.m_instance == null || this.m_instance.SymbolicLink == null)
			{
				base.gameObject.SetActive(false);
				this.m_rectTransform.SetParent(null);
				this.m_dropZone = null;
				this.m_countSubscribed = false;
				return;
			}
			if (this.m_image && this.m_instance.Archetype)
			{
				this.m_image.overrideSprite = this.m_instance.Archetype.Icon;
				this.m_image.color = this.m_instance.Archetype.IconTint;
			}
			if (this.m_countLabel)
			{
				if (this.m_instance.Archetype && this.m_instance.Archetype.ArchetypeHasCount() && this.m_instance.ItemData != null && this.m_instance.ItemData.Count != null)
				{
					this.m_instance.ItemData.CountChanged += this.RefreshCountLabel;
					this.m_countSubscribed = true;
					this.RefreshCountLabel();
					this.m_countLabel.enabled = true;
				}
				else
				{
					this.m_countLabel.enabled = false;
				}
			}
			ContainerSlotUI containerSlotUI;
			if (instance.SymbolicLink.PreviousContainer != null && instance.SymbolicLink.PreviousContainer.ContainerUI != null && instance.SymbolicLink.PreviousContainer.ContainerUI.TryGetContainerSlotUI(this.m_instance.SymbolicLink.PreviousIndex, out containerSlotUI))
			{
				this.m_dropZone = containerSlotUI;
				this.m_rectTransform.SetParent(containerSlotUI.RectTransform);
				this.m_rectTransform.anchoredPosition = Vector2.zero;
				this.m_rectTransform.localScale = Vector3.one;
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x001B9534 File Offset: 0x001B7734
		private void RefreshCountLabel()
		{
			if (this.m_countLabel && this.m_instance != null && this.m_instance.ItemData != null && this.m_instance.ItemData.Count != null)
			{
				int value = this.m_instance.ItemData.Count.Value;
				this.m_countLabel.ZStringSetText(value.ToString());
			}
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x001B95A8 File Offset: 0x001B77A8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instance != null)
			{
				return new ArchetypeTooltipParameter
				{
					Instance = this.m_instance,
					AdditionalText = ZString.Format<ContainerType>("<i>Currently resides in {0}", this.m_instance.ContainerInstance.ContainerType)
				};
			}
			return null;
		}

		// Token: 0x1700109C RID: 4252
		// (get) Token: 0x06004B99 RID: 19353 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700109D RID: 4253
		// (get) Token: 0x06004B9A RID: 19354 RVA: 0x000732D9 File Offset: 0x000714D9
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700109E RID: 4254
		// (get) Token: 0x06004B9B RID: 19355 RVA: 0x000732E7 File Offset: 0x000714E7
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x1700109F RID: 4255
		// (get) Token: 0x06004B9C RID: 19356 RVA: 0x000732EF File Offset: 0x000714EF
		GameObject IArchetypeDropZone.GO
		{
			get
			{
				if (this.m_dropZone == null)
				{
					return null;
				}
				return this.m_dropZone.GO;
			}
		}

		// Token: 0x170010A0 RID: 4256
		// (get) Token: 0x06004B9D RID: 19357 RVA: 0x00073306 File Offset: 0x00071506
		int IArchetypeDropZone.Index
		{
			get
			{
				if (this.m_dropZone == null)
				{
					return -1;
				}
				return this.m_dropZone.Index;
			}
		}

		// Token: 0x170010A1 RID: 4257
		// (get) Token: 0x06004B9E RID: 19358 RVA: 0x0007331D File Offset: 0x0007151D
		ArchetypeInstance IArchetypeDropZone.CurrentOccupant
		{
			get
			{
				if (this.m_dropZone == null)
				{
					return null;
				}
				return this.m_dropZone.CurrentOccupant;
			}
		}

		// Token: 0x170010A2 RID: 4258
		// (get) Token: 0x06004B9F RID: 19359 RVA: 0x00073334 File Offset: 0x00071534
		IContainerUI IArchetypeDropZone.ContainerUI
		{
			get
			{
				if (this.m_dropZone == null)
				{
					return null;
				}
				return this.m_dropZone.ContainerUI;
			}
		}

		// Token: 0x06004BA0 RID: 19360 RVA: 0x0007334B File Offset: 0x0007154B
		bool IArchetypeDropZone.CanPlace(ArchetypeInstance instance, int targetIndex)
		{
			return this.m_dropZone != null && this.m_dropZone.CanPlace(instance, targetIndex);
		}

		// Token: 0x06004BA2 RID: 19362 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040045F5 RID: 17909
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x040045F6 RID: 17910
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040045F7 RID: 17911
		[SerializeField]
		private Image m_image;

		// Token: 0x040045F8 RID: 17912
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x040045F9 RID: 17913
		private ArchetypeInstance m_instance;

		// Token: 0x040045FA RID: 17914
		private IArchetypeDropZone m_dropZone;

		// Token: 0x040045FB RID: 17915
		private bool m_countSubscribed;
	}
}
