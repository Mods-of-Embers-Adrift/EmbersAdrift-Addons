using System;
using SoL.Game;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000294 RID: 660
	public static class LineOfSight
	{
		// Token: 0x06001411 RID: 5137 RVA: 0x000500E2 File Offset: 0x0004E2E2
		public static bool PlayerHasLineOfSight(GameEntity source, GameEntity target)
		{
			return source && LineOfSight.PlayerHasLineOfSight(source.gameObject.transform.position + Vector3.up * 1.4f, target);
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x00050118 File Offset: 0x0004E318
		public static bool CameraHasLineOfSight(Camera camera, GameEntity target)
		{
			return camera && LineOfSight.PlayerHasLineOfSight(camera.gameObject.transform.position, target);
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x000F91F4 File Offset: 0x000F73F4
		private static bool PlayerHasLineOfSight(Vector3 sourcePos, GameEntity target)
		{
			if (!target)
			{
				return false;
			}
			Vector3 position = target.gameObject.transform.position;
			float y = position.y + 1f;
			float num = position.y + 0.1f;
			Vector3 vector = position;
			if (target.UpperLineOfSightTarget)
			{
				float num2 = target.UpperLineOfSightTarget.gameObject.transform.position.y + 0.1f;
				vector.y = num2;
				Vector3 target2 = target.UpperLineOfSightTarget.UseFullPosition ? target.UpperLineOfSightTarget.gameObject.transform.position : vector;
				if (LineOfSight.HasLineOfSight(sourcePos, target2))
				{
					return true;
				}
				if (target.Type != GameEntityType.Player || !target.Vitals || target.Vitals.GetCurrentHealthState() != HealthState.Unconscious)
				{
					y = num + (num2 - num) * 0.5f;
				}
			}
			vector.y = y;
			if (LineOfSight.HasLineOfSight(sourcePos, vector))
			{
				return true;
			}
			vector.y = num;
			return LineOfSight.HasLineOfSight(sourcePos, vector);
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x000F92FC File Offset: 0x000F74FC
		public static bool NpcHasLineOfSight(GameEntity source, GameEntity target)
		{
			if (!source || !target)
			{
				return false;
			}
			Vector3 source2 = source.gameObject.transform.position + Vector3.up * 1f;
			Vector3 target2 = target.gameObject.transform.position + Vector3.up * 1f;
			return LineOfSight.HasLineOfSight(source2, target2);
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x000F936C File Offset: 0x000F756C
		public static bool AoeHasLineOfSight(Vector3 sourcePos, GameEntity target)
		{
			if (!target)
			{
				return false;
			}
			sourcePos += Vector3.up * 1f;
			Vector3 target2 = target.gameObject.transform.position + Vector3.up * 1f;
			return LineOfSight.HasLineOfSight(sourcePos, target2);
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0005013A File Offset: 0x0004E33A
		public static bool HasLineOfSight(Vector3 source, Vector3 target)
		{
			return LineOfSight.HasLineOfSight(source, target, target - source);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0005014A File Offset: 0x0004E34A
		public static bool HasLineOfSight(Vector3 source, Vector3 target, Vector3 direction)
		{
			return !Physics.Raycast(source, direction, direction.magnitude, ~GlobalSettings.Values.General.LineOfSightExclusionLayerMask, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x04001C66 RID: 7270
		public const float kPlayerSourceOffset = 1.4f;

		// Token: 0x04001C67 RID: 7271
		public const float kBottomOffset = 0.1f;

		// Token: 0x04001C68 RID: 7272
		public const float kMiddleOffset = 1f;

		// Token: 0x04001C69 RID: 7273
		public const float kTopOffset = 0.1f;

		// Token: 0x04001C6A RID: 7274
		public const float kNpcOffset = 1f;

		// Token: 0x04001C6B RID: 7275
		private const float kAoeOffset = 1f;
	}
}
