using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A80 RID: 2688
	[Serializable]
	public class HandheldItemRequirement
	{
		// Token: 0x170012D7 RID: 4823
		// (get) Token: 0x06005314 RID: 21268 RVA: 0x00077676 File Offset: 0x00075876
		public HandheldItemFlags MainHandFlags
		{
			get
			{
				return this.m_mainHand;
			}
		}

		// Token: 0x170012D8 RID: 4824
		// (get) Token: 0x06005315 RID: 21269 RVA: 0x0007767E File Offset: 0x0007587E
		public HandheldItemFlags OffHandFlags
		{
			get
			{
				return this.m_offHand;
			}
		}

		// Token: 0x06005316 RID: 21270 RVA: 0x00077686 File Offset: 0x00075886
		public HandheldItemRequirement()
		{
			this.m_mainHand = HandheldItemFlags.None;
			this.m_offHand = HandheldItemFlags.None;
		}

		// Token: 0x06005317 RID: 21271 RVA: 0x0007769C File Offset: 0x0007589C
		public HandheldItemRequirement(HandheldItemFlags mainHand, HandheldItemFlags offHand)
		{
			this.m_mainHand = mainHand;
			this.m_offHand = offHand;
		}

		// Token: 0x06005318 RID: 21272 RVA: 0x000776B2 File Offset: 0x000758B2
		public bool MeetsRequirements(HandheldItemFlags mainHandFlags, HandheldItemFlags offHandFlags)
		{
			return this.MeetsSingleRequirement(this.m_mainHand, mainHandFlags) && this.MeetsSingleRequirement(this.m_offHand, offHandFlags);
		}

		// Token: 0x06005319 RID: 21273 RVA: 0x000776D2 File Offset: 0x000758D2
		public bool MeetsRequirements(HandheldFlagConfig config)
		{
			return this.MeetsRequirements(config.MainHand, config.OffHand);
		}

		// Token: 0x0600531A RID: 21274 RVA: 0x001D7004 File Offset: 0x001D5204
		public bool MeetsRequirements(GameEntity entity)
		{
			HandheldFlagConfig handheldFlagConfig = entity.GetHandheldFlagConfig();
			return this.MeetsRequirements(handheldFlagConfig);
		}

		// Token: 0x0600531B RID: 21275 RVA: 0x000776E6 File Offset: 0x000758E6
		private bool MeetsSingleRequirement(HandheldItemFlags internalFlags, HandheldItemFlags inputFlag)
		{
			if (internalFlags == HandheldItemFlags.None)
			{
				return true;
			}
			if (internalFlags != HandheldItemFlags.Empty)
			{
				return internalFlags.ContainsAnyFlag(inputFlag);
			}
			return inputFlag == HandheldItemFlags.Empty;
		}

		// Token: 0x0600531C RID: 21276 RVA: 0x001D7020 File Offset: 0x001D5220
		public string GetDescription(HandheldFlagConfig? config)
		{
			if (config != null)
			{
				Color color = this.MeetsSingleRequirement(this.m_mainHand, config.Value.MainHand) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				Color color2 = this.MeetsSingleRequirement(this.m_offHand, config.Value.OffHand) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				return ZString.Format<string, string, string, string>("MH: <color={0}>{1}</color> & OH: <color={2}>{3}</color>", color.ToHex(), this.m_mainHand.ToString(), color2.ToHex(), this.m_offHand.ToString());
			}
			return ZString.Format<string, string>("MH: {0} & OH: {1}", this.m_mainHand.ToString(), this.m_offHand.ToString());
		}

		// Token: 0x04004A5C RID: 19036
		[SerializeField]
		private HandheldItemFlags m_mainHand;

		// Token: 0x04004A5D RID: 19037
		[SerializeField]
		private HandheldItemFlags m_offHand;
	}
}
