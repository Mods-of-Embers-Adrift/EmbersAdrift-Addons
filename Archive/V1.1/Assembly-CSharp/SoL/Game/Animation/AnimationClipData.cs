using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D6C RID: 3436
	[Serializable]
	public class AnimationClipData
	{
		// Token: 0x170018D1 RID: 6353
		// (get) Token: 0x0600677F RID: 26495 RVA: 0x000858D0 File Offset: 0x00083AD0
		public float PlaySpeed
		{
			get
			{
				return this.m_playSpeed;
			}
		}

		// Token: 0x170018D2 RID: 6354
		// (get) Token: 0x06006780 RID: 26496 RVA: 0x000858D8 File Offset: 0x00083AD8
		public float FadeDuration
		{
			get
			{
				if (!this.m_customFadeDuration)
				{
					return 0.3f;
				}
				return this.m_fadeDuration;
			}
		}

		// Token: 0x170018D3 RID: 6355
		// (get) Token: 0x06006781 RID: 26497 RVA: 0x000858EE File Offset: 0x00083AEE
		public AnimationClip Clip
		{
			get
			{
				return this.m_clip;
			}
		}

		// Token: 0x170018D4 RID: 6356
		// (get) Token: 0x06006782 RID: 26498 RVA: 0x000858F6 File Offset: 0x00083AF6
		public float Duration
		{
			get
			{
				if (!(this.m_clip == null))
				{
					return Mathf.Abs(this.m_clip.length / this.m_playSpeed);
				}
				return 0f;
			}
		}

		// Token: 0x06006783 RID: 26499 RVA: 0x00213128 File Offset: 0x00211328
		public string GetElementLabel()
		{
			if (this.m_clip == null)
			{
				return "NONE";
			}
			string text = string.Format("[{0:F2}s] \"{1}\" @ {2:F1}x", this.Duration, this.m_clip.name, this.m_playSpeed);
			if (this.m_playSpeed < 0f)
			{
				text += "  (REVERSED)";
			}
			return text;
		}

		// Token: 0x06006784 RID: 26500 RVA: 0x00154B30 File Offset: 0x00152D30
		public float? GetExeTime()
		{
			return null;
		}

		// Token: 0x06006785 RID: 26501 RVA: 0x00045BC3 File Offset: 0x00043DC3
		private string GetExeLabel()
		{
			return string.Empty;
		}

		// Token: 0x040059FA RID: 23034
		private const string kTimingGroupName = "Timing";

		// Token: 0x040059FB RID: 23035
		[SerializeField]
		private AnimationClip m_clip;

		// Token: 0x040059FC RID: 23036
		[SerializeField]
		private float m_playSpeed = 1f;

		// Token: 0x040059FD RID: 23037
		[SerializeField]
		private bool m_customFadeDuration;

		// Token: 0x040059FE RID: 23038
		[SerializeField]
		private float m_fadeDuration = 0.3f;
	}
}
