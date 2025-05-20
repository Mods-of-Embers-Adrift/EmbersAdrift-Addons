using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004DD RID: 1245
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Initialize NPCs")]
	public class InitializeNpc : BehaviorDesigner.Runtime.Tasks.Action
	{
		// Token: 0x060022C3 RID: 8899 RVA: 0x00127A48 File Offset: 0x00125C48
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (!this.m_initialized)
			{
				this.InitialPosition.Value = this.gameObject.transform.position;
				this.InitialRotation.Value = this.gameObject.transform.eulerAngles;
				this.m_initialized = true;
				this.IsInitialized.Value = true;
			}
			return TaskStatus.Success;
		}

		// Token: 0x040026A0 RID: 9888
		public SharedVector3 InitialPosition;

		// Token: 0x040026A1 RID: 9889
		public SharedVector3 InitialRotation;

		// Token: 0x040026A2 RID: 9890
		public SharedFloat LeashDistance;

		// Token: 0x040026A3 RID: 9891
		public SharedFloat ResetDistance;

		// Token: 0x040026A4 RID: 9892
		private bool m_initialized;

		// Token: 0x040026A5 RID: 9893
		public SharedBool IsInitialized;
	}
}
