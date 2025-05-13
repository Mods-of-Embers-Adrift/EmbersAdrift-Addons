using System;

namespace SoL.Networking.Database
{
	// Token: 0x0200045A RID: 1114
	[Flags]
	public enum GuildPermissions
	{
		// Token: 0x040024C6 RID: 9414
		None = 0,
		// Token: 0x040024C7 RID: 9415
		Disband = 1,
		// Token: 0x040024C8 RID: 9416
		EditDescription = 2,
		// Token: 0x040024C9 RID: 9417
		EditMotd = 4,
		// Token: 0x040024CA RID: 9418
		EditRanks = 8,
		// Token: 0x040024CB RID: 9419
		Promote = 16,
		// Token: 0x040024CC RID: 9420
		Demote = 32,
		// Token: 0x040024CD RID: 9421
		InviteMember = 64,
		// Token: 0x040024CE RID: 9422
		KickMember = 128,
		// Token: 0x040024CF RID: 9423
		ChatListen = 256,
		// Token: 0x040024D0 RID: 9424
		ChatSpeak = 512,
		// Token: 0x040024D1 RID: 9425
		OfficerChatListen = 1024,
		// Token: 0x040024D2 RID: 9426
		OfficerChatSpeak = 2048,
		// Token: 0x040024D3 RID: 9427
		EditPublicNote = 4096,
		// Token: 0x040024D4 RID: 9428
		EditOfficerNote = 8192,
		// Token: 0x040024D5 RID: 9429
		ViewOfficerNote = 16384
	}
}
