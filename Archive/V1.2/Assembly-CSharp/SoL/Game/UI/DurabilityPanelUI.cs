using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000877 RID: 2167
	public class DurabilityPanelUI : MonoBehaviour
	{
		// Token: 0x06003EFE RID: 16126 RVA: 0x0006A9DD File Offset: 0x00068BDD
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x06003EFF RID: 16127 RVA: 0x00186CBC File Offset: 0x00184EBC
		public void Subscribe(ArchetypeInstance instance)
		{
			this.Unsubscribe();
			this.m_durabilityCache = null;
			bool flag;
			if (instance == null)
			{
				flag = (null != null);
			}
			else
			{
				ItemInstanceData itemData = instance.ItemData;
				flag = (((itemData != null) ? itemData.Durability : null) != null);
			}
			if (flag && instance.Archetype.TryGetAsType(out this.m_durabilityCache))
			{
				this.m_instance = instance;
				this.m_instance.ItemData.Durability.DurabilityChanged += this.DurabilityOnDurabilityChanged;
				this.DurabilityOnDurabilityChanged();
				this.m_subscribed = true;
			}
			this.ToggleObject();
		}

		// Token: 0x06003F00 RID: 16128 RVA: 0x00186D40 File Offset: 0x00184F40
		public void Unsubscribe()
		{
			if (this.m_subscribed)
			{
				ArchetypeInstance instance = this.m_instance;
				bool flag;
				if (instance == null)
				{
					flag = (null != null);
				}
				else
				{
					ItemInstanceData itemData = instance.ItemData;
					flag = (((itemData != null) ? itemData.Durability : null) != null);
				}
				if (flag && this.m_durabilityCache != null)
				{
					this.m_instance.ItemData.Durability.DurabilityChanged -= this.DurabilityOnDurabilityChanged;
				}
			}
			this.m_instance = null;
			this.m_durabilityCache = null;
			this.m_subscribed = false;
		}

		// Token: 0x06003F01 RID: 16129 RVA: 0x00186DB4 File Offset: 0x00184FB4
		private void DurabilityOnDurabilityChanged()
		{
			if (this.m_durabilityCache != null)
			{
				ArchetypeInstance instance = this.m_instance;
				bool flag;
				if (instance == null)
				{
					flag = (null != null);
				}
				else
				{
					ItemInstanceData itemData = instance.ItemData;
					flag = (((itemData != null) ? itemData.Durability : null) != null);
				}
				if (flag)
				{
					int absorbed = this.m_instance.ItemData.Durability.Absorbed;
					float currentDurability = this.m_durabilityCache.GetCurrentDurability((float)absorbed);
					this.m_durabilityStatusBar.fillAmount = currentDurability;
					if (this.m_label)
					{
						this.m_label.text = currentDurability.GetAsPercentage() + "%";
					}
					float durabilityMultiplierForRepair = this.m_durabilityCache.GetDurabilityMultiplierForRepair(this.m_instance);
					if (durabilityMultiplierForRepair >= 1f)
					{
						if (this.m_repairIcon.enabled)
						{
							this.m_repairIcon.enabled = false;
						}
						if (this.m_repairIconBorder.enabled)
						{
							this.m_repairIconBorder.enabled = false;
						}
					}
					else
					{
						if (this.m_repairIconBorder)
						{
							Color redColor = UIManager.RedColor;
							redColor.a = Mathf.Lerp(1f, 0.25f, durabilityMultiplierForRepair);
							this.m_repairIconBorder.color = redColor;
							if (!this.m_repairIconBorder.enabled)
							{
								this.m_repairIconBorder.enabled = true;
							}
						}
						if (!this.m_repairIcon.enabled)
						{
							this.m_repairIcon.enabled = true;
						}
					}
					if (UIManager.EquippedRepairIcon && this.m_instance.ContainerInstance != null && this.m_instance.ContainerInstance.ContainerType == ContainerType.Equipment)
					{
						UIManager.EquippedRepairIcon.RefreshRepairIcon();
					}
				}
			}
		}

		// Token: 0x06003F02 RID: 16130 RVA: 0x00186F34 File Offset: 0x00185134
		public void SetManual(ArchetypeInstance instance)
		{
			if (instance == null)
			{
				this.m_instance = null;
				this.m_durabilityCache = null;
				this.ToggleObject();
				return;
			}
			if (instance == this.m_instance)
			{
				this.DurabilityOnDurabilityChanged();
				return;
			}
			if (instance.IsItem && instance.Archetype.TryGetAsType(out this.m_durabilityCache))
			{
				this.m_instance = instance;
				this.DurabilityOnDurabilityChanged();
			}
			else
			{
				this.m_instance = null;
				this.m_durabilityCache = null;
			}
			this.ToggleObject();
		}

		// Token: 0x06003F03 RID: 16131 RVA: 0x0006A9E5 File Offset: 0x00068BE5
		private void ToggleObject()
		{
			((this.m_objectToDisable == null) ? base.gameObject : this.m_objectToDisable).SetActive(this.m_durabilityCache != null);
		}

		// Token: 0x04003CF2 RID: 15602
		[SerializeField]
		private Image m_durabilityStatusBar;

		// Token: 0x04003CF3 RID: 15603
		[SerializeField]
		private Image m_repairIcon;

		// Token: 0x04003CF4 RID: 15604
		[SerializeField]
		private Image m_repairIconBorder;

		// Token: 0x04003CF5 RID: 15605
		[SerializeField]
		private CanvasGroup m_repairCanvasGroup;

		// Token: 0x04003CF6 RID: 15606
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003CF7 RID: 15607
		[SerializeField]
		private GameObject m_objectToDisable;

		// Token: 0x04003CF8 RID: 15608
		private ArchetypeInstance m_instance;

		// Token: 0x04003CF9 RID: 15609
		private IDurability m_durabilityCache;

		// Token: 0x04003CFA RID: 15610
		private bool m_subscribed;
	}
}
