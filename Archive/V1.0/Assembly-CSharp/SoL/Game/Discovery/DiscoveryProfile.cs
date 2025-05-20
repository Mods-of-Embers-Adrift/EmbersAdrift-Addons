using System;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA6 RID: 3238
	[CreateAssetMenu(menuName = "SoL/Profiles/Discovery")]
	public class DiscoveryProfile : BaseArchetype, IEquatable<DiscoveryProfile>
	{
		// Token: 0x1700176E RID: 5998
		// (get) Token: 0x06006220 RID: 25120 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showCategory
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700176F RID: 5999
		// (get) Token: 0x06006221 RID: 25121 RVA: 0x0008220F File Offset: 0x0008040F
		protected virtual DiscoveryCategory DiscoveryCategory
		{
			get
			{
				return this.m_category;
			}
		}

		// Token: 0x06006222 RID: 25122 RVA: 0x00203878 File Offset: 0x00201A78
		public void ClientDiscovered()
		{
			ClientGameManager.UIManager.PlayRandomClip(GlobalSettings.Values.Audio.GenericDiscoveryClipCollection, null);
			string text = "You discovered " + this.DisplayName + "!";
			switch (this.DiscoveryCategory)
			{
			case DiscoveryCategory.Respawn:
				text = "You discovered a Hallow (respawn point)!";
				break;
			case DiscoveryCategory.Area:
				text = (string.IsNullOrEmpty(this.DisplayName) ? "You discovered a new region!" : ("You discovered a new region (" + this.DisplayName + ")!"));
				break;
			case DiscoveryCategory.Monolith:
				text = "You discovered an Ember Monolith!";
				break;
			}
			if (!string.IsNullOrEmpty(this.m_customDiscoveryMessage))
			{
				text = this.m_customDiscoveryMessage;
			}
			if (this.m_centerScreenAnnouncement)
			{
				ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
				{
					Title = this.DisplayName,
					Text = text,
					TimeShown = 5f,
					ShowDelay = 0f,
					MessageType = MessageType.None,
					SourceId = new UniqueId?(base.Id)
				});
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Discovery, text);
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x001E015C File Offset: 0x001DE35C
		public bool Equals(DiscoveryProfile other)
		{
			return other != null && (this == other || base.Id.Value == other.Id.Value);
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x00082217 File Offset: 0x00080417
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((DiscoveryProfile)obj)));
		}

		// Token: 0x06006225 RID: 25125 RVA: 0x001E0198 File Offset: 0x001DE398
		public override int GetHashCode()
		{
			return base.Id.Value.GetHashCode();
		}

		// Token: 0x06006226 RID: 25126 RVA: 0x00082245 File Offset: 0x00080445
		public static bool operator ==(DiscoveryProfile left, DiscoveryProfile right)
		{
			return left && left.Equals(right);
		}

		// Token: 0x06006227 RID: 25127 RVA: 0x00082258 File Offset: 0x00080458
		public static bool operator !=(DiscoveryProfile left, DiscoveryProfile right)
		{
			return !(left == right);
		}

		// Token: 0x040055BF RID: 21951
		[SerializeField]
		private DiscoveryCategory m_category;

		// Token: 0x040055C0 RID: 21952
		[SerializeField]
		private bool m_centerScreenAnnouncement;

		// Token: 0x040055C1 RID: 21953
		[FormerlySerializedAs("m_discoveryMessage")]
		[SerializeField]
		private string m_customDiscoveryMessage = string.Empty;
	}
}
