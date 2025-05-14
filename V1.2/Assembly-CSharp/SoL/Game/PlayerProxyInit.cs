using System;
using SoL.Networking.Database;

namespace SoL.Game
{
	// Token: 0x020005B2 RID: 1458
	public class PlayerProxyInit : GameEntityComponent
	{
		// Token: 0x06002E26 RID: 11814 RVA: 0x001517D8 File Offset: 0x0014F9D8
		private void Start()
		{
			if (base.GameEntity != null && base.GameEntity != null && base.GameEntity.EffectController != null)
			{
				base.GameEntity.EffectController.Init(new CharacterRecord());
			}
		}
	}
}
