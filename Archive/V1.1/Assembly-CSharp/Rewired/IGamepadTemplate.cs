using System;

namespace Rewired
{
	// Token: 0x02000057 RID: 87
	public interface IGamepadTemplate : IControllerTemplate
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001C8 RID: 456
		IControllerTemplateButton actionBottomRow1 { get; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001C9 RID: 457
		IControllerTemplateButton a { get; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001CA RID: 458
		IControllerTemplateButton actionBottomRow2 { get; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001CB RID: 459
		IControllerTemplateButton b { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001CC RID: 460
		IControllerTemplateButton actionBottomRow3 { get; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001CD RID: 461
		IControllerTemplateButton c { get; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001CE RID: 462
		IControllerTemplateButton actionTopRow1 { get; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001CF RID: 463
		IControllerTemplateButton x { get; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001D0 RID: 464
		IControllerTemplateButton actionTopRow2 { get; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001D1 RID: 465
		IControllerTemplateButton y { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001D2 RID: 466
		IControllerTemplateButton actionTopRow3 { get; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001D3 RID: 467
		IControllerTemplateButton z { get; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001D4 RID: 468
		IControllerTemplateButton leftShoulder1 { get; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001D5 RID: 469
		IControllerTemplateButton leftBumper { get; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001D6 RID: 470
		IControllerTemplateAxis leftShoulder2 { get; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001D7 RID: 471
		IControllerTemplateAxis leftTrigger { get; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001D8 RID: 472
		IControllerTemplateButton rightShoulder1 { get; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001D9 RID: 473
		IControllerTemplateButton rightBumper { get; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001DA RID: 474
		IControllerTemplateAxis rightShoulder2 { get; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001DB RID: 475
		IControllerTemplateAxis rightTrigger { get; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001DC RID: 476
		IControllerTemplateButton center1 { get; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001DD RID: 477
		IControllerTemplateButton back { get; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001DE RID: 478
		IControllerTemplateButton center2 { get; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001DF RID: 479
		IControllerTemplateButton start { get; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001E0 RID: 480
		IControllerTemplateButton center3 { get; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001E1 RID: 481
		IControllerTemplateButton guide { get; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001E2 RID: 482
		IControllerTemplateThumbStick leftStick { get; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001E3 RID: 483
		IControllerTemplateThumbStick rightStick { get; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001E4 RID: 484
		IControllerTemplateDPad dPad { get; }
	}
}
