using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000774 RID: 1908
	public class NpcScaleAdjuster : GameEntityComponent
	{
		// Token: 0x0600385A RID: 14426 RVA: 0x000665A4 File Offset: 0x000647A4
		private void Start()
		{
			this.Initialize();
		}

		// Token: 0x0600385B RID: 14427 RVA: 0x000665AC File Offset: 0x000647AC
		internal void SetVarsExternal(bool pos, bool scale)
		{
			this.m_position = pos;
			this.m_scale = scale;
		}

		// Token: 0x0600385C RID: 14428 RVA: 0x0016D700 File Offset: 0x0016B900
		public void Initialize()
		{
			if (this.m_initialized)
			{
				return;
			}
			if (base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.TransformScale != null)
			{
				Transform transform = base.gameObject.transform;
				float d = 1.PercentModification(base.GameEntity.CharacterData.TransformScale.Value);
				if (this.m_position)
				{
					transform.localPosition *= d;
				}
				if (this.m_scale)
				{
					transform.localScale *= d;
				}
				this.m_initialized = true;
			}
		}

		// Token: 0x04003737 RID: 14135
		[SerializeField]
		private bool m_position;

		// Token: 0x04003738 RID: 14136
		[SerializeField]
		private bool m_scale;

		// Token: 0x04003739 RID: 14137
		private bool m_initialized;
	}
}
