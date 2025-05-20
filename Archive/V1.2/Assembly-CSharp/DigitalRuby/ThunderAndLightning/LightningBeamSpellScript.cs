using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D6 RID: 214
	public class LightningBeamSpellScript : LightningSpellScript
	{
		// Token: 0x060007B0 RID: 1968 RVA: 0x000AF22C File Offset: 0x000AD42C
		private void CheckCollision()
		{
			RaycastHit obj;
			if (Physics.Raycast(this.SpellStart.transform.position, this.Direction, out obj, this.MaxDistance, this.CollisionMask))
			{
				this.SpellEnd.transform.position = obj.point;
				this.SpellEnd.transform.position += UnityEngine.Random.insideUnitSphere * this.EndPointRandomization;
				base.PlayCollisionSound(this.SpellEnd.transform.position);
				if (this.CollisionParticleSystem != null)
				{
					this.CollisionParticleSystem.transform.position = obj.point;
					this.CollisionParticleSystem.Play();
				}
				base.ApplyCollisionForce(obj.point);
				if (this.CollisionCallback != null)
				{
					this.CollisionCallback(obj);
					return;
				}
			}
			else
			{
				if (this.CollisionParticleSystem != null)
				{
					this.CollisionParticleSystem.Stop();
				}
				this.SpellEnd.transform.position = this.SpellStart.transform.position + this.Direction * this.MaxDistance;
				this.SpellEnd.transform.position += UnityEngine.Random.insideUnitSphere * this.EndPointRandomization;
			}
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x000482C6 File Offset: 0x000464C6
		protected override void Start()
		{
			base.Start();
			this.LightningPathScript.ManualMode = true;
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x000482DA File Offset: 0x000464DA
		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (!base.Casting)
			{
				return;
			}
			this.CheckCollision();
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x000482F1 File Offset: 0x000464F1
		protected override void OnCastSpell()
		{
			this.LightningPathScript.ManualMode = false;
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x000482FF File Offset: 0x000464FF
		protected override void OnStopSpell()
		{
			this.LightningPathScript.ManualMode = true;
		}

		// Token: 0x040008FF RID: 2303
		[Header("Beam")]
		[Tooltip("The lightning path script creating the beam of lightning")]
		public LightningBoltPathScriptBase LightningPathScript;

		// Token: 0x04000900 RID: 2304
		[Tooltip("Give the end point some randomization")]
		public float EndPointRandomization = 1.5f;

		// Token: 0x04000901 RID: 2305
		[HideInInspector]
		public Action<RaycastHit> CollisionCallback;
	}
}
