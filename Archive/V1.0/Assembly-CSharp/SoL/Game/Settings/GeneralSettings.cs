using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000727 RID: 1831
	[Serializable]
	public class GeneralSettings
	{
		// Token: 0x060036FD RID: 14077 RVA: 0x00065A37 File Offset: 0x00063C37
		public TutorialPopupOptions GetLoginNotification()
		{
			if (this.m_loginNotificationOptionsOverride == null)
			{
				return this.m_loginNotificationOptions;
			}
			return this.m_loginNotificationOptionsOverride.Value;
		}

		// Token: 0x060036FE RID: 14078 RVA: 0x00065A58 File Offset: 0x00063C58
		public void SetLoginNotificationOverride(TutorialPopupOptions opts)
		{
			this.m_loginNotificationOptionsOverride = new TutorialPopupOptions?(opts);
		}

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x060036FF RID: 14079 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x0400352D RID: 13613
		public List<ZoneId> AvailableZones;

		// Token: 0x0400352E RID: 13614
		public float InteractiveClickDragTimeThreshold = 0.3f;

		// Token: 0x0400352F RID: 13615
		[SerializeField]
		private TutorialPopupOptions m_loginNotificationOptions;

		// Token: 0x04003530 RID: 13616
		public float GlobalInteractionDistance = 3f;

		// Token: 0x04003531 RID: 13617
		public float VerticalDistanceToIgnoreForDistanceChecks = 3f;

		// Token: 0x04003532 RID: 13618
		public LayerMask LineOfSightExclusionLayerMask;

		// Token: 0x04003533 RID: 13619
		public LayerMask MouseClickTargetLayersToInclude;

		// Token: 0x04003534 RID: 13620
		public InteractionSettingsBase GlobalInteractionSettings;

		// Token: 0x04003535 RID: 13621
		private TutorialPopupOptions? m_loginNotificationOptionsOverride;
	}
}
