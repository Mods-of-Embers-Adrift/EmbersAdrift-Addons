using System;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C0 RID: 192
	public abstract class LightningBoltPrefabScriptBase : LightningBoltScript
	{
		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x00047C36 File Offset: 0x00045E36
		// (set) Token: 0x06000704 RID: 1796 RVA: 0x00047C3E File Offset: 0x00045E3E
		public System.Random RandomOverride { get; set; }

		// Token: 0x06000705 RID: 1797 RVA: 0x00047C47 File Offset: 0x00045E47
		private void CalculateNextLightningTimestamp(float offset)
		{
			this.nextLightningTimestamp = ((this.IntervalRange.Minimum == this.IntervalRange.Maximum) ? this.IntervalRange.Minimum : (offset + this.IntervalRange.Random()));
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00047C81 File Offset: 0x00045E81
		private void CustomTransform(LightningCustomTransformStateInfo state)
		{
			if (this.CustomTransformHandler != null)
			{
				this.CustomTransformHandler.Invoke(state);
			}
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x000AB96C File Offset: 0x000A9B6C
		private void CallLightning()
		{
			this.CallLightning(null, null);
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x000AB994 File Offset: 0x000A9B94
		private void CallLightning(Vector3? start, Vector3? end)
		{
			System.Random r = this.RandomOverride ?? this.random;
			int num = this.CountRange.Random(r);
			for (int i = 0; i < num; i++)
			{
				LightningBoltParameters lightningBoltParameters = base.CreateParameters();
				if (this.CountProbabilityModifier >= 0.9999f || i == 0 || (float)lightningBoltParameters.Random.NextDouble() <= this.CountProbabilityModifier)
				{
					lightningBoltParameters.CustomTransform = ((this.CustomTransformHandler == null) ? null : new Action<LightningCustomTransformStateInfo>(this.CustomTransform));
					this.CreateLightningBolt(lightningBoltParameters);
					if (start != null)
					{
						lightningBoltParameters.Start = start.Value;
					}
					if (end != null)
					{
						lightningBoltParameters.End = end.Value;
					}
				}
				else
				{
					LightningBoltParameters.ReturnParametersToCache(lightningBoltParameters);
				}
			}
			this.CreateLightningBoltsNow();
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00047C97 File Offset: 0x00045E97
		protected void CreateLightningBoltsNow()
		{
			int maximumLightsPerBatch = LightningBolt.MaximumLightsPerBatch;
			LightningBolt.MaximumLightsPerBatch = this.MaximumLightsPerBatch;
			base.CreateLightningBolts(this.batchParameters);
			LightningBolt.MaximumLightsPerBatch = maximumLightsPerBatch;
			this.batchParameters.Clear();
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x000ABA5C File Offset: 0x000A9C5C
		protected override void PopulateParameters(LightningBoltParameters parameters)
		{
			base.PopulateParameters(parameters);
			parameters.RandomOverride = this.RandomOverride;
			float lifeTime = this.DurationRange.Random(parameters.Random);
			float trunkWidth = this.TrunkWidthRange.Random(parameters.Random);
			parameters.Generations = this.Generations;
			parameters.LifeTime = lifeTime;
			parameters.ChaosFactor = this.ChaosFactor;
			parameters.ChaosFactorForks = this.ChaosFactorForks;
			parameters.TrunkWidth = trunkWidth;
			parameters.Intensity = this.Intensity;
			parameters.GlowIntensity = this.GlowIntensity;
			parameters.GlowWidthMultiplier = this.GlowWidthMultiplier;
			parameters.Forkedness = this.Forkedness;
			parameters.ForkLengthMultiplier = this.ForkLengthMultiplier;
			parameters.ForkLengthVariance = this.ForkLengthVariance;
			parameters.FadePercent = this.FadePercent;
			parameters.FadeInMultiplier = this.FadeInMultiplier;
			parameters.FadeOutMultiplier = this.FadeOutMultiplier;
			parameters.FadeFullyLitMultiplier = this.FadeFullyLitMultiplier;
			parameters.GrowthMultiplier = this.GrowthMultiplier;
			parameters.EndWidthMultiplier = this.EndWidthMultiplier;
			parameters.ForkEndWidthMultiplier = this.ForkEndWidthMultiplier;
			parameters.DelayRange = this.DelayRange;
			parameters.LightParameters = this.LightParameters;
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x00047CC5 File Offset: 0x00045EC5
		protected override void Start()
		{
			base.Start();
			this.CalculateNextLightningTimestamp(0f);
			this.lifeTimeRemaining = ((this.LifeTime <= 0f) ? float.MaxValue : this.LifeTime);
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x000ABB88 File Offset: 0x000A9D88
		protected override void Update()
		{
			if (SceneCompositionManager.IsLoading)
			{
				return;
			}
			base.Update();
			if (Time.timeScale <= 0f)
			{
				return;
			}
			if ((this.lifeTimeRemaining -= LightningBoltScript.DeltaTime) < 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if ((this.nextLightningTimestamp -= LightningBoltScript.DeltaTime) <= 0f)
			{
				this.CalculateNextLightningTimestamp(this.nextLightningTimestamp);
				if (!this.ManualMode)
				{
					this.CallLightning();
				}
			}
			if (this.AutomaticModeSeconds > 0f)
			{
				this.AutomaticModeSeconds = Mathf.Max(0f, this.AutomaticModeSeconds - LightningBoltScript.DeltaTime);
				this.ManualMode = (this.AutomaticModeSeconds == 0f);
			}
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x00047CF8 File Offset: 0x00045EF8
		public override void CreateLightningBolt(LightningBoltParameters p)
		{
			this.batchParameters.Add(p);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x00047D06 File Offset: 0x00045F06
		public void Trigger()
		{
			this.Trigger(-1f);
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x00047D13 File Offset: 0x00045F13
		public void Trigger(float seconds)
		{
			this.CallLightning();
			if (seconds >= 0f)
			{
				this.AutomaticModeSeconds = Mathf.Max(0f, seconds);
			}
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00047D34 File Offset: 0x00045F34
		public void Trigger(Vector3? start, Vector3? end)
		{
			this.CallLightning(start, end);
		}

		// Token: 0x0400084E RID: 2126
		private readonly List<LightningBoltParameters> batchParameters = new List<LightningBoltParameters>();

		// Token: 0x0400084F RID: 2127
		private readonly System.Random random = new System.Random();

		// Token: 0x04000850 RID: 2128
		[Header("Lightning Spawn Properties")]
		[SingleLineClamp("How long to wait before creating another round of lightning bolts in seconds", 0.001, 1.7976931348623157E+308)]
		public RangeOfFloats IntervalRange = new RangeOfFloats
		{
			Minimum = 0.05f,
			Maximum = 0.1f
		};

		// Token: 0x04000851 RID: 2129
		[SingleLineClamp("How many lightning bolts to emit for each interval", 0.0, 100.0)]
		public RangeOfIntegers CountRange = new RangeOfIntegers
		{
			Minimum = 1,
			Maximum = 1
		};

		// Token: 0x04000852 RID: 2130
		[Tooltip("Reduces the probability that additional bolts from CountRange will actually happen (0 - 1).")]
		[Range(0f, 1f)]
		public float CountProbabilityModifier = 1f;

		// Token: 0x04000853 RID: 2131
		public RangeOfFloats DelayRange = new RangeOfFloats
		{
			Minimum = 0f,
			Maximum = 0f
		};

		// Token: 0x04000854 RID: 2132
		[SingleLineClamp("For each bolt emitted, how long should it stay in seconds", 0.01, 10.0)]
		public RangeOfFloats DurationRange = new RangeOfFloats
		{
			Minimum = 0.06f,
			Maximum = 0.12f
		};

		// Token: 0x04000855 RID: 2133
		[Header("Lightning Appearance Properties")]
		[SingleLineClamp("The trunk width range in unity units (x = min, y = max)", 0.0001, 100.0)]
		public RangeOfFloats TrunkWidthRange = new RangeOfFloats
		{
			Minimum = 0.1f,
			Maximum = 0.2f
		};

		// Token: 0x04000856 RID: 2134
		[Tooltip("How long (in seconds) this game object should live before destroying itself. Leave as 0 for infinite.")]
		[Range(0f, 1000f)]
		public float LifeTime;

		// Token: 0x04000857 RID: 2135
		[Tooltip("Generations (1 - 8, higher makes more detailed but more expensive lightning)")]
		[Range(1f, 8f)]
		public int Generations = 6;

		// Token: 0x04000858 RID: 2136
		[Tooltip("The chaos factor that determines how far the lightning main trunk can spread out, higher numbers spread out more. 0 - 1.")]
		[Range(0f, 1f)]
		public float ChaosFactor = 0.075f;

		// Token: 0x04000859 RID: 2137
		[Tooltip("The chaos factor that determines how far the forks of the lightning can spread out, higher numbers spread out more. 0 - 1.")]
		[Range(0f, 1f)]
		public float ChaosFactorForks = 0.095f;

		// Token: 0x0400085A RID: 2138
		[Tooltip("Intensity of the lightning")]
		[Range(0f, 10f)]
		public float Intensity = 1f;

		// Token: 0x0400085B RID: 2139
		[Tooltip("The intensity of the glow")]
		[Range(0f, 10f)]
		public float GlowIntensity = 0.1f;

		// Token: 0x0400085C RID: 2140
		[Tooltip("The width multiplier for the glow, 0 - 64")]
		[Range(0f, 64f)]
		public float GlowWidthMultiplier = 4f;

		// Token: 0x0400085D RID: 2141
		[Tooltip("What percent of time the lightning should fade in and out. For example, 0.15 fades in 15% of the time and fades out 15% of the time, with full visibility 70% of the time.")]
		[Range(0f, 0.5f)]
		public float FadePercent = 0.15f;

		// Token: 0x0400085E RID: 2142
		[Tooltip("Modify the duration of lightning fade in.")]
		[Range(0f, 1f)]
		public float FadeInMultiplier = 1f;

		// Token: 0x0400085F RID: 2143
		[Tooltip("Modify the duration of fully lit lightning.")]
		[Range(0f, 1f)]
		public float FadeFullyLitMultiplier = 1f;

		// Token: 0x04000860 RID: 2144
		[Tooltip("Modify the duration of lightning fade out.")]
		[Range(0f, 1f)]
		public float FadeOutMultiplier = 1f;

		// Token: 0x04000861 RID: 2145
		[Tooltip("0 - 1, how slowly the lightning should grow. 0 for instant, 1 for slow.")]
		[Range(0f, 1f)]
		public float GrowthMultiplier;

		// Token: 0x04000862 RID: 2146
		[Tooltip("How much smaller the lightning should get as it goes towards the end of the bolt. For example, 0.5 will make the end 50% the width of the start.")]
		[Range(0f, 10f)]
		public float EndWidthMultiplier = 0.5f;

		// Token: 0x04000863 RID: 2147
		[Tooltip("How forked should the lightning be? (0 - 1, 0 for none, 1 for lots of forks)")]
		[Range(0f, 1f)]
		public float Forkedness = 0.25f;

		// Token: 0x04000864 RID: 2148
		[Range(0f, 10f)]
		[Tooltip("Minimum distance multiplier for forks")]
		public float ForkLengthMultiplier = 0.6f;

		// Token: 0x04000865 RID: 2149
		[Range(0f, 10f)]
		[Tooltip("Fork distance multiplier variance. Random range of 0 to n that is added to Fork Length Multiplier.")]
		public float ForkLengthVariance = 0.2f;

		// Token: 0x04000866 RID: 2150
		[Tooltip("Forks have their EndWidthMultiplier multiplied by this value")]
		[Range(0f, 10f)]
		public float ForkEndWidthMultiplier = 1f;

		// Token: 0x04000867 RID: 2151
		[Header("Lightning Light Properties")]
		[Tooltip("Light parameters")]
		public LightningLightParameters LightParameters;

		// Token: 0x04000868 RID: 2152
		[Tooltip("Maximum number of lights that can be created per batch of lightning")]
		[Range(0f, 64f)]
		public int MaximumLightsPerBatch = 8;

		// Token: 0x04000869 RID: 2153
		[Header("Lightning Trigger Type")]
		[Tooltip("Manual or automatic mode. Manual requires that you call the Trigger method in script. Automatic uses the interval to create lightning continuously.")]
		public bool ManualMode;

		// Token: 0x0400086A RID: 2154
		[Tooltip("Turns lightning into automatic mode for this number of seconds, then puts it into manual mode.")]
		[Range(0f, 120f)]
		public float AutomaticModeSeconds;

		// Token: 0x0400086B RID: 2155
		[Header("Lightning custom transform handler")]
		[Tooltip("Custom handler to modify the transform of each lightning bolt, useful if it will be alive longer than a few frames and needs to scale and rotate based on the position of other objects.")]
		public LightningCustomTransformDelegate CustomTransformHandler;

		// Token: 0x0400086D RID: 2157
		private float nextLightningTimestamp;

		// Token: 0x0400086E RID: 2158
		private float lifeTimeRemaining;
	}
}
