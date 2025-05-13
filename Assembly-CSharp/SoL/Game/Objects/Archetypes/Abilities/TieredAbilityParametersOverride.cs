using System;
using SoL.Game.EffectSystem;
using SoL.Game.EffectSystem.Scriptables;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF8 RID: 2808
	[Serializable]
	public class TieredAbilityParametersOverride : TieredAbilityParameters
	{
		// Token: 0x1700145E RID: 5214
		// (get) Token: 0x060056EA RID: 22250 RVA: 0x00079DF1 File Offset: 0x00077FF1
		// (set) Token: 0x060056EB RID: 22251 RVA: 0x00079DF9 File Offset: 0x00077FF9
		public string ParentGuid { get; set; }

		// Token: 0x1700145F RID: 5215
		// (get) Token: 0x060056EC RID: 22252 RVA: 0x00079E02 File Offset: 0x00078002
		public override ExecutionParams ExecutionParams
		{
			get
			{
				if (!this.m_scriptableExecutionParams)
				{
					return base.ExecutionParams;
				}
				return this.m_scriptableExecutionParams.Params;
			}
		}

		// Token: 0x17001460 RID: 5216
		// (get) Token: 0x060056ED RID: 22253 RVA: 0x00079E23 File Offset: 0x00078023
		public override TargetingParams TargetingParams
		{
			get
			{
				if (!this.m_scriptableTargetingParams)
				{
					return base.TargetingParams;
				}
				return this.m_scriptableTargetingParams.Params;
			}
		}

		// Token: 0x17001461 RID: 5217
		// (get) Token: 0x060056EE RID: 22254 RVA: 0x00079E44 File Offset: 0x00078044
		public override CombatEffect CombatEffect
		{
			get
			{
				if (!this.m_scriptableCombatEffectWithSecondary)
				{
					return base.CombatEffect;
				}
				return this.m_scriptableCombatEffectWithSecondary.Params;
			}
		}

		// Token: 0x17001462 RID: 5218
		// (get) Token: 0x060056EF RID: 22255 RVA: 0x00079E65 File Offset: 0x00078065
		public override AbilityVFX Vfx
		{
			get
			{
				if (!this.m_scriptableVfx)
				{
					return base.Vfx;
				}
				return this.m_scriptableVfx.VFX;
			}
		}

		// Token: 0x17001463 RID: 5219
		// (get) Token: 0x060056F0 RID: 22256 RVA: 0x00079E86 File Offset: 0x00078086
		public override AbilityVFX VfxSecondary
		{
			get
			{
				if (!this.m_scriptableVfxSecondary)
				{
					return base.VfxSecondary;
				}
				return this.m_scriptableVfxSecondary.VFX;
			}
		}

		// Token: 0x17001464 RID: 5220
		// (get) Token: 0x060056F1 RID: 22257 RVA: 0x00079EA7 File Offset: 0x000780A7
		public override int LevelThreshold
		{
			get
			{
				return this.m_levelThreshold;
			}
		}

		// Token: 0x17001465 RID: 5221
		// (get) Token: 0x060056F2 RID: 22258 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ShowExecution
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001466 RID: 5222
		// (get) Token: 0x060056F3 RID: 22259 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ShowTargeting
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001467 RID: 5223
		// (get) Token: 0x060056F4 RID: 22260 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ShowKinematic
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001468 RID: 5224
		// (get) Token: 0x060056F5 RID: 22261 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ShowCombatEffect
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060056F6 RID: 22262 RVA: 0x001E1870 File Offset: 0x001DFA70
		public override bool IsOverridden(TieredAbilityParameterType type)
		{
			switch (type)
			{
			case TieredAbilityParameterType.Execution:
				return this.m_scriptableExecutionParams != null;
			case TieredAbilityParameterType.Targeting:
				return this.m_scriptableTargetingParams != null;
			case TieredAbilityParameterType.Kinematic:
				return this.m_overrideKinematic;
			case TieredAbilityParameterType.Effect:
				return this.m_scriptableCombatEffectWithSecondary != null;
			case TieredAbilityParameterType.Vfx:
				return this.m_scriptableVfx != null;
			case TieredAbilityParameterType.VfxSecondary:
				return this.m_scriptableVfxSecondary != null;
			default:
				return base.IsOverridden(type);
			}
		}

		// Token: 0x060056F7 RID: 22263 RVA: 0x001E18F0 File Offset: 0x001DFAF0
		public override bool HasAnyOverrides()
		{
			return this.m_scriptableExecutionParams || this.m_scriptableTargetingParams || this.m_scriptableCombatEffectWithSecondary || this.m_scriptableVfx || this.m_scriptableVfxSecondary || base.HasAnyOverrides();
		}

		// Token: 0x04004C9B RID: 19611
		[Range(1f, 100f)]
		[SerializeField]
		private int m_levelThreshold = 1;

		// Token: 0x04004C9C RID: 19612
		[SerializeField]
		private ScriptableExecutionParams m_scriptableExecutionParams;

		// Token: 0x04004C9D RID: 19613
		[SerializeField]
		private ScriptableTargetingParams m_scriptableTargetingParams;

		// Token: 0x04004C9E RID: 19614
		[SerializeField]
		private ScriptableCombatEffectWithSecondary m_scriptableCombatEffectWithSecondary;

		// Token: 0x04004C9F RID: 19615
		[SerializeField]
		private AbilityVFXScriptable m_scriptableVfx;

		// Token: 0x04004CA0 RID: 19616
		[SerializeField]
		private AbilityVFXScriptable m_scriptableVfxSecondary;

		// Token: 0x04004CA1 RID: 19617
		[HideInInspector]
		[SerializeField]
		private bool m_overrideExecution;

		// Token: 0x04004CA2 RID: 19618
		[HideInInspector]
		[SerializeField]
		private bool m_overrideTargeting;

		// Token: 0x04004CA3 RID: 19619
		[HideInInspector]
		[SerializeField]
		private bool m_overrideKinematic;

		// Token: 0x04004CA4 RID: 19620
		[HideInInspector]
		[SerializeField]
		private bool m_overrideEffect;
	}
}
