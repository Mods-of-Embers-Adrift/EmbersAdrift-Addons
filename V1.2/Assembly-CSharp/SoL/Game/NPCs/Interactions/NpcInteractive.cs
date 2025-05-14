using System;
using System.Collections.Generic;
using Drawing;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs.Interactions
{
	// Token: 0x02000839 RID: 2105
	public abstract class NpcInteractive : MonoBehaviourGizmos
	{
		// Token: 0x17000E02 RID: 3586
		// (get) Token: 0x06003CDD RID: 15581 RVA: 0x000693F3 File Offset: 0x000675F3
		private static Dictionary<Collider, NpcInteractive> NpcInteractColliders
		{
			get
			{
				if (NpcInteractive.m_npcInteractColliders == null)
				{
					NpcInteractive.m_npcInteractColliders = new Dictionary<Collider, NpcInteractive>();
				}
				return NpcInteractive.m_npcInteractColliders;
			}
		}

		// Token: 0x06003CDE RID: 15582 RVA: 0x0006940B File Offset: 0x0006760B
		public static bool TryGetNpcInteractiveForCollider(Collider collider, out NpcInteractive interactive)
		{
			if (!collider)
			{
				interactive = null;
				return false;
			}
			return NpcInteractive.NpcInteractColliders.TryGetValue(collider, out interactive);
		}

		// Token: 0x17000E03 RID: 3587
		// (get) Token: 0x06003CDF RID: 15583
		public abstract bool ResetInitialPosition { get; }

		// Token: 0x17000E04 RID: 3588
		// (get) Token: 0x06003CE0 RID: 15584
		public abstract bool UseLightItemAtNight { get; }

		// Token: 0x06003CE1 RID: 15585
		public abstract bool EntityCanInteract(GameEntity entity);

		// Token: 0x06003CE2 RID: 15586
		public abstract NpcInteractive GetNextInteractive(GameEntity entity);

		// Token: 0x06003CE3 RID: 15587
		public abstract void RegisterOccupant(GameEntity entity);

		// Token: 0x06003CE4 RID: 15588
		public abstract void DeregisterOccupant(GameEntity entity);

		// Token: 0x17000E05 RID: 3589
		// (get) Token: 0x06003CE5 RID: 15589 RVA: 0x00069426 File Offset: 0x00067626
		public NpcInteractiveWaitParameters WaitParameters
		{
			get
			{
				return this.m_waitParameters;
			}
		}

		// Token: 0x06003CE6 RID: 15590 RVA: 0x0006942E File Offset: 0x0006762E
		private void OnEnable()
		{
			if (Application.isPlaying && this.m_collider && !this.m_colliderAdded)
			{
				NpcInteractive.NpcInteractColliders.Add(this.m_collider, this);
				this.m_colliderAdded = true;
			}
		}

		// Token: 0x06003CE7 RID: 15591 RVA: 0x00069464 File Offset: 0x00067664
		private void OnDisable()
		{
			if (Application.isPlaying && this.m_collider && this.m_colliderAdded)
			{
				NpcInteractive.NpcInteractColliders.Remove(this.m_collider);
				this.m_colliderAdded = false;
			}
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x001813C4 File Offset: 0x0017F5C4
		protected void SetupCollider()
		{
			if (this.m_collider == null)
			{
				SphereCollider sphereCollider = base.gameObject.GetComponent<SphereCollider>();
				if (sphereCollider == null)
				{
					sphereCollider = base.gameObject.AddComponent<SphereCollider>();
				}
				this.m_collider = sphereCollider;
				this.m_collider.radius = 0.25f;
				if (Application.isPlaying && !this.m_colliderAdded)
				{
					NpcInteractive.NpcInteractColliders.Add(this.m_collider, this);
					this.m_colliderAdded = true;
				}
			}
			base.gameObject.layer = LayerMap.Interaction.Layer;
		}

		// Token: 0x04003BB2 RID: 15282
		private static Dictionary<Collider, NpcInteractive> m_npcInteractColliders;

		// Token: 0x04003BB3 RID: 15283
		[SerializeField]
		private SphereCollider m_collider;

		// Token: 0x04003BB4 RID: 15284
		[SerializeField]
		private NpcInteractiveWaitParameters m_waitParameters;

		// Token: 0x04003BB5 RID: 15285
		private bool m_colliderAdded;
	}
}
