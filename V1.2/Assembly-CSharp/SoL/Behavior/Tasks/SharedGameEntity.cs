using System;
using BehaviorDesigner.Runtime;
using SoL.Game;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004CA RID: 1226
	[Serializable]
	public class SharedGameEntity : SharedVariable<GameEntity>
	{
		// Token: 0x0600228C RID: 8844 RVA: 0x00058DDE File Offset: 0x00056FDE
		public static implicit operator SharedGameEntity(GameEntity value)
		{
			return new SharedGameEntity
			{
				Value = value
			};
		}
	}
}
