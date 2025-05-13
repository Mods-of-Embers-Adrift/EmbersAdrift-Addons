using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;
using UnityEngine.Events;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001AF RID: 431
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCursorChangeLinear")]
	[BGCc.CcDescriptor(Description = "Change cursor position linearly", Name = "Cursor Change Linear", Icon = "BGCcCursorChangeLinear123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCursorChangeLinear")]
	public class BGCcCursorChangeLinear : BGCcWithCursor
	{
		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000F55 RID: 3925 RVA: 0x000DC7C8 File Offset: 0x000DA9C8
		// (remove) Token: 0x06000F56 RID: 3926 RVA: 0x000DC800 File Offset: 0x000DAA00
		public event EventHandler<BGCcCursorChangeLinear.PointReachedArgs> PointReached;

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000F57 RID: 3927 RVA: 0x0004CE6E File Offset: 0x0004B06E
		// (set) Token: 0x06000F58 RID: 3928 RVA: 0x0004CE76 File Offset: 0x0004B076
		public BGCcCursorChangeLinear.OverflowControlEnum OverflowControl
		{
			get
			{
				return this.overflowControl;
			}
			set
			{
				if (base.ParamChanged<BGCcCursorChangeLinear.OverflowControlEnum>(ref this.overflowControl, value))
				{
					this.Stopped = false;
				}
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0004CE8E File Offset: 0x0004B08E
		// (set) Token: 0x06000F5A RID: 3930 RVA: 0x0004CE96 File Offset: 0x0004B096
		public float Speed
		{
			get
			{
				return this.speed;
			}
			set
			{
				base.ParamChanged<float>(ref this.speed, value);
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000F5B RID: 3931 RVA: 0x0004CEA6 File Offset: 0x0004B0A6
		// (set) Token: 0x06000F5C RID: 3932 RVA: 0x0004CEAE File Offset: 0x0004B0AE
		public bool AdjustByTotalLength
		{
			get
			{
				return this.adjustByTotalLength;
			}
			set
			{
				base.ParamChanged<bool>(ref this.adjustByTotalLength, value);
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0004CEBE File Offset: 0x0004B0BE
		// (set) Token: 0x06000F5E RID: 3934 RVA: 0x0004CEC6 File Offset: 0x0004B0C6
		public BGCurvePointField SpeedField
		{
			get
			{
				return this.speedField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.speedField, value);
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0004CED6 File Offset: 0x0004B0D6
		// (set) Token: 0x06000F60 RID: 3936 RVA: 0x0004CEDE File Offset: 0x0004B0DE
		public float Delay
		{
			get
			{
				return this.delay;
			}
			set
			{
				base.ParamChanged<float>(ref this.delay, value);
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0004CEEE File Offset: 0x0004B0EE
		// (set) Token: 0x06000F62 RID: 3938 RVA: 0x0004CEF6 File Offset: 0x0004B0F6
		public BGCurvePointField DelayField
		{
			get
			{
				return this.delayField;
			}
			set
			{
				base.ParamChanged<BGCurvePointField>(ref this.delayField, value);
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06000F63 RID: 3939 RVA: 0x0004CF06 File Offset: 0x0004B106
		// (set) Token: 0x06000F64 RID: 3940 RVA: 0x0004CF0E File Offset: 0x0004B10E
		public bool UseFixedUpdate
		{
			get
			{
				return this.useFixedUpdate;
			}
			set
			{
				base.ParamChanged<bool>(ref this.useFixedUpdate, value);
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06000F65 RID: 3941 RVA: 0x0004CF1E File Offset: 0x0004B11E
		// (set) Token: 0x06000F66 RID: 3942 RVA: 0x0004CF26 File Offset: 0x0004B126
		public bool Stopped { get; set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06000F67 RID: 3943 RVA: 0x0004CF2F File Offset: 0x0004B12F
		public bool SpeedReversed
		{
			get
			{
				return this.speedReversed;
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06000F68 RID: 3944 RVA: 0x000DC838 File Offset: 0x000DAA38
		public float CurrentSpeed
		{
			get
			{
				if (base.Curve.PointsCount < 2)
				{
					return 0f;
				}
				if (this.speedField == null)
				{
					return this.speed;
				}
				float @float = base.Curve[base.Cursor.CalculateSectionIndex()].GetFloat(this.speedField.FieldName);
				if (!this.speedReversed)
				{
					return @float;
				}
				return -@float;
			}
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x000DC8A4 File Offset: 0x000DAAA4
		public override void Start()
		{
			this.oldLength = base.Cursor.Math.GetDistance(-1);
			if (Application.isPlaying && base.Curve.PointsCount > 1 && (this.delay > 0f || this.delayField != null || this.pointReachedEvent.GetPersistentEventCount() > 0 || this.PointReached != null))
			{
				this.currentSectionIndex = base.Cursor.Math.Math.CalcSectionIndexByDistance(base.Cursor.Distance);
			}
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0004CF37 File Offset: 0x0004B137
		private void Update()
		{
			if (this.useFixedUpdate)
			{
				return;
			}
			this.Step();
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0004CF48 File Offset: 0x0004B148
		private void FixedUpdate()
		{
			if (!this.useFixedUpdate)
			{
				return;
			}
			this.Step();
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x000DC934 File Offset: 0x000DAB34
		private void Step()
		{
			if (this.Stopped || (this.speedField == null && Mathf.Abs(this.speed) < 1E-05f))
			{
				return;
			}
			if (base.Curve.PointsCount < 2)
			{
				return;
			}
			BGCcCursor cursor = base.Cursor;
			BGCurveBaseMath math = cursor.Math.Math;
			bool isPlaying = Application.isPlaying;
			if (isPlaying && this.delayStarted >= 0f && !this.CheckIfDelayIsOver(math, cursor))
			{
				return;
			}
			if (this.delayStarted >= 0f)
			{
				return;
			}
			float num = cursor.Distance;
			float num2 = 0f;
			if (this.adjustByTotalLength)
			{
				num2 = math.GetDistance(-1);
				if (Math.Abs(num2) > 1E-05f && Math.Abs(this.oldLength) > 1E-05f && Math.Abs(num2 - this.oldLength) > 1E-05f)
				{
					num = num * num2 / this.oldLength;
				}
			}
			int num3 = -1;
			bool flag = this.pointReachedEvent.GetPersistentEventCount() > 0 || this.PointReached != null;
			bool flag2 = isPlaying && (this.delay > 0f || this.delayField != null);
			if ((flag2 || flag) && this.CheckForNewDelay(math, num, ref num3, flag2, flag))
			{
				return;
			}
			float num4 = this.speed;
			if (this.speedField != null)
			{
				if (num3 == -1)
				{
					num3 = math.CalcSectionIndexByDistance(num);
				}
				num4 = base.Curve[num3].GetFloat(this.speedField.FieldName);
				if (this.speedReversed)
				{
					num4 = -num4;
				}
			}
			float num5 = num + num4 * Time.deltaTime;
			if (num5 < 0f || num5 > math.GetDistance(-1))
			{
				this.Overflow(math, ref num5, num4 >= 0f, flag2, flag);
			}
			cursor.Distance = num5;
			this.oldLength = num2;
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0004CF59 File Offset: 0x0004B159
		public float GetDelayAtPoint(int point)
		{
			if (!(this.delayField == null))
			{
				return base.Curve[point].GetFloat(this.delayField.FieldName);
			}
			return this.delay;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0004CF8C File Offset: 0x0004B18C
		public float GetSpeedAtPoint(int point)
		{
			if (!(this.speedField == null))
			{
				return base.Curve[point].GetFloat(this.speedField.FieldName);
			}
			return this.speed;
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x000DCB10 File Offset: 0x000DAD10
		private bool IsDelayRequired(int pointIndex)
		{
			bool flag = this.delayField != null;
			return (!flag && this.delay > 0f) || (flag && base.Curve[pointIndex].GetFloat(this.delayField.FieldName) > 1E-05f);
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0004CFBF File Offset: 0x0004B1BF
		private void StartDelay(bool speedIsPositive)
		{
			this.delayStarted = Time.time;
			this.speedWasPositiveWhileDelayed = speedIsPositive;
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x000DCB64 File Offset: 0x000DAD64
		private bool CheckForNewDelay(BGCurveBaseMath math, float distance, ref int newSectionIndex, bool checkDelay, bool firingEvents)
		{
			if (this.currentSectionIndex == 0 && this.skipZeroPoint)
			{
				return false;
			}
			if (!math.Curve.Closed && this.currentSectionIndex == math.Curve.PointsCount - 1)
			{
				return false;
			}
			newSectionIndex = math.CalcSectionIndexByDistance(distance);
			if (this.currentSectionIndex != newSectionIndex)
			{
				bool flag;
				if (this.speedField == null)
				{
					flag = (this.speed > 0f);
				}
				else
				{
					flag = (base.Curve[this.currentSectionIndex].GetFloat(this.speedField.FieldName) > 0f);
					if (this.speedReversed)
					{
						flag = !flag;
					}
				}
				if (this.CheckDelayAtSectionChanged(newSectionIndex, checkDelay, firingEvents, flag))
				{
					return true;
				}
			}
			this.delayStarted = -1f;
			return false;
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x000DCC2C File Offset: 0x000DAE2C
		private bool CheckDelayAtSectionChanged(int newSectionIndex, bool checkDelay, bool firingEvents, bool speedPositive)
		{
			BGCcCursor cursor = base.Cursor;
			BGCurveBaseMath math = cursor.Math.Math;
			int num = base.Curve.PointsCount - 1;
			if (speedPositive)
			{
				if (newSectionIndex > this.currentSectionIndex)
				{
					for (int i = this.currentSectionIndex + 1; i <= newSectionIndex; i++)
					{
						if (firingEvents)
						{
							this.FirePointReachedEvent(i);
						}
						if (checkDelay && this.CheckDelayAtPoint(math, cursor, i, speedPositive))
						{
							return true;
						}
					}
				}
			}
			else
			{
				if (this.currentSectionIndex == 0 && !base.Curve.Closed)
				{
					this.currentSectionIndex = num;
				}
				if (newSectionIndex < this.currentSectionIndex)
				{
					for (int j = this.currentSectionIndex; j > newSectionIndex; j--)
					{
						if (firingEvents)
						{
							this.FirePointReachedEvent(j);
						}
						if (checkDelay && this.CheckDelayAtPoint(math, cursor, j, speedPositive))
						{
							return true;
						}
					}
				}
			}
			this.currentSectionIndex = newSectionIndex;
			return false;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x000DCCFC File Offset: 0x000DAEFC
		private bool CheckDelayAtPoint(BGCurveBaseMath math, BGCcCursor cursor, int pointIndex, bool speedPositive)
		{
			if (this.IsDelayRequired(pointIndex))
			{
				this.currentSectionIndex = pointIndex;
				cursor.Distance = ((base.Curve.PointsCount - 1 == pointIndex && !base.Curve.Closed) ? math.GetDistance(-1) : math[pointIndex].DistanceFromStartToOrigin);
				this.StartDelay(speedPositive);
				return true;
			}
			return false;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x000DCD5C File Offset: 0x000DAF5C
		private void Overflow(BGCurveBaseMath math, ref float newDistance, bool currentSpeedPositive, bool checkDelay, bool firingEvents)
		{
			bool flag = newDistance < 0f;
			float distance = math.GetDistance(-1);
			int num = base.Curve.PointsCount - 1;
			if (checkDelay || firingEvents)
			{
				if (currentSpeedPositive)
				{
					int num2 = num;
					if (this.currentSectionIndex != num2 && this.CheckDelayAtSectionChanged(num2, checkDelay, firingEvents, true))
					{
						return;
					}
				}
				else
				{
					if (this.currentSectionIndex > 0 && this.CheckDelayAtSectionChanged(0, checkDelay, firingEvents, false))
					{
						return;
					}
					if (!this.skipZeroPoint && checkDelay && this.CheckDelayAtPoint(math, base.Cursor, 0, false))
					{
						if (firingEvents)
						{
							this.FirePointReachedEvent(0);
						}
						this.skipZeroPoint = true;
						return;
					}
				}
			}
			switch (this.overflowControl)
			{
			case BGCcCursorChangeLinear.OverflowControlEnum.Cycle:
				newDistance = (flag ? (distance + newDistance) : (newDistance - distance));
				break;
			case BGCcCursorChangeLinear.OverflowControlEnum.PingPong:
				if (this.speedField == null)
				{
					this.speed = -this.speed;
				}
				this.speedReversed = !this.speedReversed;
				currentSpeedPositive = !currentSpeedPositive;
				newDistance = (flag ? (-newDistance) : (distance * 2f - newDistance));
				break;
			case BGCcCursorChangeLinear.OverflowControlEnum.Stop:
				newDistance = (flag ? 0f : distance);
				this.Stopped = true;
				break;
			}
			if (newDistance < 0f)
			{
				newDistance = 0f;
			}
			else if (newDistance > distance)
			{
				newDistance = distance;
			}
			if (checkDelay || firingEvents)
			{
				if (base.Curve.Closed)
				{
					if (this.skipZeroPoint)
					{
						this.skipZeroPoint = false;
						return;
					}
					if (firingEvents)
					{
						this.FirePointReachedEvent(0);
					}
					this.currentSectionIndex = (currentSpeedPositive ? 0 : num);
					if (checkDelay)
					{
						this.CheckDelayAtPoint(math, base.Cursor, 0, currentSpeedPositive);
						return;
					}
				}
				else if (flag)
				{
					if (this.skipZeroPoint)
					{
						this.skipZeroPoint = false;
						return;
					}
					this.currentSectionIndex = 0;
					if (firingEvents)
					{
						this.FirePointReachedEvent(0);
					}
					if (checkDelay)
					{
						this.CheckDelayAtPoint(math, base.Cursor, 0, currentSpeedPositive);
						return;
					}
				}
				else if (!base.Curve.Closed)
				{
					if (currentSpeedPositive)
					{
						this.currentSectionIndex = 0;
						if (firingEvents)
						{
							this.FirePointReachedEvent(0);
						}
						if (checkDelay)
						{
							this.CheckDelayAtPoint(math, base.Cursor, 0, currentSpeedPositive);
							return;
						}
					}
					else
					{
						this.currentSectionIndex = num - 1;
					}
				}
			}
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x000DCF68 File Offset: 0x000DB168
		private bool CheckIfDelayIsOver(BGCurveBaseMath math, BGCcCursor cursor)
		{
			int num = base.Curve.PointsCount - 1;
			if (this.adjustByTotalLength)
			{
				this.oldLength = math.GetDistance(-1);
			}
			cursor.Distance = ((!base.Curve.Closed && this.currentSectionIndex == num) ? math.GetDistance(-1) : math[this.currentSectionIndex].DistanceFromStartToOrigin);
			float delayAtPoint = this.GetDelayAtPoint(this.currentSectionIndex);
			if (Time.time - this.delayStarted <= delayAtPoint)
			{
				return false;
			}
			float @float = this.speed;
			if (this.speedField != null)
			{
				@float = base.Curve[this.currentSectionIndex].GetFloat(this.speedField.FieldName);
			}
			this.delayStarted = -1f;
			if (this.speedWasPositiveWhileDelayed)
			{
				cursor.Distance += Mathf.Abs(@float * Time.deltaTime);
			}
			else if (this.currentSectionIndex > 0)
			{
				this.currentSectionIndex--;
				cursor.Distance -= Mathf.Abs(@float * Time.deltaTime);
			}
			else if (!this.skipZeroPoint)
			{
				this.currentSectionIndex = num;
				cursor.Distance = math.GetDistance(-1) - Mathf.Abs(@float * Time.deltaTime);
			}
			return true;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0004CFD3 File Offset: 0x0004B1D3
		private void FirePointReachedEvent(int pointIndex)
		{
			if (this.PointReached != null)
			{
				this.PointReached(this, BGCcCursorChangeLinear.PointReachedArgs.GetInstance(pointIndex));
			}
			if (this.pointReachedEvent.GetPersistentEventCount() > 0)
			{
				this.pointReachedEvent.Invoke(pointIndex);
			}
		}

		// Token: 0x04000D2E RID: 3374
		public const float SpeedThreshold = 1E-05f;

		// Token: 0x04000D30 RID: 3376
		[SerializeField]
		[Tooltip("Cursor will be moved in FixedUpdate instead of Update")]
		private bool useFixedUpdate;

		// Token: 0x04000D31 RID: 3377
		[SerializeField]
		[Tooltip("Constant movement speed along the curve (Speed * Time.deltaTime).You can override this value for each point with speedField")]
		private float speed = 5f;

		// Token: 0x04000D32 RID: 3378
		[SerializeField]
		[Tooltip("How to change speed, then curve's end reached.")]
		private BGCcCursorChangeLinear.OverflowControlEnum overflowControl;

		// Token: 0x04000D33 RID: 3379
		[SerializeField]
		[Tooltip("If curve's length changed, cursor position be adjusted with curve's length to ensure visually constant speed along the curve. ")]
		private bool adjustByTotalLength;

		// Token: 0x04000D34 RID: 3380
		[SerializeField]
		[Tooltip("Field to store the speed between each point. It should be a float field.")]
		private BGCurvePointField speedField;

		// Token: 0x04000D35 RID: 3381
		[SerializeField]
		[Tooltip("Delay at each point. You can override this value for each point with delayField")]
		private float delay;

		// Token: 0x04000D36 RID: 3382
		[SerializeField]
		[Tooltip("Field to store the delays at points. It should be a float field.")]
		private BGCurvePointField delayField;

		// Token: 0x04000D37 RID: 3383
		[SerializeField]
		[Tooltip("Event is fired, then point is reached")]
		private BGCcCursorChangeLinear.PointReachedEvent pointReachedEvent = new BGCcCursorChangeLinear.PointReachedEvent();

		// Token: 0x04000D38 RID: 3384
		private float oldLength;

		// Token: 0x04000D39 RID: 3385
		private bool speedReversed;

		// Token: 0x04000D3A RID: 3386
		private int currentSectionIndex;

		// Token: 0x04000D3B RID: 3387
		private float delayStarted = -1f;

		// Token: 0x04000D3C RID: 3388
		private bool speedWasPositiveWhileDelayed;

		// Token: 0x04000D3D RID: 3389
		private bool skipZeroPoint;

		// Token: 0x020001B0 RID: 432
		public enum OverflowControlEnum
		{
			// Token: 0x04000D40 RID: 3392
			Cycle,
			// Token: 0x04000D41 RID: 3393
			PingPong,
			// Token: 0x04000D42 RID: 3394
			Stop
		}

		// Token: 0x020001B1 RID: 433
		[Serializable]
		public class PointReachedEvent : UnityEvent<int>
		{
		}

		// Token: 0x020001B2 RID: 434
		public class PointReachedArgs : EventArgs
		{
			// Token: 0x1700042D RID: 1069
			// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0004D03A File Offset: 0x0004B23A
			// (set) Token: 0x06000F7A RID: 3962 RVA: 0x0004D042 File Offset: 0x0004B242
			public int PointIndex { get; private set; }

			// Token: 0x06000F7B RID: 3963 RVA: 0x0004BC76 File Offset: 0x00049E76
			private PointReachedArgs()
			{
			}

			// Token: 0x06000F7C RID: 3964 RVA: 0x0004D04B File Offset: 0x0004B24B
			public static BGCcCursorChangeLinear.PointReachedArgs GetInstance(int index)
			{
				BGCcCursorChangeLinear.PointReachedArgs.Instance.PointIndex = index;
				return BGCcCursorChangeLinear.PointReachedArgs.Instance;
			}

			// Token: 0x04000D43 RID: 3395
			private static readonly BGCcCursorChangeLinear.PointReachedArgs Instance = new BGCcCursorChangeLinear.PointReachedArgs();
		}
	}
}
