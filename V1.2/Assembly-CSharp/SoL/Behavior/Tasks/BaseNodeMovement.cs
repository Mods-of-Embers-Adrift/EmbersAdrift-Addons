using System;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using SoL.Game;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D0 RID: 1232
	public abstract class BaseNodeMovement<T> : NavMeshMovement where T : GameEntityComponent
	{
		// Token: 0x0600229E RID: 8862 RVA: 0x00127290 File Offset: 0x00125490
		public override void OnAwake()
		{
			base.OnAwake();
			this.m_controller = this.gameObject.GetComponent<T>();
			if (this.m_controller && this.m_controller.GameEntity)
			{
				this.m_controller.GameEntity.Destroyed += this.GameEntityOnDestroyed;
			}
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x00058E6A File Offset: 0x0005706A
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			this.Cleanup();
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x00058E78 File Offset: 0x00057078
		private void GameEntityOnDestroyed()
		{
			this.Cleanup();
		}

		// Token: 0x060022A1 RID: 8865 RVA: 0x00127300 File Offset: 0x00125500
		private void Cleanup()
		{
			if (this.m_controller && this.m_controller.GameEntity)
			{
				this.m_controller.GameEntity.Destroyed -= this.GameEntityOnDestroyed;
			}
			if (Task.CleanupReferences)
			{
				this.m_controller = default(T);
			}
		}

		// Token: 0x0400267F RID: 9855
		protected T m_controller;
	}
}
