using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000671 RID: 1649
	[CreateAssetMenu(menuName = "SoL/Profiles/Call For Help")]
	public class CallForHelpScriptableProfile : ScriptableObject, ICallForHelpSettings
	{
		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x06003328 RID: 13096 RVA: 0x00063396 File Offset: 0x00061596
		public CallForHelpData InitialThreat
		{
			get
			{
				CallForHelpSettings settings = this.m_settings;
				if (settings == null)
				{
					return null;
				}
				return settings.InitialThreat;
			}
		}

		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06003329 RID: 13097 RVA: 0x000633A9 File Offset: 0x000615A9
		public CallForHelpData Death
		{
			get
			{
				CallForHelpSettings settings = this.m_settings;
				if (settings == null)
				{
					return null;
				}
				return settings.Death;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x0600332A RID: 13098 RVA: 0x000633BC File Offset: 0x000615BC
		public CallForHelpPeriodicData Periodic
		{
			get
			{
				CallForHelpSettings settings = this.m_settings;
				if (settings == null)
				{
					return null;
				}
				return settings.Periodic;
			}
		}

		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x0600332B RID: 13099 RVA: 0x000633CF File Offset: 0x000615CF
		public CallForHelpPeriodicData Fleeing
		{
			get
			{
				CallForHelpSettings settings = this.m_settings;
				if (settings == null)
				{
					return null;
				}
				return settings.Fleeing;
			}
		}

		// Token: 0x0600332C RID: 13100 RVA: 0x000633E2 File Offset: 0x000615E2
		private void OnValidate()
		{
			CallForHelpSettings settings = this.m_settings;
			if (settings == null)
			{
				return;
			}
			settings.NormalizeProbabilities();
		}

		// Token: 0x04003163 RID: 12643
		[SerializeField]
		private CallForHelpSettings m_settings;
	}
}
