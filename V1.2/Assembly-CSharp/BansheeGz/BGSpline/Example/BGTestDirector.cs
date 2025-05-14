using System;
using System.Collections;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001F3 RID: 499
	public class BGTestDirector : MonoBehaviour
	{
		// Token: 0x0600114B RID: 4427 RVA: 0x000E3438 File Offset: 0x000E1638
		public void Sun(int point)
		{
			switch (point)
			{
			case 0:
				base.StartCoroutine(this.ChangeBackColor(BGTestDirector.NightColor, BGTestDirector.DayColor));
				base.StartCoroutine(this.ChangeDirectLightIntensity(0f, 0.8f));
				this.SunParticles.Play();
				return;
			case 1:
				this.SunLight.intensity = 1f;
				this.Stars.transform.localPosition += new Vector3(0f, -20f);
				return;
			case 2:
				break;
			case 3:
				this.Stars.transform.localPosition -= new Vector3(0f, -20f);
				this.SunLight.intensity = 0f;
				this.SunParticles.Stop();
				break;
			default:
				return;
			}
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x000E3518 File Offset: 0x000E1718
		public void Moon(int point)
		{
			switch (point)
			{
			case 0:
				base.StartCoroutine(this.ChangeBackColor(BGTestDirector.DayColor, BGTestDirector.NightColor));
				base.StartCoroutine(this.ChangeDirectLightIntensity(0.8f, 0f));
				this.StarsParticles.Play();
				return;
			case 1:
				this.MoonAnimator.SetBool("play", true);
				this.MoonLight.intensity = 1f;
				return;
			case 2:
				this.StarsParticles.Stop();
				return;
			case 3:
				this.MoonAnimator.SetBool("play", false);
				this.MoonLight.intensity = 0f;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0004E5D7 File Offset: 0x0004C7D7
		private IEnumerator ChangeBackColor(Color from, Color to)
		{
			float started = Time.time;
			while (Time.time - started < 1f)
			{
				Camera.main.backgroundColor = Color.Lerp(from, to, (Time.time - started) / 1f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0004E5ED File Offset: 0x0004C7ED
		private IEnumerator ChangeDirectLightIntensity(float from, float to)
		{
			float started = Time.time;
			while (Time.time - started < 1f)
			{
				this.DirectionalLight.intensity = Mathf.Lerp(from, to, (Time.time - started) / 1f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x04000EAF RID: 3759
		private static readonly Color NightColor = Color.black;

		// Token: 0x04000EB0 RID: 3760
		private static readonly Color DayColor = new Color32(176, 224, 240, byte.MaxValue);

		// Token: 0x04000EB1 RID: 3761
		public Light SunLight;

		// Token: 0x04000EB2 RID: 3762
		public Light DirectionalLight;

		// Token: 0x04000EB3 RID: 3763
		public ParticleSystem SunParticles;

		// Token: 0x04000EB4 RID: 3764
		public Animator MoonAnimator;

		// Token: 0x04000EB5 RID: 3765
		public Light MoonLight;

		// Token: 0x04000EB6 RID: 3766
		public ParticleSystem StarsParticles;

		// Token: 0x04000EB7 RID: 3767
		public GameObject Stars;
	}
}
