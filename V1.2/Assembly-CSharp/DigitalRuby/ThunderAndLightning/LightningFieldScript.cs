using System;
using SoL.Managers;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000CB RID: 203
	public class LightningFieldScript : LightningBoltPrefabScriptBase
	{
		// Token: 0x06000762 RID: 1890 RVA: 0x000ACEA0 File Offset: 0x000AB0A0
		private Vector3 RandomPointInBounds()
		{
			float x = UnityEngine.Random.Range(this.FieldBounds.min.x, this.FieldBounds.max.x);
			float y = UnityEngine.Random.Range(this.FieldBounds.min.y, this.FieldBounds.max.y);
			float z = UnityEngine.Random.Range(this.FieldBounds.min.z, this.FieldBounds.max.z);
			return new Vector3(x, y, z);
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x000480B4 File Offset: 0x000462B4
		protected override void Start()
		{
			base.Start();
			if (this.Light != null)
			{
				this.Light.enabled = false;
			}
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x000ACF28 File Offset: 0x000AB128
		protected override void Update()
		{
			if (SceneCompositionManager.IsLoading)
			{
				return;
			}
			base.Update();
			if (Time.timeScale <= 0f)
			{
				return;
			}
			if (this.Light != null)
			{
				this.Light.transform.position = this.FieldBounds.center;
				this.Light.intensity = UnityEngine.Random.Range(2.8f, 3.2f);
			}
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x000ACF94 File Offset: 0x000AB194
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			this.minimumLengthSquared = this.MinimumLength * this.MinimumLength;
			for (int i = 0; i < 16; i++)
			{
				parameters.Start = this.RandomPointInBounds();
				parameters.End = this.RandomPointInBounds();
				if ((parameters.End - parameters.Start).sqrMagnitude >= this.minimumLengthSquared)
				{
					break;
				}
			}
			if (this.Light != null)
			{
				this.Light.enabled = true;
			}
			base.CreateLightningBolt(parameters);
		}

		// Token: 0x040008C0 RID: 2240
		[Header("Lightning Field Properties")]
		[Tooltip("The minimum length for a field segment")]
		public float MinimumLength = 0.01f;

		// Token: 0x040008C1 RID: 2241
		private float minimumLengthSquared;

		// Token: 0x040008C2 RID: 2242
		[Tooltip("The bounds to put the field in.")]
		public Bounds FieldBounds;

		// Token: 0x040008C3 RID: 2243
		[Tooltip("Optional light for the lightning field to emit")]
		public Light Light;
	}
}
