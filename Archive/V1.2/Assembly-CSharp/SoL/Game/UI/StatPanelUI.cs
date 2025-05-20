using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008DA RID: 2266
	[Obsolete]
	public class StatPanelUI : MonoBehaviour
	{
		// Token: 0x06004242 RID: 16962 RVA: 0x00192038 File Offset: 0x00190238
		public void Init()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_element, this.m_content, false);
			this.m_stats = gameObject.GetComponent<StatPanelElementUI>();
			this.m_stats.InitStats();
			this.m_elements = new Dictionary<EffectChannelType, StatPanelElementUI>();
			for (int i = 0; i < EffectChannelTypeExtensions.EffectChannelTypes.Length; i++)
			{
				EffectChannelType effectChannelType = EffectChannelTypeExtensions.EffectChannelTypes[i];
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_element, this.m_content, false);
				StatPanelElementUI component = gameObject.GetComponent<StatPanelElementUI>();
				this.m_elements.Add(effectChannelType, component);
				component.InitMod(effectChannelType);
			}
		}

		// Token: 0x06004243 RID: 16963 RVA: 0x001920C4 File Offset: 0x001902C4
		public void UpdatePanels()
		{
			if (this.m_elements == null)
			{
				return;
			}
			foreach (KeyValuePair<EffectChannelType, StatPanelElementUI> keyValuePair in this.m_elements)
			{
				keyValuePair.Value.RefreshPanel();
			}
		}

		// Token: 0x04003F52 RID: 16210
		[SerializeField]
		private GameObject m_element;

		// Token: 0x04003F53 RID: 16211
		[SerializeField]
		private RectTransform m_content;

		// Token: 0x04003F54 RID: 16212
		private Dictionary<EffectChannelType, StatPanelElementUI> m_elements;

		// Token: 0x04003F55 RID: 16213
		private StatPanelElementUI m_stats;
	}
}
