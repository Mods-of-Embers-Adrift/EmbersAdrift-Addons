using System;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DAC RID: 3500
	public class LogTest : MonoBehaviour
	{
		// Token: 0x060068CF RID: 26831 RVA: 0x0004475B File Offset: 0x0004295B
		private void NewLogger()
		{
		}

		// Token: 0x060068D0 RID: 26832 RVA: 0x000865A4 File Offset: 0x000847A4
		private void Verbose()
		{
			this.LogToIndex(LogLevel.Verbose);
		}

		// Token: 0x060068D1 RID: 26833 RVA: 0x000865AD File Offset: 0x000847AD
		private void Debug()
		{
			this.LogToIndex(LogLevel.Debug);
		}

		// Token: 0x060068D2 RID: 26834 RVA: 0x000865B6 File Offset: 0x000847B6
		private void Information()
		{
			this.LogToIndex(LogLevel.Information);
		}

		// Token: 0x060068D3 RID: 26835 RVA: 0x000865BF File Offset: 0x000847BF
		private void Warning()
		{
			this.LogToIndex(LogLevel.Warning);
		}

		// Token: 0x060068D4 RID: 26836 RVA: 0x000865C8 File Offset: 0x000847C8
		private void Error()
		{
			this.LogToIndex(LogLevel.Error);
		}

		// Token: 0x060068D5 RID: 26837 RVA: 0x000865D1 File Offset: 0x000847D1
		private void Fatal()
		{
			this.LogToIndex(LogLevel.Fatal);
		}

		// Token: 0x060068D6 RID: 26838 RVA: 0x00215BA8 File Offset: 0x00213DA8
		private void LogToIndex(LogLevel logLevel)
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			string messageTemplate = "Random Value of: {@RandomValue}";
			SolDebug.LogToIndex(logLevel, this.m_indexType, messageTemplate, new object[]
			{
				num
			});
		}

		// Token: 0x04005B35 RID: 23349
		[SerializeField]
		private LogIndex m_indexType;
	}
}
