using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Utilities.Extensions;

namespace SoL.Game.UI
{
	// Token: 0x0200085A RID: 2138
	public static class BindingLabels
	{
		// Token: 0x06003DB5 RID: 15797 RVA: 0x001834F0 File Offset: 0x001816F0
		private static bool TryGetBindingDictForType(this BindingType type, out Dictionary<int, BindingLabels.BindingLabelData> data)
		{
			if (BindingLabels.m_bindings == null)
			{
				BindingLabels.m_bindings = new Dictionary<BindingType, Dictionary<int, BindingLabels.BindingLabelData>>(default(BindingTypeComparer))
				{
					{
						BindingType.Ability,
						new Dictionary<int, BindingLabels.BindingLabelData>()
					},
					{
						BindingType.Consumable,
						new Dictionary<int, BindingLabels.BindingLabelData>()
					},
					{
						BindingType.Reagent,
						new Dictionary<int, BindingLabels.BindingLabelData>()
					},
					{
						BindingType.Group,
						new Dictionary<int, BindingLabels.BindingLabelData>()
					},
					{
						BindingType.Action,
						new Dictionary<int, BindingLabels.BindingLabelData>()
					},
					{
						BindingType.Macro,
						new Dictionary<int, BindingLabels.BindingLabelData>()
					}
				};
			}
			return BindingLabels.m_bindings.TryGetValue(type, out data);
		}

		// Token: 0x06003DB6 RID: 15798 RVA: 0x00183570 File Offset: 0x00181770
		public static void RegisterBinding(IBindingLabel binding, bool allowSecondaries)
		{
			if (binding == null || binding.Type == BindingType.None || binding.Index < 0 || binding.Label == null)
			{
				return;
			}
			Dictionary<int, BindingLabels.BindingLabelData> dictionary;
			if (!binding.Type.TryGetBindingDictForType(out dictionary))
			{
				return;
			}
			BindingLabels.BindingLabelData bindingLabelData;
			if (dictionary.TryGetValue(binding.Index, out bindingLabelData))
			{
				if (allowSecondaries)
				{
					bindingLabelData.SecondaryBindingLabel = binding;
					return;
				}
				BindingLabels.DeregisterBinding(bindingLabelData, dictionary);
			}
			bindingLabelData = new BindingLabels.BindingLabelData(binding);
			dictionary.AddOrReplace(binding.Index, bindingLabelData);
		}

		// Token: 0x06003DB7 RID: 15799 RVA: 0x001835E8 File Offset: 0x001817E8
		public static void DeregisterBinding(IBindingLabel binding)
		{
			if (binding == null)
			{
				return;
			}
			Dictionary<int, BindingLabels.BindingLabelData> dictionary;
			BindingLabels.BindingLabelData bindingLabelData;
			if (binding.Type.TryGetBindingDictForType(out dictionary) && dictionary.TryGetValue(binding.Index, out bindingLabelData))
			{
				BindingLabels.DeregisterBinding(bindingLabelData, dictionary);
			}
		}

		// Token: 0x06003DB8 RID: 15800 RVA: 0x00069C5B File Offset: 0x00067E5B
		private static void DeregisterBinding(BindingLabels.BindingLabelData bindingLabelData, Dictionary<int, BindingLabels.BindingLabelData> bindingDict)
		{
			if (bindingLabelData == null)
			{
				return;
			}
			bindingLabelData.Destroy();
			bindingDict.Remove(bindingLabelData.BindingLabel.Index);
		}

		// Token: 0x06003DB9 RID: 15801 RVA: 0x00183620 File Offset: 0x00181820
		public static string GetAbbreviatedBindingLabel(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return string.Empty;
			}
			return txt.Replace("Mouse Button", "mb").Replace("Back Quote", "~").Replace("Shift", "S").Replace("Ctrl", "C").Replace("Alt", "A").Replace("Cmd", "M").Replace("Page", "Pg").Replace("Down", "Dn").Replace("Insert", "Ins").Replace("Delete", "Del").Replace(" + ", "");
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x001836E4 File Offset: 0x001818E4
		private static int GetActionIdForIndex(BindingType type, int index)
		{
			switch (type)
			{
			case BindingType.Ability:
				return BindingLabels.GetActionForAbilityIndex(index);
			case BindingType.Consumable:
				return BindingLabels.GetActionForConsumableIndex(index);
			case BindingType.Reagent:
				return BindingLabels.GetActionForReagentIndex(index);
			case BindingType.Group:
				return BindingLabels.GetActionForGroupIndex(index);
			case BindingType.Action:
				return BindingLabels.GetActionForActionIndex(index);
			case BindingType.Macro:
				return BindingLabels.GetActionForMacroIndex(index);
			default:
				return -1;
			}
		}

		// Token: 0x06003DBB RID: 15803 RVA: 0x00183740 File Offset: 0x00181940
		private static int GetActionForAbilityIndex(int index)
		{
			switch (index)
			{
			case 0:
				return 22;
			case 1:
				return 23;
			case 2:
				return 24;
			case 3:
				return 25;
			case 4:
				return 26;
			case 5:
				return 27;
			case 6:
				return 28;
			case 7:
				return 29;
			case 8:
				return 30;
			case 9:
				return 31;
			default:
				if (index != 1024)
				{
					return -1;
				}
				return 20;
			}
		}

		// Token: 0x06003DBC RID: 15804 RVA: 0x00069C79 File Offset: 0x00067E79
		private static int GetActionForConsumableIndex(int index)
		{
			switch (index)
			{
			case 0:
				return 88;
			case 1:
				return 89;
			case 2:
				return 90;
			case 3:
				return 91;
			default:
				return -1;
			}
		}

		// Token: 0x06003DBD RID: 15805 RVA: 0x00069CA0 File Offset: 0x00067EA0
		private static int GetActionForReagentIndex(int index)
		{
			switch (index)
			{
			case 0:
				return 92;
			case 1:
				return 93;
			case 2:
				return 94;
			case 3:
				return 95;
			default:
				return -1;
			}
		}

		// Token: 0x06003DBE RID: 15806 RVA: 0x00069CC7 File Offset: 0x00067EC7
		private static int GetActionForGroupIndex(int index)
		{
			switch (index)
			{
			case 0:
				return 32;
			case 1:
				return 33;
			case 2:
				return 34;
			case 3:
				return 35;
			case 4:
				return 36;
			case 5:
				return 37;
			case 6:
				return 38;
			default:
				return -1;
			}
		}

		// Token: 0x06003DBF RID: 15807 RVA: 0x00069D03 File Offset: 0x00067F03
		private static int GetActionForActionIndex(int index)
		{
			if (index == 0)
			{
				return 13;
			}
			return -1;
		}

		// Token: 0x06003DC0 RID: 15808 RVA: 0x00069D0C File Offset: 0x00067F0C
		private static int GetActionForMacroIndex(int index)
		{
			switch (index)
			{
			case 0:
				return 117;
			case 1:
				return 118;
			case 2:
				return 119;
			case 3:
				return 120;
			case 4:
				return 121;
			default:
				return -1;
			}
		}

		// Token: 0x04003C46 RID: 15430
		public const int kAutoAttackActionBarIndex = 1024;

		// Token: 0x04003C47 RID: 15431
		private static Dictionary<BindingType, Dictionary<int, BindingLabels.BindingLabelData>> m_bindings;

		// Token: 0x0200085B RID: 2139
		private class BindingLabelData
		{
			// Token: 0x17000E4B RID: 3659
			// (get) Token: 0x06003DC1 RID: 15809 RVA: 0x00069D3A File Offset: 0x00067F3A
			public IBindingLabel BindingLabel
			{
				get
				{
					return this.m_bindingLabel;
				}
			}

			// Token: 0x17000E4C RID: 3660
			// (get) Token: 0x06003DC2 RID: 15810 RVA: 0x00069D42 File Offset: 0x00067F42
			// (set) Token: 0x06003DC3 RID: 15811 RVA: 0x00069D4A File Offset: 0x00067F4A
			public IBindingLabel SecondaryBindingLabel
			{
				get
				{
					return this.m_secondaryBindingLabel;
				}
				internal set
				{
					this.m_secondaryBindingLabel = value;
					this.RefreshLabel();
				}
			}

			// Token: 0x06003DC4 RID: 15812 RVA: 0x001837A8 File Offset: 0x001819A8
			public BindingLabelData(IBindingLabel bindingLabel)
			{
				this.m_bindingLabel = bindingLabel;
				int actionIdForIndex = BindingLabels.GetActionIdForIndex(bindingLabel.Type, this.m_bindingLabel.Index);
				this.m_binding = SolInput.Mapper.GetBinding(actionIdForIndex);
				if (this.m_binding != null)
				{
					this.m_binding.PrimaryMappingUpdated += this.BindingOnPrimaryMappingUpdated;
				}
				this.RefreshLabel();
			}

			// Token: 0x06003DC5 RID: 15813 RVA: 0x00069D59 File Offset: 0x00067F59
			public void Destroy()
			{
				if (this.m_binding != null)
				{
					this.m_binding.PrimaryMappingUpdated -= this.BindingOnPrimaryMappingUpdated;
				}
			}

			// Token: 0x06003DC6 RID: 15814 RVA: 0x00069D7A File Offset: 0x00067F7A
			private void BindingOnPrimaryMappingUpdated(BindingEventData obj)
			{
				this.RefreshLabel();
			}

			// Token: 0x06003DC7 RID: 15815 RVA: 0x00183810 File Offset: 0x00181A10
			private void RefreshLabel()
			{
				if (this.m_bindingLabel == null || this.m_bindingLabel.Label == null)
				{
					return;
				}
				string text = string.Empty;
				if (this.m_binding != null)
				{
					text = SolInput.Mapper.GetPrimaryBindingNameForAction(this.m_binding.ActionId);
					if (!string.IsNullOrEmpty(text))
					{
						text = BindingLabels.GetAbbreviatedBindingLabel(text);
						if (!string.IsNullOrEmpty(this.m_bindingLabel.FormattedString))
						{
							text = ZString.Format<string>(this.m_bindingLabel.FormattedString, text);
						}
					}
				}
				this.m_bindingLabel.Label.SetText(text);
				IBindingLabel secondaryBindingLabel = this.m_secondaryBindingLabel;
				if (secondaryBindingLabel == null)
				{
					return;
				}
				secondaryBindingLabel.Label.SetText(text);
			}

			// Token: 0x04003C48 RID: 15432
			private IBindingLabel m_secondaryBindingLabel;

			// Token: 0x04003C49 RID: 15433
			private readonly IBindingLabel m_bindingLabel;

			// Token: 0x04003C4A RID: 15434
			private readonly Binding m_binding;
		}
	}
}
