using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A57 RID: 2647
	[CreateAssetMenu(menuName = "SoL/Profiles/Component Effect Operations")]
	public class ComponentEffectOperationsProfile : ScriptableObject
	{
		// Token: 0x17001289 RID: 4745
		// (get) Token: 0x06005210 RID: 21008 RVA: 0x00076C97 File Offset: 0x00074E97
		public ComponentEffectOperation[] Operations
		{
			get
			{
				return this.m_operations;
			}
		}

		// Token: 0x06005211 RID: 21009 RVA: 0x001D2E90 File Offset: 0x001D1090
		private bool ValidateOperations(ComponentEffectOperation[] value, ref string errorMessage)
		{
			if (value == null || value.Length == 0)
			{
				return true;
			}
			bool flag = false;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i].IsReductionType)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				errorMessage = "You must have at least one reduction step in your operations list so that the value can be assigned to the output stat.";
				return false;
			}
			return true;
		}

		// Token: 0x0400497E RID: 18814
		[SerializeField]
		private ComponentEffectOperation[] m_operations;
	}
}
