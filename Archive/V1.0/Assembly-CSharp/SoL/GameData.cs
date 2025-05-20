using System;
using UnityEngine;

namespace SoL
{
	// Token: 0x02000210 RID: 528
	[CreateAssetMenu(menuName = "SoL/GameVersion")]
	public class GameData : ScriptableObject
	{
		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x060011B5 RID: 4533 RVA: 0x0004EAFC File Offset: 0x0004CCFC
		private bool m_showUpdateApiTimestamp
		{
			get
			{
				return this.m_previousServerApiVersion != this.m_serverApiVersion;
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x060011B6 RID: 4534 RVA: 0x0004EB0F File Offset: 0x0004CD0F
		public string ServerApiVersion
		{
			get
			{
				return this.m_serverApiVersion;
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x060011B7 RID: 4535 RVA: 0x0004EB17 File Offset: 0x0004CD17
		public long LastServerApiUpdate
		{
			get
			{
				return this.m_lastServerApiUpdate;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x060011B8 RID: 4536 RVA: 0x0004EB1F File Offset: 0x0004CD1F
		public string BuildDate
		{
			get
			{
				return this.m_buildDate;
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x060011B9 RID: 4537 RVA: 0x0004EB27 File Offset: 0x0004CD27
		public string BuildVersion
		{
			get
			{
				return this.m_buildVersion;
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x060011BA RID: 4538 RVA: 0x0004EB2F File Offset: 0x0004CD2F
		public string DeploymentBranch
		{
			get
			{
				return this.m_deploymentBranch;
			}
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x000E4604 File Offset: 0x000E2804
		public string GetDataString()
		{
			string text = "";
			string text2 = "WIN";
			return string.Concat(new string[]
			{
				text2,
				text,
				"_",
				this.m_deploymentBranch.ToUpper(),
				"_b",
				this.m_buildVersion,
				"_",
				this.m_buildDate
			});
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x060011BC RID: 4540 RVA: 0x0004EB37 File Offset: 0x0004CD37
		public bool UsingGraphicsJobs
		{
			get
			{
				return this.m_useGraphicsJobs;
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x060011BD RID: 4541 RVA: 0x0004EB3F File Offset: 0x0004CD3F
		public bool UsingIncrementalGc
		{
			get
			{
				return this.m_useIncrementalGc;
			}
		}

		// Token: 0x04000F87 RID: 3975
		private const int kMaxPreviousToStore = 5;

		// Token: 0x04000F88 RID: 3976
		[SerializeField]
		private string m_buildDate;

		// Token: 0x04000F89 RID: 3977
		[SerializeField]
		private string m_buildVersion;

		// Token: 0x04000F8A RID: 3978
		[SerializeField]
		private string m_deploymentBranch;

		// Token: 0x04000F8B RID: 3979
		private const string kServerApiGroupName = "Server API";

		// Token: 0x04000F8C RID: 3980
		private const string kServerApiButtonGroupName = "Server API/Button";

		// Token: 0x04000F8D RID: 3981
		[SerializeField]
		private long m_lastServerApiUpdate = long.MinValue;

		// Token: 0x04000F8E RID: 3982
		[SerializeField]
		private string m_previousServerApiVersion;

		// Token: 0x04000F8F RID: 3983
		[SerializeField]
		private string m_serverApiVersion;

		// Token: 0x04000F90 RID: 3984
		[SerializeField]
		private GameData.ApiVersionData[] m_previousVersions;

		// Token: 0x04000F91 RID: 3985
		[SerializeField]
		private bool m_useGraphicsJobs;

		// Token: 0x04000F92 RID: 3986
		[SerializeField]
		private bool m_useIncrementalGc;

		// Token: 0x02000211 RID: 529
		[Serializable]
		private class ApiVersionData
		{
			// Token: 0x04000F93 RID: 3987
			private const string kButtonGroup = "BTN";

			// Token: 0x04000F94 RID: 3988
			[HideInInspector]
			public GameData Parent;

			// Token: 0x04000F95 RID: 3989
			public long LastUpdate = long.MinValue;

			// Token: 0x04000F96 RID: 3990
			public string Version;
		}
	}
}
