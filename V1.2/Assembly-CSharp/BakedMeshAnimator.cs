using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class BakedMeshAnimator : MonoBehaviour
{
	// Token: 0x060000A2 RID: 162 RVA: 0x00093818 File Offset: 0x00091A18
	private void Awake()
	{
		if (this.animationMeshRenderer == null)
		{
			this.animationMeshRenderer = base.GetComponent<MeshRenderer>();
		}
		if (this.animationMeshRenderer == null)
		{
			Debug.LogError("BakedMeshAnimator: " + ((this != null) ? this.ToString() : null) + " has no assigned MeshRenderer!");
		}
		this.meshFilter = this.animationMeshRenderer.GetComponent<MeshFilter>();
		this.meshCacheCount = this.animations[this.currentAnimation].meshes.Length;
		this.currentSpeed = this.animations[this.currentAnimation].playSpeed;
		this.CreateCrossFadeMesh();
		this.StartCrossfade();
		if (this.meshFilter.sharedMesh == null)
		{
			this.meshFilter.sharedMesh = this.animations[0].meshes[0];
		}
		this.SetAnimation(this.startAnimation);
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00044C5D File Offset: 0x00042E5D
	public void AnimateUpdate()
	{
		if (this.animations.Length == 0)
		{
			return;
		}
		this.Animate();
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00044C6F File Offset: 0x00042E6F
	public void SetAnimation(int _animation, int _transitionFrame)
	{
		if (this.currentAnimation == _animation)
		{
			return;
		}
		this.transitionFrame = _transitionFrame;
		this.anim = _animation;
		this.SetAnimCommon();
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00044C8F File Offset: 0x00042E8F
	public void SetAnimation(int _animation)
	{
		if (this.currentAnimation == _animation)
		{
			return;
		}
		this.transitionFrame = this.animations[this.currentAnimation].transitionFrame;
		this.anim = _animation;
		this.SetAnimCommon();
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00044CC0 File Offset: 0x00042EC0
	private void SetAnimCommon()
	{
		base.enabled = true;
		this.transitioning = true;
		this.StartCrossfade();
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x000938F8 File Offset: 0x00091AF8
	public void Animate()
	{
		if (!this.animationMeshRenderer.isVisible)
		{
			return;
		}
		if (this.transitioning)
		{
			if (this.crossfade || (int)this.currentFrame == this.transitionFrame || this.failsafe > this.transitionFailsafe)
			{
				this.failsafe = 0f;
				this.transitioning = false;
				this.currentAnimation = this.anim;
				this.meshCacheCount = this.animations[this.currentAnimation].meshes.Length;
				this.currentSpeed = this.animations[this.currentAnimation].playSpeed;
				if (Time.time < 1f && this.animations[this.currentAnimation].randomStartFrame)
				{
					this.currentFrame = (float)UnityEngine.Random.Range(this.meshCacheCount, 0);
				}
				else if (this.crossfade)
				{
					this.currentFrame = (float)this.animations[this.currentAnimation].crossfadeFrame;
				}
				else
				{
					this.currentFrame = (float)this.animations[this.currentAnimation].transitionFrame;
				}
			}
			else
			{
				this.failsafe += Time.deltaTime;
			}
		}
		if (!this.doCrossfade)
		{
			if (this.animations[this.currentAnimation].pingPong)
			{
				this.PingPongFrame();
			}
			else
			{
				this.NextFrame();
			}
			if (this.currentFrameInt != (int)this.currentFrame)
			{
				this.currentFrameInt = (int)this.currentFrame;
				if (this.crossfade && this.crossfadeNormalFix)
				{
					this.animations[this.currentAnimation].meshes[(int)this.currentFrame].GetVertices(this.norms);
					this.crossfadeMeshEnd.SetVertices(this.norms);
				}
				else
				{
					this.meshFilter.sharedMesh = this.animations[this.currentAnimation].meshes[(int)this.currentFrame];
				}
			}
		}
		this.UpdateCrossfade();
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00093ADC File Offset: 0x00091CDC
	public bool NextFrame()
	{
		this.currentFrame += this.currentSpeed * Time.deltaTime * this.playSpeedMultiplier;
		if (this.currentFrame > (float)(this.meshCacheCount + 1))
		{
			this.currentFrame = 0f;
			if (!this.animations[this.currentAnimation].loop)
			{
				base.enabled = false;
			}
			return true;
		}
		if (this.currentFrame >= (float)this.meshCacheCount)
		{
			this.currentFrame = (float)this.meshCacheCount - this.currentFrame;
			if (!this.animations[this.currentAnimation].loop)
			{
				base.enabled = false;
			}
			return true;
		}
		return false;
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00093B84 File Offset: 0x00091D84
	public bool PingPongFrame()
	{
		if (this.pingPongToggle)
		{
			this.currentFrame += this.currentSpeed * Time.deltaTime * this.playSpeedMultiplier;
		}
		else
		{
			this.currentFrame -= this.currentSpeed * Time.deltaTime * this.playSpeedMultiplier;
		}
		if (this.currentFrame <= 0f)
		{
			this.currentFrame = 0f;
			this.pingPongToggle = true;
			return true;
		}
		if (this.currentFrame >= (float)this.meshCacheCount)
		{
			this.pingPongToggle = false;
			this.currentFrame = (float)(this.meshCacheCount - 1);
			return true;
		}
		return false;
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00044CD6 File Offset: 0x00042ED6
	public void SetSpeedMultiplier(float speed)
	{
		this.playSpeedMultiplier = speed;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00093C24 File Offset: 0x00091E24
	private void CrossfadeInit()
	{
		if (this.vertsDiff == null)
		{
			this.vertsDiff = new Vector3[this.vertsStart.Count];
		}
		this.crossfadeMestTo.GetVertices(this.meshVerts);
		for (int i = 0; i < this.vertsStart.Count; i++)
		{
			this.vertsDiff[i] = this.meshVerts[i] - this.vertsStart[i];
		}
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00093CA0 File Offset: 0x00091EA0
	private void CreateCrossFadeMesh()
	{
		if (!this.crossfade)
		{
			return;
		}
		this.crossfadeMeshStart = this.meshFilter.sharedMesh;
		this.crossfadeMeshStart.GetVertices(this.vertsStart);
		if (this.crossfadeMeshEnd == null)
		{
			this.crossfadeMeshEnd = new Mesh();
			this.crossfadeMeshEnd.MarkDynamic();
			this.crossfadeMeshEnd.SetVertices(this.vertsStart);
			this.crossfadeMeshEnd.triangles = this.crossfadeMeshStart.triangles;
			this.crossfadeMeshEnd.uv = this.crossfadeMeshStart.uv;
			this.crossfadeMeshStart.GetNormals(this.norms);
			this.crossfadeMeshEnd.SetNormals(this.norms);
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00093D5C File Offset: 0x00091F5C
	private void StartCrossfade()
	{
		if (!this.crossfade)
		{
			return;
		}
		this.crossfadeMeshStart = this.meshFilter.sharedMesh;
		this.crossfadeMeshStart.GetVertices(this.vertsStart);
		this.doCrossfade = true;
		this.crossfadeWeight = 0f;
		this.crossfadeMeshEnd.SetVertices(this.vertsStart);
		this.meshFilter.mesh = this.crossfadeMeshEnd;
		this.crossfadeMeshStart.GetVertices(this.vertsStart);
		this.crossfadeMeshEnd.colors = this.crossfadeMeshStart.colors;
		if (this.vertsCurrent == null)
		{
			this.vertsCurrent = new Vector3[this.vertsStart.Count];
		}
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00093E10 File Offset: 0x00092010
	private void UpdateCrossfade()
	{
		if (!this.crossfade)
		{
			return;
		}
		this.nextUpdate += Time.deltaTime;
		if (this.nextUpdate < this.crossfadeFrequency)
		{
			return;
		}
		this.nextUpdate = 0f;
		if (this.crossfadeWeight >= 1f)
		{
			this.doCrossfade = false;
			return;
		}
		this.crossfadeMestTo = this.animations[this.currentAnimation].meshes[this.animations[this.currentAnimation].crossfadeFrame];
		if (this.crossfadeWeight == 0f)
		{
			this.CrossfadeInit();
		}
		for (int i = 0; i < this.vertsCurrent.Length; i++)
		{
			this.vertsCurrent[i] = this.vertsStart[i];
		}
		if (this.vertsDiff.Length != this.vertsStart.Count)
		{
			return;
		}
		for (int j = 0; j < this.vertsCurrent.Length; j++)
		{
			this.vertsCurrent[j] += this.vertsDiff[j] * this.crossfadeWeight;
		}
		this.crossfadeMeshEnd.SetVertices(this.vertsCurrent);
		this.crossfadeWeight += this.crossfadeWeightAdd;
	}

	// Token: 0x040001D1 RID: 465
	public MeshRenderer animationMeshRenderer;

	// Token: 0x040001D2 RID: 466
	public BakedMeshAnimation[] animations;

	// Token: 0x040001D3 RID: 467
	public int startAnimation;

	// Token: 0x040001D4 RID: 468
	private int currentAnimation;

	// Token: 0x040001D5 RID: 469
	private MeshFilter meshFilter;

	// Token: 0x040001D6 RID: 470
	public float currentFrame;

	// Token: 0x040001D7 RID: 471
	private int currentFrameInt;

	// Token: 0x040001D8 RID: 472
	private float currentSpeed;

	// Token: 0x040001D9 RID: 473
	private bool pingPongToggle;

	// Token: 0x040001DA RID: 474
	public float playSpeedMultiplier = 1f;

	// Token: 0x040001DB RID: 475
	private int meshCacheCount;

	// Token: 0x040001DC RID: 476
	public float transitionFailsafe = 0.4f;

	// Token: 0x040001DD RID: 477
	private float failsafe;

	// Token: 0x040001DE RID: 478
	private int transitionFrame;

	// Token: 0x040001DF RID: 479
	private int anim;

	// Token: 0x040001E0 RID: 480
	private bool transitioning = true;

	// Token: 0x040001E1 RID: 481
	public bool crossfade;

	// Token: 0x040001E2 RID: 482
	public bool crossfadeNormalFix;

	// Token: 0x040001E3 RID: 483
	public float crossfadeFrequency = 0.05f;

	// Token: 0x040001E4 RID: 484
	public float crossfadeWeightAdd = 0.221f;

	// Token: 0x040001E5 RID: 485
	private bool doCrossfade;

	// Token: 0x040001E6 RID: 486
	private float crossfadeWeight = 1f;

	// Token: 0x040001E7 RID: 487
	private Mesh crossfadeMestTo;

	// Token: 0x040001E8 RID: 488
	private Mesh crossfadeMeshStart;

	// Token: 0x040001E9 RID: 489
	private Mesh crossfadeMeshEnd;

	// Token: 0x040001EA RID: 490
	private List<Vector3> vertsStart = new List<Vector3>();

	// Token: 0x040001EB RID: 491
	private List<Vector3> norms = new List<Vector3>();

	// Token: 0x040001EC RID: 492
	private Vector3[] vertsCurrent;

	// Token: 0x040001ED RID: 493
	private Vector3[] vertsDiff;

	// Token: 0x040001EE RID: 494
	private List<Vector3> meshVerts = new List<Vector3>();

	// Token: 0x040001EF RID: 495
	private float nextUpdate;
}
