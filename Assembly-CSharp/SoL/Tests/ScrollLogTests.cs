using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Com.TheFallenGames.OSA.Core;
using TMPro;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DB6 RID: 3510
	public class ScrollLogTests : OSA<TestParams, TestLineViewsHolder>
	{
		// Token: 0x06006902 RID: 26882 RVA: 0x00216544 File Offset: 0x00214744
		private void AddToQueue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = UnityEngine.Random.Range((int)this.m_linesToInsert.x, (int)this.m_linesToInsert.y);
			for (int i = 0; i < num; i++)
			{
				int num2 = UnityEngine.Random.Range((int)this.m_charsToInsert.x, (int)this.m_charsToInsert.y);
				for (int j = 0; j < num2; j++)
				{
					int index = UnityEngine.Random.Range(0, this.m_availableChars.Length);
					stringBuilder.Append(this.m_availableChars[index]);
				}
				this.m_addQueue.Enqueue(stringBuilder.ToString());
				stringBuilder.Clear();
			}
		}

		// Token: 0x06006903 RID: 26883 RVA: 0x00086748 File Offset: 0x00084948
		protected override void Start()
		{
			base.Start();
			base.StartCoroutine("UpdateCo");
		}

		// Token: 0x06006904 RID: 26884 RVA: 0x0008675C File Offset: 0x0008495C
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				bool modified = false;
				while (this.m_addQueue.Count > 0)
				{
					string text = this.m_addQueue.Dequeue();
					this.m_messages.Add(text);
					this.m_messageHeights.Add(this.m_sampleText.GetPreferredValues(text, this.m_sampleText.rectTransform.rect.width, this.m_sampleText.rectTransform.rect.height).y);
					while (this.m_messages.Count >= this.m_maxLinesToHold)
					{
						this.m_messages.RemoveAt(0);
						this.m_messageHeights.RemoveAt(0);
					}
					modified = true;
					yield return null;
				}
				if (modified)
				{
					this.ResetItems(this.m_messages.Count, false, false);
					base.SetNormalizedPosition(0.0);
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06006905 RID: 26885 RVA: 0x0008676B File Offset: 0x0008496B
		protected override TestLineViewsHolder CreateViewsHolder(int itemIndex)
		{
			TestLineViewsHolder testLineViewsHolder = new TestLineViewsHolder();
			testLineViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return testLineViewsHolder;
		}

		// Token: 0x06006906 RID: 26886 RVA: 0x00086791 File Offset: 0x00084991
		protected override void UpdateViewsHolder(TestLineViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateChatLine(this.m_messages[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex);
		}

		// Token: 0x06006907 RID: 26887 RVA: 0x002165F0 File Offset: 0x002147F0
		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);
			if (changeMode != ItemCountChangeMode.RESET)
			{
				return;
			}
			if (count == 0)
			{
				return;
			}
			int num = 0;
			int num2 = num + count;
			itemsDesc.BeginChangingItemsSizes(num);
			for (int i = num; i < num2; i++)
			{
				itemsDesc[i] = (double)this.m_messageHeights[i];
			}
			itemsDesc.EndChangingItemsSizes();
		}

		// Token: 0x04005B5E RID: 23390
		private List<float> m_messageHeights = new List<float>();

		// Token: 0x04005B5F RID: 23391
		private List<string> m_messages = new List<string>();

		// Token: 0x04005B60 RID: 23392
		private Queue<string> m_addQueue = new Queue<string>();

		// Token: 0x04005B61 RID: 23393
		private string m_availableChars = " ABCDE FGHIJ KLMNO PQRST UVWXYZ ";

		// Token: 0x04005B62 RID: 23394
		[SerializeField]
		private TextMeshProUGUI m_sampleText;

		// Token: 0x04005B63 RID: 23395
		[SerializeField]
		private int m_maxLinesToHold = 100;

		// Token: 0x04005B64 RID: 23396
		[SerializeField]
		private Vector2 m_linesToInsert = new Vector2(10f, 30f);

		// Token: 0x04005B65 RID: 23397
		[SerializeField]
		private Vector2 m_charsToInsert = new Vector2(10f, 100f);
	}
}
