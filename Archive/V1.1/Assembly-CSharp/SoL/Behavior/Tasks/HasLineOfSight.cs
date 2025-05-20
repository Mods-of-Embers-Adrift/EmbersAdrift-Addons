using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game;
using SoL.Utilities;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004DB RID: 1243
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Has Line of Sight")]
	public class HasLineOfSight : Conditional
	{
		// Token: 0x060022BD RID: 8893 RVA: 0x00059027 File Offset: 0x00057227
		public override void OnStart()
		{
			base.OnStart();
			this.m_gameEntity = this.gameObject.transform.parent.gameObject.GetComponent<GameEntity>();
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x0005904F File Offset: 0x0005724F
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (!this.m_gameEntity || !LineOfSight.NpcHasLineOfSight(this.m_gameEntity, this.TargetEntity.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x00059080 File Offset: 0x00057280
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			if (Task.CleanupReferences)
			{
				this.m_gameEntity = null;
			}
		}

		// Token: 0x0400269A RID: 9882
		private GameEntity m_gameEntity;

		// Token: 0x0400269B RID: 9883
		public SharedGameEntity TargetEntity;
	}
}
