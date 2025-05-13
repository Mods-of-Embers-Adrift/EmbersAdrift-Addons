using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A65 RID: 2661
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Consumables/FireStarter")]
	public class FireStarterItem : ConsumableItemStackable
	{
		// Token: 0x0600526C RID: 21100 RVA: 0x001D3CD8 File Offset: 0x001D1ED8
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			Collider[] colliders = Hits.Colliders5;
			int num = Physics.OverlapSphereNonAlloc(executionCache.SourceEntity.gameObject.transform.position, 5f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			int i = 0;
			while (i < num)
			{
				LightableCampfire component = colliders[i].gameObject.GetComponent<LightableCampfire>();
				if (component != null)
				{
					if (component.IsLit)
					{
						executionCache.Message = "Already lit!";
						return false;
					}
					executionCache.SetTargetNetworkEntity(component.GameEntity.NetworkEntity);
					return true;
				}
				else
				{
					i++;
				}
			}
			executionCache.Message = "Nothing to light!";
			return false;
		}

		// Token: 0x0600526D RID: 21101 RVA: 0x001D3D74 File Offset: 0x001D1F74
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			if (GameManager.IsServer)
			{
				LightableCampfire component = executionCache.TargetNetworkEntity.GetComponent<LightableCampfire>();
				if (component != null)
				{
					component.LightFire(executionCache.SourceEntity, 1);
				}
			}
		}

		// Token: 0x0600526E RID: 21102 RVA: 0x001D3DB4 File Offset: 0x001D1FB4
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			dataBlock.AppendLine("Fire Duration: " + this.m_fireDuration.GetFormattedTime(true), 0);
			dataBlock.AppendLine("Power: " + this.m_profileFraction.GetAsPercentage(), 0);
		}

		// Token: 0x0600526F RID: 21103 RVA: 0x0007704B File Offset: 0x0007524B
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.FireDuration <= 1 || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x06005270 RID: 21104 RVA: 0x001D3E08 File Offset: 0x001D2008
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.FireDuration)
			{
				this.m_fireDuration = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_fireDuration);
				return true;
			}
			if (assignerName != ComponentEffectAssignerName.ProfileFraction)
			{
				return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
			}
			this.m_profileFraction = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_profileFraction);
			return true;
		}

		// Token: 0x040049BE RID: 18878
		public const float kRange = 5f;

		// Token: 0x040049BF RID: 18879
		[SerializeField]
		private float m_fireDuration = 300f;

		// Token: 0x040049C0 RID: 18880
		[SerializeField]
		private float m_profileFraction = 1f;
	}
}
