using System;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BC5 RID: 3013
	public class InfluenceRegister : MonoBehaviour
	{
		// Token: 0x06005D24 RID: 23844 RVA: 0x0007E9F1 File Offset: 0x0007CBF1
		private void Start()
		{
			if (this.m_spatial && this.m_networkEntity)
			{
				this.m_spatial.RegisterEntity(this.m_networkEntity);
			}
		}

		// Token: 0x06005D25 RID: 23845 RVA: 0x0007EA1E File Offset: 0x0007CC1E
		private void OnDestroy()
		{
			if (this.m_spatial && this.m_networkEntity)
			{
				this.m_spatial.UnregisterEntity(this.m_networkEntity);
			}
		}

		// Token: 0x04005091 RID: 20625
		[SerializeField]
		private SpatialManager m_spatial;

		// Token: 0x04005092 RID: 20626
		[SerializeField]
		private NetworkEntity m_networkEntity;
	}
}
