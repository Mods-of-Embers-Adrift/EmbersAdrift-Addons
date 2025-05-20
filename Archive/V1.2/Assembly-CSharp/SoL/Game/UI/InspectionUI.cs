using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Utilities;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200089C RID: 2204
	public class InspectionUI : ContainerUI<EquipmentSlot, EquipmentSlotUI>
	{
		// Token: 0x17000EBF RID: 3775
		// (get) Token: 0x06004034 RID: 16436 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColors
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x06004035 RID: 16437 RVA: 0x0018BBEC File Offset: 0x00189DEC
		protected override void InitializeSlots()
		{
			this.m_slots = new DictionaryList<EquipmentSlot, EquipmentSlotUI>(default(EquipmentSlotComparer), false);
			for (int i = 0; i < this.m_slotSettings.Length; i++)
			{
				EquipmentSlotSetting equipmentSlotSetting = this.m_slotSettings[i];
				equipmentSlotSetting.UI.AssignSettings(equipmentSlotSetting, this.m_defaultBorderColor, this.m_armorBorderColor, false);
				equipmentSlotSetting.UI.Initialize(this, (int)equipmentSlotSetting.Type);
				this.m_slots.Add(equipmentSlotSetting.Type, equipmentSlotSetting.UI);
			}
		}

		// Token: 0x06004036 RID: 16438 RVA: 0x0006B78C File Offset: 0x0006998C
		private void ClearItems()
		{
			ContainerInstance container = this.m_container;
			if (container == null)
			{
				return;
			}
			container.DestroyContents();
		}

		// Token: 0x06004037 RID: 16439 RVA: 0x0006B79E File Offset: 0x0006999E
		protected override void UiWindowOnWindowClosed()
		{
			base.UiWindowOnWindowClosed();
			this.ClearItems();
		}

		// Token: 0x06004038 RID: 16440 RVA: 0x0018BC78 File Offset: 0x00189E78
		public void InitInspection(string targetName)
		{
			if (this.m_titleLabel)
			{
				string arg = string.IsNullOrEmpty(targetName) ? "Inspect" : targetName;
				this.m_titleLabel.ZStringSetText(arg);
			}
			if (!base.IsShown)
			{
				base.ForceToggleWindow(true);
			}
		}

		// Token: 0x04003E1A RID: 15898
		[SerializeField]
		private EquipmentSlotSetting[] m_slotSettings;

		// Token: 0x04003E1B RID: 15899
		[SerializeField]
		private Color m_defaultBorderColor = new Color(0.1490196f, 0.1098039f, 0.07450981f);

		// Token: 0x04003E1C RID: 15900
		[SerializeField]
		private Color m_armorBorderColor = Colors.Gold;

		// Token: 0x04003E1D RID: 15901
		[SerializeField]
		private TextMeshProUGUI m_titleLabel;
	}
}
