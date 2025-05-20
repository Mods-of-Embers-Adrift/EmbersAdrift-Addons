using System;
using System.Collections;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D2 RID: 210
	[RequireComponent(typeof(AudioSource))]
	public class LightningWhipScript : MonoBehaviour
	{
		// Token: 0x0600078E RID: 1934 RVA: 0x000481EF File Offset: 0x000463EF
		private IEnumerator WhipForward()
		{
			if (this.canWhip)
			{
				this.canWhip = false;
				for (int i = 0; i < this.whipStart.transform.childCount; i++)
				{
					Rigidbody2D component = this.whipStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>();
					if (component != null)
					{
						component.drag = 0f;
					}
				}
				this.audioSource.PlayOneShot(this.WhipCrack);
				this.whipSpring.GetComponent<SpringJoint2D>().enabled = true;
				this.whipSpring.GetComponent<Rigidbody2D>().position = this.whipHandle.GetComponent<Rigidbody2D>().position + new Vector2(-15f, 5f);
				yield return new WaitForSecondsLightning(0.2f);
				this.whipSpring.GetComponent<Rigidbody2D>().position = this.whipHandle.GetComponent<Rigidbody2D>().position + new Vector2(15f, 2.5f);
				yield return new WaitForSecondsLightning(0.15f);
				this.audioSource.PlayOneShot(this.WhipCrackThunder, 0.5f);
				yield return new WaitForSecondsLightning(0.15f);
				this.whipEndStrike.GetComponent<ParticleSystem>().Play();
				this.whipSpring.GetComponent<SpringJoint2D>().enabled = false;
				yield return new WaitForSecondsLightning(0.65f);
				for (int j = 0; j < this.whipStart.transform.childCount; j++)
				{
					Rigidbody2D component2 = this.whipStart.transform.GetChild(j).gameObject.GetComponent<Rigidbody2D>();
					if (component2 != null)
					{
						component2.velocity = Vector2.zero;
						component2.drag = 0.5f;
					}
				}
				this.canWhip = true;
			}
			yield break;
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x000AE38C File Offset: 0x000AC58C
		private void Start()
		{
			this.whipStart = GameObject.Find("WhipStart");
			this.whipEndStrike = GameObject.Find("WhipEndStrike");
			this.whipHandle = GameObject.Find("WhipHandle");
			this.whipSpring = GameObject.Find("WhipSpring");
			this.audioSource = base.GetComponent<AudioSource>();
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x000AE3E8 File Offset: 0x000AC5E8
		private void Update()
		{
			if (!this.dragging && Input.GetMouseButtonDown(0))
			{
				Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Collider2D collider2D = Physics2D.OverlapPoint(point);
				if (collider2D != null && collider2D.gameObject == this.whipHandle)
				{
					this.dragging = true;
					this.prevDrag = point;
				}
			}
			else if (this.dragging && Input.GetMouseButton(0))
			{
				Vector2 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 b = a - this.prevDrag;
				Rigidbody2D component = this.whipHandle.GetComponent<Rigidbody2D>();
				component.MovePosition(component.position + b);
				this.prevDrag = a;
			}
			else
			{
				this.dragging = false;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				base.StartCoroutine(this.WhipForward());
			}
		}

		// Token: 0x040008E5 RID: 2277
		public AudioClip WhipCrack;

		// Token: 0x040008E6 RID: 2278
		public AudioClip WhipCrackThunder;

		// Token: 0x040008E7 RID: 2279
		private AudioSource audioSource;

		// Token: 0x040008E8 RID: 2280
		private GameObject whipStart;

		// Token: 0x040008E9 RID: 2281
		private GameObject whipEndStrike;

		// Token: 0x040008EA RID: 2282
		private GameObject whipHandle;

		// Token: 0x040008EB RID: 2283
		private GameObject whipSpring;

		// Token: 0x040008EC RID: 2284
		private Vector2 prevDrag;

		// Token: 0x040008ED RID: 2285
		private bool dragging;

		// Token: 0x040008EE RID: 2286
		private bool canWhip = true;
	}
}
