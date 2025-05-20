using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200061A RID: 1562
	[Serializable]
	public struct HumanoidReferencePoints
	{
		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x0600318A RID: 12682 RVA: 0x00062288 File Offset: 0x00060488
		// (set) Token: 0x0600318B RID: 12683 RVA: 0x00062290 File Offset: 0x00060490
		public Transform HeadTransform { readonly get; set; }

		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x0600318C RID: 12684 RVA: 0x00062299 File Offset: 0x00060499
		// (set) Token: 0x0600318D RID: 12685 RVA: 0x000622A1 File Offset: 0x000604A1
		public Transform LeftEyeTransform { readonly get; set; }

		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x0600318E RID: 12686 RVA: 0x000622AA File Offset: 0x000604AA
		// (set) Token: 0x0600318F RID: 12687 RVA: 0x000622B2 File Offset: 0x000604B2
		public Transform RightEyeTransform { readonly get; set; }

		// Token: 0x06003190 RID: 12688 RVA: 0x0015D054 File Offset: 0x0015B254
		public void DestroyReferencePoints()
		{
			this.DestroyPoint(this.LeftMount);
			this.DestroyPoint(this.RightMount);
			this.DestroyPoint(this.BackMount);
			this.DestroyPoint(this.LeftHipMount);
			this.DestroyPoint(this.RightHipMount);
			this.DestroyPoint(this.LeftShoulderMount);
			this.DestroyPoint(this.RightShoulderMount);
			this.DestroyPoint(this.EyeHeight);
			this.DestroyPoint(this.Overhead);
			this.DestroyPoint(this.DamageTarget);
		}

		// Token: 0x06003191 RID: 12689 RVA: 0x000622BB File Offset: 0x000604BB
		private void DestroyPoint(GameObject obj)
		{
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		// Token: 0x04002FEE RID: 12270
		public GameObject LeftMount;

		// Token: 0x04002FEF RID: 12271
		public GameObject RightMount;

		// Token: 0x04002FF0 RID: 12272
		public GameObject BackMount;

		// Token: 0x04002FF1 RID: 12273
		public GameObject LeftHipMount;

		// Token: 0x04002FF2 RID: 12274
		public GameObject RightHipMount;

		// Token: 0x04002FF3 RID: 12275
		public GameObject LeftShoulderMount;

		// Token: 0x04002FF4 RID: 12276
		public GameObject RightShoulderMount;

		// Token: 0x04002FF5 RID: 12277
		public GameObject EyeHeight;

		// Token: 0x04002FF6 RID: 12278
		public GameObject Overhead;

		// Token: 0x04002FF7 RID: 12279
		public GameObject DamageTarget;
	}
}
