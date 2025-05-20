using System;
using SoL.Game.Crafting;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B82 RID: 2946
	public class InteractiveChest : GameEntityComponent
	{
		// Token: 0x06005AD1 RID: 23249 RVA: 0x0007CF95 File Offset: 0x0007B195
		private void Awake()
		{
			if (InteractiveChest.m_isOpenKey == -1)
			{
				InteractiveChest.m_isOpenKey = Animator.StringToHash("IsOpen");
			}
		}

		// Token: 0x06005AD2 RID: 23250 RVA: 0x001ED9DC File Offset: 0x001EBBDC
		private void Start()
		{
			if (!GameManager.IsServer && base.GameEntity != null && base.GameEntity.Interactive != null && base.GameEntity.Interactive.TryGetAsType(out this.m_node) && this.m_node.LooterString != null)
			{
				this.m_node.LooterString.Changed += this.LooterStringOnChanged;
				this.RefreshOpenState();
			}
		}

		// Token: 0x06005AD3 RID: 23251 RVA: 0x0007CFAE File Offset: 0x0007B1AE
		private void OnDestroy()
		{
			if (!GameManager.IsServer && this.m_node && this.m_node.LooterString != null)
			{
				this.m_node.LooterString.Changed -= this.LooterStringOnChanged;
			}
		}

		// Token: 0x06005AD4 RID: 23252 RVA: 0x0007CFED File Offset: 0x0007B1ED
		private void LooterStringOnChanged(string obj)
		{
			this.RefreshOpenState();
		}

		// Token: 0x06005AD5 RID: 23253 RVA: 0x0007CFF5 File Offset: 0x0007B1F5
		private void RefreshOpenState()
		{
			if (this.m_animator)
			{
				this.m_animator.SetBool(InteractiveChest.m_isOpenKey, !string.IsNullOrEmpty(this.m_node.LooterString.Value));
			}
		}

		// Token: 0x04004F95 RID: 20373
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04004F96 RID: 20374
		private InteractiveGatheringNode m_node;

		// Token: 0x04004F97 RID: 20375
		private const string kIsOpenParameter = "IsOpen";

		// Token: 0x04004F98 RID: 20376
		private static int m_isOpenKey = -1;
	}
}
