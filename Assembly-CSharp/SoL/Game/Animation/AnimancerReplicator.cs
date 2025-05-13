using System;
using NetStack.Quantization;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D63 RID: 3427
	public class AnimancerReplicator : Replicator, IAnimancerReplicator
	{
		// Token: 0x170018C9 RID: 6345
		// (get) Token: 0x0600675C RID: 26460 RVA: 0x0005D9AA File Offset: 0x0005BBAA
		public override ReplicatorTypes Type
		{
			get
			{
				return ReplicatorTypes.Animancer;
			}
		}

		// Token: 0x170018CA RID: 6346
		// (get) Token: 0x0600675D RID: 26461 RVA: 0x00085760 File Offset: 0x00083960
		// (set) Token: 0x0600675E RID: 26462 RVA: 0x00085768 File Offset: 0x00083968
		public Vector2 RawLocomotion { get; set; }

		// Token: 0x170018CB RID: 6347
		// (get) Token: 0x0600675F RID: 26463 RVA: 0x00085771 File Offset: 0x00083971
		// (set) Token: 0x06006760 RID: 26464 RVA: 0x00085779 File Offset: 0x00083979
		public float RawRotation { get; set; }

		// Token: 0x170018CC RID: 6348
		// (get) Token: 0x06006761 RID: 26465 RVA: 0x00085782 File Offset: 0x00083982
		// (set) Token: 0x06006762 RID: 26466 RVA: 0x0008578A File Offset: 0x0008398A
		public float Speed { get; set; } = 1f;

		// Token: 0x06006763 RID: 26467 RVA: 0x00085793 File Offset: 0x00083993
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.AnimatorReplicator = this;
			}
		}

		// Token: 0x06006764 RID: 26468 RVA: 0x00212E18 File Offset: 0x00211018
		public void SetHumanoidSpeedBasedOnSizeValue(float sizeSmall, float sizeLarge)
		{
			if (sizeSmall <= 0f && sizeLarge <= 0f)
			{
				this.Speed = 1f;
				return;
			}
			float f = (sizeSmall > 0f) ? Mathf.Lerp(1f, 0.8f, sizeSmall) : Mathf.Lerp(1f, 1.2f, sizeLarge);
			this.Speed = 1f / Mathf.Sqrt(f);
		}

		// Token: 0x06006765 RID: 26469 RVA: 0x00212E80 File Offset: 0x00211080
		public override BitBuffer PackData(BitBuffer outBuffer)
		{
			outBuffer = base.PackData(outBuffer);
			Vector2 value = new Vector2(this.m_cachedMovement.x, this.m_cachedMovement.y);
			outBuffer.AddVector2(value, AnimancerReplicator.m_replicatorRange);
			return outBuffer;
		}

		// Token: 0x06006766 RID: 26470 RVA: 0x00212EC4 File Offset: 0x002110C4
		public override BitBuffer ReadData(BitBuffer inBuffer)
		{
			inBuffer = base.ReadData(inBuffer);
			Vector3 vector = inBuffer.ReadVector2(AnimancerReplicator.m_replicatorRange);
			this.RawLocomotion = new Vector2(vector.x, vector.y);
			return inBuffer;
		}

		// Token: 0x06006767 RID: 26471 RVA: 0x000857AF File Offset: 0x000839AF
		public override BitBuffer PackInitialData(BitBuffer outBuffer)
		{
			return this.PackData(outBuffer);
		}

		// Token: 0x06006768 RID: 26472 RVA: 0x000857B8 File Offset: 0x000839B8
		public override BitBuffer ReadInitialData(BitBuffer inBuffer)
		{
			return this.ReadData(inBuffer);
		}

		// Token: 0x06006769 RID: 26473 RVA: 0x00212F00 File Offset: 0x00211100
		public override void SetDirtyFlags(DateTime timestamp)
		{
			if (!this.m_netEntity)
			{
				base.Dirty = false;
				return;
			}
			if (this.RawLocomotion == this.m_cachedMovement)
			{
				base.Dirty = (this.m_lastCache >= timestamp);
				return;
			}
			this.m_cachedRotation = this.RawRotation;
			this.m_cachedMovement = this.RawLocomotion;
			this.m_cachedSpeed = this.Speed;
			this.m_lastCache = DateTime.UtcNow;
			base.Dirty = true;
		}

		// Token: 0x040059C2 RID: 22978
		private DateTime m_lastCache = DateTime.MinValue;

		// Token: 0x040059C3 RID: 22979
		private float m_cachedSpeed = 1f;

		// Token: 0x040059C4 RID: 22980
		private float m_cachedRotation;

		// Token: 0x040059C5 RID: 22981
		private Vector2 m_cachedMovement = Vector2.zero;

		// Token: 0x040059C6 RID: 22982
		private static readonly BoundedRange kLocomotionRange = new BoundedRange(-20f, 20f, 0.1f);

		// Token: 0x040059C7 RID: 22983
		private static readonly BoundedRange kRotationRange = new BoundedRange(-360f, 360f, 1f);

		// Token: 0x040059C8 RID: 22984
		private static readonly BoundedRange kSpeedRange = new BoundedRange(-5f, 5f, 0.1f);

		// Token: 0x040059C9 RID: 22985
		private static readonly BoundedRange[] m_replicatorRange = new BoundedRange[]
		{
			AnimancerReplicator.kLocomotionRange,
			AnimancerReplicator.kLocomotionRange
		};
	}
}
