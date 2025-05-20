using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004DC RID: 1244
	[TaskDescription("Wander using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WanderIcon.png")]
	public class Hunt : Wander
	{
		// Token: 0x060022C1 RID: 8897 RVA: 0x001277E0 File Offset: 0x001259E0
		protected override bool TrySetTarget()
		{
			if (Hunt.m_entityQueries == null)
			{
				Hunt.m_entityQueries = new List<NetworkEntity>(100);
			}
			ServerGameManager.SpatialManager.PhysicsQueryRadius(this.gameObject.transform.position, this.MaxQueryRange.Value, Hunt.m_entityQueries, true, null);
			if (Hunt.m_entityQueries.Count <= 0)
			{
				return base.TrySetTarget();
			}
			NetworkEntity networkEntity = null;
			float num = float.MaxValue;
			Vector3 position = this.transform.position;
			for (int i = 0; i < Hunt.m_entityQueries.Count; i++)
			{
				NetworkEntity networkEntity2 = Hunt.m_entityQueries[i];
				if (networkEntity2 && networkEntity2.GameEntity && networkEntity2.GameEntity.Type == GameEntityType.Player && networkEntity2.GameEntity.Vitals && networkEntity2.GameEntity.Vitals.GetCurrentHealthState() == HealthState.Alive && networkEntity2.GameEntity.CharacterData && !networkEntity2.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.NoTarget))
				{
					float sqrMagnitude = (networkEntity2.gameObject.transform.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						networkEntity = networkEntity2;
					}
				}
			}
			if (networkEntity)
			{
				bool flag = false;
				Vector3 vector = position;
				Vector2 vector2 = UnityEngine.Random.insideUnitCircle * this.RandomTargetPosition.Value;
				Vector3 b = networkEntity.transform.position + new Vector3(vector2.x, 0f, vector2.y);
				int num2 = this.targetRetries.Value;
				while (!flag && num2 > 0)
				{
					float t = UnityEngine.Random.Range(this.RandomLerpRange.Value.x, this.RandomLerpRange.Value.y);
					vector = Vector3.Lerp(position, b, t);
					flag = base.SamplePosition(vector);
					num2--;
				}
				if (flag)
				{
					this.SetDestination(vector);
				}
				return flag;
			}
			return base.TrySetTarget();
		}

		// Token: 0x0400269C RID: 9884
		private static List<NetworkEntity> m_entityQueries;

		// Token: 0x0400269D RID: 9885
		public SharedFloat MaxQueryRange = 100f;

		// Token: 0x0400269E RID: 9886
		public SharedFloat RandomTargetPosition = 5f;

		// Token: 0x0400269F RID: 9887
		public SharedVector2 RandomLerpRange = new Vector2(0.4f, 0.6f);
	}
}
