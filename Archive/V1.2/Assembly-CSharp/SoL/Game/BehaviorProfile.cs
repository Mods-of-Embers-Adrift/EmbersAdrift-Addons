using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using SoL.Game.Behavior;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005D1 RID: 1489
	[CreateAssetMenu(menuName = "SoL/Profiles/Behavior")]
	public class BehaviorProfile : ScriptableObject
	{
		// Token: 0x06002F65 RID: 12133 RVA: 0x00157128 File Offset: 0x00155328
		public void LoadBehaviorProfile(BehaviorTree tree)
		{
			tree.StartWhenEnabled = false;
			tree.ExternalBehavior = this.m_tree;
			if (this.m_subTrees != null)
			{
				for (int i = 0; i < this.m_subTrees.Length; i++)
				{
					this.LoadExternalTree(tree, this.m_subTrees[i]);
				}
			}
		}

		// Token: 0x06002F66 RID: 12134 RVA: 0x00157174 File Offset: 0x00155374
		private void LoadExternalTree(BehaviorTree tree, BehaviorProfile.ExternalBehaviorSubTree subTree)
		{
			if (string.IsNullOrEmpty(subTree.Name) || subTree.Behavior == null || !tree.LoadTreeToTask(subTree.Name, subTree.Behavior))
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Unable to load ExternalTree:",
					subTree.Behavior.name,
					" as ",
					subTree.Name,
					" for ",
					tree.gameObject.name,
					"!"
				}));
			}
		}

		// Token: 0x06002F67 RID: 12135 RVA: 0x00060BBC File Offset: 0x0005EDBC
		private IEnumerable GetTree()
		{
			return SolOdinUtilities.GetDropdownItems<ExternalBehaviorTree>();
		}

		// Token: 0x04002E6D RID: 11885
		[SerializeField]
		private ExternalBehaviorTree m_tree;

		// Token: 0x04002E6E RID: 11886
		[SerializeField]
		private BehaviorProfile.ExternalBehaviorSubTree[] m_subTrees;

		// Token: 0x020005D2 RID: 1490
		[Serializable]
		private class ExternalBehaviorSubTree
		{
			// Token: 0x06002F69 RID: 12137 RVA: 0x00060BBC File Offset: 0x0005EDBC
			private IEnumerable GetTree()
			{
				return SolOdinUtilities.GetDropdownItems<ExternalBehaviorTree>();
			}

			// Token: 0x04002E6F RID: 11887
			public string Name;

			// Token: 0x04002E70 RID: 11888
			public ExternalBehaviorTree Behavior;
		}
	}
}
