using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DC3 RID: 3523
	public class UniqueIdComparisonTest : MonoBehaviour
	{
		// Token: 0x06006934 RID: 26932 RVA: 0x00086891 File Offset: 0x00084A91
		private void Update()
		{
			if (this.m_performComparison)
			{
				this.Compare();
				this.m_performComparison = false;
			}
		}

		// Token: 0x06006935 RID: 26933 RVA: 0x002171A8 File Offset: 0x002153A8
		private void Compare()
		{
			UniqueId uniqueId = UniqueId.GenerateFromGuid();
			UniqueId other = UniqueId.GenerateFromGuid();
			for (int i = 0; i < this.m_comparisonCount; i++)
			{
				uniqueId.Equals(other);
				uniqueId.GetHashCode();
				other.GetHashCode();
			}
		}

		// Token: 0x06006936 RID: 26934 RVA: 0x000868A8 File Offset: 0x00084AA8
		public void TriggerComparison()
		{
			if (!this.m_performComparison)
			{
				this.m_performComparison = true;
			}
		}

		// Token: 0x04005B98 RID: 23448
		[SerializeField]
		private int m_comparisonCount = 1000;

		// Token: 0x04005B99 RID: 23449
		private bool m_performComparison;
	}
}
