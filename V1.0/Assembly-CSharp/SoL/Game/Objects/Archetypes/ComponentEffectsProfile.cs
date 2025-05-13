using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A59 RID: 2649
	[CreateAssetMenu(menuName = "SoL/Profiles/Component Effects")]
	[Serializable]
	public class ComponentEffectsProfile : ScriptableObject
	{
		// Token: 0x1700128D RID: 4749
		// (get) Token: 0x06005218 RID: 21016 RVA: 0x00076CC3 File Offset: 0x00074EC3
		public ComponentEffect[] ComponentEffects
		{
			get
			{
				return this.m_componentEffects;
			}
		}

		// Token: 0x1700128E RID: 4750
		// (get) Token: 0x06005219 RID: 21017 RVA: 0x00076CCB File Offset: 0x00074ECB
		public bool HasComponentEffects
		{
			get
			{
				return this.m_componentEffects != null && this.m_componentEffects.Length != 0;
			}
		}

		// Token: 0x1700128F RID: 4751
		// (get) Token: 0x0600521A RID: 21018 RVA: 0x00076CE1 File Offset: 0x00074EE1
		public bool HasNumericComponentEffects
		{
			get
			{
				return this.m_componentEffects != null && this.m_componentEffects.Length != 0 && this.HasNumericEffects();
			}
		}

		// Token: 0x0600521B RID: 21019 RVA: 0x001D3008 File Offset: 0x001D1208
		public void AffectItem(ItemArchetype archetype, ArchetypeInstance instance, RangeOverride[] rangeOverrides, ComponentParentage[] componentFiltersOverride = null)
		{
			if (this.m_componentEffects != null)
			{
				foreach (ComponentEffect componentEffect in this.m_componentEffects)
				{
					bool flag = false;
					foreach (ComponentParentage componentParentage in componentFiltersOverride)
					{
						flag = (flag || (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentParentage.ComponentList.Length != 0));
					}
					if (!flag && this.m_filtersForArchetype != null)
					{
						foreach (ComponentFilterForArchetype componentFilterForArchetype in this.m_filtersForArchetype)
						{
							if (componentFilterForArchetype.Archetype.Id == archetype.Id)
							{
								componentFiltersOverride = componentFilterForArchetype.ComponentFilters;
								break;
							}
						}
					}
					componentEffect.AffectItem(archetype, instance, rangeOverrides, componentFiltersOverride);
				}
			}
		}

		// Token: 0x0600521C RID: 21020 RVA: 0x001D30E0 File Offset: 0x001D12E0
		private bool HasNumericEffects()
		{
			bool flag = false;
			if (this.m_componentEffects != null)
			{
				foreach (ComponentEffect componentEffect in this.m_componentEffects)
				{
					bool flag2;
					if (!flag && (componentEffect.OperationType != OperationType.Numeric || componentEffect.Operations.Length == 0))
					{
						if (componentEffect.OperationType == OperationType.Profile)
						{
							ComponentEffectsProfile componentEffectsProfile = componentEffect.ComponentEffectsProfile;
							flag2 = (componentEffectsProfile != null && componentEffectsProfile.HasNumericComponentEffects);
						}
						else
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = true;
					}
					flag = flag2;
				}
			}
			return flag;
		}

		// Token: 0x04004981 RID: 18817
		[SerializeField]
		private ComponentEffect[] m_componentEffects;

		// Token: 0x04004982 RID: 18818
		[SerializeField]
		private ComponentFilterForArchetype[] m_filtersForArchetype;
	}
}
