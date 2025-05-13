using System;
using System.Collections;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000DC RID: 220
	public class LightningWhipSpell : LightningSpellScript
	{
		// Token: 0x060007E3 RID: 2019 RVA: 0x00048497 File Offset: 0x00046697
		private IEnumerator WhipForward()
		{
			for (int i = 0; i < this.WhipStart.transform.childCount; i++)
			{
				Rigidbody component = this.WhipStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.drag = 0f;
					component.velocity = Vector3.zero;
					component.angularVelocity = Vector3.zero;
				}
			}
			this.WhipSpring.SetActive(true);
			Vector3 position = this.WhipStart.GetComponent<Rigidbody>().position;
			RaycastHit raycastHit;
			Vector3 whipPositionForwards;
			Vector3 position2;
			if (Physics.Raycast(position, this.Direction, out raycastHit, this.MaxDistance, this.CollisionMask))
			{
				Vector3 normalized = (raycastHit.point - position).normalized;
				whipPositionForwards = position + normalized * this.MaxDistance;
				position2 = position - normalized * 25f;
			}
			else
			{
				whipPositionForwards = position + this.Direction * this.MaxDistance;
				position2 = position - this.Direction * 25f;
			}
			this.WhipSpring.GetComponent<Rigidbody>().position = position2;
			yield return new WaitForSecondsLightning(0.25f);
			this.WhipSpring.GetComponent<Rigidbody>().position = whipPositionForwards;
			yield return new WaitForSecondsLightning(0.1f);
			if (this.WhipCrackAudioSource != null)
			{
				this.WhipCrackAudioSource.Play();
			}
			yield return new WaitForSecondsLightning(0.1f);
			if (this.CollisionParticleSystem != null)
			{
				this.CollisionParticleSystem.Play();
			}
			base.ApplyCollisionForce(this.SpellEnd.transform.position);
			this.WhipSpring.SetActive(false);
			if (this.CollisionCallback != null)
			{
				this.CollisionCallback(this.SpellEnd.transform.position);
			}
			yield return new WaitForSecondsLightning(0.1f);
			for (int j = 0; j < this.WhipStart.transform.childCount; j++)
			{
				Rigidbody component2 = this.WhipStart.transform.GetChild(j).gameObject.GetComponent<Rigidbody>();
				if (component2 != null)
				{
					component2.velocity = Vector3.zero;
					component2.angularVelocity = Vector3.zero;
					component2.drag = 0.5f;
				}
			}
			yield break;
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x000484A6 File Offset: 0x000466A6
		protected override void Start()
		{
			base.Start();
			this.WhipSpring.SetActive(false);
			this.WhipHandle.SetActive(false);
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x000AFB48 File Offset: 0x000ADD48
		protected override void Update()
		{
			base.Update();
			base.gameObject.transform.position = this.AttachTo.transform.position;
			base.gameObject.transform.rotation = this.RotateWith.transform.rotation;
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x000484C6 File Offset: 0x000466C6
		protected override void OnCastSpell()
		{
			base.StartCoroutine(this.WhipForward());
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void OnStopSpell()
		{
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x000484D5 File Offset: 0x000466D5
		protected override void OnActivated()
		{
			base.OnActivated();
			this.WhipHandle.SetActive(true);
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x000484E9 File Offset: 0x000466E9
		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			this.WhipHandle.SetActive(false);
		}

		// Token: 0x0400092C RID: 2348
		[Header("Whip")]
		[Tooltip("Attach the whip to what object")]
		public GameObject AttachTo;

		// Token: 0x0400092D RID: 2349
		[Tooltip("Rotate the whip with this object")]
		public GameObject RotateWith;

		// Token: 0x0400092E RID: 2350
		[Tooltip("Whip handle")]
		public GameObject WhipHandle;

		// Token: 0x0400092F RID: 2351
		[Tooltip("Whip start")]
		public GameObject WhipStart;

		// Token: 0x04000930 RID: 2352
		[Tooltip("Whip spring")]
		public GameObject WhipSpring;

		// Token: 0x04000931 RID: 2353
		[Tooltip("Whip crack audio source")]
		public AudioSource WhipCrackAudioSource;

		// Token: 0x04000932 RID: 2354
		[HideInInspector]
		public Action<Vector3> CollisionCallback;
	}
}
