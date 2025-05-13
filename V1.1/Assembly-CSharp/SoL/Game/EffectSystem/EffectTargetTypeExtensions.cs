using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C70 RID: 3184
	public static class EffectTargetTypeExtensions
	{
		// Token: 0x0600614C RID: 24908 RVA: 0x0008197D File Offset: 0x0007FB7D
		public static bool IsAOE(this EffectTargetType type)
		{
			return type - EffectTargetType.Self_Offensive_AOE <= 1 || type - EffectTargetType.Offensive_AOE <= 2 || type - EffectTargetType.Defensive_AOE <= 2;
		}

		// Token: 0x0600614D RID: 24909 RVA: 0x00081996 File Offset: 0x0007FB96
		public static bool IsChain(this EffectTargetType type)
		{
			return type == EffectTargetType.Offensive_Chain || type == EffectTargetType.Defensive_Chain;
		}

		// Token: 0x0600614E RID: 24910 RVA: 0x000819A5 File Offset: 0x0007FBA5
		public static bool IsFriendly(this EffectTargetType type)
		{
			return type == EffectTargetType.Self || type == EffectTargetType.Self_Defensive_AOE || type - EffectTargetType.Defensive <= 4;
		}

		// Token: 0x0600614F RID: 24911 RVA: 0x000819B8 File Offset: 0x0007FBB8
		public static bool CanExcludeSelf(this EffectTargetType type)
		{
			return type == EffectTargetType.Self_Defensive_AOE || type - EffectTargetType.Defensive <= 4;
		}

		// Token: 0x06006150 RID: 24912 RVA: 0x000819C8 File Offset: 0x0007FBC8
		public static bool CheckAngle(this EffectTargetType type)
		{
			return type == EffectTargetType.Offensive_AOE_Conal || type == EffectTargetType.Defensive_AOE_Conal;
		}

		// Token: 0x06006151 RID: 24913 RVA: 0x000819D7 File Offset: 0x0007FBD7
		public static bool RequiresLineOfSight(this EffectTargetType type)
		{
			return type > EffectTargetType.Self_Defensive_AOE;
		}

		// Token: 0x06006152 RID: 24914 RVA: 0x000819E0 File Offset: 0x0007FBE0
		public static bool RequiresTarget(this EffectTargetType appTarget)
		{
			return appTarget > EffectTargetType.Self_Defensive_AOE && appTarget != EffectTargetType.Offensive_AOE_Conal && appTarget != EffectTargetType.Defensive_AOE_Conal;
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x000819F3 File Offset: 0x0007FBF3
		public static bool MustMaintainDefensiveTarget(this EffectTargetType type)
		{
			return type - EffectTargetType.Defensive <= 2 || type == EffectTargetType.Defensive_AOE_Ground;
		}

		// Token: 0x06006154 RID: 24916 RVA: 0x00081A04 File Offset: 0x0007FC04
		public static bool MustMaintainOffensiveTarget(this EffectTargetType type)
		{
			return type - EffectTargetType.Offensive <= 2 || type == EffectTargetType.Offensive_AOE_Ground;
		}

		// Token: 0x06006155 RID: 24917 RVA: 0x001FF648 File Offset: 0x001FD848
		public static GameEntity GetTarget(this EffectTargetType type, GameEntity source)
		{
			if (source == null || source.TargetController == null)
			{
				return null;
			}
			GameEntity gameEntity = null;
			if (type > EffectTargetType.Self_Defensive_AOE)
			{
				switch (type)
				{
				case EffectTargetType.Offensive:
				case EffectTargetType.Offensive_Chain:
				case EffectTargetType.Offensive_AOE:
					gameEntity = source.TargetController.OffensiveTarget;
					goto IL_80;
				case EffectTargetType.Offensive_AOE_Conal:
				case EffectTargetType.Defensive_AOE_Conal:
					break;
				case EffectTargetType.Offensive_AOE_Ground:
				case (EffectTargetType)15:
				case (EffectTargetType)16:
				case (EffectTargetType)17:
				case (EffectTargetType)18:
				case (EffectTargetType)19:
					goto IL_80;
				case EffectTargetType.Defensive:
				case EffectTargetType.Defensive_Chain:
				case EffectTargetType.Defensive_AOE:
					gameEntity = source.TargetController.DefensiveTarget;
					goto IL_80;
				default:
					goto IL_80;
				}
			}
			gameEntity = source;
			IL_80:
			if (gameEntity != null && (gameEntity.VitalsReplicator == null || gameEntity.VitalsReplicator.CurrentHealthState.Value == HealthState.Dead))
			{
				gameEntity = null;
			}
			return gameEntity;
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x001FF704 File Offset: 0x001FD904
		public static string GetOffensiveDefensiveDescription(this EffectTargetType type)
		{
			switch (type)
			{
			case EffectTargetType.Self:
			case EffectTargetType.Self_Offensive_AOE:
			case EffectTargetType.Self_Defensive_AOE:
				return "Self";
			case EffectTargetType.Offensive:
			case EffectTargetType.Offensive_Chain:
			case EffectTargetType.Offensive_AOE:
			case EffectTargetType.Offensive_AOE_Conal:
			case EffectTargetType.Offensive_AOE_Ground:
				return "Offensive";
			case EffectTargetType.Defensive:
			case EffectTargetType.Defensive_Chain:
			case EffectTargetType.Defensive_AOE:
			case EffectTargetType.Defensive_AOE_Conal:
			case EffectTargetType.Defensive_AOE_Ground:
				return "Defensive";
			}
			return "Unknown";
		}
	}
}
