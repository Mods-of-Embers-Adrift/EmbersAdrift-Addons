using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C20 RID: 3104
	[Serializable]
	public class DeliveryParams
	{
		// Token: 0x170016E2 RID: 5858
		// (get) Token: 0x06005FBD RID: 24509 RVA: 0x000806E5 File Offset: 0x0007E8E5
		public bool HasDelay
		{
			get
			{
				return this.m_deliveryMethod.HasDelay();
			}
		}

		// Token: 0x06005FBE RID: 24510 RVA: 0x000806F2 File Offset: 0x0007E8F2
		private void ValidateNumbers()
		{
			this.m_delay = Mathf.Clamp(this.m_delay, 0f, float.MaxValue);
			this.m_velocity = Mathf.Clamp(this.m_velocity, 0f, float.MaxValue);
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x001FB22C File Offset: 0x001F942C
		public float? GetDelay(Vector3 startPos, Vector3 endPos)
		{
			DeliveryMethodTypes deliveryMethod = this.m_deliveryMethod;
			if (deliveryMethod == DeliveryMethodTypes.Delayed)
			{
				return new float?(this.m_delay);
			}
			if (deliveryMethod != DeliveryMethodTypes.Projectile)
			{
				return null;
			}
			float num = Vector3.Distance(startPos, endPos);
			return new float?(1f / this.m_velocity * num);
		}

		// Token: 0x170016E3 RID: 5859
		// (get) Token: 0x06005FC0 RID: 24512 RVA: 0x0008072A File Offset: 0x0007E92A
		public DeliveryMethodTypes DeliveryMethod
		{
			get
			{
				return this.m_deliveryMethod;
			}
		}

		// Token: 0x170016E4 RID: 5860
		// (get) Token: 0x06005FC1 RID: 24513 RVA: 0x00080732 File Offset: 0x0007E932
		public float Delay
		{
			get
			{
				return this.m_delay;
			}
		}

		// Token: 0x170016E5 RID: 5861
		// (get) Token: 0x06005FC2 RID: 24514 RVA: 0x0008073A File Offset: 0x0007E93A
		public float Velocity
		{
			get
			{
				return this.m_velocity;
			}
		}

		// Token: 0x04005299 RID: 21145
		private const string kDeliveryGroupName = "Delivery";

		// Token: 0x0400529A RID: 21146
		[SerializeField]
		private DeliveryMethodTypes m_deliveryMethod;

		// Token: 0x0400529B RID: 21147
		[SerializeField]
		private float m_delay = 2f;

		// Token: 0x0400529C RID: 21148
		[SerializeField]
		private float m_velocity = 10f;
	}
}
