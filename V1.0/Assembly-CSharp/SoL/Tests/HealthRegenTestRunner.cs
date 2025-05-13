using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Tests
{
	// Token: 0x02000DA7 RID: 3495
	public class HealthRegenTestRunner : MonoBehaviour
	{
		// Token: 0x060068BC RID: 26812 RVA: 0x00086484 File Offset: 0x00084684
		private void Awake()
		{
			if (this.m_executeButton)
			{
				this.m_executeButton.onClick.AddListener(new UnityAction(this.Execute));
			}
		}

		// Token: 0x060068BD RID: 26813 RVA: 0x000864AF File Offset: 0x000846AF
		private void OnDestroy()
		{
			if (this.m_executeButton)
			{
				this.m_executeButton.onClick.RemoveListener(new UnityAction(this.Execute));
			}
		}

		// Token: 0x060068BE RID: 26814 RVA: 0x000864DA File Offset: 0x000846DA
		private void Execute()
		{
			if (this.m_data == null)
			{
				return;
			}
			if (this.m_runnerCo != null)
			{
				base.StopCoroutine(this.m_runnerCo);
			}
			this.m_runnerCo = this.RunCo();
			base.StartCoroutine(this.m_runnerCo);
		}

		// Token: 0x060068BF RID: 26815 RVA: 0x00086518 File Offset: 0x00084718
		private IEnumerator RunCo()
		{
			Debug.Log("Starting!");
			List<HealthRegenResult> processing = new List<HealthRegenResult>(this.m_data.RegenResults.Length);
			for (int i = 0; i < this.m_data.RegenResults.Length; i++)
			{
				this.m_data.RegenResults[i].Init(this.m_data);
				processing.Add(this.m_data.RegenResults[i]);
			}
			while (processing.Count > 0)
			{
				float deltaTime = Time.deltaTime * this.m_timeMultiplier;
				for (int j = processing.Count - 1; j >= 0; j--)
				{
					processing[j].Update(deltaTime);
					if (processing[j].IsFinishedProcessing())
					{
						processing.RemoveAt(j);
					}
				}
				if (this.m_label)
				{
					this.m_label.ZStringSetText("Remaining Runs: " + processing.Count.ToString());
				}
				yield return null;
			}
			Debug.Log("END!");
			yield break;
		}

		// Token: 0x04005B1D RID: 23325
		[Range(1f, 10f)]
		[SerializeField]
		private float m_timeMultiplier = 1f;

		// Token: 0x04005B1E RID: 23326
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04005B1F RID: 23327
		[SerializeField]
		private Button m_executeButton;

		// Token: 0x04005B20 RID: 23328
		[SerializeField]
		private HealthRegenTestScriptable m_data;

		// Token: 0x04005B21 RID: 23329
		private IEnumerator m_runnerCo;
	}
}
