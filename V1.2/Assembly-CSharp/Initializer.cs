using System;
using ENet;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class Initializer : MonoBehaviour
{
	// Token: 0x06000029 RID: 41 RVA: 0x0004484D File Offset: 0x00042A4D
	private void Awake()
	{
		Library.Initialize();
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00044855 File Offset: 0x00042A55
	private void OnDestroy()
	{
		Library.Deinitialize();
	}
}
