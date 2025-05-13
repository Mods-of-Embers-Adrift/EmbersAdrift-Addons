using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x0200065C RID: 1628
	public class BaseState : GameEntityComponent, IState
	{
		// Token: 0x140000AC RID: 172
		// (add) Token: 0x060032A8 RID: 12968 RVA: 0x00161454 File Offset: 0x0015F654
		// (remove) Token: 0x060032A9 RID: 12969 RVA: 0x0016148C File Offset: 0x0015F68C
		public event Action<byte> StateChanged;

		// Token: 0x140000AD RID: 173
		// (add) Token: 0x060032AA RID: 12970 RVA: 0x001614C4 File Offset: 0x0015F6C4
		// (remove) Token: 0x060032AB RID: 12971 RVA: 0x001614FC File Offset: 0x0015F6FC
		public event Action<byte> StateChangedFromClient;

		// Token: 0x17000ACB RID: 2763
		// (get) Token: 0x060032AC RID: 12972 RVA: 0x00062E51 File Offset: 0x00061051
		// (set) Token: 0x060032AD RID: 12973 RVA: 0x00161534 File Offset: 0x0015F734
		public byte CurrentState
		{
			get
			{
				return this.m_currentState;
			}
			protected set
			{
				if (this.m_currentState != value)
				{
					this.m_currentState = value;
					if (GameManager.IsServer && StateReplicator.Instance)
					{
						StateReplicator.Instance.SetServerState(this);
					}
					Action<byte> stateChanged = this.StateChanged;
					if (stateChanged != null)
					{
						stateChanged(this.m_currentState);
					}
					if (this.m_clientChanged)
					{
						Action<byte> stateChangedFromClient = this.StateChangedFromClient;
						if (stateChangedFromClient != null)
						{
							stateChangedFromClient(this.m_currentState);
						}
					}
					this.StateChangedInternal();
				}
			}
		}

		// Token: 0x17000ACC RID: 2764
		// (get) Token: 0x060032AE RID: 12974 RVA: 0x00062E59 File Offset: 0x00061059
		public virtual int MaxState
		{
			get
			{
				return this.m_maxState;
			}
		}

		// Token: 0x060032AF RID: 12975 RVA: 0x00062E61 File Offset: 0x00061061
		protected virtual void Awake()
		{
			StateReplicator.Register(this);
		}

		// Token: 0x060032B0 RID: 12976 RVA: 0x00062E69 File Offset: 0x00061069
		protected virtual void OnDestroy()
		{
			StateReplicator.Unregister(this);
		}

		// Token: 0x060032B1 RID: 12977 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void StateChangedInternal()
		{
		}

		// Token: 0x060032B2 RID: 12978 RVA: 0x00062E71 File Offset: 0x00061071
		public void ServerSetState(byte value)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			this.CurrentState = value;
		}

		// Token: 0x060032B3 RID: 12979 RVA: 0x001615AC File Offset: 0x0015F7AC
		protected void ProgressState()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			int num = (int)((this.CurrentState == byte.MaxValue || (int)(this.CurrentState + 1) > this.MaxState) ? 0 : (this.CurrentState + 1));
			this.CurrentState = (byte)num;
		}

		// Token: 0x17000ACD RID: 2765
		// (get) Token: 0x060032B4 RID: 12980 RVA: 0x00062E82 File Offset: 0x00061082
		protected int Key
		{
			get
			{
				return this.m_key;
			}
		}

		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x060032B5 RID: 12981 RVA: 0x00062E82 File Offset: 0x00061082
		// (set) Token: 0x060032B6 RID: 12982 RVA: 0x00062E8A File Offset: 0x0006108A
		int IState.Key
		{
			get
			{
				return this.m_key;
			}
			set
			{
				this.m_key = value;
			}
		}

		// Token: 0x060032B7 RID: 12983 RVA: 0x00062E93 File Offset: 0x00061093
		byte IState.GetState()
		{
			return this.CurrentState;
		}

		// Token: 0x060032B8 RID: 12984 RVA: 0x00062E9B File Offset: 0x0006109B
		void IState.SetState(byte value)
		{
			this.CurrentState = value;
		}

		// Token: 0x060032BB RID: 12987 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IState.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003112 RID: 12562
		public static int kStateKey = Animator.StringToHash("State");

		// Token: 0x04003115 RID: 12565
		private byte m_currentState;

		// Token: 0x04003116 RID: 12566
		protected bool m_clientChanged;

		// Token: 0x04003117 RID: 12567
		[Range(1f, 255f)]
		[SerializeField]
		private int m_maxState = 1;

		// Token: 0x04003118 RID: 12568
		private int m_key;
	}
}
