using System;

namespace Rewired
{
	// Token: 0x0200005E RID: 94
	public sealed class RacingWheelTemplate : ControllerTemplate, IRacingWheelTemplate, IControllerTemplate
	{
		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x000455CE File Offset: 0x000437CE
		IControllerTemplateAxis IRacingWheelTemplate.wheel
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(0);
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x000455D7 File Offset: 0x000437D7
		IControllerTemplateAxis IRacingWheelTemplate.accelerator
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(1);
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x000455E0 File Offset: 0x000437E0
		IControllerTemplateAxis IRacingWheelTemplate.brake
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(2);
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000455E9 File Offset: 0x000437E9
		IControllerTemplateAxis IRacingWheelTemplate.clutch
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(3);
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x00045519 File Offset: 0x00043719
		IControllerTemplateButton IRacingWheelTemplate.shiftDown
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x00045522 File Offset: 0x00043722
		IControllerTemplateButton IRacingWheelTemplate.shiftUp
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0004552B File Offset: 0x0004372B
		IControllerTemplateButton IRacingWheelTemplate.wheelButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060002EA RID: 746 RVA: 0x00045534 File Offset: 0x00043734
		IControllerTemplateButton IRacingWheelTemplate.wheelButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0004553D File Offset: 0x0004373D
		IControllerTemplateButton IRacingWheelTemplate.wheelButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060002EC RID: 748 RVA: 0x00045546 File Offset: 0x00043746
		IControllerTemplateButton IRacingWheelTemplate.wheelButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060002ED RID: 749 RVA: 0x00045550 File Offset: 0x00043750
		IControllerTemplateButton IRacingWheelTemplate.wheelButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000455F2 File Offset: 0x000437F2
		IControllerTemplateButton IRacingWheelTemplate.wheelButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(11);
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060002EF RID: 751 RVA: 0x00045564 File Offset: 0x00043764
		IControllerTemplateButton IRacingWheelTemplate.wheelButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x000455FC File Offset: 0x000437FC
		IControllerTemplateButton IRacingWheelTemplate.wheelButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x00045578 File Offset: 0x00043778
		IControllerTemplateButton IRacingWheelTemplate.wheelButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x00045582 File Offset: 0x00043782
		IControllerTemplateButton IRacingWheelTemplate.wheelButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0004558C File Offset: 0x0004378C
		IControllerTemplateButton IRacingWheelTemplate.consoleButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x00045606 File Offset: 0x00043806
		IControllerTemplateButton IRacingWheelTemplate.consoleButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(17);
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x00045610 File Offset: 0x00043810
		IControllerTemplateButton IRacingWheelTemplate.consoleButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0004561A File Offset: 0x0004381A
		IControllerTemplateButton IRacingWheelTemplate.consoleButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x00045624 File Offset: 0x00043824
		IControllerTemplateButton IRacingWheelTemplate.consoleButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x0004562E File Offset: 0x0004382E
		IControllerTemplateButton IRacingWheelTemplate.consoleButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x00045638 File Offset: 0x00043838
		IControllerTemplateButton IRacingWheelTemplate.consoleButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060002FA RID: 762 RVA: 0x00045642 File Offset: 0x00043842
		IControllerTemplateButton IRacingWheelTemplate.consoleButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060002FB RID: 763 RVA: 0x0004564C File Offset: 0x0004384C
		IControllerTemplateButton IRacingWheelTemplate.consoleButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060002FC RID: 764 RVA: 0x00045656 File Offset: 0x00043856
		IControllerTemplateButton IRacingWheelTemplate.consoleButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060002FD RID: 765 RVA: 0x00045660 File Offset: 0x00043860
		IControllerTemplateButton IRacingWheelTemplate.shifter1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060002FE RID: 766 RVA: 0x0004566A File Offset: 0x0004386A
		IControllerTemplateButton IRacingWheelTemplate.shifter2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(27);
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060002FF RID: 767 RVA: 0x00045674 File Offset: 0x00043874
		IControllerTemplateButton IRacingWheelTemplate.shifter3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(28);
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000300 RID: 768 RVA: 0x0004567E File Offset: 0x0004387E
		IControllerTemplateButton IRacingWheelTemplate.shifter4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(29);
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000301 RID: 769 RVA: 0x00045688 File Offset: 0x00043888
		IControllerTemplateButton IRacingWheelTemplate.shifter5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(30);
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000302 RID: 770 RVA: 0x00045692 File Offset: 0x00043892
		IControllerTemplateButton IRacingWheelTemplate.shifter6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(31);
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0004569C File Offset: 0x0004389C
		IControllerTemplateButton IRacingWheelTemplate.shifter7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(32);
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000304 RID: 772 RVA: 0x000456A6 File Offset: 0x000438A6
		IControllerTemplateButton IRacingWheelTemplate.shifter8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(33);
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000305 RID: 773 RVA: 0x000456B0 File Offset: 0x000438B0
		IControllerTemplateButton IRacingWheelTemplate.shifter9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(34);
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000306 RID: 774 RVA: 0x000456BA File Offset: 0x000438BA
		IControllerTemplateButton IRacingWheelTemplate.shifter10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(35);
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000307 RID: 775 RVA: 0x000456C4 File Offset: 0x000438C4
		IControllerTemplateButton IRacingWheelTemplate.reverseGear
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(44);
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000308 RID: 776 RVA: 0x000456CE File Offset: 0x000438CE
		IControllerTemplateButton IRacingWheelTemplate.select
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(36);
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000309 RID: 777 RVA: 0x000456D8 File Offset: 0x000438D8
		IControllerTemplateButton IRacingWheelTemplate.start
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(37);
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600030A RID: 778 RVA: 0x000456E2 File Offset: 0x000438E2
		IControllerTemplateButton IRacingWheelTemplate.systemButton
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(38);
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600030B RID: 779 RVA: 0x000456EC File Offset: 0x000438EC
		IControllerTemplateButton IRacingWheelTemplate.horn
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(43);
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600030C RID: 780 RVA: 0x000456F6 File Offset: 0x000438F6
		IControllerTemplateDPad IRacingWheelTemplate.dPad
		{
			get
			{
				return base.GetElement<IControllerTemplateDPad>(45);
			}
		}

		// Token: 0x0600030D RID: 781 RVA: 0x000455B4 File Offset: 0x000437B4
		public RacingWheelTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040003DB RID: 987
		public static readonly Guid typeGuid = new Guid("104e31d8-9115-4dd5-a398-2e54d35e6c83");

		// Token: 0x040003DC RID: 988
		public const int elementId_wheel = 0;

		// Token: 0x040003DD RID: 989
		public const int elementId_accelerator = 1;

		// Token: 0x040003DE RID: 990
		public const int elementId_brake = 2;

		// Token: 0x040003DF RID: 991
		public const int elementId_clutch = 3;

		// Token: 0x040003E0 RID: 992
		public const int elementId_shiftDown = 4;

		// Token: 0x040003E1 RID: 993
		public const int elementId_shiftUp = 5;

		// Token: 0x040003E2 RID: 994
		public const int elementId_wheelButton1 = 6;

		// Token: 0x040003E3 RID: 995
		public const int elementId_wheelButton2 = 7;

		// Token: 0x040003E4 RID: 996
		public const int elementId_wheelButton3 = 8;

		// Token: 0x040003E5 RID: 997
		public const int elementId_wheelButton4 = 9;

		// Token: 0x040003E6 RID: 998
		public const int elementId_wheelButton5 = 10;

		// Token: 0x040003E7 RID: 999
		public const int elementId_wheelButton6 = 11;

		// Token: 0x040003E8 RID: 1000
		public const int elementId_wheelButton7 = 12;

		// Token: 0x040003E9 RID: 1001
		public const int elementId_wheelButton8 = 13;

		// Token: 0x040003EA RID: 1002
		public const int elementId_wheelButton9 = 14;

		// Token: 0x040003EB RID: 1003
		public const int elementId_wheelButton10 = 15;

		// Token: 0x040003EC RID: 1004
		public const int elementId_consoleButton1 = 16;

		// Token: 0x040003ED RID: 1005
		public const int elementId_consoleButton2 = 17;

		// Token: 0x040003EE RID: 1006
		public const int elementId_consoleButton3 = 18;

		// Token: 0x040003EF RID: 1007
		public const int elementId_consoleButton4 = 19;

		// Token: 0x040003F0 RID: 1008
		public const int elementId_consoleButton5 = 20;

		// Token: 0x040003F1 RID: 1009
		public const int elementId_consoleButton6 = 21;

		// Token: 0x040003F2 RID: 1010
		public const int elementId_consoleButton7 = 22;

		// Token: 0x040003F3 RID: 1011
		public const int elementId_consoleButton8 = 23;

		// Token: 0x040003F4 RID: 1012
		public const int elementId_consoleButton9 = 24;

		// Token: 0x040003F5 RID: 1013
		public const int elementId_consoleButton10 = 25;

		// Token: 0x040003F6 RID: 1014
		public const int elementId_shifter1 = 26;

		// Token: 0x040003F7 RID: 1015
		public const int elementId_shifter2 = 27;

		// Token: 0x040003F8 RID: 1016
		public const int elementId_shifter3 = 28;

		// Token: 0x040003F9 RID: 1017
		public const int elementId_shifter4 = 29;

		// Token: 0x040003FA RID: 1018
		public const int elementId_shifter5 = 30;

		// Token: 0x040003FB RID: 1019
		public const int elementId_shifter6 = 31;

		// Token: 0x040003FC RID: 1020
		public const int elementId_shifter7 = 32;

		// Token: 0x040003FD RID: 1021
		public const int elementId_shifter8 = 33;

		// Token: 0x040003FE RID: 1022
		public const int elementId_shifter9 = 34;

		// Token: 0x040003FF RID: 1023
		public const int elementId_shifter10 = 35;

		// Token: 0x04000400 RID: 1024
		public const int elementId_reverseGear = 44;

		// Token: 0x04000401 RID: 1025
		public const int elementId_select = 36;

		// Token: 0x04000402 RID: 1026
		public const int elementId_start = 37;

		// Token: 0x04000403 RID: 1027
		public const int elementId_systemButton = 38;

		// Token: 0x04000404 RID: 1028
		public const int elementId_horn = 43;

		// Token: 0x04000405 RID: 1029
		public const int elementId_dPadUp = 39;

		// Token: 0x04000406 RID: 1030
		public const int elementId_dPadRight = 40;

		// Token: 0x04000407 RID: 1031
		public const int elementId_dPadDown = 41;

		// Token: 0x04000408 RID: 1032
		public const int elementId_dPadLeft = 42;

		// Token: 0x04000409 RID: 1033
		public const int elementId_dPad = 45;
	}
}
