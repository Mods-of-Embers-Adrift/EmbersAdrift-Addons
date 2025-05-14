using System;
using SoL.Game.EffectSystem;
using SoL.Game.NPCs;
using SoL.Game.Targeting;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000565 RID: 1381
	public class CharacterDataProxy : CharacterData
	{
		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x06002A32 RID: 10802 RVA: 0x00053500 File Offset: 0x00051700
		// (set) Token: 0x06002A33 RID: 10803 RVA: 0x0004475B File Offset: 0x0004295B
		public override Faction Faction
		{
			get
			{
				return Faction.Player;
			}
			set
			{
			}
		}

		// Token: 0x06002A34 RID: 10804 RVA: 0x00142B78 File Offset: 0x00140D78
		private void SetFlags()
		{
			if (this.m_targetEntity != null)
			{
				if (this.m_targetEntity.Type == GameEntityType.Npc)
				{
					this.m_targetEntity.ServerNpcController.BehaviorFlagsInternal = this.m_behaviorFlags;
					return;
				}
			}
			else
			{
				ServerNpcController[] array = UnityEngine.Object.FindObjectsOfType<ServerNpcController>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].BehaviorFlagsInternal = this.m_behaviorFlags;
				}
			}
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x0005D0F1 File Offset: 0x0005B2F1
		private void AddThreat()
		{
			this.AlterThreat(this.m_threat);
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x0005D0FF File Offset: 0x0005B2FF
		private void RemoveThreat()
		{
			this.AlterThreat(-this.m_threat);
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x00142BDC File Offset: 0x00140DDC
		private void AlterThreat(float threat)
		{
			NpcTargetController[] array = UnityEngine.Object.FindObjectsOfType<NpcTargetController>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddThreatToTarget(base.GameEntity.NetworkEntity, threat);
			}
			this.m_cumulativeThreat += threat;
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x0005D10E File Offset: 0x0005B30E
		private void AddTargetedThreat()
		{
			this.AlterTargetedThreat(this.m_threat);
		}

		// Token: 0x06002A39 RID: 10809 RVA: 0x0005D11C File Offset: 0x0005B31C
		private void RemoveTargetedThreat()
		{
			this.AlterTargetedThreat(-this.m_threat);
		}

		// Token: 0x06002A3A RID: 10810 RVA: 0x00142C20 File Offset: 0x00140E20
		private void AlterTargetedThreat(float threat)
		{
			NpcTargetController npcTargetController;
			if (this.m_targetEntity != null && this.m_targetEntity.TargetController != null && this.m_targetEntity.TargetController.TryGetAsType(out npcTargetController))
			{
				npcTargetController.AddThreatToTarget(base.GameEntity.NetworkEntity, threat);
			}
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x06002A3B RID: 10811 RVA: 0x0005D12B File Offset: 0x0005B32B
		private bool m_showTargetedThreat
		{
			get
			{
				return this.m_targetEntity != null;
			}
		}

		// Token: 0x04002AE8 RID: 10984
		[SerializeField]
		private GameEntity m_targetEntity;

		// Token: 0x04002AE9 RID: 10985
		[SerializeField]
		private float m_threat = 1f;

		// Token: 0x04002AEA RID: 10986
		[SerializeField]
		private BehaviorEffectTypeFlags m_behaviorFlags;

		// Token: 0x04002AEB RID: 10987
		private float m_cumulativeThreat;
	}
}
