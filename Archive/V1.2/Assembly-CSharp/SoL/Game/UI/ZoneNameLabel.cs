using System;
using Cysharp.Text;
using SoL.Managers;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008F1 RID: 2289
	public class ZoneNameLabel : MonoBehaviour
	{
		// Token: 0x06004324 RID: 17188 RVA: 0x0006D43E File Offset: 0x0006B63E
		private void Awake()
		{
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06004325 RID: 17189 RVA: 0x0006D451 File Offset: 0x0006B651
		private void Start()
		{
			if (LocalZoneManager.ZoneRecord != null && LocalPlayer.GameEntity)
			{
				this.LocalPlayerOnLocalPlayerInitialized();
			}
		}

		// Token: 0x06004326 RID: 17190 RVA: 0x0006D46C File Offset: 0x0006B66C
		private void OnDestroy()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06004327 RID: 17191 RVA: 0x0006D47F File Offset: 0x0006B67F
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			if (this.m_label)
			{
				this.m_label.ZStringSetText(LocalPlayer.GetFormattedZoneName());
			}
		}

		// Token: 0x04003FD9 RID: 16345
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}
