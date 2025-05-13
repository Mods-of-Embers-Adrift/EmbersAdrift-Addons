using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Networking
{
	// Token: 0x020003C8 RID: 968
	[CreateAssetMenu(menuName = "SoL/Networking/Animator Param Collection", fileName = "NetworkedAnimatorParams")]
	public class NetworkedAnimatorParamCollection : ScriptableObject, ISerializationCallbackReceiver
	{
		// Token: 0x060019FD RID: 6653 RVA: 0x00107C2C File Offset: 0x00105E2C
		public NetworkedAnimatorParamSetting GetSettings(string key)
		{
			NetworkedAnimatorParamSetting result = null;
			this.m_settingsDict.TryGetValue(key, out result);
			return result;
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x00107C4C File Offset: 0x00105E4C
		public void OnAfterDeserialize()
		{
			this.m_settingsDict = new Dictionary<string, NetworkedAnimatorParamSetting>();
			for (int i = 0; i < this.m_settings.Length; i++)
			{
				if (!string.IsNullOrEmpty(this.m_settings[i].ParamName))
				{
					this.m_settingsDict.Add(this.m_settings[i].ParamName, this.m_settings[i]);
				}
			}
		}

		// Token: 0x04002123 RID: 8483
		[SerializeField]
		private NetworkedAnimatorParamSetting[] m_settings;

		// Token: 0x04002124 RID: 8484
		private Dictionary<string, NetworkedAnimatorParamSetting> m_settingsDict;
	}
}
