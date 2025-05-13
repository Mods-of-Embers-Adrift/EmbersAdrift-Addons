using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.RPC;
using UnityEngine;

namespace SoL.Networking.Replication
{
	// Token: 0x0200047C RID: 1148
	public sealed class AnimatorReplicator : RpcHandler
	{
		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x0600200F RID: 8207 RVA: 0x0005773E File Offset: 0x0005593E
		public Animator Animator
		{
			get
			{
				return this.m_animator;
			}
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x00057746 File Offset: 0x00055946
		public bool GetParameterAutoSend(int index)
		{
			return (this.m_syncBits & 1 << index) != 0;
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x001215F8 File Offset: 0x0011F7F8
		private void SetTrigger_Internal(string key)
		{
			BitBuffer rpcBuffer = RpcHandler.RpcBuffer;
			rpcBuffer.AddHeader(this.m_netEntity, OpCodes.RPC, true);
			rpcBuffer.AddInt(1171008415);
			rpcBuffer.AddString(key);
			base.SendCmdInternal(rpcBuffer, RpcType.ClientToServer);
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002012 RID: 8210 RVA: 0x00057758 File Offset: 0x00055958
		public IReplicator Sync
		{
			get
			{
				return this.m_sync;
			}
		}

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x06002013 RID: 8211 RVA: 0x00057760 File Offset: 0x00055960
		public IReplicator State
		{
			get
			{
				return this.m_state;
			}
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x00121638 File Offset: 0x0011F838
		public override void Init(INetworkManager network, NetworkEntity netEntity, BitBuffer buffer, float updateRate)
		{
			if (this.m_initialized)
			{
				return;
			}
			base.Init(network, netEntity, buffer, updateRate);
			this.m_sync = new AnimatorReplicator.InternalReplicator(this);
			this.m_state = new AnimatorReplicator.InternalReplicator(this);
			AnimatorControllerParameter[] parameters = this.m_animator.parameters;
			int i = 0;
			while (i < parameters.Length)
			{
				AnimatorParameter animatorParameter = null;
				AnimatorControllerParameterType type = parameters[i].type;
				if (type == AnimatorControllerParameterType.Float)
				{
					goto IL_70;
				}
				if (type - AnimatorControllerParameterType.Int > 1)
				{
					if (type == AnimatorControllerParameterType.Trigger)
					{
						goto IL_70;
					}
				}
				else
				{
					animatorParameter = this.m_sync.AddParameter(parameters[i], this.GetParameterAutoSend(i));
				}
				IL_86:
				this.m_animatorParamLookup.Add(animatorParameter.Name, animatorParameter);
				i++;
				continue;
				IL_70:
				animatorParameter = this.m_state.AddParameter(parameters[i], this.GetParameterAutoSend(i));
				goto IL_86;
			}
			this.m_initialized = true;
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x001216F0 File Offset: 0x0011F8F0
		private void Update()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			foreach (KeyValuePair<string, AnimatorParameter> keyValuePair in this.m_animatorParamLookup)
			{
				keyValuePair.Value.ExternalUpdate();
			}
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x00121750 File Offset: 0x0011F950
		protected override void OnDestroy()
		{
			if (NullifyMemoryLeakSettings.CleanRpcHandlerMonoRefs)
			{
				AnimatorReplicator.InternalReplicator state = this.m_state;
				if (state != null)
				{
					state.ClearMonoReferences();
				}
				AnimatorReplicator.InternalReplicator sync = this.m_sync;
				if (sync != null)
				{
					sync.ClearMonoReferences();
				}
				Dictionary<string, AnimatorParameter> animatorParamLookup = this.m_animatorParamLookup;
				if (animatorParamLookup != null)
				{
					animatorParamLookup.Clear();
				}
			}
			base.OnDestroy();
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x001217A0 File Offset: 0x0011F9A0
		[NetworkRPC(RpcType.ClientToServer)]
		public void SetTrigger(string key)
		{
			if (!this.m_netEntity.IsLocal)
			{
				bool isServer = this.m_netEntity.IsServer;
			}
			AnimatorParameter animatorParameter = null;
			if (this.m_animatorParamLookup.TryGetValue(key, out animatorParameter))
			{
				this.m_animator.SetTrigger(animatorParameter.NameHash);
			}
		}

		// Token: 0x06002018 RID: 8216 RVA: 0x001217EC File Offset: 0x0011F9EC
		public void SetFloat(string key, float value)
		{
			AnimatorParameter animatorParameter = null;
			if (this.m_animatorParamLookup.TryGetValue(key, out animatorParameter))
			{
				animatorParameter.FloatValue = value;
			}
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x00121814 File Offset: 0x0011FA14
		public void SetInteger(string key, int value)
		{
			AnimatorParameter animatorParameter = null;
			if (this.m_animatorParamLookup.TryGetValue(key, out animatorParameter))
			{
				animatorParameter.IntValue = value;
			}
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x0012183C File Offset: 0x0011FA3C
		public void SetBool(string key, bool value)
		{
			AnimatorParameter animatorParameter = null;
			if (this.m_animatorParamLookup.TryGetValue(key, out animatorParameter))
			{
				animatorParameter.BoolValue = value;
			}
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x00057768 File Offset: 0x00055968
		static AnimatorReplicator()
		{
			RpcHandler.RegisterCommandDelegate("SetTrigger", typeof(AnimatorReplicator), RpcType.ClientToServer, 1171008415, new RpcHandler.CommandDelegate(AnimatorReplicator.Invoke_SetTrigger));
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x00121864 File Offset: 0x0011FA64
		private static void Invoke_SetTrigger(NetworkEntity target, BitBuffer buffer)
		{
			string trigger = buffer.ReadString();
			((AnimatorReplicator)target.RpcHandler).SetTrigger(trigger);
		}

		// Token: 0x04002575 RID: 9589
		[SerializeField]
		private int m_syncBits;

		// Token: 0x04002576 RID: 9590
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04002577 RID: 9591
		private readonly Dictionary<string, AnimatorParameter> m_animatorParamLookup = new Dictionary<string, AnimatorParameter>();

		// Token: 0x04002578 RID: 9592
		private AnimatorReplicator.InternalReplicator m_sync;

		// Token: 0x04002579 RID: 9593
		private AnimatorReplicator.InternalReplicator m_state;

		// Token: 0x0400257A RID: 9594
		private bool m_initialized;

		// Token: 0x0400257B RID: 9595
		private const int kHash_SetTrigger = 1171008415;

		// Token: 0x0200047D RID: 1149
		private class InternalReplicator : IReplicator
		{
			// Token: 0x170006B1 RID: 1713
			// (get) Token: 0x0600201E RID: 8222 RVA: 0x00053500 File Offset: 0x00051700
			public ReplicatorTypes Type
			{
				get
				{
					return ReplicatorTypes.Animator;
				}
			}

			// Token: 0x170006B2 RID: 1714
			// (get) Token: 0x0600201F RID: 8223 RVA: 0x000577A3 File Offset: 0x000559A3
			// (set) Token: 0x06002020 RID: 8224 RVA: 0x000577AB File Offset: 0x000559AB
			public bool Dirty { get; private set; }

			// Token: 0x06002021 RID: 8225 RVA: 0x000577B4 File Offset: 0x000559B4
			public InternalReplicator(AnimatorReplicator replicator)
			{
				this.m_replicator = replicator;
			}

			// Token: 0x06002022 RID: 8226 RVA: 0x0012188C File Offset: 0x0011FA8C
			public AnimatorParameter AddParameter(AnimatorControllerParameter parameter, bool shouldSync)
			{
				AnimatorParameter animatorParameter = new AnimatorParameter(this.m_replicator.m_netEntity, this.m_replicator.m_animator, parameter, this.m_params.Count, shouldSync);
				this.m_params.Add(animatorParameter.Name, animatorParameter);
				return animatorParameter;
			}

			// Token: 0x06002023 RID: 8227 RVA: 0x0004475B File Offset: 0x0004295B
			public void Init(int index, INetworkManager network, NetworkEntity netEntity)
			{
			}

			// Token: 0x06002024 RID: 8228 RVA: 0x001218D8 File Offset: 0x0011FAD8
			public void ClearMonoReferences()
			{
				for (int i = 0; i < this.m_params.Count; i++)
				{
					AnimatorParameter animatorParameter = this.m_params[i];
					if (animatorParameter != null)
					{
						animatorParameter.ClearMonoReferences();
					}
				}
				this.m_params.Clear();
				this.m_replicator = null;
			}

			// Token: 0x06002025 RID: 8229 RVA: 0x00121924 File Offset: 0x0011FB24
			public void SetDirtyFlags(DateTime timestamp)
			{
				if (this.m_replicator.m_netEntity == null || (!this.m_replicator.m_netEntity.IsLocal && !this.m_replicator.m_netEntity.IsServer))
				{
					this.Dirty = false;
					return;
				}
				this.m_dirtyBits = this.GetDirtyBits(timestamp);
				this.Dirty = (this.m_dirtyBits != 0);
			}

			// Token: 0x06002026 RID: 8230 RVA: 0x0012198C File Offset: 0x0011FB8C
			private int GetDirtyBits(DateTime timestamp)
			{
				int num = 0;
				for (int i = 0; i < this.m_params.Count; i++)
				{
					AnimatorParameter animatorParameter = this.m_params[i];
					animatorParameter.SetDirtyFlags(timestamp);
					if (animatorParameter.Dirty)
					{
						num |= this.m_params[i].BitFlag;
					}
				}
				return num;
			}

			// Token: 0x06002027 RID: 8231 RVA: 0x001219E0 File Offset: 0x0011FBE0
			public BitBuffer PackData(BitBuffer outBuffer)
			{
				outBuffer.AddInt(this.m_dirtyBits);
				for (int i = 0; i < this.m_params.Count; i++)
				{
					if (this.m_params[i].Dirty)
					{
						outBuffer.AddAnimatorParameter(this.m_params[i]);
					}
				}
				return outBuffer;
			}

			// Token: 0x06002028 RID: 8232 RVA: 0x00121A38 File Offset: 0x0011FC38
			public BitBuffer ReadData(BitBuffer inBuffer)
			{
				int num = inBuffer.ReadInt();
				if (num != 0)
				{
					for (int i = 0; i < this.m_params.Count; i++)
					{
						if ((num & this.m_params[i].BitFlag) == this.m_params[i].BitFlag)
						{
							this.m_params[i].ReadData(inBuffer);
						}
					}
				}
				return inBuffer;
			}

			// Token: 0x06002029 RID: 8233 RVA: 0x00121AA0 File Offset: 0x0011FCA0
			public BitBuffer PackInitialData(BitBuffer outBuffer)
			{
				for (int i = 0; i < this.m_params.Count; i++)
				{
					outBuffer.AddInitialAnimatorParameter(this.m_params[i]);
				}
				return outBuffer;
			}

			// Token: 0x0600202A RID: 8234 RVA: 0x00121AD8 File Offset: 0x0011FCD8
			public BitBuffer ReadInitialData(BitBuffer inBuffer)
			{
				for (int i = 0; i < this.m_params.Count; i++)
				{
					this.m_params[i].ReadData(inBuffer);
				}
				return inBuffer;
			}

			// Token: 0x0400257C RID: 9596
			private readonly DictionaryList<string, AnimatorParameter> m_params = new DictionaryList<string, AnimatorParameter>(false);

			// Token: 0x0400257D RID: 9597
			private AnimatorReplicator m_replicator;

			// Token: 0x0400257E RID: 9598
			private int m_dirtyBits;
		}
	}
}
