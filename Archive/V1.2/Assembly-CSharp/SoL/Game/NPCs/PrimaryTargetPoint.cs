using System;
using Cysharp.Text;
using SoL.Game.Randomization;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000827 RID: 2087
	public class PrimaryTargetPoint : GameEntityComponent
	{
		// Token: 0x06003CAC RID: 15532 RVA: 0x00180D78 File Offset: 0x0017EF78
		private void Awake()
		{
			if (base.GameEntity != null && this.m_point)
			{
				base.GameEntity.PrimaryTargetPoint = this.m_point;
				NpcScaleAdjuster npcScaleAdjuster = base.gameObject.AddComponent<NpcScaleAdjuster>();
				if (npcScaleAdjuster)
				{
					npcScaleAdjuster.SetVarsExternal(true, true);
				}
			}
		}

		// Token: 0x06003CAD RID: 15533 RVA: 0x00069195 File Offset: 0x00067395
		private void OnDrawGizmosSelected()
		{
			if (this.m_point && this.m_point.transform)
			{
				Gizmos.DrawSphere(this.m_point.transform.position, 0.25f);
			}
		}

		// Token: 0x06003CAE RID: 15534 RVA: 0x00180DD0 File Offset: 0x0017EFD0
		private string GetMatchPosMessage()
		{
			string result = string.Empty;
			if (this.m_matchPos != null && this.m_matchPos.Length != 0 && this.m_point)
			{
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					for (int i = 0; i < this.m_matchPos.Length; i++)
					{
						if (this.m_matchPos[i] == null)
						{
							utf16ValueStringBuilder.AppendFormat<int>("[{0}] NULL!\n", i);
						}
						else
						{
							Vector3 arg = this.m_matchPos[i].transform.localPosition - this.m_point.transform.localPosition;
							utf16ValueStringBuilder.AppendFormat<int, Vector3, string>("[{0}] {1} ({2})\n", i, arg, this.m_matchPos[i].name);
						}
					}
					result = utf16ValueStringBuilder.ToString();
				}
			}
			return result;
		}

		// Token: 0x04003B68 RID: 15208
		[SerializeField]
		private GameObject m_point;

		// Token: 0x04003B69 RID: 15209
		[SerializeField]
		private GameObject[] m_matchPos;

		// Token: 0x04003B6A RID: 15210
		[SerializeField]
		private DummyClass m_dummy;
	}
}
