using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D49 RID: 3401
	[Serializable]
	public class AbilityAnimation
	{
		// Token: 0x1700187C RID: 6268
		// (get) Token: 0x0600665D RID: 26205 RVA: 0x00084E86 File Offset: 0x00083086
		public string EDITOR_Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x0600665E RID: 26206 RVA: 0x00084E8E File Offset: 0x0008308E
		private string GetBeginLabel()
		{
			return this.GetLabelForSequence("Begin", this.m_beginSequence);
		}

		// Token: 0x0600665F RID: 26207 RVA: 0x00084EA1 File Offset: 0x000830A1
		private string GetFinishLabel()
		{
			return this.GetLabelForSequence("Finish", this.m_finishSequence);
		}

		// Token: 0x06006660 RID: 26208 RVA: 0x00084EB4 File Offset: 0x000830B4
		private string GetCancelLabel()
		{
			return this.GetLabelForSequence("Cancel", this.m_cancelSequence);
		}

		// Token: 0x06006661 RID: 26209 RVA: 0x002107A4 File Offset: 0x0020E9A4
		private string GetLabelForSequence(string sequenceName, AnimationSequenceWithOverride sequence)
		{
			string str = string.Empty;
			if (sequence != null)
			{
				str = (sequence.IsEmpty ? "[E] " : string.Format("[{0}] ", sequence.ClipData.Length));
			}
			return str + sequenceName + " Sequence";
		}

		// Token: 0x06006662 RID: 26210 RVA: 0x002107F0 File Offset: 0x0020E9F0
		public AnimationSequence GetAnimationSequence(AbilityAnimationType type)
		{
			switch (type)
			{
			case AbilityAnimationType.Start:
				return this.m_beginSequence;
			case AbilityAnimationType.Loop:
				throw new ArgumentException("Loop is no longer valid!");
			case AbilityAnimationType.End:
				return this.m_finishSequence;
			case AbilityAnimationType.Cancel:
				return this.m_cancelSequence;
			default:
				throw new ArgumentException("type");
			}
		}

		// Token: 0x040058FC RID: 22780
		public const string kGroupName = "Animation";

		// Token: 0x040058FD RID: 22781
		[SerializeField]
		private string m_description;

		// Token: 0x040058FE RID: 22782
		[SerializeField]
		private AnimationSequenceWithOverride m_beginSequence;

		// Token: 0x040058FF RID: 22783
		[SerializeField]
		private AnimationSequenceWithOverride m_finishSequence;

		// Token: 0x04005900 RID: 22784
		[SerializeField]
		private AnimationSequenceWithOverride m_cancelSequence;
	}
}
