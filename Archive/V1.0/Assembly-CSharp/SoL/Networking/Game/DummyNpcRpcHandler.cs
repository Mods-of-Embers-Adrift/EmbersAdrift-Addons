using System;
using NetStack.Serialization;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Networking.RPC;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.Game
{
	// Token: 0x0200040A RID: 1034
	public class DummyNpcRpcHandler : NetworkEntityRpcs
	{
		// Token: 0x06001B3B RID: 6971 RVA: 0x0010BB0C File Offset: 0x00109D0C
		private void GenerateColor_Internal()
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-2025853023);
			base.SendCmdInternal(rpcBuffer, RpcType.AnyClientToServer);
		}

		// Token: 0x06001B3C RID: 6972 RVA: 0x00055129 File Offset: 0x00053329
		private void Awake()
		{
			this.m_replicator = base.gameObject.GetComponent<DummyNpcSyncVarReplicator>();
		}

		// Token: 0x06001B3D RID: 6973 RVA: 0x0010BB44 File Offset: 0x00109D44
		private void Update()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			if (this.m_mousePresent && Input.GetMouseButtonDown(0) && Vector3.Distance(LocalPlayer.NetworkEntity.gameObject.transform.position, base.gameObject.transform.position) < 10f)
			{
				this.GenerateColor();
			}
		}

		// Token: 0x06001B3E RID: 6974 RVA: 0x0005513C File Offset: 0x0005333C
		private void OnMouseEnter()
		{
			this.m_mousePresent = true;
		}

		// Token: 0x06001B3F RID: 6975 RVA: 0x00055145 File Offset: 0x00053345
		private void OnMouseExit()
		{
			this.m_mousePresent = false;
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x0005514E File Offset: 0x0005334E
		[NetworkRPC(RpcType.AnyClientToServer)]
		public void GenerateColor()
		{
			if (this.m_netEntity.IsServer)
			{
				if (this.m_replicator != null)
				{
					this.m_replicator.Color.Value = ColorExtensions.GetRandomColor(false);
					return;
				}
			}
			else
			{
				this.GenerateColor_Internal();
			}
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x00055188 File Offset: 0x00053388
		static DummyNpcRpcHandler()
		{
			RpcHandler.RegisterCommandDelegate("GenerateColor", typeof(DummyNpcRpcHandler), RpcType.AnyClientToServer, -2025853023, new RpcHandler.CommandDelegate(DummyNpcRpcHandler.Invoke_GenerateColor));
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x000551B0 File Offset: 0x000533B0
		private static void Invoke_GenerateColor(NetworkEntity target, BitBuffer buffer)
		{
			((DummyNpcRpcHandler)target.RpcHandler).GenerateColor();
		}

		// Token: 0x04002270 RID: 8816
		private DummyNpcSyncVarReplicator m_replicator;

		// Token: 0x04002271 RID: 8817
		private bool m_mousePresent;

		// Token: 0x04002272 RID: 8818
		private const int kHash_GenerateColor = -2025853023;
	}
}
