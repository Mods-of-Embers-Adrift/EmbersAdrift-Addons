using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000389 RID: 905
	public static class TooltipExtensions
	{
		// Token: 0x060018C8 RID: 6344 RVA: 0x001053F0 File Offset: 0x001035F0
		public static void AddRoleLevelRequirement(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, LevelRequirement levelRequirement, BaseRoleRequirement roleRequirement)
		{
			if (tooltip == null)
			{
				return;
			}
			int num = 0;
			string levelTypeDescription = string.Empty;
			bool meetsLevelRequirement = false;
			bool meetsRoleRequirement = false;
			string empty = string.Empty;
			string endString = string.Empty;
			if (levelRequirement != null)
			{
				num = levelRequirement.Level;
				levelTypeDescription = levelRequirement.Type.ToString();
				if (instance != null && instance.ItemData != null && instance.ItemData.Augment != null && instance.ItemData.Augment.AugmentItemRef && instance.ItemData.Augment.AugmentItemRef.LevelReq > num)
				{
					Color color = instance.ItemData.Augment.AugmentItemRef.MeetsLevelRequirement(entity) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
					endString = ZString.Format<string, int, string>(" (<color={0}>{1}</color> via <color={2}>Augment</color>)", color.ToHex(), instance.ItemData.Augment.AugmentItemRef.LevelReq, UIManager.AugmentColor.ToHex());
				}
			}
			bool showLevelRequirement = levelRequirement != null && levelRequirement.ShowLevelRequirementForTooltip(entity, out meetsLevelRequirement);
			bool showRoleRequirement = roleRequirement != null && roleRequirement.ShowRoleRequirement(entity, out meetsRoleRequirement, out empty);
			TooltipExtensions.AddRoleLevelRequirementInternal(tooltip, showLevelRequirement, showRoleRequirement, meetsLevelRequirement, meetsRoleRequirement, num, levelTypeDescription, empty, endString);
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x00105524 File Offset: 0x00103724
		public static void AddRoleLevelRequirementForReagent(ArchetypeTooltip tooltip, GameEntity entity, LevelRequirement levelRequirement, ReagentItem reagent)
		{
			if (tooltip == null || reagent == null)
			{
				return;
			}
			int level = 0;
			string levelTypeDescription = string.Empty;
			bool meetsLevelRequirement = false;
			bool meetsRoleRequirement = false;
			string roleDescription = string.Empty;
			bool showLevelRequirement = false;
			bool showRoleRequirement = true;
			if (levelRequirement != null)
			{
				level = levelRequirement.Level;
				levelTypeDescription = levelRequirement.Type.ToString();
				showLevelRequirement = levelRequirement.ShowLevelRequirementForTooltip(entity, out meetsLevelRequirement);
			}
			BaseRoleFlags a;
			SpecializedRoleFlags specializedRoleFlags;
			if (reagent.Type.GetBaseSpecializedRoles(out a, out specializedRoleFlags))
			{
				if (specializedRoleFlags != SpecializedRoleFlags.None)
				{
					roleDescription = specializedRoleFlags.GetSpecializationRoleAbbreviation();
					if (entity && entity.CharacterData && !entity.CharacterData.SpecializedRoleId.IsEmpty)
					{
						SpecializedRoleFlags specializedRoleFlag = GlobalSettings.Values.Roles.GetSpecializedRoleFlag(entity.CharacterData.SpecializedRoleId);
						meetsRoleRequirement = specializedRoleFlags.HasBitFlag(specializedRoleFlag);
					}
				}
				else
				{
					roleDescription = a.ToString();
					if (entity && entity.CharacterData && !entity.CharacterData.BaseRoleId.IsEmpty)
					{
						BaseRoleFlags baseRoleFlag = GlobalSettings.Values.Roles.GetBaseRoleFlag(entity.CharacterData.BaseRoleId);
						meetsRoleRequirement = a.HasBitFlag(baseRoleFlag);
					}
				}
			}
			TooltipExtensions.AddRoleLevelRequirementInternal(tooltip, showLevelRequirement, showRoleRequirement, meetsLevelRequirement, meetsRoleRequirement, level, levelTypeDescription, roleDescription, string.Empty);
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0010567C File Offset: 0x0010387C
		private static void AddRoleLevelRequirementInternal(ArchetypeTooltip tooltip, bool showLevelRequirement, bool showRoleRequirement, bool meetsLevelRequirement, bool meetsRoleRequirement, int level, string levelTypeDescription, string roleDescription, string endString)
		{
			if (showLevelRequirement && showRoleRequirement)
			{
				Color color = (meetsLevelRequirement && meetsRoleRequirement) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				tooltip.RequirementsBlock.AppendLine(ZString.Format<string, int, string, string>("<color={0}>{1} {2}</color>{3}", color.ToHex(), level, roleDescription, endString), 0);
				return;
			}
			if (showLevelRequirement)
			{
				Color color2 = meetsLevelRequirement ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				tooltip.RequirementsBlock.AppendLine(ZString.Format<string, int, string, string>("<color={0}>{1} {2}</color>{3}", color2.ToHex(), level, levelTypeDescription, endString), 0);
				return;
			}
			if (showRoleRequirement)
			{
				Color color3 = meetsRoleRequirement ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				tooltip.RequirementsBlock.AppendLine(ZString.Format<string, string, string>("<color={0}>{1}</color>{2}", color3.ToHex(), roleDescription, endString), 0);
			}
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x00053551 File Offset: 0x00051751
		public static string GetThreatDescription()
		{
			if (!UIManager.TooltipShowMore)
			{
				return "Thr";
			}
			return "Threat";
		}

		// Token: 0x04001FDD RID: 8157
		public static readonly List<string> ToCombine = new List<string>(10);
	}
}
