using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE2 RID: 3298
	[Serializable]
	public class ItemAttributes
	{
		// Token: 0x060063EF RID: 25583 RVA: 0x00083492 File Offset: 0x00081692
		public int GetAttribute(ItemAttributes.Names enumName)
		{
			if (ItemAttributes.m_getters.ContainsKey(enumName))
			{
				return ItemAttributes.m_getters[enumName](this);
			}
			return 0;
		}

		// Token: 0x060063F0 RID: 25584 RVA: 0x000834B4 File Offset: 0x000816B4
		public bool IsActive(ItemAttributes.Names enumName)
		{
			return ItemAttributes.m_isActive.ContainsKey(enumName) && ItemAttributes.m_isActive[enumName](this);
		}

		// Token: 0x170017F0 RID: 6128
		// (get) Token: 0x060063F1 RID: 25585 RVA: 0x000834D6 File Offset: 0x000816D6
		public static int AttributeCount
		{
			get
			{
				return ItemAttributes.m_getters.Count;
			}
		}

		// Token: 0x170017F1 RID: 6129
		// (get) Token: 0x060063F2 RID: 25586 RVA: 0x000834E2 File Offset: 0x000816E2
		public static IEnumerable<ItemAttributes.Names> AttributeNames
		{
			get
			{
				return ItemAttributes.m_getters.Keys;
			}
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x00208008 File Offset: 0x00206208
		static ItemAttributes()
		{
			foreach (FieldInfo fieldInfo in typeof(ItemAttributes).GetFields())
			{
				if (!fieldInfo.IsStatic)
				{
					if (fieldInfo.FieldType == typeof(int))
					{
						ParameterExpression parameterExpression = Expression.Variable(typeof(ItemAttributes), "obj");
						Func<ItemAttributes, int> value = Expression.Lambda<Func<ItemAttributes, int>>(Expression.MakeMemberAccess(parameterExpression, fieldInfo), new ParameterExpression[]
						{
							parameterExpression
						}).Compile();
						ItemAttributes.m_getters.Add((ItemAttributes.Names)Enum.Parse(typeof(ItemAttributes.Names), fieldInfo.Name), value);
					}
					else if (fieldInfo.FieldType == typeof(bool))
					{
						ParameterExpression parameterExpression2 = Expression.Variable(typeof(ItemAttributes), "obj");
						Func<ItemAttributes, bool> value2 = Expression.Lambda<Func<ItemAttributes, bool>>(Expression.MakeMemberAccess(parameterExpression2, fieldInfo), new ParameterExpression[]
						{
							parameterExpression2
						}).Compile();
						ItemAttributes.m_isActive.Add((ItemAttributes.Names)Enum.Parse(typeof(ItemAttributes.Names), fieldInfo.Name.Replace("Enabled", string.Empty)), value2);
					}
				}
			}
		}

		// Token: 0x040056C6 RID: 22214
		public const int kMinValue = 0;

		// Token: 0x040056C7 RID: 22215
		public const int kMaxValue = 1000;

		// Token: 0x040056C8 RID: 22216
		public bool DensityEnabled = true;

		// Token: 0x040056C9 RID: 22217
		[Range(0f, 1000f)]
		public int Density;

		// Token: 0x040056CA RID: 22218
		public bool HardnessEnabled = true;

		// Token: 0x040056CB RID: 22219
		[Range(0f, 1000f)]
		public int Hardness;

		// Token: 0x040056CC RID: 22220
		public bool ThicknessEnabled = true;

		// Token: 0x040056CD RID: 22221
		[Range(0f, 1000f)]
		public int Thickness;

		// Token: 0x040056CE RID: 22222
		public bool InsulationEnabled = true;

		// Token: 0x040056CF RID: 22223
		[Range(0f, 1000f)]
		public int Insulation;

		// Token: 0x040056D0 RID: 22224
		public bool DuctilityEnabled = true;

		// Token: 0x040056D1 RID: 22225
		[Range(0f, 1000f)]
		public int Ductility;

		// Token: 0x040056D2 RID: 22226
		public bool MalleabilityEnabled = true;

		// Token: 0x040056D3 RID: 22227
		[Range(0f, 1000f)]
		public int Malleability;

		// Token: 0x040056D4 RID: 22228
		public bool DurabilityEnabled = true;

		// Token: 0x040056D5 RID: 22229
		[Range(0f, 1000f)]
		public int Durability;

		// Token: 0x040056D6 RID: 22230
		public bool FlavorEnabled = true;

		// Token: 0x040056D7 RID: 22231
		[Range(0f, 1000f)]
		public int Flavor;

		// Token: 0x040056D8 RID: 22232
		public bool EnergyEnabled = true;

		// Token: 0x040056D9 RID: 22233
		[Range(0f, 1000f)]
		public int Energy;

		// Token: 0x040056DA RID: 22234
		private static Dictionary<ItemAttributes.Names, Func<ItemAttributes, int>> m_getters = new Dictionary<ItemAttributes.Names, Func<ItemAttributes, int>>();

		// Token: 0x040056DB RID: 22235
		private static Dictionary<ItemAttributes.Names, Func<ItemAttributes, bool>> m_isActive = new Dictionary<ItemAttributes.Names, Func<ItemAttributes, bool>>();

		// Token: 0x02000CE3 RID: 3299
		public enum Names
		{
			// Token: 0x040056DD RID: 22237
			Density,
			// Token: 0x040056DE RID: 22238
			Hardness,
			// Token: 0x040056DF RID: 22239
			Thickness,
			// Token: 0x040056E0 RID: 22240
			Insulation,
			// Token: 0x040056E1 RID: 22241
			Ductility = 20,
			// Token: 0x040056E2 RID: 22242
			Malleability,
			// Token: 0x040056E3 RID: 22243
			Durability,
			// Token: 0x040056E4 RID: 22244
			Flavor = 100,
			// Token: 0x040056E5 RID: 22245
			Energy
		}
	}
}
