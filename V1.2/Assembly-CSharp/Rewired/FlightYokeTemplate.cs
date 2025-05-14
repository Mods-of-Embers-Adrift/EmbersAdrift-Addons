using System;

namespace Rewired
{
	// Token: 0x02000060 RID: 96
	public sealed class FlightYokeTemplate : ControllerTemplate, IFlightYokeTemplate, IControllerTemplate
	{
		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000369 RID: 873 RVA: 0x000457AF File Offset: 0x000439AF
		IControllerTemplateButton IFlightYokeTemplate.leftPaddle
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(59);
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600036A RID: 874 RVA: 0x000457B9 File Offset: 0x000439B9
		IControllerTemplateButton IFlightYokeTemplate.rightPaddle
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(60);
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600036B RID: 875 RVA: 0x00045534 File Offset: 0x00043734
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x0600036C RID: 876 RVA: 0x0004553D File Offset: 0x0004373D
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600036D RID: 877 RVA: 0x00045546 File Offset: 0x00043746
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00045550 File Offset: 0x00043750
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x0600036F RID: 879 RVA: 0x000455F2 File Offset: 0x000437F2
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(11);
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00045564 File Offset: 0x00043764
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000371 RID: 881 RVA: 0x000455FC File Offset: 0x000437FC
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000372 RID: 882 RVA: 0x00045578 File Offset: 0x00043778
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000373 RID: 883 RVA: 0x00045582 File Offset: 0x00043782
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0004558C File Offset: 0x0004378C
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000375 RID: 885 RVA: 0x00045606 File Offset: 0x00043806
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(17);
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00045610 File Offset: 0x00043810
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0004561A File Offset: 0x0004381A
		IControllerTemplateButton IFlightYokeTemplate.centerButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000378 RID: 888 RVA: 0x00045624 File Offset: 0x00043824
		IControllerTemplateButton IFlightYokeTemplate.centerButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0004562E File Offset: 0x0004382E
		IControllerTemplateButton IFlightYokeTemplate.centerButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x0600037A RID: 890 RVA: 0x00045638 File Offset: 0x00043838
		IControllerTemplateButton IFlightYokeTemplate.centerButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x0600037B RID: 891 RVA: 0x00045642 File Offset: 0x00043842
		IControllerTemplateButton IFlightYokeTemplate.centerButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600037C RID: 892 RVA: 0x0004564C File Offset: 0x0004384C
		IControllerTemplateButton IFlightYokeTemplate.centerButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x0600037D RID: 893 RVA: 0x00045656 File Offset: 0x00043856
		IControllerTemplateButton IFlightYokeTemplate.centerButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00045660 File Offset: 0x00043860
		IControllerTemplateButton IFlightYokeTemplate.centerButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x0600037F RID: 895 RVA: 0x00045773 File Offset: 0x00043973
		IControllerTemplateButton IFlightYokeTemplate.wheel1Up
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(53);
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000380 RID: 896 RVA: 0x0004577D File Offset: 0x0004397D
		IControllerTemplateButton IFlightYokeTemplate.wheel1Down
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(54);
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000381 RID: 897 RVA: 0x00045787 File Offset: 0x00043987
		IControllerTemplateButton IFlightYokeTemplate.wheel1Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(55);
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000382 RID: 898 RVA: 0x00045791 File Offset: 0x00043991
		IControllerTemplateButton IFlightYokeTemplate.wheel2Up
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(56);
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000383 RID: 899 RVA: 0x0004579B File Offset: 0x0004399B
		IControllerTemplateButton IFlightYokeTemplate.wheel2Down
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(57);
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000384 RID: 900 RVA: 0x000457A5 File Offset: 0x000439A5
		IControllerTemplateButton IFlightYokeTemplate.wheel2Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(58);
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000385 RID: 901 RVA: 0x000456EC File Offset: 0x000438EC
		IControllerTemplateButton IFlightYokeTemplate.consoleButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(43);
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000386 RID: 902 RVA: 0x000456C4 File Offset: 0x000438C4
		IControllerTemplateButton IFlightYokeTemplate.consoleButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(44);
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000387 RID: 903 RVA: 0x00045741 File Offset: 0x00043941
		IControllerTemplateButton IFlightYokeTemplate.consoleButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(45);
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000388 RID: 904 RVA: 0x0004574B File Offset: 0x0004394B
		IControllerTemplateButton IFlightYokeTemplate.consoleButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(46);
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000389 RID: 905 RVA: 0x00045A1A File Offset: 0x00043C1A
		IControllerTemplateButton IFlightYokeTemplate.consoleButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(47);
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x0600038A RID: 906 RVA: 0x00045A24 File Offset: 0x00043C24
		IControllerTemplateButton IFlightYokeTemplate.consoleButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(48);
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x0600038B RID: 907 RVA: 0x00045A2E File Offset: 0x00043C2E
		IControllerTemplateButton IFlightYokeTemplate.consoleButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(49);
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x0600038C RID: 908 RVA: 0x00045755 File Offset: 0x00043955
		IControllerTemplateButton IFlightYokeTemplate.consoleButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(50);
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x0600038D RID: 909 RVA: 0x0004575F File Offset: 0x0004395F
		IControllerTemplateButton IFlightYokeTemplate.consoleButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(51);
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600038E RID: 910 RVA: 0x00045769 File Offset: 0x00043969
		IControllerTemplateButton IFlightYokeTemplate.consoleButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(52);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600038F RID: 911 RVA: 0x000457C3 File Offset: 0x000439C3
		IControllerTemplateButton IFlightYokeTemplate.mode1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(61);
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000390 RID: 912 RVA: 0x000457CD File Offset: 0x000439CD
		IControllerTemplateButton IFlightYokeTemplate.mode2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(62);
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000391 RID: 913 RVA: 0x000457D7 File Offset: 0x000439D7
		IControllerTemplateButton IFlightYokeTemplate.mode3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(63);
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00045A38 File Offset: 0x00043C38
		IControllerTemplateYoke IFlightYokeTemplate.yoke
		{
			get
			{
				return base.GetElement<IControllerTemplateYoke>(69);
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000393 RID: 915 RVA: 0x00045A42 File Offset: 0x00043C42
		IControllerTemplateThrottle IFlightYokeTemplate.lever1
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(70);
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000394 RID: 916 RVA: 0x00045A4C File Offset: 0x00043C4C
		IControllerTemplateThrottle IFlightYokeTemplate.lever2
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(71);
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000395 RID: 917 RVA: 0x00045A56 File Offset: 0x00043C56
		IControllerTemplateThrottle IFlightYokeTemplate.lever3
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(72);
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000396 RID: 918 RVA: 0x00045A60 File Offset: 0x00043C60
		IControllerTemplateThrottle IFlightYokeTemplate.lever4
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(73);
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000397 RID: 919 RVA: 0x00045A6A File Offset: 0x00043C6A
		IControllerTemplateThrottle IFlightYokeTemplate.lever5
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(74);
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00045A74 File Offset: 0x00043C74
		IControllerTemplateHat IFlightYokeTemplate.leftGripHat
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(75);
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000399 RID: 921 RVA: 0x00045A7E File Offset: 0x00043C7E
		IControllerTemplateHat IFlightYokeTemplate.rightGripHat
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(76);
			}
		}

		// Token: 0x0600039A RID: 922 RVA: 0x000455B4 File Offset: 0x000437B4
		public FlightYokeTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040004B3 RID: 1203
		public static readonly Guid typeGuid = new Guid("f311fa16-0ccc-41c0-ac4b-50f7100bb8ff");

		// Token: 0x040004B4 RID: 1204
		public const int elementId_rotateYoke = 0;

		// Token: 0x040004B5 RID: 1205
		public const int elementId_yokeZ = 1;

		// Token: 0x040004B6 RID: 1206
		public const int elementId_leftPaddle = 59;

		// Token: 0x040004B7 RID: 1207
		public const int elementId_rightPaddle = 60;

		// Token: 0x040004B8 RID: 1208
		public const int elementId_lever1Axis = 2;

		// Token: 0x040004B9 RID: 1209
		public const int elementId_lever1MinDetent = 64;

		// Token: 0x040004BA RID: 1210
		public const int elementId_lever2Axis = 3;

		// Token: 0x040004BB RID: 1211
		public const int elementId_lever2MinDetent = 65;

		// Token: 0x040004BC RID: 1212
		public const int elementId_lever3Axis = 4;

		// Token: 0x040004BD RID: 1213
		public const int elementId_lever3MinDetent = 66;

		// Token: 0x040004BE RID: 1214
		public const int elementId_lever4Axis = 5;

		// Token: 0x040004BF RID: 1215
		public const int elementId_lever4MinDetent = 67;

		// Token: 0x040004C0 RID: 1216
		public const int elementId_lever5Axis = 6;

		// Token: 0x040004C1 RID: 1217
		public const int elementId_lever5MinDetent = 68;

		// Token: 0x040004C2 RID: 1218
		public const int elementId_leftGripButton1 = 7;

		// Token: 0x040004C3 RID: 1219
		public const int elementId_leftGripButton2 = 8;

		// Token: 0x040004C4 RID: 1220
		public const int elementId_leftGripButton3 = 9;

		// Token: 0x040004C5 RID: 1221
		public const int elementId_leftGripButton4 = 10;

		// Token: 0x040004C6 RID: 1222
		public const int elementId_leftGripButton5 = 11;

		// Token: 0x040004C7 RID: 1223
		public const int elementId_leftGripButton6 = 12;

		// Token: 0x040004C8 RID: 1224
		public const int elementId_rightGripButton1 = 13;

		// Token: 0x040004C9 RID: 1225
		public const int elementId_rightGripButton2 = 14;

		// Token: 0x040004CA RID: 1226
		public const int elementId_rightGripButton3 = 15;

		// Token: 0x040004CB RID: 1227
		public const int elementId_rightGripButton4 = 16;

		// Token: 0x040004CC RID: 1228
		public const int elementId_rightGripButton5 = 17;

		// Token: 0x040004CD RID: 1229
		public const int elementId_rightGripButton6 = 18;

		// Token: 0x040004CE RID: 1230
		public const int elementId_centerButton1 = 19;

		// Token: 0x040004CF RID: 1231
		public const int elementId_centerButton2 = 20;

		// Token: 0x040004D0 RID: 1232
		public const int elementId_centerButton3 = 21;

		// Token: 0x040004D1 RID: 1233
		public const int elementId_centerButton4 = 22;

		// Token: 0x040004D2 RID: 1234
		public const int elementId_centerButton5 = 23;

		// Token: 0x040004D3 RID: 1235
		public const int elementId_centerButton6 = 24;

		// Token: 0x040004D4 RID: 1236
		public const int elementId_centerButton7 = 25;

		// Token: 0x040004D5 RID: 1237
		public const int elementId_centerButton8 = 26;

		// Token: 0x040004D6 RID: 1238
		public const int elementId_wheel1Up = 53;

		// Token: 0x040004D7 RID: 1239
		public const int elementId_wheel1Down = 54;

		// Token: 0x040004D8 RID: 1240
		public const int elementId_wheel1Press = 55;

		// Token: 0x040004D9 RID: 1241
		public const int elementId_wheel2Up = 56;

		// Token: 0x040004DA RID: 1242
		public const int elementId_wheel2Down = 57;

		// Token: 0x040004DB RID: 1243
		public const int elementId_wheel2Press = 58;

		// Token: 0x040004DC RID: 1244
		public const int elementId_leftGripHatUp = 27;

		// Token: 0x040004DD RID: 1245
		public const int elementId_leftGripHatUpRight = 28;

		// Token: 0x040004DE RID: 1246
		public const int elementId_leftGripHatRight = 29;

		// Token: 0x040004DF RID: 1247
		public const int elementId_leftGripHatDownRight = 30;

		// Token: 0x040004E0 RID: 1248
		public const int elementId_leftGripHatDown = 31;

		// Token: 0x040004E1 RID: 1249
		public const int elementId_leftGripHatDownLeft = 32;

		// Token: 0x040004E2 RID: 1250
		public const int elementId_leftGripHatLeft = 33;

		// Token: 0x040004E3 RID: 1251
		public const int elementId_leftGripHatUpLeft = 34;

		// Token: 0x040004E4 RID: 1252
		public const int elementId_rightGripHatUp = 35;

		// Token: 0x040004E5 RID: 1253
		public const int elementId_rightGripHatUpRight = 36;

		// Token: 0x040004E6 RID: 1254
		public const int elementId_rightGripHatRight = 37;

		// Token: 0x040004E7 RID: 1255
		public const int elementId_rightGripHatDownRight = 38;

		// Token: 0x040004E8 RID: 1256
		public const int elementId_rightGripHatDown = 39;

		// Token: 0x040004E9 RID: 1257
		public const int elementId_rightGripHatDownLeft = 40;

		// Token: 0x040004EA RID: 1258
		public const int elementId_rightGripHatLeft = 41;

		// Token: 0x040004EB RID: 1259
		public const int elementId_rightGripHatUpLeft = 42;

		// Token: 0x040004EC RID: 1260
		public const int elementId_consoleButton1 = 43;

		// Token: 0x040004ED RID: 1261
		public const int elementId_consoleButton2 = 44;

		// Token: 0x040004EE RID: 1262
		public const int elementId_consoleButton3 = 45;

		// Token: 0x040004EF RID: 1263
		public const int elementId_consoleButton4 = 46;

		// Token: 0x040004F0 RID: 1264
		public const int elementId_consoleButton5 = 47;

		// Token: 0x040004F1 RID: 1265
		public const int elementId_consoleButton6 = 48;

		// Token: 0x040004F2 RID: 1266
		public const int elementId_consoleButton7 = 49;

		// Token: 0x040004F3 RID: 1267
		public const int elementId_consoleButton8 = 50;

		// Token: 0x040004F4 RID: 1268
		public const int elementId_consoleButton9 = 51;

		// Token: 0x040004F5 RID: 1269
		public const int elementId_consoleButton10 = 52;

		// Token: 0x040004F6 RID: 1270
		public const int elementId_mode1 = 61;

		// Token: 0x040004F7 RID: 1271
		public const int elementId_mode2 = 62;

		// Token: 0x040004F8 RID: 1272
		public const int elementId_mode3 = 63;

		// Token: 0x040004F9 RID: 1273
		public const int elementId_yoke = 69;

		// Token: 0x040004FA RID: 1274
		public const int elementId_lever1 = 70;

		// Token: 0x040004FB RID: 1275
		public const int elementId_lever2 = 71;

		// Token: 0x040004FC RID: 1276
		public const int elementId_lever3 = 72;

		// Token: 0x040004FD RID: 1277
		public const int elementId_lever4 = 73;

		// Token: 0x040004FE RID: 1278
		public const int elementId_lever5 = 74;

		// Token: 0x040004FF RID: 1279
		public const int elementId_leftGripHat = 75;

		// Token: 0x04000500 RID: 1280
		public const int elementId_rightGripHat = 76;
	}
}
