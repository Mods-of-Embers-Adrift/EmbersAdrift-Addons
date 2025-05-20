using System;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000266 RID: 614
	[Serializable]
	public abstract class ParameterizedCurve
	{
		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06001376 RID: 4982
		protected abstract float MaxValue { get; }

		// Token: 0x06001377 RID: 4983 RVA: 0x0004FB78 File Offset: 0x0004DD78
		private string GetHorizontalShiftLabel()
		{
			if (this.m_curveType == ParameterizedCurve.CurveType.Logistic)
			{
				return "Inflection (Horizontal Shift)";
			}
			return "Horizontal Shift";
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x000F6E90 File Offset: 0x000F5090
		internal void CalculateCurve()
		{
			this.m_visualization = new AnimationCurve();
			int num = 0;
			while ((float)num <= this.MaxValue)
			{
				this.m_visualization.AddKey((float)num, this.GetValue((float)num));
				num++;
			}
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x000F6ED0 File Offset: 0x000F50D0
		public float GetValue(float input)
		{
			switch (this.m_curveType)
			{
			case ParameterizedCurve.CurveType.Linear:
			{
				float num = input + this.m_horizontalShift;
				return this.m_scalar * num + this.m_verticalShift;
			}
			case ParameterizedCurve.CurveType.Exponential:
			{
				float p = input - this.m_horizontalShift;
				return this.m_scalar * Mathf.Pow(this.m_exponentialBase, p) + this.m_verticalShift;
			}
			case ParameterizedCurve.CurveType.Logistic:
			{
				float power = -this.m_logisticGrowthRate * (input - this.m_horizontalShift);
				return this.m_logisticMaxValue / (1f + this.m_scalar * Mathf.Exp(power)) + this.m_verticalShift;
			}
			default:
				return 0f;
			}
		}

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x0600137A RID: 4986 RVA: 0x0004FB8E File Offset: 0x0004DD8E
		// (set) Token: 0x0600137B RID: 4987 RVA: 0x0004FB96 File Offset: 0x0004DD96
		[JsonProperty]
		[JsonConverter(typeof(StringEnumConverter))]
		[BsonElement]
		[BsonRepresentation(BsonType.String)]
		public ParameterizedCurve.CurveType Type
		{
			get
			{
				return this.m_curveType;
			}
			private set
			{
				this.m_curveType = value;
			}
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x0004FB9F File Offset: 0x0004DD9F
		// (set) Token: 0x0600137D RID: 4989 RVA: 0x0004FBA7 File Offset: 0x0004DDA7
		[JsonProperty]
		[BsonElement]
		public float Scalar
		{
			get
			{
				return this.m_scalar;
			}
			private set
			{
				this.m_scalar = value;
			}
		}

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x0600137E RID: 4990 RVA: 0x0004FBB0 File Offset: 0x0004DDB0
		// (set) Token: 0x0600137F RID: 4991 RVA: 0x0004FBB8 File Offset: 0x0004DDB8
		[JsonProperty]
		[BsonElement]
		public float HorizontalShift
		{
			get
			{
				return this.m_horizontalShift;
			}
			private set
			{
				this.m_horizontalShift = value;
			}
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x0004FBC1 File Offset: 0x0004DDC1
		// (set) Token: 0x06001381 RID: 4993 RVA: 0x0004FBC9 File Offset: 0x0004DDC9
		[JsonProperty]
		[BsonElement]
		public float VerticalShift
		{
			get
			{
				return this.m_verticalShift;
			}
			private set
			{
				this.m_verticalShift = value;
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x0004FBD2 File Offset: 0x0004DDD2
		// (set) Token: 0x06001383 RID: 4995 RVA: 0x0004FBDA File Offset: 0x0004DDDA
		[JsonProperty]
		[BsonElement]
		public float ExponentialBase
		{
			get
			{
				return this.m_exponentialBase;
			}
			private set
			{
				this.m_exponentialBase = value;
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x0004FBE3 File Offset: 0x0004DDE3
		// (set) Token: 0x06001385 RID: 4997 RVA: 0x0004FBEB File Offset: 0x0004DDEB
		[JsonProperty]
		[BsonElement]
		public float LogisticGrowthRate
		{
			get
			{
				return this.m_logisticGrowthRate;
			}
			private set
			{
				this.m_logisticGrowthRate = value;
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06001386 RID: 4998 RVA: 0x0004FBF4 File Offset: 0x0004DDF4
		// (set) Token: 0x06001387 RID: 4999 RVA: 0x0004FBFC File Offset: 0x0004DDFC
		[JsonProperty]
		[BsonElement]
		public float LogisticMaxValue
		{
			get
			{
				return this.m_logisticMaxValue;
			}
			private set
			{
				this.m_logisticMaxValue = value;
			}
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x0004FC05 File Offset: 0x0004DE05
		private string GetValueString()
		{
			if (this.m_horizontalShift != 0f)
			{
				return "(value - " + this.m_horizontalShift.ToString(CultureInfo.InvariantCulture) + ")";
			}
			return "value";
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x000F6F70 File Offset: 0x000F5170
		public override string ToString()
		{
			switch (this.m_curveType)
			{
			case ParameterizedCurve.CurveType.Linear:
				return string.Concat(new string[]
				{
					"[LINEAR]  ",
					this.m_scalar.ToString(CultureInfo.InvariantCulture),
					" * ",
					this.GetValueString(),
					" + ",
					this.m_verticalShift.ToString(CultureInfo.InvariantCulture)
				});
			case ParameterizedCurve.CurveType.Exponential:
				return string.Concat(new string[]
				{
					"[EXPONENTIAL]  ",
					this.m_scalar.ToString(CultureInfo.InvariantCulture),
					" * ",
					this.m_exponentialBase.ToString(CultureInfo.InvariantCulture),
					"^",
					this.GetValueString(),
					" + ",
					this.m_verticalShift.ToString(CultureInfo.InvariantCulture)
				});
			case ParameterizedCurve.CurveType.Logistic:
				return string.Concat(new string[]
				{
					"[LOGISTIC]  ",
					this.m_logisticMaxValue.ToString(CultureInfo.InvariantCulture),
					" / (1f + ",
					this.m_scalar.ToString(CultureInfo.InvariantCulture),
					" * exp(-",
					this.m_logisticGrowthRate.ToString(CultureInfo.InvariantCulture),
					" * ",
					this.GetValueString(),
					")) + ",
					this.m_verticalShift.ToString(CultureInfo.InvariantCulture)
				});
			default:
				return "[UNKNOWN] 0";
			}
		}

		// Token: 0x04001BD5 RID: 7125
		[SerializeField]
		private ParameterizedCurve.CurveType m_curveType;

		// Token: 0x04001BD6 RID: 7126
		[SerializeField]
		private float m_scalar = 1f;

		// Token: 0x04001BD7 RID: 7127
		[SerializeField]
		private float m_horizontalShift;

		// Token: 0x04001BD8 RID: 7128
		[SerializeField]
		private float m_verticalShift;

		// Token: 0x04001BD9 RID: 7129
		[SerializeField]
		private float m_exponentialBase = 1f;

		// Token: 0x04001BDA RID: 7130
		[SerializeField]
		private float m_logisticGrowthRate = 1f;

		// Token: 0x04001BDB RID: 7131
		[SerializeField]
		private float m_logisticMaxValue = 1500f;

		// Token: 0x04001BDC RID: 7132
		[JsonIgnore]
		[BsonIgnore]
		[SerializeField]
		private AnimationCurve m_visualization;

		// Token: 0x04001BDD RID: 7133
		[TextArea(1, 100)]
		[SerializeField]
		private string m_notes;

		// Token: 0x02000267 RID: 615
		public enum CurveType
		{
			// Token: 0x04001BDF RID: 7135
			Linear,
			// Token: 0x04001BE0 RID: 7136
			Exponential,
			// Token: 0x04001BE1 RID: 7137
			Logistic
		}
	}
}
