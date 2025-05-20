using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000CC RID: 204
	public class LightningGenerator
	{
		// Token: 0x06000767 RID: 1895 RVA: 0x000AD01C File Offset: 0x000AB21C
		private void GetPerpendicularVector(ref Vector3 directionNormalized, out Vector3 side)
		{
			if (directionNormalized == Vector3.zero)
			{
				side = Vector3.right;
				return;
			}
			float x = directionNormalized.x;
			float y = directionNormalized.y;
			float z = directionNormalized.z;
			float num = Mathf.Abs(x);
			float num2 = Mathf.Abs(y);
			float num3 = Mathf.Abs(z);
			float num4;
			float num5;
			float num6;
			if (num >= num2 && num2 >= num3)
			{
				num4 = 1f;
				num5 = 1f;
				num6 = -(y * num4 + z * num5) / x;
			}
			else if (num2 >= num3)
			{
				num6 = 1f;
				num5 = 1f;
				num4 = -(x * num6 + z * num5) / y;
			}
			else
			{
				num6 = 1f;
				num4 = 1f;
				num5 = -(x * num6 + y * num4) / z;
			}
			side = new Vector3(num6, num4, num5).normalized;
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x000480E9 File Offset: 0x000462E9
		protected virtual void OnGenerateLightningBolt(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters parameters)
		{
			this.GenerateLightningBoltStandard(bolt, start, end, parameters.Generations, parameters.Generations, 0f, parameters);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00048109 File Offset: 0x00046309
		public bool ShouldCreateFork(LightningBoltParameters parameters, int generation, int totalGenerations)
		{
			return generation > parameters.generationWhereForksStop && generation >= totalGenerations - parameters.forkednessCalculated && (float)parameters.Random.NextDouble() < parameters.Forkedness;
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x000AD0F0 File Offset: 0x000AB2F0
		public void CreateFork(LightningBolt bolt, LightningBoltParameters parameters, int generation, int totalGenerations, Vector3 start, Vector3 midPoint)
		{
			if (this.ShouldCreateFork(parameters, generation, totalGenerations))
			{
				Vector3 b = (midPoint - start) * parameters.ForkMultiplier();
				Vector3 end = midPoint + b;
				this.GenerateLightningBoltStandard(bolt, midPoint, end, generation, totalGenerations, 0f, parameters);
			}
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x000AD13C File Offset: 0x000AB33C
		public void GenerateLightningBoltStandard(LightningBolt bolt, Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount, LightningBoltParameters parameters)
		{
			if (generation < 1)
			{
				return;
			}
			LightningBoltSegmentGroup lightningBoltSegmentGroup = bolt.AddGroup();
			lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
			{
				Start = start,
				End = end
			});
			float num = (float)generation / (float)totalGenerations;
			num *= num;
			lightningBoltSegmentGroup.LineWidth = parameters.TrunkWidth * num;
			lightningBoltSegmentGroup.Generation = generation;
			lightningBoltSegmentGroup.Color = parameters.Color;
			if (generation == parameters.Generations && (parameters.MainTrunkTintColor.r != 255 || parameters.MainTrunkTintColor.g != 255 || parameters.MainTrunkTintColor.b != 255 || parameters.MainTrunkTintColor.a != 255))
			{
				lightningBoltSegmentGroup.Color.r = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.r * (float)parameters.MainTrunkTintColor.r);
				lightningBoltSegmentGroup.Color.g = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.g * (float)parameters.MainTrunkTintColor.g);
				lightningBoltSegmentGroup.Color.b = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.b * (float)parameters.MainTrunkTintColor.b);
				lightningBoltSegmentGroup.Color.a = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.a * (float)parameters.MainTrunkTintColor.a);
			}
			lightningBoltSegmentGroup.Color.a = (byte)(255f * num);
			lightningBoltSegmentGroup.EndWidthMultiplier = parameters.EndWidthMultiplier * parameters.ForkEndWidthMultiplier;
			if (offsetAmount <= 0f)
			{
				offsetAmount = (end - start).magnitude * ((generation == totalGenerations) ? parameters.ChaosFactor : parameters.ChaosFactorForks);
			}
			while (generation-- > 0)
			{
				int startIndex = lightningBoltSegmentGroup.StartIndex;
				lightningBoltSegmentGroup.StartIndex = lightningBoltSegmentGroup.Segments.Count;
				for (int i = startIndex; i < lightningBoltSegmentGroup.StartIndex; i++)
				{
					start = lightningBoltSegmentGroup.Segments[i].Start;
					end = lightningBoltSegmentGroup.Segments[i].End;
					Vector3 vector = (start + end) * 0.5f;
					Vector3 b;
					this.RandomVector(bolt, ref start, ref end, offsetAmount, parameters.Random, out b);
					vector += b;
					lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
					{
						Start = start,
						End = vector
					});
					lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
					{
						Start = vector,
						End = end
					});
					this.CreateFork(bolt, parameters, generation, totalGenerations, start, vector);
				}
				offsetAmount *= 0.5f;
			}
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x000AD418 File Offset: 0x000AB618
		public Vector3 RandomDirection3D(System.Random random)
		{
			float num = 2f * (float)random.NextDouble() - 1f;
			Vector3 result = this.RandomDirection2D(random) * Mathf.Sqrt(1f - num * num);
			result.z = num;
			return result;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x000AD460 File Offset: 0x000AB660
		public Vector3 RandomDirection2D(System.Random random)
		{
			float f = (float)random.NextDouble() * 2f * 3.1415927f;
			return new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x000AD498 File Offset: 0x000AB698
		public Vector3 RandomDirection2DXZ(System.Random random)
		{
			float f = (float)random.NextDouble() * 2f * 3.1415927f;
			return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x000AD4D0 File Offset: 0x000AB6D0
		public void RandomVector(LightningBolt bolt, ref Vector3 start, ref Vector3 end, float offsetAmount, System.Random random, out Vector3 result)
		{
			if (bolt.CameraMode == CameraMode.Perspective)
			{
				Vector3 vector = (end - start).normalized;
				Vector3 vector2 = Vector3.Cross(start, end);
				if (vector2 == Vector3.zero)
				{
					this.GetPerpendicularVector(ref vector, out vector2);
				}
				else
				{
					vector2.Normalize();
				}
				float d = ((float)random.NextDouble() + 0.1f) * offsetAmount;
				float num = (float)random.NextDouble() * 3.1415927f;
				vector *= (float)Math.Sin((double)num);
				Quaternion rotation;
				rotation.x = vector.x;
				rotation.y = vector.y;
				rotation.z = vector.z;
				rotation.w = (float)Math.Cos((double)num);
				result = rotation * vector2 * d;
				return;
			}
			if (bolt.CameraMode == CameraMode.OrthographicXY)
			{
				end.z = start.z;
				Vector3 normalized = (end - start).normalized;
				Vector3 a = new Vector3(-normalized.y, normalized.x, 0f);
				float d2 = (float)random.NextDouble() * offsetAmount * 2f - offsetAmount;
				result = a * d2;
				return;
			}
			end.y = start.y;
			Vector3 normalized2 = (end - start).normalized;
			Vector3 a2 = new Vector3(-normalized2.z, 0f, normalized2.x);
			float d3 = (float)random.NextDouble() * offsetAmount * 2f - offsetAmount;
			result = a2 * d3;
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x000AD690 File Offset: 0x000AB890
		public void GenerateLightningBolt(LightningBolt bolt, LightningBoltParameters parameters)
		{
			Vector3 vector;
			Vector3 vector2;
			this.GenerateLightningBolt(bolt, parameters, out vector, out vector2);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x000AD6AC File Offset: 0x000AB8AC
		public void GenerateLightningBolt(LightningBolt bolt, LightningBoltParameters parameters, out Vector3 start, out Vector3 end)
		{
			start = parameters.ApplyVariance(parameters.Start, parameters.StartVariance);
			end = parameters.ApplyVariance(parameters.End, parameters.EndVariance);
			this.OnGenerateLightningBolt(bolt, start, end, parameters);
		}

		// Token: 0x040008C4 RID: 2244
		internal const float oneOver255 = 0.003921569f;

		// Token: 0x040008C5 RID: 2245
		internal const float mainTrunkMultiplier = 0.003921569f;

		// Token: 0x040008C6 RID: 2246
		public static readonly LightningGenerator GeneratorInstance = new LightningGenerator();
	}
}
