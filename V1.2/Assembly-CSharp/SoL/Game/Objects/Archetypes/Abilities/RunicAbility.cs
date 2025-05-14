using System;
using SoL.Game.UI;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF5 RID: 2805
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Runic Ability")]
	public class RunicAbility : AppliableEffectAbility
	{
		// Token: 0x17001452 RID: 5202
		// (get) Token: 0x060056D6 RID: 22230 RVA: 0x00079DA1 File Offset: 0x00077FA1
		public int UseCost
		{
			get
			{
				return this.m_useCost;
			}
		}

		// Token: 0x060056D7 RID: 22231 RVA: 0x001E15F8 File Offset: 0x001DF7F8
		private bool CheckForRuneBattery(GameEntity entity, bool mainHand, out ArchetypeInstance batteryInstance, out RunicBattery battery)
		{
			batteryInstance = null;
			battery = null;
			bool flag = mainHand ? entity.TryGetHandheldItem_MainHandAsType(out batteryInstance, out battery) : entity.TryGetHandheldItem_OffHandAsType(out batteryInstance, out battery);
			if (flag && battery.RequiresFreeOffHand)
			{
				ArchetypeInstance archetypeInstance2;
				IHandheldItem handheldItem2;
				if (mainHand)
				{
					ArchetypeInstance archetypeInstance;
					IHandheldItem handheldItem;
					if (entity.TryGetHandheldItem_OffHandAsType(out archetypeInstance, out handheldItem))
					{
						return false;
					}
				}
				else if (entity.TryGetHandheldItem_MainHandAsType(out archetypeInstance2, out handheldItem2))
				{
					return false;
				}
			}
			return flag && battery.Mastery.Id == base.Mastery.Id && batteryInstance.ItemData.Charges != null && batteryInstance.ItemData.Charges.Value >= this.m_useCost;
		}

		// Token: 0x060056D8 RID: 22232 RVA: 0x001E16AC File Offset: 0x001DF8AC
		protected override bool MeetsRequirementsForUI(GameEntity entity, float level)
		{
			ArchetypeInstance archetypeInstance;
			RunicBattery runicBattery;
			return base.MeetsRequirementsForUI(entity, level) && (this.CheckForRuneBattery(entity, false, out archetypeInstance, out runicBattery) || this.CheckForRuneBattery(entity, true, out archetypeInstance, out runicBattery));
		}

		// Token: 0x060056D9 RID: 22233 RVA: 0x001E16E4 File Offset: 0x001DF8E4
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (!base.ExecutionCheck(executionCache, executionProgress))
			{
				return false;
			}
			bool flag = executionProgress <= 0f;
			ArchetypeInstance instance = null;
			RunicBattery runicBattery = null;
			if (this.CheckForRuneBattery(executionCache.SourceEntity, false, out instance, out runicBattery) || this.CheckForRuneBattery(executionCache.SourceEntity, true, out instance, out runicBattery))
			{
				if (flag)
				{
					executionCache.AddReductionTask(ReductionTaskType.Charge, instance, this.m_useCost);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060056DA RID: 22234 RVA: 0x001E1748 File Offset: 0x001DF948
		protected override void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, int masteryLevel, int abilityLevel)
		{
			base.FillTooltipBlocksInternal(tooltip, instance, entity, masteryLevel, abilityLevel);
			RuneMasteryArchetype runeMasteryArchetype;
			if (base.Mastery.TryGetAsType(out runeMasteryArchetype))
			{
				TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
				ArchetypeInstance archetypeInstance;
				RunicBattery runicBattery;
				Color color = (this.CheckForRuneBattery(entity, false, out archetypeInstance, out runicBattery) || this.CheckForRuneBattery(entity, true, out archetypeInstance, out runicBattery)) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				string str = "Rune Type".Color(color).Indent(5);
				requirementsBlock.AppendLine("<sprite=\"SolIcons\" name=\"Circle\" tint=1>" + str, (runeMasteryArchetype.RuneSource.ToString() ?? "").Color(color));
				bool flag;
				if (archetypeInstance != null && archetypeInstance.ItemData.Charges != null)
				{
					int? charges = archetypeInstance.ItemData.Charges;
					int useCost = this.m_useCost;
					flag = (charges.GetValueOrDefault() >= useCost & charges != null);
				}
				else
				{
					flag = false;
				}
				color = (flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				str = "Charges".Color(color).Indent(5);
				requirementsBlock.AppendLine("<sprite=\"SolIcons\" name=\"Circle\" tint=1>" + str, (this.m_useCost.ToString() ?? "").Color(color));
				int num;
				if ((float)this.m_useCost > 0f && EquipmentUI.TryGetAvailableCharges(runeMasteryArchetype.RuneSource, out num))
				{
					tooltip.DataBlock.AppendLine("Remaining Casts", Mathf.FloorToInt((float)num / (float)this.m_useCost).ToString());
				}
			}
		}

		// Token: 0x04004C86 RID: 19590
		[SerializeField]
		private int m_useCost = 10;
	}
}
