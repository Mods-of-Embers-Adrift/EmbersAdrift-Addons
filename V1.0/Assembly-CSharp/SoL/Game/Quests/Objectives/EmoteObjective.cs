using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A1 RID: 1953
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/EmoteObjective")]
	public class EmoteObjective : OrderDrivenObjective<EmoteObjective>
	{
		// Token: 0x17000D3C RID: 3388
		// (get) Token: 0x0600399C RID: 14748 RVA: 0x0006705A File Offset: 0x0006525A
		public Emote[] Emotes
		{
			get
			{
				return this.m_emotes;
			}
		}

		// Token: 0x0600399D RID: 14749 RVA: 0x00067062 File Offset: 0x00065262
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			return base.Validate(sourceEntity, cache, out message) && sourceEntity.Vitals.Stance.CanEmote();
		}

		// Token: 0x0600399E RID: 14750 RVA: 0x00067093 File Offset: 0x00065293
		public bool IsValidEmote(Emote emote)
		{
			return this.IsValidEmote(emote.Id);
		}

		// Token: 0x0600399F RID: 14751 RVA: 0x001739AC File Offset: 0x00171BAC
		public bool IsValidEmote(UniqueId emoteId)
		{
			bool flag = false;
			foreach (Emote emote in this.m_emotes)
			{
				flag = (flag || emoteId == emote.Id);
			}
			return flag;
		}

		// Token: 0x04003847 RID: 14407
		[SerializeField]
		private Emote[] m_emotes;
	}
}
