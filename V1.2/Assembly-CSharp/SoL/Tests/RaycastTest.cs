using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DB4 RID: 3508
	public class RaycastTest : MonoBehaviour
	{
		// Token: 0x060068FD RID: 26877 RVA: 0x002164B0 File Offset: 0x002146B0
		private void RaycastStuff()
		{
			int layerMask = 1 << LayerMask.NameToLayer(this.m_layerName);
			RaycastHit[] array = new RaycastHit[10];
			Vector3 position = base.gameObject.transform.position;
			Debug.DrawLine(position, new Vector3(position.x, position.y - this.m_distance, position.z), Color.green);
			int num = Physics.RaycastNonAlloc(base.gameObject.transform.position, Vector3.down, array, this.m_distance, layerMask);
			Debug.Log(string.Concat(new string[]
			{
				"LayerName: ",
				this.m_layerName,
				" (",
				layerMask.ToString(),
				"), nHits = ",
				num.ToString()
			}));
			for (int i = 0; i < num; i++)
			{
				Debug.Log(array[i].collider.gameObject.name);
			}
		}

		// Token: 0x04005B5C RID: 23388
		[SerializeField]
		private string m_layerName;

		// Token: 0x04005B5D RID: 23389
		[SerializeField]
		private float m_distance = 100f;
	}
}
