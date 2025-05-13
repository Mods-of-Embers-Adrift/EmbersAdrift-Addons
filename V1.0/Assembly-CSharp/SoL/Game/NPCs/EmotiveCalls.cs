using System;
using System.Collections;
using SoL.Game.Messages;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007F8 RID: 2040
	[Serializable]
	public class EmotiveCalls
	{
		// Token: 0x17000D8C RID: 3468
		// (get) Token: 0x06003B3F RID: 15167 RVA: 0x00068207 File Offset: 0x00066407
		private bool m_showLocalEmoteCollection
		{
			get
			{
				return this.m_emotiveCalls && this.m_override == null;
			}
		}

		// Token: 0x17000D8D RID: 3469
		// (get) Token: 0x06003B40 RID: 15168 RVA: 0x00063B0C File Offset: 0x00061D0C
		private IEnumerable GetStringProbCollections
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<StringScriptableProbabilityCollection>();
			}
		}

		// Token: 0x06003B41 RID: 15169 RVA: 0x0017AEE8 File Offset: 0x001790E8
		public void EmoteToNearbyPlayers(GameEntity source, float? rangeOverride = null)
		{
			if (this.m_emotiveCalls && GameManager.IsServer && ServerGameManager.SpatialManager != null)
			{
				string text;
				if (!this.m_override)
				{
					StringProbabilityCollection emotes = this.m_emotes;
					if (emotes == null)
					{
						text = null;
					}
					else
					{
						StringProbabilityEntry entry = emotes.GetEntry(null, false);
						text = ((entry != null) ? entry.Obj : null);
					}
				}
				else
				{
					StringProbabilityEntry entry2 = this.m_override.GetEntry();
					text = ((entry2 != null) ? entry2.Obj : null);
				}
				string text2 = text;
				if (!string.IsNullOrEmpty(text2))
				{
					float radius = (rangeOverride != null) ? rangeOverride.Value : 30f;
					ServerGameManager.SpatialManager.MessageNearbyPlayers(source, radius, MessageType.Emote | MessageType.PreFormatted, "<i>" + source.CharacterData.Name.Value + "</i> " + text2, MessageEventType.None, false);
				}
			}
		}

		// Token: 0x06003B42 RID: 15170 RVA: 0x0006821F File Offset: 0x0006641F
		public void Normalize()
		{
			StringProbabilityCollection emotes = this.m_emotes;
			if (emotes == null)
			{
				return;
			}
			emotes.Normalize();
		}

		// Token: 0x040039B4 RID: 14772
		private const string kGroup = "Npc Emotive Calls";

		// Token: 0x040039B5 RID: 14773
		[SerializeField]
		private bool m_emotiveCalls;

		// Token: 0x040039B6 RID: 14774
		[SerializeField]
		private StringScriptableProbabilityCollection m_override;

		// Token: 0x040039B7 RID: 14775
		[SerializeField]
		private StringProbabilityCollection m_emotes;
	}
}
