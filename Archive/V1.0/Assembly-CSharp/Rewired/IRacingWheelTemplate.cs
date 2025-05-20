using System;

namespace Rewired
{
	// Token: 0x02000058 RID: 88
	public interface IRacingWheelTemplate : IControllerTemplate
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001E5 RID: 485
		IControllerTemplateAxis wheel { get; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001E6 RID: 486
		IControllerTemplateAxis accelerator { get; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001E7 RID: 487
		IControllerTemplateAxis brake { get; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001E8 RID: 488
		IControllerTemplateAxis clutch { get; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001E9 RID: 489
		IControllerTemplateButton shiftDown { get; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001EA RID: 490
		IControllerTemplateButton shiftUp { get; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001EB RID: 491
		IControllerTemplateButton wheelButton1 { get; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001EC RID: 492
		IControllerTemplateButton wheelButton2 { get; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001ED RID: 493
		IControllerTemplateButton wheelButton3 { get; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001EE RID: 494
		IControllerTemplateButton wheelButton4 { get; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001EF RID: 495
		IControllerTemplateButton wheelButton5 { get; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001F0 RID: 496
		IControllerTemplateButton wheelButton6 { get; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001F1 RID: 497
		IControllerTemplateButton wheelButton7 { get; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001F2 RID: 498
		IControllerTemplateButton wheelButton8 { get; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001F3 RID: 499
		IControllerTemplateButton wheelButton9 { get; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001F4 RID: 500
		IControllerTemplateButton wheelButton10 { get; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001F5 RID: 501
		IControllerTemplateButton consoleButton1 { get; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001F6 RID: 502
		IControllerTemplateButton consoleButton2 { get; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001F7 RID: 503
		IControllerTemplateButton consoleButton3 { get; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001F8 RID: 504
		IControllerTemplateButton consoleButton4 { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001F9 RID: 505
		IControllerTemplateButton consoleButton5 { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001FA RID: 506
		IControllerTemplateButton consoleButton6 { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001FB RID: 507
		IControllerTemplateButton consoleButton7 { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001FC RID: 508
		IControllerTemplateButton consoleButton8 { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001FD RID: 509
		IControllerTemplateButton consoleButton9 { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001FE RID: 510
		IControllerTemplateButton consoleButton10 { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001FF RID: 511
		IControllerTemplateButton shifter1 { get; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000200 RID: 512
		IControllerTemplateButton shifter2 { get; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000201 RID: 513
		IControllerTemplateButton shifter3 { get; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000202 RID: 514
		IControllerTemplateButton shifter4 { get; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000203 RID: 515
		IControllerTemplateButton shifter5 { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000204 RID: 516
		IControllerTemplateButton shifter6 { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000205 RID: 517
		IControllerTemplateButton shifter7 { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000206 RID: 518
		IControllerTemplateButton shifter8 { get; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000207 RID: 519
		IControllerTemplateButton shifter9 { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000208 RID: 520
		IControllerTemplateButton shifter10 { get; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000209 RID: 521
		IControllerTemplateButton reverseGear { get; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600020A RID: 522
		IControllerTemplateButton select { get; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600020B RID: 523
		IControllerTemplateButton start { get; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600020C RID: 524
		IControllerTemplateButton systemButton { get; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600020D RID: 525
		IControllerTemplateButton horn { get; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600020E RID: 526
		IControllerTemplateDPad dPad { get; }
	}
}
