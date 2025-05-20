using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class ParticleCollisionInstance : MonoBehaviour
{
	// Token: 0x06000152 RID: 338 RVA: 0x0004511D File Offset: 0x0004331D
	private void Start()
	{
		this.part = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00099080 File Offset: 0x00097280
	private void OnParticleCollision(GameObject other)
	{
		int num = this.part.GetCollisionEvents(other, this.collisionEvents);
		for (int i = 0; i < num; i++)
		{
			GameObject[] effectsOnCollision = this.EffectsOnCollision;
			for (int j = 0; j < effectsOnCollision.Length; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(effectsOnCollision[j], this.collisionEvents[i].intersection + this.collisionEvents[i].normal * this.Offset, default(Quaternion));
				if (!this.UseWorldSpacePosition)
				{
					gameObject.transform.parent = base.transform;
				}
				if (this.UseFirePointRotation)
				{
					gameObject.transform.LookAt(base.transform.position);
				}
				else if (this.rotationOffset != Vector3.zero && this.useOnlyRotationOffset)
				{
					gameObject.transform.rotation = Quaternion.Euler(this.rotationOffset);
				}
				else
				{
					gameObject.transform.LookAt(this.collisionEvents[i].intersection + this.collisionEvents[i].normal);
					gameObject.transform.rotation *= Quaternion.Euler(this.rotationOffset);
				}
				UnityEngine.Object.Destroy(gameObject, this.DestroyTimeDelay);
			}
		}
		if (this.DestoyMainEffect)
		{
			UnityEngine.Object.Destroy(base.gameObject, this.DestroyTimeDelay + 0.5f);
		}
	}

	// Token: 0x04000324 RID: 804
	public GameObject[] EffectsOnCollision;

	// Token: 0x04000325 RID: 805
	public float DestroyTimeDelay = 5f;

	// Token: 0x04000326 RID: 806
	public bool UseWorldSpacePosition;

	// Token: 0x04000327 RID: 807
	public float Offset;

	// Token: 0x04000328 RID: 808
	public Vector3 rotationOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x04000329 RID: 809
	public bool useOnlyRotationOffset = true;

	// Token: 0x0400032A RID: 810
	public bool UseFirePointRotation;

	// Token: 0x0400032B RID: 811
	public bool DestoyMainEffect = true;

	// Token: 0x0400032C RID: 812
	private ParticleSystem part;

	// Token: 0x0400032D RID: 813
	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

	// Token: 0x0400032E RID: 814
	private ParticleSystem ps;
}
