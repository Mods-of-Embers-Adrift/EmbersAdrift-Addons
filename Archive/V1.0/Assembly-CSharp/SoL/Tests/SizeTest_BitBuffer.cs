using System;
using NetStack.Serialization;
using SoL.Networking;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DBE RID: 3518
	public class SizeTest_BitBuffer : MonoBehaviour
	{
		// Token: 0x06006923 RID: 26915 RVA: 0x00216B00 File Offset: 0x00214D00
		private void Start()
		{
			BitBuffer bitBuffer = new BitBuffer(128);
			UniqueId id = UniqueId.GenerateFromGuid();
			bitBuffer.AddUniqueId(id);
			Debug.Log(string.Format("Unique Id string: {0}", bitBuffer.Length));
			bitBuffer.Clear();
			int hashCode = id.Value.GetHashCode();
			bitBuffer.AddInt(hashCode);
			Debug.Log(string.Format("Unique Id int: {0}", bitBuffer.Length));
		}
	}
}
