using System;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class Hovl_Laser2 : MonoBehaviour
{
	// Token: 0x0600014D RID: 333 RVA: 0x00098BD0 File Offset: 0x00096DD0
	private void Start()
	{
		this.laserPS = base.GetComponent<ParticleSystem>();
		this.laserMat = base.GetComponent<ParticleSystemRenderer>().material;
		this.Flash = this.FlashEffect.GetComponentsInChildren<ParticleSystem>();
		this.Hit = this.HitEffect.GetComponentsInChildren<ParticleSystem>();
		this.laserMat.SetFloat("_Scale", this.laserScale);
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00098C34 File Offset: 0x00096E34
	private void Update()
	{
		if (this.laserPS != null && !this.UpdateSaver)
		{
			this.laserMat.SetVector("_StartPoint", base.transform.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward), out raycastHit, this.MaxLength))
			{
				this.particleCount = Mathf.RoundToInt(raycastHit.distance / (2f * this.laserScale));
				if ((float)this.particleCount < raycastHit.distance / (2f * this.laserScale))
				{
					this.particleCount++;
				}
				this.particlesPositions = new Vector3[this.particleCount];
				this.AddParticles();
				this.laserMat.SetFloat("_Distance", raycastHit.distance);
				this.laserMat.SetVector("_EndPoint", raycastHit.point);
				if (this.Hit != null)
				{
					this.HitEffect.transform.position = raycastHit.point + raycastHit.normal * this.HitOffset;
					this.HitEffect.transform.LookAt(raycastHit.point);
					foreach (ParticleSystem particleSystem in this.Hit)
					{
						if (!particleSystem.isPlaying)
						{
							particleSystem.Play();
						}
					}
					foreach (ParticleSystem particleSystem2 in this.Flash)
					{
						if (!particleSystem2.isPlaying)
						{
							particleSystem2.Play();
						}
					}
				}
			}
			else
			{
				Vector3 vector = base.transform.position + base.transform.forward * this.MaxLength;
				float num = Vector3.Distance(vector, base.transform.position);
				this.particleCount = Mathf.RoundToInt(num / (2f * this.laserScale));
				if ((float)this.particleCount < num / (2f * this.laserScale))
				{
					this.particleCount++;
				}
				this.particlesPositions = new Vector3[this.particleCount];
				this.AddParticles();
				this.laserMat.SetFloat("_Distance", num);
				this.laserMat.SetVector("_EndPoint", vector);
				if (this.Hit != null)
				{
					this.HitEffect.transform.position = vector;
					foreach (ParticleSystem particleSystem3 in this.Hit)
					{
						if (particleSystem3.isPlaying)
						{
							particleSystem3.Stop();
						}
					}
				}
			}
		}
		if (this.startDissovle)
		{
			this.dissovleTimer += Time.deltaTime;
			this.laserMat.SetFloat("_Dissolve", this.dissovleTimer * 5f);
		}
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00098F1C File Offset: 0x0009711C
	private void AddParticles()
	{
		this.particles = new ParticleSystem.Particle[this.particleCount];
		for (int i = 0; i < this.particleCount; i++)
		{
			this.particlesPositions[i] = new Vector3(0f, 0f, 0f) + new Vector3(0f, 0f, (float)(i * 2) * this.laserScale);
			this.particles[i].position = this.particlesPositions[i];
			this.particles[i].startSize3D = new Vector3(0.001f, 0.001f, 2f * this.laserScale);
			this.particles[i].startColor = this.laserColor;
		}
		this.laserPS.SetParticles(this.particles, this.particles.Length);
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00099010 File Offset: 0x00097210
	public void DisablePrepare()
	{
		base.transform.parent = null;
		this.dissovleTimer = 0f;
		this.startDissovle = true;
		this.UpdateSaver = true;
		if (this.Flash != null && this.Hit != null)
		{
			foreach (ParticleSystem particleSystem in this.Hit)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
				}
			}
			foreach (ParticleSystem particleSystem2 in this.Flash)
			{
				if (particleSystem2.isPlaying)
				{
					particleSystem2.Stop();
				}
			}
		}
	}

	// Token: 0x04000314 RID: 788
	public float laserScale = 1f;

	// Token: 0x04000315 RID: 789
	public Color laserColor = new Vector4(1f, 1f, 1f, 1f);

	// Token: 0x04000316 RID: 790
	public GameObject HitEffect;

	// Token: 0x04000317 RID: 791
	public GameObject FlashEffect;

	// Token: 0x04000318 RID: 792
	public float HitOffset;

	// Token: 0x04000319 RID: 793
	public float MaxLength;

	// Token: 0x0400031A RID: 794
	private bool UpdateSaver;

	// Token: 0x0400031B RID: 795
	private ParticleSystem laserPS;

	// Token: 0x0400031C RID: 796
	private ParticleSystem[] Flash;

	// Token: 0x0400031D RID: 797
	private ParticleSystem[] Hit;

	// Token: 0x0400031E RID: 798
	private Material laserMat;

	// Token: 0x0400031F RID: 799
	private int particleCount;

	// Token: 0x04000320 RID: 800
	private ParticleSystem.Particle[] particles;

	// Token: 0x04000321 RID: 801
	private Vector3[] particlesPositions;

	// Token: 0x04000322 RID: 802
	private float dissovleTimer;

	// Token: 0x04000323 RID: 803
	private bool startDissovle;
}
