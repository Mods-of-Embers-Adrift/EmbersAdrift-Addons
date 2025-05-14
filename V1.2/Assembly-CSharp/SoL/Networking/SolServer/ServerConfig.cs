using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003D8 RID: 984
	[Serializable]
	public class ServerConfig
	{
		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06001A67 RID: 6759 RVA: 0x000548B6 File Offset: 0x00052AB6
		// (set) Token: 0x06001A68 RID: 6760 RVA: 0x000548BE File Offset: 0x00052ABE
		[JsonProperty]
		public string Host
		{
			get
			{
				return this.m_host;
			}
			set
			{
				this.m_host = value;
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06001A69 RID: 6761 RVA: 0x000548C7 File Offset: 0x00052AC7
		// (set) Token: 0x06001A6A RID: 6762 RVA: 0x000548CF File Offset: 0x00052ACF
		[JsonProperty]
		public string Port
		{
			get
			{
				return this.m_port;
			}
			set
			{
				this.m_port = value;
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06001A6B RID: 6763 RVA: 0x000548D8 File Offset: 0x00052AD8
		// (set) Token: 0x06001A6C RID: 6764 RVA: 0x000548E0 File Offset: 0x00052AE0
		[JsonProperty]
		public string Database
		{
			get
			{
				return this.m_database;
			}
			set
			{
				this.m_database = value;
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06001A6D RID: 6765 RVA: 0x000548E9 File Offset: 0x00052AE9
		// (set) Token: 0x06001A6E RID: 6766 RVA: 0x000548F1 File Offset: 0x00052AF1
		[JsonProperty]
		public string Username
		{
			get
			{
				return this.m_username;
			}
			set
			{
				this.m_username = value;
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06001A6F RID: 6767 RVA: 0x000548FA File Offset: 0x00052AFA
		// (set) Token: 0x06001A70 RID: 6768 RVA: 0x00054902 File Offset: 0x00052B02
		[JsonProperty]
		public string Password
		{
			get
			{
				return this.m_password;
			}
			set
			{
				this.m_password = value;
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06001A71 RID: 6769 RVA: 0x0005490B File Offset: 0x00052B0B
		// (set) Token: 0x06001A72 RID: 6770 RVA: 0x00054913 File Offset: 0x00052B13
		[JsonProperty]
		public int PortRangeMin
		{
			get
			{
				return this.m_portRangeMin;
			}
			set
			{
				this.m_portRangeMin = value;
			}
		}

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06001A73 RID: 6771 RVA: 0x0005491C File Offset: 0x00052B1C
		// (set) Token: 0x06001A74 RID: 6772 RVA: 0x00054924 File Offset: 0x00052B24
		[JsonProperty]
		public int PortRangeMax
		{
			get
			{
				return this.m_portRangeMax;
			}
			set
			{
				this.m_portRangeMax = value;
			}
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06001A75 RID: 6773 RVA: 0x0005492D File Offset: 0x00052B2D
		// (set) Token: 0x06001A76 RID: 6774 RVA: 0x00054935 File Offset: 0x00052B35
		[JsonProperty]
		public string ElasticUri
		{
			get
			{
				return this.m_elasticUri;
			}
			set
			{
				this.m_elasticUri = value;
			}
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x001087A0 File Offset: 0x001069A0
		public override string ToString()
		{
			string text = new string('*', this.Password.Length);
			return string.Concat(new string[]
			{
				"Host: ",
				this.Host,
				"\nPort: ",
				this.Port,
				"\nDB: ",
				this.Database,
				"\nUN: ",
				this.Username,
				"\nPW: ",
				text,
				"\nPort Range: ",
				this.PortRangeMin.ToString(),
				"-",
				this.PortRangeMax.ToString(),
				"\nElasticURI: ",
				this.ElasticUri
			});
		}

		// Token: 0x04002151 RID: 8529
		[SerializeField]
		private string m_host;

		// Token: 0x04002152 RID: 8530
		[SerializeField]
		private string m_port;

		// Token: 0x04002153 RID: 8531
		[SerializeField]
		private string m_database;

		// Token: 0x04002154 RID: 8532
		[SerializeField]
		private string m_username;

		// Token: 0x04002155 RID: 8533
		[SerializeField]
		private string m_password;

		// Token: 0x04002156 RID: 8534
		[SerializeField]
		private int m_portRangeMin;

		// Token: 0x04002157 RID: 8535
		[SerializeField]
		private int m_portRangeMax;

		// Token: 0x04002158 RID: 8536
		[SerializeField]
		private string m_elasticUri;
	}
}
