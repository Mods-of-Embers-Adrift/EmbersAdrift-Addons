using System;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003EB RID: 1003
	public enum CommandType
	{
		// Token: 0x04002199 RID: 8601
		none,
		// Token: 0x0400219A RID: 8602
		auth,
		// Token: 0x0400219B RID: 8603
		reauth,
		// Token: 0x0400219C RID: 8604
		sessionkeyauth,
		// Token: 0x0400219D RID: 8605
		clearsession,
		// Token: 0x0400219E RID: 8606
		getallofit,
		// Token: 0x0400219F RID: 8607
		disconnect,
		// Token: 0x040021A0 RID: 8608
		playcharacter,
		// Token: 0x040021A1 RID: 8609
		initworld,
		// Token: 0x040021A2 RID: 8610
		init_login,
		// Token: 0x040021A3 RID: 8611
		versioncheck,
		// Token: 0x040021A4 RID: 8612
		createcharacter,
		// Token: 0x040021A5 RID: 8613
		deletecharacter,
		// Token: 0x040021A6 RID: 8614
		updatecharactervisuals,
		// Token: 0x040021A7 RID: 8615
		notification,
		// Token: 0x040021A8 RID: 8616
		renamed,
		// Token: 0x040021A9 RID: 8617
		setactivecharacters,
		// Token: 0x040021AA RID: 8618
		say,
		// Token: 0x040021AB RID: 8619
		tell,
		// Token: 0x040021AC RID: 8620
		yell,
		// Token: 0x040021AD RID: 8621
		zone,
		// Token: 0x040021AE RID: 8622
		group,
		// Token: 0x040021AF RID: 8623
		raid,
		// Token: 0x040021B0 RID: 8624
		motd,
		// Token: 0x040021B1 RID: 8625
		guild,
		// Token: 0x040021B2 RID: 8626
		officer,
		// Token: 0x040021B3 RID: 8627
		subscriber,
		// Token: 0x040021B4 RID: 8628
		help,
		// Token: 0x040021B5 RID: 8629
		invite,
		// Token: 0x040021B6 RID: 8630
		invited,
		// Token: 0x040021B7 RID: 8631
		selfjoined,
		// Token: 0x040021B8 RID: 8632
		joined,
		// Token: 0x040021B9 RID: 8633
		selfdisbaned,
		// Token: 0x040021BA RID: 8634
		disbanded,
		// Token: 0x040021BB RID: 8635
		left,
		// Token: 0x040021BC RID: 8636
		leave,
		// Token: 0x040021BD RID: 8637
		kick,
		// Token: 0x040021BE RID: 8638
		kicked,
		// Token: 0x040021BF RID: 8639
		promote,
		// Token: 0x040021C0 RID: 8640
		promoted,
		// Token: 0x040021C1 RID: 8641
		statusupdate,
		// Token: 0x040021C2 RID: 8642
		accept,
		// Token: 0x040021C3 RID: 8643
		decline,
		// Token: 0x040021C4 RID: 8644
		roll,
		// Token: 0x040021C5 RID: 8645
		who,
		// Token: 0x040021C6 RID: 8646
		stuck,
		// Token: 0x040021C7 RID: 8647
		zonecheck,
		// Token: 0x040021C8 RID: 8648
		error,
		// Token: 0x040021C9 RID: 8649
		performance,
		// Token: 0x040021CA RID: 8650
		system,
		// Token: 0x040021CB RID: 8651
		systemzone,
		// Token: 0x040021CC RID: 8652
		time,
		// Token: 0x040021CD RID: 8653
		played,
		// Token: 0x040021CE RID: 8654
		presence,
		// Token: 0x040021CF RID: 8655
		namecheck,
		// Token: 0x040021D0 RID: 8656
		send,
		// Token: 0x040021D1 RID: 8657
		add,
		// Token: 0x040021D2 RID: 8658
		delete,
		// Token: 0x040021D3 RID: 8659
		friend,
		// Token: 0x040021D4 RID: 8660
		unfriend,
		// Token: 0x040021D5 RID: 8661
		block,
		// Token: 0x040021D6 RID: 8662
		unblock,
		// Token: 0x040021D7 RID: 8663
		raidleave,
		// Token: 0x040021D8 RID: 8664
		raidpromote,
		// Token: 0x040021D9 RID: 8665
		raidkick,
		// Token: 0x040021DA RID: 8666
		raiddisband,
		// Token: 0x040021DB RID: 8667
		create,
		// Token: 0x040021DC RID: 8668
		read,
		// Token: 0x040021DD RID: 8669
		update,
		// Token: 0x040021DE RID: 8670
		deletemember,
		// Token: 0x040021DF RID: 8671
		demote,
		// Token: 0x040021E0 RID: 8672
		disband,
		// Token: 0x040021E1 RID: 8673
		transfer,
		// Token: 0x040021E2 RID: 8674
		editdescription,
		// Token: 0x040021E3 RID: 8675
		editmotd,
		// Token: 0x040021E4 RID: 8676
		addrank,
		// Token: 0x040021E5 RID: 8677
		editrank,
		// Token: 0x040021E6 RID: 8678
		removerank,
		// Token: 0x040021E7 RID: 8679
		editpublicnote,
		// Token: 0x040021E8 RID: 8680
		editofficernote,
		// Token: 0x040021E9 RID: 8681
		gcreate,
		// Token: 0x040021EA RID: 8682
		ginvite,
		// Token: 0x040021EB RID: 8683
		gpromote,
		// Token: 0x040021EC RID: 8684
		gdemote,
		// Token: 0x040021ED RID: 8685
		gkick,
		// Token: 0x040021EE RID: 8686
		gleave,
		// Token: 0x040021EF RID: 8687
		gdisband,
		// Token: 0x040021F0 RID: 8688
		listall,
		// Token: 0x040021F1 RID: 8689
		listlfg,
		// Token: 0x040021F2 RID: 8690
		startlfg,
		// Token: 0x040021F3 RID: 8691
		stoplfg,
		// Token: 0x040021F4 RID: 8692
		listlfm,
		// Token: 0x040021F5 RID: 8693
		startlfm,
		// Token: 0x040021F6 RID: 8694
		stoplfm,
		// Token: 0x040021F7 RID: 8695
		report,
		// Token: 0x040021F8 RID: 8696
		emote,
		// Token: 0x040021F9 RID: 8697
		debugposition,
		// Token: 0x040021FA RID: 8698
		world,
		// Token: 0x040021FB RID: 8699
		trade,
		// Token: 0x040021FC RID: 8700
		renamecharacter,
		// Token: 0x040021FD RID: 8701
		steampurchase
	}
}
