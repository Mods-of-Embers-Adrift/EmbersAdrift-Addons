using System;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D6 RID: 1238
	[TaskDescription("Pursue the target specified using the Unity NavMesh.")]
	[TaskCategory("SoL/Npc")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=5")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PursueIcon.png")]
	public class GameEntityPursue : Pursue
	{
		// Token: 0x060022B0 RID: 8880 RVA: 0x00058F72 File Offset: 0x00057172
		public override void OnStart()
		{
			this.target.Value = ((this.TargetEntity.Value == null) ? null : this.TargetEntity.Value.gameObject);
			base.OnStart();
		}

		// Token: 0x0400268E RID: 9870
		public SharedGameEntity TargetEntity;
	}
}
