using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoL.Game.AssetBundles
{
	// Token: 0x02000D46 RID: 3398
	public class AddressableOnDestroy : MonoBehaviour
	{
		// Token: 0x1700187A RID: 6266
		// (get) Token: 0x06006654 RID: 26196 RVA: 0x00084E06 File Offset: 0x00083006
		// (set) Token: 0x06006655 RID: 26197 RVA: 0x00084E0E File Offset: 0x0008300E
		public AssetReference Reference { get; set; }

		// Token: 0x06006656 RID: 26198 RVA: 0x00084E17 File Offset: 0x00083017
		private void OnDestroy()
		{
			AddressableManager.DestroyInstance(this.Reference, base.gameObject);
		}
	}
}
