using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000AB RID: 171
	public class LightningCustomTransformStateInfo
	{
		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x00047709 File Offset: 0x00045909
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x00047711 File Offset: 0x00045911
		public LightningCustomTransformState State { get; set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x0004771A File Offset: 0x0004591A
		// (set) Token: 0x0600067B RID: 1659 RVA: 0x00047722 File Offset: 0x00045922
		public LightningBoltParameters Parameters { get; set; }

		// Token: 0x0600067C RID: 1660 RVA: 0x000A896C File Offset: 0x000A6B6C
		public static LightningCustomTransformStateInfo GetOrCreateStateInfo()
		{
			if (LightningCustomTransformStateInfo.cache.Count == 0)
			{
				return new LightningCustomTransformStateInfo();
			}
			int index = LightningCustomTransformStateInfo.cache.Count - 1;
			LightningCustomTransformStateInfo result = LightningCustomTransformStateInfo.cache[index];
			LightningCustomTransformStateInfo.cache.RemoveAt(index);
			return result;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x000A89B0 File Offset: 0x000A6BB0
		public static void ReturnStateInfoToCache(LightningCustomTransformStateInfo info)
		{
			if (info != null)
			{
				info.Transform = (info.StartTransform = (info.EndTransform = null));
				info.UserInfo = null;
				LightningCustomTransformStateInfo.cache.Add(info);
			}
		}

		// Token: 0x04000786 RID: 1926
		public Vector3 BoltStartPosition;

		// Token: 0x04000787 RID: 1927
		public Vector3 BoltEndPosition;

		// Token: 0x04000788 RID: 1928
		public Transform Transform;

		// Token: 0x04000789 RID: 1929
		public Transform StartTransform;

		// Token: 0x0400078A RID: 1930
		public Transform EndTransform;

		// Token: 0x0400078B RID: 1931
		public object UserInfo;

		// Token: 0x0400078C RID: 1932
		private static readonly List<LightningCustomTransformStateInfo> cache = new List<LightningCustomTransformStateInfo>();
	}
}
