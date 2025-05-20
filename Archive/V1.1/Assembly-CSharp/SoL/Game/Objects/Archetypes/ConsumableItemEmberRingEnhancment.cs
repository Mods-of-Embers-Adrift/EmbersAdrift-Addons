using System;
using System.Collections;
using SoL.Game.Interactives;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A5F RID: 2655
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Consumables/Ember Ring Enhancement")]
	public class ConsumableItemEmberRingEnhancment : ConsumableItemStackable
	{
		// Token: 0x170012A8 RID: 4776
		// (get) Token: 0x06005254 RID: 21076 RVA: 0x00076F49 File Offset: 0x00075149
		public float HealthRegenBooster
		{
			get
			{
				return this.m_healthRegenBooster;
			}
		}

		// Token: 0x170012A9 RID: 4777
		// (get) Token: 0x06005255 RID: 21077 RVA: 0x00076F51 File Offset: 0x00075151
		public float WoundRegenBooster
		{
			get
			{
				return this.m_woundRegenBooster;
			}
		}

		// Token: 0x170012AA RID: 4778
		// (get) Token: 0x06005256 RID: 21078 RVA: 0x00076F59 File Offset: 0x00075159
		public int Duration
		{
			get
			{
				return this.m_duration;
			}
		}

		// Token: 0x170012AB RID: 4779
		// (get) Token: 0x06005257 RID: 21079 RVA: 0x00076F61 File Offset: 0x00075161
		public int Level
		{
			get
			{
				return this.m_level;
			}
		}

		// Token: 0x170012AC RID: 4780
		// (get) Token: 0x06005258 RID: 21080 RVA: 0x00076F69 File Offset: 0x00075169
		public Color Color
		{
			get
			{
				return this.m_color;
			}
		}

		// Token: 0x170012AD RID: 4781
		// (get) Token: 0x06005259 RID: 21081 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x0600525A RID: 21082 RVA: 0x001D3534 File Offset: 0x001D1734
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			if (!base.ExecutionCheckInternal(executionCache))
			{
				return false;
			}
			Collider[] colliders = Hits.Colliders50;
			int num = Physics.OverlapSphereNonAlloc(executionCache.SourceEntity.gameObject.transform.position, 5f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			int i = 0;
			while (i < num)
			{
				InteractiveEmberRing interactiveEmberRing;
				if (colliders[i].gameObject.TryGetComponent<InteractiveEmberRing>(out interactiveEmberRing))
				{
					string message;
					if (!interactiveEmberRing.CanEnhanceWithItem(executionCache.SourceEntity, this, out message))
					{
						executionCache.Message = message;
						return false;
					}
					executionCache.SetTargetNetworkEntity(interactiveEmberRing.GameEntity.NetworkEntity);
					return true;
				}
				else
				{
					i++;
				}
			}
			executionCache.Message = "No Ember Ring nearby!";
			return false;
		}

		// Token: 0x0600525B RID: 21083 RVA: 0x00076F71 File Offset: 0x00075171
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
		}

		// Token: 0x040049A3 RID: 18851
		private const float kRange = 5f;

		// Token: 0x040049A4 RID: 18852
		[Range(1f, 50f)]
		[SerializeField]
		private int m_level = 1;

		// Token: 0x040049A5 RID: 18853
		[SerializeField]
		private float m_healthRegenBooster;

		// Token: 0x040049A6 RID: 18854
		[SerializeField]
		private float m_woundRegenBooster;

		// Token: 0x040049A7 RID: 18855
		[SerializeField]
		private int m_duration = 1800;

		// Token: 0x040049A8 RID: 18856
		[SerializeField]
		private Color m_color = Color.blue;
	}
}
