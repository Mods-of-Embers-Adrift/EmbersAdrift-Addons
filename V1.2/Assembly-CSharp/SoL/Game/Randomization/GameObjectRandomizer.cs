using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000763 RID: 1891
	public class GameObjectRandomizer : BaseRandomizer
	{
		// Token: 0x06003835 RID: 14389 RVA: 0x0016CFB8 File Offset: 0x0016B1B8
		protected override void RandomizeInternal(System.Random seed)
		{
			if (this.m_gameObjects == null || this.m_gameObjects.Length == 0)
			{
				return;
			}
			GameObjectRandomizer.RandomType type = this.m_type;
			if (type != GameObjectRandomizer.RandomType.Single)
			{
				if (type == GameObjectRandomizer.RandomType.All)
				{
					for (int i = 0; i < this.m_gameObjects.Length; i++)
					{
						this.ToggleObject(this.m_gameObjects[i], seed.NextDouble() < 0.5);
					}
					return;
				}
			}
			else
			{
				int num = seed.Next(0, this.m_gameObjects.Length);
				for (int j = 0; j < this.m_gameObjects.Length; j++)
				{
					this.ToggleObject(this.m_gameObjects[j], j == num);
				}
			}
		}

		// Token: 0x06003836 RID: 14390 RVA: 0x0016D050 File Offset: 0x0016B250
		private void EnableAll()
		{
			for (int i = 0; i < this.m_gameObjects.Length; i++)
			{
				this.ToggleObject(this.m_gameObjects[i], true);
			}
		}

		// Token: 0x06003837 RID: 14391 RVA: 0x0016D080 File Offset: 0x0016B280
		private void DisableAll()
		{
			for (int i = 0; i < this.m_gameObjects.Length; i++)
			{
				this.ToggleObject(this.m_gameObjects[i], false);
			}
		}

		// Token: 0x06003838 RID: 14392 RVA: 0x000664C6 File Offset: 0x000646C6
		private void ToggleObject(GameObject obj, bool enabled)
		{
			if (!obj)
			{
				return;
			}
			obj.SetActive(enabled);
		}

		// Token: 0x04003711 RID: 14097
		[SerializeField]
		private GameObjectRandomizer.RandomType m_type = GameObjectRandomizer.RandomType.All;

		// Token: 0x04003712 RID: 14098
		[SerializeField]
		private GameObject[] m_gameObjects;

		// Token: 0x02000764 RID: 1892
		private enum RandomType
		{
			// Token: 0x04003714 RID: 14100
			Single,
			// Token: 0x04003715 RID: 14101
			All
		}
	}
}
