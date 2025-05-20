using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE3 RID: 2787
	[CreateAssetMenu(menuName = "SoL/Profiles/Set Bonus")]
	public class SetBonusProfile : BaseArchetype, IEquatable<SetBonusProfile>
	{
		// Token: 0x060055EC RID: 21996 RVA: 0x001DFCAC File Offset: 0x001DDEAC
		public void ModifyPieceCount(int prevCount, int newCount, Dictionary<StatType, int> baseStats, Dictionary<StatType, int> combatStats)
		{
			if (prevCount == newCount)
			{
				return;
			}
			SetBonusProfile.SetBonus setBonus = this.GetSetBonus(prevCount);
			SetBonusProfile.SetBonus setBonus2 = this.GetSetBonus(newCount);
			if (setBonus != setBonus2)
			{
				SetBonusProfile.ModifyForSetBonus(setBonus, false, baseStats, combatStats);
				SetBonusProfile.ModifyForSetBonus(setBonus2, true, baseStats, combatStats);
			}
		}

		// Token: 0x060055ED RID: 21997 RVA: 0x001DFCE8 File Offset: 0x001DDEE8
		private SetBonusProfile.SetBonus GetSetBonus(int pieceCount)
		{
			if (this.m_bonuses != null)
			{
				for (int i = this.m_bonuses.Length - 1; i >= 0; i--)
				{
					if (pieceCount >= this.m_bonuses[i].PieceCount)
					{
						return this.m_bonuses[i];
					}
				}
			}
			return null;
		}

		// Token: 0x060055EE RID: 21998 RVA: 0x001DFD2C File Offset: 0x001DDF2C
		private static void ModifyForSetBonus(SetBonusProfile.SetBonus setBonus, bool adding, Dictionary<StatType, int> baseStats, Dictionary<StatType, int> combatStats)
		{
			if (setBonus != null && setBonus.StatModifiers != null)
			{
				for (int i = 0; i < setBonus.StatModifiers.Length; i++)
				{
					if (setBonus.StatModifiers[i] != null)
					{
						Dictionary<StatType, int> dictionary = setBonus.StatModifiers[i].StatType.IsCombatOnly() ? combatStats : baseStats;
						if (dictionary != null)
						{
							setBonus.StatModifiers[i].ModifyValue(dictionary, adding);
						}
					}
				}
			}
		}

		// Token: 0x060055EF RID: 21999 RVA: 0x001DFD8C File Offset: 0x001DDF8C
		public void FillTooltipBlocks(TooltipTextBlock block, IEquipable equipable, GameEntity entity)
		{
			if (!block || this.m_bonuses == null || this.m_bonuses.Length == 0)
			{
				return;
			}
			block.Title = (string.IsNullOrEmpty(this.DisplayName) ? "Set Bonus" : ZString.Format<string, string>("{0} {1}", this.DisplayName, "Set Bonus"));
			int activePieceCount = this.GetActivePieceCount(equipable, entity);
			int? num = null;
			if (activePieceCount > 0)
			{
				for (int i = this.m_bonuses.Length - 1; i >= 0; i--)
				{
					if (activePieceCount >= this.m_bonuses[i].PieceCount)
					{
						num = new int?(i);
						break;
					}
				}
			}
			for (int j = 0; j < this.m_bonuses.Length; j++)
			{
				bool flag = num != null && num.Value == j;
				Color? colorOverride = flag ? null : new Color?(UIManager.GrayColor);
				Color color = flag ? UIManager.BlueColor : UIManager.GrayColor;
				string arg = (this.m_bonuses[j].PieceCount == 1) ? "Piece" : "Pieces";
				block.AppendLine(ZString.Format<string, int, string>("<color={0}>{1} {2} Equipped</color>", color.ToHex(), this.m_bonuses[j].PieceCount, arg), 0);
				if (!flag)
				{
					block.Append(ZString.Format<string>("<size=75%><color={0}>", color.ToHex()), 0);
				}
				block.Append("<indent=5%>", 0);
				for (int k = 0; k < this.m_bonuses[j].StatModifiers.Length; k++)
				{
					this.m_bonuses[j].StatModifiers[k].AddToTooltipBlock(block, colorOverride);
				}
				block.Sb.Insert(block.Sb.Length - Environment.NewLine.Length, "</indent>");
				if (!flag)
				{
					block.Append("</size></color>", 0);
				}
			}
		}

		// Token: 0x060055F0 RID: 22000 RVA: 0x001DFF60 File Offset: 0x001DE160
		private int GetActivePieceCount(IEquipable thisEquipable, GameEntity entity)
		{
			int num = 0;
			if (thisEquipable != null && entity && entity.CollectionController != null && entity.CollectionController.Equipment != null)
			{
				if (thisEquipable is WeaponItem)
				{
					bool flag = !entity.CharacterData || !entity.CharacterData.MainHand_SecondaryActive;
					EquipmentSlot index = flag ? EquipmentSlot.PrimaryWeapon_MainHand : EquipmentSlot.SecondaryWeapon_MainHand;
					ArchetypeInstance archetypeInstance;
					entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out archetypeInstance);
					IEquipable equipable;
					SetBonusProfile setBonusProfile = (archetypeInstance != null && archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out equipable)) ? equipable.SetBonus : null;
					EquipmentSlot index2 = flag ? EquipmentSlot.PrimaryWeapon_OffHand : EquipmentSlot.SecondaryWeapon_OffHand;
					ArchetypeInstance archetypeInstance2;
					entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index2, out archetypeInstance2);
					IEquipable equipable2;
					SetBonusProfile setBonusProfile2 = (archetypeInstance2 != null && archetypeInstance2.Archetype && archetypeInstance2.Archetype.TryGetAsType(out equipable2)) ? equipable2.SetBonus : null;
					if (setBonusProfile && setBonusProfile2 && setBonusProfile == setBonusProfile2)
					{
						num = 2;
					}
				}
				else
				{
					foreach (ArchetypeInstance archetypeInstance3 in entity.CollectionController.Equipment.Instances)
					{
						IEquipable equipable3;
						if (archetypeInstance3 != null && archetypeInstance3.Index != 65536 && archetypeInstance3.Archetype && !(archetypeInstance3.Archetype is WeaponItem) && archetypeInstance3.Archetype.TryGetAsType(out equipable3) && equipable3.SetBonus && equipable3.SetBonus == this)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060055F1 RID: 22001 RVA: 0x001E012C File Offset: 0x001DE32C
		public bool HasStat(StatType statType, out int value)
		{
			value = 0;
			if (this.m_bonuses != null)
			{
				for (int i = 0; i < this.m_bonuses.Length; i++)
				{
					if (this.m_bonuses[i].StatModifiers != null)
					{
						for (int j = 0; j < this.m_bonuses[i].StatModifiers.Length; j++)
						{
							if (this.m_bonuses[i].StatModifiers[j].StatType == statType)
							{
								value += this.m_bonuses[i].StatModifiers[j].Value;
							}
						}
					}
				}
			}
			return value != 0;
		}

		// Token: 0x060055F2 RID: 22002 RVA: 0x001E01B8 File Offset: 0x001DE3B8
		public bool Equals(SetBonusProfile other)
		{
			return other != null && (this == other || base.Id.Value == other.Id.Value);
		}

		// Token: 0x060055F3 RID: 22003 RVA: 0x0007951A File Offset: 0x0007771A
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((SetBonusProfile)obj)));
		}

		// Token: 0x060055F4 RID: 22004 RVA: 0x001E01F4 File Offset: 0x001DE3F4
		public override int GetHashCode()
		{
			return base.Id.Value.GetHashCode();
		}

		// Token: 0x04004C31 RID: 19505
		private const string kSetBonusTitle = "Set Bonus";

		// Token: 0x04004C32 RID: 19506
		[SerializeField]
		private SetBonusProfile.SetBonus[] m_bonuses;

		// Token: 0x02000AE4 RID: 2788
		[Serializable]
		private class SetBonus
		{
			// Token: 0x04004C33 RID: 19507
			[Range(2f, 12f)]
			public int PieceCount = 2;

			// Token: 0x04004C34 RID: 19508
			public StatModifier[] StatModifiers;
		}
	}
}
