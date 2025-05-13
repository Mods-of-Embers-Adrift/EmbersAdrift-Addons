using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DAD RID: 3501
	public class LogTesting : MonoBehaviour
	{
		// Token: 0x060068D8 RID: 26840 RVA: 0x000865DA File Offset: 0x000847DA
		private void Log()
		{
			Debug.Log("Test!");
		}

		// Token: 0x060068D9 RID: 26841 RVA: 0x000865E6 File Offset: 0x000847E6
		private void Warning()
		{
			Debug.LogWarning("Test!");
		}

		// Token: 0x060068DA RID: 26842 RVA: 0x000865F2 File Offset: 0x000847F2
		private void Error()
		{
			Debug.LogError("Test!");
		}

		// Token: 0x04005B36 RID: 23350
		private const string kMessage = "Test!";

		// Token: 0x04005B37 RID: 23351
		private const string kButtonGroup = "Buttons";
	}
}
