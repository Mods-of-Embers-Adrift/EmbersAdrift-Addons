using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Networking
{
	// Token: 0x020003CA RID: 970
	[CreateAssetMenu(menuName = "SoL/Networking/Prefab Collection", fileName = "NetworkedPrefabs", order = 5)]
	public class NetworkedPrefabCollection : ScriptableObject
	{
		// Token: 0x06001A04 RID: 6660 RVA: 0x00107CAC File Offset: 0x00105EAC
		public GameObject GetPrefabForIdOrName(string id)
		{
			this.InitializeLookup();
			GameObject result;
			this.m_prefabLookup.TryGetValue(id.GetHashCode(), out result);
			return result;
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x00107CD4 File Offset: 0x00105ED4
		private void InitializeLookup()
		{
			if (this.m_prefabLookup != null)
			{
				return;
			}
			this.m_prefabLookup = new Dictionary<int, GameObject>(this.m_prefabs.Count);
			for (int i = 0; i < this.m_prefabs.Count; i++)
			{
				if (!(this.m_prefabs[i].Prefab == null) && !string.IsNullOrEmpty(this.m_prefabs[i].Id.Value))
				{
					int hashCode = this.m_prefabs[i].Id.Value.GetHashCode();
					if (!this.m_prefabLookup.ContainsKey(hashCode))
					{
						this.m_prefabLookup.Add(hashCode, this.m_prefabs[i].Prefab);
					}
					if (!string.IsNullOrEmpty(this.m_prefabs[i].Name))
					{
						hashCode = this.m_prefabs[i].Name.GetHashCode();
						if (!this.m_prefabLookup.ContainsKey(hashCode))
						{
							this.m_prefabLookup.Add(hashCode, this.m_prefabs[i].Prefab);
						}
					}
				}
			}
		}

		// Token: 0x04002128 RID: 8488
		[SerializeField]
		private List<PrefabEntry> m_prefabs = new List<PrefabEntry>();

		// Token: 0x04002129 RID: 8489
		private Dictionary<int, GameObject> m_prefabLookup;
	}
}
