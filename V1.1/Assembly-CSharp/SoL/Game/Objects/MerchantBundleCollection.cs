using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009FA RID: 2554
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Misc/Merchant Bundle Collection", order = 5)]
	public class MerchantBundleCollection : ScriptableObject
	{
		// Token: 0x17001121 RID: 4385
		// (get) Token: 0x06004D9A RID: 19866 RVA: 0x00074759 File Offset: 0x00072959
		public MerchantBundle[] MerchantBundles
		{
			get
			{
				return this.m_merchantBundles;
			}
		}

		// Token: 0x06004D9B RID: 19867 RVA: 0x00063B81 File Offset: 0x00061D81
		private IEnumerable GetMerchantBundles()
		{
			return SolOdinUtilities.GetDropdownItems<MerchantBundle>();
		}

		// Token: 0x04004734 RID: 18228
		[SerializeField]
		private MerchantBundle[] m_merchantBundles;
	}
}
