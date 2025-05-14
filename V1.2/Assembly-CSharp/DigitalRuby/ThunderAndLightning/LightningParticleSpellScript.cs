using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D9 RID: 217
	public class LightningParticleSpellScript : LightningSpellScript, ICollisionHandler
	{
		// Token: 0x060007BA RID: 1978 RVA: 0x000AF3D4 File Offset: 0x000AD5D4
		private void PopulateParticleLight(Light src)
		{
			src.bounceIntensity = 0f;
			src.type = LightType.Point;
			src.shadows = LightShadows.None;
			src.color = new Color(UnityEngine.Random.Range(this.ParticleLightColor1.r, this.ParticleLightColor2.r), UnityEngine.Random.Range(this.ParticleLightColor1.g, this.ParticleLightColor2.g), UnityEngine.Random.Range(this.ParticleLightColor1.b, this.ParticleLightColor2.b), 1f);
			src.cullingMask = this.ParticleLightCullingMask;
			src.intensity = UnityEngine.Random.Range(this.ParticleLightIntensity.Minimum, this.ParticleLightIntensity.Maximum);
			src.range = UnityEngine.Random.Range(this.ParticleLightRange.Minimum, this.ParticleLightRange.Maximum);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x000AF4B0 File Offset: 0x000AD6B0
		private void UpdateParticleLights()
		{
			if (!this.EnableParticleLights)
			{
				return;
			}
			int num = this.ParticleSystem.GetParticles(this.particles);
			while (this.particleLights.Count < num)
			{
				GameObject gameObject = new GameObject("LightningParticleSpellLight");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				this.PopulateParticleLight(gameObject.AddComponent<Light>());
				this.particleLights.Add(gameObject);
			}
			while (this.particleLights.Count > num)
			{
				UnityEngine.Object.Destroy(this.particleLights[this.particleLights.Count - 1]);
				this.particleLights.RemoveAt(this.particleLights.Count - 1);
			}
			for (int i = 0; i < num; i++)
			{
				this.particleLights[i].transform.position = this.particles[i].position;
			}
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x000AF58C File Offset: 0x000AD78C
		private void UpdateParticleSystems()
		{
			if (this.EmissionParticleSystem != null && this.EmissionParticleSystem.isPlaying)
			{
				this.EmissionParticleSystem.transform.position = this.SpellStart.transform.position;
				this.EmissionParticleSystem.transform.forward = this.Direction;
			}
			if (this.ParticleSystem != null)
			{
				if (this.ParticleSystem.isPlaying)
				{
					this.ParticleSystem.transform.position = this.SpellStart.transform.position;
					this.ParticleSystem.transform.forward = this.Direction;
				}
				this.UpdateParticleLights();
			}
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x000AF644 File Offset: 0x000AD844
		protected override void OnDestroy()
		{
			base.OnDestroy();
			foreach (GameObject obj in this.particleLights)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x00048341 File Offset: 0x00046541
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00048349 File Offset: 0x00046549
		protected override void Update()
		{
			base.Update();
			this.UpdateParticleSystems();
			this.collisionTimer -= LightningBoltScript.DeltaTime;
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00048369 File Offset: 0x00046569
		protected override void OnCastSpell()
		{
			if (this.ParticleSystem != null)
			{
				this.ParticleSystem.Play();
				this.UpdateParticleSystems();
			}
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x0004838A File Offset: 0x0004658A
		protected override void OnStopSpell()
		{
			if (this.ParticleSystem != null)
			{
				this.ParticleSystem.Stop();
			}
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x000AF69C File Offset: 0x000AD89C
		void ICollisionHandler.HandleCollision(GameObject obj, List<ParticleCollisionEvent> collisions, int collisionCount)
		{
			if (this.collisionTimer <= 0f)
			{
				this.collisionTimer = this.CollisionInterval;
				base.PlayCollisionSound(collisions[0].intersection);
				base.ApplyCollisionForce(collisions[0].intersection);
				if (this.CollisionCallback != null)
				{
					this.CollisionCallback(obj, collisions, collisionCount);
				}
			}
		}

		// Token: 0x04000905 RID: 2309
		[Header("Particle system")]
		public ParticleSystem ParticleSystem;

		// Token: 0x04000906 RID: 2310
		[Tooltip("Particle system collision interval. This time must elapse before another collision will be registered.")]
		public float CollisionInterval;

		// Token: 0x04000907 RID: 2311
		protected float collisionTimer;

		// Token: 0x04000908 RID: 2312
		[HideInInspector]
		public Action<GameObject, List<ParticleCollisionEvent>, int> CollisionCallback;

		// Token: 0x04000909 RID: 2313
		[Header("Particle Light Properties")]
		[Tooltip("Whether to enable point lights for the particles")]
		public bool EnableParticleLights = true;

		// Token: 0x0400090A RID: 2314
		[SingleLineClamp("Possible range for particle lights", 0.001, 100.0)]
		public RangeOfFloats ParticleLightRange = new RangeOfFloats
		{
			Minimum = 2f,
			Maximum = 5f
		};

		// Token: 0x0400090B RID: 2315
		[SingleLineClamp("Possible range of intensity for particle lights", 0.009999999776482582, 8.0)]
		public RangeOfFloats ParticleLightIntensity = new RangeOfFloats
		{
			Minimum = 0.2f,
			Maximum = 0.3f
		};

		// Token: 0x0400090C RID: 2316
		[Tooltip("Possible range of colors for particle lights")]
		public Color ParticleLightColor1 = Color.white;

		// Token: 0x0400090D RID: 2317
		[Tooltip("Possible range of colors for particle lights")]
		public Color ParticleLightColor2 = Color.white;

		// Token: 0x0400090E RID: 2318
		[Tooltip("The culling mask for particle lights")]
		public LayerMask ParticleLightCullingMask = -1;

		// Token: 0x0400090F RID: 2319
		private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[512];

		// Token: 0x04000910 RID: 2320
		private readonly List<GameObject> particleLights = new List<GameObject>();
	}
}
