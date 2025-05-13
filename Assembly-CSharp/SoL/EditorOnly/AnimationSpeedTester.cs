using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DC8 RID: 3528
	public class AnimationSpeedTester : MonoBehaviour
	{
		// Token: 0x06006948 RID: 26952 RVA: 0x0008696E File Offset: 0x00084B6E
		private void Awake()
		{
			this.m_speedList = new List<Vector3>(1000);
		}

		// Token: 0x06006949 RID: 26953 RVA: 0x00086980 File Offset: 0x00084B80
		private void Start()
		{
			this.m_timeOfNextSample = Time.time + this.m_sampleTime;
		}

		// Token: 0x0600694A RID: 26954 RVA: 0x00217450 File Offset: 0x00215650
		private void Update()
		{
			if (this.m_distanceViaUpdate)
			{
				Vector3 item = (base.gameObject.transform.position - this.m_lastPos) / Time.deltaTime;
				this.m_speedList.Add(item);
				this.m_lastPos = base.gameObject.transform.position;
				if (Time.time >= this.m_timeOfNextSample)
				{
					this.AverageSpeeds();
				}
			}
			if (this.m_manuallyMove)
			{
				base.gameObject.transform.position += Vector3.forward * this.m_manualMoveSpeed * Time.deltaTime;
			}
		}

		// Token: 0x0600694B RID: 26955 RVA: 0x00217500 File Offset: 0x00215700
		private void OnAnimatorMove()
		{
			if (this.m_distanceViaUpdate)
			{
				return;
			}
			this.m_animatorDelta = this.m_animator.deltaPosition / Time.deltaTime;
			this.m_speedList.Add(this.m_animatorDelta);
			if (this.m_moveWithRootMotion)
			{
				Vector3 position = base.gameObject.transform.position + this.m_animator.deltaPosition;
				base.gameObject.transform.position = position;
			}
			if (Time.time >= this.m_timeOfNextSample)
			{
				this.AverageSpeeds();
			}
		}

		// Token: 0x0600694C RID: 26956 RVA: 0x00217590 File Offset: 0x00215790
		private void AverageSpeeds()
		{
			Vector3 a = Vector3.zero;
			for (int i = 0; i < this.m_speedList.Count; i++)
			{
				a += this.m_speedList[i];
			}
			this.m_averageDelta = a / (float)this.m_speedList.Count;
			this.ResetStuff();
		}

		// Token: 0x0600694D RID: 26957 RVA: 0x0004475B File Offset: 0x0004295B
		private void Walk()
		{
		}

		// Token: 0x0600694E RID: 26958 RVA: 0x0004475B File Offset: 0x0004295B
		private void Run()
		{
		}

		// Token: 0x0600694F RID: 26959 RVA: 0x00086994 File Offset: 0x00084B94
		private void ResetStuff()
		{
			this.m_speedList.Clear();
			this.m_timeOfNextSample = Time.time + this.m_sampleTime;
		}

		// Token: 0x04005BA4 RID: 23460
		[SerializeField]
		private float m_sampleTime = 1f;

		// Token: 0x04005BA5 RID: 23461
		[SerializeField]
		private Vector3 m_averageDelta = Vector3.zero;

		// Token: 0x04005BA6 RID: 23462
		[SerializeField]
		private Vector3 m_animatorDelta = Vector3.zero;

		// Token: 0x04005BA7 RID: 23463
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04005BA8 RID: 23464
		[SerializeField]
		private bool m_distanceViaUpdate;

		// Token: 0x04005BA9 RID: 23465
		[SerializeField]
		private bool m_moveWithRootMotion;

		// Token: 0x04005BAA RID: 23466
		[SerializeField]
		private bool m_manuallyMove;

		// Token: 0x04005BAB RID: 23467
		[SerializeField]
		private float m_manualMoveSpeed = 0.5f;

		// Token: 0x04005BAC RID: 23468
		private Vector3 m_lastPos;

		// Token: 0x04005BAD RID: 23469
		private float m_timeOfNextSample = 1f;

		// Token: 0x04005BAE RID: 23470
		private List<Vector3> m_speedList;
	}
}
