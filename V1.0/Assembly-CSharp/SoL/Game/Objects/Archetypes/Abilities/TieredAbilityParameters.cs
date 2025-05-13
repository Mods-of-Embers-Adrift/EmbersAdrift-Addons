using System;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF7 RID: 2807
	[Serializable]
	public class TieredAbilityParameters
	{
		// Token: 0x17001453 RID: 5203
		// (get) Token: 0x060056DC RID: 22236 RVA: 0x00079DB9 File Offset: 0x00077FB9
		public virtual ExecutionParams ExecutionParams
		{
			get
			{
				return this.m_executionParams;
			}
		}

		// Token: 0x17001454 RID: 5204
		// (get) Token: 0x060056DD RID: 22237 RVA: 0x00079DC1 File Offset: 0x00077FC1
		public virtual TargetingParams TargetingParams
		{
			get
			{
				return this.m_targetingParams;
			}
		}

		// Token: 0x17001455 RID: 5205
		// (get) Token: 0x060056DE RID: 22238 RVA: 0x00079DC9 File Offset: 0x00077FC9
		public KinematicParameters KinematicParams
		{
			get
			{
				return this.m_kinematicParams;
			}
		}

		// Token: 0x17001456 RID: 5206
		// (get) Token: 0x060056DF RID: 22239 RVA: 0x00079DD1 File Offset: 0x00077FD1
		public virtual CombatEffect CombatEffect
		{
			get
			{
				return this.m_combatEffect;
			}
		}

		// Token: 0x17001457 RID: 5207
		// (get) Token: 0x060056E0 RID: 22240 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual AbilityVFX Vfx
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001458 RID: 5208
		// (get) Token: 0x060056E1 RID: 22241 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual AbilityVFX VfxSecondary
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001459 RID: 5209
		// (get) Token: 0x060056E2 RID: 22242 RVA: 0x0004479C File Offset: 0x0004299C
		public virtual int LevelThreshold
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700145A RID: 5210
		// (get) Token: 0x060056E3 RID: 22243 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool ShowExecution
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700145B RID: 5211
		// (get) Token: 0x060056E4 RID: 22244 RVA: 0x00079DD9 File Offset: 0x00077FD9
		protected virtual bool ShowTargeting
		{
			get
			{
				return this.ShowTargetingParameters;
			}
		}

		// Token: 0x1700145C RID: 5212
		// (get) Token: 0x060056E5 RID: 22245 RVA: 0x00079DE1 File Offset: 0x00077FE1
		protected virtual bool ShowKinematic
		{
			get
			{
				return this.ShowKinematicParameters;
			}
		}

		// Token: 0x1700145D RID: 5213
		// (get) Token: 0x060056E6 RID: 22246 RVA: 0x00079DE9 File Offset: 0x00077FE9
		protected virtual bool ShowCombatEffect
		{
			get
			{
				return this.ShowCombatParameters;
			}
		}

		// Token: 0x060056E7 RID: 22247 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsOverridden(TieredAbilityParameterType type)
		{
			return false;
		}

		// Token: 0x060056E8 RID: 22248 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool HasAnyOverrides()
		{
			return false;
		}

		// Token: 0x04004C8F RID: 19599
		protected const int kExecutionOrder = 20;

		// Token: 0x04004C90 RID: 19600
		protected const int kTargetingOrder = 30;

		// Token: 0x04004C91 RID: 19601
		protected const int kKinematicOrder = 40;

		// Token: 0x04004C92 RID: 19602
		protected const int kEffectOrder = 50;

		// Token: 0x04004C93 RID: 19603
		[HideInInspector]
		public bool ShowTargetingParameters;

		// Token: 0x04004C94 RID: 19604
		[HideInInspector]
		public bool ShowKinematicParameters;

		// Token: 0x04004C95 RID: 19605
		[HideInInspector]
		public bool ShowCombatParameters;

		// Token: 0x04004C96 RID: 19606
		[SerializeField]
		protected ExecutionParams m_executionParams;

		// Token: 0x04004C97 RID: 19607
		[SerializeField]
		protected TargetingParamsSpatial m_targetingParams;

		// Token: 0x04004C98 RID: 19608
		[SerializeField]
		protected KinematicParameters m_kinematicParams;

		// Token: 0x04004C99 RID: 19609
		[SerializeField]
		protected CombatEffectWithSecondary m_combatEffect;
	}
}
