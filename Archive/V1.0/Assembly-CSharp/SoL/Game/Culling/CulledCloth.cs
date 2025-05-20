using System;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CBD RID: 3261
	public class CulledCloth : CulledObject
	{
		// Token: 0x060062CE RID: 25294 RVA: 0x00205A94 File Offset: 0x00203C94
		private void Awake()
		{
			if (!this.m_cloth)
			{
				base.enabled = false;
				return;
			}
			if (GameManager.IsServer)
			{
				this.m_cloth.enabled = false;
				return;
			}
			if (CulledCloth.m_validCapsuleColliders == null)
			{
				CulledCloth.m_validCapsuleColliders = new List<CapsuleCollider>(10);
			}
			CulledCloth.m_validCapsuleColliders.Clear();
			for (int i = 0; i < this.m_colliders.Length; i++)
			{
				if (this.m_colliders[i] && this.m_colliders[i].gameObject.activeInHierarchy)
				{
					CulledCloth.m_validCapsuleColliders.Add(this.m_colliders[i]);
				}
			}
			if (CulledCloth.m_validCapsuleColliders.Count > 0)
			{
				this.m_cloth.capsuleColliders = CulledCloth.m_validCapsuleColliders.ToArray();
				CulledCloth.m_validCapsuleColliders.Clear();
			}
			this.RefreshCloth();
			Options.GameOptions.EnableClothSim.Changed += this.EnableClothSimOnChanged;
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x00082943 File Offset: 0x00080B43
		private void OnDestroy()
		{
			Options.GameOptions.EnableClothSim.Changed -= this.EnableClothSimOnChanged;
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x0008295B File Offset: 0x00080B5B
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			this.RefreshCloth();
		}

		// Token: 0x060062D1 RID: 25297 RVA: 0x00082969 File Offset: 0x00080B69
		private void EnableClothSimOnChanged()
		{
			this.RefreshCloth();
		}

		// Token: 0x060062D2 RID: 25298 RVA: 0x00205B78 File Offset: 0x00203D78
		private void RefreshCloth()
		{
			if (this.m_cloth)
			{
				bool flag = (GameManager.IsOffline || Options.GameOptions.EnableClothSim.Value) && !this.IsCulled();
				if (flag != this.m_cloth.enabled)
				{
					this.m_cloth.enabled = flag;
				}
			}
		}

		// Token: 0x04005623 RID: 22051
		private static List<CapsuleCollider> m_validCapsuleColliders;

		// Token: 0x04005624 RID: 22052
		[SerializeField]
		private Cloth m_cloth;

		// Token: 0x04005625 RID: 22053
		[SerializeField]
		private CapsuleCollider[] m_colliders;
	}
}
