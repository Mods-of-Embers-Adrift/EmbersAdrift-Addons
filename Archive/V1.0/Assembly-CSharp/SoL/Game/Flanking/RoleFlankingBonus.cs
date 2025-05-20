using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BF7 RID: 3063
	[Serializable]
	public class RoleFlankingBonus
	{
		// Token: 0x1700164F RID: 5711
		// (get) Token: 0x06005E75 RID: 24181 RVA: 0x0007F863 File Offset: 0x0007DA63
		private RoleFlankingBonus.RoleFlankingBonusData Front
		{
			get
			{
				return this.m_front;
			}
		}

		// Token: 0x17001650 RID: 5712
		// (get) Token: 0x06005E76 RID: 24182 RVA: 0x0007F86B File Offset: 0x0007DA6B
		private RoleFlankingBonus.RoleFlankingBonusData Sides
		{
			get
			{
				return this.m_sides;
			}
		}

		// Token: 0x17001651 RID: 5713
		// (get) Token: 0x06005E77 RID: 24183 RVA: 0x0007F873 File Offset: 0x0007DA73
		private RoleFlankingBonus.RoleFlankingBonusData Rear
		{
			get
			{
				return this.m_rear;
			}
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x001F69F8 File Offset: 0x001F4BF8
		private string GetBonusDescription()
		{
			if (!this.HasValidBonus())
			{
				return "No Valid Bonuses";
			}
			return string.Concat(new string[]
			{
				this.GetBonusDescriptionForEditor(FlankingPosition.Front, this.Front),
				"\n",
				this.GetBonusDescriptionForEditor(FlankingPosition.Sides, this.Sides),
				"\n",
				this.GetBonusDescriptionForEditor(FlankingPosition.Rear, this.Rear)
			});
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x001F6A60 File Offset: 0x001F4C60
		private string GetBonusDescriptionForEditor(FlankingPosition position, RoleFlankingBonus.RoleFlankingBonusData bonusData)
		{
			if (bonusData == null || !bonusData.IsValid)
			{
				return position.ToString() + " has NO bonus";
			}
			return string.Concat(new string[]
			{
				position.ToString(),
				" has +",
				bonusData.Value.ToString(),
				" to ",
				bonusData.Type.ToString()
			});
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x001F6AE4 File Offset: 0x001F4CE4
		private bool HasValidBonus()
		{
			return (this.Front != null && this.Front.IsValid) || (this.Sides != null && this.Sides.IsValid) || (this.Rear != null && this.Rear.IsValid);
		}

		// Token: 0x06005E7B RID: 24187 RVA: 0x001F6B34 File Offset: 0x001F4D34
		public bool TryGetRoleFlankingBonus(FlankingPosition position, out RoleFlankingBonusType type, out int value)
		{
			type = RoleFlankingBonusType.None;
			value = 0;
			RoleFlankingBonus.RoleFlankingBonusData roleFlankingBonusData = null;
			switch (position)
			{
			case FlankingPosition.Front:
				roleFlankingBonusData = this.Front;
				break;
			case FlankingPosition.Sides:
				roleFlankingBonusData = this.Sides;
				break;
			case FlankingPosition.Rear:
				roleFlankingBonusData = this.Rear;
				break;
			}
			if (roleFlankingBonusData != null && roleFlankingBonusData.IsValid)
			{
				type = roleFlankingBonusData.Type;
				value = roleFlankingBonusData.Value;
				return true;
			}
			return false;
		}

		// Token: 0x06005E7C RID: 24188 RVA: 0x001F6B94 File Offset: 0x001F4D94
		public void FillTooltip(ArchetypeTooltip tooltip)
		{
			if (this.HasValidBonus())
			{
				TooltipTextBlock effectsBlock = tooltip.EffectsBlock;
				effectsBlock.AppendLine(ZString.Format<string>("{0} Bonus:", StatType.Flanking.GetStatPanelDisplay()), 0);
				FlankingPositionExtensions.BonusArray[0] = (float)this.Front.Value;
				FlankingPositionExtensions.BonusArray[1] = (float)this.Sides.Value;
				FlankingPositionExtensions.BonusArray[2] = (float)this.Rear.Value;
				float min = Mathf.Min(FlankingPositionExtensions.BonusArray);
				float max = Mathf.Max(FlankingPositionExtensions.BonusArray);
				this.AddBonusToTooltip(effectsBlock, FlankingPosition.Front, this.Front, min, max);
				this.AddBonusToTooltip(effectsBlock, FlankingPosition.Sides, this.Sides, min, max);
				this.AddBonusToTooltip(effectsBlock, FlankingPosition.Rear, this.Rear, min, max);
			}
		}

		// Token: 0x06005E7D RID: 24189 RVA: 0x001F6C4C File Offset: 0x001F4E4C
		private void AddBonusToTooltip(TooltipTextBlock block, FlankingPosition position, RoleFlankingBonus.RoleFlankingBonusData bonusData, float min, float max)
		{
			if (block != null && bonusData != null && bonusData.IsValid)
			{
				string arg = (bonusData.Type == RoleFlankingBonusType.ReduceThreat) ? "-" : "+";
				string rotatedArrow = position.GetRotatedArrow((float)bonusData.Value, min, max);
				block.AppendLine(ZString.Format<string, string, int, string>("{0} {1}{2} {3}", rotatedArrow, arg, bonusData.Value, bonusData.Type.GetAbbreviation()), 0);
			}
		}

		// Token: 0x040051B7 RID: 20919
		[SerializeField]
		private RoleFlankingBonus.RoleFlankingBonusData m_front;

		// Token: 0x040051B8 RID: 20920
		[SerializeField]
		private RoleFlankingBonus.RoleFlankingBonusData m_sides;

		// Token: 0x040051B9 RID: 20921
		[SerializeField]
		private RoleFlankingBonus.RoleFlankingBonusData m_rear;

		// Token: 0x040051BA RID: 20922
		[SerializeField]
		private DummyClass m_flankingBonusDummy;

		// Token: 0x02000BF8 RID: 3064
		[Serializable]
		private class RoleFlankingBonusData
		{
			// Token: 0x17001652 RID: 5714
			// (get) Token: 0x06005E7F RID: 24191 RVA: 0x0007F87B File Offset: 0x0007DA7B
			public RoleFlankingBonusType Type
			{
				get
				{
					return this.m_type;
				}
			}

			// Token: 0x17001653 RID: 5715
			// (get) Token: 0x06005E80 RID: 24192 RVA: 0x0007F883 File Offset: 0x0007DA83
			public int Value
			{
				get
				{
					return this.m_value;
				}
			}

			// Token: 0x17001654 RID: 5716
			// (get) Token: 0x06005E81 RID: 24193 RVA: 0x0007F88B File Offset: 0x0007DA8B
			public bool IsValid
			{
				get
				{
					return this.m_type != RoleFlankingBonusType.None && this.m_value > 0;
				}
			}

			// Token: 0x040051BB RID: 20923
			[SerializeField]
			private RoleFlankingBonusType m_type;

			// Token: 0x040051BC RID: 20924
			[SerializeField]
			private int m_value;
		}
	}
}
