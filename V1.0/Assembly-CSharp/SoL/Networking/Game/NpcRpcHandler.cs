using System;
using NetStack.Serialization;
using SoL.Game.EffectSystem;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.RPC;

namespace SoL.Networking.Game
{
	// Token: 0x0200040C RID: 1036
	public class NpcRpcHandler : NetworkEntityRpcs
	{
		// Token: 0x06001B49 RID: 6985 RVA: 0x0010BBDC File Offset: 0x00109DDC
		private void Server_ExecuteAbility_Internal(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(-1961646024);
			archetypeId.PackData(rpcBuffer);
			rpcBuffer.AddUInt(targetEntity.NetworkId.Value);
			rpcBuffer.AddByte(abilityLevel);
			base.SendCmdInternal(rpcBuffer, RpcType.ServerBroadcast);
		}

		// Token: 0x06001B4A RID: 6986 RVA: 0x0005524C File Offset: 0x0005344C
		[NetworkRPC(RpcType.ServerBroadcast)]
		public void Server_ExecuteAbility(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
			if (this.m_netEntity.IsServer)
			{
				this.Server_ExecuteAbility_Internal(archetypeId, targetEntity, abilityLevel);
				return;
			}
			base.GameEntity.SkillsController.Client_Execution_Begin(archetypeId, targetEntity, abilityLevel, AlchemyPowerLevel.None);
			base.GameEntity.SkillsController.Client_Execution_Complete(archetypeId, null);
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x0005528B File Offset: 0x0005348B
		static NpcRpcHandler()
		{
			RpcHandler.RegisterCommandDelegate("Server_ExecuteAbility", typeof(NpcRpcHandler), RpcType.ServerBroadcast, -1961646024, new RpcHandler.CommandDelegate(NpcRpcHandler.Invoke_Server_ExecuteAbility));
		}

		// Token: 0x06001B4C RID: 6988 RVA: 0x0010BC3C File Offset: 0x00109E3C
		private static void Invoke_Server_ExecuteAbility(NetworkEntity target, BitBuffer buffer)
		{
			UniqueId archetypeId = default(UniqueId);
			archetypeId.ReadData(buffer);
			uint id = buffer.ReadUInt();
			NetworkEntity netEntityForId = NetworkManager.EntityManager.GetNetEntityForId(id);
			byte abilityLevel = buffer.ReadByte();
			((NpcRpcHandler)target.RpcHandler).Server_ExecuteAbility(archetypeId, netEntityForId, abilityLevel);
		}

		// Token: 0x04002275 RID: 8821
		private const int kHash_Server_ExecuteAbility = -1961646024;
	}
}
