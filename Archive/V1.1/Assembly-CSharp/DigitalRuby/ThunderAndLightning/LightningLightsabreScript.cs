using System;
using SoL.Managers;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000CF RID: 207
	public class LightningLightsabreScript : LightningBoltPrefabScript
	{
		// Token: 0x06000779 RID: 1913 RVA: 0x00048162 File Offset: 0x00046362
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x000ADA48 File Offset: 0x000ABC48
		protected override void Update()
		{
			if (SceneCompositionManager.IsLoading)
			{
				return;
			}
			if (this.state == 2 || this.state == 3)
			{
				this.bladeTime += LightningBoltScript.DeltaTime;
				float num = Mathf.Lerp(0.01f, 1f, this.bladeTime / this.ActivationTime);
				Vector3 position = this.bladeStart + this.bladeDir * num * this.BladeHeight;
				this.Destination.transform.position = position;
				this.GlowIntensity = this.bladeIntensity * ((this.state == 3) ? num : (1f - num));
				if (this.bladeTime >= this.ActivationTime)
				{
					this.GlowIntensity = this.bladeIntensity;
					this.bladeTime = 0f;
					if (this.state == 2)
					{
						this.ManualMode = true;
						this.state = 0;
					}
					else
					{
						this.state = 1;
					}
				}
			}
			base.Update();
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x000ADB40 File Offset: 0x000ABD40
		public bool TurnOn(bool value)
		{
			if (this.state == 2 || this.state == 3 || (this.state == 1 && value) || (this.state == 0 && !value))
			{
				return false;
			}
			this.bladeStart = this.Destination.transform.position;
			this.ManualMode = false;
			this.bladeIntensity = this.GlowIntensity;
			if (value)
			{
				this.bladeDir = (base.Camera.orthographic ? base.transform.up : base.transform.forward);
				this.state = 3;
				this.StartSound.Play();
				this.StopSound.Stop();
				this.ConstantSound.Play();
			}
			else
			{
				this.bladeDir = -(base.Camera.orthographic ? base.transform.up : base.transform.forward);
				this.state = 2;
				this.StartSound.Stop();
				this.StopSound.Play();
				this.ConstantSound.Stop();
			}
			return true;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x0004816A File Offset: 0x0004636A
		public void TurnOnGUI(bool value)
		{
			this.TurnOn(value);
		}

		// Token: 0x040008C8 RID: 2248
		[Header("Lightsabre Properties")]
		[Tooltip("Height of the blade")]
		public float BladeHeight = 19f;

		// Token: 0x040008C9 RID: 2249
		[Tooltip("How long it takes to turn the lightsabre on and off")]
		public float ActivationTime = 0.5f;

		// Token: 0x040008CA RID: 2250
		[Tooltip("Sound to play when the lightsabre turns on")]
		public AudioSource StartSound;

		// Token: 0x040008CB RID: 2251
		[Tooltip("Sound to play when the lightsabre turns off")]
		public AudioSource StopSound;

		// Token: 0x040008CC RID: 2252
		[Tooltip("Sound to play when the lightsabre stays on")]
		public AudioSource ConstantSound;

		// Token: 0x040008CD RID: 2253
		private int state;

		// Token: 0x040008CE RID: 2254
		private Vector3 bladeStart;

		// Token: 0x040008CF RID: 2255
		private Vector3 bladeDir;

		// Token: 0x040008D0 RID: 2256
		private float bladeTime;

		// Token: 0x040008D1 RID: 2257
		private float bladeIntensity;
	}
}
