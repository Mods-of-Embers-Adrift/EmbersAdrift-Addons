using System;
using SoL.Game.EffectSystem;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200092C RID: 2348
	public class AlchemyUI : MonoBehaviour
	{
		// Token: 0x06004513 RID: 17683 RVA: 0x0006EA06 File Offset: 0x0006CC06
		protected virtual void Awake()
		{
			if (!GameManager.AllowAlchemy)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}

		// Token: 0x04004192 RID: 16786
		[SerializeField]
		protected AlchemyPowerLevel m_alchemyPowerLevel;
	}
}
