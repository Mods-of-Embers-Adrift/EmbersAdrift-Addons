using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class HS_ParticleEndSound : MonoBehaviour
{
	// Token: 0x06000126 RID: 294 RVA: 0x00044FF3 File Offset: 0x000431F3
	private void Awake()
	{
		HS_ParticleEndSound.SharedInstance = this;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00097A10 File Offset: 0x00095C10
	private void Start()
	{
		this.poolDictionary = new Dictionary<string, Queue<GameObject>>();
		foreach (HS_ParticleEndSound.Pool pool in this.pools)
		{
			Queue<GameObject> queue = new Queue<GameObject>();
			for (int i = 0; i < pool.size; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(pool.prefab);
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (pool.tag == "AudioExplosion")
				{
					component.clip = this.audioExplosion[UnityEngine.Random.Range(0, this.audioExplosion.Length)];
					component.volume = UnityEngine.Random.Range(this.explosionMinVolume, this.explosionMaxVolume);
					component.pitch = UnityEngine.Random.Range(this.explosionPitchMin, this.explosionPitchMax);
				}
				else if (pool.tag == "AudioShot")
				{
					component.clip = this.audioShot[UnityEngine.Random.Range(0, this.audioExplosion.Length)];
					component.volume = UnityEngine.Random.Range(this.shootMinVolume, this.shootMaxVolume);
					component.pitch = UnityEngine.Random.Range(this.shootPitchMin, this.shootPitchMax);
				}
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.SetActive(false);
				queue.Enqueue(gameObject);
			}
			this.poolDictionary.Add(pool.tag, queue);
		}
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00097BA4 File Offset: 0x00095DA4
	public GameObject SpawnFromPool(string tag, Vector3 position)
	{
		if (!this.poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + " does not excist.");
			return null;
		}
		GameObject gameObject = this.poolDictionary[tag].Dequeue();
		gameObject.SetActive(true);
		gameObject.transform.position = position;
		this.poolDictionary[tag].Enqueue(gameObject);
		return gameObject;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00097C10 File Offset: 0x00095E10
	public void LateUpdate()
	{
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[base.GetComponent<ParticleSystem>().particleCount];
		int particles = base.GetComponent<ParticleSystem>().GetParticles(array);
		for (int i = 0; i < particles; i++)
		{
			if (this.audioExplosion.Length != 0 && array[i].remainingLifetime < Time.deltaTime)
			{
				GameObject gameObject = HS_ParticleEndSound.SharedInstance.SpawnFromPool("AudioExplosion", array[i].position);
				if (gameObject != null)
				{
					base.StartCoroutine(this.LateCall(gameObject));
				}
			}
			if (this.audioShot.Length != 0 && array[i].remainingLifetime >= array[i].startLifetime - Time.deltaTime)
			{
				GameObject gameObject2 = HS_ParticleEndSound.SharedInstance.SpawnFromPool("AudioShot", array[i].position);
				if (gameObject2 != null)
				{
					base.StartCoroutine(this.LateCall(gameObject2));
				}
			}
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00044FFB File Offset: 0x000431FB
	private IEnumerator LateCall(GameObject soundInstance)
	{
		yield return new WaitForSeconds(this.poolReturnTimer);
		soundInstance.SetActive(false);
		yield break;
	}

	// Token: 0x040002C8 RID: 712
	public float poolReturnTimer = 1.5f;

	// Token: 0x040002C9 RID: 713
	public float explosionMinVolume = 0.3f;

	// Token: 0x040002CA RID: 714
	public float explosionMaxVolume = 0.7f;

	// Token: 0x040002CB RID: 715
	public float explosionPitchMin = 0.75f;

	// Token: 0x040002CC RID: 716
	public float explosionPitchMax = 1.25f;

	// Token: 0x040002CD RID: 717
	public float shootMinVolume = 0.05f;

	// Token: 0x040002CE RID: 718
	public float shootMaxVolume = 0.1f;

	// Token: 0x040002CF RID: 719
	public float shootPitchMin = 0.75f;

	// Token: 0x040002D0 RID: 720
	public float shootPitchMax = 1.25f;

	// Token: 0x040002D1 RID: 721
	public AudioClip[] audioExplosion;

	// Token: 0x040002D2 RID: 722
	public AudioClip[] audioShot;

	// Token: 0x040002D3 RID: 723
	public static HS_ParticleEndSound SharedInstance;

	// Token: 0x040002D4 RID: 724
	public List<HS_ParticleEndSound.Pool> pools;

	// Token: 0x040002D5 RID: 725
	public Dictionary<string, Queue<GameObject>> poolDictionary;

	// Token: 0x0200003A RID: 58
	[Serializable]
	public class Pool
	{
		// Token: 0x040002D6 RID: 726
		public string tag;

		// Token: 0x040002D7 RID: 727
		public GameObject prefab;

		// Token: 0x040002D8 RID: 728
		public int size;
	}
}
