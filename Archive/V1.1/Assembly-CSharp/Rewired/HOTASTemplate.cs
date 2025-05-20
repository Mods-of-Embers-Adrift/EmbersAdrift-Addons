using System;

namespace Rewired
{
	// Token: 0x0200005F RID: 95
	public sealed class HOTASTemplate : ControllerTemplate, IHOTASTemplate, IControllerTemplate
	{
		// Token: 0x17000171 RID: 369
		// (get) Token: 0x0600030F RID: 783 RVA: 0x00045711 File Offset: 0x00043911
		IControllerTemplateButton IHOTASTemplate.stickTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(3);
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000310 RID: 784 RVA: 0x00045519 File Offset: 0x00043719
		IControllerTemplateButton IHOTASTemplate.stickTriggerStage2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000311 RID: 785 RVA: 0x00045522 File Offset: 0x00043722
		IControllerTemplateButton IHOTASTemplate.stickPinkyButton
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0004571A File Offset: 0x0004391A
		IControllerTemplateButton IHOTASTemplate.stickPinkyTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(154);
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0004552B File Offset: 0x0004372B
		IControllerTemplateButton IHOTASTemplate.stickButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000314 RID: 788 RVA: 0x00045534 File Offset: 0x00043734
		IControllerTemplateButton IHOTASTemplate.stickButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0004553D File Offset: 0x0004373D
		IControllerTemplateButton IHOTASTemplate.stickButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000316 RID: 790 RVA: 0x00045546 File Offset: 0x00043746
		IControllerTemplateButton IHOTASTemplate.stickButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000317 RID: 791 RVA: 0x00045550 File Offset: 0x00043750
		IControllerTemplateButton IHOTASTemplate.stickButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000318 RID: 792 RVA: 0x000455F2 File Offset: 0x000437F2
		IControllerTemplateButton IHOTASTemplate.stickButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(11);
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000319 RID: 793 RVA: 0x00045564 File Offset: 0x00043764
		IControllerTemplateButton IHOTASTemplate.stickButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600031A RID: 794 RVA: 0x000455FC File Offset: 0x000437FC
		IControllerTemplateButton IHOTASTemplate.stickButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x0600031B RID: 795 RVA: 0x00045578 File Offset: 0x00043778
		IControllerTemplateButton IHOTASTemplate.stickButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x0600031C RID: 796 RVA: 0x00045582 File Offset: 0x00043782
		IControllerTemplateButton IHOTASTemplate.stickButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x0600031D RID: 797 RVA: 0x00045610 File Offset: 0x00043810
		IControllerTemplateButton IHOTASTemplate.stickBaseButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0004561A File Offset: 0x0004381A
		IControllerTemplateButton IHOTASTemplate.stickBaseButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600031F RID: 799 RVA: 0x00045624 File Offset: 0x00043824
		IControllerTemplateButton IHOTASTemplate.stickBaseButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0004562E File Offset: 0x0004382E
		IControllerTemplateButton IHOTASTemplate.stickBaseButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000321 RID: 801 RVA: 0x00045638 File Offset: 0x00043838
		IControllerTemplateButton IHOTASTemplate.stickBaseButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000322 RID: 802 RVA: 0x00045642 File Offset: 0x00043842
		IControllerTemplateButton IHOTASTemplate.stickBaseButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0004564C File Offset: 0x0004384C
		IControllerTemplateButton IHOTASTemplate.stickBaseButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000324 RID: 804 RVA: 0x00045656 File Offset: 0x00043856
		IControllerTemplateButton IHOTASTemplate.stickBaseButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000325 RID: 805 RVA: 0x00045660 File Offset: 0x00043860
		IControllerTemplateButton IHOTASTemplate.stickBaseButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000326 RID: 806 RVA: 0x0004566A File Offset: 0x0004386A
		IControllerTemplateButton IHOTASTemplate.stickBaseButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(27);
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000327 RID: 807 RVA: 0x00045727 File Offset: 0x00043927
		IControllerTemplateButton IHOTASTemplate.stickBaseButton11
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(161);
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000328 RID: 808 RVA: 0x00045734 File Offset: 0x00043934
		IControllerTemplateButton IHOTASTemplate.stickBaseButton12
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(162);
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000329 RID: 809 RVA: 0x000456C4 File Offset: 0x000438C4
		IControllerTemplateButton IHOTASTemplate.mode1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(44);
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600032A RID: 810 RVA: 0x00045741 File Offset: 0x00043941
		IControllerTemplateButton IHOTASTemplate.mode2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(45);
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0004574B File Offset: 0x0004394B
		IControllerTemplateButton IHOTASTemplate.mode3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(46);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00045755 File Offset: 0x00043955
		IControllerTemplateButton IHOTASTemplate.throttleButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(50);
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0004575F File Offset: 0x0004395F
		IControllerTemplateButton IHOTASTemplate.throttleButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(51);
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00045769 File Offset: 0x00043969
		IControllerTemplateButton IHOTASTemplate.throttleButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(52);
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x0600032F RID: 815 RVA: 0x00045773 File Offset: 0x00043973
		IControllerTemplateButton IHOTASTemplate.throttleButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(53);
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000330 RID: 816 RVA: 0x0004577D File Offset: 0x0004397D
		IControllerTemplateButton IHOTASTemplate.throttleButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(54);
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000331 RID: 817 RVA: 0x00045787 File Offset: 0x00043987
		IControllerTemplateButton IHOTASTemplate.throttleButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(55);
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00045791 File Offset: 0x00043991
		IControllerTemplateButton IHOTASTemplate.throttleButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(56);
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000333 RID: 819 RVA: 0x0004579B File Offset: 0x0004399B
		IControllerTemplateButton IHOTASTemplate.throttleButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(57);
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000334 RID: 820 RVA: 0x000457A5 File Offset: 0x000439A5
		IControllerTemplateButton IHOTASTemplate.throttleButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(58);
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000335 RID: 821 RVA: 0x000457AF File Offset: 0x000439AF
		IControllerTemplateButton IHOTASTemplate.throttleButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(59);
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000336 RID: 822 RVA: 0x000457B9 File Offset: 0x000439B9
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(60);
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000337 RID: 823 RVA: 0x000457C3 File Offset: 0x000439C3
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(61);
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000338 RID: 824 RVA: 0x000457CD File Offset: 0x000439CD
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(62);
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000339 RID: 825 RVA: 0x000457D7 File Offset: 0x000439D7
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(63);
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600033A RID: 826 RVA: 0x000457E1 File Offset: 0x000439E1
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(64);
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600033B RID: 827 RVA: 0x000457EB File Offset: 0x000439EB
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(65);
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600033C RID: 828 RVA: 0x000457F5 File Offset: 0x000439F5
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(66);
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x0600033D RID: 829 RVA: 0x000457FF File Offset: 0x000439FF
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(67);
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00045809 File Offset: 0x00043A09
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(68);
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x0600033F RID: 831 RVA: 0x00045813 File Offset: 0x00043A13
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(69);
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0004581D File Offset: 0x00043A1D
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton11
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(132);
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0004582A File Offset: 0x00043A2A
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton12
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(133);
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00045837 File Offset: 0x00043A37
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton13
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(134);
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000343 RID: 835 RVA: 0x00045844 File Offset: 0x00043A44
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton14
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(135);
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000344 RID: 836 RVA: 0x00045851 File Offset: 0x00043A51
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton15
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(136);
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0004585E File Offset: 0x00043A5E
		IControllerTemplateAxis IHOTASTemplate.throttleSlider1
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(70);
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000346 RID: 838 RVA: 0x00045868 File Offset: 0x00043A68
		IControllerTemplateAxis IHOTASTemplate.throttleSlider2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(71);
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000347 RID: 839 RVA: 0x00045872 File Offset: 0x00043A72
		IControllerTemplateAxis IHOTASTemplate.throttleSlider3
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(72);
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000348 RID: 840 RVA: 0x0004587C File Offset: 0x00043A7C
		IControllerTemplateAxis IHOTASTemplate.throttleSlider4
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(73);
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000349 RID: 841 RVA: 0x00045886 File Offset: 0x00043A86
		IControllerTemplateAxis IHOTASTemplate.throttleDial1
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(74);
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x0600034A RID: 842 RVA: 0x00045890 File Offset: 0x00043A90
		IControllerTemplateAxis IHOTASTemplate.throttleDial2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(142);
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0004589D File Offset: 0x00043A9D
		IControllerTemplateAxis IHOTASTemplate.throttleDial3
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(143);
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600034C RID: 844 RVA: 0x000458AA File Offset: 0x00043AAA
		IControllerTemplateAxis IHOTASTemplate.throttleDial4
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(144);
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600034D RID: 845 RVA: 0x000458B7 File Offset: 0x00043AB7
		IControllerTemplateButton IHOTASTemplate.throttleWheel1Forward
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(145);
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x0600034E RID: 846 RVA: 0x000458C4 File Offset: 0x00043AC4
		IControllerTemplateButton IHOTASTemplate.throttleWheel1Back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(146);
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x0600034F RID: 847 RVA: 0x000458D1 File Offset: 0x00043AD1
		IControllerTemplateButton IHOTASTemplate.throttleWheel1Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(147);
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000350 RID: 848 RVA: 0x000458DE File Offset: 0x00043ADE
		IControllerTemplateButton IHOTASTemplate.throttleWheel2Forward
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(148);
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000351 RID: 849 RVA: 0x000458EB File Offset: 0x00043AEB
		IControllerTemplateButton IHOTASTemplate.throttleWheel2Back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(149);
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000352 RID: 850 RVA: 0x000458F8 File Offset: 0x00043AF8
		IControllerTemplateButton IHOTASTemplate.throttleWheel2Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(150);
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000353 RID: 851 RVA: 0x00045905 File Offset: 0x00043B05
		IControllerTemplateButton IHOTASTemplate.throttleWheel3Forward
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(151);
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000354 RID: 852 RVA: 0x00045912 File Offset: 0x00043B12
		IControllerTemplateButton IHOTASTemplate.throttleWheel3Back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(152);
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0004591F File Offset: 0x00043B1F
		IControllerTemplateButton IHOTASTemplate.throttleWheel3Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(153);
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000356 RID: 854 RVA: 0x0004592C File Offset: 0x00043B2C
		IControllerTemplateAxis IHOTASTemplate.leftPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(168);
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000357 RID: 855 RVA: 0x00045939 File Offset: 0x00043B39
		IControllerTemplateAxis IHOTASTemplate.rightPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(169);
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00045946 File Offset: 0x00043B46
		IControllerTemplateAxis IHOTASTemplate.slidePedals
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(170);
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000359 RID: 857 RVA: 0x00045953 File Offset: 0x00043B53
		IControllerTemplateStick IHOTASTemplate.stick
		{
			get
			{
				return base.GetElement<IControllerTemplateStick>(171);
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600035A RID: 858 RVA: 0x00045960 File Offset: 0x00043B60
		IControllerTemplateThumbStick IHOTASTemplate.stickMiniStick1
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(172);
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0004596D File Offset: 0x00043B6D
		IControllerTemplateThumbStick IHOTASTemplate.stickMiniStick2
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(173);
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0004597A File Offset: 0x00043B7A
		IControllerTemplateHat IHOTASTemplate.stickHat1
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(174);
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600035D RID: 861 RVA: 0x00045987 File Offset: 0x00043B87
		IControllerTemplateHat IHOTASTemplate.stickHat2
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(175);
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x0600035E RID: 862 RVA: 0x00045994 File Offset: 0x00043B94
		IControllerTemplateHat IHOTASTemplate.stickHat3
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(176);
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x0600035F RID: 863 RVA: 0x000459A1 File Offset: 0x00043BA1
		IControllerTemplateHat IHOTASTemplate.stickHat4
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(177);
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000360 RID: 864 RVA: 0x000459AE File Offset: 0x00043BAE
		IControllerTemplateThrottle IHOTASTemplate.throttle1
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(178);
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000361 RID: 865 RVA: 0x000459BB File Offset: 0x00043BBB
		IControllerTemplateThrottle IHOTASTemplate.throttle2
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(179);
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000362 RID: 866 RVA: 0x000459C8 File Offset: 0x00043BC8
		IControllerTemplateThumbStick IHOTASTemplate.throttleMiniStick
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(180);
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000363 RID: 867 RVA: 0x000459D5 File Offset: 0x00043BD5
		IControllerTemplateHat IHOTASTemplate.throttleHat1
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(181);
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000364 RID: 868 RVA: 0x000459E2 File Offset: 0x00043BE2
		IControllerTemplateHat IHOTASTemplate.throttleHat2
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(182);
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000365 RID: 869 RVA: 0x000459EF File Offset: 0x00043BEF
		IControllerTemplateHat IHOTASTemplate.throttleHat3
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(183);
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000366 RID: 870 RVA: 0x000459FC File Offset: 0x00043BFC
		IControllerTemplateHat IHOTASTemplate.throttleHat4
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(184);
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x000455B4 File Offset: 0x000437B4
		public HOTASTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x0400040A RID: 1034
		public static readonly Guid typeGuid = new Guid("061a00cf-d8c2-4f8d-8cb5-a15a010bc53e");

		// Token: 0x0400040B RID: 1035
		public const int elementId_stickX = 0;

		// Token: 0x0400040C RID: 1036
		public const int elementId_stickY = 1;

		// Token: 0x0400040D RID: 1037
		public const int elementId_stickRotate = 2;

		// Token: 0x0400040E RID: 1038
		public const int elementId_stickMiniStick1X = 78;

		// Token: 0x0400040F RID: 1039
		public const int elementId_stickMiniStick1Y = 79;

		// Token: 0x04000410 RID: 1040
		public const int elementId_stickMiniStick1Press = 80;

		// Token: 0x04000411 RID: 1041
		public const int elementId_stickMiniStick2X = 81;

		// Token: 0x04000412 RID: 1042
		public const int elementId_stickMiniStick2Y = 82;

		// Token: 0x04000413 RID: 1043
		public const int elementId_stickMiniStick2Press = 83;

		// Token: 0x04000414 RID: 1044
		public const int elementId_stickTrigger = 3;

		// Token: 0x04000415 RID: 1045
		public const int elementId_stickTriggerStage2 = 4;

		// Token: 0x04000416 RID: 1046
		public const int elementId_stickPinkyButton = 5;

		// Token: 0x04000417 RID: 1047
		public const int elementId_stickPinkyTrigger = 154;

		// Token: 0x04000418 RID: 1048
		public const int elementId_stickButton1 = 6;

		// Token: 0x04000419 RID: 1049
		public const int elementId_stickButton2 = 7;

		// Token: 0x0400041A RID: 1050
		public const int elementId_stickButton3 = 8;

		// Token: 0x0400041B RID: 1051
		public const int elementId_stickButton4 = 9;

		// Token: 0x0400041C RID: 1052
		public const int elementId_stickButton5 = 10;

		// Token: 0x0400041D RID: 1053
		public const int elementId_stickButton6 = 11;

		// Token: 0x0400041E RID: 1054
		public const int elementId_stickButton7 = 12;

		// Token: 0x0400041F RID: 1055
		public const int elementId_stickButton8 = 13;

		// Token: 0x04000420 RID: 1056
		public const int elementId_stickButton9 = 14;

		// Token: 0x04000421 RID: 1057
		public const int elementId_stickButton10 = 15;

		// Token: 0x04000422 RID: 1058
		public const int elementId_stickBaseButton1 = 18;

		// Token: 0x04000423 RID: 1059
		public const int elementId_stickBaseButton2 = 19;

		// Token: 0x04000424 RID: 1060
		public const int elementId_stickBaseButton3 = 20;

		// Token: 0x04000425 RID: 1061
		public const int elementId_stickBaseButton4 = 21;

		// Token: 0x04000426 RID: 1062
		public const int elementId_stickBaseButton5 = 22;

		// Token: 0x04000427 RID: 1063
		public const int elementId_stickBaseButton6 = 23;

		// Token: 0x04000428 RID: 1064
		public const int elementId_stickBaseButton7 = 24;

		// Token: 0x04000429 RID: 1065
		public const int elementId_stickBaseButton8 = 25;

		// Token: 0x0400042A RID: 1066
		public const int elementId_stickBaseButton9 = 26;

		// Token: 0x0400042B RID: 1067
		public const int elementId_stickBaseButton10 = 27;

		// Token: 0x0400042C RID: 1068
		public const int elementId_stickBaseButton11 = 161;

		// Token: 0x0400042D RID: 1069
		public const int elementId_stickBaseButton12 = 162;

		// Token: 0x0400042E RID: 1070
		public const int elementId_stickHat1Up = 28;

		// Token: 0x0400042F RID: 1071
		public const int elementId_stickHat1UpRight = 29;

		// Token: 0x04000430 RID: 1072
		public const int elementId_stickHat1Right = 30;

		// Token: 0x04000431 RID: 1073
		public const int elementId_stickHat1DownRight = 31;

		// Token: 0x04000432 RID: 1074
		public const int elementId_stickHat1Down = 32;

		// Token: 0x04000433 RID: 1075
		public const int elementId_stickHat1DownLeft = 33;

		// Token: 0x04000434 RID: 1076
		public const int elementId_stickHat1Left = 34;

		// Token: 0x04000435 RID: 1077
		public const int elementId_stickHat1Up_Left = 35;

		// Token: 0x04000436 RID: 1078
		public const int elementId_stickHat2Up = 36;

		// Token: 0x04000437 RID: 1079
		public const int elementId_stickHat2Up_right = 37;

		// Token: 0x04000438 RID: 1080
		public const int elementId_stickHat2Right = 38;

		// Token: 0x04000439 RID: 1081
		public const int elementId_stickHat2Down_Right = 39;

		// Token: 0x0400043A RID: 1082
		public const int elementId_stickHat2Down = 40;

		// Token: 0x0400043B RID: 1083
		public const int elementId_stickHat2Down_Left = 41;

		// Token: 0x0400043C RID: 1084
		public const int elementId_stickHat2Left = 42;

		// Token: 0x0400043D RID: 1085
		public const int elementId_stickHat2Up_Left = 43;

		// Token: 0x0400043E RID: 1086
		public const int elementId_stickHat3Up = 84;

		// Token: 0x0400043F RID: 1087
		public const int elementId_stickHat3Up_Right = 85;

		// Token: 0x04000440 RID: 1088
		public const int elementId_stickHat3Right = 86;

		// Token: 0x04000441 RID: 1089
		public const int elementId_stickHat3Down_Right = 87;

		// Token: 0x04000442 RID: 1090
		public const int elementId_stickHat3Down = 88;

		// Token: 0x04000443 RID: 1091
		public const int elementId_stickHat3Down_Left = 89;

		// Token: 0x04000444 RID: 1092
		public const int elementId_stickHat3Left = 90;

		// Token: 0x04000445 RID: 1093
		public const int elementId_stickHat3Up_Left = 91;

		// Token: 0x04000446 RID: 1094
		public const int elementId_stickHat4Up = 92;

		// Token: 0x04000447 RID: 1095
		public const int elementId_stickHat4Up_Right = 93;

		// Token: 0x04000448 RID: 1096
		public const int elementId_stickHat4Right = 94;

		// Token: 0x04000449 RID: 1097
		public const int elementId_stickHat4Down_Right = 95;

		// Token: 0x0400044A RID: 1098
		public const int elementId_stickHat4Down = 96;

		// Token: 0x0400044B RID: 1099
		public const int elementId_stickHat4Down_Left = 97;

		// Token: 0x0400044C RID: 1100
		public const int elementId_stickHat4Left = 98;

		// Token: 0x0400044D RID: 1101
		public const int elementId_stickHat4Up_Left = 99;

		// Token: 0x0400044E RID: 1102
		public const int elementId_mode1 = 44;

		// Token: 0x0400044F RID: 1103
		public const int elementId_mode2 = 45;

		// Token: 0x04000450 RID: 1104
		public const int elementId_mode3 = 46;

		// Token: 0x04000451 RID: 1105
		public const int elementId_throttle1Axis = 49;

		// Token: 0x04000452 RID: 1106
		public const int elementId_throttle2Axis = 155;

		// Token: 0x04000453 RID: 1107
		public const int elementId_throttle1MinDetent = 166;

		// Token: 0x04000454 RID: 1108
		public const int elementId_throttle2MinDetent = 167;

		// Token: 0x04000455 RID: 1109
		public const int elementId_throttleButton1 = 50;

		// Token: 0x04000456 RID: 1110
		public const int elementId_throttleButton2 = 51;

		// Token: 0x04000457 RID: 1111
		public const int elementId_throttleButton3 = 52;

		// Token: 0x04000458 RID: 1112
		public const int elementId_throttleButton4 = 53;

		// Token: 0x04000459 RID: 1113
		public const int elementId_throttleButton5 = 54;

		// Token: 0x0400045A RID: 1114
		public const int elementId_throttleButton6 = 55;

		// Token: 0x0400045B RID: 1115
		public const int elementId_throttleButton7 = 56;

		// Token: 0x0400045C RID: 1116
		public const int elementId_throttleButton8 = 57;

		// Token: 0x0400045D RID: 1117
		public const int elementId_throttleButton9 = 58;

		// Token: 0x0400045E RID: 1118
		public const int elementId_throttleButton10 = 59;

		// Token: 0x0400045F RID: 1119
		public const int elementId_throttleBaseButton1 = 60;

		// Token: 0x04000460 RID: 1120
		public const int elementId_throttleBaseButton2 = 61;

		// Token: 0x04000461 RID: 1121
		public const int elementId_throttleBaseButton3 = 62;

		// Token: 0x04000462 RID: 1122
		public const int elementId_throttleBaseButton4 = 63;

		// Token: 0x04000463 RID: 1123
		public const int elementId_throttleBaseButton5 = 64;

		// Token: 0x04000464 RID: 1124
		public const int elementId_throttleBaseButton6 = 65;

		// Token: 0x04000465 RID: 1125
		public const int elementId_throttleBaseButton7 = 66;

		// Token: 0x04000466 RID: 1126
		public const int elementId_throttleBaseButton8 = 67;

		// Token: 0x04000467 RID: 1127
		public const int elementId_throttleBaseButton9 = 68;

		// Token: 0x04000468 RID: 1128
		public const int elementId_throttleBaseButton10 = 69;

		// Token: 0x04000469 RID: 1129
		public const int elementId_throttleBaseButton11 = 132;

		// Token: 0x0400046A RID: 1130
		public const int elementId_throttleBaseButton12 = 133;

		// Token: 0x0400046B RID: 1131
		public const int elementId_throttleBaseButton13 = 134;

		// Token: 0x0400046C RID: 1132
		public const int elementId_throttleBaseButton14 = 135;

		// Token: 0x0400046D RID: 1133
		public const int elementId_throttleBaseButton15 = 136;

		// Token: 0x0400046E RID: 1134
		public const int elementId_throttleSlider1 = 70;

		// Token: 0x0400046F RID: 1135
		public const int elementId_throttleSlider2 = 71;

		// Token: 0x04000470 RID: 1136
		public const int elementId_throttleSlider3 = 72;

		// Token: 0x04000471 RID: 1137
		public const int elementId_throttleSlider4 = 73;

		// Token: 0x04000472 RID: 1138
		public const int elementId_throttleDial1 = 74;

		// Token: 0x04000473 RID: 1139
		public const int elementId_throttleDial2 = 142;

		// Token: 0x04000474 RID: 1140
		public const int elementId_throttleDial3 = 143;

		// Token: 0x04000475 RID: 1141
		public const int elementId_throttleDial4 = 144;

		// Token: 0x04000476 RID: 1142
		public const int elementId_throttleMiniStickX = 75;

		// Token: 0x04000477 RID: 1143
		public const int elementId_throttleMiniStickY = 76;

		// Token: 0x04000478 RID: 1144
		public const int elementId_throttleMiniStickPress = 77;

		// Token: 0x04000479 RID: 1145
		public const int elementId_throttleWheel1Forward = 145;

		// Token: 0x0400047A RID: 1146
		public const int elementId_throttleWheel1Back = 146;

		// Token: 0x0400047B RID: 1147
		public const int elementId_throttleWheel1Press = 147;

		// Token: 0x0400047C RID: 1148
		public const int elementId_throttleWheel2Forward = 148;

		// Token: 0x0400047D RID: 1149
		public const int elementId_throttleWheel2Back = 149;

		// Token: 0x0400047E RID: 1150
		public const int elementId_throttleWheel2Press = 150;

		// Token: 0x0400047F RID: 1151
		public const int elementId_throttleWheel3Forward = 151;

		// Token: 0x04000480 RID: 1152
		public const int elementId_throttleWheel3Back = 152;

		// Token: 0x04000481 RID: 1153
		public const int elementId_throttleWheel3Press = 153;

		// Token: 0x04000482 RID: 1154
		public const int elementId_throttleHat1Up = 100;

		// Token: 0x04000483 RID: 1155
		public const int elementId_throttleHat1Up_Right = 101;

		// Token: 0x04000484 RID: 1156
		public const int elementId_throttleHat1Right = 102;

		// Token: 0x04000485 RID: 1157
		public const int elementId_throttleHat1Down_Right = 103;

		// Token: 0x04000486 RID: 1158
		public const int elementId_throttleHat1Down = 104;

		// Token: 0x04000487 RID: 1159
		public const int elementId_throttleHat1Down_Left = 105;

		// Token: 0x04000488 RID: 1160
		public const int elementId_throttleHat1Left = 106;

		// Token: 0x04000489 RID: 1161
		public const int elementId_throttleHat1Up_Left = 107;

		// Token: 0x0400048A RID: 1162
		public const int elementId_throttleHat2Up = 108;

		// Token: 0x0400048B RID: 1163
		public const int elementId_throttleHat2Up_Right = 109;

		// Token: 0x0400048C RID: 1164
		public const int elementId_throttleHat2Right = 110;

		// Token: 0x0400048D RID: 1165
		public const int elementId_throttleHat2Down_Right = 111;

		// Token: 0x0400048E RID: 1166
		public const int elementId_throttleHat2Down = 112;

		// Token: 0x0400048F RID: 1167
		public const int elementId_throttleHat2Down_Left = 113;

		// Token: 0x04000490 RID: 1168
		public const int elementId_throttleHat2Left = 114;

		// Token: 0x04000491 RID: 1169
		public const int elementId_throttleHat2Up_Left = 115;

		// Token: 0x04000492 RID: 1170
		public const int elementId_throttleHat3Up = 116;

		// Token: 0x04000493 RID: 1171
		public const int elementId_throttleHat3Up_Right = 117;

		// Token: 0x04000494 RID: 1172
		public const int elementId_throttleHat3Right = 118;

		// Token: 0x04000495 RID: 1173
		public const int elementId_throttleHat3Down_Right = 119;

		// Token: 0x04000496 RID: 1174
		public const int elementId_throttleHat3Down = 120;

		// Token: 0x04000497 RID: 1175
		public const int elementId_throttleHat3Down_Left = 121;

		// Token: 0x04000498 RID: 1176
		public const int elementId_throttleHat3Left = 122;

		// Token: 0x04000499 RID: 1177
		public const int elementId_throttleHat3Up_Left = 123;

		// Token: 0x0400049A RID: 1178
		public const int elementId_throttleHat4Up = 124;

		// Token: 0x0400049B RID: 1179
		public const int elementId_throttleHat4Up_Right = 125;

		// Token: 0x0400049C RID: 1180
		public const int elementId_throttleHat4Right = 126;

		// Token: 0x0400049D RID: 1181
		public const int elementId_throttleHat4Down_Right = 127;

		// Token: 0x0400049E RID: 1182
		public const int elementId_throttleHat4Down = 128;

		// Token: 0x0400049F RID: 1183
		public const int elementId_throttleHat4Down_Left = 129;

		// Token: 0x040004A0 RID: 1184
		public const int elementId_throttleHat4Left = 130;

		// Token: 0x040004A1 RID: 1185
		public const int elementId_throttleHat4Up_Left = 131;

		// Token: 0x040004A2 RID: 1186
		public const int elementId_leftPedal = 168;

		// Token: 0x040004A3 RID: 1187
		public const int elementId_rightPedal = 169;

		// Token: 0x040004A4 RID: 1188
		public const int elementId_slidePedals = 170;

		// Token: 0x040004A5 RID: 1189
		public const int elementId_stick = 171;

		// Token: 0x040004A6 RID: 1190
		public const int elementId_stickMiniStick1 = 172;

		// Token: 0x040004A7 RID: 1191
		public const int elementId_stickMiniStick2 = 173;

		// Token: 0x040004A8 RID: 1192
		public const int elementId_stickHat1 = 174;

		// Token: 0x040004A9 RID: 1193
		public const int elementId_stickHat2 = 175;

		// Token: 0x040004AA RID: 1194
		public const int elementId_stickHat3 = 176;

		// Token: 0x040004AB RID: 1195
		public const int elementId_stickHat4 = 177;

		// Token: 0x040004AC RID: 1196
		public const int elementId_throttle1 = 178;

		// Token: 0x040004AD RID: 1197
		public const int elementId_throttle2 = 179;

		// Token: 0x040004AE RID: 1198
		public const int elementId_throttleMiniStick = 180;

		// Token: 0x040004AF RID: 1199
		public const int elementId_throttleHat1 = 181;

		// Token: 0x040004B0 RID: 1200
		public const int elementId_throttleHat2 = 182;

		// Token: 0x040004B1 RID: 1201
		public const int elementId_throttleHat3 = 183;

		// Token: 0x040004B2 RID: 1202
		public const int elementId_throttleHat4 = 184;
	}
}
