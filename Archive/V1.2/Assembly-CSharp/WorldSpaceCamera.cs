using System;
using SoL.Game;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class WorldSpaceCamera : MonoBehaviour
{
	// Token: 0x0600000D RID: 13 RVA: 0x000877E8 File Offset: 0x000859E8
	private void Start()
	{
		if (this.m_camera != null && ZoneSettings.Instance != null && ZoneSettings.Instance.Profile != null)
		{
			this.m_camera.fieldOfView = ZoneSettings.Instance.Profile.FieldOfView.GetValue(this.m_camera.fieldOfView);
		}
	}

	// Token: 0x04000007 RID: 7
	[SerializeField]
	private Camera m_camera;
}
