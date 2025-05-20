using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
[RequireComponent(typeof(AudioSource))]
public class FlockChildSound : MonoBehaviour
{
	// Token: 0x060000DD RID: 221 RVA: 0x00095A3C File Offset: 0x00093C3C
	public void Start()
	{
		this._flockChild = base.GetComponent<FlockChild>();
		this._audio = base.GetComponent<AudioSource>();
		base.InvokeRepeating("PlayRandomSound", UnityEngine.Random.value + 1f, 1f);
		if (this._scareSounds.Length != 0)
		{
			base.InvokeRepeating("ScareSound", 1f, 0.01f);
		}
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00095A9C File Offset: 0x00093C9C
	public void PlayRandomSound()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (!this._audio.isPlaying && this._flightSounds.Length != 0 && this._flightSoundRandomChance > UnityEngine.Random.value && !this._flockChild._landing)
			{
				this._audio.clip = this._flightSounds[UnityEngine.Random.Range(0, this._flightSounds.Length)];
				this._audio.pitch = UnityEngine.Random.Range(this._pitchMin, this._pitchMax);
				this._audio.volume = UnityEngine.Random.Range(this._volumeMin, this._volumeMax);
				this._audio.Play();
				return;
			}
			if (!this._audio.isPlaying && this._idleSounds.Length != 0 && this._idleSoundRandomChance > UnityEngine.Random.value && this._flockChild._landing)
			{
				this._audio.clip = this._idleSounds[UnityEngine.Random.Range(0, this._idleSounds.Length)];
				this._audio.pitch = UnityEngine.Random.Range(this._pitchMin, this._pitchMax);
				this._audio.volume = UnityEngine.Random.Range(this._volumeMin, this._volumeMax);
				this._audio.Play();
				this._hasLanded = true;
			}
		}
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00095BF0 File Offset: 0x00093DF0
	public void ScareSound()
	{
		if (base.gameObject.activeInHierarchy && this._hasLanded && !this._flockChild._landing && this._idleSoundRandomChance * 2f > UnityEngine.Random.value)
		{
			this._audio.clip = this._scareSounds[UnityEngine.Random.Range(0, this._scareSounds.Length)];
			this._audio.volume = UnityEngine.Random.Range(this._volumeMin, this._volumeMax);
			this._audio.PlayDelayed(UnityEngine.Random.value * 0.2f);
			this._hasLanded = false;
		}
	}

	// Token: 0x04000225 RID: 549
	public AudioClip[] _idleSounds;

	// Token: 0x04000226 RID: 550
	public float _idleSoundRandomChance = 0.05f;

	// Token: 0x04000227 RID: 551
	public AudioClip[] _flightSounds;

	// Token: 0x04000228 RID: 552
	public float _flightSoundRandomChance = 0.05f;

	// Token: 0x04000229 RID: 553
	public AudioClip[] _scareSounds;

	// Token: 0x0400022A RID: 554
	public float _pitchMin = 0.85f;

	// Token: 0x0400022B RID: 555
	public float _pitchMax = 1f;

	// Token: 0x0400022C RID: 556
	public float _volumeMin = 0.6f;

	// Token: 0x0400022D RID: 557
	public float _volumeMax = 0.8f;

	// Token: 0x0400022E RID: 558
	private FlockChild _flockChild;

	// Token: 0x0400022F RID: 559
	private AudioSource _audio;

	// Token: 0x04000230 RID: 560
	private bool _hasLanded;
}
