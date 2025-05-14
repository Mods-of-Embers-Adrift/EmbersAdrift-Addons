using System;
using SoL.Game;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE7 RID: 3559
	[Serializable]
	public class CameraInitializationSettings
	{
		// Token: 0x17001949 RID: 6473
		// (get) Token: 0x06006A27 RID: 27175 RVA: 0x000871AC File Offset: 0x000853AC
		public float XSpeed
		{
			get
			{
				return this.m_speed.x;
			}
		}

		// Token: 0x1700194A RID: 6474
		// (get) Token: 0x06006A28 RID: 27176 RVA: 0x000871B9 File Offset: 0x000853B9
		public float YSpeed
		{
			get
			{
				return this.m_speed.y;
			}
		}

		// Token: 0x1700194B RID: 6475
		// (get) Token: 0x06006A29 RID: 27177 RVA: 0x000871C6 File Offset: 0x000853C6
		public float AccelerationTime
		{
			get
			{
				return this.m_accelerationTime;
			}
		}

		// Token: 0x1700194C RID: 6476
		// (get) Token: 0x06006A2A RID: 27178 RVA: 0x000871CE File Offset: 0x000853CE
		public float DecelerationTime
		{
			get
			{
				return this.m_decelerationTime;
			}
		}

		// Token: 0x1700194D RID: 6477
		// (get) Token: 0x06006A2B RID: 27179 RVA: 0x000871D6 File Offset: 0x000853D6
		public float OffsetLerpSpeed
		{
			get
			{
				return this.m_offsetLerpSpeed;
			}
		}

		// Token: 0x06006A2C RID: 27180 RVA: 0x0021A2DC File Offset: 0x002184DC
		public bool TryGetStanceOffset(Stance stance, out float offset)
		{
			offset = 0f;
			switch (stance)
			{
			case Stance.Crouch:
				offset = this.m_crouchOffset;
				return this.m_overrideCrouchOffset;
			case Stance.Sit:
				offset = this.m_sitOffset;
				return this.m_overrideSitOffset;
			case Stance.Swim:
				offset = this.m_swimOffset;
				return this.m_overrideSwimOffset;
			case Stance.Looting:
				offset = this.m_lootingOffset;
				return this.m_overrideLootOffset;
			}
			return false;
		}

		// Token: 0x04005C5A RID: 23642
		[SerializeField]
		private Vector2 m_speed = new Vector2(8f, 0.1f);

		// Token: 0x04005C5B RID: 23643
		[SerializeField]
		private float m_accelerationTime = 0.1f;

		// Token: 0x04005C5C RID: 23644
		[SerializeField]
		private float m_decelerationTime = 0.1f;

		// Token: 0x04005C5D RID: 23645
		[SerializeField]
		private float m_offsetLerpSpeed = 10f;

		// Token: 0x04005C5E RID: 23646
		[SerializeField]
		private bool m_overrideSitOffset;

		// Token: 0x04005C5F RID: 23647
		[SerializeField]
		private float m_sitOffset;

		// Token: 0x04005C60 RID: 23648
		[SerializeField]
		private bool m_overrideCrouchOffset;

		// Token: 0x04005C61 RID: 23649
		[SerializeField]
		private float m_crouchOffset;

		// Token: 0x04005C62 RID: 23650
		[SerializeField]
		private bool m_overrideLootOffset;

		// Token: 0x04005C63 RID: 23651
		[SerializeField]
		private float m_lootingOffset;

		// Token: 0x04005C64 RID: 23652
		[SerializeField]
		private bool m_overrideSwimOffset;

		// Token: 0x04005C65 RID: 23653
		[SerializeField]
		private float m_swimOffset;
	}
}
