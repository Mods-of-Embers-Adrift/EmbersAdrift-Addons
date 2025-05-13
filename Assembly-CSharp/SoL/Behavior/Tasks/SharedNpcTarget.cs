using System;
using BehaviorDesigner.Runtime;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004CD RID: 1229
	[Serializable]
	public class SharedNpcTarget : SharedVariable<NpcTarget>
	{
		// Token: 0x06002292 RID: 8850 RVA: 0x00058E20 File Offset: 0x00057020
		public static implicit operator SharedNpcTarget(NpcTarget value)
		{
			return new SharedNpcTarget
			{
				Value = value
			};
		}
	}
}
