using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Networking.RPC
{
	// Token: 0x02000475 RID: 1141
	public abstract class RpcHandler : GameEntityComponent
	{
		// Token: 0x06001FE8 RID: 8168 RVA: 0x00057597 File Offset: 0x00055797
		protected static void RegisterCommandDelegate(string methodName, Type classType, RpcType rpcType, int cmdHash, RpcHandler.CommandDelegate func)
		{
			if (!RpcHandler.m_commandDelegates.ContainsKey(cmdHash))
			{
				RpcHandler.m_commandDelegates.Add(cmdHash, new RpcHandler.Invoker(methodName, classType, rpcType, func));
			}
		}

		// Token: 0x06001FE9 RID: 8169 RVA: 0x001210FC File Offset: 0x0011F2FC
		public static void HandleRpc(NetworkEntity source, NetworkEntity target, BitBuffer buffer)
		{
			int key = buffer.ReadInt();
			RpcHandler.Invoker invoker = null;
			if (RpcHandler.m_commandDelegates.TryGetValue(key, out invoker))
			{
				if (!RpcHandler.ValidateIncomingRpcPermissions(source, target, invoker.RpcType))
				{
					string text = (source == null) ? "NULL" : source.name;
					string text2 = (target == null) ? "NULL" : target.name;
					Debug.LogWarning(string.Format("Invalid incoming RPC permissions!  RpcType={0} for {1}.{2} where Target={3}, Source={4}.", new object[]
					{
						invoker.RpcType,
						invoker.ClassType,
						invoker.MethodName,
						text2,
						text
					}));
					return;
				}
				invoker.InvokerFunction(target, buffer);
			}
		}

		// Token: 0x06001FEA RID: 8170 RVA: 0x001211AC File Offset: 0x0011F3AC
		protected void SendCmdInternal(BitBuffer buffer, RpcType rpcType)
		{
			if (!this.ValidateOutgoingRpcPermissions(this.m_netEntity, rpcType))
			{
				Debug.LogWarning(string.Format("Invalid outgoing RPC permissions!  RpcType={0} from {1}", rpcType, this.m_netEntity.name));
				return;
			}
			CommandType commandType = rpcType.GetCommandTypeForRpcType();
			Peer[] array = null;
			if (commandType == CommandType.BroadcastAll && this.m_netEntity.UseProximity)
			{
				commandType = CommandType.BroadcastGroup;
				array = this.m_netEntity.GetAllObservers(true);
				if (array == null)
				{
					return;
				}
				if (array.Length == 0)
				{
					array.ReturnToPool();
					return;
				}
			}
			Packet packetFromBuffer = buffer.GetPacketFromBuffer(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = commandType;
			networkCommand.Channel = rpcType.GetChannelForRpcType();
			networkCommand.Source = this.m_netEntity.NetworkId.Peer;
			networkCommand.Target = this.m_netEntity.NetworkId.Peer;
			networkCommand.TargetGroup = array;
			networkCommand.Packet = packetFromBuffer;
			this.m_network.AddCommandToQueue(networkCommand);
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x00121290 File Offset: 0x0011F490
		private static bool ValidateIncomingRpcPermissions(NetworkEntity source, NetworkEntity target, RpcType rpcType)
		{
			if (NetworkManager.IsServer)
			{
				switch (rpcType)
				{
				case RpcType.ClientToServer:
					return source == target;
				case RpcType.AnyClientToServer:
					return source != null;
				case RpcType.ServerToClient:
				case RpcType.ServerBroadcast:
					return true;
				}
			}
			else
			{
				if (rpcType <= RpcType.AnyClientToServer)
				{
					return false;
				}
				if (rpcType - RpcType.ServerToClient <= 1)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x001212E0 File Offset: 0x0011F4E0
		private bool ValidateOutgoingRpcPermissions(NetworkEntity source, RpcType rpcType)
		{
			if (NetworkManager.IsServer)
			{
				if (rpcType <= RpcType.AnyClientToServer)
				{
					return false;
				}
				if (rpcType - RpcType.ServerToClient <= 1)
				{
					return true;
				}
			}
			else
			{
				switch (rpcType)
				{
				case RpcType.ClientToServer:
					return source == NetworkManager.MyEntity;
				case RpcType.AnyClientToServer:
					return NetworkManager.MyEntity != null;
				case RpcType.ServerToClient:
				case RpcType.ServerBroadcast:
					return false;
				}
			}
			return false;
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x000575BB File Offset: 0x000557BB
		public virtual void Init(INetworkManager network, NetworkEntity netEntity, BitBuffer buffer, float updateRate)
		{
			this.m_network = network;
			this.m_netEntity = netEntity;
			this.m_buffer = buffer;
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x000575D2 File Offset: 0x000557D2
		protected virtual void OnDestroy()
		{
			if (NullifyMemoryLeakSettings.CleanRpcHandlerMonoRefs)
			{
				this.m_network = null;
				this.m_netEntity = null;
				this.m_buffer = null;
			}
		}

		// Token: 0x04002559 RID: 9561
		public static readonly BitBuffer RpcBuffer = new BitBuffer(375);

		// Token: 0x0400255A RID: 9562
		protected static readonly Dictionary<int, RpcHandler.Invoker> m_commandDelegates = new Dictionary<int, RpcHandler.Invoker>();

		// Token: 0x0400255B RID: 9563
		protected INetworkManager m_network;

		// Token: 0x0400255C RID: 9564
		protected NetworkEntity m_netEntity;

		// Token: 0x0400255D RID: 9565
		protected BitBuffer m_buffer;

		// Token: 0x02000476 RID: 1142
		// (Invoke) Token: 0x06001FF2 RID: 8178
		protected delegate void CommandDelegate(NetworkEntity netEntity, BitBuffer buffer);

		// Token: 0x02000477 RID: 1143
		protected class Invoker
		{
			// Token: 0x06001FF5 RID: 8181 RVA: 0x0005760B File Offset: 0x0005580B
			public Invoker(string methodName, Type classType, RpcType rpcType, RpcHandler.CommandDelegate func)
			{
				this.MethodName = methodName;
				this.RpcType = rpcType;
				this.ClassType = classType;
				this.InvokerFunction = func;
			}

			// Token: 0x0400255E RID: 9566
			public readonly string MethodName;

			// Token: 0x0400255F RID: 9567
			public readonly Type ClassType;

			// Token: 0x04002560 RID: 9568
			public readonly RpcType RpcType;

			// Token: 0x04002561 RID: 9569
			public readonly RpcHandler.CommandDelegate InvokerFunction;
		}
	}
}
