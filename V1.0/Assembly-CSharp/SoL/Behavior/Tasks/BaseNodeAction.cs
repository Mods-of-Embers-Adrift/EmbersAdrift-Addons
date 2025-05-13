using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004CE RID: 1230
	public abstract class BaseNodeAction<T> : BehaviorDesigner.Runtime.Tasks.Action where T : GameEntityComponent
	{
		// Token: 0x06002294 RID: 8852 RVA: 0x001270B8 File Offset: 0x001252B8
		public override void OnAwake()
		{
			base.OnAwake();
			this.m_controller = this.gameObject.GetComponent<T>();
			if (this.m_controller && this.m_controller.GameEntity)
			{
				this.m_controller.GameEntity.Destroyed += this.GameEntityOnDestroyed;
			}
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x00058E36 File Offset: 0x00057036
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			this.Cleanup();
		}

		// Token: 0x06002296 RID: 8854 RVA: 0x00058E44 File Offset: 0x00057044
		private void GameEntityOnDestroyed()
		{
			this.Cleanup();
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x00127128 File Offset: 0x00125328
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

		// Token: 0x0400267D RID: 9853
		protected T m_controller;
	}
}
