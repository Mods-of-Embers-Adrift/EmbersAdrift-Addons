using System;
using SoL.Game.EffectSystem;

namespace SoL.Game
{
	// Token: 0x020005FA RID: 1530
	public class VitalsProxy : Vitals
	{
		// Token: 0x060030EE RID: 12526 RVA: 0x00061BBD File Offset: 0x0005FDBD
		private void Start()
		{
			this.m_baseStats = StatTypeExtensions.GetStatTypeCollection();
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x0006109C File Offset: 0x0005F29C
		public override float GetHealthPercent()
		{
			return 1f;
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x0006109C File Offset: 0x0005F29C
		public override float GetArmorClassPercent()
		{
			return 1f;
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x060030F1 RID: 12529 RVA: 0x00061227 File Offset: 0x0005F427
		public override float Health
		{
			get
			{
				return 10f;
			}
		}

		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x060030F2 RID: 12530 RVA: 0x00061BCA File Offset: 0x0005FDCA
		public override float HealthWound { get; }

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x060030F3 RID: 12531 RVA: 0x00061BD2 File Offset: 0x0005FDD2
		public override int MaxHealth { get; }

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x060030F4 RID: 12532 RVA: 0x0006109C File Offset: 0x0005F29C
		public override float Stamina
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x060030F5 RID: 12533 RVA: 0x00061BDA File Offset: 0x0005FDDA
		public override float StaminaWound { get; }

		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x060030F6 RID: 12534 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		public override int ArmorClass
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x060030F7 RID: 12535 RVA: 0x00061BE6 File Offset: 0x0005FDE6
		public override int MaxArmorClass { get; }
	}
}
