using System;
using Cysharp.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x0200087C RID: 2172
	public class EquipmentSlotUI : ContainerSlotUI
	{
		// Token: 0x140000BC RID: 188
		// (add) Token: 0x06003F39 RID: 16185 RVA: 0x00187FF4 File Offset: 0x001861F4
		// (remove) Token: 0x06003F3A RID: 16186 RVA: 0x00188028 File Offset: 0x00186228
		public static event Action HandEquipmentUpdated;

		// Token: 0x17000E9E RID: 3742
		// (get) Token: 0x06003F3B RID: 16187 RVA: 0x0006AC6F File Offset: 0x00068E6F
		public EquipmentSlot Slot
		{
			get
			{
				return this.m_slot;
			}
		}

		// Token: 0x17000E9F RID: 3743
		// (get) Token: 0x06003F3C RID: 16188 RVA: 0x0018805C File Offset: 0x0018625C
		private Image IconToUse
		{
			get
			{
				EquipmentSlot slot = this.m_slot;
				if (slot <= EquipmentSlot.Ear_Right)
				{
					if (slot != EquipmentSlot.Ear_Left && slot != EquipmentSlot.Ear_Right)
					{
						goto IL_38;
					}
				}
				else if (slot != EquipmentSlot.Finger_Left && slot != EquipmentSlot.Finger_Right)
				{
					goto IL_38;
				}
				return this.m_smallIcon;
				IL_38:
				return this.m_icon;
			}
		}

		// Token: 0x06003F3D RID: 16189 RVA: 0x0004475B File Offset: 0x0004295B
		private void Awake()
		{
		}

		// Token: 0x06003F3E RID: 16190 RVA: 0x001880A8 File Offset: 0x001862A8
		public void AssignSettings(EquipmentSlotSetting setting, Color defaultBorderColor, Color armorBorderColor, bool triggerHandEquipmentEvent = true)
		{
			this.m_slot = setting.Type;
			this.m_icon.enabled = false;
			this.m_smallIcon.enabled = false;
			this.IconToUse.enabled = true;
			this.m_border.color = (this.m_slot.HasArmorCost() ? armorBorderColor : defaultBorderColor);
			base.gameObject.name = this.m_slot.ToString();
			this.IconToUse.sprite = setting.Icon;
			this.IconToUse.rectTransform.rotation = Quaternion.Euler(setting.Rotation);
			this.m_highlight.enabled = true;
			this.m_triggerHandEquipmentEvent = triggerHandEquipmentEvent;
		}

		// Token: 0x06003F3F RID: 16191 RVA: 0x00188160 File Offset: 0x00186360
		public override void InstanceAdded(ArchetypeInstance instance)
		{
			if (this.m_triggerHandEquipmentEvent && this.m_slot.TriggerWeaponSwapCooldownOnChange() && instance != this.m_instance)
			{
				Action handEquipmentUpdated = EquipmentSlotUI.HandEquipmentUpdated;
				if (handEquipmentUpdated != null)
				{
					handEquipmentUpdated();
				}
			}
			base.InstanceAdded(instance);
			this.IconToUse.CrossFadeAlpha(this.m_alphaRange.x, 0.1f, true);
		}

		// Token: 0x06003F40 RID: 16192 RVA: 0x0006AC77 File Offset: 0x00068E77
		public override void InstanceRemoved(ArchetypeInstance instance)
		{
			base.InstanceRemoved(instance);
			this.IconToUse.CrossFadeAlpha(this.m_alphaRange.y, 0.1f, true);
		}

		// Token: 0x06003F41 RID: 16193 RVA: 0x001881C0 File Offset: 0x001863C0
		protected override ITooltipParameter GetTooltipParameter()
		{
			string txt = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendLine(this.m_slot.GetDisplayName());
				if (this.m_slot == EquipmentSlot.Cosmetic)
				{
					utf16ValueStringBuilder.AppendLine("Replaces the visuals of a visible slot.");
					utf16ValueStringBuilder.AppendLine("<i>Does not contribute stats or armor class.</i>");
					utf16ValueStringBuilder.AppendLine();
					utf16ValueStringBuilder.AppendLine(ZString.Format<string>("<size=80%>Compatible Equipment: {0}</size>", EquipmentExtensions.GetCommaSeparatedListOfCosmeticSlots()));
					if (this.m_subscriberOnly && SessionData.User != null && !SessionData.User.IsSubscriber())
					{
						utf16ValueStringBuilder.AppendLine();
						utf16ValueStringBuilder.AppendLine();
						utf16ValueStringBuilder.AppendLine("This slot is reserved for subscribers.");
					}
				}
				else if (this.m_slot.HasArmorCost())
				{
					MinMaxIntRange? rangeForSlot = GlobalSettings.Values.Armor.GetRangeForSlot(this.m_slot);
					if (rangeForSlot != null)
					{
						utf16ValueStringBuilder.AppendFormat<int, int>("{0}-{1} Armor Weight", rangeForSlot.Value.Min, rangeForSlot.Value.Max);
						utf16ValueStringBuilder.AppendLine();
					}
				}
				else if (this.m_slot.ShowArmorWeightOnTooltip())
				{
					utf16ValueStringBuilder.AppendLine("0 Armor Weight");
				}
				txt = utf16ValueStringBuilder.ToString();
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x04003D22 RID: 15650
		[SerializeField]
		protected Image m_smallIcon;

		// Token: 0x04003D23 RID: 15651
		[SerializeField]
		private Image m_border;

		// Token: 0x04003D24 RID: 15652
		[SerializeField]
		private EquipmentSlot m_slot;

		// Token: 0x04003D25 RID: 15653
		private bool m_triggerHandEquipmentEvent;
	}
}
