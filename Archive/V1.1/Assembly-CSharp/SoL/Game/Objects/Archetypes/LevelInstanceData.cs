using System;
using NetStack.Serialization;
using SoL.Networking;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC9 RID: 2761
	[Serializable]
	public class LevelInstanceData : INetworkSerializable
	{
		// Token: 0x14000111 RID: 273
		// (add) Token: 0x06005542 RID: 21826 RVA: 0x001DE478 File Offset: 0x001DC678
		// (remove) Token: 0x06005543 RID: 21827 RVA: 0x001DE4B0 File Offset: 0x001DC6B0
		public event Action LevelDataChanged;

		// Token: 0x170013AB RID: 5035
		// (get) Token: 0x06005544 RID: 21828 RVA: 0x00078ECA File Offset: 0x000770CA
		// (set) Token: 0x06005545 RID: 21829 RVA: 0x00078ED2 File Offset: 0x000770D2
		public float Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = Mathf.Clamp(value, 1f, 50f);
				Action levelDataChanged = this.LevelDataChanged;
				if (levelDataChanged == null)
				{
					return;
				}
				levelDataChanged();
			}
		}

		// Token: 0x06005546 RID: 21830 RVA: 0x00078EFA File Offset: 0x000770FA
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddFloat(this.m_level);
			return buffer;
		}

		// Token: 0x06005547 RID: 21831 RVA: 0x00078F0A File Offset: 0x0007710A
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.m_level = buffer.ReadFloat();
			return buffer;
		}

		// Token: 0x04004BBE RID: 19390
		private float m_level;
	}
}
