using System;
using NetStack.Serialization;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.Replication
{
	// Token: 0x0200049E RID: 1182
	public class SynchronizedEnum<T> : SynchronizedVariable<T> where T : Enum, IConvertible
	{
		// Token: 0x0600210E RID: 8462 RVA: 0x00057FB7 File Offset: 0x000561B7
		public SynchronizedEnum()
		{
			this.AssignType();
		}

		// Token: 0x0600210F RID: 8463 RVA: 0x00057FC5 File Offset: 0x000561C5
		public SynchronizedEnum(T initial) : base(initial)
		{
			this.AssignType();
		}

		// Token: 0x06002110 RID: 8464 RVA: 0x00057FD4 File Offset: 0x000561D4
		private void AssignType()
		{
			this.m_type = EnumExtensions.GetUnderlyingEnumType<T>();
			if (this.m_type == UnderlyingEnumType.None)
			{
				Debug.LogError(string.Format("Unsupported Enum underlying type: {0}", typeof(T)));
			}
		}

		// Token: 0x06002111 RID: 8465 RVA: 0x00058002 File Offset: 0x00056202
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddEnum(base.Value, this.m_type);
			return buffer;
		}

		// Token: 0x06002112 RID: 8466 RVA: 0x00058018 File Offset: 0x00056218
		protected override T ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadEnum(this.m_type);
		}

		// Token: 0x040025AD RID: 9645
		private UnderlyingEnumType m_type;
	}
}
