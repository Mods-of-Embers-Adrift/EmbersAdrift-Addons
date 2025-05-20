using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000880 RID: 2176
	public class EquippedRepairIcon : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000EA8 RID: 3752
		// (get) Token: 0x06003F65 RID: 16229 RVA: 0x0006ADF8 File Offset: 0x00068FF8
		// (set) Token: 0x06003F66 RID: 16230 RVA: 0x0006ADFF File Offset: 0x00068FFF
		public static bool PauseRefresh { get; set; }

		// Token: 0x06003F67 RID: 16231 RVA: 0x0006AE07 File Offset: 0x00069007
		private void Awake()
		{
			UIManager.EquippedRepairIcon = this;
		}

		// Token: 0x06003F68 RID: 16232 RVA: 0x0006AE0F File Offset: 0x0006900F
		private void Start()
		{
			if (LocalPlayer.IsInitialized)
			{
				this.Subscribe();
				this.RefreshRepairIcon();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x06003F69 RID: 16233 RVA: 0x0006AE36 File Offset: 0x00069036
		private void OnDestroy()
		{
			if (UIManager.EquippedRepairIcon == this)
			{
				UIManager.EquippedRepairIcon = null;
			}
			this.Unsubscribe();
		}

		// Token: 0x06003F6A RID: 16234 RVA: 0x0006AE51 File Offset: 0x00069051
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.Subscribe();
			this.RefreshRepairIcon();
		}

		// Token: 0x06003F6B RID: 16235 RVA: 0x00188B7C File Offset: 0x00186D7C
		private void Subscribe()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Equipment != null)
			{
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
			}
		}

		// Token: 0x06003F6C RID: 16236 RVA: 0x00188BD4 File Offset: 0x00186DD4
		private void Unsubscribe()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Equipment != null)
			{
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
			}
		}

		// Token: 0x06003F6D RID: 16237 RVA: 0x0006AE70 File Offset: 0x00069070
		private void EquipmentOnContentsChanged()
		{
			this.RefreshRepairIcon();
		}

		// Token: 0x06003F6E RID: 16238 RVA: 0x0006AE78 File Offset: 0x00069078
		public void ForceRefreshRepairIcon()
		{
			this.m_lastFrameUpdate = 0;
			this.RefreshRepairIcon();
		}

		// Token: 0x06003F6F RID: 16239 RVA: 0x00188C2C File Offset: 0x00186E2C
		public void RefreshRepairIcon()
		{
			if (EquippedRepairIcon.PauseRefresh || Time.frameCount <= this.m_lastFrameUpdate)
			{
				return;
			}
			EquipmentSlot equipmentSlot = EquipmentSlot.None;
			float num = 0f;
			int num2 = 0;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Equipment != null)
			{
				foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Equipment.Instances)
				{
					IDurability source;
					EquipmentSlot equipmentSlot2;
					if (archetypeInstance != null && archetypeInstance.Index != 65536 && archetypeInstance.ItemData != null && archetypeInstance.ItemData.Durability != null && archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out source) && EquipmentExtensions.IndexToSlotDict.TryGetValue(archetypeInstance.Index, out equipmentSlot2))
					{
						float durabilityMultiplierForRepair = source.GetDurabilityMultiplierForRepair(archetypeInstance);
						if (durabilityMultiplierForRepair < 1f)
						{
							equipmentSlot |= equipmentSlot2;
						}
						num += durabilityMultiplierForRepair;
						num2++;
					}
				}
			}
			num = ((num2 > 0) ? (num / (float)num2) : 0f);
			this.m_impactedSlots = equipmentSlot;
			this.m_lastFrameUpdate = Time.frameCount;
			if (this.m_impactedSlots == EquipmentSlot.None)
			{
				if (this.m_repairIcon && this.m_repairIcon.enabled)
				{
					this.m_repairIcon.enabled = false;
				}
				if (this.m_repairIconBorder && this.m_repairIconBorder.enabled)
				{
					this.m_repairIconBorder.enabled = false;
					return;
				}
			}
			else
			{
				if (this.m_repairIconBorder)
				{
					Color redColor = UIManager.RedColor;
					redColor.a = Mathf.Lerp(1f, 0.25f, num);
					this.m_repairIconBorder.color = redColor;
					if (!this.m_repairIconBorder.enabled)
					{
						this.m_repairIconBorder.enabled = true;
					}
				}
				if (this.m_repairIcon && !this.m_repairIcon.enabled)
				{
					this.m_repairIcon.enabled = true;
				}
			}
		}

		// Token: 0x06003F70 RID: 16240 RVA: 0x00188E4C File Offset: 0x0018704C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_impactedSlots == EquipmentSlot.None)
			{
				return null;
			}
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			for (int i = 0; i < EquipmentExtensions.EquipmentSlots.Length; i++)
			{
				if (EquipmentExtensions.EquipmentSlots[i] != EquipmentSlot.None && this.m_impactedSlots.HasBitFlag(EquipmentExtensions.EquipmentSlots[i]))
				{
					fromPool.Add(EquipmentExtensions.EquipmentSlots[i].GetSlotDescriptionForRepairTooltip());
				}
			}
			string txt = ZString.Format<string>("Equipment in need of repair:\n{0}", string.Join(", ", fromPool));
			StaticListPool<string>.ReturnToPool(fromPool);
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000EA9 RID: 3753
		// (get) Token: 0x06003F71 RID: 16241 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000EAA RID: 3754
		// (get) Token: 0x06003F72 RID: 16242 RVA: 0x0006AE87 File Offset: 0x00069087
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EAB RID: 3755
		// (get) Token: 0x06003F73 RID: 16243 RVA: 0x0006AE95 File Offset: 0x00069095
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003F75 RID: 16245 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D3D RID: 15677
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003D3E RID: 15678
		[SerializeField]
		private Image m_repairIcon;

		// Token: 0x04003D3F RID: 15679
		[SerializeField]
		private Image m_repairIconBorder;

		// Token: 0x04003D40 RID: 15680
		private int m_lastFrameUpdate = -1;

		// Token: 0x04003D41 RID: 15681
		private EquipmentSlot m_impactedSlots;
	}
}
