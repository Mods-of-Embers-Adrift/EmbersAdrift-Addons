using System;
using BehaviorDesigner.Runtime;
using SoL.Game.Behavior;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006E4 RID: 1764
	public static class BehaviorTreeExtensions
	{
		// Token: 0x0600354D RID: 13645 RVA: 0x0006476D File Offset: 0x0006296D
		private static string GetNodeName(this BehaviorTreeNodeName nodeName)
		{
			switch (nodeName)
			{
			case BehaviorTreeNodeName.Idle:
				return "Idle";
			case BehaviorTreeNodeName.Search:
				return "Search";
			case BehaviorTreeNodeName.Neutral:
				return "Neutral";
			case BehaviorTreeNodeName.Combat:
				return "Combat";
			default:
				throw new ArgumentException("nodeName");
			}
		}

		// Token: 0x0600354E RID: 13646 RVA: 0x00166C60 File Offset: 0x00164E60
		public static void LoadExternalSubTree(this BehaviorTree tree, BehaviorTreeNodeName nodeName, ExternalBehaviorTree subTree)
		{
			if (nodeName == BehaviorTreeNodeName.None)
			{
				return;
			}
			if (tree == null)
			{
				throw new ArgumentNullException("tree");
			}
			if (subTree == null)
			{
				throw new ArgumentNullException("subTree");
			}
			if (!tree.LoadTreeToTask(nodeName.GetNodeName(), subTree))
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Unable to load ExternalTree:",
					subTree.name,
					" as ",
					nodeName.GetNodeName(),
					" for ",
					tree.gameObject.name,
					"!"
				}));
			}
		}
	}
}
