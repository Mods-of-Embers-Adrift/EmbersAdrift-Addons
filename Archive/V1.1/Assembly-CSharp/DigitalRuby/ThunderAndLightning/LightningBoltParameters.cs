using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000AE RID: 174
	[Serializable]
	public sealed class LightningBoltParameters
	{
		// Token: 0x06000683 RID: 1667 RVA: 0x000A8A50 File Offset: 0x000A6C50
		static LightningBoltParameters()
		{
			string[] names = QualitySettings.names;
			for (int i = 0; i < names.Length; i++)
			{
				switch (i)
				{
				case 0:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 3,
						MaximumLightPercent = 0f,
						MaximumShadowPercent = 0f
					};
					break;
				case 1:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 4,
						MaximumLightPercent = 0f,
						MaximumShadowPercent = 0f
					};
					break;
				case 2:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 5,
						MaximumLightPercent = 0.1f,
						MaximumShadowPercent = 0f
					};
					break;
				case 3:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 5,
						MaximumLightPercent = 0.1f,
						MaximumShadowPercent = 0f
					};
					break;
				case 4:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 6,
						MaximumLightPercent = 0.05f,
						MaximumShadowPercent = 0.1f
					};
					break;
				case 5:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 7,
						MaximumLightPercent = 0.025f,
						MaximumShadowPercent = 0.05f
					};
					break;
				default:
					LightningBoltParameters.QualityMaximums[i] = new LightningQualityMaximum
					{
						MaximumGenerations = 8,
						MaximumLightPercent = 0.025f,
						MaximumShadowPercent = 0.05f
					};
					break;
				}
			}
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x000A8C18 File Offset: 0x000A6E18
		public LightningBoltParameters()
		{
			this.random = (this.currentRandom = new System.Random(LightningBoltParameters.randomSeed++));
			this.Points = new List<Vector3>();
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000685 RID: 1669 RVA: 0x0004777C File Offset: 0x0004597C
		// (set) Token: 0x06000686 RID: 1670 RVA: 0x000A8D0C File Offset: 0x000A6F0C
		public int Generations
		{
			get
			{
				return this.generations;
			}
			set
			{
				int b = Mathf.Clamp(value, 1, 8);
				if (this.quality == LightningBoltQualitySetting.UseScript)
				{
					this.generations = b;
					return;
				}
				int qualityLevel = QualitySettings.GetQualityLevel();
				LightningQualityMaximum lightningQualityMaximum;
				if (LightningBoltParameters.QualityMaximums.TryGetValue(qualityLevel, out lightningQualityMaximum))
				{
					this.generations = Mathf.Min(lightningQualityMaximum.MaximumGenerations, b);
					return;
				}
				this.generations = b;
				Debug.LogError("Unable to read lightning quality settings from level " + qualityLevel.ToString());
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x00047784 File Offset: 0x00045984
		// (set) Token: 0x06000688 RID: 1672 RVA: 0x0004778C File Offset: 0x0004598C
		public System.Random Random
		{
			get
			{
				return this.currentRandom;
			}
			set
			{
				this.random = (value ?? this.random);
				this.currentRandom = (this.randomOverride ?? this.random);
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000689 RID: 1673 RVA: 0x000477B5 File Offset: 0x000459B5
		// (set) Token: 0x0600068A RID: 1674 RVA: 0x000477BD File Offset: 0x000459BD
		public System.Random RandomOverride
		{
			get
			{
				return this.randomOverride;
			}
			set
			{
				this.randomOverride = value;
				this.currentRandom = (this.randomOverride ?? this.random);
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x000477DC File Offset: 0x000459DC
		// (set) Token: 0x0600068C RID: 1676 RVA: 0x000477E4 File Offset: 0x000459E4
		public float GrowthMultiplier
		{
			get
			{
				return this.growthMultiplier;
			}
			set
			{
				this.growthMultiplier = Mathf.Clamp(value, 0f, 0.999f);
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x000477FC File Offset: 0x000459FC
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x00047804 File Offset: 0x00045A04
		public List<Vector3> Points { get; set; }

		// Token: 0x0600068F RID: 1679 RVA: 0x0004780D File Offset: 0x00045A0D
		public float ForkMultiplier()
		{
			return (float)this.Random.NextDouble() * this.ForkLengthVariance + this.ForkLengthMultiplier;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x000A8D78 File Offset: 0x000A6F78
		public Vector3 ApplyVariance(Vector3 pos, Vector3 variance)
		{
			return new Vector3(pos.x + ((float)this.Random.NextDouble() * 2f - 1f) * variance.x, pos.y + ((float)this.Random.NextDouble() * 2f - 1f) * variance.y, pos.z + ((float)this.Random.NextDouble() * 2f - 1f) * variance.z);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x000A8DFC File Offset: 0x000A6FFC
		public void Reset()
		{
			this.Start = (this.End = Vector3.zero);
			this.Generator = null;
			this.SmoothingFactor = 0;
			this.RandomOverride = null;
			this.CustomTransform = null;
			if (this.Points != null)
			{
				this.Points.Clear();
			}
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x000A8E4C File Offset: 0x000A704C
		public static LightningBoltParameters GetOrCreateParameters()
		{
			LightningBoltParameters result;
			if (LightningBoltParameters.cache.Count == 0)
			{
				result = new LightningBoltParameters();
			}
			else
			{
				int index = LightningBoltParameters.cache.Count - 1;
				result = LightningBoltParameters.cache[index];
				LightningBoltParameters.cache.RemoveAt(index);
			}
			return result;
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x00047829 File Offset: 0x00045A29
		public static void ReturnParametersToCache(LightningBoltParameters p)
		{
			if (!LightningBoltParameters.cache.Contains(p))
			{
				p.Reset();
				LightningBoltParameters.cache.Add(p);
			}
		}

		// Token: 0x0400079C RID: 1948
		private static int randomSeed = Environment.TickCount;

		// Token: 0x0400079D RID: 1949
		private static readonly List<LightningBoltParameters> cache = new List<LightningBoltParameters>();

		// Token: 0x0400079E RID: 1950
		internal int generationWhereForksStop;

		// Token: 0x0400079F RID: 1951
		internal int forkednessCalculated;

		// Token: 0x040007A0 RID: 1952
		internal LightningBoltQualitySetting quality;

		// Token: 0x040007A1 RID: 1953
		internal float delaySeconds;

		// Token: 0x040007A2 RID: 1954
		internal int maxLights;

		// Token: 0x040007A3 RID: 1955
		public static float Scale = 1f;

		// Token: 0x040007A4 RID: 1956
		public static readonly Dictionary<int, LightningQualityMaximum> QualityMaximums = new Dictionary<int, LightningQualityMaximum>();

		// Token: 0x040007A5 RID: 1957
		public LightningGenerator Generator;

		// Token: 0x040007A6 RID: 1958
		public Vector3 Start;

		// Token: 0x040007A7 RID: 1959
		public Vector3 End;

		// Token: 0x040007A8 RID: 1960
		public Vector3 StartVariance;

		// Token: 0x040007A9 RID: 1961
		public Vector3 EndVariance;

		// Token: 0x040007AA RID: 1962
		public Action<LightningCustomTransformStateInfo> CustomTransform;

		// Token: 0x040007AB RID: 1963
		private int generations;

		// Token: 0x040007AC RID: 1964
		public float LifeTime;

		// Token: 0x040007AD RID: 1965
		public float Delay;

		// Token: 0x040007AE RID: 1966
		public RangeOfFloats DelayRange;

		// Token: 0x040007AF RID: 1967
		public float ChaosFactor;

		// Token: 0x040007B0 RID: 1968
		public float ChaosFactorForks = -1f;

		// Token: 0x040007B1 RID: 1969
		public float TrunkWidth;

		// Token: 0x040007B2 RID: 1970
		public float EndWidthMultiplier = 0.5f;

		// Token: 0x040007B3 RID: 1971
		public float Intensity = 1f;

		// Token: 0x040007B4 RID: 1972
		public float GlowIntensity;

		// Token: 0x040007B5 RID: 1973
		public float GlowWidthMultiplier;

		// Token: 0x040007B6 RID: 1974
		public float Forkedness;

		// Token: 0x040007B7 RID: 1975
		public int GenerationWhereForksStopSubtractor = 5;

		// Token: 0x040007B8 RID: 1976
		public Color32 Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		// Token: 0x040007B9 RID: 1977
		public Color32 MainTrunkTintColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		// Token: 0x040007BA RID: 1978
		private System.Random random;

		// Token: 0x040007BB RID: 1979
		private System.Random currentRandom;

		// Token: 0x040007BC RID: 1980
		private System.Random randomOverride;

		// Token: 0x040007BD RID: 1981
		public float FadePercent = 0.15f;

		// Token: 0x040007BE RID: 1982
		public float FadeInMultiplier = 1f;

		// Token: 0x040007BF RID: 1983
		public float FadeFullyLitMultiplier = 1f;

		// Token: 0x040007C0 RID: 1984
		public float FadeOutMultiplier = 1f;

		// Token: 0x040007C1 RID: 1985
		private float growthMultiplier;

		// Token: 0x040007C2 RID: 1986
		public float ForkLengthMultiplier = 0.6f;

		// Token: 0x040007C3 RID: 1987
		public float ForkLengthVariance = 0.2f;

		// Token: 0x040007C4 RID: 1988
		public float ForkEndWidthMultiplier = 1f;

		// Token: 0x040007C5 RID: 1989
		public LightningLightParameters LightParameters;

		// Token: 0x040007C7 RID: 1991
		public int SmoothingFactor;
	}
}
