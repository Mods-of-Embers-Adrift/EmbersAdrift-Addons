using System;
using BehaviorDesigner.Runtime;
using SoL.Game.NPCs.Interactions;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004CB RID: 1227
	public class SharedNpcInteractive : SharedVariable<NpcInteractive>
	{
		// Token: 0x0600228E RID: 8846 RVA: 0x00058DF4 File Offset: 0x00056FF4
		public static implicit operator SharedNpcInteractive(NpcInteractive value)
		{
			return new SharedNpcInteractive
			{
				Value = value
			};
		}
	}
}
