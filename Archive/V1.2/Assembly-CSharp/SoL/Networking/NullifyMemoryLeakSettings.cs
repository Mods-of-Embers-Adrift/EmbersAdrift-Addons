using System;
using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Text;
using SoL.Managers;

namespace SoL.Networking
{
	// Token: 0x020003B7 RID: 951
	[Serializable]
	public class NullifyMemoryLeakSettings
	{
		// Token: 0x060019CA RID: 6602 RVA: 0x00054289 File Offset: 0x00052489
		public void Init()
		{
			Task.CleanupReferences = this.BehaviorDesigner;
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x0010791C File Offset: 0x00105B1C
		public string GetStartupString()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendLine("[Memory Leak Settings]");
				utf16ValueStringBuilder.AppendFormat<bool>("       CharacterRecord : {0}\n", this.CharacterRecord);
				utf16ValueStringBuilder.AppendFormat<bool>("     ContainerInstance : {0}\n", this.ContainerInstance);
				utf16ValueStringBuilder.AppendFormat<bool>("       SyncVarMonoRefs : {0}\n", this.SyncVarMonoRefs);
				utf16ValueStringBuilder.AppendFormat<bool>("     RpcHandlerMonoRefs: {0}\n", this.RpcHandlerMonoRefs);
				utf16ValueStringBuilder.AppendFormat<bool>("                Vitals : {0}\n", this.Vitals);
				utf16ValueStringBuilder.AppendFormat<bool>("   NpcTargetController : {0}\n", this.NpcTargetController);
				utf16ValueStringBuilder.AppendFormat<bool>("   ServerNpcController : {0}\n", this.ServerNpcController);
				utf16ValueStringBuilder.AppendFormat<bool>("      EffectController : {0}\n", this.EffectController);
				utf16ValueStringBuilder.AppendFormat<bool>("      BehaviorDesigner : {0}\n", this.BehaviorDesigner);
				utf16ValueStringBuilder.AppendFormat<bool>("       RemoteContainer : {0}", this.RemoteContainer);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x060019CC RID: 6604 RVA: 0x00054296 File Offset: 0x00052496
		private static bool Exists
		{
			get
			{
				return GameManager.IsServer && ServerGameManager.GameServerConfig != null && ServerGameManager.GameServerConfig.MemoryLeakSettings != null;
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x060019CD RID: 6605 RVA: 0x000542B5 File Offset: 0x000524B5
		public static bool CleanCharacterRecord
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.CharacterRecord;
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x060019CE RID: 6606 RVA: 0x000542CF File Offset: 0x000524CF
		public static bool CleanContainerInstance
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.ContainerInstance;
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x060019CF RID: 6607 RVA: 0x000542E9 File Offset: 0x000524E9
		public static bool CleanSyncVarMonoRefs
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.SyncVarMonoRefs;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x060019D0 RID: 6608 RVA: 0x00054303 File Offset: 0x00052503
		public static bool CleanRpcHandlerMonoRefs
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.RpcHandlerMonoRefs;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x060019D1 RID: 6609 RVA: 0x0005431D File Offset: 0x0005251D
		public static bool CleanVitals
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.Vitals;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x060019D2 RID: 6610 RVA: 0x00054337 File Offset: 0x00052537
		public static bool CleanNpcTargetController
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.NpcTargetController;
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x060019D3 RID: 6611 RVA: 0x00054351 File Offset: 0x00052551
		public static bool CleanServerNpcController
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.ServerNpcController;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x060019D4 RID: 6612 RVA: 0x0005436B File Offset: 0x0005256B
		public static bool CleanEffectController
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.EffectController;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x060019D5 RID: 6613 RVA: 0x00054385 File Offset: 0x00052585
		public static bool CleanBehaviorDesigner
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.BehaviorDesigner;
			}
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x060019D6 RID: 6614 RVA: 0x0005439F File Offset: 0x0005259F
		public static bool CleanRemoteContainer
		{
			get
			{
				return NullifyMemoryLeakSettings.Exists && ServerGameManager.GameServerConfig.MemoryLeakSettings.RemoteContainer;
			}
		}

		// Token: 0x040020CC RID: 8396
		public bool CharacterRecord;

		// Token: 0x040020CD RID: 8397
		public bool ContainerInstance;

		// Token: 0x040020CE RID: 8398
		public bool SyncVarMonoRefs;

		// Token: 0x040020CF RID: 8399
		public bool RpcHandlerMonoRefs;

		// Token: 0x040020D0 RID: 8400
		public bool Vitals;

		// Token: 0x040020D1 RID: 8401
		public bool NpcTargetController;

		// Token: 0x040020D2 RID: 8402
		public bool ServerNpcController;

		// Token: 0x040020D3 RID: 8403
		public bool EffectController;

		// Token: 0x040020D4 RID: 8404
		public bool BehaviorDesigner;

		// Token: 0x040020D5 RID: 8405
		public bool RemoteContainer;
	}
}
