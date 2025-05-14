using System;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000277 RID: 631
	public static class DistanceAngleChecks
	{
		// Token: 0x060013C4 RID: 5060 RVA: 0x000F79A8 File Offset: 0x000F5BA8
		public static bool MeetsDistanceRequirements(GameEntity source, GameEntity target, TargetingParams targetingParams, IHandHeldItems handHeldItems, float? distance, float? executionProgress)
		{
			if (targetingParams == null || !source || !target)
			{
				return false;
			}
			MinMaxFloatRange targetDistance = targetingParams.GetTargetDistance(source, handHeldItems);
			return DistanceAngleChecks.MeetsDistanceRequirements(source, target, targetDistance.Min, targetDistance.Max, distance, executionProgress);
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x000F79EC File Offset: 0x000F5BEC
		public static bool MeetsDistanceRequirements(GameEntity source, GameEntity target, float minDistance, float maxDistance, float? distance, float? executionProgress)
		{
			if (!source || !target)
			{
				return false;
			}
			if (distance == null)
			{
				distance = new float?(source.PrimaryTargetPoint.DistanceTo(target.PrimaryTargetPoint));
			}
			if (target.Targetable != null)
			{
				distance -= target.Targetable.DistanceBuffer;
			}
			if (source.Targetable != null)
			{
				distance -= source.Targetable.DistanceBuffer;
			}
			if (DistanceAngleChecks.WithinTargetDistance(source, minDistance, maxDistance, distance.Value, executionProgress))
			{
				return true;
			}
			GameObject closestAlternatePoint = DistanceAngleChecks.GetClosestAlternatePoint(source, target);
			if (closestAlternatePoint)
			{
				distance = new float?(source.PrimaryTargetPoint.DistanceTo(closestAlternatePoint));
				if (target.Targetable != null)
				{
					distance -= target.Targetable.DistanceBuffer;
				}
				if (source.Targetable != null)
				{
					distance -= source.Targetable.DistanceBuffer;
				}
				if (DistanceAngleChecks.WithinTargetDistance(source, minDistance, maxDistance, distance.Value, executionProgress))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x000F7B6C File Offset: 0x000F5D6C
		private static bool WithinTargetDistance(GameEntity entity, float minDistance, float maxDistance, float distance, float? executionProgress)
		{
			distance = Mathf.Clamp(distance, 0f, float.MaxValue);
			if (executionProgress != null)
			{
				maxDistance = Mathf.Lerp(maxDistance, maxDistance + GlobalSettings.Values.Combat.CoyoteTimeDistance, executionProgress.Value);
			}
			bool flag = (entity.Type == GameEntityType.Npc) ? (distance <= maxDistance) : (distance >= minDistance && distance <= maxDistance);
			if (flag)
			{
				TargetingParams.DistanceFailureReason = TargetingParams.DistanceFailureReasons.None;
				return flag;
			}
			TargetingParams.DistanceFailureReason = ((distance > maxDistance) ? TargetingParams.DistanceFailureReasons.TooFar : TargetingParams.DistanceFailureReasons.TooClose);
			return flag;
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x000F7BEC File Offset: 0x000F5DEC
		public static bool MeetsAngleRequirements(GameEntity source, GameEntity target, TargetingParams targetingParams, IHandHeldItems handHeldItems, float? angle, float? executionProgress)
		{
			if (targetingParams == null || !source || !target)
			{
				return false;
			}
			float angleReq = targetingParams.GetTargetAngle(source, handHeldItems) * 0.5f;
			return DistanceAngleChecks.MeetsAngleRequirements(source, target, angleReq, angle, executionProgress, targetingParams.ConalType);
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x000F7C30 File Offset: 0x000F5E30
		public static bool MeetsAngleRequirements(GameEntity source, GameEntity target, float angleReq, float? angle, float? executionProgress, ConalTypes conalType = ConalTypes.None)
		{
			if (!source || !target)
			{
				return false;
			}
			if (angle == null)
			{
				angle = new float?(Mathf.Abs(source.PrimaryTargetPoint.AngleTo(target.PrimaryTargetPoint, true)));
			}
			if (DistanceAngleChecks.WithinTargetAngle(angle.Value, angleReq, executionProgress, conalType))
			{
				return true;
			}
			GameObject closestAlternatePoint = DistanceAngleChecks.GetClosestAlternatePoint(source, target);
			if (closestAlternatePoint)
			{
				angle = new float?(Mathf.Abs(source.PrimaryTargetPoint.AngleTo(closestAlternatePoint, true)));
				if (DistanceAngleChecks.WithinTargetAngle(angle.Value, angleReq, executionProgress, conalType))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x000F7CCC File Offset: 0x000F5ECC
		private static bool WithinTargetAngle(float angle, float angleReq, float? executionProgress, ConalTypes conalType)
		{
			if (executionProgress != null)
			{
				angleReq = Mathf.Lerp(angleReq, angleReq + GlobalSettings.Values.Combat.CoyoteTimeAngle * 0.5f, executionProgress.Value);
			}
			if (conalType == ConalTypes.None)
			{
				return angle <= angleReq;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (conalType.HasBitFlag(ConalTypes.Frontal))
			{
				flag = (angle <= angleReq);
			}
			if (conalType.HasBitFlag(ConalTypes.Rear))
			{
				flag2 = (180f - angleReq <= angle && angle <= 180f);
			}
			if (conalType.HasBitFlag(ConalTypes.Sides))
			{
				float num = 90f - angleReq;
				float num2 = 90f + angleReq;
				flag3 = (num <= angle && angle <= num2);
			}
			return flag || flag2 || flag3;
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x000F7D78 File Offset: 0x000F5F78
		public static bool EDITOR_MeetsAngleRequirements(GameObject source, GameObject target, float angleReq, float? angle, ConalTypes conalType)
		{
			if (!source || !target)
			{
				return false;
			}
			if (angle == null)
			{
				angle = new float?(Mathf.Abs(source.gameObject.AngleTo(target.gameObject, true)));
			}
			return DistanceAngleChecks.WithinTargetAngle(angle.Value, angleReq, new float?(0f), conalType);
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x000F7DDC File Offset: 0x000F5FDC
		private static GameObject GetClosestAlternatePoint(GameEntity source, GameEntity target)
		{
			if (!source || !target)
			{
				return null;
			}
			GameObject gameObject = source.GetCurrentAlternateTarget();
			if (gameObject)
			{
				return gameObject;
			}
			if (target.AlternateTargetPoints && target.AlternateTargetPoints.Points != null && target.AlternateTargetPoints.Points.Length != 0)
			{
				float num = float.MaxValue;
				GameObject gameObject2 = null;
				for (int i = 0; i < target.AlternateTargetPoints.Points.Length; i++)
				{
					if (target.AlternateTargetPoints.Points[i] && source.PrimaryTargetPoint.transform.InverseTransformPoint(target.AlternateTargetPoints.Points[i].transform.position).z > 0f)
					{
						float sqrMagnitude = (source.PrimaryTargetPoint.transform.position - target.AlternateTargetPoints.Points[i].transform.position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							num = sqrMagnitude;
							gameObject2 = target.AlternateTargetPoints.Points[i];
						}
					}
				}
				if (gameObject2)
				{
					gameObject = gameObject2;
					source.SetCurrentAlternateTarget(gameObject);
				}
			}
			return gameObject;
		}
	}
}
