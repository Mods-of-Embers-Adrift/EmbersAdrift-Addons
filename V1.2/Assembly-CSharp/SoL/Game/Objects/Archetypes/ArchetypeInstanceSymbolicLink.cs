using System;
using SoL.Game.Objects.Containers;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A28 RID: 2600
	public class ArchetypeInstanceSymbolicLink : IPoolable
	{
		// Token: 0x06005089 RID: 20617 RVA: 0x001CD944 File Offset: 0x001CBB44
		public void InitUI()
		{
			if (this.m_symbolicLinkUi == null && ClientGameManager.UIManager && ClientGameManager.UIManager.ArchetypeInstanceSymbolicLinkUIPrefab)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ClientGameManager.UIManager.ArchetypeInstanceSymbolicLinkUIPrefab);
				this.m_symbolicLinkUi = gameObject.GetComponent<ArchetypeInstanceSymbolicLinkUI>();
			}
			if (this.m_symbolicLinkUi)
			{
				this.m_symbolicLinkUi.Initialize(this.Instance);
			}
		}

		// Token: 0x170011E4 RID: 4580
		// (get) Token: 0x0600508A RID: 20618 RVA: 0x00075F2B File Offset: 0x0007412B
		// (set) Token: 0x0600508B RID: 20619 RVA: 0x00075F33 File Offset: 0x00074133
		bool IPoolable.InPool { get; set; }

		// Token: 0x0600508C RID: 20620 RVA: 0x00075F3C File Offset: 0x0007413C
		void IPoolable.Reset()
		{
			this.Freeze = false;
			this.Instance = null;
			this.PreviousContainer = null;
			this.PreviousIndex = -1;
			if (this.m_symbolicLinkUi)
			{
				this.m_symbolicLinkUi.Initialize(null);
			}
		}

		// Token: 0x0600508D RID: 20621 RVA: 0x001CD9B8 File Offset: 0x001CBBB8
		public override string ToString()
		{
			string str = (this.PreviousContainer != null) ? this.PreviousContainer.ContainerType.ToString() : "NULL";
			return "from " + str + " @ " + this.PreviousIndex.ToString();
		}

		// Token: 0x04004853 RID: 18515
		public bool Freeze;

		// Token: 0x04004854 RID: 18516
		public ArchetypeInstance Instance;

		// Token: 0x04004855 RID: 18517
		public ContainerInstance PreviousContainer;

		// Token: 0x04004856 RID: 18518
		public int PreviousIndex = -1;

		// Token: 0x04004857 RID: 18519
		private ArchetypeInstanceSymbolicLinkUI m_symbolicLinkUi;
	}
}
