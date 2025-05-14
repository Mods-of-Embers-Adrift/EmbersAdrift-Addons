using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using Ionic.Zlib;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace SoL.Tests
{
	// Token: 0x02000DC4 RID: 3524
	public class UploadFileTest : MonoBehaviour
	{
		// Token: 0x06006938 RID: 26936 RVA: 0x000868CC File Offset: 0x00084ACC
		private void Upload()
		{
			if (this.m_uplaodCo != null)
			{
				base.StopCoroutine(this.m_uplaodCo);
			}
			this.m_uplaodCo = this.UploadFileCo();
			base.StartCoroutine(this.m_uplaodCo);
		}

		// Token: 0x06006939 RID: 26937 RVA: 0x000868FB File Offset: 0x00084AFB
		private IEnumerator UploadFileCo()
		{
			string uri = string.Concat(new string[]
			{
				"http://",
				this.m_address,
				":",
				this.m_port.ToString(),
				"/upload_crashlog/dev"
			});
			WWWForm wwwform = new WWWForm();
			byte[] array = Encoding.UTF8.GetBytes(this.m_textFile.text);
			array = Ionic.Zlib.GZipStream.CompressBuffer(array);
			Debug.Log("NonCompressed: " + array.Length.ToString() + ", Compressed: " + this.Compress(this.m_textFile.text).Length.ToString());
			string fileName = DateTime.UtcNow.ToString("MMddyyHHmmss") + "_crashlog.zip";
			wwwform.AddBinaryData("file", array, fileName);
			using (UnityWebRequest www = UnityWebRequest.Post(uri, wwwform))
			{
				yield return www.SendWebRequest();
				if (www.IsWebError())
				{
					Debug.LogWarning("Error updating indexes! " + www.error);
				}
			}
			UnityWebRequest www = null;
			yield break;
			yield break;
		}

		// Token: 0x0600693A RID: 26938 RVA: 0x002171F8 File Offset: 0x002153F8
		public byte[] Compress(string uncompressedString)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(memoryStream2, System.IO.Compression.CompressionLevel.Fastest, true))
					{
						memoryStream.CopyTo(deflateStream);
					}
					result = memoryStream2.ToArray();
				}
			}
			return result;
		}

		// Token: 0x04005B9A RID: 23450
		[SerializeField]
		private string m_address = "127.0.0.1";

		// Token: 0x04005B9B RID: 23451
		[SerializeField]
		private int m_port = 14123;

		// Token: 0x04005B9C RID: 23452
		[SerializeField]
		private TextAsset m_textFile;

		// Token: 0x04005B9D RID: 23453
		private IEnumerator m_uplaodCo;
	}
}
