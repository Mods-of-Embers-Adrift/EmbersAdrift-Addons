using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SoL.Game.Crafting;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A56 RID: 2646
	[Serializable]
	public class ComponentEffectOperation
	{
		// Token: 0x17001273 RID: 4723
		// (get) Token: 0x060051F3 RID: 20979 RVA: 0x00076B42 File Offset: 0x00074D42
		public ItemAttributes.Names Attribute
		{
			get
			{
				return this.m_attribute;
			}
		}

		// Token: 0x17001274 RID: 4724
		// (get) Token: 0x060051F4 RID: 20980 RVA: 0x00076B4A File Offset: 0x00074D4A
		private bool m_type_value
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Value;
			}
		}

		// Token: 0x17001275 RID: 4725
		// (get) Token: 0x060051F5 RID: 20981 RVA: 0x00076B55 File Offset: 0x00074D55
		private bool m_type_sum
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Sum;
			}
		}

		// Token: 0x17001276 RID: 4726
		// (get) Token: 0x060051F6 RID: 20982 RVA: 0x00076B60 File Offset: 0x00074D60
		private bool m_type_mean
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Mean;
			}
		}

		// Token: 0x17001277 RID: 4727
		// (get) Token: 0x060051F7 RID: 20983 RVA: 0x00076B6B File Offset: 0x00074D6B
		private bool m_type_median
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Median;
			}
		}

		// Token: 0x17001278 RID: 4728
		// (get) Token: 0x060051F8 RID: 20984 RVA: 0x00076B76 File Offset: 0x00074D76
		private bool m_type_clamp
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Clamp;
			}
		}

		// Token: 0x17001279 RID: 4729
		// (get) Token: 0x060051F9 RID: 20985 RVA: 0x00076B81 File Offset: 0x00074D81
		private bool m_type_add
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Add;
			}
		}

		// Token: 0x1700127A RID: 4730
		// (get) Token: 0x060051FA RID: 20986 RVA: 0x00076B8C File Offset: 0x00074D8C
		private bool m_type_subtract
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Subtract;
			}
		}

		// Token: 0x1700127B RID: 4731
		// (get) Token: 0x060051FB RID: 20987 RVA: 0x00076B97 File Offset: 0x00074D97
		private bool m_type_multiply
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Multiply;
			}
		}

		// Token: 0x1700127C RID: 4732
		// (get) Token: 0x060051FC RID: 20988 RVA: 0x00076BA2 File Offset: 0x00074DA2
		private bool m_type_divide
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Divide;
			}
		}

		// Token: 0x1700127D RID: 4733
		// (get) Token: 0x060051FD RID: 20989 RVA: 0x00076BAD File Offset: 0x00074DAD
		private bool m_type_normalize
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Normalize;
			}
		}

		// Token: 0x1700127E RID: 4734
		// (get) Token: 0x060051FE RID: 20990 RVA: 0x00076BB9 File Offset: 0x00074DB9
		private bool m_type_min
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Min;
			}
		}

		// Token: 0x1700127F RID: 4735
		// (get) Token: 0x060051FF RID: 20991 RVA: 0x00076BC5 File Offset: 0x00074DC5
		private bool m_type_max
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Max;
			}
		}

		// Token: 0x17001280 RID: 4736
		// (get) Token: 0x06005200 RID: 20992 RVA: 0x00076BD1 File Offset: 0x00074DD1
		private bool m_type_quality
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.Quality;
			}
		}

		// Token: 0x17001281 RID: 4737
		// (get) Token: 0x06005201 RID: 20993 RVA: 0x00076BDD File Offset: 0x00074DDD
		private bool m_type_aggquality
		{
			get
			{
				return this.m_type == ComponentEffectOperationType.AggregateQuality;
			}
		}

		// Token: 0x17001282 RID: 4738
		// (get) Token: 0x06005202 RID: 20994 RVA: 0x001D2850 File Offset: 0x001D0A50
		private bool m_isReductionType
		{
			get
			{
				return this.m_type_value || this.m_type_sum || this.m_type_mean || this.m_type_median || this.m_type_min || this.m_type_max || this.m_type_quality || this.m_type_aggquality;
			}
		}

		// Token: 0x17001283 RID: 4739
		// (get) Token: 0x06005203 RID: 20995 RVA: 0x00076BE9 File Offset: 0x00074DE9
		private bool m_needsScalarValue
		{
			get
			{
				return this.m_type_value || this.m_type_add || this.m_type_subtract || this.m_type_multiply || this.m_type_divide;
			}
		}

		// Token: 0x17001284 RID: 4740
		// (get) Token: 0x06005204 RID: 20996 RVA: 0x00076C13 File Offset: 0x00074E13
		private bool m_isRangeType
		{
			get
			{
				return this.m_type_clamp || this.m_type_normalize;
			}
		}

		// Token: 0x17001285 RID: 4741
		// (get) Token: 0x06005205 RID: 20997 RVA: 0x00076C25 File Offset: 0x00074E25
		private bool m_isQualityType
		{
			get
			{
				return this.m_type_quality || this.m_type_aggquality;
			}
		}

		// Token: 0x17001286 RID: 4742
		// (get) Token: 0x06005206 RID: 20998 RVA: 0x001D28A0 File Offset: 0x001D0AA0
		private bool m_shouldSelectAttribute
		{
			get
			{
				if (this.m_type_value || this.m_isQualityType)
				{
					return false;
				}
				if (!(this.FindOperationsProfile() == null))
				{
					ComponentEffectOperationsProfile componentEffectOperationsProfile = this.FindOperationsProfile();
					return ((componentEffectOperationsProfile != null) ? componentEffectOperationsProfile.Operations[0] : null) == this;
				}
				ComponentEffect componentEffect = this.FindComponentEffect();
				return ((componentEffect != null) ? componentEffect.Operations[0] : null) == this;
			}
		}

		// Token: 0x17001287 RID: 4743
		// (get) Token: 0x06005207 RID: 20999 RVA: 0x00076C37 File Offset: 0x00074E37
		public bool IsReductionType
		{
			get
			{
				return this.m_isReductionType;
			}
		}

		// Token: 0x17001288 RID: 4744
		// (get) Token: 0x06005208 RID: 21000 RVA: 0x00076C3F File Offset: 0x00074E3F
		public ComponentEffectOperationType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x06005209 RID: 21001 RVA: 0x001D28FC File Offset: 0x001D0AFC
		private ComponentEffect FindComponentEffect()
		{
			if (this.m_parentEffect != null)
			{
				return this.m_parentEffect;
			}
			ComponentEffectsProfile componentEffectsProfile = this.FindEffectsProfile();
			if (componentEffectsProfile != null)
			{
				foreach (ComponentEffect componentEffect in componentEffectsProfile.ComponentEffects)
				{
					if (((componentEffect != null) ? componentEffect.Operations : null) != null)
					{
						ComponentEffectOperation[] operations = componentEffect.Operations;
						for (int j = 0; j < operations.Length; j++)
						{
							if (operations[j] == this)
							{
								ComponentEffect result = this.m_parentEffect = componentEffect;
								return result;
							}
						}
					}
				}
				return null;
			}
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				ItemArchetype itemArchetype = baseArchetype as ItemArchetype;
				if (itemArchetype != null && itemArchetype.HasComponentEffects)
				{
					foreach (ComponentEffect componentEffect2 in itemArchetype.ComponentEffects)
					{
						if (((componentEffect2 != null) ? componentEffect2.Operations : null) != null)
						{
							ComponentEffectOperation[] operations = componentEffect2.Operations;
							for (int j = 0; j < operations.Length; j++)
							{
								if (operations[j] == this)
								{
									ComponentEffect result = this.m_parentEffect = componentEffect2;
									return result;
								}
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x0600520A RID: 21002 RVA: 0x00049FFA File Offset: 0x000481FA
		private ComponentEffectsProfile FindEffectsProfile()
		{
			return null;
		}

		// Token: 0x0600520B RID: 21003 RVA: 0x00049FFA File Offset: 0x000481FA
		private ComponentEffectOperationsProfile FindOperationsProfile()
		{
			return null;
		}

		// Token: 0x0600520C RID: 21004 RVA: 0x00076C47 File Offset: 0x00074E47
		public void ResetAuthoringAwareness()
		{
			this.m_parentEffect = null;
			this.m_effectsProfile = null;
			this.m_hasLookedForEffectsProfile = false;
			this.m_operationsProfile = null;
			this.m_hasLookedForOperationsProfile = false;
		}

		// Token: 0x0600520D RID: 21005 RVA: 0x001D2A48 File Offset: 0x001D0C48
		private IEnumerable<IValueDropdownItem> GetAttributeNames()
		{
			List<IValueDropdownItem> list = new List<IValueDropdownItem>(ItemAttributes.AttributeCount);
			foreach (ItemAttributes.Names names in ItemAttributes.AttributeNames)
			{
				list.Add(new ValueDropdownItem(names.ToString(), names));
			}
			return list;
		}

		// Token: 0x0600520E RID: 21006 RVA: 0x001D2ABC File Offset: 0x001D0CBC
		public void Perform(float[] input, ArchetypeInstance instance, ComponentParentage[] componentFilters = null)
		{
			if (input == null || input.Length == 0)
			{
				return;
			}
			switch (this.m_type)
			{
			case ComponentEffectOperationType.Value:
				input[0] = this.m_value;
				for (int i = 1; i < input.Length; i++)
				{
					input[i] = 0f;
				}
				return;
			case ComponentEffectOperationType.Sum:
				for (int j = 1; j < input.Length; j++)
				{
					input[0] += input[j];
				}
				for (int k = 1; k < input.Length; k++)
				{
					input[k] = 0f;
				}
				return;
			case ComponentEffectOperationType.Mean:
				for (int l = 1; l < input.Length; l++)
				{
					input[0] += input[l];
				}
				input[0] /= (float)input.Length;
				for (int m = 1; m < input.Length; m++)
				{
					input[m] = 0f;
				}
				return;
			case ComponentEffectOperationType.Median:
			{
				float num = float.MaxValue;
				float num2 = float.MinValue;
				foreach (float val in input)
				{
					num = Math.Min(val, num);
					num2 = Math.Max(val, num2);
				}
				input[0] = (num2 - num) / 2f + num;
				for (int num3 = 1; num3 < input.Length; num3++)
				{
					input[num3] = 0f;
				}
				return;
			}
			case ComponentEffectOperationType.Clamp:
				for (int num4 = 0; num4 < input.Length; num4++)
				{
					input[num4] = Math.Max(Math.Min(input[num4], this.m_range.Max), this.m_range.Min);
				}
				return;
			case ComponentEffectOperationType.Add:
				for (int num5 = 0; num5 < input.Length; num5++)
				{
					input[num5] += this.m_value;
				}
				return;
			case ComponentEffectOperationType.Subtract:
				for (int num6 = 0; num6 < input.Length; num6++)
				{
					input[num6] -= this.m_value;
				}
				return;
			case ComponentEffectOperationType.Multiply:
				for (int num7 = 0; num7 < input.Length; num7++)
				{
					input[num7] *= this.m_value;
				}
				return;
			case ComponentEffectOperationType.Divide:
				if (this.m_value == 0f)
				{
					return;
				}
				for (int num8 = 0; num8 < input.Length; num8++)
				{
					input[num8] /= this.m_value;
				}
				return;
			case ComponentEffectOperationType.Normalize:
				for (int num9 = 0; num9 < input.Length; num9++)
				{
					input[num9] = (input[num9] - this.m_range.Min) / this.m_range.Delta;
				}
				return;
			case ComponentEffectOperationType.Min:
			{
				float num = float.MaxValue;
				for (int n = 0; n < input.Length; n++)
				{
					num = Math.Min(input[n], num);
				}
				input[0] = num;
				return;
			}
			case ComponentEffectOperationType.Max:
			{
				float num2 = float.MinValue;
				for (int n = 0; n < input.Length; n++)
				{
					num2 = Math.Max(input[n], num2);
				}
				input[0] = num2;
				return;
			}
			case ComponentEffectOperationType.Quality:
			{
				int num10 = 0;
				ItemInstanceData itemData = instance.ItemData;
				float? num11;
				if (itemData == null)
				{
					num11 = null;
				}
				else
				{
					ItemComponentTree itemComponentTree = itemData.ItemComponentTree;
					num11 = ((itemComponentTree != null) ? itemComponentTree.QualityModifier : null);
				}
				input[num10] = (num11 ?? ((float)100));
				for (int num12 = 1; num12 < input.Length; num12++)
				{
					input[num12] = 0f;
				}
				return;
			}
			case ComponentEffectOperationType.AggregateQuality:
			{
				ItemInstanceData itemData2 = instance.ItemData;
				List<RawComponentWithQuality> list;
				if (itemData2 == null)
				{
					list = null;
				}
				else
				{
					ItemComponentTree itemComponentTree2 = itemData2.ItemComponentTree;
					list = ((itemComponentTree2 != null) ? itemComponentTree2.GetRawMaterialsListWithQualities(componentFilters) : null);
				}
				input[0] = 100f;
				foreach (RawComponentWithQuality rawComponentWithQuality in list)
				{
					input[0] = (float)rawComponentWithQuality.Quality / 100f * (input[0] / 100f) * 100f;
				}
				for (int num13 = 1; num13 < input.Length; num13++)
				{
					input[num13] = 0f;
				}
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04004975 RID: 18805
		[SerializeField]
		private ItemAttributes.Names m_attribute = ItemAttributes.Names.Hardness;

		// Token: 0x04004976 RID: 18806
		[SerializeField]
		private ComponentEffectOperationType m_type = ComponentEffectOperationType.Sum;

		// Token: 0x04004977 RID: 18807
		[SerializeField]
		private float m_value;

		// Token: 0x04004978 RID: 18808
		[SerializeField]
		private MinMaxFloatRange m_range = new MinMaxFloatRange(0f, 0f);

		// Token: 0x04004979 RID: 18809
		[HideInInspector]
		[NonSerialized]
		private ComponentEffect m_parentEffect;

		// Token: 0x0400497A RID: 18810
		private ComponentEffectsProfile m_effectsProfile;

		// Token: 0x0400497B RID: 18811
		private bool m_hasLookedForEffectsProfile;

		// Token: 0x0400497C RID: 18812
		private ComponentEffectOperationsProfile m_operationsProfile;

		// Token: 0x0400497D RID: 18813
		private bool m_hasLookedForOperationsProfile;
	}
}
