using System;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D55 RID: 3413
	public class HumanoidSexAssigner : GameEntityComponent
	{
		// Token: 0x060066BB RID: 26299 RVA: 0x00211B00 File Offset: 0x0020FD00
		private void Start()
		{
			if (this.m_controller)
			{
				HumanoidAnimancerController controller = this.m_controller;
				if (controller != null)
				{
					((IAnimationController)controller).AssignSex(this.m_sex);
				}
			}
			if (this.m_animator)
			{
				HumanoidReferencePoints referencePoints = this.m_animator.GetReferencePoints(this.m_sex, false);
				if (base.GameEntity)
				{
					if (base.GameEntity.NameplateHeightOffset != null)
					{
						referencePoints.Overhead.transform.SetParent(base.GameEntity.gameObject.transform);
						referencePoints.Overhead.transform.localPosition = base.GameEntity.NameplateHeightOffset.Value;
						referencePoints.Overhead.transform.localRotation = Quaternion.identity;
					}
					if (base.GameEntity.CharacterData)
					{
						base.GameEntity.CharacterData.ReferencePoints = new HumanoidReferencePoints?(referencePoints);
					}
				}
				if (this.m_ikController)
				{
					this.m_ikController.Initialize(referencePoints);
				}
			}
		}

		// Token: 0x04005942 RID: 22850
		[SerializeField]
		private CharacterSex m_sex = CharacterSex.Male;

		// Token: 0x04005943 RID: 22851
		[SerializeField]
		private HumanoidAnimancerController m_controller;

		// Token: 0x04005944 RID: 22852
		[SerializeField]
		private HumanoidIKController m_ikController;

		// Token: 0x04005945 RID: 22853
		[SerializeField]
		private Animator m_animator;
	}
}
