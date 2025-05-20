using System;
using SoL.Game.UI;
using SoL.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Utilities.Offline
{
	// Token: 0x0200030E RID: 782
	public class OfflineInputManager : MonoBehaviour, IInputManager
	{
		// Token: 0x060015BB RID: 5563 RVA: 0x00051420 File Offset: 0x0004F620
		private void Start()
		{
			if (ClientGameManager.InputManager != null)
			{
				base.enabled = false;
				return;
			}
			ClientGameManager.InputManager = this;
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x000FDD18 File Offset: 0x000FBF18
		private void Update()
		{
			bool flag = EventSystem.current && EventSystem.current.IsPointerOverGameObject();
			bool mouseButton = Input.GetMouseButton(0);
			bool mouseButton2 = Input.GetMouseButton(1);
			this.HoldingLMB = (flag ? (this.HoldingLMB && mouseButton) : mouseButton);
			this.HoldingRMB = (flag ? (this.HoldingRMB && mouseButton2) : mouseButton2);
			this.HoldingShift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			this.HoldingCtrl = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
			this.HoldingAlt = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
			this.EnterDown = (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
			this.SpaceDown = Input.GetKeyDown(KeyCode.Space);
			this.TabDown = Input.GetKeyDown(KeyCode.Tab);
			int num = Input.GetKey(KeyCode.W) ? 1 : 0;
			int num2 = Input.GetKey(KeyCode.S) ? -1 : 0;
			int num3 = Input.GetKey(KeyCode.A) ? -1 : 0;
			int num4 = Input.GetKey(KeyCode.D) ? 1 : 0;
			int num5 = num3 + num4;
			float y = (this.HoldingLMB && this.HoldingRMB) ? 1f : ((float)(num + num2));
			this.MovementInput = new Vector2((float)num5, y);
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060015BD RID: 5565 RVA: 0x000FDE74 File Offset: 0x000FC074
		// (remove) Token: 0x060015BE RID: 5566 RVA: 0x000FDEAC File Offset: 0x000FC0AC
		public event Action OffhandSwap;

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x00051437 File Offset: 0x0004F637
		public bool HoldingLMBRaw
		{
			get
			{
				return this.HoldingLMB;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x060015C0 RID: 5568 RVA: 0x0005143F File Offset: 0x0004F63F
		public bool HoldingRMBRaw
		{
			get
			{
				return this.HoldingRMB;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x060015C1 RID: 5569 RVA: 0x00051447 File Offset: 0x0004F647
		// (set) Token: 0x060015C2 RID: 5570 RVA: 0x0005144F File Offset: 0x0004F64F
		public bool HoldingLMB { get; private set; }

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x060015C3 RID: 5571 RVA: 0x00051458 File Offset: 0x0004F658
		// (set) Token: 0x060015C4 RID: 5572 RVA: 0x00051460 File Offset: 0x0004F660
		public bool HoldingRMB { get; private set; }

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x060015C5 RID: 5573 RVA: 0x00051469 File Offset: 0x0004F669
		// (set) Token: 0x060015C6 RID: 5574 RVA: 0x00051471 File Offset: 0x0004F671
		public bool HoldingShift { get; private set; }

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x060015C7 RID: 5575 RVA: 0x0005147A File Offset: 0x0004F67A
		// (set) Token: 0x060015C8 RID: 5576 RVA: 0x00051482 File Offset: 0x0004F682
		public bool HoldingCtrl { get; private set; }

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x060015C9 RID: 5577 RVA: 0x0005148B File Offset: 0x0004F68B
		// (set) Token: 0x060015CA RID: 5578 RVA: 0x00051493 File Offset: 0x0004F693
		public bool HoldingAlt { get; private set; }

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x0005149C File Offset: 0x0004F69C
		// (set) Token: 0x060015CC RID: 5580 RVA: 0x000514A4 File Offset: 0x0004F6A4
		public bool IsTurning { get; private set; }

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x060015CD RID: 5581 RVA: 0x000514AD File Offset: 0x0004F6AD
		// (set) Token: 0x060015CE RID: 5582 RVA: 0x000514B5 File Offset: 0x0004F6B5
		public bool IsWalking { get; private set; }

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x060015CF RID: 5583 RVA: 0x000514BE File Offset: 0x0004F6BE
		// (set) Token: 0x060015D0 RID: 5584 RVA: 0x000514C6 File Offset: 0x0004F6C6
		public bool IsCrouching { get; private set; }

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x060015D1 RID: 5585 RVA: 0x000514CF File Offset: 0x0004F6CF
		// (set) Token: 0x060015D2 RID: 5586 RVA: 0x000514D7 File Offset: 0x0004F6D7
		public bool EnterDown { get; private set; }

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x000514E0 File Offset: 0x0004F6E0
		// (set) Token: 0x060015D4 RID: 5588 RVA: 0x000514E8 File Offset: 0x0004F6E8
		public bool SpaceDown { get; private set; }

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x060015D5 RID: 5589 RVA: 0x000514F1 File Offset: 0x0004F6F1
		// (set) Token: 0x060015D6 RID: 5590 RVA: 0x000514F9 File Offset: 0x0004F6F9
		public bool TabDown { get; private set; }

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x060015D7 RID: 5591 RVA: 0x00051502 File Offset: 0x0004F702
		public bool PreventInput
		{
			get
			{
				return this.InputPreventionFlags > InputPreventionFlags.None;
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x060015D8 RID: 5592 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool PreventCharacterMovement
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x060015D9 RID: 5593 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool PreventCharacterRotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x060015DA RID: 5594 RVA: 0x0005150D File Offset: 0x0004F70D
		public bool PreventInputForUI
		{
			get
			{
				return this.InputPreventionFlags.PreventForUI();
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x060015DB RID: 5595 RVA: 0x0005151A File Offset: 0x0004F71A
		public bool PreventInputForLook
		{
			get
			{
				return this.InputPreventionFlags.PreventForLook();
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x060015DC RID: 5596 RVA: 0x00051527 File Offset: 0x0004F727
		// (set) Token: 0x060015DD RID: 5597 RVA: 0x0005152F File Offset: 0x0004F72F
		public float MovementInputSqrMagnitude { get; private set; }

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x060015DE RID: 5598 RVA: 0x00051538 File Offset: 0x0004F738
		// (set) Token: 0x060015DF RID: 5599 RVA: 0x00051540 File Offset: 0x0004F740
		public float NormalizedMovementInputMagnitude { get; private set; }

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x060015E0 RID: 5600 RVA: 0x00051549 File Offset: 0x0004F749
		// (set) Token: 0x060015E1 RID: 5601 RVA: 0x00051551 File Offset: 0x0004F751
		public Vector2 MovementInput { get; private set; }

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x0005155A File Offset: 0x0004F75A
		// (set) Token: 0x060015E3 RID: 5603 RVA: 0x00051562 File Offset: 0x0004F762
		public Vector2 NormalizedMovementInput { get; private set; }

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x060015E4 RID: 5604 RVA: 0x0005156B File Offset: 0x0004F76B
		// (set) Token: 0x060015E5 RID: 5605 RVA: 0x00051573 File Offset: 0x0004F773
		public Vector2 LookInput { get; private set; }

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x0005157C File Offset: 0x0004F77C
		// (set) Token: 0x060015E7 RID: 5607 RVA: 0x00051584 File Offset: 0x0004F784
		public InputPreventionFlags InputPreventionFlags { get; private set; }

		// Token: 0x060015E8 RID: 5608 RVA: 0x0005158D File Offset: 0x0004F78D
		void IInputManager.SetInputPreventionFlag(InputPreventionFlags flag)
		{
			this.InputPreventionFlags |= flag;
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x0005159D File Offset: 0x0004F79D
		void IInputManager.UnsetInputPreventionFlag(InputPreventionFlags flag)
		{
			this.InputPreventionFlags &= ~flag;
		}
	}
}
