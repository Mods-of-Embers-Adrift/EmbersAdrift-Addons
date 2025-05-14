using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B5F RID: 2911
	[CreateAssetMenu(menuName = "SoL/UMA/Slider Restrictions", order = 5)]
	public class UMADNARestrictions : ScriptableObject, ISerializationCallbackReceiver
	{
		// Token: 0x0600598A RID: 22922 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x0600598B RID: 22923 RVA: 0x001EA3C4 File Offset: 0x001E85C4
		public void OnAfterDeserialize()
		{
			this.m_restrictionDict = new Dictionary<UMADnaType, DNASliderRestriction>(default(UMADnaTypeComparer));
			for (int i = 0; i < this.m_restrictions.Length; i++)
			{
				if (!this.m_restrictionDict.ContainsKey(this.m_restrictions[i].DnaType))
				{
					this.m_restrictionDict.Add(this.m_restrictions[i].DnaType, this.m_restrictions[i]);
				}
			}
		}

		// Token: 0x0600598C RID: 22924 RVA: 0x001EA438 File Offset: 0x001E8638
		public DNASliderRestriction GetRestriction(UMADnaType type)
		{
			DNASliderRestriction result = null;
			this.m_restrictionDict.TryGetValue(type, out result);
			return result;
		}

		// Token: 0x0600598D RID: 22925 RVA: 0x0007BFC0 File Offset: 0x0007A1C0
		public bool TryGetRestriction(UMADnaType type, out DNASliderRestriction restriction)
		{
			return this.m_restrictionDict.TryGetValue(type, out restriction);
		}

		// Token: 0x04004EC0 RID: 20160
		[SerializeField]
		private DNASliderRestriction[] m_restrictions;

		// Token: 0x04004EC1 RID: 20161
		private Dictionary<UMADnaType, DNASliderRestriction> m_restrictionDict;
	}
}
