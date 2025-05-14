using System;
using System.Collections.Generic;
using SoL.Game.Pooling;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DB3 RID: 3507
	public class PooledObjectTests : MonoBehaviour
	{
		// Token: 0x060068FA RID: 26874 RVA: 0x002163C4 File Offset: 0x002145C4
		private void StartNew()
		{
			PooledObject pooledInstance = this.PO.GetPooledInstance<PooledObject>();
			Vector3 position = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
			Quaternion rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)));
			pooledInstance.transform.SetPositionAndRotation(position, rotation);
			this.m_objects.Add(pooledInstance);
		}

		// Token: 0x060068FB RID: 26875 RVA: 0x00216464 File Offset: 0x00214664
		private void EndOne()
		{
			if (this.m_objects.Count <= 0)
			{
				return;
			}
			int index = UnityEngine.Random.Range(0, this.m_objects.Count);
			this.m_objects[index].ReturnToPool();
			this.m_objects.RemoveAt(index);
		}

		// Token: 0x04005B5A RID: 23386
		[SerializeField]
		private PooledObject PO;

		// Token: 0x04005B5B RID: 23387
		private List<PooledObject> m_objects = new List<PooledObject>();
	}
}
