using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004DE RID: 1246
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Is Too Close")]
	public class IsTooClose : BaseNodeConditional<NpcSkillsController>
	{
		// Token: 0x060022C5 RID: 8901 RVA: 0x00127AB0 File Offset: 0x00125CB0
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			this.MoveBackDistance.Value = 0f;
			if (!this.TargetGameObject.Value || !this.m_controller)
			{
				return TaskStatus.Failure;
			}
			float value = 0f;
			float magnitude = (this.TargetGameObject.Value.transform.position - this.transform.position).magnitude;
			float primaryTargetPointDistance = this.m_controller.PrimaryTargetPointDistance;
			WeaponItem currentWeaponItem = this.m_controller.GetCurrentWeaponItem();
			float minDistance;
			float maxDistance;
			if (currentWeaponItem && currentWeaponItem.GetWeaponType().IsRanged())
			{
				MinMaxFloatRange weaponDistance = currentWeaponItem.GetWeaponDistance();
				minDistance = weaponDistance.Min + primaryTargetPointDistance;
				maxDistance = weaponDistance.Max + primaryTargetPointDistance;
				if (weaponDistance.Min > 0f && IsTooClose.IsTooCloseForParameters(magnitude, minDistance, maxDistance, 0.9f, out value))
				{
					this.MoveBackDistance.Value = value;
					return TaskStatus.Success;
				}
			}
			float selfDistanceBuffer = this.m_controller.GetSelfDistanceBuffer();
			minDistance = selfDistanceBuffer + primaryTargetPointDistance;
			maxDistance = selfDistanceBuffer * 2f + primaryTargetPointDistance;
			if (IsTooClose.IsTooCloseForParameters(magnitude, minDistance, maxDistance, 0.2f, out value))
			{
				this.MoveBackDistance.Value = value;
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x00059096 File Offset: 0x00057296
		public override void OnReset()
		{
			base.OnReset();
			this.MoveBackDistance.Value = 0f;
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x00127BF0 File Offset: 0x00125DF0
		private static bool IsTooCloseForParameters(float distance, float minDistance, float maxDistance, float moveBackFraction, out float moveBackDistance)
		{
			moveBackDistance = 0f;
			if (distance >= minDistance)
			{
				return false;
			}
			float num = Mathf.Lerp(minDistance, maxDistance, moveBackFraction);
			moveBackDistance = num;
			return true;
		}

		// Token: 0x040026A6 RID: 9894
		private const float kDefaultMoveBackFraction = 0.2f;

		// Token: 0x040026A7 RID: 9895
		private const float kRangedMoveBackFraction = 0.9f;

		// Token: 0x040026A8 RID: 9896
		public SharedGameObject TargetGameObject;

		// Token: 0x040026A9 RID: 9897
		public SharedFloat MoveBackDistance;
	}
}
