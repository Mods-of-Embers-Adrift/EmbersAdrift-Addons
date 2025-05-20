using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004CF RID: 1231
	public abstract class BaseNodeConditional<T> : Conditional where T : GameEntityComponent
	{
		// Token: 0x06002299 RID: 8857 RVA: 0x00127194 File Offset: 0x00125394
		public override void OnAwake()
		{
			base.OnAwake();
			this.m_controller = this.gameObject.GetComponent<T>();
			if (this.m_controller && this.m_controller.GameEntity)
			{
				this.m_controller.GameEntity.Destroyed += this.GameEntityOnDestroyed;
			}
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x00058E4C File Offset: 0x0005704C
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			this.Cleanup();
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x00058E5A File Offset: 0x0005705A
		private void GameEntityOnDestroyed()
		{
			this.Cleanup();
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x00127204 File Offset: 0x00125404
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

		// Token: 0x0400267E RID: 9854
		protected T m_controller;
	}
}
