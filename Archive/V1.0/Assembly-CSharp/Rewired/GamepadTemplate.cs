using System;

namespace Rewired
{
	// Token: 0x0200005D RID: 93
	public sealed class GamepadTemplate : ControllerTemplate, IGamepadTemplate, IControllerTemplate
	{
		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x00045519 File Offset: 0x00043719
		IControllerTemplateButton IGamepadTemplate.actionBottomRow1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x00045519 File Offset: 0x00043719
		IControllerTemplateButton IGamepadTemplate.a
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00045522 File Offset: 0x00043722
		IControllerTemplateButton IGamepadTemplate.actionBottomRow2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x00045522 File Offset: 0x00043722
		IControllerTemplateButton IGamepadTemplate.b
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0004552B File Offset: 0x0004372B
		IControllerTemplateButton IGamepadTemplate.actionBottomRow3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x0004552B File Offset: 0x0004372B
		IControllerTemplateButton IGamepadTemplate.c
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00045534 File Offset: 0x00043734
		IControllerTemplateButton IGamepadTemplate.actionTopRow1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060002CB RID: 715 RVA: 0x00045534 File Offset: 0x00043734
		IControllerTemplateButton IGamepadTemplate.x
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0004553D File Offset: 0x0004373D
		IControllerTemplateButton IGamepadTemplate.actionTopRow2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0004553D File Offset: 0x0004373D
		IControllerTemplateButton IGamepadTemplate.y
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00045546 File Offset: 0x00043746
		IControllerTemplateButton IGamepadTemplate.actionTopRow3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060002CF RID: 719 RVA: 0x00045546 File Offset: 0x00043746
		IControllerTemplateButton IGamepadTemplate.z
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00045550 File Offset: 0x00043750
		IControllerTemplateButton IGamepadTemplate.leftShoulder1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x00045550 File Offset: 0x00043750
		IControllerTemplateButton IGamepadTemplate.leftBumper
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0004555A File Offset: 0x0004375A
		IControllerTemplateAxis IGamepadTemplate.leftShoulder2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(11);
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0004555A File Offset: 0x0004375A
		IControllerTemplateAxis IGamepadTemplate.leftTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(11);
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00045564 File Offset: 0x00043764
		IControllerTemplateButton IGamepadTemplate.rightShoulder1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x00045564 File Offset: 0x00043764
		IControllerTemplateButton IGamepadTemplate.rightBumper
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0004556E File Offset: 0x0004376E
		IControllerTemplateAxis IGamepadTemplate.rightShoulder2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(13);
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x0004556E File Offset: 0x0004376E
		IControllerTemplateAxis IGamepadTemplate.rightTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(13);
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x00045578 File Offset: 0x00043778
		IControllerTemplateButton IGamepadTemplate.center1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x00045578 File Offset: 0x00043778
		IControllerTemplateButton IGamepadTemplate.back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060002DA RID: 730 RVA: 0x00045582 File Offset: 0x00043782
		IControllerTemplateButton IGamepadTemplate.center2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060002DB RID: 731 RVA: 0x00045582 File Offset: 0x00043782
		IControllerTemplateButton IGamepadTemplate.start
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x060002DC RID: 732 RVA: 0x0004558C File Offset: 0x0004378C
		IControllerTemplateButton IGamepadTemplate.center3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060002DD RID: 733 RVA: 0x0004558C File Offset: 0x0004378C
		IControllerTemplateButton IGamepadTemplate.guide
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060002DE RID: 734 RVA: 0x00045596 File Offset: 0x00043796
		IControllerTemplateThumbStick IGamepadTemplate.leftStick
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(23);
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060002DF RID: 735 RVA: 0x000455A0 File Offset: 0x000437A0
		IControllerTemplateThumbStick IGamepadTemplate.rightStick
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(24);
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x000455AA File Offset: 0x000437AA
		IControllerTemplateDPad IGamepadTemplate.dPad
		{
			get
			{
				return base.GetElement<IControllerTemplateDPad>(25);
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x000455B4 File Offset: 0x000437B4
		public GamepadTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040003B3 RID: 947
		public static readonly Guid typeGuid = new Guid("83b427e4-086f-47f3-bb06-be266abd1ca5");

		// Token: 0x040003B4 RID: 948
		public const int elementId_leftStickX = 0;

		// Token: 0x040003B5 RID: 949
		public const int elementId_leftStickY = 1;

		// Token: 0x040003B6 RID: 950
		public const int elementId_rightStickX = 2;

		// Token: 0x040003B7 RID: 951
		public const int elementId_rightStickY = 3;

		// Token: 0x040003B8 RID: 952
		public const int elementId_actionBottomRow1 = 4;

		// Token: 0x040003B9 RID: 953
		public const int elementId_a = 4;

		// Token: 0x040003BA RID: 954
		public const int elementId_actionBottomRow2 = 5;

		// Token: 0x040003BB RID: 955
		public const int elementId_b = 5;

		// Token: 0x040003BC RID: 956
		public const int elementId_actionBottomRow3 = 6;

		// Token: 0x040003BD RID: 957
		public const int elementId_c = 6;

		// Token: 0x040003BE RID: 958
		public const int elementId_actionTopRow1 = 7;

		// Token: 0x040003BF RID: 959
		public const int elementId_x = 7;

		// Token: 0x040003C0 RID: 960
		public const int elementId_actionTopRow2 = 8;

		// Token: 0x040003C1 RID: 961
		public const int elementId_y = 8;

		// Token: 0x040003C2 RID: 962
		public const int elementId_actionTopRow3 = 9;

		// Token: 0x040003C3 RID: 963
		public const int elementId_z = 9;

		// Token: 0x040003C4 RID: 964
		public const int elementId_leftShoulder1 = 10;

		// Token: 0x040003C5 RID: 965
		public const int elementId_leftBumper = 10;

		// Token: 0x040003C6 RID: 966
		public const int elementId_leftShoulder2 = 11;

		// Token: 0x040003C7 RID: 967
		public const int elementId_leftTrigger = 11;

		// Token: 0x040003C8 RID: 968
		public const int elementId_rightShoulder1 = 12;

		// Token: 0x040003C9 RID: 969
		public const int elementId_rightBumper = 12;

		// Token: 0x040003CA RID: 970
		public const int elementId_rightShoulder2 = 13;

		// Token: 0x040003CB RID: 971
		public const int elementId_rightTrigger = 13;

		// Token: 0x040003CC RID: 972
		public const int elementId_center1 = 14;

		// Token: 0x040003CD RID: 973
		public const int elementId_back = 14;

		// Token: 0x040003CE RID: 974
		public const int elementId_center2 = 15;

		// Token: 0x040003CF RID: 975
		public const int elementId_start = 15;

		// Token: 0x040003D0 RID: 976
		public const int elementId_center3 = 16;

		// Token: 0x040003D1 RID: 977
		public const int elementId_guide = 16;

		// Token: 0x040003D2 RID: 978
		public const int elementId_leftStickButton = 17;

		// Token: 0x040003D3 RID: 979
		public const int elementId_rightStickButton = 18;

		// Token: 0x040003D4 RID: 980
		public const int elementId_dPadUp = 19;

		// Token: 0x040003D5 RID: 981
		public const int elementId_dPadRight = 20;

		// Token: 0x040003D6 RID: 982
		public const int elementId_dPadDown = 21;

		// Token: 0x040003D7 RID: 983
		public const int elementId_dPadLeft = 22;

		// Token: 0x040003D8 RID: 984
		public const int elementId_leftStick = 23;

		// Token: 0x040003D9 RID: 985
		public const int elementId_rightStick = 24;

		// Token: 0x040003DA RID: 986
		public const int elementId_dPad = 25;
	}
}
