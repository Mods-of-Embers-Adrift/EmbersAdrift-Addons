using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class AutoDestroyPS : MonoBehaviour
{
	// Token: 0x0600013E RID: 318 RVA: 0x00098484 File Offset: 0x00096684
	private void Awake()
	{
		ParticleSystem.MainModule main = base.GetComponent<ParticleSystem>().main;
		this.timeLeft = main.startLifetimeMultiplier + main.duration;
		UnityEngine.Object.Destroy(base.gameObject, this.timeLeft);
	}

	// Token: 0x040002F2 RID: 754
	private float timeLeft;
}
