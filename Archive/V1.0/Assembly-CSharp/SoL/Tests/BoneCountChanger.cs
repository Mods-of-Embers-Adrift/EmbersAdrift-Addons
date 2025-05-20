using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D8F RID: 3471
	public class BoneCountChanger : MonoBehaviour
	{
		// Token: 0x0600685E RID: 26718 RVA: 0x00086129 File Offset: 0x00084329
		private void One()
		{
			this.SetBoneCount(SkinQuality.Bone1);
		}

		// Token: 0x0600685F RID: 26719 RVA: 0x00086132 File Offset: 0x00084332
		private void Two()
		{
			this.SetBoneCount(SkinQuality.Bone2);
		}

		// Token: 0x06006860 RID: 26720 RVA: 0x0008613B File Offset: 0x0008433B
		private void Four()
		{
			this.SetBoneCount(SkinQuality.Bone4);
		}

		// Token: 0x06006861 RID: 26721 RVA: 0x00086144 File Offset: 0x00084344
		private void SetBoneCount(SkinQuality quality)
		{
			if (this.m_renderer && this.m_renderer.quality != quality)
			{
				this.m_renderer.quality = quality;
			}
		}

		// Token: 0x04005A96 RID: 23190
		[SerializeField]
		private SkinnedMeshRenderer m_renderer;
	}
}
