using System;
using BehaviorDesigner.Runtime;
using SoL.Game.Behavior;
using SoL.Game.Settings;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000812 RID: 2066
	public class NpcRotation : GameEntityComponent
	{
		// Token: 0x06003BD6 RID: 15318 RVA: 0x0017D02C File Offset: 0x0017B22C
		private static void LinkDirections(NpcRotation.Direction[] directionArray)
		{
			if (directionArray == null)
			{
				return;
			}
			for (int i = 0; i < directionArray.Length; i++)
			{
				int num = directionArray.Length - 1;
				int num2 = (i == 0) ? num : (i - 1);
				int num3 = (i == num) ? 0 : (i + 1);
				directionArray[i].Previous = directionArray[num2];
				directionArray[i].Next = directionArray[num3];
			}
			NpcRotation.m_directionsLinked = true;
		}

		// Token: 0x06003BD7 RID: 15319 RVA: 0x0017D084 File Offset: 0x0017B284
		private static NpcRotation.Direction UpdateDirectionsAndSelect(Transform thisTransform, LayerMask layerMask, NpcRotation.Direction[] directionArray, float distanceToCheck, float verticalOffset)
		{
			if (!thisTransform || directionArray == null)
			{
				return null;
			}
			Vector3 vector = thisTransform.position + Vector3.up * verticalOffset;
			Vector3 forward = thisTransform.forward;
			for (int i = 0; i < directionArray.Length; i++)
			{
				Vector3 vector2 = Quaternion.AngleAxis((float)directionArray[i].Angle, Vector3.up) * forward;
				Vector3 a = vector + vector2;
				bool flag = Physics.Raycast(new Ray(vector, a - vector), distanceToCheck, layerMask, QueryTriggerInteraction.Ignore);
				directionArray[i].Pos = vector + vector2 * distanceToCheck;
				directionArray[i].IsValid = !flag;
			}
			NpcRotation.Direction direction = null;
			for (int j = 0; j < directionArray.Length; j++)
			{
				if (directionArray[j].AllAreValid && (direction == null || direction.AbsAngle > directionArray[j].AbsAngle))
				{
					direction = directionArray[j];
				}
			}
			if (direction == null)
			{
				for (int k = 0; k < directionArray.Length; k++)
				{
					if (directionArray[k].IsValid && (direction == null || direction.AbsAngle > directionArray[k].AbsAngle))
					{
						direction = directionArray[k];
					}
				}
			}
			return direction;
		}

		// Token: 0x06003BD8 RID: 15320 RVA: 0x0017D1A4 File Offset: 0x0017B3A4
		private void Start()
		{
			if (!this.m_tree || !this.m_npcTargetController || !GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
			this.m_destinationReachTime = this.m_tree.GetSharedVariable("DestinationReachTime");
			if (!NpcRotation.m_directionsLinked)
			{
				NpcRotation.LinkDirections(NpcRotation.m_directions);
			}
			this.m_layerMask = ~GlobalSettings.Values.General.LineOfSightExclusionLayerMask;
		}

		// Token: 0x04003A82 RID: 14978
		private static bool m_directionsLinked = false;

		// Token: 0x04003A83 RID: 14979
		private static readonly NpcRotation.Direction[] m_directions = new NpcRotation.Direction[]
		{
			new NpcRotation.Direction(0),
			new NpcRotation.Direction(45),
			new NpcRotation.Direction(90),
			new NpcRotation.Direction(135),
			new NpcRotation.Direction(180),
			new NpcRotation.Direction(-135),
			new NpcRotation.Direction(-90),
			new NpcRotation.Direction(-45)
		};

		// Token: 0x04003A84 RID: 14980
		[SerializeField]
		private BehaviorTree m_tree;

		// Token: 0x04003A85 RID: 14981
		[SerializeField]
		private NpcTargetController m_npcTargetController;

		// Token: 0x04003A86 RID: 14982
		[SerializeField]
		private ServerNpcController m_serverNpcController;

		// Token: 0x04003A87 RID: 14983
		[SerializeField]
		private float m_verticalOffset = 1f;

		// Token: 0x04003A88 RID: 14984
		[SerializeField]
		private float m_distanceToCheck = 5f;

		// Token: 0x04003A89 RID: 14985
		private LayerMask m_layerMask;

		// Token: 0x04003A8A RID: 14986
		private SharedFloat m_destinationReachTime;

		// Token: 0x04003A8B RID: 14987
		private Quaternion? m_targetRotation;

		// Token: 0x02000813 RID: 2067
		private class Direction
		{
			// Token: 0x17000DC3 RID: 3523
			// (get) Token: 0x06003BDB RID: 15323 RVA: 0x000687CE File Offset: 0x000669CE
			private bool PrevIsValid
			{
				get
				{
					return this.Previous != null && this.Previous.IsValid;
				}
			}

			// Token: 0x17000DC4 RID: 3524
			// (get) Token: 0x06003BDC RID: 15324 RVA: 0x000687E5 File Offset: 0x000669E5
			private bool NextIsValid
			{
				get
				{
					return this.Next != null && this.Next.IsValid;
				}
			}

			// Token: 0x17000DC5 RID: 3525
			// (get) Token: 0x06003BDD RID: 15325 RVA: 0x000687FC File Offset: 0x000669FC
			public bool AllAreValid
			{
				get
				{
					return this.IsValid && this.PrevIsValid && this.NextIsValid;
				}
			}

			// Token: 0x06003BDE RID: 15326 RVA: 0x00068816 File Offset: 0x00066A16
			public Direction(int angle)
			{
				this.Angle = angle;
				this.AbsAngle = Mathf.Abs(angle);
			}

			// Token: 0x04003A8C RID: 14988
			public readonly int Angle;

			// Token: 0x04003A8D RID: 14989
			public readonly int AbsAngle;

			// Token: 0x04003A8E RID: 14990
			public Vector3 Pos;

			// Token: 0x04003A8F RID: 14991
			public bool IsValid;

			// Token: 0x04003A90 RID: 14992
			public NpcRotation.Direction Previous;

			// Token: 0x04003A91 RID: 14993
			public NpcRotation.Direction Next;
		}
	}
}
