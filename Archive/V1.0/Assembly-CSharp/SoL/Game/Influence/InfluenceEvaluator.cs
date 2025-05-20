using System;
using DynamicExpresso;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BBF RID: 3007
	[Serializable]
	public class InfluenceEvaluator
	{
		// Token: 0x06005D18 RID: 23832 RVA: 0x0007E986 File Offset: 0x0007CB86
		public float Evaluate(Vector3 position, float radius)
		{
			if (!this.m_useExpression)
			{
				return this.EvaluateSingleFlag(position, radius);
			}
			return this.EvaluateExpression(position, radius);
		}

		// Token: 0x06005D19 RID: 23833 RVA: 0x0007E9A1 File Offset: 0x0007CBA1
		private float EvaluateSingleFlag(Vector3 position, float radius)
		{
			return InfluenceMap.SampleValueAtPoint(position, radius, this.m_flags, true);
		}

		// Token: 0x06005D1A RID: 23834 RVA: 0x001F2B60 File Offset: 0x001F0D60
		private float EvaluateExpression(Vector3 position, float radius)
		{
			if (this.m_expressionValues == null)
			{
				this.m_expressionValues = new float[this.m_expressionFlags.Length];
				this.m_interpreter = new Interpreter();
				this.m_interpreter.SetVariable("myArr", this.m_expressionValues);
				this.m_getInterpreterResult = this.m_interpreter.ParseAsDelegate<Func<float>>(this.m_expression, Array.Empty<string>());
			}
			for (int i = 0; i < this.m_expressionFlags.Length; i++)
			{
				this.m_expressionValues[i] = InfluenceMap.SampleValueAtPoint(position, radius, this.m_expressionFlags[i], i == 0);
			}
			return this.m_getInterpreterResult();
		}

		// Token: 0x04005079 RID: 20601
		private const string kArrayName = "myArr";

		// Token: 0x0400507A RID: 20602
		[SerializeField]
		private bool m_useExpression;

		// Token: 0x0400507B RID: 20603
		[SerializeField]
		private InfluenceFlags m_flags;

		// Token: 0x0400507C RID: 20604
		[SerializeField]
		private InfluenceFlags[] m_expressionFlags;

		// Token: 0x0400507D RID: 20605
		[SerializeField]
		private string m_expression;

		// Token: 0x0400507E RID: 20606
		private float[] m_expressionValues;

		// Token: 0x0400507F RID: 20607
		private Interpreter m_interpreter;

		// Token: 0x04005080 RID: 20608
		private Func<float> m_getInterpreterResult;
	}
}
