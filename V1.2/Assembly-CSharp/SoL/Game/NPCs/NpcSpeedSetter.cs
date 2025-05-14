using System;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000816 RID: 2070
	public class NpcSpeedSetter : MonoBehaviour
	{
		// Token: 0x06003C01 RID: 15361 RVA: 0x00068A5E File Offset: 0x00066C5E
		public float GetWalkSpeed()
		{
			return ((this.m_useAnimationClips && this.m_walkClip != null) ? (this.GetSpeed(this.m_walkClip) * this.m_walkClipSpeedMultiplier) : this.m_walkSpeed) * this.m_scale;
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x00068A98 File Offset: 0x00066C98
		public float GetRunSpeed()
		{
			return ((this.m_useAnimationClips && this.m_runClip != null) ? (this.GetSpeed(this.m_runClip) * this.m_runClipSpeedMultiplier) : this.m_runSpeed) * this.m_scale;
		}

		// Token: 0x06003C03 RID: 15363 RVA: 0x0017DDF4 File Offset: 0x0017BFF4
		private float GetSpeed(AnimationClip clip)
		{
			if (clip == null)
			{
				throw new ArgumentException("clip");
			}
			switch (this.m_speedAxis)
			{
			case NpcSpeedSetter.SpeedAxis.X:
				return clip.averageSpeed.x;
			case NpcSpeedSetter.SpeedAxis.Y:
				return clip.averageSpeed.y;
			case NpcSpeedSetter.SpeedAxis.Z:
				return clip.averageSpeed.z;
			default:
				throw new ArgumentException("m_speedAxis");
			}
		}

		// Token: 0x04003AAB RID: 15019
		[SerializeField]
		private bool m_useAnimationClips;

		// Token: 0x04003AAC RID: 15020
		[SerializeField]
		private NpcSpeedSetter.SpeedAxis m_speedAxis = NpcSpeedSetter.SpeedAxis.Y;

		// Token: 0x04003AAD RID: 15021
		[SerializeField]
		private AnimationClip m_walkClip;

		// Token: 0x04003AAE RID: 15022
		[SerializeField]
		private AnimationClip m_runClip;

		// Token: 0x04003AAF RID: 15023
		[SerializeField]
		private float m_walkClipSpeedMultiplier = 1f;

		// Token: 0x04003AB0 RID: 15024
		[SerializeField]
		private float m_runClipSpeedMultiplier = 1f;

		// Token: 0x04003AB1 RID: 15025
		[SerializeField]
		private float m_walkSpeed = 1.5f;

		// Token: 0x04003AB2 RID: 15026
		[SerializeField]
		private float m_runSpeed = 3.5f;

		// Token: 0x04003AB3 RID: 15027
		[Tooltip("use this ONLY if the prefab's default scale is not 1,1,1")]
		[SerializeField]
		private float m_scale = 1f;

		// Token: 0x02000817 RID: 2071
		private enum SpeedAxis
		{
			// Token: 0x04003AB5 RID: 15029
			X,
			// Token: 0x04003AB6 RID: 15030
			Y,
			// Token: 0x04003AB7 RID: 15031
			Z
		}
	}
}
