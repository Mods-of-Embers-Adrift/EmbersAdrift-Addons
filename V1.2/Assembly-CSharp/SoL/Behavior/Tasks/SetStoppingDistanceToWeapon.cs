using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;
using UnityEngine;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E7 RID: 1255
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Set stopping distance to weapon distance")]
	public class SetStoppingDistanceToWeapon : BaseNodeAction<NpcSkillsController>
	{
		// Token: 0x060022DD RID: 8925 RVA: 0x00127FA4 File Offset: 0x001261A4
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			float num;
			if (this.m_controller && this.m_controller.TryGetWeaponStoppingDistance(out num))
			{
				num *= this.m_fraction;
			}
			else
			{
				num = this.m_defaultDistance;
			}
			this.StoppingDistance.Value = Mathf.Max(this.m_defaultDistance, num);
			return TaskStatus.Success;
		}

		// Token: 0x040026BA RID: 9914
		public SharedFloat StoppingDistance;

		// Token: 0x040026BB RID: 9915
		[Range(0f, 1f)]
		[SerializeField]
		private float m_fraction = 0.9f;

		// Token: 0x040026BC RID: 9916
		[SerializeField]
		private float m_defaultDistance = 1f;
	}
}
