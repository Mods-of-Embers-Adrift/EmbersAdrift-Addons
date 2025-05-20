using System;
using System.Collections;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001C3 RID: 451
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcTriangulate2D")]
	[DisallowMultipleComponent]
	[BGCc.CcDescriptor(Description = "Triangulate 2D spline. Currently only simple polygons are supported.", Name = "Triangulate 2D", Icon = "BGCcTriangulate2D123")]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcTriangulate2D")]
	[ExecuteInEditMode]
	public class BGCcTriangulate2D : BGCcSplitterPolyline
	{
		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001025 RID: 4133 RVA: 0x0004D891 File Offset: 0x0004BA91
		// (set) Token: 0x06001026 RID: 4134 RVA: 0x000DEB94 File Offset: 0x000DCD94
		public Vector2 ScaleUv
		{
			get
			{
				return this.scaleUV;
			}
			set
			{
				if (Mathf.Abs(this.scaleUV.x - value.x) < 1E-05f && Mathf.Abs(this.scaleUV.y - value.y) < 1E-05f)
				{
					return;
				}
				base.ParamChanged<Vector2>(ref this.scaleUV, value);
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001027 RID: 4135 RVA: 0x0004D899 File Offset: 0x0004BA99
		// (set) Token: 0x06001028 RID: 4136 RVA: 0x0004D8A1 File Offset: 0x0004BAA1
		public bool Flip
		{
			get
			{
				return this.flip;
			}
			set
			{
				if (this.flip == value)
				{
					return;
				}
				base.ParamChanged<bool>(ref this.flip, value);
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001029 RID: 4137 RVA: 0x0004D8BB File Offset: 0x0004BABB
		// (set) Token: 0x0600102A RID: 4138 RVA: 0x0004D8C3 File Offset: 0x0004BAC3
		public bool DoubleSided
		{
			get
			{
				return this.doubleSided;
			}
			set
			{
				base.ParamChanged<bool>(ref this.doubleSided, value);
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x0600102B RID: 4139 RVA: 0x0004D8D3 File Offset: 0x0004BAD3
		// (set) Token: 0x0600102C RID: 4140 RVA: 0x000DEBEC File Offset: 0x000DCDEC
		public bool UpdateEveryFrame
		{
			get
			{
				return this.updateEveryFrame;
			}
			set
			{
				if (this.updateEveryFrame == value)
				{
					return;
				}
				this.updateEveryFrame = value;
				base.ParamChanged<bool>(ref this.updateEveryFrame, value);
				if (this.updateEveryFrame && !this.everyFrameUpdateIsRunning && base.gameObject.activeSelf && Application.isPlaying)
				{
					base.StartCoroutine(this.UiUpdater());
				}
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool UseLocal
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x0600102E RID: 4142 RVA: 0x0004CD0B File Offset: 0x0004AF0B
		public override string Error
		{
			get
			{
				if (base.Curve.Mode2DOn)
				{
					return null;
				}
				return "Curve should be in 2D mode";
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x0600102F RID: 4143 RVA: 0x000DEC4C File Offset: 0x000DCE4C
		public override string Info
		{
			get
			{
				MeshFilter meshFilter = this.MeshFilter;
				if (meshFilter == null)
				{
					return "No data.";
				}
				Mesh sharedMesh = meshFilter.sharedMesh;
				if (sharedMesh == null)
				{
					return "No data.";
				}
				return string.Concat(new string[]
				{
					"Mesh uses ",
					sharedMesh.vertexCount.ToString(),
					" vertices and ",
					(sharedMesh.triangles.Length / 3).ToString(),
					" triangles."
				});
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06001030 RID: 4144 RVA: 0x0004D8DB File Offset: 0x0004BADB
		public MeshFilter MeshFilter
		{
			get
			{
				if (this.meshFilter == null)
				{
					this.meshFilter = base.GetComponent<MeshFilter>();
				}
				return this.meshFilter;
			}
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x000DECD0 File Offset: 0x000DCED0
		public override void Start()
		{
			base.Start();
			if (this.MeshFilter.sharedMesh == null)
			{
				this.UpdateUI();
			}
			if (this.updateEveryFrame && base.gameObject.activeSelf && Application.isPlaying)
			{
				base.StartCoroutine(this.UiUpdater());
			}
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x0004D8FD File Offset: 0x0004BAFD
		private void OnEnable()
		{
			if (this.updateEveryFrame && !this.everyFrameUpdateIsRunning && Application.isPlaying)
			{
				base.StartCoroutine(this.UiUpdater());
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x0004D923 File Offset: 0x0004BB23
		private void OnDisable()
		{
			if (this.updateEveryFrame && this.everyFrameUpdateIsRunning && Application.isPlaying)
			{
				this.everyFrameUpdateIsRunning = false;
			}
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x000DED28 File Offset: 0x000DCF28
		public void UpdateUI()
		{
			this.updateAtFrame = Time.frameCount;
			if (!base.Curve.Mode2DOn)
			{
				return;
			}
			List<Vector3> positions = base.Positions;
			MeshFilter meshFilter;
			try
			{
				meshFilter = this.MeshFilter;
			}
			catch (MissingReferenceException)
			{
				base.RemoveListeners();
				return;
			}
			Mesh mesh = meshFilter.mesh;
			if (mesh == null)
			{
				mesh = new Mesh();
				meshFilter.mesh = mesh;
			}
			if (this.triangulator == null)
			{
				this.triangulator = new BGTriangulator2D();
			}
			this.triangulator.Bind(mesh, positions, new BGTriangulator2D.Config
			{
				Closed = base.Curve.Closed,
				Mode2D = base.Curve.Mode2D,
				Flip = this.flip,
				ScaleUV = this.scaleUV,
				OffsetUV = this.offsetUV,
				DoubleSided = this.doubleSided,
				ScaleBackUV = this.scaleBackUV,
				OffsetBackUV = this.offsetBackUV
			});
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x0004D943 File Offset: 0x0004BB43
		protected override void UpdateRequested(object sender, EventArgs e)
		{
			base.UpdateRequested(sender, e);
			this.UpdateUI();
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x0004D953 File Offset: 0x0004BB53
		private IEnumerator UiUpdater()
		{
			this.everyFrameUpdateIsRunning = true;
			while (this.updateEveryFrame)
			{
				if (this.updateAtFrame != Time.frameCount)
				{
					this.UpdateUI();
				}
				yield return null;
			}
			this.everyFrameUpdateIsRunning = false;
			yield break;
		}

		// Token: 0x04000D9D RID: 3485
		[SerializeField]
		[Tooltip("UV scale")]
		private Vector2 scaleUV = new Vector2(1f, 1f);

		// Token: 0x04000D9E RID: 3486
		[SerializeField]
		[Tooltip("UV offset")]
		private Vector2 offsetUV = new Vector2(0f, 0f);

		// Token: 0x04000D9F RID: 3487
		[SerializeField]
		[Tooltip("Flip triangles")]
		private bool flip;

		// Token: 0x04000DA0 RID: 3488
		[SerializeField]
		[Tooltip("Double sided")]
		private bool doubleSided;

		// Token: 0x04000DA1 RID: 3489
		[SerializeField]
		[Tooltip("UV scale for back side")]
		private Vector2 scaleBackUV = new Vector2(1f, 1f);

		// Token: 0x04000DA2 RID: 3490
		[SerializeField]
		[Tooltip("UV offset for back side")]
		private Vector2 offsetBackUV = new Vector2(0f, 0f);

		// Token: 0x04000DA3 RID: 3491
		[SerializeField]
		[Tooltip("Update mesh every frame, even if curve's not changed. This can be useful, if UVs are animated.")]
		private bool updateEveryFrame;

		// Token: 0x04000DA4 RID: 3492
		private int updateAtFrame;

		// Token: 0x04000DA5 RID: 3493
		private bool everyFrameUpdateIsRunning;

		// Token: 0x04000DA6 RID: 3494
		[NonSerialized]
		private MeshFilter meshFilter;

		// Token: 0x04000DA7 RID: 3495
		[NonSerialized]
		private BGTriangulator2D triangulator;
	}
}
