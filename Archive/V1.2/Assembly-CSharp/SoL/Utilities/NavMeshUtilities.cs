using System;
using SoL.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Utilities
{
	// Token: 0x020002A2 RID: 674
	public static class NavMeshUtilities
	{
		// Token: 0x06001438 RID: 5176 RVA: 0x00050302 File Offset: 0x0004E502
		public static bool SamplePosition(Vector3 samplePosition, out NavMeshHit navHit, float sampleDistance, int areaMask = -1)
		{
			return NavMesh.SamplePosition(samplePosition, out navHit, sampleDistance, areaMask);
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x000F9710 File Offset: 0x000F7910
		public static void InitializePathfindingIterationsPerFrame()
		{
			if (ServerGameManager.GameServerConfig != null && ServerGameManager.GameServerConfig.PathfindingIterationsPerFrame > 0)
			{
				NavMeshUtilities.SetPathfindingIterationsPerFrame(ServerGameManager.GameServerConfig.PathfindingIterationsPerFrame);
			}
			Debug.Log("Initialized PathfindingIterationsPerFrame: " + NavMesh.pathfindingIterationsPerFrame.ToString());
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x000F975C File Offset: 0x000F795C
		public static void SetPathfindingIterationsPerFrame(int iterations)
		{
			NavMesh.pathfindingIterationsPerFrame = Mathf.Clamp(iterations, 100, 2000);
			Debug.Log("Set PathfindingIterationsPerFrame: " + NavMesh.pathfindingIterationsPerFrame.ToString());
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x0005030D File Offset: 0x0004E50D
		public static int GetPathfindingIterationsPerFrame()
		{
			return NavMesh.pathfindingIterationsPerFrame;
		}

		// Token: 0x04001C91 RID: 7313
		private const int kDefaultPathfindingIterationsPerFrame = 100;

		// Token: 0x04001C92 RID: 7314
		private const int kMaxPathfindingIterationsPerFrame = 2000;
	}
}
