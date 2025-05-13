using System;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F0 RID: 2544
	[Serializable]
	public struct DiceSet : IEquatable<DiceSet>
	{
		// Token: 0x17001114 RID: 4372
		// (get) Token: 0x06004D60 RID: 19808 RVA: 0x00074433 File Offset: 0x00072633
		public int NDice
		{
			get
			{
				return this.m_nDice;
			}
		}

		// Token: 0x17001115 RID: 4373
		// (get) Token: 0x06004D61 RID: 19809 RVA: 0x0007443B File Offset: 0x0007263B
		public int NSides
		{
			get
			{
				return this.m_nSides;
			}
		}

		// Token: 0x17001116 RID: 4374
		// (get) Token: 0x06004D62 RID: 19810 RVA: 0x00074443 File Offset: 0x00072643
		public int Modifier
		{
			get
			{
				return this.m_modifier;
			}
		}

		// Token: 0x06004D63 RID: 19811 RVA: 0x0007444B File Offset: 0x0007264B
		public DiceSet(int nDice, int nSides)
		{
			this.m_nDice = nDice;
			this.m_nSides = nSides;
			this.m_modifier = 0;
		}

		// Token: 0x06004D64 RID: 19812 RVA: 0x00074462 File Offset: 0x00072662
		public DiceSet(int nDice, int nSides, int modifier)
		{
			this.m_nDice = nDice;
			this.m_nSides = nSides;
			this.m_modifier = modifier;
		}

		// Token: 0x06004D65 RID: 19813 RVA: 0x001C0334 File Offset: 0x001BE534
		public int RollDice()
		{
			int num = 0;
			for (int i = 0; i < this.m_nDice; i++)
			{
				num += UnityEngine.Random.Range(1, this.m_nSides + 1);
			}
			return num + this.m_modifier;
		}

		// Token: 0x06004D66 RID: 19814 RVA: 0x001C0370 File Offset: 0x001BE570
		public override string ToString()
		{
			if (this.m_nDice != 0 || this.m_nSides != 0 || this.m_modifier == 0)
			{
				string arg = string.Empty;
				if (this.m_modifier > 0)
				{
					arg = ZString.Format<int>("+{0}", this.m_modifier);
				}
				else if (this.m_modifier < 0)
				{
					arg = this.m_modifier.ToString();
				}
				return ZString.Format<int, int, string>("{0}d{1}{2}", this.m_nDice, this.m_nSides, arg);
			}
			if (this.m_modifier <= 0)
			{
				return this.m_modifier.ToString();
			}
			return ZString.Format<int>("+{0}", this.m_modifier);
		}

		// Token: 0x06004D67 RID: 19815 RVA: 0x001C040C File Offset: 0x001BE60C
		public float GetAverageDps(int delay, out int minDamage, out int maxDamage, out float minDps, out float maxDps)
		{
			minDamage = this.GetMinValue();
			maxDamage = this.GetMaxValue();
			minDps = (float)minDamage / (float)delay;
			maxDps = (float)maxDamage / (float)delay;
			return ((float)(this.m_nSides + 1) / 2f * (float)this.m_nDice + (float)this.m_modifier) / (float)delay;
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x00074479 File Offset: 0x00072679
		public int GetMinValue()
		{
			return this.m_nDice + this.m_modifier;
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x00074488 File Offset: 0x00072688
		public int GetMaxValue()
		{
			return this.m_nDice * this.m_nSides + this.m_modifier;
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x0007449E File Offset: 0x0007269E
		public bool Equals(DiceSet other)
		{
			return this.m_nDice == other.m_nDice && this.m_nSides == other.m_nSides && this.m_modifier == other.m_modifier;
		}

		// Token: 0x06004D6B RID: 19819 RVA: 0x001C0460 File Offset: 0x001BE660
		public override bool Equals(object obj)
		{
			if (obj is DiceSet)
			{
				DiceSet other = (DiceSet)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004D6C RID: 19820 RVA: 0x000744CC File Offset: 0x000726CC
		public override int GetHashCode()
		{
			return (this.m_nDice * 397 ^ this.m_nSides) * 397 ^ this.m_modifier;
		}

		// Token: 0x06004D6D RID: 19821 RVA: 0x000744EE File Offset: 0x000726EE
		public static bool operator ==(DiceSet A, DiceSet B)
		{
			return A.Equals(B);
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x000744F8 File Offset: 0x000726F8
		public static bool operator !=(DiceSet A, DiceSet B)
		{
			return !(A == B);
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x00074504 File Offset: 0x00072704
		public bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.DiceCount <= 1 || assignerName == ComponentEffectAssignerName.DiceModifier;
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x001C0488 File Offset: 0x001BE688
		public bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.DiceCount)
			{
				this.m_nDice = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_nDice);
				return true;
			}
			if (assignerName == ComponentEffectAssignerName.DiceSides)
			{
				this.m_nSides = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_nSides);
				return true;
			}
			if (assignerName != ComponentEffectAssignerName.DiceModifier)
			{
				return false;
			}
			this.m_modifier = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_modifier);
			return true;
		}

		// Token: 0x04004717 RID: 18199
		private const string kGroupName = "Dice";

		// Token: 0x04004718 RID: 18200
		private const float kWidth = 0.3f;

		// Token: 0x04004719 RID: 18201
		private const int kLabelWidth = 45;

		// Token: 0x0400471A RID: 18202
		[SerializeField]
		private int m_nDice;

		// Token: 0x0400471B RID: 18203
		[SerializeField]
		private int m_nSides;

		// Token: 0x0400471C RID: 18204
		[SerializeField]
		private int m_modifier;
	}
}
