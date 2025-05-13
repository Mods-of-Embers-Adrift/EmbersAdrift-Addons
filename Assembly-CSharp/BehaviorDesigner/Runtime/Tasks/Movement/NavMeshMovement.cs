using System;
using SoL.Game;
using SoL.Game.NPCs;
using SoL.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x0200016F RID: 367
	public abstract class NavMeshMovement : Movement
	{
		// Token: 0x06000C52 RID: 3154 RVA: 0x0004AF0E File Offset: 0x0004910E
		public override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06000C53 RID: 3155 RVA: 0x000D094C File Offset: 0x000CEB4C
		protected NavMeshAgent navMeshAgent
		{
			get
			{
				NpcMotor npcMotor;
				if (!this._navMeshAgent && this.gameObject && this.gameObject.TryGetComponent<NpcMotor>(out npcMotor))
				{
					this._navMeshAgent = npcMotor.UnityNavAgent;
				}
				return this._navMeshAgent;
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x000D0994 File Offset: 0x000CEB94
		protected GameEntity gameEntity
		{
			get
			{
				if (!this._cachedEntity && this._entity == null)
				{
					this._entity = this.gameObject.transform.parent.gameObject.GetComponent<GameEntity>();
					this._cachedEntity = true;
				}
				return this._entity;
			}
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x0004475B File Offset: 0x0004295B
		protected void UpdateSpeed()
		{
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x0004AF16 File Offset: 0x00049116
		protected bool ShouldInterrupt()
		{
			return this.Interrupt != null && this.Interrupt.Value;
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x0004475B File Offset: 0x0004295B
		public override void OnAwake()
		{
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x000D09E4 File Offset: 0x000CEBE4
		public override void OnStart()
		{
			if (!this.navMeshAgent)
			{
				return;
			}
			this.navMeshAgent.angularSpeed = this.angularSpeed.Value;
			this.navMeshAgent.isStopped = false;
			this.UpdateRotation(this.updateRotation.Value);
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x000D0A34 File Offset: 0x000CEC34
		protected override bool SetDestination(Vector3 destination)
		{
			if (!this.navMeshAgent)
			{
				return false;
			}
			this.navMeshAgent.isStopped = false;
			if (ServerGameManager.GameServerConfig == null || !ServerGameManager.GameServerConfig.CachePathfindingDestination)
			{
				return this.navMeshAgent.SetDestination(destination);
			}
			if (destination.Equals(this.m_previousDestination))
			{
				return true;
			}
			if (this.navMeshAgent.SetDestination(destination))
			{
				this.m_previousDestination = destination;
				return true;
			}
			return false;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x0004AF2D File Offset: 0x0004912D
		protected override void UpdateRotation(bool update)
		{
			if (this.navMeshAgent)
			{
				this.navMeshAgent.updateRotation = update;
			}
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0004AF48 File Offset: 0x00049148
		protected override bool HasPath()
		{
			return this.navMeshAgent && this.navMeshAgent.hasPath && this.navMeshAgent.remainingDistance > this.arriveDistance.Value;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x0004AF7E File Offset: 0x0004917E
		protected override Vector3 Velocity()
		{
			if (!this.navMeshAgent)
			{
				return Vector3.zero;
			}
			return this.navMeshAgent.velocity;
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x000D0AA8 File Offset: 0x000CECA8
		protected bool SamplePosition(Vector3 position)
		{
			NavMeshHit navMeshHit;
			return NavMesh.SamplePosition(position, out navMeshHit, 25f, -1);
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x000D0AC4 File Offset: 0x000CECC4
		protected override bool HasArrived()
		{
			if (!this.navMeshAgent)
			{
				return false;
			}
			float num;
			if (this.navMeshAgent.pathPending)
			{
				num = float.PositiveInfinity;
			}
			else
			{
				num = this.navMeshAgent.remainingDistance;
			}
			return num <= this.arriveDistance.Value;
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x0004AF9E File Offset: 0x0004919E
		protected override void Stop()
		{
			this.UpdateRotation(this.updateRotation.Value);
			if (this.navMeshAgent && this.navMeshAgent.hasPath)
			{
				this.navMeshAgent.isStopped = true;
			}
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x0004AFD7 File Offset: 0x000491D7
		public override void OnEnd()
		{
			if (this.stopOnTaskEnd.Value)
			{
				this.Stop();
				return;
			}
			this.UpdateRotation(this.updateRotation.Value);
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x0004AFFE File Offset: 0x000491FE
		public override void OnBehaviorComplete()
		{
			this.Stop();
			if (Task.CleanupReferences)
			{
				this._navMeshAgent = null;
				this._entity = null;
			}
			base.OnBehaviorComplete();
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x000D0B14 File Offset: 0x000CED14
		public override void OnReset()
		{
			this.speed = 10f;
			this.angularSpeed = 120f;
			this.arriveDistance = 1f;
			this.stopOnTaskEnd = true;
			this.speedModifier = 0f;
		}

		// Token: 0x04000BAC RID: 2988
		[Tooltip("Speed adjustment modifier")]
		public SharedFloat speedModifier = 0f;

		// Token: 0x04000BAD RID: 2989
		[Tooltip("The speed of the agent")]
		public SharedFloat speed = 10f;

		// Token: 0x04000BAE RID: 2990
		[Tooltip("The angular speed of the agent")]
		public SharedFloat angularSpeed = 120f;

		// Token: 0x04000BAF RID: 2991
		[Tooltip("The agent has arrived when the destination is less than the specified amount")]
		public SharedFloat arriveDistance = 0.2f;

		// Token: 0x04000BB0 RID: 2992
		[Tooltip("Should the NavMeshAgent be stopped when the task ends?")]
		public SharedBool stopOnTaskEnd = true;

		// Token: 0x04000BB1 RID: 2993
		[Tooltip("Should the NavMeshAgent rotation be updated when the task ends?")]
		public SharedBool updateRotation = true;

		// Token: 0x04000BB2 RID: 2994
		public SharedBool Interrupt = false;

		// Token: 0x04000BB3 RID: 2995
		private Vector3 m_previousDestination;

		// Token: 0x04000BB4 RID: 2996
		private NavMeshAgent _navMeshAgent;

		// Token: 0x04000BB5 RID: 2997
		private bool _cachedEntity;

		// Token: 0x04000BB6 RID: 2998
		private GameEntity _entity;
	}
}
