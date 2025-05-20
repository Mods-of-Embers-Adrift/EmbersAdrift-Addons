using System;
using Cysharp.Text;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C22 RID: 3106
	[Serializable]
	public class ExecutionParams
	{
		// Token: 0x06005FC4 RID: 24516 RVA: 0x001FB21C File Offset: 0x001F941C
		private void ValidateNumbers()
		{
			this.m_executionTime = Mathf.Clamp(this.m_executionTime, 0f, float.MaxValue);
			this.m_cooldown = Mathf.Clamp(this.m_cooldown, 0, int.MaxValue);
			this.m_staminaCost = Mathf.Clamp(this.m_staminaCost, 0, 100);
			this.m_movementModifier = Mathf.Clamp(this.m_movementModifier, -1f, 0f);
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x00080760 File Offset: 0x0007E960
		public float GetMovementModifier()
		{
			if (!this.m_overrideMovementModifier)
			{
				return GlobalSettings.Values.Player.ExecutionMovementPenalty;
			}
			return this.m_movementModifier;
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x001FB28C File Offset: 0x001F948C
		public bool ExecutionCheck(ExecutionCache executionCache, bool initial)
		{
			if (executionCache == null)
			{
				Debug.LogError("ExecutionCheck: executionCache is null!");
				return false;
			}
			if (executionCache.SourceEntity == null || executionCache.SourceEntity.Vitals == null)
			{
				executionCache.Message = "Invalid SourceEntity!";
				return false;
			}
			if (!executionCache.SourceEntity.Vitals.Stance.CanExecute(this.m_validStances))
			{
				executionCache.Message = "Invalid stance!";
				return false;
			}
			if ((!GlobalSettings.Values.Player.StaminaDrainsDuringExecution || (initial && (float)this.m_staminaCost > 0f)) && executionCache.SourceEntity.Vitals.Stamina < (float)this.m_staminaCost)
			{
				executionCache.Message = "Not enough STA!";
				return false;
			}
			return true;
		}

		// Token: 0x170016E6 RID: 5862
		// (get) Token: 0x06005FC7 RID: 24519 RVA: 0x00080780 File Offset: 0x0007E980
		public StanceFlags ValidStances
		{
			get
			{
				return this.m_validStances;
			}
		}

		// Token: 0x170016E7 RID: 5863
		// (get) Token: 0x06005FC8 RID: 24520 RVA: 0x00080788 File Offset: 0x0007E988
		public AutoAttackStateChange AutoAttackState
		{
			get
			{
				return this.m_autoAttackStateChange;
			}
		}

		// Token: 0x170016E8 RID: 5864
		// (get) Token: 0x06005FC9 RID: 24521 RVA: 0x00080790 File Offset: 0x0007E990
		public bool IsInstant
		{
			get
			{
				return this.m_executionTime <= 0f;
			}
		}

		// Token: 0x170016E9 RID: 5865
		// (get) Token: 0x06005FCA RID: 24522 RVA: 0x000807A2 File Offset: 0x0007E9A2
		public float ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
		}

		// Token: 0x170016EA RID: 5866
		// (get) Token: 0x06005FCB RID: 24523 RVA: 0x000807AA File Offset: 0x0007E9AA
		public int Cooldown
		{
			get
			{
				return this.m_cooldown;
			}
		}

		// Token: 0x170016EB RID: 5867
		// (get) Token: 0x06005FCC RID: 24524 RVA: 0x000807B2 File Offset: 0x0007E9B2
		public int StaminaCost
		{
			get
			{
				return this.m_staminaCost;
			}
		}

		// Token: 0x170016EC RID: 5868
		// (get) Token: 0x06005FCD RID: 24525 RVA: 0x000807BA File Offset: 0x0007E9BA
		public float MovementModifier
		{
			get
			{
				return this.m_movementModifier;
			}
		}

		// Token: 0x170016ED RID: 5869
		// (get) Token: 0x06005FCE RID: 24526 RVA: 0x000807C2 File Offset: 0x0007E9C2
		public bool PreventRotation
		{
			get
			{
				return this.m_preventRotation;
			}
		}

		// Token: 0x170016EE RID: 5870
		// (get) Token: 0x06005FCF RID: 24527 RVA: 0x000807CA File Offset: 0x0007E9CA
		public bool Interruptable
		{
			get
			{
				return this.m_interruptable;
			}
		}

		// Token: 0x06005FD0 RID: 24528 RVA: 0x000807D2 File Offset: 0x0007E9D2
		public float GetInterruptProbability(float elapsedPercent)
		{
			if (!this.m_interruptable)
			{
				return 0f;
			}
			return this.m_interruptProbability.Evaluate(elapsedPercent);
		}

		// Token: 0x06005FD1 RID: 24529 RVA: 0x001FB34C File Offset: 0x001F954C
		public void FillTooltipDataBlock(ArchetypeTooltip tooltip)
		{
			if (this.m_autoAttackStateChange == AutoAttackStateChange.Disable)
			{
				tooltip.AddLineToLeftSubHeader("<i>Disables Auto Attack on execution</i>");
			}
			float movementModifier = this.GetMovementModifier();
			if (movementModifier != 0f)
			{
				tooltip.DataBlock.AppendLine("Movement", ZString.Format<string>("{0}%", movementModifier.GetAsPercentage()));
			}
		}

		// Token: 0x06005FD2 RID: 24530 RVA: 0x001FB39C File Offset: 0x001F959C
		public void FillTooltipRequirementsBlock(ArchetypeTooltip tooltip, bool hasEnoughStamina, GameEntity entity)
		{
			if (this.m_staminaCost > 0)
			{
				Color color = hasEnoughStamina ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				string str = "Stamina".Color(color).Indent(5);
				tooltip.RequirementsBlock.AppendLineAtStart("<sprite=\"SolIcons\" name=\"Circle\" tint=1>" + str, (this.m_staminaCost.ToString() + "%").Color(color));
			}
			if (this.m_validStances != StanceFlags.None)
			{
				bool flag = entity.Vitals.Stance.CanExecute(this.m_validStances);
				if (!flag || UIManager.TooltipShowMore)
				{
					Color color2 = flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
					string str2 = "Stance".Color(color2).Indent(5);
					tooltip.RequirementsBlock.AppendLine("<sprite=\"SolIcons\" name=\"Circle\" tint=1>" + str2, this.m_validStances.ToString().Color(color2));
				}
			}
		}

		// Token: 0x06005FD3 RID: 24531 RVA: 0x001FB480 File Offset: 0x001F9680
		public bool TryGetValidMovementTooltip(out string movementString)
		{
			movementString = string.Empty;
			float movementModifier = this.GetMovementModifier();
			if (movementModifier != 0f)
			{
				movementString = ZString.Format<string>("<b>{0}%</b> Movement", movementModifier.GetAsPercentage());
				return true;
			}
			return false;
		}

		// Token: 0x06005FD4 RID: 24532 RVA: 0x001FB4B8 File Offset: 0x001F96B8
		public bool TryGetValidStanceTooltip(GameEntity entity, out string stanceString)
		{
			stanceString = string.Empty;
			if (this.m_validStances != StanceFlags.None)
			{
				bool flag = entity.Vitals.Stance.CanExecute(this.m_validStances);
				if (!flag || UIManager.TooltipShowMore)
				{
					Color color = flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
					stanceString = ZString.Format<string, string>("<color={0}>{1} Stance</color>", color.ToHex(), this.m_validStances.GetStanceFlagTooltipDescription());
				}
			}
			return !string.IsNullOrEmpty(stanceString);
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x001FB52C File Offset: 0x001F972C
		public string GetStaminaCostTooltip(bool hasEnoughStamina)
		{
			if (this.m_staminaCost > 0)
			{
				Color color = hasEnoughStamina ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				return ZString.Format<string, int>("<color={0}><b>{1}%</b> Stamina</color>", color.ToHex(), this.m_staminaCost);
			}
			return string.Empty;
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x000807EE File Offset: 0x0007E9EE
		public void FillTooltip(ArchetypeTooltip tooltip)
		{
			if (this.m_autoAttackStateChange == AutoAttackStateChange.Disable)
			{
				tooltip.AddLineToLeftSubHeader("<i>Disables Auto Attack on execution</i>");
			}
		}

		// Token: 0x040052A1 RID: 21153
		private const string kExecutionGroupName = "Execution";

		// Token: 0x040052A2 RID: 21154
		private const string kMovementModGroupName = "Execution/MovementMod";

		// Token: 0x040052A3 RID: 21155
		private const string kInterruptGroupName = "Execution/Interrupt";

		// Token: 0x040052A4 RID: 21156
		[SerializeField]
		private StanceFlags m_validStances = StanceFlags.Idle;

		// Token: 0x040052A5 RID: 21157
		[SerializeField]
		private AutoAttackStateChange m_autoAttackStateChange;

		// Token: 0x040052A6 RID: 21158
		[SerializeField]
		private bool m_requireCombatStance;

		// Token: 0x040052A7 RID: 21159
		[SerializeField]
		private bool m_preventRotation;

		// Token: 0x040052A8 RID: 21160
		[SerializeField]
		private float m_executionTime = 1f;

		// Token: 0x040052A9 RID: 21161
		[SerializeField]
		private int m_cooldown = 10;

		// Token: 0x040052AA RID: 21162
		[Range(0f, 100f)]
		[SerializeField]
		private int m_staminaCost = 10;

		// Token: 0x040052AB RID: 21163
		[SerializeField]
		private bool m_overrideMovementModifier;

		// Token: 0x040052AC RID: 21164
		[Range(-1f, 0f)]
		[SerializeField]
		private float m_movementModifier;

		// Token: 0x040052AD RID: 21165
		[SerializeField]
		private bool m_interruptable;

		// Token: 0x040052AE RID: 21166
		[SerializeField]
		private AnimationCurve m_interruptProbability = AnimationCurve.Linear(0f, 0f, 1f, 0f);
	}
}
