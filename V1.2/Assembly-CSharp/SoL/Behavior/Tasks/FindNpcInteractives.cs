using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game;
using SoL.Game.NPCs.Interactions;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D5 RID: 1237
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Find Interactives")]
	public class FindNpcInteractives : BaseNodeAction<ServerNpcController>
	{
		// Token: 0x060022AD RID: 8877 RVA: 0x00127464 File Offset: 0x00125664
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (this.m_controller.InteractionFlags == NpcInteractionFlags.None)
			{
				return TaskStatus.Failure;
			}
			if (this.Interactive.Value)
			{
				this.m_controller.Interactive = this.Interactive.Value;
				this.m_timeOfNextCheck = Time.time + 5f;
				this.SetPosition();
				return TaskStatus.Success;
			}
			if (Time.time >= this.m_timeOfNextCheck)
			{
				RaycastHit[] hits = Hits.Hits100;
				int num = Physics.SphereCastNonAlloc(this.gameObject.transform.position, 20f, this.gameObject.transform.forward, hits, 100f, LayerMap.Interaction.LayerMask);
				for (int i = 0; i < num; i++)
				{
					NpcInteractive npcInteractive;
					if (NpcInteractive.TryGetNpcInteractiveForCollider(hits[i].collider, out npcInteractive) && npcInteractive.EntityCanInteract(this.m_controller.GameEntity))
					{
						this.Interactive.Value = npcInteractive;
						break;
					}
				}
				this.m_timeOfNextCheck = Time.time + 5f;
			}
			this.SetPosition();
			this.m_controller.Interactive = this.Interactive.Value;
			if (!this.Interactive.Value)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x00058F31 File Offset: 0x00057131
		private void SetPosition()
		{
			if (this.Interactive.Value)
			{
				this.InteractivePosition.Value = this.Interactive.Value.gameObject.transform.position;
			}
		}

		// Token: 0x04002689 RID: 9865
		private const float kTimeBetweenChecks = 5f;

		// Token: 0x0400268A RID: 9866
		private const float kInteractionRange = 20f;

		// Token: 0x0400268B RID: 9867
		public SharedNpcInteractive Interactive;

		// Token: 0x0400268C RID: 9868
		public SharedVector3 InteractivePosition;

		// Token: 0x0400268D RID: 9869
		private float m_timeOfNextCheck;
	}
}
