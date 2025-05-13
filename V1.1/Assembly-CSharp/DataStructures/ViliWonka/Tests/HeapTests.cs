using System;
using System.Collections.Generic;
using DataStructures.ViliWonka.Heap;
using UnityEngine;

namespace DataStructures.ViliWonka.Tests
{
	// Token: 0x02000151 RID: 337
	public class HeapTests : MonoBehaviour
	{
		// Token: 0x06000B88 RID: 2952 RVA: 0x000CC58C File Offset: 0x000CA78C
		private void Start()
		{
			KSmallestHeap<int> ksmallestHeap = new KSmallestHeap<int>(13);
			for (int i = 0; i < 100; i++)
			{
				ksmallestHeap.PushObj(i, UnityEngine.Random.value);
			}
			List<int> list = new List<int>();
			List<float> list2 = new List<float>();
			ksmallestHeap.FlushResult(list, list2);
			for (int j = 0; j < list.Count; j++)
			{
				Debug.Log(list[j].ToString() + " " + list2[j].ToString());
			}
			ksmallestHeap.HeapPropertyHolds(0, 0);
		}
	}
}
