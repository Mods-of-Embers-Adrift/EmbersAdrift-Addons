using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002BF RID: 703
	public class RuntimeLayerAssignment : MonoBehaviour
	{
		// Token: 0x060014BA RID: 5306 RVA: 0x0005070E File Offset: 0x0004E90E
		private void Awake()
		{
			base.gameObject.layer = this.m_layer.LayerIndex;
		}

		// Token: 0x04001CF4 RID: 7412
		[SerializeField]
		private SingleUnityLayer m_layer = new SingleUnityLayer();
	}
}
