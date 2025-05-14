using System;
using SoL.Game.UI;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x020004F6 RID: 1270
	public interface IInputManager
	{
		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x060023B1 RID: 9137
		bool HoldingLMBRaw { get; }

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x060023B2 RID: 9138
		bool HoldingRMBRaw { get; }

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x060023B3 RID: 9139
		bool HoldingLMB { get; }

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x060023B4 RID: 9140
		bool HoldingRMB { get; }

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x060023B5 RID: 9141
		bool HoldingShift { get; }

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x060023B6 RID: 9142
		bool HoldingCtrl { get; }

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x060023B7 RID: 9143
		bool HoldingAlt { get; }

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x060023B8 RID: 9144
		bool IsTurning { get; }

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x060023B9 RID: 9145
		bool IsWalking { get; }

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x060023BA RID: 9146
		bool IsCrouching { get; }

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x060023BB RID: 9147
		bool EnterDown { get; }

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x060023BC RID: 9148
		bool SpaceDown { get; }

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x060023BD RID: 9149
		bool TabDown { get; }

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x060023BE RID: 9150
		bool PreventInput { get; }

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x060023BF RID: 9151
		bool PreventCharacterMovement { get; }

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x060023C0 RID: 9152
		bool PreventCharacterRotation { get; }

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x060023C1 RID: 9153
		bool PreventInputForUI { get; }

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x060023C2 RID: 9154
		bool PreventInputForLook { get; }

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x060023C3 RID: 9155
		float MovementInputSqrMagnitude { get; }

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x060023C4 RID: 9156
		float NormalizedMovementInputMagnitude { get; }

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x060023C5 RID: 9157
		Vector2 MovementInput { get; }

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x060023C6 RID: 9158
		Vector2 NormalizedMovementInput { get; }

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x060023C7 RID: 9159
		Vector2 LookInput { get; }

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x060023C8 RID: 9160
		InputPreventionFlags InputPreventionFlags { get; }

		// Token: 0x060023C9 RID: 9161
		void SetInputPreventionFlag(InputPreventionFlags flag);

		// Token: 0x060023CA RID: 9162
		void UnsetInputPreventionFlag(InputPreventionFlags flag);
	}
}
