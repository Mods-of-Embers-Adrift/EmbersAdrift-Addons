using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B7E RID: 2942
	[Serializable]
	public class InteractionSettingsBase
	{
		// Token: 0x17001525 RID: 5413
		// (get) Token: 0x06005A99 RID: 23193 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showIgnoreDistanceCheck
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001526 RID: 5414
		// (get) Token: 0x06005A9A RID: 23194 RVA: 0x0007CD4C File Offset: 0x0007AF4C
		protected virtual bool m_showManualDistance
		{
			get
			{
				return !this.m_ignoreDistanceCheck;
			}
		}

		// Token: 0x17001527 RID: 5415
		// (get) Token: 0x06005A9B RID: 23195 RVA: 0x0007CD57 File Offset: 0x0007AF57
		public virtual float DistanceValue
		{
			get
			{
				return this.m_distance;
			}
		}

		// Token: 0x06005A9C RID: 23196 RVA: 0x0007CD5F File Offset: 0x0007AF5F
		private void InitDistance()
		{
			if (this.m_distanceInitialized)
			{
				return;
			}
			this.m_distanceValue = this.DistanceValue;
			this.m_distanceValueSquared = this.m_distanceValue * this.m_distanceValue;
			this.m_distanceInitialized = true;
		}

		// Token: 0x06005A9D RID: 23197 RVA: 0x0007CD90 File Offset: 0x0007AF90
		public bool IsWithinRange(GameObject obj, GameEntity entity)
		{
			this.InitDistance();
			return this.m_ignoreDistanceCheck || (obj && entity && this.GetSqrDistance(entity.gameObject, obj) <= this.m_distanceValueSquared);
		}

		// Token: 0x06005A9E RID: 23198 RVA: 0x001ED200 File Offset: 0x001EB400
		public bool IsWithinRange(GameEntity targetEntity, GameEntity playerEntity)
		{
			if (!targetEntity || !playerEntity)
			{
				return false;
			}
			if (this.m_ignoreDistanceCheck)
			{
				return true;
			}
			this.InitDistance();
			if (this.GetSqrDistance(playerEntity.gameObject, targetEntity.gameObject) <= this.m_distanceValueSquared)
			{
				return true;
			}
			if (targetEntity.AlternateTargetPoints != null)
			{
				for (int i = 0; i < targetEntity.AlternateTargetPoints.Points.Length; i++)
				{
					if (targetEntity.AlternateTargetPoints.Points[i] != null && this.GetSqrDistance(playerEntity.gameObject, targetEntity.AlternateTargetPoints.Points[i]) <= this.m_distanceValueSquared)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005A9F RID: 23199 RVA: 0x001ED2AC File Offset: 0x001EB4AC
		private float GetSqrDistance(GameObject objA, GameObject objB)
		{
			return (objA.transform.position - objB.transform.position).sqrMagnitude;
		}

		// Token: 0x17001528 RID: 5416
		// (get) Token: 0x06005AA0 RID: 23200 RVA: 0x0007CDCC File Offset: 0x0007AFCC
		public virtual InteractionManager.MouseButtonType InteractionMouseButton
		{
			get
			{
				return this.m_interactionMouseButton;
			}
		}

		// Token: 0x17001529 RID: 5417
		// (get) Token: 0x06005AA1 RID: 23201 RVA: 0x0007CDD4 File Offset: 0x0007AFD4
		public virtual bool DoubleClickInteraction
		{
			get
			{
				return this.m_doubleClickInteraction;
			}
		}

		// Token: 0x1700152A RID: 5418
		// (get) Token: 0x06005AA2 RID: 23202 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showInteractionOptions
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005AA3 RID: 23203 RVA: 0x0007CDDC File Offset: 0x0007AFDC
		public bool ValidateInteraction(InteractionManager.MouseButtonType buttonType, bool doubleClick)
		{
			return buttonType == this.InteractionMouseButton && doubleClick == this.DoubleClickInteraction;
		}

		// Token: 0x1700152B RID: 5419
		// (get) Token: 0x06005AA4 RID: 23204 RVA: 0x0007CDF2 File Offset: 0x0007AFF2
		public virtual InteractionManager.MouseButtonType ContextMouseButton
		{
			get
			{
				return this.m_contextMouseButton;
			}
		}

		// Token: 0x1700152C RID: 5420
		// (get) Token: 0x06005AA5 RID: 23205 RVA: 0x0007CDFA File Offset: 0x0007AFFA
		public virtual bool DoubleClickContext
		{
			get
			{
				return this.m_doubleClickContext;
			}
		}

		// Token: 0x1700152D RID: 5421
		// (get) Token: 0x06005AA6 RID: 23206 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showContextOptions
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005AA7 RID: 23207 RVA: 0x0007CE02 File Offset: 0x0007B002
		public bool ValidateContext(InteractionManager.MouseButtonType buttonType, bool doubleClick)
		{
			return buttonType == this.ContextMouseButton && doubleClick == this.DoubleClickContext;
		}

		// Token: 0x04004F7E RID: 20350
		protected const string kDistanceGroup = "Distance";

		// Token: 0x04004F7F RID: 20351
		[SerializeField]
		protected bool m_ignoreDistanceCheck;

		// Token: 0x04004F80 RID: 20352
		[SerializeField]
		protected float m_distance = 3f;

		// Token: 0x04004F81 RID: 20353
		private float m_distanceValue;

		// Token: 0x04004F82 RID: 20354
		private float m_distanceValueSquared;

		// Token: 0x04004F83 RID: 20355
		private bool m_distanceInitialized;

		// Token: 0x04004F84 RID: 20356
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x04004F85 RID: 20357
		private const string kMouseGroup = "Mouse";

		// Token: 0x04004F86 RID: 20358
		protected const string kMouseInteractionGroup = "Mouse/Interaction";

		// Token: 0x04004F87 RID: 20359
		protected const string kMouseContextGroup = "Mouse/Context";

		// Token: 0x04004F88 RID: 20360
		[SerializeField]
		private InteractionManager.MouseButtonType m_interactionMouseButton = InteractionManager.MouseButtonType.Right;

		// Token: 0x04004F89 RID: 20361
		[SerializeField]
		private bool m_doubleClickInteraction;

		// Token: 0x04004F8A RID: 20362
		[SerializeField]
		private InteractionManager.MouseButtonType m_contextMouseButton = InteractionManager.MouseButtonType.Right;

		// Token: 0x04004F8B RID: 20363
		[SerializeField]
		private bool m_doubleClickContext;
	}
}
