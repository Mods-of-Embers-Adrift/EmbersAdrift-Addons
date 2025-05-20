using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000CD RID: 205
	public class LightningGeneratorPath : LightningGenerator
	{
		// Token: 0x06000774 RID: 1908 RVA: 0x000AD700 File Offset: 0x000AB900
		public void GenerateLightningBoltPath(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters parameters)
		{
			if (parameters.Points.Count < 2)
			{
				Debug.LogError("Lightning path should have at least two points");
				return;
			}
			int generations = parameters.Generations;
			int totalGenerations = generations;
			float num = (generations == parameters.Generations) ? parameters.ChaosFactor : parameters.ChaosFactorForks;
			int num2 = parameters.SmoothingFactor - 1;
			LightningBoltSegmentGroup lightningBoltSegmentGroup = bolt.AddGroup();
			lightningBoltSegmentGroup.LineWidth = parameters.TrunkWidth;
			lightningBoltSegmentGroup.Generation = generations--;
			lightningBoltSegmentGroup.EndWidthMultiplier = parameters.EndWidthMultiplier;
			lightningBoltSegmentGroup.Color = parameters.Color;
			if (generations == parameters.Generations && (parameters.MainTrunkTintColor.r != 255 || parameters.MainTrunkTintColor.g != 255 || parameters.MainTrunkTintColor.b != 255 || parameters.MainTrunkTintColor.a != 255))
			{
				lightningBoltSegmentGroup.Color.r = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.r * (float)parameters.MainTrunkTintColor.r);
				lightningBoltSegmentGroup.Color.g = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.g * (float)parameters.MainTrunkTintColor.g);
				lightningBoltSegmentGroup.Color.b = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.b * (float)parameters.MainTrunkTintColor.b);
				lightningBoltSegmentGroup.Color.a = (byte)(0.003921569f * (float)lightningBoltSegmentGroup.Color.a * (float)parameters.MainTrunkTintColor.a);
			}
			parameters.Start = parameters.Points[0] + start;
			parameters.End = parameters.Points[parameters.Points.Count - 1] + end;
			end = parameters.Start;
			for (int i = 1; i < parameters.Points.Count; i++)
			{
				start = end;
				end = parameters.Points[i];
				Vector3 a = end - start;
				float num3 = PathGenerator.SquareRoot(a.sqrMagnitude);
				if (num > 0f)
				{
					if (bolt.CameraMode == CameraMode.Perspective)
					{
						end += num3 * num * base.RandomDirection3D(parameters.Random);
					}
					else if (bolt.CameraMode == CameraMode.OrthographicXY)
					{
						end += num3 * num * base.RandomDirection2D(parameters.Random);
					}
					else
					{
						end += num3 * num * base.RandomDirection2DXZ(parameters.Random);
					}
					a = end - start;
				}
				lightningBoltSegmentGroup.Segments.Add(new LightningBoltSegment
				{
					Start = start,
					End = end
				});
				float offsetAmount = num3 * num;
				Vector3 b;
				base.RandomVector(bolt, ref start, ref end, offsetAmount, parameters.Random, out b);
				if (base.ShouldCreateFork(parameters, generations, totalGenerations))
				{
					Vector3 b2 = a * parameters.ForkMultiplier() * (float)num2 * 0.5f;
					Vector3 end2 = end + b2 + b;
					base.GenerateLightningBoltStandard(bolt, start, end2, generations, totalGenerations, 0f, parameters);
				}
				num2 = ((num2 - 1) ?? (parameters.SmoothingFactor - 1));
			}
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00048141 File Offset: 0x00046341
		protected override void OnGenerateLightningBolt(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters parameters)
		{
			this.GenerateLightningBoltPath(bolt, start, end, parameters);
		}

		// Token: 0x040008C7 RID: 2247
		public static readonly LightningGeneratorPath PathGeneratorInstance = new LightningGeneratorPath();
	}
}
