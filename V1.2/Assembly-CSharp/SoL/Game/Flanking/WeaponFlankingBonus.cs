using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BFB RID: 3067
	[Serializable]
	public class WeaponFlankingBonus
	{
		// Token: 0x17001655 RID: 5717
		// (get) Token: 0x06005E84 RID: 24196 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_hideInternal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001656 RID: 5718
		// (get) Token: 0x06005E85 RID: 24197 RVA: 0x0007F8BF File Offset: 0x0007DABF
		internal virtual WeaponFlankingBonus.WeaponFlankingBonusData Front
		{
			get
			{
				return this.m_front;
			}
		}

		// Token: 0x17001657 RID: 5719
		// (get) Token: 0x06005E86 RID: 24198 RVA: 0x0007F8C7 File Offset: 0x0007DAC7
		internal virtual WeaponFlankingBonus.WeaponFlankingBonusData Sides
		{
			get
			{
				return this.m_sides;
			}
		}

		// Token: 0x17001658 RID: 5720
		// (get) Token: 0x06005E87 RID: 24199 RVA: 0x0007F8CF File Offset: 0x0007DACF
		internal virtual WeaponFlankingBonus.WeaponFlankingBonusData Rear
		{
			get
			{
				return this.m_rear;
			}
		}

		// Token: 0x06005E88 RID: 24200 RVA: 0x001F6D1C File Offset: 0x001F4F1C
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

		// Token: 0x06005E89 RID: 24201 RVA: 0x001F6D84 File Offset: 0x001F4F84
		private string GetBonusDescriptionForEditor(FlankingPosition position, WeaponFlankingBonus.WeaponFlankingBonusData bonusData)
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
				bonusData.Type.ToString(),
				" + (",
				bonusData.FlankingMultiplier.ToString(),
				" * flanking)"
			});
		}

		// Token: 0x06005E8A RID: 24202 RVA: 0x001F6E28 File Offset: 0x001F5028
		private bool HasValidBonus()
		{
			return (this.Front != null && this.Front.IsValid) || (this.Sides != null && this.Sides.IsValid) || (this.Rear != null && this.Rear.IsValid);
		}

		// Token: 0x06005E8B RID: 24203 RVA: 0x001F6E78 File Offset: 0x001F5078
		public bool TryGetWeaponFlankingBonus(FlankingPosition position, out WeaponFlankingBonusType type, out int value, out float flankingMultiplier)
		{
			type = WeaponFlankingBonusType.None;
			value = 0;
			flankingMultiplier = 1f;
			WeaponFlankingBonus.WeaponFlankingBonusData weaponFlankingBonusData = null;
			switch (position)
			{
			case FlankingPosition.Front:
				weaponFlankingBonusData = this.Front;
				break;
			case FlankingPosition.Sides:
				weaponFlankingBonusData = this.Sides;
				break;
			case FlankingPosition.Rear:
				weaponFlankingBonusData = this.Rear;
				break;
			}
			if (weaponFlankingBonusData != null && weaponFlankingBonusData.IsValid)
			{
				type = weaponFlankingBonusData.Type;
				value = weaponFlankingBonusData.Value;
				flankingMultiplier = weaponFlankingBonusData.FlankingMultiplier;
				return true;
			}
			return false;
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x0007F8D7 File Offset: 0x0007DAD7
		public void GetBonusValues(out int front, out int sides, out int rear)
		{
			front = this.Front.Value;
			sides = this.Sides.Value;
			rear = this.Rear.Value;
		}

		// Token: 0x06005E8D RID: 24205 RVA: 0x001F6EEC File Offset: 0x001F50EC
		public void GetMultiplierValues(out float front, out float sides, out float rear)
		{
			front = ((this.Front.Value > 0) ? this.Front.FlankingMultiplier : 0f);
			sides = ((this.Sides.Value > 0) ? this.Sides.FlankingMultiplier : 0f);
			rear = ((this.Rear.Value > 0) ? this.Rear.FlankingMultiplier : 0f);
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x001F6F60 File Offset: 0x001F5160
		public void FillTooltip(ArchetypeTooltip tooltip)
		{
			if (this.HasValidBonus())
			{
				float num;
				float num2;
				float num3;
				this.GetMultiplierValues(out num, out num2, out num3);
				FlankingPositionExtensions.BonusArray[0] = num;
				FlankingPositionExtensions.BonusArray[1] = num2;
				FlankingPositionExtensions.BonusArray[2] = num3;
				float min = Mathf.Min(FlankingPositionExtensions.BonusArray);
				float max = Mathf.Max(FlankingPositionExtensions.BonusArray);
				int playerFlankingStat = (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.Vitals != null) ? LocalPlayer.GameEntity.Vitals.GetStatusEffectValueInCombat(StatType.Flanking) : 0;
				TooltipTextBlock combatBlock = tooltip.CombatBlock;
				combatBlock.AddSpacer(20);
				this.AddBonusToTooltip(combatBlock, FlankingPosition.Front, this.Front, playerFlankingStat, min, max);
				this.AddBonusToTooltip(combatBlock, FlankingPosition.Sides, this.Sides, playerFlankingStat, min, max);
				this.AddBonusToTooltip(combatBlock, FlankingPosition.Rear, this.Rear, playerFlankingStat, min, max);
			}
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x001F7038 File Offset: 0x001F5238
		private void AddBonusToTooltip(TooltipTextBlock block, FlankingPosition position, WeaponFlankingBonus.WeaponFlankingBonusData bonusData, int playerFlankingStat, float min, float max)
		{
			if (block != null && bonusData != null && bonusData.IsValid)
			{
				string rotatedArrow = position.GetRotatedArrow(bonusData.FlankingMultiplier, min, max);
				string text = ZString.Format<string, int, string, string, string, string>("{0} {1} {2} + {3}% <size=80%><color={4}>{5}</color></size>", rotatedArrow, bonusData.Value, bonusData.Type.GetAbbreviation(), bonusData.FlankingMultiplier.GetAsPercentage(), UIManager.BlueColor.ToHex(), StatType.Flanking.GetStatPanelDisplay());
				if (playerFlankingStat != 0)
				{
					float num = (float)playerFlankingStat * bonusData.FlankingMultiplier;
					string right = ZString.Format<string, float>("(<color={0}>{1:0.##}</color>)", UIManager.BlueColor.ToHex(), (float)bonusData.Value + num);
					block.AppendLine(text, right);
					return;
				}
				block.AppendLine(text, 0);
			}
		}

		// Token: 0x040051C2 RID: 20930
		[SerializeField]
		private WeaponFlankingBonus.WeaponFlankingBonusData m_front;

		// Token: 0x040051C3 RID: 20931
		[SerializeField]
		private WeaponFlankingBonus.WeaponFlankingBonusData m_sides;

		// Token: 0x040051C4 RID: 20932
		[SerializeField]
		private WeaponFlankingBonus.WeaponFlankingBonusData m_rear;

		// Token: 0x040051C5 RID: 20933
		[SerializeField]
		private DummyClass m_flankingBonusDummy;

		// Token: 0x02000BFC RID: 3068
		[Serializable]
		internal class WeaponFlankingBonusData
		{
			// Token: 0x17001659 RID: 5721
			// (get) Token: 0x06005E91 RID: 24209 RVA: 0x0007F900 File Offset: 0x0007DB00
			public WeaponFlankingBonusType Type
			{
				get
				{
					return this.m_type;
				}
			}

			// Token: 0x1700165A RID: 5722
			// (get) Token: 0x06005E92 RID: 24210 RVA: 0x0007F908 File Offset: 0x0007DB08
			public int Value
			{
				get
				{
					return this.m_value;
				}
			}

			// Token: 0x1700165B RID: 5723
			// (get) Token: 0x06005E93 RID: 24211 RVA: 0x0007F910 File Offset: 0x0007DB10
			public float FlankingMultiplier
			{
				get
				{
					return this.m_flankingMultiplier;
				}
			}

			// Token: 0x1700165C RID: 5724
			// (get) Token: 0x06005E94 RID: 24212 RVA: 0x0007F918 File Offset: 0x0007DB18
			public bool IsValid
			{
				get
				{
					return this.m_type != WeaponFlankingBonusType.None && this.m_value > 0;
				}
			}

			// Token: 0x040051C6 RID: 20934
			[SerializeField]
			private WeaponFlankingBonusType m_type;

			// Token: 0x040051C7 RID: 20935
			[SerializeField]
			private int m_value;

			// Token: 0x040051C8 RID: 20936
			[SerializeField]
			private float m_flankingMultiplier = 1f;
		}
	}
}
