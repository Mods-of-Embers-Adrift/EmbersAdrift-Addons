using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D7 RID: 215
	public interface ICollisionHandler
	{
		// Token: 0x060007B6 RID: 1974
		void HandleCollision(GameObject obj, List<ParticleCollisionEvent> collision, int collisionCount);
	}
}
