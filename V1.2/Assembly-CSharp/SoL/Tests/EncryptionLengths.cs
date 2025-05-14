using System;
using System.Text;
using SoL.Networking.SolServer;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D9E RID: 3486
	public class EncryptionLengths : MonoBehaviour
	{
		// Token: 0x06006897 RID: 26775 RVA: 0x00215068 File Offset: 0x00213268
		private void AttemptEncryption()
		{
			string s = this.m_manualPassword ? this.m_password : new string('*', this.m_length);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			Debug.Log("ByteArray Length=" + bytes.Length.ToString());
			Debug.Log(SolServerEncryption.EncryptRsa(bytes));
		}

		// Token: 0x06006898 RID: 26776 RVA: 0x002150C4 File Offset: 0x002132C4
		private string GetPasswordLength()
		{
			int num = this.m_length;
			if (this.m_manualPassword)
			{
				num = (string.IsNullOrEmpty(this.m_password) ? 0 : this.m_password.Length);
			}
			return "Password Length: " + num.ToString();
		}

		// Token: 0x06006899 RID: 26777 RVA: 0x00045BC3 File Offset: 0x00043DC3
		private string GetDetails()
		{
			return string.Empty;
		}

		// Token: 0x04005AE0 RID: 23264
		[SerializeField]
		private bool m_manualPassword;

		// Token: 0x04005AE1 RID: 23265
		[SerializeField]
		private int m_length = 16;

		// Token: 0x04005AE2 RID: 23266
		[SerializeField]
		private string m_password;

		// Token: 0x04005AE3 RID: 23267
		[SerializeField]
		private DummyClass m_dummy;
	}
}
