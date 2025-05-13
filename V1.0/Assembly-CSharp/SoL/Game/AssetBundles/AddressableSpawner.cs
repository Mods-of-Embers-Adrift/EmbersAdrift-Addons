using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoL.Game.AssetBundles
{
	// Token: 0x02000D47 RID: 3399
	public class AddressableSpawner : MonoBehaviour
	{
		// Token: 0x1700187B RID: 6267
		// (get) Token: 0x06006658 RID: 26200 RVA: 0x00084E2A File Offset: 0x0008302A
		// (set) Token: 0x06006659 RID: 26201 RVA: 0x00084E32 File Offset: 0x00083032
		public IAddressableSpawnedNotifier ToNotify { get; set; }

		// Token: 0x0600665A RID: 26202 RVA: 0x00084E3B File Offset: 0x0008303B
		private Transform GetTargetTransform()
		{
			if (!(this.m_parentTransformOverride == null))
			{
				return this.m_parentTransformOverride;
			}
			return base.gameObject.transform;
		}

		// Token: 0x0600665B RID: 26203 RVA: 0x00084E5D File Offset: 0x0008305D
		private void Start()
		{
			if (GameManager.IsServer && !this.m_allowOnServer)
			{
				return;
			}
			AddressableManager.SpawnInstance(this.m_reference, this.GetTargetTransform(), this.ToNotify);
		}

		// Token: 0x040058F3 RID: 22771
		[SerializeField]
		private bool m_allowOnServer;

		// Token: 0x040058F4 RID: 22772
		[SerializeField]
		private Transform m_parentTransformOverride;

		// Token: 0x040058F6 RID: 22774
		[SerializeField]
		private AssetReference m_reference;
	}
}
