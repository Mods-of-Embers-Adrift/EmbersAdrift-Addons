using System;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Utilities
{
	// Token: 0x020002A1 RID: 673
	public static class NavMeshAgentExtensions
	{
		// Token: 0x06001437 RID: 5175 RVA: 0x000F964C File Offset: 0x000F784C
		public static NavMeshAgent CopyNavMeshAgentTo(this NavMeshAgent agent, GameObject target)
		{
			NavMeshAgent navMeshAgent = target.AddComponent<NavMeshAgent>();
			navMeshAgent.agentTypeID = agent.agentTypeID;
			navMeshAgent.baseOffset = agent.baseOffset;
			navMeshAgent.speed = agent.speed;
			navMeshAgent.angularSpeed = agent.angularSpeed;
			navMeshAgent.acceleration = agent.acceleration;
			navMeshAgent.stoppingDistance = agent.stoppingDistance;
			navMeshAgent.autoBraking = agent.autoBraking;
			navMeshAgent.radius = agent.radius;
			navMeshAgent.height = agent.height;
			navMeshAgent.obstacleAvoidanceType = agent.obstacleAvoidanceType;
			navMeshAgent.avoidancePriority = agent.avoidancePriority;
			navMeshAgent.autoTraverseOffMeshLink = agent.autoTraverseOffMeshLink;
			navMeshAgent.autoRepath = agent.autoRepath;
			navMeshAgent.areaMask = agent.areaMask;
			UnityEngine.Object.Destroy(agent);
			return navMeshAgent;
		}
	}
}
