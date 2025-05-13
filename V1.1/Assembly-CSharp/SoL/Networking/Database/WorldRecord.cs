using System;
using Newtonsoft.Json;

namespace SoL.Networking.Database
{
	// Token: 0x0200046F RID: 1135
	public class WorldRecord
	{
		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06001FD4 RID: 8148 RVA: 0x000574DC File Offset: 0x000556DC
		[JsonIgnore]
		public AccessFlags Flags
		{
			get
			{
				return (AccessFlags)this.m_flags;
			}
		}

		// Token: 0x0400253D RID: 9533
		[JsonProperty(PropertyName = "_id")]
		public string Id;

		// Token: 0x0400253E RID: 9534
		[JsonProperty(PropertyName = "world_id")]
		public int WorldId;

		// Token: 0x0400253F RID: 9535
		[JsonProperty(PropertyName = "world_name")]
		public string Name;

		// Token: 0x04002540 RID: 9536
		[JsonProperty(PropertyName = "ip")]
		public string Address;

		// Token: 0x04002541 RID: 9537
		[JsonProperty(PropertyName = "port")]
		public int Port;

		// Token: 0x04002542 RID: 9538
		[JsonProperty(PropertyName = "tier")]
		public int Tier;

		// Token: 0x04002543 RID: 9539
		[JsonProperty(PropertyName = "flags")]
		private int m_flags;

		// Token: 0x04002544 RID: 9540
		[JsonProperty(PropertyName = "isOnline")]
		public bool IsOnline;
	}
}
