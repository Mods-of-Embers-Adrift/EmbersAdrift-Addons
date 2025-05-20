using System;
using System.Collections.Generic;
using UnityEngine;

namespace CafofoStudio
{
	// Token: 0x02000050 RID: 80
	public abstract class AmbienceMixer<P> : MonoBehaviour where P : AmbientPreset
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600019A RID: 410
		[SerializeField]
		protected abstract List<SoundElement> elements { get; }

		// Token: 0x0600019B RID: 411 RVA: 0x0009A474 File Offset: 0x00098674
		private void OnEnable()
		{
			foreach (SoundElement soundElement in this.elements)
			{
				soundElement.InitializeAudioSources(base.gameObject);
			}
			if (this.playOnAwake)
			{
				this.Play();
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0009A4D8 File Offset: 0x000986D8
		private void Update()
		{
			foreach (SoundElement soundElement in this.elements)
			{
				soundElement.UpdateSampleTimer();
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0009A528 File Offset: 0x00098728
		public void Play()
		{
			foreach (SoundElement soundElement in this.elements)
			{
				soundElement.Play();
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0009A578 File Offset: 0x00098778
		public void Stop()
		{
			foreach (SoundElement soundElement in this.elements)
			{
				soundElement.Stop();
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0009A5C8 File Offset: 0x000987C8
		private void OnDisable()
		{
			AudioSource[] components = base.GetComponents<AudioSource>();
			for (int i = 0; i < components.Length; i++)
			{
				UnityEngine.Object.Destroy(components[i]);
			}
		}

		// Token: 0x060001A0 RID: 416
		public abstract void ApplyPreset(P selectedPreset);

		// Token: 0x0400039A RID: 922
		public bool playOnAwake = true;

		// Token: 0x0400039B RID: 923
		[SerializeField]
		public List<P> presets;
	}
}
