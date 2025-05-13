using System;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class Hovl_Laser : MonoBehaviour
{
	// Token: 0x06000149 RID: 329 RVA: 0x0004507E File Offset: 0x0004327E
	private void Start()
	{
		this.Laser = base.GetComponent<LineRenderer>();
		this.Effects = base.GetComponentsInChildren<ParticleSystem>();
		this.Hit = this.HitEffect.GetComponentsInChildren<ParticleSystem>();
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00098890 File Offset: 0x00096A90
	private void Update()
	{
		this.Laser.material.SetTextureScale("_MainTex", new Vector2(this.Length[0], this.Length[1]));
		this.Laser.material.SetTextureScale("_Noise", new Vector2(this.Length[2], this.Length[3]));
		if (this.Laser != null && !this.UpdateSaver)
		{
			this.Laser.SetPosition(0, base.transform.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward), out raycastHit, this.MaxLength))
			{
				this.Laser.SetPosition(1, raycastHit.point);
				this.HitEffect.transform.position = raycastHit.point + raycastHit.normal * this.HitOffset;
				if (this.useLaserRotation)
				{
					this.HitEffect.transform.rotation = base.transform.rotation;
				}
				else
				{
					this.HitEffect.transform.LookAt(raycastHit.point + raycastHit.normal);
				}
				foreach (ParticleSystem particleSystem in this.Effects)
				{
					if (!particleSystem.isPlaying)
					{
						particleSystem.Play();
					}
				}
				this.Length[0] = this.MainTextureLength * Vector3.Distance(base.transform.position, raycastHit.point);
				this.Length[2] = this.NoiseTextureLength * Vector3.Distance(base.transform.position, raycastHit.point);
			}
			else
			{
				Vector3 vector = base.transform.position + base.transform.forward * this.MaxLength;
				this.Laser.SetPosition(1, vector);
				this.HitEffect.transform.position = vector;
				foreach (ParticleSystem particleSystem2 in this.Hit)
				{
					if (particleSystem2.isPlaying)
					{
						particleSystem2.Stop();
					}
				}
				this.Length[0] = this.MainTextureLength * Vector3.Distance(base.transform.position, vector);
				this.Length[2] = this.NoiseTextureLength * Vector3.Distance(base.transform.position, vector);
			}
			if (!this.Laser.enabled && !this.LaserSaver)
			{
				this.LaserSaver = true;
				this.Laser.enabled = true;
			}
		}
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00098B50 File Offset: 0x00096D50
	public void DisablePrepare()
	{
		if (this.Laser != null)
		{
			this.Laser.enabled = false;
		}
		this.UpdateSaver = true;
		if (this.Effects != null)
		{
			foreach (ParticleSystem particleSystem in this.Effects)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
				}
			}
		}
	}

	// Token: 0x04000308 RID: 776
	public GameObject HitEffect;

	// Token: 0x04000309 RID: 777
	public float HitOffset;

	// Token: 0x0400030A RID: 778
	public bool useLaserRotation;

	// Token: 0x0400030B RID: 779
	public float MaxLength;

	// Token: 0x0400030C RID: 780
	private LineRenderer Laser;

	// Token: 0x0400030D RID: 781
	public float MainTextureLength = 1f;

	// Token: 0x0400030E RID: 782
	public float NoiseTextureLength = 1f;

	// Token: 0x0400030F RID: 783
	private Vector4 Length = new Vector4(1f, 1f, 1f, 1f);

	// Token: 0x04000310 RID: 784
	private bool LaserSaver;

	// Token: 0x04000311 RID: 785
	private bool UpdateSaver;

	// Token: 0x04000312 RID: 786
	private ParticleSystem[] Effects;

	// Token: 0x04000313 RID: 787
	private ParticleSystem[] Hit;
}
