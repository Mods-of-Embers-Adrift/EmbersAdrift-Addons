using System;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game
{
	// Token: 0x02000614 RID: 1556
	public class TargetPosition : MonoBehaviour
	{
		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x0600316C RID: 12652 RVA: 0x00062194 File Offset: 0x00060394
		private bool m_randomizePositionCircle
		{
			get
			{
				return this.m_randomizePosition && this.m_boundsType == TargetPosition.BoundsType.Circle;
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x0600316D RID: 12653 RVA: 0x000621A9 File Offset: 0x000603A9
		private bool m_randomizePositionRectangle
		{
			get
			{
				return this.m_randomizePosition && this.m_boundsType == TargetPosition.BoundsType.Rectangle;
			}
		}

		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x0600316E RID: 12654 RVA: 0x0015C898 File Offset: 0x0015AA98
		protected GameObject Target
		{
			get
			{
				TargetPosition.TargetType targetType = this.m_targetType;
				if (targetType == TargetPosition.TargetType.GameObject)
				{
					return this.m_target;
				}
				if (targetType != TargetPosition.TargetType.TargetPosition)
				{
					return base.gameObject;
				}
				if (!(this.m_targetPosition == null))
				{
					return this.m_targetPosition.gameObject;
				}
				return null;
			}
		}

		// Token: 0x0600316F RID: 12655 RVA: 0x0015C8E0 File Offset: 0x0015AAE0
		public Vector3 GetPosition()
		{
			if (this.m_targetType == TargetPosition.TargetType.TargetPosition)
			{
				return this.m_targetPosition.GetPosition();
			}
			Vector3 vector = this.Target.transform.position;
			if (this.m_randomizePosition)
			{
				TargetPosition.BoundsType boundsType = this.m_boundsType;
				if (boundsType != TargetPosition.BoundsType.Circle)
				{
					if (boundsType == TargetPosition.BoundsType.Rectangle)
					{
						float num = this.m_randomPositionOuterBounds.extents.x - this.m_randomPositionInnerBounds.extents.x;
						float num2 = this.m_randomPositionOuterBounds.extents.z - this.m_randomPositionInnerBounds.extents.z;
						float num3 = UnityEngine.Random.Range(this.m_randomPositionInnerBounds.extents.x, this.m_randomPositionInnerBounds.extents.x + num);
						float num4 = UnityEngine.Random.Range(this.m_randomPositionInnerBounds.extents.z, this.m_randomPositionInnerBounds.extents.z + num2);
						if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
						{
							num3 *= -1f;
						}
						if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
						{
							num4 *= -1f;
						}
						vector = base.gameObject.transform.position + new Vector3(num3, 0f, num4);
						vector = vector.RotateAroundPivot(base.gameObject.transform.position, new Vector3(0f, base.gameObject.transform.eulerAngles.y, 0f));
					}
				}
				else
				{
					float f = UnityEngine.Random.Range(0f, 360f) * 0.017453292f;
					float num5 = this.m_randomPositionRadius.RandomWithinRange();
					vector.x += Mathf.Cos(f) * num5;
					vector.z += Mathf.Sin(f) * num5;
				}
			}
			NavMeshHit navMeshHit;
			if (GameManager.IsServer && NavMeshUtilities.SamplePosition(vector, out navMeshHit, 5f, -1))
			{
				vector = navMeshHit.position;
			}
			return vector;
		}

		// Token: 0x06003170 RID: 12656 RVA: 0x0015CADC File Offset: 0x0015ACDC
		public Quaternion GetRotation()
		{
			if (this.m_targetType == TargetPosition.TargetType.TargetPosition)
			{
				return this.m_targetPosition.GetRotation();
			}
			float num = this.Target.transform.eulerAngles.y;
			if (this.m_randomizeRotation)
			{
				float num2 = UnityEngine.Random.Range(0f, this.m_randomRotationAngleRange) - this.m_randomRotationAngleRange * 0.5f;
				num += num2;
				num += this.m_randomRotationOffset;
			}
			return Quaternion.Euler(new Vector3(0f, num, 0f));
		}

		// Token: 0x06003171 RID: 12657 RVA: 0x0015CB5C File Offset: 0x0015AD5C
		protected virtual void OnValidate()
		{
			if (this.m_targetType == TargetPosition.TargetType.TargetPosition && this.m_targetPosition == null)
			{
				this.m_randomizePosition = false;
				this.m_randomizeRotation = false;
			}
			if (this.m_randomizeRotation)
			{
				this.m_randomRotationAngleRange = Mathf.Clamp(this.m_randomRotationAngleRange, 0f, 360f);
			}
		}

		// Token: 0x04002FC5 RID: 12229
		[SerializeField]
		private TargetPosition.TargetType m_targetType;

		// Token: 0x04002FC6 RID: 12230
		[SerializeField]
		private TargetPosition.BoundsType m_boundsType;

		// Token: 0x04002FC7 RID: 12231
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04002FC8 RID: 12232
		[SerializeField]
		private TargetPosition m_targetPosition;

		// Token: 0x04002FC9 RID: 12233
		[SerializeField]
		private bool m_randomizePosition;

		// Token: 0x04002FCA RID: 12234
		[SerializeField]
		private MinMaxFloatRange m_randomPositionRadius = new MinMaxFloatRange(1f, 5f);

		// Token: 0x04002FCB RID: 12235
		[SerializeField]
		private Bounds m_randomPositionInnerBounds = new Bounds(Vector3.zero, new Vector3(1f, 0.2f, 1f));

		// Token: 0x04002FCC RID: 12236
		[SerializeField]
		private Bounds m_randomPositionOuterBounds = new Bounds(Vector3.zero, new Vector3(5f, 0.2f, 5f));

		// Token: 0x04002FCD RID: 12237
		[SerializeField]
		private bool m_randomizeRotation;

		// Token: 0x04002FCE RID: 12238
		[SerializeField]
		private float m_randomRotationAngleRange = 360f;

		// Token: 0x04002FCF RID: 12239
		[SerializeField]
		private float m_randomRotationOffset;

		// Token: 0x02000615 RID: 1557
		private enum BoundsType
		{
			// Token: 0x04002FD1 RID: 12241
			Circle,
			// Token: 0x04002FD2 RID: 12242
			Rectangle
		}

		// Token: 0x02000616 RID: 1558
		private enum TargetType
		{
			// Token: 0x04002FD4 RID: 12244
			None,
			// Token: 0x04002FD5 RID: 12245
			GameObject,
			// Token: 0x04002FD6 RID: 12246
			TargetPosition
		}
	}
}
