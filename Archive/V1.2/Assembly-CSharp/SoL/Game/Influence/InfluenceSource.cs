using System;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BC6 RID: 3014
	public class InfluenceSource : GameEntityComponent, IInfluenceSource
	{
		// Token: 0x06005D27 RID: 23847 RVA: 0x0007EA4B File Offset: 0x0007CC4B
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.InfluenceSource = this;
			}
		}

		// Token: 0x170015FA RID: 5626
		// (get) Token: 0x06005D28 RID: 23848 RVA: 0x0007EA67 File Offset: 0x0007CC67
		// (set) Token: 0x06005D29 RID: 23849 RVA: 0x0007EA6F File Offset: 0x0007CC6F
		InfluenceProfile IInfluenceSource.InfluenceProfile
		{
			get
			{
				return this.m_influenceProfile;
			}
			set
			{
				this.m_influenceProfile = value;
			}
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x0007EA78 File Offset: 0x0007CC78
		float IInfluenceSource.GetInfluence(Vector3 samplePosition, InfluenceFlags flags)
		{
			if (!(this.m_influenceProfile == null) && base.enabled)
			{
				return this.m_influenceProfile.SampleInfluenceAtPointFromSource(base.gameObject.transform.position, samplePosition, flags);
			}
			return 0f;
		}

		// Token: 0x04005093 RID: 20627
		[SerializeField]
		private InfluenceProfile m_influenceProfile;
	}
}
