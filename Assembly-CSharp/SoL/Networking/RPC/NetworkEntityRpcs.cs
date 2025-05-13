using System;
using NetStack.Serialization;
using SoL.Game.Animation;
using SoL.Networking.Managers;
using SoL.Networking.Objects;

namespace SoL.Networking.RPC
{
	// Token: 0x02000473 RID: 1139
	public class NetworkEntityRpcs : RpcHandler
	{
		// Token: 0x06001FDF RID: 8159 RVA: 0x00120F70 File Offset: 0x0011F170
		private void Server_Execute_AutoAttack_Internal(NetworkEntity targetEntity, AnimationFlags animFlags)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1541248640);
			rpcBuffer.AddUInt(targetEntity.NetworkId.Value);
			rpcBuffer.AddEnum(animFlags);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x0005751A File Offset: 0x0005571A
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_Execute_AutoAttack(NetworkEntity targetEntity, AnimationFlags animFlags)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_Execute_AutoAttack_Internal(targetEntity, animFlags);
				return;
			}
			base.GameEntity.SkillsController.Client_AutoAttack(targetEntity, animFlags);
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x00120FC4 File Offset: 0x0011F1C4
		private void UpdateLastingEffectTriggerCount_Internal(UniqueId instanceId, byte newCount)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(2100708741);
			instanceId.PackData(rpcBuffer);
			rpcBuffer.AddByte(newCount);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x00057544 File Offset: 0x00055744
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void UpdateLastingEffectTriggerCount(UniqueId instanceId, byte newCount)
		{
			if (this.m_netEntity.IsServer)
			{
				this.UpdateLastingEffectTriggerCount_Internal(instanceId, newCount);
				return;
			}
			if (base.GameEntity.Vitals)
			{
				base.GameEntity.Vitals.UpdateLastingEffectTriggerCount(instanceId, newCount);
			}
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x0012100C File Offset: 0x0011F20C
		static NetworkEntityRpcs()
		{
			RpcHandler.RegisterCommandDelegate("Server_Execute_AutoAttack", typeof(NetworkEntityRpcs), RpcType.ServerBroadcast, -1541248640, new RpcHandler.CommandDelegate(NetworkEntityRpcs.Invoke_Server_Execute_AutoAttack));
			RpcHandler.RegisterCommandDelegate("UpdateLastingEffectTriggerCount", typeof(NetworkEntityRpcs), RpcType.ServerBroadcast, 2100708741, new RpcHandler.CommandDelegate(NetworkEntityRpcs.Invoke_UpdateLastingEffectTriggerCount));
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x00121068 File Offset: 0x0011F268
		private static void Invoke_Server_Execute_AutoAttack(NetworkEntity target, BitBuffer buffer)
		{
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			AnimationFlags animFlags = buffer.ReadEnum<AnimationFlags>();
			((NetworkEntityRpcs)target.RpcHandler).Server_Execute_AutoAttack(netEntityForId, animFlags);
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x001210A4 File Offset: 0x0011F2A4
		private static void Invoke_UpdateLastingEffectTriggerCount(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId instanceId = default(UniqueId);
			instanceId.ReadData(buffer);
			byte newCount = buffer.ReadByte();
			((NetworkEntityRpcs)target.RpcHandler).UpdateLastingEffectTriggerCount(instanceId, newCount);
		}

		// Token: 0x04002556 RID: 9558
		private const int kHash_Server_Execute_AutoAttack = -1541248640;

		// Token: 0x04002557 RID: 9559
		private const int kHash_UpdateLastingEffectTriggerCount = 2100708741;
	}
}
