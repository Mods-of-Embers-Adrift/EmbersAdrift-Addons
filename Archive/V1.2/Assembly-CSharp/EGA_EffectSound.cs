using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
public class EGA_EffectSound : MonoBehaviour
{
	// Token: 0x06000140 RID: 320 RVA: 0x000984C4 File Offset: 0x000966C4
	private void Start()
	{
		this.soundComponent = base.GetComponent<AudioSource>();
		this.clip = this.soundComponent.clip;
		if (this.RandomVolume)
		{
			this.soundComponent.volume = UnityEngine.Random.Range(this.minVolume, this.maxVolume);
			this.RepeatSound();
		}
		if (this.Repeating)
		{
			base.InvokeRepeating("RepeatSound", this.StartTime, this.RepeatTime);
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0004503B File Offset: 0x0004323B
	private void RepeatSound()
	{
		this.soundComponent.PlayOneShot(this.clip);
	}

	// Token: 0x040002F3 RID: 755
	public bool Repeating = true;

	// Token: 0x040002F4 RID: 756
	public float RepeatTime = 2f;

	// Token: 0x040002F5 RID: 757
	public float StartTime;

	// Token: 0x040002F6 RID: 758
	public bool RandomVolume;

	// Token: 0x040002F7 RID: 759
	public float minVolume = 0.4f;

	// Token: 0x040002F8 RID: 760
	public float maxVolume = 1f;

	// Token: 0x040002F9 RID: 761
	private AudioClip clip;

	// Token: 0x040002FA RID: 762
	private AudioSource soundComponent;
}
