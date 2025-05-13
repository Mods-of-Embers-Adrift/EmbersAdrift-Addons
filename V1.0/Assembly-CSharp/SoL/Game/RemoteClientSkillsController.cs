using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005C7 RID: 1479
	public class RemoteClientSkillsController : SkillsController
	{
		// Token: 0x06002F25 RID: 12069 RVA: 0x0006084B File Offset: 0x0005EA4B
		protected override void Awake()
		{
			base.Awake();
			this.m_pending = new SkillsController.PendingExecution(this);
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x0006085F File Offset: 0x0005EA5F
		private void Update()
		{
			SkillsController.PendingExecution pending = this.m_pending;
			if (pending == null)
			{
				return;
			}
			pending.UpdateDelayedAnimation();
		}

		// Token: 0x06002F27 RID: 12071 RVA: 0x00155780 File Offset: 0x00153980
		public override void Client_Execute_Instant(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
			IExecutable executable;
			if (!InternalGameDatabase.Archetypes.TryGetAsType<IExecutable>(archetypeId, out executable))
			{
				Debug.LogError("Unable to find an IExecutable for " + archetypeId.ToString() + "??");
				return;
			}
			this.m_pending.InitRemoteClient(archetypeId, executable, targetEntity, abilityLevel, AlchemyPowerLevel.None, true);
			this.m_pending.Client_Execution_Begin(executable, targetEntity);
			this.m_pending.Status = SkillsController.PendingExecution.PendingStatus.CompleteReceived;
		}

		// Token: 0x06002F28 RID: 12072 RVA: 0x001557E8 File Offset: 0x001539E8
		public override void Client_Execution_Begin(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel, AlchemyPowerLevel alchemyPowerLevel)
		{
			IExecutable executable;
			if (!InternalGameDatabase.Archetypes.TryGetAsType<IExecutable>(archetypeId, out executable))
			{
				Debug.LogError("Unable to find an IExecutable for " + archetypeId.ToString() + "??");
				return;
			}
			this.m_pending.InitRemoteClient(archetypeId, executable, targetEntity, abilityLevel, alchemyPowerLevel, false);
			this.m_pending.Client_Execution_Begin(executable, targetEntity);
			base.OnPendingExecutionChanged(this.m_pending);
		}

		// Token: 0x06002F29 RID: 12073 RVA: 0x00060871 File Offset: 0x0005EA71
		public override void Client_Execution_Cancelled(UniqueId archetypeId)
		{
			if (this.m_pending.ArchetypeId == archetypeId)
			{
				this.m_pending.Status = SkillsController.PendingExecution.PendingStatus.CancelReceived;
				base.OnPendingExecutionChanged(this.m_pending);
				this.m_pending.Reset();
			}
		}

		// Token: 0x06002F2A RID: 12074 RVA: 0x00155854 File Offset: 0x00153A54
		public override void Client_Execution_Complete(UniqueId archetypeId, NetworkEntity updatedTargetEntity)
		{
			if (this.m_pending.ArchetypeId == archetypeId)
			{
				if (updatedTargetEntity)
				{
					this.m_pending.UpdateTargetNetworkEntity(updatedTargetEntity);
				}
				this.m_pending.Status = SkillsController.PendingExecution.PendingStatus.CompleteReceived;
				base.OnPendingExecutionChanged(this.m_pending);
			}
		}
	}
}
