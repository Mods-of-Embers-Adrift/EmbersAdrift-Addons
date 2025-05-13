using System;
using System.Collections.Generic;
using SoL.Game.SkyDome;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B3C RID: 2876
	public class RandomEyeshineController : MonoBehaviour
	{
		// Token: 0x06005860 RID: 22624 RVA: 0x0007B06D File Offset: 0x0007926D
		private void Awake()
		{
			this.m_consumedBlinks = new List<RandomEyeshineBlink>(this.m_blinks);
			this.m_consumedBlinks.Shuffle<RandomEyeshineBlink>();
			this.m_queuedBlinks = new Queue<RandomEyeshineBlink>(this.m_consumedBlinks);
			this.m_consumedBlinks.Clear();
			this.SubscribeToBlinks();
		}

		// Token: 0x06005861 RID: 22625 RVA: 0x0007B0AD File Offset: 0x000792AD
		private void Start()
		{
			if (SkyDomeManager.SkyDomeController == null)
			{
				SkyDomeManager.SkydomeControllerChanged += this.SkyDomeControllerChanged;
				return;
			}
			this.SkyDomeControllerChanged();
		}

		// Token: 0x06005862 RID: 22626 RVA: 0x0007B0CE File Offset: 0x000792CE
		private void OnDestroy()
		{
			this.UnsubscribeFromBlinks();
			if (SkyDomeManager.SkyDomeController != null)
			{
				SkyDomeManager.SkyDomeController.DayNightChanged -= this.SkyDomeControllerOnDayNightChanged;
			}
		}

		// Token: 0x06005863 RID: 22627 RVA: 0x0007B0F3 File Offset: 0x000792F3
		private void SkyDomeControllerChanged()
		{
			if (SkyDomeManager.SkyDomeController != null)
			{
				SkyDomeManager.SkydomeControllerChanged -= this.SkyDomeControllerChanged;
				SkyDomeManager.SkyDomeController.DayNightChanged += this.SkyDomeControllerOnDayNightChanged;
				this.SkyDomeControllerOnDayNightChanged();
			}
		}

		// Token: 0x06005864 RID: 22628 RVA: 0x001E5CE8 File Offset: 0x001E3EE8
		private void SubscribeToBlinks()
		{
			for (int i = 0; i < this.m_blinks.Length; i++)
			{
				if (this.m_blinks[i])
				{
					this.m_blinks[i].enabled = false;
					this.m_blinks[i].StateClosed += this.OnStateChanged;
				}
			}
		}

		// Token: 0x06005865 RID: 22629 RVA: 0x001E5D40 File Offset: 0x001E3F40
		private void UnsubscribeFromBlinks()
		{
			for (int i = 0; i < this.m_blinks.Length; i++)
			{
				if (this.m_blinks[i])
				{
					this.m_blinks[i].StateClosed -= this.OnStateChanged;
				}
			}
		}

		// Token: 0x06005866 RID: 22630 RVA: 0x001E5D88 File Offset: 0x001E3F88
		private void OnStateChanged(RandomEyeshineBlink obj)
		{
			if (obj != this.m_currentBlink)
			{
				return;
			}
			obj.enabled = false;
			this.m_consumedBlinks.Add(obj);
			this.m_currentBlink = null;
			if (this.m_queuedBlinks.Count <= 0)
			{
				this.m_consumedBlinks.Shuffle<RandomEyeshineBlink>();
				for (int i = 0; i < this.m_consumedBlinks.Count; i++)
				{
					this.m_queuedBlinks.Enqueue(this.m_consumedBlinks[i]);
				}
				this.m_consumedBlinks.Clear();
			}
			if (!this.m_skyDomePresent || !SkyDomeManager.SkyDomeController.IsDay)
			{
				this.DequeueNext();
			}
		}

		// Token: 0x06005867 RID: 22631 RVA: 0x0007B129 File Offset: 0x00079329
		private void DequeueNext()
		{
			if (this.m_queuedBlinks.Count <= 0)
			{
				return;
			}
			this.m_currentBlink = this.m_queuedBlinks.Dequeue();
			if (this.m_currentBlink)
			{
				this.m_currentBlink.enabled = true;
			}
		}

		// Token: 0x06005868 RID: 22632 RVA: 0x0007B164 File Offset: 0x00079364
		private void SkyDomeControllerOnDayNightChanged()
		{
			if (this.m_currentBlink == null && SkyDomeManager.SkyDomeController != null && !SkyDomeManager.SkyDomeController.IsDay)
			{
				this.DequeueNext();
			}
		}

		// Token: 0x04004DCA RID: 19914
		[SerializeField]
		private RandomEyeshineBlink[] m_blinks;

		// Token: 0x04004DCB RID: 19915
		private Queue<RandomEyeshineBlink> m_queuedBlinks;

		// Token: 0x04004DCC RID: 19916
		private List<RandomEyeshineBlink> m_consumedBlinks;

		// Token: 0x04004DCD RID: 19917
		private RandomEyeshineBlink m_currentBlink;

		// Token: 0x04004DCE RID: 19918
		private bool m_skyDomePresent;
	}
}
