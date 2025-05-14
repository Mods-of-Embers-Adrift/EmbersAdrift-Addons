using System;
using System.Collections;
using Sirenix.OdinInspector;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE8 RID: 2792
	public static class ReagentTypeExtensions
	{
		// Token: 0x170013FF RID: 5119
		// (get) Token: 0x06005610 RID: 22032 RVA: 0x00079689 File Offset: 0x00077889
		public static ReagentType[] AllReagentTypes
		{
			get
			{
				if (ReagentTypeExtensions.m_reagentTypes == null)
				{
					ReagentTypeExtensions.m_reagentTypes = (ReagentType[])Enum.GetValues(typeof(ReagentType));
				}
				return ReagentTypeExtensions.m_reagentTypes;
			}
		}

		// Token: 0x06005611 RID: 22033 RVA: 0x000796B0 File Offset: 0x000778B0
		public static IEnumerable GetReagentDropdownItems()
		{
			int num;
			for (int i = 0; i < ReagentTypeExtensions.AllReagentTypes.Length; i = num + 1)
			{
				yield return new ValueDropdownItem(ReagentTypeExtensions.AllReagentTypes[i].GetEnumDrawerDescriptionForReagentType(), ReagentTypeExtensions.AllReagentTypes[i]);
				num = i;
			}
			yield break;
		}

		// Token: 0x06005612 RID: 22034 RVA: 0x000796B9 File Offset: 0x000778B9
		public static ArchetypeInstance GetReagentForEntity(this ReagentType reagentType, GameEntity entity, float level)
		{
			if (!entity || entity.CollectionController == null || entity.CollectionController.ReagentPouch == null)
			{
				return null;
			}
			return ReagentTypeExtensions.GetReagentTypeFromContainer(reagentType, level, entity.CollectionController.ReagentPouch, entity);
		}

		// Token: 0x06005613 RID: 22035 RVA: 0x001E0378 File Offset: 0x001DE578
		private static ArchetypeInstance GetReagentTypeFromContainer(ReagentType reagentType, float level, ContainerInstance containerInstance, GameEntity entity)
		{
			if (containerInstance == null)
			{
				throw new ArgumentNullException("containerInstance");
			}
			if (reagentType == ReagentType.None)
			{
				return null;
			}
			ArchetypeInstance archetypeInstance = null;
			for (int i = 0; i < containerInstance.Count; i++)
			{
				ArchetypeInstance index = containerInstance.GetIndex(i);
				ReagentItem reagentItem;
				if (index != null && index.Archetype && index.Archetype.TryGetAsType(out reagentItem) && reagentItem.Type == reagentType && index.ContainerInstance.GetToggle(index.Index) && reagentItem.MeetsRequirements(entity) && (archetypeInstance == null || index.Index < archetypeInstance.Index))
				{
					archetypeInstance = index;
				}
			}
			return archetypeInstance;
		}

		// Token: 0x06005614 RID: 22036 RVA: 0x001E040C File Offset: 0x001DE60C
		internal static bool GetBaseSpecializedRoles(this ReagentType reagentType, out BaseRoleFlags baseRole, out SpecializedRoleFlags specializedRole)
		{
			baseRole = BaseRoleFlags.None;
			specializedRole = SpecializedRoleFlags.None;
			if (reagentType <= ReagentType.Flask)
			{
				if (reagentType <= ReagentType.Salve)
				{
					if (reagentType == ReagentType.Irritant)
					{
						baseRole = BaseRoleFlags.Defender;
						return true;
					}
					if (reagentType == ReagentType.NumbingAgent)
					{
						baseRole = BaseRoleFlags.Striker;
						return true;
					}
					if (reagentType == ReagentType.Salve)
					{
						baseRole = BaseRoleFlags.Supporter;
						return true;
					}
				}
				else
				{
					if (reagentType - ReagentType.Tonic <= 1)
					{
						baseRole = BaseRoleFlags.Defender;
						specializedRole = SpecializedRoleFlags.Juggernaut;
						return true;
					}
					if (reagentType == ReagentType.Pheromones)
					{
						baseRole = BaseRoleFlags.Defender;
						specializedRole = SpecializedRoleFlags.Knight;
						return true;
					}
					if (reagentType == ReagentType.Flask)
					{
						baseRole = BaseRoleFlags.Defender;
						specializedRole = SpecializedRoleFlags.Marshal;
						return true;
					}
				}
			}
			else if (reagentType <= ReagentType.Serum)
			{
				if (reagentType == ReagentType.Dopant)
				{
					baseRole = BaseRoleFlags.Striker;
					specializedRole = SpecializedRoleFlags.Berserker;
					return true;
				}
				if (reagentType == ReagentType.Toxin)
				{
					baseRole = BaseRoleFlags.Striker;
					specializedRole = SpecializedRoleFlags.Warden;
					return true;
				}
				if (reagentType - ReagentType.Dose <= 1)
				{
					baseRole = BaseRoleFlags.Striker;
					specializedRole = SpecializedRoleFlags.Brigand;
					return true;
				}
			}
			else
			{
				if (reagentType == ReagentType.Coagulent)
				{
					baseRole = BaseRoleFlags.Supporter;
					specializedRole = SpecializedRoleFlags.Duelist;
					return true;
				}
				if (reagentType == ReagentType.Tincture)
				{
					baseRole = BaseRoleFlags.Supporter;
					specializedRole = SpecializedRoleFlags.Sentinel;
					return true;
				}
				if (reagentType == ReagentType.Infection)
				{
					baseRole = BaseRoleFlags.Supporter;
					specializedRole = SpecializedRoleFlags.Warlord;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005615 RID: 22037 RVA: 0x001E04E0 File Offset: 0x001DE6E0
		internal static string GetEnumDrawerDescriptionForReagentType(this ReagentType reagentType)
		{
			BaseRoleFlags flag;
			SpecializedRoleFlags specializedRoleFlags;
			if (!reagentType.GetBaseSpecializedRoles(out flag, out specializedRoleFlags))
			{
				return "None";
			}
			if (specializedRoleFlags != SpecializedRoleFlags.None)
			{
				string secondaryRoleAbbreviation = specializedRoleFlags.GetSecondaryRoleAbbreviation();
				return string.Concat(new string[]
				{
					reagentType.ToString(),
					" (",
					specializedRoleFlags.ToString(),
					" - ",
					flag.GetBaseRoleAbbreviation(),
					"/",
					secondaryRoleAbbreviation,
					")"
				});
			}
			return reagentType.ToString() + " (" + flag.ToString() + ")";
		}

		// Token: 0x06005616 RID: 22038 RVA: 0x001E058C File Offset: 0x001DE78C
		public static bool IsCompatibleWithEntity(this ReagentType reagentType, GameEntity entity)
		{
			BaseRoleFlags baseRoleFlags;
			SpecializedRoleFlags specializedRoleFlags;
			if (entity && entity.CharacterData && reagentType.GetBaseSpecializedRoles(out baseRoleFlags, out specializedRoleFlags))
			{
				if (specializedRoleFlags != SpecializedRoleFlags.None)
				{
					SpecializedRoleFlags specializedRoleFlag = GlobalSettings.Values.Roles.GetSpecializedRoleFlag(entity.CharacterData.SpecializedRoleId);
					return specializedRoleFlag != SpecializedRoleFlags.None && specializedRoleFlags.HasBitFlag(specializedRoleFlag);
				}
				if (baseRoleFlags != BaseRoleFlags.None)
				{
					BaseRoleFlags baseRoleFlag = GlobalSettings.Values.Roles.GetBaseRoleFlag(entity.CharacterData.BaseRoleId);
					return baseRoleFlag != BaseRoleFlags.None && baseRoleFlags.HasBitFlag(baseRoleFlag);
				}
			}
			return false;
		}

		// Token: 0x04004C59 RID: 19545
		private static ReagentType[] m_reagentTypes;
	}
}
