using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005C9 RID: 1481
	[ExecuteInEditMode]
	public class AmbientSkyColorToFogColor : MonoBehaviour
	{
		// Token: 0x04002E50 RID: 11856
		[Range(0f, 1f)]
		[SerializeField]
		private float m_valueReduction = 0.12f;
	}
}
