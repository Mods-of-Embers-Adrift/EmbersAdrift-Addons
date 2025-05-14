using System;
using Cysharp.Text;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C26 RID: 3110
	[Serializable]
	public class TargetingParams
	{
		// Token: 0x170016F7 RID: 5879
		// (get) Token: 0x06005FE8 RID: 24552 RVA: 0x000808EB File Offset: 0x0007EAEB
		private bool m_showExcludeSelf
		{
			get
			{
				return this.m_targetType.CanExcludeSelf();
			}
		}

		// Token: 0x170016F8 RID: 5880
		// (get) Token: 0x06005FE9 RID: 24553 RVA: 0x000808F8 File Offset: 0x0007EAF8
		private bool m_showAoeGroupOnly
		{
			get
			{
				return this.m_showAoeMaxTargets && this.m_targetType.IsFriendly();
			}
		}

		// Token: 0x170016F9 RID: 5881
		// (get) Token: 0x06005FEA RID: 24554 RVA: 0x0008090F File Offset: 0x0007EB0F
		private bool m_showAoeMaxTargets
		{
			get
			{
				return this.m_targetType.IsAOE() || this.m_targetType.IsChain();
			}
		}

		// Token: 0x170016FA RID: 5882
		// (get) Token: 0x06005FEB RID: 24555 RVA: 0x0008092B File Offset: 0x0007EB2B
		protected virtual bool m_showAoeRadius
		{
			get
			{
				return this.m_targetType.IsAOE();
			}
		}

		// Token: 0x170016FB RID: 5883
		// (get) Token: 0x06005FEC RID: 24556 RVA: 0x00080938 File Offset: 0x0007EB38
		protected virtual bool m_showAoeAngle
		{
			get
			{
				return this.m_targetType.CheckAngle();
			}
		}

		// Token: 0x170016FC RID: 5884
		// (get) Token: 0x06005FED RID: 24557 RVA: 0x00080945 File Offset: 0x0007EB45
		protected virtual bool m_showTargetDistanceAngle
		{
			get
			{
				return this.m_targetType.RequiresTarget();
			}
		}

		// Token: 0x170016FD RID: 5885
		// (get) Token: 0x06005FEE RID: 24558 RVA: 0x00080952 File Offset: 0x0007EB52
		private bool m_showConalTypes
		{
			get
			{
				return this.m_targetType == EffectTargetType.Defensive_AOE_Conal || this.m_targetType == EffectTargetType.Offensive_AOE_Conal;
			}
		}

		// Token: 0x170016FE RID: 5886
		// (get) Token: 0x06005FEF RID: 24559 RVA: 0x0008096A File Offset: 0x0007EB6A
		// (set) Token: 0x06005FF0 RID: 24560 RVA: 0x00080971 File Offset: 0x0007EB71
		public static TargetingParams.DistanceFailureReasons DistanceFailureReason
		{
			get
			{
				return TargetingParams.m_distanceFailureReason;
			}
			set
			{
				TargetingParams.m_distanceFailureReason = value;
			}
		}

		// Token: 0x06005FF1 RID: 24561 RVA: 0x00080979 File Offset: 0x0007EB79
		public virtual MinMaxFloatRange GetTargetDistance(GameEntity entity, IHandHeldItems handHeldItems)
		{
			return this.m_targetDistance;
		}

		// Token: 0x06005FF2 RID: 24562 RVA: 0x00080981 File Offset: 0x0007EB81
		public virtual float GetTargetAngle(GameEntity entity, IHandHeldItems handHeldItems)
		{
			return this.m_targetAngle;
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x00080989 File Offset: 0x0007EB89
		public virtual float GetAoeRadius(GameEntity entity, IHandHeldItems handHeldItems)
		{
			return this.m_aoeRadius;
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x00080991 File Offset: 0x0007EB91
		public virtual float GetAoeAngle(GameEntity entity, IHandHeldItems handHeldItems)
		{
			return this.m_aoeAngle;
		}

		// Token: 0x170016FF RID: 5887
		// (get) Token: 0x06005FF5 RID: 24565 RVA: 0x00080999 File Offset: 0x0007EB99
		public EffectTargetType TargetType
		{
			get
			{
				return this.m_targetType;
			}
		}

		// Token: 0x17001700 RID: 5888
		// (get) Token: 0x06005FF6 RID: 24566 RVA: 0x000809A1 File Offset: 0x0007EBA1
		public HealthStateFlags RequiredHealthState
		{
			get
			{
				return this.m_requiredHealthState;
			}
		}

		// Token: 0x17001701 RID: 5889
		// (get) Token: 0x06005FF7 RID: 24567 RVA: 0x000809A9 File Offset: 0x0007EBA9
		public ConalTypes ConalType
		{
			get
			{
				if (!this.m_showConalTypes)
				{
					return ConalTypes.None;
				}
				return this.m_conalType;
			}
		}

		// Token: 0x17001702 RID: 5890
		// (get) Token: 0x06005FF8 RID: 24568 RVA: 0x000809BB File Offset: 0x0007EBBB
		public bool ExcludeSelf
		{
			get
			{
				return this.m_targetType.CanExcludeSelf() && this.m_excludeSelf;
			}
		}

		// Token: 0x17001703 RID: 5891
		// (get) Token: 0x06005FF9 RID: 24569 RVA: 0x000809D2 File Offset: 0x0007EBD2
		public bool AoeGroupOnly
		{
			get
			{
				return this.m_aoeGroupOnly;
			}
		}

		// Token: 0x17001704 RID: 5892
		// (get) Token: 0x06005FFA RID: 24570 RVA: 0x000809DA File Offset: 0x0007EBDA
		public int AoeMaxTargets
		{
			get
			{
				return this.m_aoeMaxTargets;
			}
		}

		// Token: 0x17001705 RID: 5893
		// (get) Token: 0x06005FFB RID: 24571 RVA: 0x000809E2 File Offset: 0x0007EBE2
		public bool AoeRandomTargetSelection
		{
			get
			{
				return this.m_aoeRandomTargetSelection;
			}
		}

		// Token: 0x17001706 RID: 5894
		// (get) Token: 0x06005FFC RID: 24572 RVA: 0x000809EA File Offset: 0x0007EBEA
		public bool ShowOnTooltip
		{
			get
			{
				return !this.m_hideOnTooltip;
			}
		}

		// Token: 0x17001707 RID: 5895
		// (get) Token: 0x06005FFD RID: 24573 RVA: 0x000809F5 File Offset: 0x0007EBF5
		public bool HideOnTooltip
		{
			get
			{
				return this.m_hideOnTooltip;
			}
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x001FB680 File Offset: 0x001F9880
		public Vector3 GetTargetPosition(ITargetData targetData)
		{
			EffectTargetType targetType = this.m_targetType;
			if (targetType == EffectTargetType.Offensive_AOE_Ground || targetType == EffectTargetType.Defensive_AOE_Ground)
			{
				throw new NotImplementedException("AOE Ground not currently implemented!");
			}
			return this.GetInitialTarget(targetData).gameObject.transform.position;
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x001FB6C0 File Offset: 0x001F98C0
		public GameEntity GetInitialTarget(ITargetData targetData)
		{
			switch (this.m_targetType)
			{
			case EffectTargetType.Self:
			case EffectTargetType.Self_Offensive_AOE:
			case EffectTargetType.Self_Defensive_AOE:
			case EffectTargetType.Offensive_AOE_Conal:
			case EffectTargetType.Defensive_AOE_Conal:
				if (!targetData.Self)
				{
					throw new ArgumentException("Attempting to get Self target position but we have no Self target!");
				}
				return targetData.Self;
			case EffectTargetType.Offensive:
			case EffectTargetType.Offensive_Chain:
			case EffectTargetType.Offensive_AOE:
				if (!targetData.Offensive)
				{
					throw new ArgumentException("Attempting to get Offensive target position but we have no Offensive target!");
				}
				return targetData.Offensive;
			case EffectTargetType.Offensive_AOE_Ground:
			case EffectTargetType.Defensive_AOE_Ground:
				throw new NotImplementedException("AOE Ground does not return a target entity!");
			case EffectTargetType.Defensive:
			case EffectTargetType.Defensive_Chain:
			case EffectTargetType.Defensive_AOE:
				if (!targetData.Defensive)
				{
					throw new ArgumentException("Attempting to get Defensive target position but we have no Defensive target!");
				}
				return targetData.Defensive;
			}
			throw new ArgumentException("m_targetType");
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x001FB7B4 File Offset: 0x001F99B4
		public bool MeetsDistanceRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems)
		{
			return this.MeetsDistanceRequirementsInternal(source, target, handHeldItems, null, null);
		}

		// Token: 0x06006001 RID: 24577 RVA: 0x001FB7DC File Offset: 0x001F99DC
		public bool MeetsDistanceRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems, float distance)
		{
			return this.MeetsDistanceRequirementsInternal(source, target, handHeldItems, new float?(distance), null);
		}

		// Token: 0x06006002 RID: 24578 RVA: 0x000809FD File Offset: 0x0007EBFD
		private bool MeetsDistanceRequirementsInternal(GameEntity source, GameEntity target, IHandHeldItems handHeldItems, float? distance, float? executionProgress)
		{
			return !this.m_targetType.RequiresTarget() || DistanceAngleChecks.MeetsDistanceRequirements(source, target, this, handHeldItems, distance, executionProgress);
		}

		// Token: 0x06006003 RID: 24579 RVA: 0x001FB804 File Offset: 0x001F9A04
		public bool MeetsAngleRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems)
		{
			return this.MeetsAngleRequirementsInternal(source, target, handHeldItems, null, null);
		}

		// Token: 0x06006004 RID: 24580 RVA: 0x001FB82C File Offset: 0x001F9A2C
		public bool MeetsAngleRequirements(GameEntity source, GameEntity target, IHandHeldItems handHeldItems, float angle)
		{
			return this.MeetsAngleRequirementsInternal(source, target, handHeldItems, new float?(angle), null);
		}

		// Token: 0x06006005 RID: 24581 RVA: 0x00080A1B File Offset: 0x0007EC1B
		private bool MeetsAngleRequirementsInternal(GameEntity source, GameEntity target, IHandHeldItems handHeldItems, float? angle, float? executionProgress)
		{
			return !this.m_targetType.RequiresTarget() || DistanceAngleChecks.MeetsAngleRequirements(source, target, this, handHeldItems, angle, executionProgress);
		}

		// Token: 0x06006006 RID: 24582 RVA: 0x001FB854 File Offset: 0x001F9A54
		private void OnValidateValues()
		{
			this.m_targetDistance = new MinMaxFloatRange(Mathf.Clamp(this.m_targetDistance.Min, 0f, float.MaxValue), Mathf.Clamp(this.m_targetDistance.Max, 0f, float.MaxValue));
			this.m_targetAngle = Mathf.Clamp(this.m_targetAngle, 0f, 360f);
			this.m_aoeMaxTargets = Mathf.Clamp(this.m_aoeMaxTargets, 1, int.MaxValue);
			this.m_aoeRadius = Mathf.Clamp(this.m_aoeRadius, 0f, float.MaxValue);
			this.m_aoeAngle = Mathf.Clamp(this.m_aoeAngle, 0f, 360f);
		}

		// Token: 0x06006007 RID: 24583 RVA: 0x001FB908 File Offset: 0x001F9B08
		public bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (executionCache == null || !executionCache.SourceEntity)
			{
				return false;
			}
			GameEntity gameEntity = executionCache.TargetEntity;
			if (executionProgress <= 0f || executionCache.UseTargetAtExecutionComplete)
			{
				executionCache.UpdateOffensiveDefensiveTargets();
				gameEntity = this.TargetType.GetTarget(executionCache.SourceEntity);
			}
			if (executionCache.ForceTargetingCheck && this.TargetType.RequiresTarget() && this.TargetType.IsFriendly() && !this.ExcludeSelf && !gameEntity)
			{
				gameEntity = executionCache.SourceEntity;
			}
			if (!gameEntity || !gameEntity.NetworkEntity)
			{
				executionCache.TargetOverlay = TargetOverlayState.Invalid;
				executionCache.Message = "Invalid Target!";
				return false;
			}
			if (this.ExcludeSelf && gameEntity == executionCache.SourceEntity)
			{
				executionCache.TargetOverlay = TargetOverlayState.Invalid;
				executionCache.Message = "You must target someone else!";
				return false;
			}
			executionCache.SetTargetNetworkEntity(gameEntity.NetworkEntity);
			if (this.RequiredHealthState != HealthStateFlags.None && gameEntity.Vitals && !this.RequiredHealthState.MeetsRequirements(gameEntity.Vitals.GetCurrentHealthState()))
			{
				executionCache.TargetOverlay = TargetOverlayState.Invalid;
				executionCache.Message = "Invalid Target Health State!";
				return false;
			}
			if (!this.TargetType.RequiresTarget())
			{
				return true;
			}
			if (!DistanceAngleChecks.MeetsDistanceRequirements(executionCache.SourceEntity, gameEntity, this, executionCache, null, new float?(executionProgress)))
			{
				executionCache.TargetOverlay = TargetOverlayState.DistanceAngle;
				executionCache.Message = ((TargetingParams.m_distanceFailureReason == TargetingParams.DistanceFailureReasons.TooFar) ? "Target is too far!" : "Target is too close!");
				return false;
			}
			if (!this.MeetsAngleRequirementsInternal(executionCache.SourceEntity, gameEntity, executionCache, null, new float?(executionProgress)))
			{
				executionCache.TargetOverlay = TargetOverlayState.DistanceAngle;
				executionCache.Message = "Not facing target!";
				return false;
			}
			if (!GameManager.IsServer && (executionProgress <= 0f || executionProgress >= 1f) && !LineOfSight.PlayerHasLineOfSight(executionCache.SourceEntity, gameEntity))
			{
				executionCache.Message = "No line of sight!";
				return false;
			}
			return true;
		}

		// Token: 0x06006008 RID: 24584 RVA: 0x001FBAE8 File Offset: 0x001F9CE8
		public void FillTooltipTargetingBlock(ArchetypeTooltip tooltip, GameEntity entity)
		{
			if (!this.m_targetType.RequiresTarget())
			{
				return;
			}
			GameEntity target = this.m_targetType.GetTarget(entity);
			Color color = (target != null) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
			string left = this.ExcludeSelf ? ZString.Format<string, string, string>("<color={0}>{1} Target</color> {2}", color.ToHex(), this.m_targetType.GetOffensiveDefensiveDescription(), "<i><size=80%>(Excluding Self)</size></i>") : ZString.Format<string, string>("<color={0}>{1} Target</color>", color.ToHex(), this.m_targetType.GetOffensiveDefensiveDescription());
			string right = string.Empty;
			HealthStateFlags healthStateFlags = this.m_requiredHealthState;
			healthStateFlags &= ~HealthStateFlags.WakingUp;
			if (healthStateFlags != HealthStateFlags.None)
			{
				bool flag = target && target && target.VitalsReplicator && this.m_requiredHealthState.MeetsRequirements(target.VitalsReplicator.CurrentHealthState.Value);
				if (!flag || UIManager.TooltipShowMore)
				{
					color = (flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
					right = ZString.Format<string, string>("<color={0}>{1}</color>", color.ToHex(), healthStateFlags.ToStringWithSpaces());
				}
			}
			tooltip.RequirementsBlock.AppendLine(left, right);
		}

		// Token: 0x06006009 RID: 24585 RVA: 0x001FBC08 File Offset: 0x001F9E08
		public bool TryGetTargetDistanceAngleForTooltip(GameEntity entity, out string distance, out string angle)
		{
			distance = string.Empty;
			angle = string.Empty;
			GameEntity target = this.m_targetType.GetTarget(entity);
			entity.SetCurrentAlternateTarget(null);
			MinMaxFloatRange targetDistance = this.GetTargetDistance(entity, null);
			bool flag = this.MeetsDistanceRequirements(entity, target ? target : null, null);
			float targetAngle = this.GetTargetAngle(entity, null);
			bool flag2 = this.MeetsAngleRequirements(entity, (target == null) ? null : target, null);
			entity.SetCurrentAlternateTarget(null);
			bool flag3 = !flag || !flag2 || UIManager.TooltipShowMore;
			if (flag3)
			{
				Color color = flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				distance = targetDistance.GetRangeDisplay().Color(color);
			}
			if (flag3)
			{
				Color color2 = flag2 ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				angle = targetAngle.GetAngleDisplay().Color(color2);
			}
			return flag3;
		}

		// Token: 0x040052B9 RID: 21177
		public const string kExcludeSelfText = "<i><size=80%>(Excluding Self)</size></i>";

		// Token: 0x040052BA RID: 21178
		protected const string kTargetingGroupName = "Targeting";

		// Token: 0x040052BB RID: 21179
		protected const string kAoeGroupName = "Targeting/AoE";

		// Token: 0x040052BC RID: 21180
		private static TargetingParams.DistanceFailureReasons m_distanceFailureReason;

		// Token: 0x040052BD RID: 21181
		[SerializeField]
		protected EffectTargetType m_targetType = EffectTargetType.Offensive;

		// Token: 0x040052BE RID: 21182
		[SerializeField]
		private ConalTypes m_conalType = ConalTypes.Frontal;

		// Token: 0x040052BF RID: 21183
		[SerializeField]
		private HealthStateFlags m_requiredHealthState = HealthStateFlags.Alive;

		// Token: 0x040052C0 RID: 21184
		[SerializeField]
		private MinMaxFloatRange m_targetDistance = new MinMaxFloatRange(0f, 5f);

		// Token: 0x040052C1 RID: 21185
		[Range(0f, 360f)]
		[SerializeField]
		private float m_targetAngle = 10f;

		// Token: 0x040052C2 RID: 21186
		[SerializeField]
		private bool m_excludeSelf;

		// Token: 0x040052C3 RID: 21187
		[SerializeField]
		private bool m_hideOnTooltip;

		// Token: 0x040052C4 RID: 21188
		[SerializeField]
		private bool m_aoeGroupOnly;

		// Token: 0x040052C5 RID: 21189
		[SerializeField]
		private int m_aoeMaxTargets = 3;

		// Token: 0x040052C6 RID: 21190
		[SerializeField]
		private float m_aoeRadius = 3f;

		// Token: 0x040052C7 RID: 21191
		[Range(0f, 360f)]
		[SerializeField]
		private float m_aoeAngle = 10f;

		// Token: 0x040052C8 RID: 21192
		[Tooltip("Instead of sorting targets by distance, randomly select desired count from all within range.")]
		[SerializeField]
		private bool m_aoeRandomTargetSelection;

		// Token: 0x02000C27 RID: 3111
		public enum DistanceFailureReasons
		{
			// Token: 0x040052CA RID: 21194
			None,
			// Token: 0x040052CB RID: 21195
			TooFar,
			// Token: 0x040052CC RID: 21196
			TooClose
		}
	}
}
