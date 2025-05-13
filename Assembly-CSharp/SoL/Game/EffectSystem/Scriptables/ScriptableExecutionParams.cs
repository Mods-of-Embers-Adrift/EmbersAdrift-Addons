using System;
using UnityEngine;

namespace SoL.Game.EffectSystem.Scriptables
{
	// Token: 0x02000C85 RID: 3205
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Ability Parameters/Execution")]
	public class ScriptableExecutionParams : ScriptableObject
	{
		// Token: 0x1700175C RID: 5980
		// (get) Token: 0x06006188 RID: 24968 RVA: 0x00081C01 File Offset: 0x0007FE01
		internal ExecutionParams Params
		{
			get
			{
				return this.m_executionParams;
			}
		}

		// Token: 0x04005500 RID: 21760
		[SerializeField]
		private ExecutionParams m_executionParams;
	}
}
