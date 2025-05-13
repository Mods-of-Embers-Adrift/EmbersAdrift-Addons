using System;
using SoL.Game.Discovery;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000612 RID: 1554
	public class PlayerSpawn : TargetPosition
	{
		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06003164 RID: 12644 RVA: 0x0006211D File Offset: 0x0006031D
		public UniqueId DiscoveryId
		{
			get
			{
				if (!(this.m_discovery == null))
				{
					return this.m_discovery.DiscoveryId;
				}
				return UniqueId.Empty;
			}
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06003165 RID: 12645 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsDefault
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003166 RID: 12646 RVA: 0x0006213E File Offset: 0x0006033E
		private void Awake()
		{
			LocalZoneManager.RegisterPlayerSpawn(this);
		}

		// Token: 0x06003167 RID: 12647 RVA: 0x00062146 File Offset: 0x00060346
		private void OnDestroy()
		{
			LocalZoneManager.DeregisterPlayerSpawn(this);
		}

		// Token: 0x06003168 RID: 12648 RVA: 0x0006214E File Offset: 0x0006034E
		public Vector3 GetTargetPosition()
		{
			if (!(base.Target != null))
			{
				return base.gameObject.transform.position;
			}
			return base.Target.gameObject.transform.position;
		}

		// Token: 0x04002FC4 RID: 12228
		[SerializeField]
		private DiscoveryTrigger m_discovery;
	}
}
