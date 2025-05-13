using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D64 RID: 3428
	public class AnimancerReplicatorDummy : MonoBehaviour, IAnimancerReplicator
	{
		// Token: 0x170018CD RID: 6349
		// (get) Token: 0x0600676C RID: 26476 RVA: 0x000857F5 File Offset: 0x000839F5
		private bool m_showBaseAnimancerController
		{
			get
			{
				return this.m_nonHumanoidAnimancerController == null;
			}
		}

		// Token: 0x170018CE RID: 6350
		// (get) Token: 0x0600676D RID: 26477 RVA: 0x00085803 File Offset: 0x00083A03
		// (set) Token: 0x0600676E RID: 26478 RVA: 0x00085840 File Offset: 0x00083A40
		Vector2 IAnimancerReplicator.RawLocomotion
		{
			get
			{
				return new Vector2(Mathf.Lerp(0f, this.m_locomotion.x, this.m_locomotionLerp), Mathf.Lerp(0f, this.m_locomotion.y, this.m_locomotionLerp));
			}
			set
			{
				this.m_locomotion = value;
			}
		}

		// Token: 0x170018CF RID: 6351
		// (get) Token: 0x0600676F RID: 26479 RVA: 0x00085849 File Offset: 0x00083A49
		// (set) Token: 0x06006770 RID: 26480 RVA: 0x00085851 File Offset: 0x00083A51
		float IAnimancerReplicator.RawRotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				this.m_rotation = value;
			}
		}

		// Token: 0x170018D0 RID: 6352
		// (get) Token: 0x06006771 RID: 26481 RVA: 0x0008585A File Offset: 0x00083A5A
		// (set) Token: 0x06006772 RID: 26482 RVA: 0x00085862 File Offset: 0x00083A62
		float IAnimancerReplicator.Speed
		{
			get
			{
				return this.m_speed;
			}
			set
			{
				this.m_speed = value;
			}
		}

		// Token: 0x040059CA RID: 22986
		[SerializeField]
		private NonHumanoidAnimancerController m_nonHumanoidAnimancerController;

		// Token: 0x040059CB RID: 22987
		[SerializeField]
		private BaseAnimancerController m_animancerController;

		// Token: 0x040059CC RID: 22988
		[Range(0f, 1f)]
		[SerializeField]
		private float m_locomotionLerp;

		// Token: 0x040059CD RID: 22989
		[SerializeField]
		private Vector2 m_locomotion = Vector2.zero;

		// Token: 0x040059CE RID: 22990
		[SerializeField]
		private float m_rotation;

		// Token: 0x040059CF RID: 22991
		[SerializeField]
		private float m_speed;

		// Token: 0x040059D0 RID: 22992
		[SerializeField]
		private AnimationSequence m_sequence;

		// Token: 0x040059D1 RID: 22993
		private IAnimationController m_animController;
	}
}
