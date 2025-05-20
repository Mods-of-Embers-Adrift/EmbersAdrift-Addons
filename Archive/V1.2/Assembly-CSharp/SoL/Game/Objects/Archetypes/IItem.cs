using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A2A RID: 2602
	public interface IItem
	{
		// Token: 0x06005092 RID: 20626
		bool CanUse();

		// Token: 0x06005093 RID: 20627
		void OnUse();
	}
}
