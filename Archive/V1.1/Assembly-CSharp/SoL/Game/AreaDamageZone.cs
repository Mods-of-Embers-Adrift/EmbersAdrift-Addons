using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000558 RID: 1368
	public class AreaDamageZone : MonoBehaviour
	{
		// Token: 0x06002985 RID: 10629 RVA: 0x00141C48 File Offset: 0x0013FE48
		private void Awake()
		{
			if (!GameManager.IsServer)
			{
				this.m_collider.enabled = false;
				base.enabled = false;
				return;
			}
			this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
			this.m_collider.isTrigger = true;
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x00141C9C File Offset: 0x0013FE9C
		private void IncrementTime()
		{
			float num = this.m_overrideApplicationDelta ? this.m_applicationDelta : 2f;
			this.m_timeOfNextHit = Time.time + num;
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x0005CB3A File Offset: 0x0005AD3A
		private IEnumerable GetEffects()
		{
			return SolOdinUtilities.GetDropdownItems<ScriptableEffectData>();
		}

		// Token: 0x04002A74 RID: 10868
		private const float kDefaultApplicationDelta = 2f;

		// Token: 0x04002A75 RID: 10869
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04002A76 RID: 10870
		[SerializeField]
		private GameEntityTypeFlags m_applicationTargets;

		// Token: 0x04002A77 RID: 10871
		[Range(0f, 1f)]
		[SerializeField]
		private float m_chanceToApply = 1f;

		// Token: 0x04002A78 RID: 10872
		[SerializeField]
		private ScriptableEffectData m_effectToApply;

		// Token: 0x04002A79 RID: 10873
		private const string kOverrideGroup = "Override";

		// Token: 0x04002A7A RID: 10874
		[SerializeField]
		private bool m_overrideApplicationDelta;

		// Token: 0x04002A7B RID: 10875
		[SerializeField]
		private float m_applicationDelta = 2f;

		// Token: 0x04002A7C RID: 10876
		[Tooltip("if set to true the effect will NOT be applied via OnTriggerEnter")]
		[SerializeField]
		private bool m_bypassApplyOnEntry;

		// Token: 0x04002A7D RID: 10877
		private float m_timeOfNextHit;

		// Token: 0x04002A7E RID: 10878
		private List<GameEntity> m_currentEntities;
	}
}
