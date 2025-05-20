using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001DA RID: 474
	public class BGTestCcChangeCursorLinear : MonoBehaviour
	{
		// Token: 0x060010C0 RID: 4288 RVA: 0x000E0C34 File Offset: 0x000DEE34
		private void Start()
		{
			BGCcCursorChangeLinear[] componentsInChildren = base.GetComponentsInChildren<BGCcCursorChangeLinear>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.Register(componentsInChildren[i]);
			}
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x0004DECD File Offset: 0x0004C0CD
		private void Register(BGCcCursorChangeLinear curve)
		{
			this.sequences.Add(new BGTestCcChangeCursorLinear.Sequence(curve));
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x000E0C60 File Offset: 0x000DEE60
		private void Update()
		{
			foreach (BGTestCcChangeCursorLinear.Sequence sequence in this.sequences)
			{
				if (sequence.Curve.gameObject.activeInHierarchy)
				{
					sequence.Check();
				}
			}
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x0004DEE0 File Offset: 0x0004C0E0
		public void PointReached0(int point)
		{
			this.Process(0, point);
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x0004DEEA File Offset: 0x0004C0EA
		public void PointReached1(int point)
		{
			this.Process(1, point);
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x0004DEF4 File Offset: 0x0004C0F4
		public void PointReached2(int point)
		{
			this.Process(2, point);
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0004DEFE File Offset: 0x0004C0FE
		public void PointReached3(int point)
		{
			this.Process(3, point);
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0004DF08 File Offset: 0x0004C108
		public void PointReached4(int point)
		{
			this.Process(4, point);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0004DF12 File Offset: 0x0004C112
		public void PointReached5(int point)
		{
			this.Process(5, point);
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0004DF1C File Offset: 0x0004C11C
		public void PointReached6(int point)
		{
			this.Process(6, point);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0004DF26 File Offset: 0x0004C126
		public void PointReached7(int point)
		{
			this.Process(7, point);
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0004DF30 File Offset: 0x0004C130
		public void PointReached8(int point)
		{
			this.Process(8, point);
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0004DF3A File Offset: 0x0004C13A
		public void PointReached9(int point)
		{
			this.Process(9, point);
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0004DF45 File Offset: 0x0004C145
		public void PointReached10(int point)
		{
			this.Process(10, point);
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0004DF50 File Offset: 0x0004C150
		public void PointReached11(int point)
		{
			this.Process(11, point);
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0004DF5B File Offset: 0x0004C15B
		public void PointReached12(int point)
		{
			this.Process(12, point);
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0004DF66 File Offset: 0x0004C166
		public void PointReached13(int point)
		{
			this.Process(13, point);
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0004DF71 File Offset: 0x0004C171
		public void PointReached14(int point)
		{
			this.Process(14, point);
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0004DF7C File Offset: 0x0004C17C
		public void PointReached15(int point)
		{
			this.Process(15, point);
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0004DF87 File Offset: 0x0004C187
		public void PointReached16(int point)
		{
			this.Process(16, point);
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0004DF92 File Offset: 0x0004C192
		public void PointReached17(int point)
		{
			this.Process(17, point);
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0004DF9D File Offset: 0x0004C19D
		public void PointReached18(int point)
		{
			this.Process(18, point);
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0004DFA8 File Offset: 0x0004C1A8
		public void PointReached19(int point)
		{
			this.Process(19, point);
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x0004DFB3 File Offset: 0x0004C1B3
		public void PointReached20(int point)
		{
			this.Process(20, point);
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0004DFBE File Offset: 0x0004C1BE
		public void PointReached21(int point)
		{
			this.Process(21, point);
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x0004DFC9 File Offset: 0x0004C1C9
		public void PointReached22(int point)
		{
			this.Process(22, point);
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x0004DFD4 File Offset: 0x0004C1D4
		public void PointReached23(int point)
		{
			this.Process(23, point);
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x0004DFDF File Offset: 0x0004C1DF
		public void PointReached24(int point)
		{
			this.Process(24, point);
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x0004DFEA File Offset: 0x0004C1EA
		public void PointReached25(int point)
		{
			this.Process(25, point);
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x0004DFF5 File Offset: 0x0004C1F5
		public void PointReached26(int point)
		{
			this.Process(26, point);
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0004E000 File Offset: 0x0004C200
		public void PointReached27(int point)
		{
			this.Process(27, point);
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0004E00B File Offset: 0x0004C20B
		public void PointReached28(int point)
		{
			this.Process(28, point);
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0004E016 File Offset: 0x0004C216
		public void PointReached29(int point)
		{
			this.Process(29, point);
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x0004E021 File Offset: 0x0004C221
		public void PointReached30(int point)
		{
			this.Process(30, point);
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0004E02C File Offset: 0x0004C22C
		public void PointReached31(int point)
		{
			this.Process(31, point);
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0004E037 File Offset: 0x0004C237
		public void PointReached32(int point)
		{
			this.Process(32, point);
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x000E0CC4 File Offset: 0x000DEEC4
		private void Process(int curve, int pointIndex)
		{
			BGTestCcChangeCursorLinear.Sequence sequence = this.sequences[curve];
			if (!sequence.Curve.gameObject.activeSelf)
			{
				return;
			}
			sequence.Reached(pointIndex);
		}

		// Token: 0x04000E0B RID: 3595
		private readonly List<BGTestCcChangeCursorLinear.Sequence> sequences = new List<BGTestCcChangeCursorLinear.Sequence>(10);

		// Token: 0x020001DB RID: 475
		private sealed class Sequence
		{
			// Token: 0x17000492 RID: 1170
			// (get) Token: 0x060010E6 RID: 4326 RVA: 0x0004E057 File Offset: 0x0004C257
			private float Elapsed
			{
				get
				{
					return Time.time - this.started;
				}
			}

			// Token: 0x060010E7 RID: 4327 RVA: 0x000E0CF8 File Offset: 0x000DEEF8
			public Sequence(BGCcCursorChangeLinear changeCursor)
			{
				this.changeCursor = changeCursor;
				BGCcCursor cursor = changeCursor.Cursor;
				BGCurve curve = changeCursor.Curve;
				this.Curve = curve;
				this.started = Time.time;
				if (!this.Curve.gameObject.activeInHierarchy)
				{
					return;
				}
				this.ThrowIf("Stop overflow control is not supported", changeCursor.OverflowControl == BGCcCursorChangeLinear.OverflowControlEnum.Stop);
				int pointsCount = curve.PointsCount;
				this.ThrowIf("Curve should have at least 2 points.", pointsCount < 2);
				BGCurveBaseMath math = changeCursor.Cursor.Math.Math;
				int num = cursor.CalculateSectionIndex();
				float currentSpeed = changeCursor.CurrentSpeed;
				if (currentSpeed > 0f)
				{
					if (curve.Closed && num == pointsCount - 1)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(0, math.GetDistance(-1) - cursor.Distance, currentSpeed, 0f));
					}
					else if (!curve.Closed && num == pointsCount - 2)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(pointsCount - 1, math.GetDistance(-1) - cursor.Distance, currentSpeed, 0f));
					}
					else
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(num + 1, math[num + 1].DistanceFromStartToOrigin - cursor.Distance, currentSpeed, 0f));
					}
					for (int i = num + 2; i < pointsCount; i++)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(i, math[i - 1].Distance, changeCursor.GetSpeedAtPoint(i - 1), changeCursor.GetDelayAtPoint(i - 1)));
					}
					if (curve.Closed && num != pointsCount)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(0, math[pointsCount - 1].Distance, changeCursor.GetSpeedAtPoint(pointsCount - 1), changeCursor.GetDelayAtPoint(pointsCount - 1)));
					}
					if (changeCursor.OverflowControl == BGCcCursorChangeLinear.OverflowControlEnum.PingPong)
					{
						if (curve.Closed)
						{
							this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(pointsCount - 1, math[pointsCount - 1].Distance, changeCursor.GetSpeedAtPoint(pointsCount - 1), changeCursor.GetDelayAtPoint(0)));
						}
						for (int j = pointsCount - 2; j >= 0; j--)
						{
							this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(j, math[j].Distance, changeCursor.GetSpeedAtPoint(j), changeCursor.GetDelayAtPoint(j + 1)));
						}
					}
					else if (!curve.Closed)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(0, math[pointsCount - 2].Distance, 0f, changeCursor.GetDelayAtPoint(pointsCount - 1)));
					}
					for (int k = 1; k <= num; k++)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(k, math[k - 1].Distance, changeCursor.GetSpeedAtPoint(k - 1), changeCursor.GetDelayAtPoint(k - 1)));
					}
					this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(-1, cursor.Distance - math[num].DistanceFromStartToOrigin, currentSpeed, changeCursor.GetDelayAtPoint(num)));
					return;
				}
				this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(num, cursor.Distance - math[num].DistanceFromStartToOrigin, currentSpeed, 0f));
				for (int l = num - 1; l >= 0; l--)
				{
					this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(l, math[l].Distance, changeCursor.GetSpeedAtPoint(l), changeCursor.GetDelayAtPoint(l + 1)));
				}
				if (changeCursor.OverflowControl == BGCcCursorChangeLinear.OverflowControlEnum.PingPong)
				{
					for (int m = 1; m < pointsCount; m++)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(m, math[m - 1].Distance, changeCursor.GetSpeedAtPoint(m - 1), changeCursor.GetDelayAtPoint(m - 1)));
					}
					if (curve.Closed)
					{
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(0, math[pointsCount - 1].Distance, changeCursor.GetSpeedAtPoint(pointsCount - 1), changeCursor.GetDelayAtPoint(pointsCount - 1)));
						this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(pointsCount - 1, math[pointsCount - 1].Distance, changeCursor.GetSpeedAtPoint(pointsCount - 1), changeCursor.GetDelayAtPoint(0)));
					}
				}
				else if (curve.Closed)
				{
					this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(pointsCount - 1, math[pointsCount - 1].Distance, changeCursor.GetSpeedAtPoint(pointsCount - 1), changeCursor.GetDelayAtPoint(0)));
				}
				else
				{
					this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(pointsCount - 1, 0f, 0f, changeCursor.GetDelayAtPoint(0)));
				}
				for (int n = pointsCount - 2; n > num; n--)
				{
					this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(n, math[n].Distance, changeCursor.GetSpeedAtPoint(n), changeCursor.GetDelayAtPoint(n + 1)));
				}
				this.expectedPoints.Add(new BGTestCcChangeCursorLinear.ExpectedPoint(-1, math[num].DistanceFromEndToOrigin - cursor.Distance, changeCursor.GetSpeedAtPoint(num), changeCursor.GetDelayAtPoint(num + 1)));
			}

			// Token: 0x060010E8 RID: 4328 RVA: 0x0004E065 File Offset: 0x0004C265
			private void ThrowIf(string message, bool condition)
			{
				if (condition)
				{
					throw this.GetException(message);
				}
			}

			// Token: 0x060010E9 RID: 4329 RVA: 0x0004E072 File Offset: 0x0004C272
			private UnityException GetException(string message)
			{
				return new UnityException(message + ". Curve=" + this.changeCursor.Curve.gameObject.name);
			}

			// Token: 0x060010EA RID: 4330 RVA: 0x000E1214 File Offset: 0x000DF414
			public void Check()
			{
				if (!this.valid)
				{
					return;
				}
				BGTestCcChangeCursorLinear.ExpectedPoint expectedPoint = this.expectedPoints[this.pointCursor];
				if (expectedPoint.PointIndex == -1)
				{
					if (expectedPoint.ExpectedDelay < (double)this.Elapsed)
					{
						this.MoveNext();
						return;
					}
				}
				else if (expectedPoint.ExpectedDelay < (double)(this.Elapsed - 0.1f))
				{
					this.valid = false;
					string str = "Missing event: expected ";
					BGTestCcChangeCursorLinear.ExpectedPoint expectedPoint2 = expectedPoint;
					Debug.LogException(this.GetException(str + ((expectedPoint2 != null) ? expectedPoint2.ToString() : null) + " event did not occur"));
					return;
				}
			}

			// Token: 0x060010EB RID: 4331 RVA: 0x000E12A0 File Offset: 0x000DF4A0
			public void Reached(int point)
			{
				if (!this.valid)
				{
					return;
				}
				BGTestCcChangeCursorLinear.ExpectedPoint expectedPoint = this.expectedPoints[this.pointCursor];
				if (expectedPoint.PointIndex >= 0 && expectedPoint.PointIndex != point)
				{
					this.valid = false;
					Debug.LogException(this.GetException("Points indexes mismatch: expected " + expectedPoint.PointIndex.ToString() + ", actual=" + point.ToString()));
					return;
				}
				double expectedDelay = expectedPoint.ExpectedDelay;
				float elapsed = this.Elapsed;
				if (Math.Abs(expectedDelay - (double)elapsed) > 0.10000000149011612)
				{
					this.valid = false;
					Debug.LogException(this.GetException(string.Concat(new string[]
					{
						"Timing mismatch at point {",
						expectedPoint.PointIndex.ToString(),
						"}: expected ",
						expectedDelay.ToString(),
						", actual=",
						elapsed.ToString()
					})));
					return;
				}
				this.MoveNext();
			}

			// Token: 0x060010EC RID: 4332 RVA: 0x0004E099 File Offset: 0x0004C299
			private void MoveNext()
			{
				this.started = Time.time;
				this.pointCursor = ((this.pointCursor == this.expectedPoints.Count - 1) ? 0 : (this.pointCursor + 1));
			}

			// Token: 0x04000E0C RID: 3596
			public const float Epsilon = 0.1f;

			// Token: 0x04000E0D RID: 3597
			private readonly List<BGTestCcChangeCursorLinear.ExpectedPoint> expectedPoints = new List<BGTestCcChangeCursorLinear.ExpectedPoint>();

			// Token: 0x04000E0E RID: 3598
			private BGCcCursorChangeLinear changeCursor;

			// Token: 0x04000E0F RID: 3599
			private int pointCursor;

			// Token: 0x04000E10 RID: 3600
			private bool valid = true;

			// Token: 0x04000E11 RID: 3601
			private float lastPoint;

			// Token: 0x04000E12 RID: 3602
			private float started;

			// Token: 0x04000E13 RID: 3603
			public BGCurve Curve;
		}

		// Token: 0x020001DC RID: 476
		private sealed class ExpectedPoint
		{
			// Token: 0x060010ED RID: 4333 RVA: 0x0004E0CC File Offset: 0x0004C2CC
			public ExpectedPoint(int pointIndex, float distance, float speed, float delay)
			{
				this.Speed = speed;
				this.Distance = distance;
				this.PointIndex = pointIndex;
				this.Delay = delay;
			}

			// Token: 0x17000493 RID: 1171
			// (get) Token: 0x060010EE RID: 4334 RVA: 0x000E1394 File Offset: 0x000DF594
			public double ExpectedDelay
			{
				get
				{
					float num = Math.Abs(this.Speed);
					return (double)(Mathf.Clamp(this.Delay, 0f, float.MaxValue) + ((num < 0.1f) ? 0.1f : (this.Distance / num)));
				}
			}

			// Token: 0x060010EF RID: 4335 RVA: 0x000E13DC File Offset: 0x000DF5DC
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"Point ",
					this.PointIndex.ToString(),
					" after ",
					this.ExpectedDelay.ToString(),
					" delay."
				});
			}

			// Token: 0x04000E14 RID: 3604
			public readonly float Distance;

			// Token: 0x04000E15 RID: 3605
			public readonly int PointIndex;

			// Token: 0x04000E16 RID: 3606
			public readonly float Speed;

			// Token: 0x04000E17 RID: 3607
			public readonly float Delay;
		}
	}
}
