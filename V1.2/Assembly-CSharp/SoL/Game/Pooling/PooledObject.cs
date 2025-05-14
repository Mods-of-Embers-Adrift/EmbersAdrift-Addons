using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007DE RID: 2014
	public class PooledObject : MonoBehaviour
	{
		// Token: 0x17000D71 RID: 3441
		// (get) Token: 0x06003AAE RID: 15022 RVA: 0x00067C3C File Offset: 0x00065E3C
		// (set) Token: 0x06003AAF RID: 15023 RVA: 0x00067C44 File Offset: 0x00065E44
		public ObjectPool Pool { get; set; }

		// Token: 0x17000D72 RID: 3442
		// (get) Token: 0x06003AB0 RID: 15024 RVA: 0x00045BCA File Offset: 0x00043DCA
		internal virtual bool DelayedDestruction
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000D73 RID: 3443
		// (get) Token: 0x06003AB1 RID: 15025 RVA: 0x00067C4D File Offset: 0x00065E4D
		internal virtual float DelayedDestructionTime
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x17000D74 RID: 3444
		// (get) Token: 0x06003AB2 RID: 15026 RVA: 0x00067C54 File Offset: 0x00065E54
		// (set) Token: 0x06003AB3 RID: 15027 RVA: 0x00067C5C File Offset: 0x00065E5C
		internal float TimeReturnedToPool { get; private set; }

		// Token: 0x06003AB4 RID: 15028 RVA: 0x00067C65 File Offset: 0x00065E65
		protected virtual void Update()
		{
			if (this.m_returnTime != null && Time.time >= this.m_returnTime.Value)
			{
				this.m_returnTime = null;
				this.ReturnToPool();
			}
		}

		// Token: 0x06003AB5 RID: 15029 RVA: 0x00067C98 File Offset: 0x00065E98
		public T GetPooledInstance<T>() where T : PooledObject
		{
			if (this.m_pooledInstanceForPrefab == null)
			{
				this.m_pooledInstanceForPrefab = ObjectPool.GetPool(this, this.DelayedDestruction);
			}
			return (T)((object)this.m_pooledInstanceForPrefab.GetFromPool());
		}

		// Token: 0x06003AB6 RID: 15030 RVA: 0x001789BC File Offset: 0x00176BBC
		public virtual void ResetPooledObject()
		{
			if (base.gameObject)
			{
				base.gameObject.SetActive(false);
				base.gameObject.transform.SetParentResetScale(this.Pool.gameObject.transform);
				base.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
			this.TimeReturnedToPool = Time.time;
		}

		// Token: 0x06003AB7 RID: 15031 RVA: 0x00067CCA File Offset: 0x00065ECA
		public void Initialize(Transform parent, Vector3 pos, Quaternion rot, float timeout)
		{
			this.Initialize(parent, pos, rot);
			this.m_returnTime = new float?(Time.time + timeout);
		}

		// Token: 0x06003AB8 RID: 15032 RVA: 0x00178A28 File Offset: 0x00176C28
		public void Initialize(Transform parent, Vector3 pos, Quaternion rot)
		{
			base.gameObject.transform.SetParentResetScale(parent);
			base.gameObject.transform.localPosition = pos;
			base.gameObject.transform.localRotation = rot;
			base.gameObject.SetActive(true);
		}

		// Token: 0x06003AB9 RID: 15033 RVA: 0x00178A74 File Offset: 0x00176C74
		protected Transform GetMountPoint(VfxMountPoint mountPoint, GameEntity entity, GameEntity source = null)
		{
			if (!entity)
			{
				return null;
			}
			if (!entity.CharacterData || entity.CharacterData.ReferencePoints == null)
			{
				return entity.gameObject.transform;
			}
			HumanoidReferencePoints value = entity.CharacterData.ReferencePoints.Value;
			GameObject gameObject = null;
			switch (mountPoint)
			{
			case VfxMountPoint.None:
				break;
			case VfxMountPoint.LeftHand:
				gameObject = value.LeftMount;
				break;
			case VfxMountPoint.RightHand:
				gameObject = value.RightMount;
				break;
			case VfxMountPoint.Head:
			case VfxMountPoint.Overhead:
				gameObject = value.Overhead;
				break;
			case VfxMountPoint.DamageTarget:
				gameObject = (entity.NpcReferencePoints ? entity.NpcReferencePoints.GetDamageTargetForFx(source) : value.DamageTarget);
				break;
			case VfxMountPoint.Root:
				gameObject = entity.gameObject;
				break;
			default:
				throw new ArgumentException("mountPoint");
			}
			if (!(gameObject == null))
			{
				return gameObject.transform;
			}
			return null;
		}

		// Token: 0x04003931 RID: 14641
		public bool IsInPool = true;

		// Token: 0x04003932 RID: 14642
		private ObjectPool m_pooledInstanceForPrefab;

		// Token: 0x04003933 RID: 14643
		protected float? m_returnTime;
	}
}
