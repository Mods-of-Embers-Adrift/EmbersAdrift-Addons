using System;
using System.Collections;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000DA RID: 218
	public abstract class LightningSpellScript : MonoBehaviour
	{
		// Token: 0x060007C4 RID: 1988 RVA: 0x000483A5 File Offset: 0x000465A5
		private IEnumerator StopAfterSecondsCoRoutine(float seconds)
		{
			int token = this.stopToken;
			yield return new WaitForSecondsLightning(seconds);
			if (token == this.stopToken)
			{
				this.StopSpell();
			}
			yield break;
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060007C5 RID: 1989 RVA: 0x000483BB File Offset: 0x000465BB
		// (set) Token: 0x060007C6 RID: 1990 RVA: 0x000483C3 File Offset: 0x000465C3
		private protected float DurationTimer { protected get; private set; }

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060007C7 RID: 1991 RVA: 0x000483CC File Offset: 0x000465CC
		// (set) Token: 0x060007C8 RID: 1992 RVA: 0x000483D4 File Offset: 0x000465D4
		private protected float CooldownTimer { protected get; private set; }

		// Token: 0x060007C9 RID: 1993 RVA: 0x000AF7AC File Offset: 0x000AD9AC
		protected void ApplyCollisionForce(Vector3 point)
		{
			if (this.CollisionForce > 0f && this.CollisionRadius > 0f)
			{
				Collider[] array = Physics.OverlapSphere(point, this.CollisionRadius, this.CollisionMask);
				for (int i = 0; i < array.Length; i++)
				{
					Rigidbody component = array[i].GetComponent<Rigidbody>();
					if (component != null)
					{
						if (this.CollisionIsExplosion)
						{
							component.AddExplosionForce(this.CollisionForce, point, this.CollisionRadius, this.CollisionForce * 0.02f, this.CollisionForceMode);
						}
						else
						{
							component.AddForce(this.CollisionForce * this.Direction, this.CollisionForceMode);
						}
					}
				}
			}
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x000AF860 File Offset: 0x000ADA60
		protected void PlayCollisionSound(Vector3 pos)
		{
			if (this.CollisionAudioSource != null && this.CollisionAudioClips != null && this.CollisionAudioClips.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, this.CollisionAudioClips.Length - 1);
				float volumeScale = UnityEngine.Random.Range(this.CollisionVolumeRange.Minimum, this.CollisionVolumeRange.Maximum);
				this.CollisionAudioSource.transform.position = pos;
				this.CollisionAudioSource.PlayOneShot(this.CollisionAudioClips[num], volumeScale);
			}
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x000483DD File Offset: 0x000465DD
		protected virtual void Start()
		{
			if (this.EmissionLight != null)
			{
				this.EmissionLight.enabled = false;
			}
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x000483F9 File Offset: 0x000465F9
		protected virtual void Update()
		{
			this.CooldownTimer = Mathf.Max(0f, this.CooldownTimer - LightningBoltScript.DeltaTime);
			this.DurationTimer = Mathf.Max(0f, this.DurationTimer - LightningBoltScript.DeltaTime);
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LateUpdate()
		{
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnDestroy()
		{
		}

		// Token: 0x060007CF RID: 1999
		protected abstract void OnCastSpell();

		// Token: 0x060007D0 RID: 2000
		protected abstract void OnStopSpell();

		// Token: 0x060007D1 RID: 2001 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnActivated()
		{
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnDeactivated()
		{
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x000AF8E0 File Offset: 0x000ADAE0
		public bool CastSpell()
		{
			if (!this.CanCastSpell)
			{
				return false;
			}
			this.Casting = true;
			this.DurationTimer = this.Duration;
			this.CooldownTimer = this.Cooldown;
			this.OnCastSpell();
			if (this.Duration > 0f)
			{
				this.StopAfterSeconds(this.Duration);
			}
			if (this.EmissionParticleSystem != null)
			{
				this.EmissionParticleSystem.Play();
			}
			if (this.EmissionLight != null)
			{
				this.EmissionLight.transform.position = this.SpellStart.transform.position;
				this.EmissionLight.enabled = true;
			}
			if (this.EmissionSound != null)
			{
				this.EmissionSound.Play();
			}
			return true;
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x000AF9A4 File Offset: 0x000ADBA4
		public void StopSpell()
		{
			if (this.Casting)
			{
				this.stopToken++;
				if (this.EmissionParticleSystem != null)
				{
					this.EmissionParticleSystem.Stop();
				}
				if (this.EmissionLight != null)
				{
					this.EmissionLight.enabled = false;
				}
				if (this.EmissionSound != null && this.EmissionSound.loop)
				{
					this.EmissionSound.Stop();
				}
				this.DurationTimer = 0f;
				this.Casting = false;
				this.OnStopSpell();
			}
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00048433 File Offset: 0x00046633
		public void ActivateSpell()
		{
			this.OnActivated();
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0004843B File Offset: 0x0004663B
		public void DeactivateSpell()
		{
			this.OnDeactivated();
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x00048443 File Offset: 0x00046643
		public void StopAfterSeconds(float seconds)
		{
			base.StartCoroutine(this.StopAfterSecondsCoRoutine(seconds));
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x000AFA38 File Offset: 0x000ADC38
		public static GameObject FindChildRecursively(Transform t, string name)
		{
			if (t.name == name)
			{
				return t.gameObject;
			}
			for (int i = 0; i < t.childCount; i++)
			{
				GameObject gameObject = LightningSpellScript.FindChildRecursively(t.GetChild(i), name);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
			return null;
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x00048453 File Offset: 0x00046653
		// (set) Token: 0x060007DA RID: 2010 RVA: 0x0004845B File Offset: 0x0004665B
		public bool Casting { get; private set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x00048464 File Offset: 0x00046664
		public bool CanCastSpell
		{
			get
			{
				return !this.Casting && this.CooldownTimer <= 0f;
			}
		}

		// Token: 0x04000911 RID: 2321
		[Header("Direction and distance")]
		[Tooltip("The start point of the spell. Set this to a muzzle end or hand.")]
		public GameObject SpellStart;

		// Token: 0x04000912 RID: 2322
		[Tooltip("The end point of the spell. Set this to an empty game object. This will change depending on things like collisions, randomness, etc. Not all spells need an end object, but create this anyway to be sure.")]
		public GameObject SpellEnd;

		// Token: 0x04000913 RID: 2323
		[HideInInspector]
		[Tooltip("The direction of the spell. Should be normalized. Does not change unless explicitly modified.")]
		public Vector3 Direction;

		// Token: 0x04000914 RID: 2324
		[Tooltip("The maximum distance of the spell")]
		public float MaxDistance = 15f;

		// Token: 0x04000915 RID: 2325
		[Header("Collision")]
		[Tooltip("Whether the collision is an exploision. If not explosion, collision is directional.")]
		public bool CollisionIsExplosion;

		// Token: 0x04000916 RID: 2326
		[Tooltip("The radius of the collision explosion")]
		public float CollisionRadius = 1f;

		// Token: 0x04000917 RID: 2327
		[Tooltip("The force to explode with when there is a collision")]
		public float CollisionForce = 50f;

		// Token: 0x04000918 RID: 2328
		[Tooltip("Collision force mode")]
		public ForceMode CollisionForceMode = ForceMode.Impulse;

		// Token: 0x04000919 RID: 2329
		[Tooltip("The particle system for collisions. For best effects, this should emit particles in bursts at time 0 and not loop.")]
		public ParticleSystem CollisionParticleSystem;

		// Token: 0x0400091A RID: 2330
		[Tooltip("The layers that the spell should collide with")]
		public LayerMask CollisionMask = -1;

		// Token: 0x0400091B RID: 2331
		[Tooltip("Collision audio source")]
		public AudioSource CollisionAudioSource;

		// Token: 0x0400091C RID: 2332
		[Tooltip("Collision audio clips. One will be chosen at random and played one shot with CollisionAudioSource.")]
		public AudioClip[] CollisionAudioClips;

		// Token: 0x0400091D RID: 2333
		[Tooltip("Collision sound volume range.")]
		public RangeOfFloats CollisionVolumeRange = new RangeOfFloats
		{
			Minimum = 0.4f,
			Maximum = 0.6f
		};

		// Token: 0x0400091E RID: 2334
		[Header("Duration and Cooldown")]
		[Tooltip("The duration in seconds that the spell will last. Not all spells support a duration. For one shot spells, this is how long the spell cast / emission light, etc. will last.")]
		public float Duration;

		// Token: 0x0400091F RID: 2335
		[Tooltip("The cooldown in seconds. Once cast, the spell must wait for the cooldown before being cast again.")]
		public float Cooldown;

		// Token: 0x04000920 RID: 2336
		[Header("Emission")]
		[Tooltip("Emission sound")]
		public AudioSource EmissionSound;

		// Token: 0x04000921 RID: 2337
		[Tooltip("Emission particle system. For best results use world space, turn off looping and play on awake.")]
		public ParticleSystem EmissionParticleSystem;

		// Token: 0x04000922 RID: 2338
		[Tooltip("Light to illuminate when spell is cast")]
		public Light EmissionLight;

		// Token: 0x04000923 RID: 2339
		private int stopToken;
	}
}
