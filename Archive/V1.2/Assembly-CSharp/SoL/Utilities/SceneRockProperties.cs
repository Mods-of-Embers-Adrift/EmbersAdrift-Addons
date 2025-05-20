using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002DC RID: 732
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class SceneRockProperties : MonoBehaviour
	{
		// Token: 0x06001513 RID: 5395 RVA: 0x00050BCC File Offset: 0x0004EDCC
		private void Update()
		{
			Shader.SetGlobalColor(SceneRockProperties.kAlbedoColorId, this.m_color);
			Shader.SetGlobalFloat(SceneRockProperties.kTopMultiplier, this.m_topMultiplier);
			Shader.SetGlobalVector(SceneRockProperties.kTopDirection, this.m_topDirection);
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x00050C03 File Offset: 0x0004EE03
		private void ResetColor()
		{
			this.m_color = Color.white;
		}

		// Token: 0x04001D52 RID: 7506
		private const string kPrefix = "Rock";

		// Token: 0x04001D53 RID: 7507
		private const string kColorPalette = "Rock";

		// Token: 0x04001D54 RID: 7508
		private static readonly int kAlbedoColorId = Shader.PropertyToID("RockColor");

		// Token: 0x04001D55 RID: 7509
		private static readonly int kTopMultiplier = Shader.PropertyToID("RockTopMultiplier");

		// Token: 0x04001D56 RID: 7510
		private static readonly int kTopDirection = Shader.PropertyToID("RockTopDirection");

		// Token: 0x04001D57 RID: 7511
		[SerializeField]
		private Color m_color = Color.white;

		// Token: 0x04001D58 RID: 7512
		[Range(0f, 1f)]
		[SerializeField]
		private float m_topMultiplier = 1f;

		// Token: 0x04001D59 RID: 7513
		[SerializeField]
		private Vector3 m_topDirection = Vector3.up;
	}
}
