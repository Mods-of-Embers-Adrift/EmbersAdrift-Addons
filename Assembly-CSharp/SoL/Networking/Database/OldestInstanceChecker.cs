using System;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x02000462 RID: 1122
	public class OldestInstanceChecker : MonoBehaviour
	{
		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06001F9F RID: 8095 RVA: 0x0012035C File Offset: 0x0011E55C
		// (remove) Token: 0x06001FA0 RID: 8096 RVA: 0x00120390 File Offset: 0x0011E590
		public static event Action IsOldestInstanceChanged;

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06001FA1 RID: 8097 RVA: 0x0005734E File Offset: 0x0005554E
		// (set) Token: 0x06001FA2 RID: 8098 RVA: 0x00057356 File Offset: 0x00055556
		public bool IsOldestInstance
		{
			get
			{
				return this.m_isOldestInstance;
			}
			set
			{
				if (this.m_isOldestInstance == value)
				{
					return;
				}
				this.m_isOldestInstance = value;
				Action isOldestInstanceChanged = OldestInstanceChecker.IsOldestInstanceChanged;
				if (isOldestInstanceChanged == null)
				{
					return;
				}
				isOldestInstanceChanged();
			}
		}

		// Token: 0x04002502 RID: 9474
		public static OldestInstanceChecker Instance;

		// Token: 0x04002504 RID: 9476
		private bool m_isOldestInstance;
	}
}
