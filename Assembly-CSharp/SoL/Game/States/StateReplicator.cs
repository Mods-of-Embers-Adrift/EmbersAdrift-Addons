using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Managers;
using SoL.Networking.Replication;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000662 RID: 1634
	public class StateReplicator : SyncVarReplicator
	{
		// Token: 0x060032DC RID: 13020 RVA: 0x00062FD5 File Offset: 0x000611D5
		public static void Register(IState state)
		{
			if (StateReplicator.Instance)
			{
				StateReplicator.Instance.RegisterState(state);
				return;
			}
			StateReplicator.m_pendingRegistration.Add(state);
		}

		// Token: 0x060032DD RID: 13021 RVA: 0x00062FFA File Offset: 0x000611FA
		public static void Unregister(IState state)
		{
			if (StateReplicator.Instance)
			{
				StateReplicator.Instance.UnregisterState(state);
				return;
			}
			StateReplicator.m_pendingRegistration.Remove(state);
		}

		// Token: 0x060032DE RID: 13022 RVA: 0x00063020 File Offset: 0x00061220
		public static void SetState(IState state)
		{
			if (StateReplicator.Instance)
			{
				StateReplicator.Instance.SetServerState(state);
			}
		}

		// Token: 0x060032DF RID: 13023 RVA: 0x001617F4 File Offset: 0x0015F9F4
		private static int GetUniqueKey(IState state)
		{
			if (state == null || !state.gameObject || !state.gameObject.transform)
			{
				return 0;
			}
			if (state.Key != 0)
			{
				return state.Key;
			}
			state.Key = StateReplicator.GetUniqueKey(state.gameObject.transform);
			return state.Key;
		}

		// Token: 0x060032E0 RID: 13024 RVA: 0x00161850 File Offset: 0x0015FA50
		private static int GetUniqueKey(Transform trans)
		{
			if (!trans)
			{
				return 0;
			}
			StateReplicator.m_parents.Clear();
			StateReplicator.FillParents(trans);
			StateReplicator.m_parents.Reverse();
			StateReplicator.m_paths.Clear();
			for (int i = 0; i < StateReplicator.m_parents.Count; i++)
			{
				string item = (i == 0) ? StateReplicator.m_parents[i].name : ZString.Format<int, string>("{0}/{1}", StateReplicator.m_parents[i].GetSiblingIndex(), StateReplicator.m_parents[i].name);
				StateReplicator.m_paths.Add(item);
			}
			return Animator.StringToHash(string.Join("/", StateReplicator.m_paths));
		}

		// Token: 0x060032E1 RID: 13025 RVA: 0x00063039 File Offset: 0x00061239
		private static void FillParents(Transform trans)
		{
			StateReplicator.m_parents.Add(trans);
			if (trans.parent)
			{
				StateReplicator.FillParents(trans.parent);
			}
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x060032E2 RID: 13026 RVA: 0x0006305E File Offset: 0x0006125E
		// (set) Token: 0x060032E3 RID: 13027 RVA: 0x00063066 File Offset: 0x00061266
		public int Registered { get; set; }

		// Token: 0x060032E4 RID: 13028 RVA: 0x00161900 File Offset: 0x0015FB00
		private void Awake()
		{
			if (StateReplicator.Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			StateReplicator.Instance = this;
			if (StateReplicator.m_pendingRegistration != null && StateReplicator.m_pendingRegistration.Count > 0)
			{
				for (int i = 0; i < StateReplicator.m_pendingRegistration.Count; i++)
				{
					this.RegisterState(StateReplicator.m_pendingRegistration[i]);
				}
				StateReplicator.m_pendingRegistration.Clear();
			}
			if (!GameManager.IsServer)
			{
				this.m_states.Changed += this.StatesOnChanged;
			}
		}

		// Token: 0x060032E5 RID: 13029 RVA: 0x0006306F File Offset: 0x0006126F
		protected override void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.m_states.Changed -= this.StatesOnChanged;
			}
			base.OnDestroy();
		}

		// Token: 0x060032E6 RID: 13030 RVA: 0x0016198C File Offset: 0x0015FB8C
		private void StatesOnChanged(SynchronizedCollection<int, byte>.Operation op, int key, byte oldValue, byte newValue)
		{
			IState state;
			if (this.m_stateMap.TryGetValue(key, out state))
			{
				switch (op)
				{
				case SynchronizedCollection<int, byte>.Operation.Add:
				case SynchronizedCollection<int, byte>.Operation.Insert:
				case SynchronizedCollection<int, byte>.Operation.Set:
					state.SetState(newValue);
					break;
				case SynchronizedCollection<int, byte>.Operation.Clear:
				case SynchronizedCollection<int, byte>.Operation.RemoveAt:
					break;
				case SynchronizedCollection<int, byte>.Operation.InitialAdd:
				case SynchronizedCollection<int, byte>.Operation.InitialAddFinal:
					state.SetState(newValue);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x060032E7 RID: 13031 RVA: 0x001619E0 File Offset: 0x0015FBE0
		private void RegisterState(IState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			int uniqueKey = StateReplicator.GetUniqueKey(state);
			if (this.m_stateMap.TryAdd(uniqueKey, state))
			{
				this.Registered++;
				if (GameManager.IsServer)
				{
					this.m_states.Add(uniqueKey, state.GetState());
					return;
				}
				byte state2;
				if (this.m_states.TryGetValue(uniqueKey, out state2))
				{
					state.SetState(state2);
					return;
				}
			}
			else
			{
				IState state3;
				if (this.m_stateMap.TryGetValue(uniqueKey, out state3) && state3.gameObject && state.gameObject)
				{
					string path = state3.gameObject.transform.GetPath();
					string path2 = state.gameObject.transform.GetPath();
					Debug.LogWarning(string.Concat(new string[]
					{
						"Duplicate Interactive Key? ",
						uniqueKey.ToString(),
						"\nEXISTING:",
						path,
						"\nNEW:",
						path2
					}));
					return;
				}
				Debug.LogWarning("Duplicate Interactive Key? " + uniqueKey.ToString());
			}
		}

		// Token: 0x060032E8 RID: 13032 RVA: 0x00161AF4 File Offset: 0x0015FCF4
		private void UnregisterState(IState state)
		{
			if (state != null)
			{
				int key = state.Key;
				if (key != 0)
				{
					if (this.m_stateMap.Remove(key))
					{
						this.Registered--;
					}
					if (GameManager.IsServer)
					{
						this.m_states.Remove(key);
					}
				}
			}
		}

		// Token: 0x060032E9 RID: 13033 RVA: 0x00161B40 File Offset: 0x0015FD40
		private void RefreshState(IState state)
		{
			if (!GameManager.IsServer || state == null || state.Key == 0)
			{
				return;
			}
			byte b;
			if (this.m_states.TryGetValue(state.Key, out b) && b != state.GetState())
			{
				this.m_states[state.Key] = state.GetState();
			}
		}

		// Token: 0x060032EA RID: 13034 RVA: 0x00063095 File Offset: 0x00061295
		public bool TryGetState(int key, out IState state)
		{
			return this.m_stateMap.TryGetValue(key, out state);
		}

		// Token: 0x060032EB RID: 13035 RVA: 0x00161B40 File Offset: 0x0015FD40
		public void SetServerState(IState state)
		{
			if (!GameManager.IsServer || state == null || state.Key == 0)
			{
				return;
			}
			byte b;
			if (this.m_states.TryGetValue(state.Key, out b) && b != state.GetState())
			{
				this.m_states[state.Key] = state.GetState();
			}
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x00161B98 File Offset: 0x0015FD98
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_states);
			this.m_states.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04003128 RID: 12584
		public const int kDefaultKey = 0;

		// Token: 0x04003129 RID: 12585
		public static StateReplicator Instance = null;

		// Token: 0x0400312A RID: 12586
		private static readonly List<Transform> m_parents = new List<Transform>(128);

		// Token: 0x0400312B RID: 12587
		private static readonly List<string> m_paths = new List<string>(128);

		// Token: 0x0400312C RID: 12588
		private static readonly List<IState> m_pendingRegistration = new List<IState>(128);

		// Token: 0x0400312E RID: 12590
		private readonly SynchronizedStateDictionary m_states = new SynchronizedStateDictionary();

		// Token: 0x0400312F RID: 12591
		private readonly Dictionary<int, IState> m_stateMap = new Dictionary<int, IState>();
	}
}
