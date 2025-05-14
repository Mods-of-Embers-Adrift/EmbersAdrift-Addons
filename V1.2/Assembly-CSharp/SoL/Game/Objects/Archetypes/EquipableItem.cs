using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Pooling;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A70 RID: 2672
	public abstract class EquipableItem : ItemArchetype, IEquipable
	{
		// Token: 0x170012C5 RID: 4805
		// (get) Token: 0x060052AF RID: 21167
		public abstract EquipmentType Type { get; }

		// Token: 0x170012C6 RID: 4806
		// (get) Token: 0x060052B0 RID: 21168 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool AllowHandheldItems
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170012C7 RID: 4807
		// (get) Token: 0x060052B1 RID: 21169 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool CanBlockInternal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170012C8 RID: 4808
		// (get) Token: 0x060052B2 RID: 21170 RVA: 0x0007736A File Offset: 0x0007556A
		protected virtual MinMaxIntRange BlockValueInternal
		{
			get
			{
				return new MinMaxIntRange(0, 0);
			}
		}

		// Token: 0x170012C9 RID: 4809
		// (get) Token: 0x060052B3 RID: 21171 RVA: 0x00077373 File Offset: 0x00075573
		public override bool ExcludeFromInspection
		{
			get
			{
				return this.m_excludeFromInspect;
			}
		}

		// Token: 0x170012CA RID: 4810
		// (get) Token: 0x060052B4 RID: 21172 RVA: 0x0007737B File Offset: 0x0007557B
		private bool ShowHandheldSingle
		{
			get
			{
				return this.AllowHandheldItems && this.m_visualsType == EquipableItem.VisualsType.Single;
			}
		}

		// Token: 0x170012CB RID: 4811
		// (get) Token: 0x060052B5 RID: 21173 RVA: 0x00077390 File Offset: 0x00075590
		private bool ShowHandheldArray
		{
			get
			{
				return this.AllowHandheldItems && this.m_visualsType > EquipableItem.VisualsType.Single;
			}
		}

		// Token: 0x060052B6 RID: 21174 RVA: 0x001D47F4 File Offset: 0x001D29F4
		public PooledHandheldItem GetHandheldItem(byte? visualIndex)
		{
			if (!this.AllowHandheldItems)
			{
				return null;
			}
			EquipableItem.VisualsType visualsType = this.m_visualsType;
			if (visualsType - EquipableItem.VisualsType.Random > 1 || this.m_randomHandheldItems == null || this.m_randomHandheldItems.Length == 0)
			{
				return this.m_handheldItem;
			}
			if (visualIndex == null || (int)visualIndex.Value >= this.m_randomHandheldItems.Length)
			{
				return this.m_randomHandheldItems[0].Item;
			}
			return this.m_randomHandheldItems[(int)visualIndex.Value].Item;
		}

		// Token: 0x060052B7 RID: 21175 RVA: 0x001D486C File Offset: 0x001D2A6C
		public override string GetModifiedDisplayName(ArchetypeInstance instance)
		{
			if (!this.AllowHandheldItems || this.m_visualsType == EquipableItem.VisualsType.Single || this.m_randomHandheldItems == null || this.m_randomHandheldItems.Length == 0 || instance == null || instance.ItemData == null || instance.ItemData.VisualIndex == null)
			{
				return base.GetModifiedDisplayName(instance);
			}
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append(base.GetModifiedDisplayName(instance));
				EquipableItem.HandheldItemData handheldItemData = ((int)instance.ItemData.VisualIndex.Value < this.m_randomHandheldItems.Length) ? this.m_randomHandheldItems[(int)instance.ItemData.VisualIndex.Value] : this.m_randomHandheldItems[0];
				if (handheldItemData != null && !string.IsNullOrEmpty(handheldItemData.Description))
				{
					utf16ValueStringBuilder.AppendFormat<string>(" {0}", handheldItemData.Description);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060052B8 RID: 21176 RVA: 0x001D4968 File Offset: 0x001D2B68
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			if (this.AllowHandheldItems && this.m_visualsType == EquipableItem.VisualsType.Random && this.m_randomHandheldItems != null && this.m_randomHandheldItems.Length != 0)
			{
				instance.ItemData.VisualIndex = new byte?((byte)UnityEngine.Random.Range(0, this.m_randomHandheldItems.Length));
			}
		}

		// Token: 0x170012CC RID: 4812
		// (get) Token: 0x060052B9 RID: 21177 RVA: 0x000773A5 File Offset: 0x000755A5
		public int LevelRequirementLevel
		{
			get
			{
				if (this.m_roleLevelRequirement == null)
				{
					return 0;
				}
				return this.m_roleLevelRequirement.LevelRequirementLevel;
			}
		}

		// Token: 0x060052BA RID: 21178 RVA: 0x000773BC File Offset: 0x000755BC
		public void SetRoleLevelRequirement(RoleLevelRequirement roleLevelRequirement)
		{
			this.m_roleLevelRequirement = roleLevelRequirement;
		}

		// Token: 0x060052BB RID: 21179 RVA: 0x001D49C0 File Offset: 0x001D2BC0
		public override bool MatchesTextFilter(string filter)
		{
			if (this.Type.GetDisplayName().Contains(filter, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (this.m_roleLevelRequirement != null && this.m_roleLevelRequirement.MatchesNameFilter(filter))
			{
				return true;
			}
			if (this.m_additionalRoleLevelRequirements != null)
			{
				for (int i = 0; i < this.m_additionalRoleLevelRequirements.Length; i++)
				{
					if (this.m_additionalRoleLevelRequirements[i] != null && this.m_additionalRoleLevelRequirements[i].MatchesNameFilter(filter))
					{
						return true;
					}
				}
			}
			return base.MatchesTextFilter(filter);
		}

		// Token: 0x060052BC RID: 21180 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ResetSlotColor(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, bool refresh = true)
		{
		}

		// Token: 0x060052BD RID: 21181 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnEquipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, byte? colorIndex, bool refresh = true)
		{
		}

		// Token: 0x060052BE RID: 21182 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnUnequipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, byte? colorIndex, bool refresh = true)
		{
		}

		// Token: 0x060052BF RID: 21183 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnEquip()
		{
		}

		// Token: 0x060052C0 RID: 21184 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnUnequip()
		{
		}

		// Token: 0x060052C1 RID: 21185 RVA: 0x001D4A3C File Offset: 0x001D2C3C
		public virtual bool CanEquip(GameEntity entity)
		{
			if (this.m_roleLevelRequirement.MeetsAllRequirements(entity))
			{
				return true;
			}
			if (this.m_additionalRoleLevelRequirements != null && this.m_additionalRoleLevelRequirements.Length != 0)
			{
				for (int i = 0; i < this.m_additionalRoleLevelRequirements.Length; i++)
				{
					if (this.m_additionalRoleLevelRequirements[i].MeetsAllRequirements(entity))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060052C2 RID: 21186 RVA: 0x001D4A90 File Offset: 0x001D2C90
		public bool MeetsRoleRequirements(GameEntity entity)
		{
			if (this.m_roleLevelRequirement.MeetsRoleRequirements(entity))
			{
				return true;
			}
			if (this.m_additionalRoleLevelRequirements != null && this.m_additionalRoleLevelRequirements.Length != 0)
			{
				for (int i = 0; i < this.m_additionalRoleLevelRequirements.Length; i++)
				{
					if (this.m_additionalRoleLevelRequirements[i].MeetsRoleRequirements(entity))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060052C3 RID: 21187 RVA: 0x001D4AE4 File Offset: 0x001D2CE4
		public bool HasRequiredTrade(GameEntity entity)
		{
			if (this.m_roleLevelRequirement.HasRequiredTrade(entity))
			{
				return true;
			}
			if (this.m_additionalRoleLevelRequirements != null && this.m_additionalRoleLevelRequirements.Length != 0)
			{
				for (int i = 0; i < this.m_additionalRoleLevelRequirements.Length; i++)
				{
					if (this.m_additionalRoleLevelRequirements[i].HasRequiredTrade(entity))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060052C4 RID: 21188 RVA: 0x001D4B38 File Offset: 0x001D2D38
		public virtual EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			using (IEnumerator<EquipmentSlot> enumerator = this.Type.GetCachedCompatibleSlots().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return EquipmentSlot.None;
		}

		// Token: 0x170012CD RID: 4813
		// (get) Token: 0x060052C5 RID: 21189 RVA: 0x000773C5 File Offset: 0x000755C5
		public StatModifier[] StatModifiers
		{
			get
			{
				return this.m_statModifiers;
			}
		}

		// Token: 0x060052C6 RID: 21190 RVA: 0x001D4B88 File Offset: 0x001D2D88
		private SetBonusProfile GetSetBonusProfile()
		{
			if (this.m_zoneSpecificSetBonuses != null && this.m_zoneSpecificSetBonuses.Count > 0 && LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId != 0)
			{
				for (int i = 0; i < this.m_zoneSpecificSetBonuses.Count; i++)
				{
					if (this.m_zoneSpecificSetBonuses[i].ZoneId == (ZoneId)LocalZoneManager.ZoneRecord.ZoneId && this.m_zoneSpecificSetBonuses[i].Profile)
					{
						return this.m_zoneSpecificSetBonuses[i].Profile;
					}
				}
			}
			return this.m_setBonus;
		}

		// Token: 0x060052C7 RID: 21191 RVA: 0x001D4C24 File Offset: 0x001D2E24
		private bool TryGetOtherZoneSpecificProfiles(out string otherZones, out bool multiple)
		{
			multiple = false;
			otherZones = string.Empty;
			List<string> list = null;
			if (this.m_zoneSpecificSetBonuses != null && this.m_zoneSpecificSetBonuses.Count > 0 && LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId != 0)
			{
				for (int i = 0; i < this.m_zoneSpecificSetBonuses.Count; i++)
				{
					ZoneRecord zoneRecord;
					if (this.m_zoneSpecificSetBonuses[i].ZoneId != (ZoneId)LocalZoneManager.ZoneRecord.ZoneId && this.m_zoneSpecificSetBonuses[i].Profile && SessionData.TryGetZoneRecord(this.m_zoneSpecificSetBonuses[i].ZoneId, out zoneRecord))
					{
						if (list == null)
						{
							list = StaticListPool<string>.GetFromPool();
						}
						list.Add(zoneRecord.DisplayName);
					}
				}
			}
			if (list != null)
			{
				if (list.Count == 1)
				{
					multiple = false;
					otherZones = list[0];
				}
				else
				{
					multiple = true;
					otherZones = string.Join(", ", list);
				}
				StaticListPool<string>.ReturnToPool(list);
				return true;
			}
			return false;
		}

		// Token: 0x170012CE RID: 4814
		// (get) Token: 0x060052C8 RID: 21192 RVA: 0x000773CD File Offset: 0x000755CD
		SetBonusProfile IEquipable.SetBonus
		{
			get
			{
				return this.GetSetBonusProfile();
			}
		}

		// Token: 0x060052C9 RID: 21193 RVA: 0x001D4D1C File Offset: 0x001D2F1C
		public void OverrideSetBonus(ZoneId zoneId, SetBonusProfile profile)
		{
			if (zoneId == ZoneId.None)
			{
				this.m_setBonus = profile;
				return;
			}
			bool flag = false;
			if (this.m_zoneSpecificSetBonuses != null)
			{
				for (int i = 0; i < this.m_zoneSpecificSetBonuses.Count; i++)
				{
					if (this.m_zoneSpecificSetBonuses[i] != null && this.m_zoneSpecificSetBonuses[i].ZoneId == zoneId)
					{
						this.m_zoneSpecificSetBonuses[i].SetData(zoneId, profile);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				if (this.m_zoneSpecificSetBonuses == null)
				{
					this.m_zoneSpecificSetBonuses = new List<EquipableItem.ZoneSpecificSetBonus>(1);
				}
				EquipableItem.ZoneSpecificSetBonus zoneSpecificSetBonus = new EquipableItem.ZoneSpecificSetBonus();
				zoneSpecificSetBonus.SetData(zoneId, profile);
				this.m_zoneSpecificSetBonuses.Add(zoneSpecificSetBonus);
			}
		}

		// Token: 0x060052CA RID: 21194 RVA: 0x000773D5 File Offset: 0x000755D5
		protected virtual string GetEquipmentTypeDisplay()
		{
			return this.Type.GetDisplayName();
		}

		// Token: 0x060052CB RID: 21195 RVA: 0x001D4DC0 File Offset: 0x001D2FC0
		public sealed override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			bool flag = false;
			tooltip.DataBlock.AppendLineAtStart("Equipment Type", this.GetEquipmentTypeDisplay());
			if (this.m_statModifiers != null && this.m_statModifiers.Length != 0)
			{
				if (this.m_sortedStatModifiers == null)
				{
					this.m_sortedStatModifiers = new List<StatModifier>(this.m_statModifiers.Length);
					for (int i = 0; i < this.m_statModifiers.Length; i++)
					{
						if (this.m_statModifiers[i] != null)
						{
							this.m_sortedStatModifiers.Add(this.m_statModifiers[i]);
						}
					}
					this.m_sortedStatModifiers.Sort((StatModifier a, StatModifier b) => a.StatType.CompareTo(b.StatType));
				}
				if (EquipableItem.EquipmentStatTypes == null)
				{
					EquipableItem.EquipmentStatTypes = new Dictionary<StatType, int>(default(StatTypeComparer));
				}
				else
				{
					EquipableItem.EquipmentStatTypes.Clear();
				}
				for (int j = 0; j < this.m_sortedStatModifiers.Count; j++)
				{
					if (this.m_sortedStatModifiers[j] != null && !this.m_sortedStatModifiers[j].StatType.IsInvalid())
					{
						if (EquipableItem.EquipmentStatTypes.ContainsKey(this.m_sortedStatModifiers[j].StatType))
						{
							Dictionary<StatType, int> equipmentStatTypes = EquipableItem.EquipmentStatTypes;
							StatType statType = this.m_sortedStatModifiers[j].StatType;
							equipmentStatTypes[statType]++;
						}
						else
						{
							EquipableItem.EquipmentStatTypes.Add(this.m_sortedStatModifiers[j].StatType, 1);
							this.m_sortedStatModifiers[j].AddToTooltipBlock(tooltip.StatsBlock, null);
							if (this.m_sortedStatModifiers[j].StatType == StatType.Block)
							{
								this.AddBlockValueToTooltip(tooltip.StatsBlock);
								flag = true;
							}
						}
					}
				}
				foreach (KeyValuePair<StatType, int> keyValuePair in EquipableItem.EquipmentStatTypes)
				{
					if (keyValuePair.Value > 1)
					{
						tooltip.DataBlock.AppendLine(string.Concat(new string[]
						{
							"(x",
							keyValuePair.Value.ToString(),
							" ",
							keyValuePair.Key.GetTooltipDisplay(),
							")"
						}).Color(UIManager.RedColor), 0);
					}
				}
			}
			if (!flag)
			{
				this.AddBlockValueToTooltip(tooltip.StatsBlock);
			}
			SetBonusProfile setBonusProfile = this.GetSetBonusProfile();
			if (setBonusProfile)
			{
				setBonusProfile.FillTooltipBlocks(tooltip.SubEffectsBlock, this, entity);
				string str;
				bool flag2;
				if (this.TryGetOtherZoneSpecificProfiles(out str, out flag2))
				{
					tooltip.SubEffectsBlock.Append("<size=70%>(Has Specific for: " + str + ")</size>", 0);
				}
			}
			this.m_roleLevelRequirement.FillTooltipBlocks(tooltip, instance, entity);
		}

		// Token: 0x060052CC RID: 21196 RVA: 0x001D50B0 File Offset: 0x001D32B0
		private void AddBlockValueToTooltip(TooltipTextBlock block)
		{
			if (this.CanBlockInternal)
			{
				block.AppendLine(ZString.Format<string, string>("<color={0}>{1}</color> Block Value", UIManager.BlueColor.ToHex(), this.BlockValueInternal.ToString()), 0);
			}
		}

		// Token: 0x060052CD RID: 21197 RVA: 0x001D50F4 File Offset: 0x001D32F4
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			if (assignerName <= ComponentEffectAssignerName.Flanking)
			{
				if (assignerName <= ComponentEffectAssignerName.CombatMovement)
				{
					if (assignerName != ComponentEffectAssignerName.ProficiencyRequirement && assignerName - ComponentEffectAssignerName.Resilience > 4)
					{
						goto IL_52;
					}
				}
				else if (assignerName - ComponentEffectAssignerName.RegenHealth > 1 && assignerName - ComponentEffectAssignerName.Hit > 2)
				{
					goto IL_52;
				}
			}
			else if (assignerName <= ComponentEffectAssignerName.DebuffResistEmber)
			{
				if (assignerName != ComponentEffectAssignerName.Healing && assignerName - ComponentEffectAssignerName.StunStatusResist > 24)
				{
					goto IL_52;
				}
			}
			else if (assignerName - ComponentEffectAssignerName.AllDamage > 3 && assignerName - ComponentEffectAssignerName.AllDamageResist > 2)
			{
				goto IL_52;
			}
			return true;
			IL_52:
			return base.IsAssignerHandled(assignerName);
		}

		// Token: 0x060052CE RID: 21198 RVA: 0x001D515C File Offset: 0x001D335C
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			StatModifier[] statModifiers = this.m_statModifiers;
			int num = (statModifiers != null) ? statModifiers.Length : 0;
			int value2 = (int)Math.Round((double)value);
			switch (assignerName)
			{
			case ComponentEffectAssignerName.ProficiencyRequirement:
				return true;
			case ComponentEffectAssignerName.ArmorCostMultiplier:
			case (ComponentEffectAssignerName)29:
			case (ComponentEffectAssignerName)30:
			case (ComponentEffectAssignerName)31:
			case (ComponentEffectAssignerName)32:
			case (ComponentEffectAssignerName)33:
			case (ComponentEffectAssignerName)34:
			case (ComponentEffectAssignerName)35:
			case (ComponentEffectAssignerName)36:
			case (ComponentEffectAssignerName)37:
			case (ComponentEffectAssignerName)38:
			case (ComponentEffectAssignerName)39:
			case ComponentEffectAssignerName.MaxDuration:
			case ComponentEffectAssignerName.DiceModifier:
			case ComponentEffectAssignerName.ArmorWeightOverride:
			case ComponentEffectAssignerName.ArmorWeightInterpolator:
				break;
			case ComponentEffectAssignerName.Resilience:
				this.AddStatModifierOrUpdateValue(value2, StatType.Resilience, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.SafeFall:
				this.AddStatModifierOrUpdateValue(value2, StatType.SafeFall, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Haste:
				this.AddStatModifierOrUpdateValue(value2, StatType.Haste, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Movement:
				this.AddStatModifierOrUpdateValue(value2, StatType.Movement, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.CombatMovement:
				this.AddStatModifierOrUpdateValue(value2, StatType.CombatMovement, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.RegenHealth:
				this.AddStatModifierOrUpdateValue(value2, StatType.RegenHealth, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.RegenStamina:
				this.AddStatModifierOrUpdateValue(value2, StatType.RegenStamina, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Hit:
				this.AddStatModifierOrUpdateValue(value2, StatType.Hit, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Penetration:
				this.AddStatModifierOrUpdateValue(value2, StatType.Penetration, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Flanking:
				this.AddStatModifierOrUpdateValue(value2, StatType.Flanking, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Damage1H:
				this.AddStatModifierOrUpdateValue(value2, StatType.Damage1H, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Damage2H:
				this.AddStatModifierOrUpdateValue(value2, StatType.Damage2H, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.DamageRanged:
				this.AddStatModifierOrUpdateValue(value2, StatType.DamageRanged, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.DamageMental:
				this.AddStatModifierOrUpdateValue(value2, StatType.DamageMental, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.DamageChemical:
				this.AddStatModifierOrUpdateValue(value2, StatType.DamageChemical, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.DamageEmber:
				this.AddStatModifierOrUpdateValue(value2, StatType.DamageEmber, type, assignerName, rangeOverride);
				return true;
			case ComponentEffectAssignerName.Healing:
				this.AddStatModifierOrUpdateValue(value2, StatType.Healing, type, assignerName, rangeOverride);
				return true;
			default:
				switch (assignerName)
				{
				case ComponentEffectAssignerName.StunStatusResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistStun, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.FearStatusResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistFear, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DazeStatusResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDaze, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.EnrageStatusResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistEnrage, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ConfuseStatusResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistConfuse, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.LullStatusResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistLull, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDamagePhysical:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamagePhysical, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDamageMental:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamageMental, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDamageChemical:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamageChemical, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDamageEmber:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamageEmber, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDebuffPhysical:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffPhysical, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDebuffMental:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffMental, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDebuffChemical:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffChemical, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDebuffEmber:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffEmber, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.ResistDebuffMovement:
					this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffMovement, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.Avoidance:
					this.AddStatModifierOrUpdateValue(value2, StatType.Avoid, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.Block:
					this.AddStatModifierOrUpdateValue(value2, StatType.Block, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.Parry:
					this.AddStatModifierOrUpdateValue(value2, StatType.Parry, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.Riposte:
					this.AddStatModifierOrUpdateValue(value2, StatType.Riposte, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DamageResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.DamageResist, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DamageResistEmber:
					this.AddStatModifierOrUpdateValue(value2, StatType.DamageResistEmber, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DamageReduction:
					this.AddStatModifierOrUpdateValue(value2, StatType.DamageReduction, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DamageReductionEmber:
					this.AddStatModifierOrUpdateValue(value2, StatType.DamageReductionEmber, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DebuffResist:
					this.AddStatModifierOrUpdateValue(value2, StatType.DebuffResist, type, assignerName, rangeOverride);
					return true;
				case ComponentEffectAssignerName.DebuffResistEmber:
					this.AddStatModifierOrUpdateValue(value2, StatType.DebuffResistEmber, type, assignerName, rangeOverride);
					return true;
				default:
					switch (assignerName)
					{
					case ComponentEffectAssignerName.AllDamage:
					{
						StatModifier[] array = new StatModifier[num + 6];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array, num);
						}
						this.m_statModifiers = array;
						this.AddStatModifierOrUpdateValue(value2, StatType.Damage1H, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.Damage2H, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageRanged, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageMental, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageChemical, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageEmber, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllPhysicalDamage:
					{
						StatModifier[] array2 = new StatModifier[num + 3];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array2, num);
						}
						this.m_statModifiers = array2;
						this.AddStatModifierOrUpdateValue(value2, StatType.Damage1H, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.Damage2H, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageRanged, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllNonPhysicalDamage:
					{
						StatModifier[] array3 = new StatModifier[num + 3];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array3, num);
						}
						this.m_statModifiers = array3;
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageMental, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageChemical, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.DamageEmber, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllMeleeDamage:
					{
						StatModifier[] array4 = new StatModifier[num + 2];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array4, num);
						}
						this.m_statModifiers = array4;
						this.AddStatModifierOrUpdateValue(value2, StatType.Damage1H, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.Damage2H, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllActiveDefense:
					{
						StatModifier[] array5 = new StatModifier[num + 4];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array5, num);
						}
						this.m_statModifiers = array5;
						this.AddStatModifierOrUpdateValue(value2, StatType.Avoid, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.Block, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.Parry, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.Riposte, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllDamageResist:
					{
						StatModifier[] array6 = new StatModifier[num + 4];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array6, num);
						}
						this.m_statModifiers = array6;
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamagePhysical, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamageMental, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamageChemical, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDamageEmber, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllDebuffResist:
					{
						StatModifier[] array7 = new StatModifier[num + 5];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array7, num);
						}
						this.m_statModifiers = array7;
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffPhysical, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffMental, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffChemical, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffEmber, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDebuffMovement, type, assignerName, rangeOverride);
						return true;
					}
					case ComponentEffectAssignerName.AllStatusResist:
					{
						StatModifier[] array8 = new StatModifier[num + 6];
						if (this.m_statModifiers != null)
						{
							Array.Copy(this.m_statModifiers, array8, num);
						}
						this.m_statModifiers = array8;
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistStun, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistFear, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistDaze, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistEnrage, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistConfuse, type, assignerName, rangeOverride);
						this.AddStatModifierOrUpdateValue(value2, StatType.ResistLull, type, assignerName, rangeOverride);
						return true;
					}
					}
					break;
				}
				break;
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x060052CF RID: 21199 RVA: 0x001D582C File Offset: 0x001D3A2C
		private void AddStatModifierOrUpdateValue(int value, StatType type, ComponentEffectOutputType outputType = ComponentEffectOutputType.Assign, ComponentEffectAssignerName assignerName = ComponentEffectAssignerName.None, MinMaxFloatRange? rangeOverride = null)
		{
			StatModifier[] statModifiers = this.m_statModifiers;
			int num = (statModifiers != null) ? statModifiers.Length : 0;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if (this.m_statModifiers[i] != null)
				{
					num2++;
					if (this.m_statModifiers[i].StatType == type)
					{
						this.m_statModifiers[i] = new StatModifier(type, ComponentEffectAssigners.Apply(assignerName, (float)value, outputType, rangeOverride, this.m_statModifiers[i].Value));
						flag = true;
					}
				}
			}
			if (this.m_statModifiers == null || num2 == this.m_statModifiers.Length)
			{
				StatModifier[] array = new StatModifier[num + 1];
				Array.Copy(this.m_statModifiers, array, num2);
				this.m_statModifiers = array;
			}
			if (!flag)
			{
				this.m_statModifiers[num2] = new StatModifier(type, ComponentEffectAssigners.Apply(assignerName, (float)value, outputType, rangeOverride, 0));
			}
		}

		// Token: 0x040049E0 RID: 18912
		protected const string kVisualsGroup = "Visuals";

		// Token: 0x040049E1 RID: 18913
		[SerializeField]
		private StatModifier[] m_statModifiers;

		// Token: 0x040049E2 RID: 18914
		[NonSerialized]
		private List<StatModifier> m_sortedStatModifiers;

		// Token: 0x040049E3 RID: 18915
		private const string kSetBonusGroup = "Set Bonus";

		// Token: 0x040049E4 RID: 18916
		[SerializeField]
		private SetBonusProfile m_setBonus;

		// Token: 0x040049E5 RID: 18917
		[SerializeField]
		private List<EquipableItem.ZoneSpecificSetBonus> m_zoneSpecificSetBonuses;

		// Token: 0x040049E6 RID: 18918
		[SerializeField]
		private bool m_excludeFromInspect;

		// Token: 0x040049E7 RID: 18919
		[SerializeField]
		protected bool m_useRandomVisuals;

		// Token: 0x040049E8 RID: 18920
		[Tooltip("'Single' does not use any visual index. 'Random' generates the visual index when the item is created. 'Index' is for assigning the index via crafting.")]
		[SerializeField]
		protected EquipableItem.VisualsType m_visualsType;

		// Token: 0x040049E9 RID: 18921
		[SerializeField]
		private PooledHandheldItem m_handheldItem;

		// Token: 0x040049EA RID: 18922
		[SerializeField]
		private EquipableItem.HandheldItemData[] m_randomHandheldItems;

		// Token: 0x040049EB RID: 18923
		private const string kRequirements = "Requirements";

		// Token: 0x040049EC RID: 18924
		[SerializeField]
		private RoleLevelRequirement m_roleLevelRequirement;

		// Token: 0x040049ED RID: 18925
		[SerializeField]
		private RoleLevelRequirement[] m_additionalRoleLevelRequirements;

		// Token: 0x040049EE RID: 18926
		private static Dictionary<StatType, int> EquipmentStatTypes;

		// Token: 0x02000A71 RID: 2673
		protected enum VisualsType
		{
			// Token: 0x040049F0 RID: 18928
			Single,
			// Token: 0x040049F1 RID: 18929
			Random,
			// Token: 0x040049F2 RID: 18930
			Index
		}

		// Token: 0x02000A72 RID: 2674
		[Serializable]
		private class ZoneSpecificSetBonus
		{
			// Token: 0x170012CF RID: 4815
			// (get) Token: 0x060052D1 RID: 21201 RVA: 0x000773EA File Offset: 0x000755EA
			public ZoneId ZoneId
			{
				get
				{
					return this.m_zoneId;
				}
			}

			// Token: 0x170012D0 RID: 4816
			// (get) Token: 0x060052D2 RID: 21202 RVA: 0x000773F2 File Offset: 0x000755F2
			public SetBonusProfile Profile
			{
				get
				{
					return this.m_profile;
				}
			}

			// Token: 0x060052D3 RID: 21203 RVA: 0x000773FA File Offset: 0x000755FA
			public void SetData(ZoneId zoneId, SetBonusProfile profile)
			{
				this.m_zoneId = zoneId;
				this.m_profile = profile;
			}

			// Token: 0x040049F3 RID: 18931
			[SerializeField]
			private ZoneId m_zoneId;

			// Token: 0x040049F4 RID: 18932
			[SerializeField]
			private SetBonusProfile m_profile;
		}

		// Token: 0x02000A73 RID: 2675
		[Serializable]
		private class HandheldItemData
		{
			// Token: 0x040049F5 RID: 18933
			public PooledHandheldItem Item;

			// Token: 0x040049F6 RID: 18934
			public string Description;
		}
	}
}
