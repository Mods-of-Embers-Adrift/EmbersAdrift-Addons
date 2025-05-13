using System;
using NetStack.Serialization;
using SoL.Game;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Networking.Replication
{
	// Token: 0x0200047B RID: 1147
	public class AnimatorParameter : ISynchronizedVariable
	{
		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06001FF9 RID: 8185 RVA: 0x00057646 File Offset: 0x00055846
		private bool RampValue
		{
			get
			{
				return this.m_paramSettings != null && this.m_paramSettings.LocallyInterpolated;
			}
		}

		// Token: 0x06001FFA RID: 8186 RVA: 0x0005765D File Offset: 0x0005585D
		private void FinalizeValueChange()
		{
			this.m_lastUpdate = DateTime.UtcNow;
			this.m_changed = true;
			this.SetAnimatorValue();
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06001FFB RID: 8187 RVA: 0x00057677 File Offset: 0x00055877
		// (set) Token: 0x06001FFC RID: 8188 RVA: 0x0005767F File Offset: 0x0005587F
		public int IntValue
		{
			get
			{
				return this.m_intValue;
			}
			set
			{
				if (this.m_intValue == value)
				{
					return;
				}
				this.m_intValue = value;
				this.FinalizeValueChange();
			}
		}

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06001FFD RID: 8189 RVA: 0x00057698 File Offset: 0x00055898
		// (set) Token: 0x06001FFE RID: 8190 RVA: 0x000576A0 File Offset: 0x000558A0
		public float FloatValue
		{
			get
			{
				return this.m_floatValue;
			}
			set
			{
				if (this.m_floatValue == value)
				{
					return;
				}
				this.m_floatValue = value;
				this.FinalizeValueChange();
			}
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06001FFF RID: 8191 RVA: 0x000576B9 File Offset: 0x000558B9
		// (set) Token: 0x06002000 RID: 8192 RVA: 0x000576C1 File Offset: 0x000558C1
		public bool BoolValue
		{
			get
			{
				return this.m_boolValue;
			}
			set
			{
				if (this.m_boolValue == value)
				{
					return;
				}
				this.m_boolValue = value;
				this.FinalizeValueChange();
			}
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x00121318 File Offset: 0x0011F518
		public AnimatorParameter(NetworkEntity netEntity, Animator animator, AnimatorControllerParameter parameter, int index, bool shouldSync)
		{
			this.m_netEntity = netEntity;
			this.m_animator = animator;
			this.Name = parameter.name;
			this.NameHash = parameter.nameHash;
			this.Type = parameter.type;
			this.m_shouldSync = shouldSync;
			this.BitFlag = 1 << index;
			switch (parameter.type)
			{
			case AnimatorControllerParameterType.Float:
				this.m_floatValue = this.m_animator.GetFloat(this.NameHash);
				break;
			case AnimatorControllerParameterType.Int:
				this.m_intValue = this.m_animator.GetInteger(this.NameHash);
				break;
			case AnimatorControllerParameterType.Bool:
				this.m_boolValue = this.m_animator.GetBool(this.NameHash);
				break;
			}
			this.m_paramSettings = NetworkManager.ParamSettingsCollection.GetSettings(this.Name);
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x00121400 File Offset: 0x0011F600
		private void SetAnimatorValue()
		{
			switch (this.Type)
			{
			case AnimatorControllerParameterType.Float:
				if (!this.RampValue)
				{
					this.m_animator.SetFloat(this.NameHash, this.FloatValue);
				}
				break;
			case (AnimatorControllerParameterType)2:
				break;
			case AnimatorControllerParameterType.Int:
				this.m_animator.SetInteger(this.NameHash, this.IntValue);
				return;
			case AnimatorControllerParameterType.Bool:
				this.m_animator.SetBool(this.NameHash, this.BoolValue);
				return;
			default:
				return;
			}
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x0012147C File Offset: 0x0011F67C
		public void ExternalUpdate()
		{
			if (!this.RampValue)
			{
				return;
			}
			float @float = this.m_animator.GetFloat(this.NameHash);
			float floatValue = this.FloatValue;
			if (!Mathf.Approximately(@float, floatValue))
			{
				float interpolatedValue = PlayerAnimatorController.GetInterpolatedValue(@float, floatValue, this.m_paramSettings);
				this.m_animator.SetFloat(this.NameHash, interpolatedValue);
			}
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06002004 RID: 8196 RVA: 0x001214D4 File Offset: 0x0011F6D4
		public bool IsDefault
		{
			get
			{
				switch (this.Type)
				{
				case AnimatorControllerParameterType.Float:
					return this.m_floatValue == 0f;
				case AnimatorControllerParameterType.Int:
					return this.m_intValue == 0;
				case AnimatorControllerParameterType.Bool:
					return !this.m_boolValue;
				}
				return true;
			}
		}

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06002005 RID: 8197 RVA: 0x000576DA File Offset: 0x000558DA
		// (set) Token: 0x06002006 RID: 8198 RVA: 0x000576E2 File Offset: 0x000558E2
		public bool Dirty { get; private set; }

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06002007 RID: 8199 RVA: 0x000576EB File Offset: 0x000558EB
		public int BitFlag { get; }

		// Token: 0x06002008 RID: 8200 RVA: 0x000576F3 File Offset: 0x000558F3
		public void SetDirtyFlags(DateTime timestamp)
		{
			this.Dirty = (this.m_shouldSync && this.m_changed && timestamp <= this.m_lastUpdate);
		}

		// Token: 0x06002009 RID: 8201 RVA: 0x0005771A File Offset: 0x0005591A
		public BitBuffer PackData(BitBuffer buffer)
		{
			return this.PackDataInternal(buffer, true);
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x00057724 File Offset: 0x00055924
		public BitBuffer PackInitialData(BitBuffer buffer)
		{
			return this.PackDataInternal(buffer, false);
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x00121528 File Offset: 0x0011F728
		private BitBuffer PackDataInternal(BitBuffer buffer, bool resetDirty)
		{
			switch (this.Type)
			{
			case AnimatorControllerParameterType.Float:
				buffer.AddFloat(this.m_floatValue);
				break;
			case AnimatorControllerParameterType.Int:
				buffer.AddInt(this.m_intValue);
				break;
			case AnimatorControllerParameterType.Bool:
				buffer.AddBool(this.m_boolValue);
				break;
			}
			return buffer;
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x00121584 File Offset: 0x0011F784
		public BitBuffer ReadData(BitBuffer buffer)
		{
			bool isLocal = this.m_netEntity.IsLocal;
			switch (this.Type)
			{
			case AnimatorControllerParameterType.Float:
			{
				float floatValue = buffer.ReadFloat();
				if (!isLocal)
				{
					this.FloatValue = floatValue;
				}
				break;
			}
			case AnimatorControllerParameterType.Int:
			{
				int intValue = buffer.ReadInt();
				if (!isLocal)
				{
					this.IntValue = intValue;
				}
				break;
			}
			case AnimatorControllerParameterType.Bool:
			{
				bool boolValue = buffer.ReadBool();
				if (!isLocal)
				{
					this.BoolValue = boolValue;
				}
				break;
			}
			}
			return buffer;
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x00048A92 File Offset: 0x00046C92
		public BitBuffer ReadDataFromClient(BitBuffer buffer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x0005772E File Offset: 0x0005592E
		public void ClearMonoReferences()
		{
			this.m_animator = null;
			this.m_netEntity = null;
		}

		// Token: 0x04002567 RID: 9575
		public readonly string Name;

		// Token: 0x04002568 RID: 9576
		public readonly int NameHash;

		// Token: 0x04002569 RID: 9577
		public readonly AnimatorControllerParameterType Type;

		// Token: 0x0400256A RID: 9578
		private Animator m_animator;

		// Token: 0x0400256B RID: 9579
		private NetworkEntity m_netEntity;

		// Token: 0x0400256C RID: 9580
		private readonly bool m_shouldSync;

		// Token: 0x0400256D RID: 9581
		private readonly NetworkedAnimatorParamSetting m_paramSettings;

		// Token: 0x0400256E RID: 9582
		private DateTime m_lastUpdate = DateTime.MinValue;

		// Token: 0x0400256F RID: 9583
		private bool m_changed;

		// Token: 0x04002570 RID: 9584
		private int m_intValue;

		// Token: 0x04002571 RID: 9585
		private float m_floatValue;

		// Token: 0x04002572 RID: 9586
		private bool m_boolValue;
	}
}
