using System;
using System.Collections.Generic;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A8 RID: 2472
	[Serializable]
	public class ChatWindowSettings
	{
		// Token: 0x060049FC RID: 18940 RVA: 0x00071C3A File Offset: 0x0006FE3A
		public ChatWindowSettings()
		{
		}

		// Token: 0x060049FD RID: 18941 RVA: 0x001B1624 File Offset: 0x001AF824
		public ChatWindowSettings(ChatWindowSettings other)
		{
			this.Enabled = other.Enabled;
			this.ShowTimestamps = other.ShowTimestamps;
			this.Opacity = other.Opacity;
			this.TabSettings = new List<ChatTabSettings>(other.TabSettings.Count);
			for (int i = 0; i < other.TabSettings.Count; i++)
			{
				this.TabSettings.Add(new ChatTabSettings(other.TabSettings[i]));
			}
		}

		// Token: 0x060049FE RID: 18942 RVA: 0x00071C54 File Offset: 0x0006FE54
		public void MarkAsDirty()
		{
			this.m_dirty = true;
		}

		// Token: 0x060049FF RID: 18943 RVA: 0x00071C5D File Offset: 0x0006FE5D
		public void MarkAsClean()
		{
			this.m_dirty = false;
		}

		// Token: 0x06004A00 RID: 18944 RVA: 0x00071C66 File Offset: 0x0006FE66
		public bool IsDirty()
		{
			return this.m_dirty;
		}

		// Token: 0x06004A01 RID: 18945 RVA: 0x00071C6E File Offset: 0x0006FE6E
		public void SetAsCurrentVersion()
		{
			this.Version = 5;
		}

		// Token: 0x06004A02 RID: 18946 RVA: 0x001B16B8 File Offset: 0x001AF8B8
		public bool CheckVersionAndUpdate()
		{
			if (this.Version == 5)
			{
				return false;
			}
			this.Version = 5;
			ChatFilter? chatFilter = null;
			CombatFilter? combatFilter = null;
			ChatFilter value;
			if (ChatWindowSettings.ChatFiltersPerVersion.TryGetValue(5, out value))
			{
				chatFilter = new ChatFilter?(value);
			}
			CombatFilter value2;
			if (ChatWindowSettings.CombatFilterPerVersion.TryGetValue(5, out value2))
			{
				combatFilter = new CombatFilter?(value2);
			}
			for (int i = 0; i < this.TabSettings.Count; i++)
			{
				if (chatFilter != null)
				{
					this.TabSettings[i].ChatFilter |= chatFilter.Value;
				}
				if (combatFilter != null)
				{
					this.TabSettings[i].CombatFilter |= combatFilter.Value;
				}
			}
			return true;
		}

		// Token: 0x040044F7 RID: 17655
		private const int kCurrentVersion = 5;

		// Token: 0x040044F8 RID: 17656
		private static readonly Dictionary<int, ChatFilter> ChatFiltersPerVersion = new Dictionary<int, ChatFilter>
		{
			{
				3,
				ChatFilter.Subscriber
			},
			{
				4,
				ChatFilter.Help
			},
			{
				5,
				ChatFilter.Raid
			}
		};

		// Token: 0x040044F9 RID: 17657
		private static readonly Dictionary<int, CombatFilter> CombatFilterPerVersion = new Dictionary<int, CombatFilter>();

		// Token: 0x040044FA RID: 17658
		private bool m_dirty;

		// Token: 0x040044FB RID: 17659
		public int Version;

		// Token: 0x040044FC RID: 17660
		public bool Enabled = true;

		// Token: 0x040044FD RID: 17661
		public bool ShowTimestamps;

		// Token: 0x040044FE RID: 17662
		public float Opacity = 0.6f;

		// Token: 0x040044FF RID: 17663
		public List<ChatTabSettings> TabSettings;
	}
}
