using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001E7 RID: 487
	public class BGTestCurveShowcase : MonoBehaviour
	{
		// Token: 0x06001122 RID: 4386 RVA: 0x000E2A60 File Offset: 0x000E0C60
		private void Start()
		{
			this.effects.Add(new BGTestCurveShowcase.EffectScale(base.gameObject, 1f, 2f, BGTestCurveShowcase.FromScale, BGTestCurveShowcase.ToScale));
			this.effects.Add(new BGTestCurveShowcase.EffectScale(this.B, 1f, 2f, BGTestCurveShowcase.FromScale, BGTestCurveShowcase.ToScale));
			this.effects.Add(new BGTestCurveShowcase.EffectScale(this.G, 1f, 2f, BGTestCurveShowcase.FromScale, BGTestCurveShowcase.ToScale));
			this.effects.Add(new BGTestCurveShowcase.EffectScale(this.Curve.gameObject, 1f, 2f, BGTestCurveShowcase.FromScale, BGTestCurveShowcase.ToScale));
			this.effects.Add(new BGTestCurveShowcase.EffectRotate(BGTestCurveShowcase.EffectRotate.CycleType.Random, this.B.gameObject, 1f, Vector3.zero, new Vector3(0f, 360f, 0f), 2f, 3f));
			this.effects.Add(new BGTestCurveShowcase.EffectRotate(BGTestCurveShowcase.EffectRotate.CycleType.Random, this.G.gameObject, 1.6f, Vector3.zero, new Vector3(0f, 360f, 0f), 4f, 6f));
			this.effects.Add(new BGTestCurveShowcase.EffectChangeTiling(2f, this.B.GetComponent<LineRenderer>().sharedMaterial, 0f, 0f, 0.2f, 1f));
			this.effects.Add(new BGTestCurveShowcase.EffectRotate(BGTestCurveShowcase.EffectRotate.CycleType.Swing, this.Light.gameObject, 3f, new Vector3(70f, -90f, 0f), new Vector3(110f, -90f, 0f)));
			this.effects.Add(new BGTestCurveShowcase.EffectRotate(BGTestCurveShowcase.EffectRotate.CycleType.FirstToLast, this.ProjectileFolder, 10f, Vector3.zero, new Vector3(360f, 0f, 0f)));
			this.effects.Add(new BGTestCurveShowcase.EffectMoveAndRotateAlongCurve(this.ProjectileCurve1, this.Projectile1.gameObject, 3f, 5, 0.1f, 0f));
			this.effects.Add(new BGTestCurveShowcase.EffectMoveAndRotateAlongCurve(this.ProjectileCurve1, this.Projectile2.gameObject, 3f, 5, 0.1f, 3.1415927f));
			this.effects.Add(new BGTestCurveShowcase.EffectDynamicCurve(this.DynamicCurve, 4f, new Light[]
			{
				this.Light1,
				this.Light2,
				this.Light3
			}));
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x000E2CF8 File Offset: 0x000E0EF8
		private void Update()
		{
			foreach (BGTestCurveShowcase.Effect effect in this.effects)
			{
				effect.Update();
			}
		}

		// Token: 0x04000E64 RID: 3684
		[Header("Light")]
		public Light Light;

		// Token: 0x04000E65 RID: 3685
		[Header("Logo parts")]
		public GameObject B;

		// Token: 0x04000E66 RID: 3686
		public GameObject G;

		// Token: 0x04000E67 RID: 3687
		public BGCcMath Curve;

		// Token: 0x04000E68 RID: 3688
		[Header("Projectiles")]
		public GameObject ProjectileFolder;

		// Token: 0x04000E69 RID: 3689
		public TrailRenderer Projectile1;

		// Token: 0x04000E6A RID: 3690
		public TrailRenderer Projectile2;

		// Token: 0x04000E6B RID: 3691
		public BGCcCursor ProjectileCurve1;

		// Token: 0x04000E6C RID: 3692
		[Header("Particles")]
		public ParticleSystem BParticles;

		// Token: 0x04000E6D RID: 3693
		public ParticleSystem GParticles;

		// Token: 0x04000E6E RID: 3694
		public ParticleSystem CurveParticles1;

		// Token: 0x04000E6F RID: 3695
		public ParticleSystem CurveParticles2;

		// Token: 0x04000E70 RID: 3696
		[Header("Dynamic")]
		public GameObject DynamicCurve;

		// Token: 0x04000E71 RID: 3697
		public Light Light1;

		// Token: 0x04000E72 RID: 3698
		public Light Light2;

		// Token: 0x04000E73 RID: 3699
		public Light Light3;

		// Token: 0x04000E74 RID: 3700
		private readonly List<BGTestCurveShowcase.Effect> effects = new List<BGTestCurveShowcase.Effect>();

		// Token: 0x04000E75 RID: 3701
		private const float ScaleMin = 0.85f;

		// Token: 0x04000E76 RID: 3702
		private const float ScaleMax = 1.15f;

		// Token: 0x04000E77 RID: 3703
		private static readonly Vector3 FromScale = new Vector3(0.85f, 0.85f, 0.85f);

		// Token: 0x04000E78 RID: 3704
		private static readonly Vector3 ToScale = new Vector3(1.15f, 1.15f, 1.15f);

		// Token: 0x04000E79 RID: 3705
		private const float ScalePeriodMin = 1f;

		// Token: 0x04000E7A RID: 3706
		private const float ScalePeriodMax = 2f;

		// Token: 0x020001E8 RID: 488
		private abstract class Phase
		{
			// Token: 0x06001126 RID: 4390 RVA: 0x0004E372 File Offset: 0x0004C572
			protected Phase(float periodMin, float periodMax)
			{
				this.periodMin = periodMin;
				this.periodMax = periodMax;
			}

			// Token: 0x06001127 RID: 4391 RVA: 0x0004E393 File Offset: 0x0004C593
			protected internal virtual void PhaseStart()
			{
				this.startTime = Time.time;
				this.period = UnityEngine.Random.Range(this.periodMin, this.periodMax);
			}

			// Token: 0x06001128 RID: 4392
			public abstract void Update();

			// Token: 0x04000E7B RID: 3707
			private readonly float periodMin;

			// Token: 0x04000E7C RID: 3708
			private readonly float periodMax;

			// Token: 0x04000E7D RID: 3709
			internal float period = -10000f;

			// Token: 0x04000E7E RID: 3710
			internal float startTime;
		}

		// Token: 0x020001E9 RID: 489
		private sealed class PhaseDelay : BGTestCurveShowcase.Phase
		{
			// Token: 0x06001129 RID: 4393 RVA: 0x0004E3B7 File Offset: 0x0004C5B7
			public PhaseDelay(float periodMin, float periodMax) : base(periodMin, periodMax)
			{
			}

			// Token: 0x0600112A RID: 4394 RVA: 0x0004475B File Offset: 0x0004295B
			public override void Update()
			{
			}
		}

		// Token: 0x020001EA RID: 490
		private abstract class Effect : BGTestCurveShowcase.Phase
		{
			// Token: 0x0600112B RID: 4395 RVA: 0x0004E3C1 File Offset: 0x0004C5C1
			protected Effect(float periodMin, float periodMax) : base(periodMin, periodMax)
			{
				this.phases.Add(this);
			}

			// Token: 0x0600112C RID: 4396 RVA: 0x0004E3E2 File Offset: 0x0004C5E2
			protected void AddPhase(BGTestCurveShowcase.Phase phase)
			{
				this.phases.Add(phase);
			}

			// Token: 0x0600112D RID: 4397 RVA: 0x0004E3F0 File Offset: 0x0004C5F0
			protected void AddPhase(BGTestCurveShowcase.Phase phase, int index)
			{
				this.phases.Insert(index, phase);
			}

			// Token: 0x0600112E RID: 4398 RVA: 0x000E2D48 File Offset: 0x000E0F48
			public override void Update()
			{
				BGTestCurveShowcase.Phase phase = this.phases[this.currentPhaseIndex];
				bool flag = false;
				if (Time.time - phase.startTime > phase.period)
				{
					flag = true;
					this.currentPhaseIndex++;
					if (this.currentPhaseIndex == this.phases.Count)
					{
						this.currentPhaseIndex = 0;
					}
					phase = this.phases[this.currentPhaseIndex];
					phase.PhaseStart();
				}
				if (phase is BGTestCurveShowcase.Effect)
				{
					BGTestCurveShowcase.Effect effect = (BGTestCurveShowcase.Effect)phase;
					if (flag)
					{
						effect.Start();
					}
					effect.Update((Time.time - phase.startTime) / phase.period);
				}
			}

			// Token: 0x0600112F RID: 4399 RVA: 0x0004E3FF File Offset: 0x0004C5FF
			protected float CheckReverse(float ratio, bool reverse)
			{
				if (!reverse)
				{
					return ratio;
				}
				return 1f - ratio;
			}

			// Token: 0x06001130 RID: 4400
			protected abstract void Update(float ratio);

			// Token: 0x06001131 RID: 4401 RVA: 0x0004475B File Offset: 0x0004295B
			protected virtual void Start()
			{
			}

			// Token: 0x06001132 RID: 4402 RVA: 0x000E2DF0 File Offset: 0x000E0FF0
			protected static float Scale(float ratio, float count)
			{
				float num = 1f / count;
				return (ratio - (float)Mathf.FloorToInt(ratio / num) * num) / num;
			}

			// Token: 0x04000E7F RID: 3711
			private readonly List<BGTestCurveShowcase.Phase> phases = new List<BGTestCurveShowcase.Phase>();

			// Token: 0x04000E80 RID: 3712
			private int currentPhaseIndex;
		}

		// Token: 0x020001EB RID: 491
		private sealed class EffectScale : BGTestCurveShowcase.Effect
		{
			// Token: 0x06001133 RID: 4403 RVA: 0x0004E40D File Offset: 0x0004C60D
			public EffectScale(GameObject target, float periodMin, float periodMax, Vector3 min, Vector3 max) : base(periodMin, periodMax)
			{
				this.target = target;
				this.newScale = target.transform.localScale;
				this.min = min;
				this.max = max;
			}

			// Token: 0x06001134 RID: 4404 RVA: 0x0004E43F File Offset: 0x0004C63F
			protected override void Update(float ratio)
			{
				this.target.transform.localScale = Vector3.Lerp(this.oldScale, this.newScale, ratio);
			}

			// Token: 0x06001135 RID: 4405 RVA: 0x000E2E14 File Offset: 0x000E1014
			protected override void Start()
			{
				this.oldScale = this.newScale;
				this.newScale = new Vector3(UnityEngine.Random.Range(this.min.x, this.max.x), UnityEngine.Random.Range(this.min.y, this.max.y), UnityEngine.Random.Range(this.min.z, this.max.z));
			}

			// Token: 0x04000E81 RID: 3713
			private readonly GameObject target;

			// Token: 0x04000E82 RID: 3714
			private Vector3 min;

			// Token: 0x04000E83 RID: 3715
			private Vector3 max;

			// Token: 0x04000E84 RID: 3716
			private Vector3 oldScale;

			// Token: 0x04000E85 RID: 3717
			private Vector3 newScale;
		}

		// Token: 0x020001EC RID: 492
		private sealed class EffectRotate : BGTestCurveShowcase.Effect
		{
			// Token: 0x06001136 RID: 4406 RVA: 0x0004E463 File Offset: 0x0004C663
			public EffectRotate(BGTestCurveShowcase.EffectRotate.CycleType cycleType, GameObject target, float period, Vector3 min, Vector3 max, float delayMin, float delayMax) : this(cycleType, target, period, min, max)
			{
				base.AddPhase(new BGTestCurveShowcase.PhaseDelay(delayMin, delayMax), 0);
			}

			// Token: 0x06001137 RID: 4407 RVA: 0x0004E482 File Offset: 0x0004C682
			public EffectRotate(BGTestCurveShowcase.EffectRotate.CycleType cycleType, GameObject target, float period, Vector3 min, Vector3 max) : base(period, period)
			{
				this.target = target;
				this.cycleType = cycleType;
				this.min = min;
				this.max = max;
			}

			// Token: 0x06001138 RID: 4408 RVA: 0x0004E4AA File Offset: 0x0004C6AA
			protected override void Update(float ratio)
			{
				this.target.transform.eulerAngles = Vector3.Lerp(this.min, this.max, base.CheckReverse(ratio, this.reverse));
			}

			// Token: 0x06001139 RID: 4409 RVA: 0x000E2E8C File Offset: 0x000E108C
			protected override void Start()
			{
				BGTestCurveShowcase.EffectRotate.CycleType cycleType = this.cycleType;
				if (cycleType == BGTestCurveShowcase.EffectRotate.CycleType.FirstToLast)
				{
					this.reverse = false;
					return;
				}
				if (cycleType != BGTestCurveShowcase.EffectRotate.CycleType.Swing)
				{
					this.reverse = (UnityEngine.Random.Range(0, 2) == 0);
					return;
				}
				this.reverse = !this.reverse;
			}

			// Token: 0x04000E86 RID: 3718
			private readonly GameObject target;

			// Token: 0x04000E87 RID: 3719
			private readonly Vector3 min;

			// Token: 0x04000E88 RID: 3720
			private readonly Vector3 max;

			// Token: 0x04000E89 RID: 3721
			private readonly BGTestCurveShowcase.EffectRotate.CycleType cycleType;

			// Token: 0x04000E8A RID: 3722
			private bool reverse;

			// Token: 0x020001ED RID: 493
			internal enum CycleType
			{
				// Token: 0x04000E8C RID: 3724
				FirstToLast,
				// Token: 0x04000E8D RID: 3725
				Swing,
				// Token: 0x04000E8E RID: 3726
				Random
			}
		}

		// Token: 0x020001EE RID: 494
		private sealed class EffectChangeTiling : BGTestCurveShowcase.Effect
		{
			// Token: 0x0600113A RID: 4410 RVA: 0x0004E4DA File Offset: 0x0004C6DA
			public EffectChangeTiling(float period, Material material, float tileXMin, float tileXMax, float tileYMin, float tileYMax) : base(period, period)
			{
				this.material = material;
				this.tileXMin = tileXMin;
				this.tileXMax = tileXMax;
				this.tileYMin = tileYMin;
				this.tileYMax = tileYMax;
			}

			// Token: 0x0600113B RID: 4411 RVA: 0x000E2ED4 File Offset: 0x000E10D4
			protected override void Update(float ratio)
			{
				ratio = base.CheckReverse(ratio, this.reverse);
				this.material.mainTextureScale = new Vector2(Mathf.Lerp(this.tileXMin, this.tileXMax, ratio), Mathf.Lerp(this.tileYMin, this.tileYMax, ratio));
			}

			// Token: 0x0600113C RID: 4412 RVA: 0x0004E50A File Offset: 0x0004C70A
			protected override void Start()
			{
				this.reverse = !this.reverse;
			}

			// Token: 0x04000E8F RID: 3727
			private readonly float tileXMin;

			// Token: 0x04000E90 RID: 3728
			private readonly float tileXMax;

			// Token: 0x04000E91 RID: 3729
			private readonly float tileYMin;

			// Token: 0x04000E92 RID: 3730
			private readonly float tileYMax;

			// Token: 0x04000E93 RID: 3731
			private readonly Material material;

			// Token: 0x04000E94 RID: 3732
			private bool reverse;
		}

		// Token: 0x020001EF RID: 495
		private sealed class EffectMoveAndRotateAlongCurve : BGTestCurveShowcase.Effect
		{
			// Token: 0x0600113D RID: 4413 RVA: 0x0004E51B File Offset: 0x0004C71B
			public EffectMoveAndRotateAlongCurve(BGCcCursor cursor, GameObject target, float period, int rotateCount, float rotationDistance, float initialRotationRadians = 0f) : base(period, period)
			{
				this.target = target;
				this.cursor = cursor;
				this.rotateCount = (float)rotateCount;
				this.rotationDistance = rotationDistance;
				this.initialRotationRadians = initialRotationRadians;
			}

			// Token: 0x0600113E RID: 4414 RVA: 0x000E2F24 File Offset: 0x000E1124
			protected override void Update(float ratio)
			{
				Vector3 a = this.cursor.CalculatePosition();
				Vector3 forward = this.cursor.CalculateTangent();
				float f = this.initialRotationRadians + Mathf.Lerp(0f, 6.2831855f, BGTestCurveShowcase.Effect.Scale(ratio, this.rotateCount));
				Vector3 position = a + Quaternion.LookRotation(forward) * (new Vector3(Mathf.Sin(f), Mathf.Cos(f)) * this.rotationDistance);
				this.target.transform.position = position;
			}

			// Token: 0x04000E95 RID: 3733
			private readonly GameObject target;

			// Token: 0x04000E96 RID: 3734
			private readonly BGCcCursor cursor;

			// Token: 0x04000E97 RID: 3735
			private readonly float rotateCount;

			// Token: 0x04000E98 RID: 3736
			private readonly float rotationDistance;

			// Token: 0x04000E99 RID: 3737
			private readonly float initialRotationRadians;
		}

		// Token: 0x020001F0 RID: 496
		private sealed class EffectDynamicCurve : BGTestCurveShowcase.Effect
		{
			// Token: 0x0600113F RID: 4415 RVA: 0x000E2FAC File Offset: 0x000E11AC
			public EffectDynamicCurve(GameObject target, float period, params Light[] lights) : base(period, period)
			{
				target.AddComponent<BGCurve>();
				this.math = target.AddComponent<BGCcMath>();
				this.math.Curve.Closed = true;
				this.lights = lights;
				this.fromDistanceRatios = new float[lights.Length];
				this.toDistanceRatios = new float[lights.Length];
			}

			// Token: 0x06001140 RID: 4416 RVA: 0x000E3008 File Offset: 0x000E1208
			protected override void Update(float ratio)
			{
				for (int i = 0; i < this.lights.Length; i++)
				{
					Light light = this.lights[i];
					light.gameObject.transform.position = this.math.Math.CalcByDistanceRatio(BGCurveBaseMath.Field.Position, Mathf.Lerp(this.fromDistanceRatios[i], this.toDistanceRatios[i], ratio), false);
					if ((double)ratio < 0.1)
					{
						light.intensity = Mathf.Lerp(0f, 3f, ratio * 10f);
					}
					else if ((double)ratio > 0.9)
					{
						light.intensity = Mathf.Lerp(3f, 0f, (ratio - 0.9f) * 10f);
					}
				}
			}

			// Token: 0x06001141 RID: 4417 RVA: 0x000E30CC File Offset: 0x000E12CC
			protected override void Start()
			{
				BGCurve curve = this.math.Curve;
				curve.Clear();
				for (int i = 0; i < 3; i++)
				{
					this.AddPoint(curve);
				}
				for (int j = 0; j < this.fromDistanceRatios.Length; j++)
				{
					this.fromDistanceRatios[j] = UnityEngine.Random.Range(0f, 1f);
					this.toDistanceRatios[j] = UnityEngine.Random.Range(0f, 1f);
				}
			}

			// Token: 0x06001142 RID: 4418 RVA: 0x000E3140 File Offset: 0x000E1340
			private void AddPoint(BGCurve curve)
			{
				Vector3 vector = this.RandomVector();
				curve.AddPoint(new BGCurvePoint(curve, this.RandomVector(), BGCurvePoint.ControlTypeEnum.BezierSymmetrical, vector, -vector, false));
			}

			// Token: 0x06001143 RID: 4419 RVA: 0x0004E54C File Offset: 0x0004C74C
			private Vector3 RandomVector()
			{
				return new Vector3(UnityEngine.Random.Range(-8f, 8f), 0f, UnityEngine.Random.Range(-4f, 4f));
			}

			// Token: 0x04000E9A RID: 3738
			private const int PointsCount = 3;

			// Token: 0x04000E9B RID: 3739
			private const float SpanX = 8f;

			// Token: 0x04000E9C RID: 3740
			private const float SpanZ = 4f;

			// Token: 0x04000E9D RID: 3741
			private readonly BGCcMath math;

			// Token: 0x04000E9E RID: 3742
			private readonly Light[] lights;

			// Token: 0x04000E9F RID: 3743
			private readonly float[] fromDistanceRatios;

			// Token: 0x04000EA0 RID: 3744
			private readonly float[] toDistanceRatios;
		}
	}
}
