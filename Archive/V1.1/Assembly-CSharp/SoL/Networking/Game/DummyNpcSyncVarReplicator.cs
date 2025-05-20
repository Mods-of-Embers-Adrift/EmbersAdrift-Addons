using System;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Networking.Game
{
	// Token: 0x0200040B RID: 1035
	public class DummyNpcSyncVarReplicator : SyncVarReplicator
	{
		// Token: 0x06001B44 RID: 6980 RVA: 0x000551CA File Offset: 0x000533CA
		private void Awake()
		{
			this.m_renderer = base.gameObject.GetComponent<Renderer>();
			this.Color.Changed += this.ColorOnChanged;
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x000551F4 File Offset: 0x000533F4
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.Color.Changed -= this.ColorOnChanged;
		}

		// Token: 0x06001B46 RID: 6982 RVA: 0x00055213 File Offset: 0x00053413
		private void ColorOnChanged(Color obj)
		{
			if (this.m_renderer != null)
			{
				this.m_renderer.material.color = obj;
			}
		}

		// Token: 0x06001B47 RID: 6983 RVA: 0x0010BBA0 File Offset: 0x00109DA0
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.Color);
			this.Color.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002273 RID: 8819
		public readonly SynchronizedColor Color = new SynchronizedColor(UnityEngine.Color.white);

		// Token: 0x04002274 RID: 8820
		private Renderer m_renderer;
	}
}
