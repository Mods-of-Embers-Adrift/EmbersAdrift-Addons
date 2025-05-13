using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D8 RID: 216
	[RequireComponent(typeof(ParticleSystem))]
	public class LightningParticleCollisionForwarder : MonoBehaviour
	{
		// Token: 0x060007B7 RID: 1975 RVA: 0x00048320 File Offset: 0x00046520
		private void Start()
		{
			this._particleSystem = base.GetComponent<ParticleSystem>();
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x000AF374 File Offset: 0x000AD574
		private void OnParticleCollision(GameObject other)
		{
			ICollisionHandler collisionHandler = this.CollisionHandler as ICollisionHandler;
			if (collisionHandler != null)
			{
				int num = this._particleSystem.GetCollisionEvents(other, this.collisionEvents);
				if (num != 0)
				{
					collisionHandler.HandleCollision(other, this.collisionEvents, num);
				}
			}
		}

		// Token: 0x04000902 RID: 2306
		[Tooltip("The script to forward the collision to. Must implement ICollisionHandler.")]
		public MonoBehaviour CollisionHandler;

		// Token: 0x04000903 RID: 2307
		private ParticleSystem _particleSystem;

		// Token: 0x04000904 RID: 2308
		private readonly List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
	}
}
