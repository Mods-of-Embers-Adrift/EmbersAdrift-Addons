using System;
using System.Collections.Generic;
using SoL;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

// Token: 0x02000009 RID: 9
[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Crafting/Material Category")]
public class MaterialCategory : ScriptableObject
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000014 RID: 20 RVA: 0x0004476D File Offset: 0x0004296D
	public string DisplayName
	{
		get
		{
			if (!string.IsNullOrEmpty(this.m_displayName))
			{
				return this.m_displayName;
			}
			return base.name;
		}
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0008782C File Offset: 0x00085A2C
	public ComponentMaterial GetComponentMaterial(UniqueId archetypeId, int amount)
	{
		if (this.m_materials != null)
		{
			foreach (ItemArchetype itemArchetype in this.m_materials)
			{
				if (!(itemArchetype == null) && itemArchetype.Id == archetypeId)
				{
					this.m_tempMaterialObj.Archetype = itemArchetype;
					this.m_tempMaterialObj.AmountRequired = amount;
					return this.m_tempMaterialObj;
				}
			}
		}
		if (this.m_includedCategories != null)
		{
			MaterialCategory[] includedCategories = this.m_includedCategories;
			for (int i = 0; i < includedCategories.Length; i++)
			{
				ComponentMaterial componentMaterial = includedCategories[i].GetComponentMaterial(archetypeId, amount);
				if (componentMaterial != null)
				{
					return componentMaterial;
				}
			}
		}
		return null;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000878C4 File Offset: 0x00085AC4
	public List<ComponentMaterial> AggregateMaterialList(int amount, List<ComponentMaterial> list)
	{
		if (list == null)
		{
			list = StaticListPool<ComponentMaterial>.GetFromPool();
		}
		if (this.m_materials != null)
		{
			foreach (ItemArchetype itemArchetype in this.m_materials)
			{
				if (!(itemArchetype == null))
				{
					bool flag = false;
					foreach (ComponentMaterial componentMaterial in list)
					{
						if (componentMaterial != null && componentMaterial.Archetype.Id == itemArchetype.Id)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						list.Add(new ComponentMaterial
						{
							Archetype = itemArchetype,
							AmountRequired = amount
						});
					}
				}
			}
		}
		if (this.m_includedCategories != null)
		{
			MaterialCategory[] includedCategories = this.m_includedCategories;
			for (int i = 0; i < includedCategories.Length; i++)
			{
				list = includedCategories[i].AggregateMaterialList(amount, list);
			}
		}
		return list;
	}

	// Token: 0x0400000D RID: 13
	[SerializeField]
	private string m_displayName;

	// Token: 0x0400000E RID: 14
	[SerializeField]
	private ItemArchetype[] m_materials;

	// Token: 0x0400000F RID: 15
	[SerializeField]
	private MaterialCategory[] m_includedCategories;

	// Token: 0x04000010 RID: 16
	private ComponentMaterial m_tempMaterialObj = new ComponentMaterial();
}
