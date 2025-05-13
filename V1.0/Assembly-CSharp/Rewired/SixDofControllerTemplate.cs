using System;

namespace Rewired
{
	// Token: 0x02000062 RID: 98
	public sealed class SixDofControllerTemplate : ControllerTemplate, ISixDofControllerTemplate, IControllerTemplate
	{
		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00045AAA File Offset: 0x00043CAA
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis1
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(8);
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x00045AB3 File Offset: 0x00043CB3
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(9);
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060003A3 RID: 931 RVA: 0x00045ABD File Offset: 0x00043CBD
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis3
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(10);
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x0004555A File Offset: 0x0004375A
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis4
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(11);
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x00045564 File Offset: 0x00043764
		IControllerTemplateButton ISixDofControllerTemplate.button1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x000455FC File Offset: 0x000437FC
		IControllerTemplateButton ISixDofControllerTemplate.button2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x00045578 File Offset: 0x00043778
		IControllerTemplateButton ISixDofControllerTemplate.button3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00045582 File Offset: 0x00043782
		IControllerTemplateButton ISixDofControllerTemplate.button4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x0004558C File Offset: 0x0004378C
		IControllerTemplateButton ISixDofControllerTemplate.button5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00045606 File Offset: 0x00043806
		IControllerTemplateButton ISixDofControllerTemplate.button6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(17);
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060003AB RID: 939 RVA: 0x00045610 File Offset: 0x00043810
		IControllerTemplateButton ISixDofControllerTemplate.button7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060003AC RID: 940 RVA: 0x0004561A File Offset: 0x0004381A
		IControllerTemplateButton ISixDofControllerTemplate.button8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060003AD RID: 941 RVA: 0x00045624 File Offset: 0x00043824
		IControllerTemplateButton ISixDofControllerTemplate.button9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060003AE RID: 942 RVA: 0x0004562E File Offset: 0x0004382E
		IControllerTemplateButton ISixDofControllerTemplate.button10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00045638 File Offset: 0x00043838
		IControllerTemplateButton ISixDofControllerTemplate.button11
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x00045642 File Offset: 0x00043842
		IControllerTemplateButton ISixDofControllerTemplate.button12
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x0004564C File Offset: 0x0004384C
		IControllerTemplateButton ISixDofControllerTemplate.button13
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x00045656 File Offset: 0x00043856
		IControllerTemplateButton ISixDofControllerTemplate.button14
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x00045660 File Offset: 0x00043860
		IControllerTemplateButton ISixDofControllerTemplate.button15
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x0004566A File Offset: 0x0004386A
		IControllerTemplateButton ISixDofControllerTemplate.button16
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(27);
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x00045674 File Offset: 0x00043874
		IControllerTemplateButton ISixDofControllerTemplate.button17
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(28);
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x0004567E File Offset: 0x0004387E
		IControllerTemplateButton ISixDofControllerTemplate.button18
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(29);
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x00045688 File Offset: 0x00043888
		IControllerTemplateButton ISixDofControllerTemplate.button19
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(30);
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00045692 File Offset: 0x00043892
		IControllerTemplateButton ISixDofControllerTemplate.button20
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(31);
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x00045787 File Offset: 0x00043987
		IControllerTemplateButton ISixDofControllerTemplate.button21
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(55);
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00045791 File Offset: 0x00043991
		IControllerTemplateButton ISixDofControllerTemplate.button22
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(56);
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060003BB RID: 955 RVA: 0x0004579B File Offset: 0x0004399B
		IControllerTemplateButton ISixDofControllerTemplate.button23
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(57);
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060003BC RID: 956 RVA: 0x000457A5 File Offset: 0x000439A5
		IControllerTemplateButton ISixDofControllerTemplate.button24
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(58);
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060003BD RID: 957 RVA: 0x000457AF File Offset: 0x000439AF
		IControllerTemplateButton ISixDofControllerTemplate.button25
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(59);
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000457B9 File Offset: 0x000439B9
		IControllerTemplateButton ISixDofControllerTemplate.button26
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(60);
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060003BF RID: 959 RVA: 0x000457C3 File Offset: 0x000439C3
		IControllerTemplateButton ISixDofControllerTemplate.button27
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(61);
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x000457CD File Offset: 0x000439CD
		IControllerTemplateButton ISixDofControllerTemplate.button28
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(62);
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x000457D7 File Offset: 0x000439D7
		IControllerTemplateButton ISixDofControllerTemplate.button29
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(63);
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x000457E1 File Offset: 0x000439E1
		IControllerTemplateButton ISixDofControllerTemplate.button30
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(64);
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x000457EB File Offset: 0x000439EB
		IControllerTemplateButton ISixDofControllerTemplate.button31
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(65);
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x000457F5 File Offset: 0x000439F5
		IControllerTemplateButton ISixDofControllerTemplate.button32
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(66);
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x00045AC7 File Offset: 0x00043CC7
		IControllerTemplateHat ISixDofControllerTemplate.hat1
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(48);
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x00045AD1 File Offset: 0x00043CD1
		IControllerTemplateHat ISixDofControllerTemplate.hat2
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(49);
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x00045ADB File Offset: 0x00043CDB
		IControllerTemplateThrottle ISixDofControllerTemplate.throttle1
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(52);
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00045AE5 File Offset: 0x00043CE5
		IControllerTemplateThrottle ISixDofControllerTemplate.throttle2
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(53);
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00045AEF File Offset: 0x00043CEF
		IControllerTemplateStick6D ISixDofControllerTemplate.stick
		{
			get
			{
				return base.GetElement<IControllerTemplateStick6D>(54);
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x000455B4 File Offset: 0x000437B4
		public SixDofControllerTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x04000505 RID: 1285
		public static readonly Guid typeGuid = new Guid("2599beb3-522b-43dd-a4ef-93fd60e5eafa");

		// Token: 0x04000506 RID: 1286
		public const int elementId_positionX = 1;

		// Token: 0x04000507 RID: 1287
		public const int elementId_positionY = 2;

		// Token: 0x04000508 RID: 1288
		public const int elementId_positionZ = 0;

		// Token: 0x04000509 RID: 1289
		public const int elementId_rotationX = 3;

		// Token: 0x0400050A RID: 1290
		public const int elementId_rotationY = 5;

		// Token: 0x0400050B RID: 1291
		public const int elementId_rotationZ = 4;

		// Token: 0x0400050C RID: 1292
		public const int elementId_throttle1Axis = 6;

		// Token: 0x0400050D RID: 1293
		public const int elementId_throttle1MinDetent = 50;

		// Token: 0x0400050E RID: 1294
		public const int elementId_throttle2Axis = 7;

		// Token: 0x0400050F RID: 1295
		public const int elementId_throttle2MinDetent = 51;

		// Token: 0x04000510 RID: 1296
		public const int elementId_extraAxis1 = 8;

		// Token: 0x04000511 RID: 1297
		public const int elementId_extraAxis2 = 9;

		// Token: 0x04000512 RID: 1298
		public const int elementId_extraAxis3 = 10;

		// Token: 0x04000513 RID: 1299
		public const int elementId_extraAxis4 = 11;

		// Token: 0x04000514 RID: 1300
		public const int elementId_button1 = 12;

		// Token: 0x04000515 RID: 1301
		public const int elementId_button2 = 13;

		// Token: 0x04000516 RID: 1302
		public const int elementId_button3 = 14;

		// Token: 0x04000517 RID: 1303
		public const int elementId_button4 = 15;

		// Token: 0x04000518 RID: 1304
		public const int elementId_button5 = 16;

		// Token: 0x04000519 RID: 1305
		public const int elementId_button6 = 17;

		// Token: 0x0400051A RID: 1306
		public const int elementId_button7 = 18;

		// Token: 0x0400051B RID: 1307
		public const int elementId_button8 = 19;

		// Token: 0x0400051C RID: 1308
		public const int elementId_button9 = 20;

		// Token: 0x0400051D RID: 1309
		public const int elementId_button10 = 21;

		// Token: 0x0400051E RID: 1310
		public const int elementId_button11 = 22;

		// Token: 0x0400051F RID: 1311
		public const int elementId_button12 = 23;

		// Token: 0x04000520 RID: 1312
		public const int elementId_button13 = 24;

		// Token: 0x04000521 RID: 1313
		public const int elementId_button14 = 25;

		// Token: 0x04000522 RID: 1314
		public const int elementId_button15 = 26;

		// Token: 0x04000523 RID: 1315
		public const int elementId_button16 = 27;

		// Token: 0x04000524 RID: 1316
		public const int elementId_button17 = 28;

		// Token: 0x04000525 RID: 1317
		public const int elementId_button18 = 29;

		// Token: 0x04000526 RID: 1318
		public const int elementId_button19 = 30;

		// Token: 0x04000527 RID: 1319
		public const int elementId_button20 = 31;

		// Token: 0x04000528 RID: 1320
		public const int elementId_button21 = 55;

		// Token: 0x04000529 RID: 1321
		public const int elementId_button22 = 56;

		// Token: 0x0400052A RID: 1322
		public const int elementId_button23 = 57;

		// Token: 0x0400052B RID: 1323
		public const int elementId_button24 = 58;

		// Token: 0x0400052C RID: 1324
		public const int elementId_button25 = 59;

		// Token: 0x0400052D RID: 1325
		public const int elementId_button26 = 60;

		// Token: 0x0400052E RID: 1326
		public const int elementId_button27 = 61;

		// Token: 0x0400052F RID: 1327
		public const int elementId_button28 = 62;

		// Token: 0x04000530 RID: 1328
		public const int elementId_button29 = 63;

		// Token: 0x04000531 RID: 1329
		public const int elementId_button30 = 64;

		// Token: 0x04000532 RID: 1330
		public const int elementId_button31 = 65;

		// Token: 0x04000533 RID: 1331
		public const int elementId_button32 = 66;

		// Token: 0x04000534 RID: 1332
		public const int elementId_hat1Up = 32;

		// Token: 0x04000535 RID: 1333
		public const int elementId_hat1UpRight = 33;

		// Token: 0x04000536 RID: 1334
		public const int elementId_hat1Right = 34;

		// Token: 0x04000537 RID: 1335
		public const int elementId_hat1DownRight = 35;

		// Token: 0x04000538 RID: 1336
		public const int elementId_hat1Down = 36;

		// Token: 0x04000539 RID: 1337
		public const int elementId_hat1DownLeft = 37;

		// Token: 0x0400053A RID: 1338
		public const int elementId_hat1Left = 38;

		// Token: 0x0400053B RID: 1339
		public const int elementId_hat1UpLeft = 39;

		// Token: 0x0400053C RID: 1340
		public const int elementId_hat2Up = 40;

		// Token: 0x0400053D RID: 1341
		public const int elementId_hat2UpRight = 41;

		// Token: 0x0400053E RID: 1342
		public const int elementId_hat2Right = 42;

		// Token: 0x0400053F RID: 1343
		public const int elementId_hat2DownRight = 43;

		// Token: 0x04000540 RID: 1344
		public const int elementId_hat2Down = 44;

		// Token: 0x04000541 RID: 1345
		public const int elementId_hat2DownLeft = 45;

		// Token: 0x04000542 RID: 1346
		public const int elementId_hat2Left = 46;

		// Token: 0x04000543 RID: 1347
		public const int elementId_hat2UpLeft = 47;

		// Token: 0x04000544 RID: 1348
		public const int elementId_hat1 = 48;

		// Token: 0x04000545 RID: 1349
		public const int elementId_hat2 = 49;

		// Token: 0x04000546 RID: 1350
		public const int elementId_throttle1 = 52;

		// Token: 0x04000547 RID: 1351
		public const int elementId_throttle2 = 53;

		// Token: 0x04000548 RID: 1352
		public const int elementId_stick = 54;
	}
}
