using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Tests
{
	// Token: 0x02000DBA RID: 3514
	public class ScrollLogTestScrollView : MonoBehaviour
	{
		// Token: 0x06006915 RID: 26901 RVA: 0x0021685C File Offset: 0x00214A5C
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

		// Token: 0x06006916 RID: 26902 RVA: 0x00216908 File Offset: 0x00214B08
		private void Start()
		{
			this.m_tmp = new List<TextMeshProUGUI>(this.m_maxLinesToHold);
			for (int i = 0; i < this.m_maxLinesToHold; i++)
			{
				TextMeshProUGUI component = UnityEngine.Object.Instantiate<GameObject>(this.m_chatPrefab, this.m_parentRect).GetComponent<TextMeshProUGUI>();
				this.m_tmp.Add(component);
				component.text = "";
			}
			this.m_index = this.m_tmp.Count - 1;
			base.StartCoroutine("UpdateCo");
		}

		// Token: 0x06006917 RID: 26903 RVA: 0x0008680F File Offset: 0x00084A0F
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				if (this.m_addQueue.Count <= 0)
				{
					yield return null;
				}
				else
				{
					string txt = this.m_addQueue.Dequeue();
					this.AddLine(txt);
					this.m_scrollRect.verticalNormalizedPosition = 0f;
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x06006918 RID: 26904 RVA: 0x00216984 File Offset: 0x00214B84
		private void AddLine(string txt)
		{
			this.m_tmp[this.m_index].text = "[" + this.m_index.ToString() + "] " + txt;
			this.m_tmp[this.m_index].gameObject.transform.SetAsLastSibling();
			this.m_index--;
			if (this.m_index < 0)
			{
				this.m_index = this.m_tmp.Count - 1;
			}
		}

		// Token: 0x04005B6D RID: 23405
		private Queue<string> m_addQueue = new Queue<string>();

		// Token: 0x04005B6E RID: 23406
		private List<TextMeshProUGUI> m_tmp;

		// Token: 0x04005B6F RID: 23407
		private int m_index;

		// Token: 0x04005B70 RID: 23408
		private string m_availableChars = " ABCDE FGHIJ KLMNO PQRST UVWXYZ ";

		// Token: 0x04005B71 RID: 23409
		[SerializeField]
		private GameObject m_chatPrefab;

		// Token: 0x04005B72 RID: 23410
		[SerializeField]
		private RectTransform m_parentRect;

		// Token: 0x04005B73 RID: 23411
		[SerializeField]
		private ScrollRect m_scrollRect;

		// Token: 0x04005B74 RID: 23412
		[SerializeField]
		private int m_maxLinesToHold = 100;

		// Token: 0x04005B75 RID: 23413
		[SerializeField]
		private Vector2 m_linesToInsert = new Vector2(10f, 30f);

		// Token: 0x04005B76 RID: 23414
		[SerializeField]
		private Vector2 m_charsToInsert = new Vector2(10f, 50f);
	}
}
