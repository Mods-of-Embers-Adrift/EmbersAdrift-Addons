using System;
using SoL.Game.NPCs;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C92 RID: 3218
	public class DungeonEntranceVisuals : MonoBehaviour
	{
		// Token: 0x060061BC RID: 25020 RVA: 0x00201688 File Offset: 0x001FF888
		private void Start()
		{
			if (this.m_entrance == null)
			{
				base.enabled = false;
				return;
			}
			this.m_entrance.Status.Changed += this.StatusOnChanged;
			this.StatusOnChanged(this.m_entrance.Status.Value);
			if (this.m_visuals != null)
			{
				for (int i = 0; i < this.m_visuals.Length; i++)
				{
					this.m_visuals[i].Obj.SetActive(this.m_visuals[i].Tier == this.m_entrance.Tier.Value);
				}
			}
		}

		// Token: 0x060061BD RID: 25021 RVA: 0x00081E1B File Offset: 0x0008001B
		private void OnDestroy()
		{
			if (this.m_entrance)
			{
				this.m_entrance.Status.Changed -= this.StatusOnChanged;
			}
		}

		// Token: 0x060061BE RID: 25022 RVA: 0x0020172C File Offset: 0x001FF92C
		private void Update()
		{
			if (this.m_timeRenderer && this.m_entrance && this.m_entrance.DeactivationTime.Value != null)
			{
				double totalSeconds = (this.m_entrance.DeactivationTime.Value.Value - this.m_entrance.ActivationTime).TotalSeconds;
				double num = (this.m_entrance.DeactivationTime.Value.Value - GameTimeReplicator.GetServerCorrectedDateTimeUtc()).TotalSeconds;
				if (num < 0.0)
				{
					num = 0.0;
				}
				float t = (float)(num / totalSeconds);
				float r = Mathf.Lerp(0f, 1f, t);
				Color color = new Color(r, 0f, 0f);
				this.m_timeRenderer.SetEmissionColor(color);
			}
		}

		// Token: 0x060061BF RID: 25023 RVA: 0x00201828 File Offset: 0x001FFA28
		private void StatusOnChanged(DungeonEntranceStatus obj)
		{
			bool flag = obj == DungeonEntranceStatus.Active;
			if (this.m_statusRenderer)
			{
				Color color = flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				this.m_statusRenderer.SetMainColor(color);
			}
			if (this.m_toggleController)
			{
				this.m_toggleController.State = (flag ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
			}
			if (this.m_dayNightManual)
			{
				this.m_dayNightManual.Toggle(flag);
			}
		}

		// Token: 0x04005540 RID: 21824
		[SerializeField]
		private BaseDungeonEntrance m_entrance;

		// Token: 0x04005541 RID: 21825
		[SerializeField]
		private DungeonEntranceVisuals.SpawnTierVisuals[] m_visuals;

		// Token: 0x04005542 RID: 21826
		[SerializeField]
		private Renderer m_statusRenderer;

		// Token: 0x04005543 RID: 21827
		[SerializeField]
		private Renderer m_timeRenderer;

		// Token: 0x04005544 RID: 21828
		[SerializeField]
		private ToggleController m_toggleController;

		// Token: 0x04005545 RID: 21829
		[SerializeField]
		private DayNightManual m_dayNightManual;

		// Token: 0x02000C93 RID: 3219
		[Serializable]
		private class SpawnTierVisuals
		{
			// Token: 0x04005546 RID: 21830
			public SpawnTier Tier;

			// Token: 0x04005547 RID: 21831
			public GameObject Obj;
		}
	}
}
