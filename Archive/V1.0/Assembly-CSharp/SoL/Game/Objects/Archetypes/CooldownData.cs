using System;
using Cysharp.Text;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SoL.Game.EffectSystem;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A33 RID: 2611
	[BsonIgnoreExtraElements]
	[Serializable]
	public class CooldownData
	{
		// Token: 0x17001208 RID: 4616
		// (get) Token: 0x060050D4 RID: 20692 RVA: 0x00076083 File Offset: 0x00074283
		[BsonIgnore]
		[JsonIgnore]
		public bool Active
		{
			get
			{
				return this.m_elapsed != null;
			}
		}

		// Token: 0x17001209 RID: 4617
		// (get) Token: 0x060050D5 RID: 20693 RVA: 0x00076090 File Offset: 0x00074290
		// (set) Token: 0x060050D6 RID: 20694 RVA: 0x00076098 File Offset: 0x00074298
		[BsonIgnore]
		[JsonIgnore]
		public float? Cooldown
		{
			get
			{
				return this.m_cooldown;
			}
			set
			{
				this.m_cooldown = value;
			}
		}

		// Token: 0x1700120A RID: 4618
		// (get) Token: 0x060050D7 RID: 20695 RVA: 0x000760A1 File Offset: 0x000742A1
		// (set) Token: 0x060050D8 RID: 20696 RVA: 0x000760A9 File Offset: 0x000742A9
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Elapsed
		{
			get
			{
				return this.m_elapsed;
			}
			set
			{
				this.m_elapsed = value;
			}
		}

		// Token: 0x060050D9 RID: 20697 RVA: 0x001CDD44 File Offset: 0x001CBF44
		public void Reset()
		{
			this.Cooldown = null;
			this.Elapsed = null;
		}

		// Token: 0x060050DA RID: 20698 RVA: 0x000760B2 File Offset: 0x000742B2
		public void CopyFrom(CooldownData data)
		{
			this.m_cooldown = data.m_cooldown;
			this.m_elapsed = data.m_elapsed;
		}

		// Token: 0x060050DB RID: 20699 RVA: 0x001CDD70 File Offset: 0x001CBF70
		public bool UpdateElapsed(float deltaTime, AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel != AlchemyPowerLevel.None && this.Elapsed != null && this.Cooldown == null)
			{
				this.Cooldown = GlobalSettings.Values.Ashen.GetAlchemyCooldownTime(alchemyPowerLevel);
			}
			if (this.Cooldown != null && this.Elapsed != null)
			{
				this.Elapsed += deltaTime;
				if (this.Elapsed.Value >= this.Cooldown.Value)
				{
					this.Reset();
					return true;
				}
			}
			return false;
		}

		// Token: 0x060050DC RID: 20700 RVA: 0x001CDE30 File Offset: 0x001CC030
		public void AddAlchemyCooldownToTooltip(TooltipTextBlock block, AlchemyPowerLevel alchemyPowerLevel)
		{
			if (this.Active && this.Cooldown != null && this.Elapsed != null && block)
			{
				int value = Mathf.FloorToInt(this.Cooldown.Value - this.Elapsed.Value);
				string right = string.Empty;
				if (alchemyPowerLevel != AlchemyPowerLevel.None && alchemyPowerLevel - AlchemyPowerLevel.I <= 1)
				{
					right = ZString.Format<string, string, string>("<color={0}><b>{1}</b> {2} Cooldown</color>", UIManager.EmberColor.ToHex(), value.GetFormattedTime(true), alchemyPowerLevel.GetAlchemyPowerLevelDescription());
				}
				block.AppendLine(string.Empty, right);
			}
		}

		// Token: 0x04004866 RID: 18534
		private float? m_cooldown;

		// Token: 0x04004867 RID: 18535
		private float? m_elapsed;
	}
}
