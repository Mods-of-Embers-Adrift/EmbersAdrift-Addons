using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200060B RID: 1547
	[ExecuteInEditMode]
	public class ZoneSettings : MonoBehaviour
	{
		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x0600313F RID: 12607 RVA: 0x00061F11 File Offset: 0x00060111
		public ZoneSettingsProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x06003140 RID: 12608 RVA: 0x00061F19 File Offset: 0x00060119
		private void OnEnable()
		{
			ZoneSettings.SettingsProfile = this.m_profile;
		}

		// Token: 0x06003141 RID: 12609 RVA: 0x00061F26 File Offset: 0x00060126
		private void OnDisable()
		{
			if (ZoneSettings.SettingsProfile != null && ZoneSettings.SettingsProfile == this.m_profile)
			{
				ZoneSettings.SettingsProfile = null;
			}
		}

		// Token: 0x06003142 RID: 12610 RVA: 0x00061F4D File Offset: 0x0006014D
		private void Awake()
		{
			if (ZoneSettings.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			ZoneSettings.Instance = this;
		}

		// Token: 0x06003143 RID: 12611 RVA: 0x00061F6E File Offset: 0x0006016E
		private void OnDestroy()
		{
			if (ZoneSettings.Instance != null && ZoneSettings.Instance == this)
			{
				ZoneSettings.Instance = null;
			}
			if (ZoneSettings.SettingsProfile)
			{
				ZoneSettings.SettingsProfile.ResetTerrainTextures();
			}
		}

		// Token: 0x06003144 RID: 12612 RVA: 0x00061FA6 File Offset: 0x000601A6
		private IEnumerable GetProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<ZoneSettingsProfile>();
		}

		// Token: 0x04002F95 RID: 12181
		public static ZoneSettings Instance;

		// Token: 0x04002F96 RID: 12182
		public static ZoneSettingsProfile SettingsProfile;

		// Token: 0x04002F97 RID: 12183
		[SerializeField]
		private ZoneSettingsProfile m_profile;
	}
}
