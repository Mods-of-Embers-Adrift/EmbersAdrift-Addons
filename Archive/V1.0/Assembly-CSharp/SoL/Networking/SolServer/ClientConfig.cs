using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003D6 RID: 982
	[Serializable]
	public class ClientConfig
	{
		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06001A57 RID: 6743 RVA: 0x00054807 File Offset: 0x00052A07
		// (set) Token: 0x06001A58 RID: 6744 RVA: 0x0005480F File Offset: 0x00052A0F
		[JsonProperty]
		public string LoginHost
		{
			get
			{
				return this.m_loginHost;
			}
			set
			{
				this.m_loginHost = value;
			}
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06001A59 RID: 6745 RVA: 0x00054818 File Offset: 0x00052A18
		// (set) Token: 0x06001A5A RID: 6746 RVA: 0x00054820 File Offset: 0x00052A20
		[JsonProperty]
		public string LoginPort
		{
			get
			{
				return this.m_loginPort;
			}
			set
			{
				this.m_loginPort = value;
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06001A5B RID: 6747 RVA: 0x00054829 File Offset: 0x00052A29
		// (set) Token: 0x06001A5C RID: 6748 RVA: 0x00054831 File Offset: 0x00052A31
		[JsonProperty]
		public string ZoneHost
		{
			get
			{
				return this.m_zoneHost;
			}
			set
			{
				this.m_zoneHost = value;
			}
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x0005483A File Offset: 0x00052A3A
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"LoginHost: ",
				this.LoginHost,
				"\nLoginPort: ",
				this.LoginPort,
				"\nZoneHost: ",
				this.ZoneHost
			});
		}

		// Token: 0x0400214A RID: 8522
		[SerializeField]
		private string m_loginHost;

		// Token: 0x0400214B RID: 8523
		[SerializeField]
		private string m_loginPort;

		// Token: 0x0400214C RID: 8524
		[SerializeField]
		private string m_zoneHost;
	}
}
