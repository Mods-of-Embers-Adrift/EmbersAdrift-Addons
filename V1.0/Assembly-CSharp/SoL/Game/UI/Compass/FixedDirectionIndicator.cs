using System;
using UnityEngine;

namespace SoL.Game.UI.Compass
{
	// Token: 0x020009A1 RID: 2465
	public class FixedDirectionIndicator : MonoBehaviour
	{
		// Token: 0x17001056 RID: 4182
		// (get) Token: 0x060049D2 RID: 18898 RVA: 0x0007198E File Offset: 0x0006FB8E
		private Transform LocalTransform
		{
			get
			{
				if (!this.m_transform)
				{
					this.m_transform = base.gameObject.transform;
				}
				return this.m_transform;
			}
		}

		// Token: 0x060049D3 RID: 18899 RVA: 0x001B0C1C File Offset: 0x001AEE1C
		internal void UpdateIndicator(CompassUI controller, float playerAngle)
		{
			if (!this.LocalTransform || !controller)
			{
				return;
			}
			float num = Mathf.DeltaAngle(playerAngle, (float)this.m_direction);
			Vector3 localPosition = this.LocalTransform.localPosition;
			localPosition.x = num * (controller.Spacing / 45f);
			this.LocalTransform.localPosition = localPosition;
			if (this.m_minScale < 1f)
			{
				float num2 = Mathf.Abs(localPosition.x);
				float num3 = controller.Spacing * controller.SpacingScaleFactor;
				float t = (num2 > num3) ? ((num2 - num3) / (controller.Spacing - num3)) : 0f;
				float d = Mathf.Lerp(1f, this.m_minScale, t);
				this.LocalTransform.localScale = Vector3.one * d;
			}
		}

		// Token: 0x040044CA RID: 17610
		[Range(0f, 359f)]
		[SerializeField]
		private int m_direction;

		// Token: 0x040044CB RID: 17611
		[Range(0f, 1f)]
		[SerializeField]
		private float m_minScale = 0.6f;

		// Token: 0x040044CC RID: 17612
		[NonSerialized]
		private Transform m_transform;
	}
}
