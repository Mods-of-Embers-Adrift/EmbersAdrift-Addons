using System;
using System.Globalization;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D9D RID: 3485
	public class DistanceTest : MonoBehaviour
	{
		// Token: 0x06006895 RID: 26773 RVA: 0x00215038 File Offset: 0x00213238
		private void Distance()
		{
			Debug.Log(base.gameObject.DistanceTo(this.m_target).ToString(CultureInfo.InvariantCulture));
		}

		// Token: 0x04005ADE RID: 23262
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04005ADF RID: 23263
		[SerializeField]
		private float m_yThreshold;
	}
}
