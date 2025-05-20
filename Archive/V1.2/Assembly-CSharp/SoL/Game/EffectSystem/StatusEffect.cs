using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C14 RID: 3092
	[Serializable]
	public class StatusEffect
	{
		// Token: 0x06005F61 RID: 24417 RVA: 0x001F9514 File Offset: 0x001F7714
		public string GetTitleText()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append("Apply ");
				for (int i = 0; i < this.Values.Length; i++)
				{
					if (!this.Values[i].Type.IsInvalid())
					{
						utf16ValueStringBuilder.Append<StatType>(this.Values[i].Type);
						if (i < this.Values.Length - 1)
						{
							utf16ValueStringBuilder.Append(", ");
						}
					}
				}
				utf16ValueStringBuilder.Append(" Status Effect");
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x170016BB RID: 5819
		// (get) Token: 0x06005F62 RID: 24418 RVA: 0x0008039D File Offset: 0x0007E59D
		public StatusEffect.StatusEffectValue[] Values
		{
			get
			{
				return this.m_statusEffects;
			}
		}

		// Token: 0x06005F63 RID: 24419 RVA: 0x001F95C8 File Offset: 0x001F77C8
		private static string GetCombinedDisplayStrings(List<StatType> types)
		{
			if (types == null || types.Count <= 0)
			{
				return null;
			}
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			List<string> fromPool2 = StaticListPool<string>.GetFromPool();
			List<string> fromPool3 = StaticListPool<string>.GetFromPool();
			List<string> fromPool4 = StaticListPool<string>.GetFromPool();
			List<string> fromPool5 = StaticListPool<string>.GetFromPool();
			for (int i = 0; i < types.Count; i++)
			{
				bool flag = false;
				StatType statType = types[i];
				switch (statType)
				{
				case StatType.Damage1H:
					fromPool2.Add("1H");
					flag = true;
					break;
				case StatType.Damage2H:
					fromPool2.Add("2H");
					flag = true;
					break;
				case StatType.DamageRanged:
					fromPool2.Add("Ranged");
					flag = true;
					break;
				default:
					switch (statType)
					{
					case StatType.ResistDamagePhysical:
						fromPool3.Add("Physical");
						flag = true;
						break;
					case StatType.ResistDamageMental:
						fromPool3.Add("Mental");
						flag = true;
						break;
					case StatType.ResistDamageChemical:
						fromPool3.Add("Chemical");
						flag = true;
						break;
					case StatType.ResistDamageEmber:
						fromPool3.Add("Ember");
						flag = true;
						break;
					case (StatType)64:
					case (StatType)65:
					case (StatType)66:
					case (StatType)67:
					case (StatType)68:
					case (StatType)69:
						break;
					case StatType.ResistDebuffPhysical:
						fromPool4.Add("Physical");
						flag = true;
						break;
					case StatType.ResistDebuffMental:
						fromPool4.Add("Mental");
						flag = true;
						break;
					case StatType.ResistDebuffChemical:
						fromPool4.Add("Chemical");
						flag = true;
						break;
					case StatType.ResistDebuffEmber:
						fromPool4.Add("Ember");
						flag = true;
						break;
					case StatType.ResistDebuffMovement:
						fromPool4.Add("Movement");
						flag = true;
						break;
					default:
						switch (statType)
						{
						case StatType.ResistStun:
							fromPool5.Add("Stun");
							flag = true;
							break;
						case StatType.ResistFear:
							fromPool5.Add("Fear");
							flag = true;
							break;
						case StatType.ResistDaze:
							fromPool5.Add("Daze");
							flag = true;
							break;
						case StatType.ResistEnrage:
							fromPool5.Add("Enrage");
							flag = true;
							break;
						case StatType.ResistConfuse:
							fromPool5.Add("Confuse");
							flag = true;
							break;
						case StatType.ResistLull:
							fromPool5.Add("Lull");
							flag = true;
							break;
						}
						break;
					}
					break;
				}
				if (flag)
				{
					types.RemoveAt(i);
					i--;
				}
			}
			if (fromPool2.Count > 0)
			{
				fromPool.Add(ZString.Format<string, string>("{0} {1}", string.Join("/", fromPool2), "Dmg"));
			}
			if (fromPool3.Count > 0)
			{
				fromPool.Add(ZString.Format<string, string, string>("{0} {1} {2}", string.Join("/", fromPool3), "Dmg", "Resists"));
			}
			if (fromPool4.Count > 0)
			{
				fromPool.Add(ZString.Format<string, string, string>("{0} {1} {2}", string.Join("/", fromPool4), "Debuff", "Resists"));
			}
			if (fromPool5.Count > 0)
			{
				fromPool.Add(ZString.Format<string, string>("{0} {1}", string.Join("/", fromPool5), "Resists"));
			}
			for (int j = 0; j < types.Count; j++)
			{
				fromPool.Add(types[j].GetTooltipDisplay());
			}
			fromPool.Sort((string a, string b) => string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase));
			string result = string.Join(", ", fromPool);
			StaticListPool<string>.ReturnToPool(fromPool);
			StaticListPool<string>.ReturnToPool(fromPool2);
			StaticListPool<string>.ReturnToPool(fromPool3);
			StaticListPool<string>.ReturnToPool(fromPool4);
			StaticListPool<string>.ReturnToPool(fromPool5);
			return result;
		}

		// Token: 0x06005F64 RID: 24420 RVA: 0x001F992C File Offset: 0x001F7B2C
		public string GetDescription(bool positive, ReagentItem reagentItem, byte? stackCount)
		{
			this.CacheData();
			if (this.m_cachedDisplayData == null || this.m_cachedDisplayData.Count <= 0)
			{
				return string.Empty;
			}
			int num = reagentItem ? reagentItem.GetStatusEffectMod() : 0;
			string text = positive ? "+" : "-";
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				for (int i = 0; i < this.m_cachedDisplayData.Count; i++)
				{
					int num2 = this.m_cachedDisplayData[i].Value;
					if (stackCount != null)
					{
						num2 *= (int)stackCount.Value;
					}
					int num3 = num2 + num;
					if (num3 < 0)
					{
						text = string.Empty;
					}
					string arg = (num != 0) ? ZString.Format<string, string, int>("<b><color={0}>{1}{2}</b>", UIManager.ReagentBonusColor.ToHex(), text, num3) : ZString.Format<string, int>("<b>{0}{1}</b>", text, num3);
					string displayName = this.m_cachedDisplayData[i].DisplayName;
					string value = ZString.Format<string, string>("{0} {1}", arg, displayName);
					utf16ValueStringBuilder.Append(value);
					if (i != this.m_cachedDisplayData.Count - 1)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005F65 RID: 24421 RVA: 0x001F9A84 File Offset: 0x001F7C84
		private void CacheData()
		{
			if (this.m_cached && StatusEffect.StaticCacheDict != null)
			{
				return;
			}
			if (StatusEffect.StaticCacheDict == null)
			{
				StatusEffect.StaticCacheDict = new Dictionary<int, StatusEffect.EffectDisplayData>(10);
			}
			StatusEffect.StaticCacheDict.Clear();
			for (int i = 0; i < this.m_statusEffects.Length; i++)
			{
				if (!this.m_statusEffects[i].Type.IsInvalid())
				{
					StatusEffect.EffectDisplayData effectDisplayData;
					if (StatusEffect.StaticCacheDict.TryGetValue(this.m_statusEffects[i].Value, out effectDisplayData))
					{
						if (!effectDisplayData.Types.Contains(this.m_statusEffects[i].Type))
						{
							effectDisplayData.Types.Add(this.m_statusEffects[i].Type);
							StatusEffect.StaticCacheDict[this.m_statusEffects[i].Value] = effectDisplayData;
						}
					}
					else
					{
						List<StatType> fromPool = StaticListPool<StatType>.GetFromPool();
						fromPool.Add(this.m_statusEffects[i].Type);
						StatusEffect.StaticCacheDict.Add(this.m_statusEffects[i].Value, new StatusEffect.EffectDisplayData
						{
							Value = this.m_statusEffects[i].Value,
							Types = fromPool
						});
					}
				}
			}
			this.m_cachedDisplayData = new List<StatusEffect.EffectDisplayData>(10);
			foreach (KeyValuePair<int, StatusEffect.EffectDisplayData> keyValuePair in StatusEffect.StaticCacheDict)
			{
				StatusEffect.EffectDisplayData value = keyValuePair.Value;
				value.CacheDisplayName();
				this.m_cachedDisplayData.Add(value);
			}
			this.m_cachedDisplayData.Sort((StatusEffect.EffectDisplayData a, StatusEffect.EffectDisplayData b) => a.Value.CompareTo(b.Value));
			this.m_cached = true;
		}

		// Token: 0x04005265 RID: 21093
		[SerializeField]
		private StatusEffect.StatusEffectValue[] m_statusEffects;

		// Token: 0x04005266 RID: 21094
		[NonSerialized]
		private bool m_cached;

		// Token: 0x04005267 RID: 21095
		[NonSerialized]
		private List<StatusEffect.EffectDisplayData> m_cachedDisplayData;

		// Token: 0x04005268 RID: 21096
		private static Dictionary<int, StatusEffect.EffectDisplayData> StaticCacheDict;

		// Token: 0x02000C15 RID: 3093
		[Serializable]
		public class StatusEffectChannel
		{
			// Token: 0x06005F67 RID: 24423 RVA: 0x001F9C48 File Offset: 0x001F7E48
			public string GetChannelString()
			{
				if (!this.m_effectType.HasSubTypes())
				{
					return this.m_effectType.ToString();
				}
				return this.m_effectType.ToString() + " " + this.m_effectSubType.ToString();
			}

			// Token: 0x170016BC RID: 5820
			// (get) Token: 0x06005F68 RID: 24424 RVA: 0x000803A5 File Offset: 0x0007E5A5
			private bool m_showSubType
			{
				get
				{
					return this.m_effectType.HasSubTypes();
				}
			}

			// Token: 0x170016BD RID: 5821
			// (get) Token: 0x06005F69 RID: 24425 RVA: 0x000803B2 File Offset: 0x0007E5B2
			private StatusEffectSubType[] m_validSubTypes
			{
				get
				{
					return this.m_effectType.GetValidSubTypes();
				}
			}

			// Token: 0x170016BE RID: 5822
			// (get) Token: 0x06005F6A RID: 24426 RVA: 0x000803BF File Offset: 0x0007E5BF
			public StatusEffectType Channel
			{
				get
				{
					return this.m_effectType;
				}
			}

			// Token: 0x170016BF RID: 5823
			// (get) Token: 0x06005F6B RID: 24427 RVA: 0x000803C7 File Offset: 0x0007E5C7
			public StatusEffectSubType SubChannel
			{
				get
				{
					return this.m_effectSubType;
				}
			}

			// Token: 0x06005F6C RID: 24428 RVA: 0x000803CF File Offset: 0x0007E5CF
			private void ValidateSubType()
			{
				this.m_effectSubType = this.m_effectType.ValidateSubType(this.m_effectSubType);
			}

			// Token: 0x04005269 RID: 21097
			[SerializeField]
			private StatusEffectType m_effectType;

			// Token: 0x0400526A RID: 21098
			[SerializeField]
			private StatusEffectSubType m_effectSubType;
		}

		// Token: 0x02000C16 RID: 3094
		[Serializable]
		public class StatusEffectValue
		{
			// Token: 0x170016C0 RID: 5824
			// (get) Token: 0x06005F6E RID: 24430 RVA: 0x000803E8 File Offset: 0x0007E5E8
			public StatType Type
			{
				get
				{
					return this.m_type;
				}
			}

			// Token: 0x170016C1 RID: 5825
			// (get) Token: 0x06005F6F RID: 24431 RVA: 0x000803F0 File Offset: 0x0007E5F0
			public int Value
			{
				get
				{
					return this.m_value;
				}
			}

			// Token: 0x0400526B RID: 21099
			[SerializeField]
			private StatType m_type;

			// Token: 0x0400526C RID: 21100
			[SerializeField]
			private int m_value;
		}

		// Token: 0x02000C17 RID: 3095
		private struct EffectDisplayData
		{
			// Token: 0x06005F71 RID: 24433 RVA: 0x000803F8 File Offset: 0x0007E5F8
			public void CacheDisplayName()
			{
				this.DisplayName = StatusEffect.GetCombinedDisplayStrings(this.Types);
				StaticListPool<StatType>.ReturnToPool(this.Types);
				this.Types = null;
			}

			// Token: 0x0400526D RID: 21101
			public int Value;

			// Token: 0x0400526E RID: 21102
			public List<StatType> Types;

			// Token: 0x0400526F RID: 21103
			public string DisplayName;
		}
	}
}
