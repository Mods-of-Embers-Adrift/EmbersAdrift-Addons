using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F6 RID: 1014
	public static class SolServerEncryption
	{
		// Token: 0x06001AFF RID: 6911 RVA: 0x00054F5B File Offset: 0x0005315B
		private static byte[] GetUtfByteArray(string input)
		{
			return Encoding.UTF8.GetBytes(input);
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x00054F68 File Offset: 0x00053168
		public static bool CanBeEncoded(string input)
		{
			return SolServerEncryption.GetUtfByteArray(input).Length <= 86;
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x00054F79 File Offset: 0x00053179
		public static string EncryptRsa(string input)
		{
			return SolServerEncryption.EncryptRsa(SolServerEncryption.GetUtfByteArray(input));
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x0010A78C File Offset: 0x0010898C
		public static string EncryptRsa(byte[] input)
		{
			string result = string.Empty;
			using (RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)new X509Certificate2(X509Certificate.CreateFromCertFile(SolServerEncryption.m_certFile)).PublicKey.Key)
			{
				result = Convert.ToBase64String(rsacryptoServiceProvider.Encrypt(input, true));
			}
			return result;
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x0010A7EC File Offset: 0x001089EC
		public static string EncryptAES(string input, out string key)
		{
			string result = string.Empty;
			using (Aes aes = Aes.Create())
			{
				aes.Mode = CipherMode.CBC;
				key = SolServerEncryption.EncryptRsa(aes.Key);
				result = Convert.ToBase64String(SolServerEncryption.EncryptStringToBytes_Aes(input, aes.Key, aes.IV));
			}
			return result;
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x0010A850 File Offset: 0x00108A50
		private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
		{
			byte[] array;
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;
				aes.Mode = CipherMode.CBC;
				ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(plainText);
						}
						array = memoryStream.ToArray();
					}
				}
			}
			byte[] array2 = new byte[IV.Length + array.Length];
			Array.Copy(IV, 0, array2, 0, IV.Length);
			Array.Copy(array, 0, array2, IV.Length, array.Length);
			return array2;
		}

		// Token: 0x04002232 RID: 8754
		private const string kCertFile = "sol.crt";

		// Token: 0x04002233 RID: 8755
		private static readonly string m_certFile = Path.Combine(Application.streamingAssetsPath, "sol.crt");
	}
}
