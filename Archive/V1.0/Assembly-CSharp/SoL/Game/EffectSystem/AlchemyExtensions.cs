using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Pooling;
using SoL.Game.Settings;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C09 RID: 3081
	public static class AlchemyExtensions
	{
		// Token: 0x1700167F RID: 5759
		// (get) Token: 0x06005ED1 RID: 24273 RVA: 0x0007FC9E File Offset: 0x0007DE9E
		public static AlchemyPowerLevel[] AlchemyPowerLevels
		{
			get
			{
				if (AlchemyExtensions.m_alchemyPowerLevels == null)
				{
					AlchemyExtensions.m_alchemyPowerLevels = (AlchemyPowerLevel[])Enum.GetValues(typeof(AlchemyPowerLevel));
				}
				return AlchemyExtensions.m_alchemyPowerLevels;
			}
		}

		// Token: 0x06005ED2 RID: 24274 RVA: 0x0007FCC5 File Offset: 0x0007DEC5
		public static float GetAddedExecutionTime(this AlchemyPowerLevel alchemyPowerLevel)
		{
			return (float)GlobalSettings.Values.Ashen.GetAlchemyAdditionalExecutionTime(alchemyPowerLevel);
		}

		// Token: 0x06005ED3 RID: 24275 RVA: 0x0007FCD8 File Offset: 0x0007DED8
		public static int GetRequiredEmberEssence(this AlchemyPowerLevel alchemyPowerLevel)
		{
			return GlobalSettings.Values.Ashen.GetAlchemyEssenceCost(alchemyPowerLevel);
		}

		// Token: 0x06005ED4 RID: 24276 RVA: 0x001F7F14 File Offset: 0x001F6114
		public static bool AlchemyPowerLevelAvailable(GameEntity entity, ArchetypeInstance abilityInstance, AlchemyPowerLevel requestedPowerLevel)
		{
			string text;
			return GlobalSettings.Values.Ashen.AlchemyPowerLevelAvailable(entity, abilityInstance, requestedPowerLevel, true, out text);
		}

		// Token: 0x06005ED5 RID: 24277 RVA: 0x0007FCEA File Offset: 0x0007DEEA
		public static bool AlchemyPowerLevelAvailable(GameEntity entity, ArchetypeInstance abilityInstance, AlchemyPowerLevel requestedPowerLevel, out string msg)
		{
			return GlobalSettings.Values.Ashen.AlchemyPowerLevelAvailable(entity, abilityInstance, requestedPowerLevel, true, out msg);
		}

		// Token: 0x06005ED6 RID: 24278 RVA: 0x001F7F38 File Offset: 0x001F6138
		public static bool AlchemyPowerLevelAvailableBypassCooldown(GameEntity entity, ArchetypeInstance abilityInstance, AlchemyPowerLevel requestedPowerLevel)
		{
			string text;
			return GlobalSettings.Values.Ashen.AlchemyPowerLevelAvailable(entity, abilityInstance, requestedPowerLevel, false, out text);
		}

		// Token: 0x06005ED7 RID: 24279 RVA: 0x0007FD00 File Offset: 0x0007DF00
		public static PooledVFX GetSourceExecution(this AlchemyPowerLevel alchemyPowerLevel)
		{
			return GlobalSettings.Values.Ashen.GetAlchemyPooledVFX(alchemyPowerLevel);
		}

		// Token: 0x06005ED8 RID: 24280 RVA: 0x001F7F5C File Offset: 0x001F615C
		public static bool EntityHasRequiredEmberEssence(GameEntity entity, AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel == AlchemyPowerLevel.None)
			{
				return true;
			}
			if (!entity || entity.CollectionController == null)
			{
				return false;
			}
			int emberEssenceCount = entity.CollectionController.GetEmberEssenceCount();
			int requiredEmberEssence = alchemyPowerLevel.GetRequiredEmberEssence();
			return emberEssenceCount >= requiredEmberEssence;
		}

		// Token: 0x06005ED9 RID: 24281 RVA: 0x0007FD12 File Offset: 0x0007DF12
		public static bool AllowActiveDefenses(this AlchemyPowerLevel alchemyPowerLevel)
		{
			return alchemyPowerLevel == AlchemyPowerLevel.None;
		}

		// Token: 0x06005EDA RID: 24282 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this AlchemyPowerLevelFlags a, AlchemyPowerLevelFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005EDB RID: 24283 RVA: 0x0007FD18 File Offset: 0x0007DF18
		public static AlchemyPowerLevel GetPreviousPowerLevel(this AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel != AlchemyPowerLevel.I && alchemyPowerLevel == AlchemyPowerLevel.II)
			{
				return AlchemyPowerLevel.I;
			}
			return AlchemyPowerLevel.None;
		}

		// Token: 0x06005EDC RID: 24284 RVA: 0x0007FD25 File Offset: 0x0007DF25
		public static string GetAlchemyPowerLevelDescription(this AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel == AlchemyPowerLevel.I)
			{
				return "Alchemy I";
			}
			if (alchemyPowerLevel != AlchemyPowerLevel.II)
			{
				return string.Empty;
			}
			return "Alchemy II";
		}

		// Token: 0x06005EDD RID: 24285 RVA: 0x0007FD42 File Offset: 0x0007DF42
		public static int GetKeybindActionId(this AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel == AlchemyPowerLevel.I)
			{
				return 106;
			}
			if (alchemyPowerLevel != AlchemyPowerLevel.II)
			{
				return -1;
			}
			return 107;
		}

		// Token: 0x06005EDE RID: 24286 RVA: 0x001F7F98 File Offset: 0x001F6198
		public static int GetUsageDeltaToUnlockAlchemyII(ArchetypeInstance instance)
		{
			if (instance == null || instance.AbilityData == null)
			{
				return 0;
			}
			int usageCount = instance.AbilityData.GetUsageCount(AlchemyPowerLevel.I);
			return GlobalSettings.Values.Ashen.GetAlchemyUsageThreshold(AlchemyPowerLevel.II) - usageCount;
		}

		// Token: 0x04005202 RID: 20994
		private static AlchemyPowerLevel[] m_alchemyPowerLevels;
	}
}
