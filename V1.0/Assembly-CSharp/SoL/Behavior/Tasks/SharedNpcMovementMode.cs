using System;
using BehaviorDesigner.Runtime;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004CC RID: 1228
	[Serializable]
	public class SharedNpcMovementMode : SharedVariable<NpcMovementMode>
	{
		// Token: 0x06002290 RID: 8848 RVA: 0x00058E0A File Offset: 0x0005700A
		public static implicit operator SharedNpcMovementMode(NpcMovementMode value)
		{
			return new SharedNpcMovementMode
			{
				Value = value
			};
		}
	}
}
