using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200009D RID: 157
	public class IvyController : MonoBehaviour
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000614 RID: 1556 RVA: 0x000A6660 File Offset: 0x000A4860
		// (remove) Token: 0x06000615 RID: 1557 RVA: 0x000A6698 File Offset: 0x000A4898
		public event Action OnGrowthStarted;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000616 RID: 1558 RVA: 0x000A66D0 File Offset: 0x000A48D0
		// (remove) Token: 0x06000617 RID: 1559 RVA: 0x000A6708 File Offset: 0x000A4908
		public event Action OnGrowthPaused;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000618 RID: 1560 RVA: 0x000A6740 File Offset: 0x000A4940
		// (remove) Token: 0x06000619 RID: 1561 RVA: 0x000A6778 File Offset: 0x000A4978
		public event Action OnGrowthFinished;

		// Token: 0x0600061A RID: 1562 RVA: 0x00047303 File Offset: 0x00045503
		private void Awake()
		{
			this.rtIvy.AwakeInit();
			this.state = IvyController.State.GROWTH_NOT_STARTED;
			if (this.growthParameters.startGrowthOnAwake)
			{
				this.StartGrowth();
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x000A67B0 File Offset: 0x000A49B0
		[ContextMenu("Start Growth")]
		public void StartGrowth()
		{
			if (this.state == IvyController.State.GROWTH_NOT_STARTED)
			{
				this.rtIvy.InitIvy(this.growthParameters, this.ivyContainer, this.ivyParameters);
				if (this.growthParameters.delay > 0f)
				{
					this.state = IvyController.State.WAITING_FOR_DELAY;
				}
				else
				{
					this.state = IvyController.State.GROWING;
				}
				if (this.OnGrowthStarted != null)
				{
					this.OnGrowthStarted();
				}
			}
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0004732A File Offset: 0x0004552A
		[ContextMenu("Pause Growth")]
		public void PauseGrowth()
		{
			if (this.state == IvyController.State.GROWING || this.state == IvyController.State.PAUSED)
			{
				this.state = IvyController.State.PAUSED;
			}
			if (this.OnGrowthPaused != null)
			{
				this.OnGrowthPaused();
			}
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00047358 File Offset: 0x00045558
		[ContextMenu("Resume Growth")]
		public void ResumeGrowth()
		{
			if (this.state == IvyController.State.GROWING || this.state == IvyController.State.PAUSED)
			{
				this.state = IvyController.State.GROWING;
			}
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00047373 File Offset: 0x00045573
		public IvyController.State GetState()
		{
			return this.state;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x000A6818 File Offset: 0x000A4A18
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			IvyController.State state = this.state;
			if (state == IvyController.State.WAITING_FOR_DELAY)
			{
				this.UpdateWaitingForDelayState(deltaTime);
				return;
			}
			if (state != IvyController.State.GROWING)
			{
				return;
			}
			this.UpdateGrowingState(deltaTime);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0004737B File Offset: 0x0004557B
		private void UpdateWaitingForDelayState(float deltaTime)
		{
			this.currentTimer += deltaTime;
			if (this.currentTimer > this.growthParameters.delay)
			{
				this.state = IvyController.State.GROWING;
				this.currentTimer = 0f;
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x000A684C File Offset: 0x000A4A4C
		private void UpdateGrowingState(float deltaTime)
		{
			if (!this.rtIvy.IsGrowingFinished() && !this.rtIvy.IsVertexLimitReached())
			{
				this.rtIvy.UpdateIvy(deltaTime);
				return;
			}
			this.state = IvyController.State.GROWTH_FINISHED;
			if (this.OnGrowthFinished != null)
			{
				this.OnGrowthFinished();
			}
		}

		// Token: 0x0400071D RID: 1821
		private float currentTimer;

		// Token: 0x0400071E RID: 1822
		public RTIvy rtIvy;

		// Token: 0x0400071F RID: 1823
		public IvyContainer ivyContainer;

		// Token: 0x04000720 RID: 1824
		public IvyParameters ivyParameters;

		// Token: 0x04000721 RID: 1825
		public RuntimeGrowthParameters growthParameters;

		// Token: 0x04000722 RID: 1826
		private IvyController.State state;

		// Token: 0x0200009E RID: 158
		public enum State
		{
			// Token: 0x04000724 RID: 1828
			GROWTH_NOT_STARTED,
			// Token: 0x04000725 RID: 1829
			WAITING_FOR_DELAY,
			// Token: 0x04000726 RID: 1830
			PAUSED,
			// Token: 0x04000727 RID: 1831
			GROWING,
			// Token: 0x04000728 RID: 1832
			GROWTH_FINISHED
		}
	}
}
