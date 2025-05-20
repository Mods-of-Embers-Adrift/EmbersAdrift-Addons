using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x0200066A RID: 1642
	public class GlobalAshenController : MonoBehaviour
	{
		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06003309 RID: 13065 RVA: 0x00063231 File Offset: 0x00061431
		// (set) Token: 0x0600330A RID: 13066 RVA: 0x00063239 File Offset: 0x00061439
		public MinMaxFloatRange ChanceToAshen
		{
			get
			{
				return this.m_chanceToAshen;
			}
			set
			{
				this.m_chanceToAshen = value;
			}
		}

		// Token: 0x0600330B RID: 13067 RVA: 0x00063242 File Offset: 0x00061442
		private void Awake()
		{
			if (GlobalAshenController.Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			GlobalAshenController.Instance = this;
		}

		// Token: 0x0600330C RID: 13068 RVA: 0x00161F10 File Offset: 0x00160110
		public bool ShouldAshen()
		{
			float num = this.m_chanceToAshen.RandomWithinRange();
			return UnityEngine.Random.Range(0f, 1f) < num;
		}

		// Token: 0x04003147 RID: 12615
		public static GlobalAshenController Instance;

		// Token: 0x04003148 RID: 12616
		[SerializeField]
		private MinMaxFloatRange m_chanceToAshen = new MinMaxFloatRange(0.4f, 0.6f);
	}
}
