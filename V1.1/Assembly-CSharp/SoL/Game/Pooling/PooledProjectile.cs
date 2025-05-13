using System;
using SoL.Game.Audio;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007E0 RID: 2016
	public class PooledProjectile : PooledObject
	{
		// Token: 0x06003ABC RID: 15036 RVA: 0x00178AFC File Offset: 0x00176CFC
		public void Initialize(GameEntity source, GameEntity target, float velocity)
		{
			this.InitializeInternal(source, target, velocity, null);
		}

		// Token: 0x06003ABD RID: 15037 RVA: 0x00178B1C File Offset: 0x00176D1C
		private void InitializeInternal(GameEntity source, GameEntity target, float velocity, float? timeout)
		{
			Transform mountPoint = base.GetMountPoint(this.m_launchPoint, source, null);
			Vector3 pos = (mountPoint == null) ? (source.gameObject.transform.position + Vector3.one) : mountPoint.gameObject.transform.position;
			Quaternion rotation = source.gameObject.transform.rotation;
			this.m_target = base.GetMountPoint(VfxMountPoint.DamageTarget, target, null);
			if (this.m_target == null)
			{
				this.m_target = target.gameObject.transform;
			}
			this.m_velocity = velocity;
			if (timeout != null)
			{
				base.Initialize(null, pos, rotation);
			}
			else
			{
				base.Initialize(null, pos, rotation);
			}
			AudioEvent audioEvent = this.m_audioEvent;
			if (audioEvent != null)
			{
				audioEvent.Play(1f);
			}
			this.m_initialized = true;
		}

		// Token: 0x06003ABE RID: 15038 RVA: 0x00178BF0 File Offset: 0x00176DF0
		protected override void Update()
		{
			if (this.m_initialized)
			{
				if (this.m_target == null)
				{
					this.ResetPooledObject();
					return;
				}
				base.gameObject.transform.position = Vector3.MoveTowards(base.gameObject.transform.position, this.m_target.position, this.m_velocity * Time.deltaTime);
				if ((base.gameObject.transform.position - this.m_target.position).sqrMagnitude <= 1f)
				{
					this.ResetPooledObject();
				}
			}
			base.Update();
		}

		// Token: 0x06003ABF RID: 15039 RVA: 0x00067D33 File Offset: 0x00065F33
		public override void ResetPooledObject()
		{
			this.m_initialized = false;
			this.m_target = null;
			this.m_velocity = 0f;
			base.ResetPooledObject();
		}

		// Token: 0x04003935 RID: 14645
		[SerializeField]
		private VfxMountPoint m_launchPoint;

		// Token: 0x04003936 RID: 14646
		private bool m_initialized;

		// Token: 0x04003937 RID: 14647
		private Transform m_target;

		// Token: 0x04003938 RID: 14648
		private float m_velocity;

		// Token: 0x04003939 RID: 14649
		[SerializeField]
		private AudioEvent m_audioEvent;
	}
}
