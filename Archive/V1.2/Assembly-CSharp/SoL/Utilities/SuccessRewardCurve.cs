using System;
using System.Collections.Generic;
using Cysharp.Text;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200026A RID: 618
	[Serializable]
	public class SuccessRewardCurve
	{
		// Token: 0x0600138F RID: 5007 RVA: 0x000F70EC File Offset: 0x000F52EC
		public float GetTotalForLevel(int level)
		{
			if (!Application.isPlaying || this.m_totalCache == null)
			{
				this.PopulateCache();
			}
			float result;
			if (!this.m_totalCache.TryGetValue(level, out result))
			{
				return float.MaxValue;
			}
			return result;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x000F7128 File Offset: 0x000F5328
		public float GetRewardForLevel(int level)
		{
			if (!Application.isPlaying || this.m_rewardCache == null)
			{
				this.PopulateCache();
			}
			float result;
			if (!this.m_rewardCache.TryGetValue(level, out result))
			{
				return 0f;
			}
			return result;
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x000F7164 File Offset: 0x000F5364
		private void PopulateCache()
		{
			this.m_totalCache = new Dictionary<int, float>();
			this.m_rewardCache = new Dictionary<int, float>();
			for (int i = 1; i <= 50; i++)
			{
				float num = this.m_useRawSuccess ? ((float)this.m_rawSuccess[i]) : this.m_success.GetValue((float)i);
				float value = this.m_reward.GetValue((float)i);
				float value2 = this.m_reward.GetValue((float)(i - 1));
				this.m_totalCache.Add(i, num * value2);
				this.m_rewardCache.Add(i, value);
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06001392 RID: 5010 RVA: 0x0004FC84 File Offset: 0x0004DE84
		// (set) Token: 0x06001393 RID: 5011 RVA: 0x0004FC8C File Offset: 0x0004DE8C
		[JsonProperty]
		[BsonElement]
		public bool UseRawSuccess
		{
			get
			{
				return this.m_useRawSuccess;
			}
			private set
			{
				this.m_useRawSuccess = value;
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06001394 RID: 5012 RVA: 0x0004FC95 File Offset: 0x0004DE95
		// (set) Token: 0x06001395 RID: 5013 RVA: 0x0004FC9D File Offset: 0x0004DE9D
		[JsonProperty]
		[BsonElement]
		public int[] RawSuccess
		{
			get
			{
				return this.m_rawSuccess;
			}
			private set
			{
				this.m_rawSuccess = value;
			}
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x0004FCA6 File Offset: 0x0004DEA6
		// (set) Token: 0x06001397 RID: 5015 RVA: 0x0004FCAE File Offset: 0x0004DEAE
		[JsonProperty]
		[BsonElement]
		public ParameterizedLevelCurve Success
		{
			get
			{
				return this.m_success;
			}
			private set
			{
				this.m_success = value;
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06001398 RID: 5016 RVA: 0x0004FCB7 File Offset: 0x0004DEB7
		// (set) Token: 0x06001399 RID: 5017 RVA: 0x0004FCBF File Offset: 0x0004DEBF
		[JsonProperty]
		[BsonElement]
		public ParameterizedLevelCurve Reward
		{
			get
			{
				return this.m_reward;
			}
			private set
			{
				this.m_reward = value;
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x0004FCC8 File Offset: 0x0004DEC8
		// (set) Token: 0x0600139B RID: 5019 RVA: 0x0004FCD0 File Offset: 0x0004DED0
		[JsonProperty]
		[BsonElement]
		public float GroupSplitMultiplier
		{
			get
			{
				return this.m_groupSplitMultiplier;
			}
			private set
			{
				this.m_groupSplitMultiplier = value;
			}
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x000F71F0 File Offset: 0x000F53F0
		public override string ToString()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendFormat<bool>("  UseRawSuccess: {0}\n", this.m_useRawSuccess);
				if (this.m_useRawSuccess)
				{
					utf16ValueStringBuilder.Append("  RawSuccess:\n");
					for (int i = 0; i < this.m_rawSuccess.Length; i++)
					{
						utf16ValueStringBuilder.AppendFormat<int, int>("    [{0}] {1}\n", i, this.m_rawSuccess[i]);
					}
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<string>("  SuccessCurve: {0}\n", this.m_success.ToString());
				}
				utf16ValueStringBuilder.AppendFormat<string>("  RewardCurve: {0}\n", this.m_reward.ToString());
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x04001BE3 RID: 7139
		private const string kRawSuccess = "Raw Success";

		// Token: 0x04001BE4 RID: 7140
		[SerializeField]
		private bool m_useRawSuccess;

		// Token: 0x04001BE5 RID: 7141
		[SerializeField]
		private int[] m_rawSuccess;

		// Token: 0x04001BE6 RID: 7142
		[JsonIgnore]
		[BsonIgnore]
		[SerializeField]
		private AnimationCurve m_rawSuccessVisualization;

		// Token: 0x04001BE7 RID: 7143
		[SerializeField]
		private ParameterizedLevelCurve m_success;

		// Token: 0x04001BE8 RID: 7144
		[SerializeField]
		private ParameterizedLevelCurve m_reward;

		// Token: 0x04001BE9 RID: 7145
		[SerializeField]
		private float m_groupSplitMultiplier = 1.5f;

		// Token: 0x04001BEA RID: 7146
		[NonSerialized]
		private Dictionary<int, float> m_totalCache;

		// Token: 0x04001BEB RID: 7147
		[NonSerialized]
		private Dictionary<int, float> m_rewardCache;
	}
}
