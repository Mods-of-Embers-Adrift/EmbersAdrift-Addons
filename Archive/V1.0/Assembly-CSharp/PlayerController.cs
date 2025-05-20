using System;
using ProceduralWorlds.RealIvy;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class PlayerController : MonoBehaviour
{
	// Token: 0x0600009F RID: 159 RVA: 0x00044C0A File Offset: 0x00042E0A
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			this.ivyCaster.CastRandomIvy(this.trIvy.position, this.trIvy.rotation);
		}
	}

	// Token: 0x040001C5 RID: 453
	public IvyCaster ivyCaster;

	// Token: 0x040001C6 RID: 454
	public Transform trIvy;
}
