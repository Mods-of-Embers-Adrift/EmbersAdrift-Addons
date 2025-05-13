using System;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD6 RID: 3542
	[Serializable]
	internal class ComparisonResult
	{
		// Token: 0x17001924 RID: 6436
		// (get) Token: 0x0600696F RID: 26991 RVA: 0x00086B84 File Offset: 0x00084D84
		// (set) Token: 0x06006970 RID: 26992 RVA: 0x00086B8C File Offset: 0x00084D8C
		[HideInInspector]
		public bool HidePosition { get; set; }

		// Token: 0x17001925 RID: 6437
		// (get) Token: 0x06006971 RID: 26993 RVA: 0x00086B95 File Offset: 0x00084D95
		// (set) Token: 0x06006972 RID: 26994 RVA: 0x00086B9D File Offset: 0x00084D9D
		[HideInInspector]
		public bool HideRotation { get; set; }

		// Token: 0x17001926 RID: 6438
		// (get) Token: 0x06006973 RID: 26995 RVA: 0x00086BA6 File Offset: 0x00084DA6
		// (set) Token: 0x06006974 RID: 26996 RVA: 0x00086BAE File Offset: 0x00084DAE
		[HideInInspector]
		public bool HideScale { get; set; }

		// Token: 0x17001927 RID: 6439
		// (get) Token: 0x06006975 RID: 26997 RVA: 0x00086BB7 File Offset: 0x00084DB7
		public Vector3 PosSmall
		{
			get
			{
				ComparisonValues smaller = this.Smaller;
				if (smaller == null)
				{
					return Vector3.zero;
				}
				return smaller.PosDelta;
			}
		}

		// Token: 0x17001928 RID: 6440
		// (get) Token: 0x06006976 RID: 26998 RVA: 0x00086BCE File Offset: 0x00084DCE
		public Vector3 PosLarge
		{
			get
			{
				ComparisonValues bigger = this.Bigger;
				if (bigger == null)
				{
					return Vector3.zero;
				}
				return bigger.PosDelta;
			}
		}

		// Token: 0x17001929 RID: 6441
		// (get) Token: 0x06006977 RID: 26999 RVA: 0x00086BE5 File Offset: 0x00084DE5
		public Vector3 RotSmall
		{
			get
			{
				ComparisonValues smaller = this.Smaller;
				if (smaller == null)
				{
					return Vector3.zero;
				}
				return smaller.RotDelta;
			}
		}

		// Token: 0x1700192A RID: 6442
		// (get) Token: 0x06006978 RID: 27000 RVA: 0x00086BFC File Offset: 0x00084DFC
		public Vector3 RotLarge
		{
			get
			{
				ComparisonValues bigger = this.Bigger;
				if (bigger == null)
				{
					return Vector3.zero;
				}
				return bigger.RotDelta;
			}
		}

		// Token: 0x1700192B RID: 6443
		// (get) Token: 0x06006979 RID: 27001 RVA: 0x00086C13 File Offset: 0x00084E13
		public Vector3 SizeSmall
		{
			get
			{
				ComparisonValues smaller = this.Smaller;
				if (smaller == null)
				{
					return Vector3.zero;
				}
				return smaller.SizeDelta;
			}
		}

		// Token: 0x1700192C RID: 6444
		// (get) Token: 0x0600697A RID: 27002 RVA: 0x00086C2A File Offset: 0x00084E2A
		public Vector3 SizeLarge
		{
			get
			{
				ComparisonValues bigger = this.Bigger;
				if (bigger == null)
				{
					return Vector3.zero;
				}
				return bigger.SizeDelta;
			}
		}

		// Token: 0x04005BE4 RID: 23524
		public string BoneName;

		// Token: 0x04005BE5 RID: 23525
		[HideInInspector]
		public ComparisonValues Smaller;

		// Token: 0x04005BE6 RID: 23526
		[HideInInspector]
		public ComparisonValues Bigger;
	}
}
