using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class ProjectileMover : MonoBehaviour
{
	// Token: 0x06000155 RID: 341 RVA: 0x0009928C File Offset: 0x0009748C
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		if (this.flash != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.flash, base.transform.position, Quaternion.identity);
			gameObject.transform.forward = base.gameObject.transform.forward;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(gameObject, component.main.duration);
			}
			else
			{
				ParticleSystem component2 = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
				UnityEngine.Object.Destroy(gameObject, component2.main.duration);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject, 5f);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0004512B File Offset: 0x0004332B
	private void FixedUpdate()
	{
		if (this.speed != 0f)
		{
			this.rb.velocity = base.transform.forward * this.speed;
		}
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0009934C File Offset: 0x0009754C
	private void OnCollisionEnter(Collision collision)
	{
		this.rb.constraints = RigidbodyConstraints.FreezeAll;
		this.speed = 0f;
		ContactPoint contactPoint = collision.contacts[0];
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
		Vector3 position = contactPoint.point + contactPoint.normal * this.hitOffset;
		if (this.hit != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.hit, position, rotation);
			if (this.UseFirePointRotation)
			{
				gameObject.transform.rotation = base.gameObject.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
			}
			else if (this.rotationOffset != Vector3.zero)
			{
				gameObject.transform.rotation = Quaternion.Euler(this.rotationOffset);
			}
			else
			{
				gameObject.transform.LookAt(contactPoint.point + contactPoint.normal);
			}
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(gameObject, component.main.duration);
			}
			else
			{
				ParticleSystem component2 = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
				UnityEngine.Object.Destroy(gameObject, component2.main.duration);
			}
		}
		foreach (GameObject gameObject2 in this.Detached)
		{
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = null;
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x0400032F RID: 815
	public float speed = 15f;

	// Token: 0x04000330 RID: 816
	public float hitOffset;

	// Token: 0x04000331 RID: 817
	public bool UseFirePointRotation;

	// Token: 0x04000332 RID: 818
	public Vector3 rotationOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x04000333 RID: 819
	public GameObject hit;

	// Token: 0x04000334 RID: 820
	public GameObject flash;

	// Token: 0x04000335 RID: 821
	private Rigidbody rb;

	// Token: 0x04000336 RID: 822
	public GameObject[] Detached;
}
