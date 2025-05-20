using System;
using NetStack.Serialization;

namespace SoL.Networking
{
	// Token: 0x020003CF RID: 975
	public interface ISynchronizedVariable
	{
		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06001A42 RID: 6722
		bool IsDefault { get; }

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06001A43 RID: 6723
		bool Dirty { get; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06001A44 RID: 6724
		int BitFlag { get; }

		// Token: 0x06001A45 RID: 6725
		void SetDirtyFlags(DateTime lastUpdate);

		// Token: 0x06001A46 RID: 6726
		BitBuffer PackData(BitBuffer buffer);

		// Token: 0x06001A47 RID: 6727
		BitBuffer ReadData(BitBuffer buffer);

		// Token: 0x06001A48 RID: 6728
		BitBuffer PackInitialData(BitBuffer buffer);

		// Token: 0x06001A49 RID: 6729
		BitBuffer ReadDataFromClient(BitBuffer buffer);

		// Token: 0x06001A4A RID: 6730
		void ClearMonoReferences();
	}
}
