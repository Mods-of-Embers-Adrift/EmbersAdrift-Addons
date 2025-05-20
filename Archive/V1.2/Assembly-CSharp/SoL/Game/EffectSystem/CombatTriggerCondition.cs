using System;
using Cysharp.Text;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C0B RID: 3083
	[Serializable]
	public class CombatTriggerCondition
	{
		// Token: 0x06005EEA RID: 24298 RVA: 0x001F8148 File Offset: 0x001F6348
		public bool Trigger(EffectApplicationFlags flags = EffectApplicationFlags.None)
		{
			bool flag = true;
			if (flags != EffectApplicationFlags.None && this.m_flags != EffectApplicationFlags.None)
			{
				flag = ((flags & this.m_flags) > EffectApplicationFlags.None);
			}
			if (flag && this.m_chance < 100f)
			{
				flag = (UnityEngine.Random.Range(0f, 100f) <= this.m_chance);
			}
			return flag;
		}

		// Token: 0x17001685 RID: 5765
		// (get) Token: 0x06005EEB RID: 24299 RVA: 0x0007FE17 File Offset: 0x0007E017
		public bool HideOnTooltip
		{
			get
			{
				return this.m_hideOnTooltip;
			}
		}

		// Token: 0x17001686 RID: 5766
		// (get) Token: 0x06005EEC RID: 24300 RVA: 0x0007FE1F File Offset: 0x0007E01F
		public bool ShowOnTooltip
		{
			get
			{
				return !this.m_hideOnTooltip;
			}
		}

		// Token: 0x17001687 RID: 5767
		// (get) Token: 0x06005EED RID: 24301 RVA: 0x0007FE2A File Offset: 0x0007E02A
		public float Chance
		{
			get
			{
				return this.m_chance;
			}
		}

		// Token: 0x17001688 RID: 5768
		// (get) Token: 0x06005EEE RID: 24302 RVA: 0x0007FE32 File Offset: 0x0007E032
		public EffectApplicationFlags Flags
		{
			get
			{
				return this.m_flags;
			}
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x001F819C File Offset: 0x001F639C
		public bool TryGetChanceDescription(out string txt)
		{
			txt = string.Empty;
			if (this.HideOnTooltip)
			{
				return false;
			}
			if (this.m_chance >= 100f && this.m_flags == EffectApplicationFlags.None)
			{
				return false;
			}
			string chanceDescription = this.GetChanceDescription();
			string hitTypeDescription = this.GetHitTypeDescription();
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>");
				utf16ValueStringBuilder.Append(" ");
				utf16ValueStringBuilder.Append("<i><size=80%>");
				bool flag = false;
				if (!string.IsNullOrEmpty(chanceDescription))
				{
					utf16ValueStringBuilder.Append(chanceDescription);
					flag = true;
				}
				if (!string.IsNullOrEmpty(hitTypeDescription))
				{
					if (flag)
					{
						utf16ValueStringBuilder.Append(" ");
					}
					utf16ValueStringBuilder.AppendFormat<string>("On {0} Hit", hitTypeDescription);
				}
				utf16ValueStringBuilder.Append("</size></i>");
				txt = utf16ValueStringBuilder.ToString();
			}
			return true;
		}

		// Token: 0x06005EF0 RID: 24304 RVA: 0x0007FE3A File Offset: 0x0007E03A
		public string GetChanceDescription()
		{
			if (!this.ShowOnTooltip || this.m_chance >= 100f)
			{
				return string.Empty;
			}
			return ZString.Format<int>("{0}% chance", Mathf.FloorToInt(this.m_chance));
		}

		// Token: 0x06005EF1 RID: 24305 RVA: 0x001F8280 File Offset: 0x001F6480
		private string GetHitTypeDescription()
		{
			if (this.HideOnTooltip || this.m_flags == EffectApplicationFlags.None)
			{
				return string.Empty;
			}
			EffectApplicationFlags flags = this.m_flags;
			if (flags == (EffectApplicationFlags.Heavy | EffectApplicationFlags.Critical))
			{
				return "Heavy/Critical";
			}
			if (flags == EffectApplicationFlags.ValidHit || flags == EffectApplicationFlags.ValidHitAndOverTime)
			{
				return "successful";
			}
			return this.m_flags.ToStringWithSpaces().Replace(", ", "/");
		}

		// Token: 0x06005EF2 RID: 24306 RVA: 0x001F82E8 File Offset: 0x001F64E8
		public string GetHitTypeDescriptionIfNotValid()
		{
			string text = this.GetHitTypeDescription();
			if (text == "successful")
			{
				text = string.Empty;
			}
			return text;
		}

		// Token: 0x06005EF3 RID: 24307 RVA: 0x001F8310 File Offset: 0x001F6510
		public string GetTriggerDescription()
		{
			if (this.m_chance < 100f && this.m_flags != EffectApplicationFlags.None)
			{
				return this.GetChanceString() + " if " + this.GetFlagsString();
			}
			if (this.m_chance < 100f)
			{
				return this.GetChanceString();
			}
			if (this.m_flags != EffectApplicationFlags.None)
			{
				return this.GetFlagsString();
			}
			return string.Empty;
		}

		// Token: 0x06005EF4 RID: 24308 RVA: 0x001F8374 File Offset: 0x001F6574
		private string GetChanceString()
		{
			if (this.m_chance < 100f)
			{
				return Mathf.FloorToInt(this.m_chance).ToString() + "%";
			}
			return string.Empty;
		}

		// Token: 0x06005EF5 RID: 24309 RVA: 0x001F83B4 File Offset: 0x001F65B4
		private string GetFlagsString()
		{
			if (this.m_flags == EffectApplicationFlags.None)
			{
				return string.Empty;
			}
			string text = this.m_flags.ToString();
			if (text.Length > 20)
			{
				int num = 0;
				for (int i = 0; i < EffectApplicationFlagsExtensions.AllEffectApplicationFlags.Length; i++)
				{
					if (EffectApplicationFlagsExtensions.AllEffectApplicationFlags[i] != EffectApplicationFlags.None && this.m_flags.HasBitFlag(EffectApplicationFlagsExtensions.AllEffectApplicationFlags[i]))
					{
						num++;
					}
				}
				return text.Substring(0, 20) + "...(" + num.ToString() + ")";
			}
			return text;
		}

		// Token: 0x0400520B RID: 21003
		private const float kMinChance = 0f;

		// Token: 0x0400520C RID: 21004
		public const float kMaxChance = 100f;

		// Token: 0x0400520D RID: 21005
		[Range(0f, 100f)]
		[SerializeField]
		private float m_chance = 100f;

		// Token: 0x0400520E RID: 21006
		[SerializeField]
		private EffectApplicationFlags m_flags;

		// Token: 0x0400520F RID: 21007
		[SerializeField]
		private bool m_hideOnTooltip;

		// Token: 0x04005210 RID: 21008
		private const string kSuccessfulHit = "successful";
	}
}
