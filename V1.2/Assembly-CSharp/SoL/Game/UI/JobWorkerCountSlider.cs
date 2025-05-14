using System;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008BE RID: 2238
	[Serializable]
	public class JobWorkerCountSlider : IntSlider
	{
		// Token: 0x06004195 RID: 16789 RVA: 0x0018FF58 File Offset: 0x0018E158
		protected override bool InitInternal()
		{
			if (!this.m_obj || !this.m_obj.Slider)
			{
				return false;
			}
			int num = Options.GameOptions.SelectedJobWorkerMaximumCount.Value;
			int value = Options.GameOptions.PreviousJobWorkerMaximumCount.Value;
			int jobWorkerMaximumCount = JobsUtility.JobWorkerMaximumCount;
			if (value < 1 || value != jobWorkerMaximumCount || num > jobWorkerMaximumCount)
			{
				num = this.GetDefaultJobCount();
			}
			num = this.GetClampedJobCount(num);
			this.m_obj.Slider.wholeNumbers = true;
			this.m_obj.Slider.minValue = 0f;
			this.m_obj.Slider.maxValue = (float)jobWorkerMaximumCount;
			this.m_obj.Slider.value = (float)num;
			Options.GameOptions.PreviousJobWorkerMaximumCount.Value = jobWorkerMaximumCount;
			Options.GameOptions.SelectedJobWorkerMaximumCount.Value = num;
			this.OnSliderChanged((float)num);
			return true;
		}

		// Token: 0x06004196 RID: 16790 RVA: 0x00190028 File Offset: 0x0018E228
		protected override void OnSliderChanged(float value)
		{
			int request = Mathf.FloorToInt(value);
			int clampedJobCount = this.GetClampedJobCount(request);
			JobsUtility.JobWorkerCount = clampedJobCount;
			base.OnSliderChanged((float)clampedJobCount);
			Debug.Log("JobWorkerCount set to " + JobsUtility.JobWorkerCount.ToString());
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x0006C4B5 File Offset: 0x0006A6B5
		protected override void ResetButtonClicked()
		{
			this.m_obj.Slider.value = (float)this.GetDefaultJobCount();
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x00190070 File Offset: 0x0018E270
		private int GetDefaultJobCount()
		{
			int request = Mathf.FloorToInt((float)SystemInfo.processorCount * 0.75f);
			return this.GetClampedJobCount(request);
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x0006C4CE File Offset: 0x0006A6CE
		private int GetClampedJobCount(int request)
		{
			return Mathf.Clamp(request, 0, JobsUtility.JobWorkerMaximumCount);
		}

		// Token: 0x04003ED2 RID: 16082
		private const int kMinimum = 0;

		// Token: 0x04003ED3 RID: 16083
		private const float kDefaultFraction = 0.75f;
	}
}
