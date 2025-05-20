using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009BA RID: 2490
	public class SocialServerStatusUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004B45 RID: 19269 RVA: 0x00072ED5 File Offset: 0x000710D5
		private void Start()
		{
			base.InvokeRepeating("UpdateStatus", 0f, 5f);
		}

		// Token: 0x06004B46 RID: 19270 RVA: 0x00072EEC File Offset: 0x000710EC
		private void OnDestroy()
		{
			base.CancelInvoke("UpdateStatus");
		}

		// Token: 0x06004B47 RID: 19271 RVA: 0x00072EF9 File Offset: 0x000710F9
		private void UpdateStatus()
		{
			this.m_indicator.color = (SolServerConnectionManager.IsOnline ? this.m_onlineColor : this.m_offlineColor);
		}

		// Token: 0x17001087 RID: 4231
		// (get) Token: 0x06004B48 RID: 19272 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x06004B49 RID: 19273 RVA: 0x001B864C File Offset: 0x001B684C
		private ITooltipParameter GetTooltipParameter()
		{
			string arg = ZString.Format<int>("<indent={0}px>", GlobalSettings.Values.UI.SocialServerStatsIndent);
			string txt = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				string arg2 = NetworkManager.MyPeer.IsSet ? NetworkManager.MyPeer.RoundTripTime.ToString() : "Unknown";
				utf16ValueStringBuilder.AppendFormat<string, string>("Ping:{0}{1} ms</indent>\n", arg, arg2);
				utf16ValueStringBuilder.AppendFormat<string, string>("Social:{0}{1}</indent>\n", arg, SolServerConnectionManager.IsOnline ? "Connected" : "Disconnected");
				utf16ValueStringBuilder.AppendFormat<string, string>("Zone:{0}{1}</indent>\n", arg, LocalZoneManager.ZoneRecord.DisplayName);
				utf16ValueStringBuilder.AppendFormat<string, int>("Instance:{0}{1}</indent>\n", arg, LocalZoneManager.ZoneRecord.InstanceId);
				txt = utf16ValueStringBuilder.ToString();
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17001088 RID: 4232
		// (get) Token: 0x06004B4A RID: 19274 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001089 RID: 4233
		// (get) Token: 0x06004B4B RID: 19275 RVA: 0x00072F1B File Offset: 0x0007111B
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700108A RID: 4234
		// (get) Token: 0x06004B4C RID: 19276 RVA: 0x00072F29 File Offset: 0x00071129
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040045D8 RID: 17880
		private const float kCadence = 5f;

		// Token: 0x040045D9 RID: 17881
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040045DA RID: 17882
		[SerializeField]
		private Image m_indicator;

		// Token: 0x040045DB RID: 17883
		[SerializeField]
		private Color m_onlineColor = Color.green;

		// Token: 0x040045DC RID: 17884
		[SerializeField]
		private Color m_offlineColor = Color.red;
	}
}
