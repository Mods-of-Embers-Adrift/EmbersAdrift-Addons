using System;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000669 RID: 1641
	public class AshenController : GameEntityComponent
	{
		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06003302 RID: 13058 RVA: 0x000631F8 File Offset: 0x000613F8
		// (set) Token: 0x06003303 RID: 13059 RVA: 0x00063200 File Offset: 0x00061400
		public MinMaxFloatRange ChanceToAshen
		{
			get
			{
				return this.m_chanceToAshen;
			}
			set
			{
				this.m_chanceToAshen = value;
			}
		}

		// Token: 0x06003304 RID: 13060 RVA: 0x00161D44 File Offset: 0x0015FF44
		private void Awake()
		{
			if (this.m_collider == null)
			{
				base.enabled = false;
				return;
			}
			if (!GameManager.IsServer)
			{
				this.m_collider.enabled = false;
				base.enabled = false;
				return;
			}
			if (base.GameEntity == null)
			{
				base.enabled = false;
				return;
			}
			base.GameEntity.AshenController = this;
			this.m_collider.isTrigger = true;
			this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
			SphereCollider sphereCollider = this.m_collider as SphereCollider;
			if (sphereCollider != null)
			{
				float radius = sphereCollider.radius;
				this.m_maxDistanceSqr = radius * radius;
			}
		}

		// Token: 0x06003305 RID: 13061 RVA: 0x00161DF0 File Offset: 0x0015FFF0
		private void OnTriggerEnter(Collider other)
		{
			GameEntity gameEntity;
			InteractiveNpc interactiveNpc;
			if (GameManager.IsServer && DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Npc && gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveNpc))
			{
				interactiveNpc.AshenController = this;
			}
		}

		// Token: 0x06003306 RID: 13062 RVA: 0x00161E38 File Offset: 0x00160038
		private void OnTriggerExit(Collider other)
		{
			GameEntity gameEntity;
			InteractiveNpc interactiveNpc;
			if (GameManager.IsServer && DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Npc && gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveNpc) && interactiveNpc.AshenController == this)
			{
				interactiveNpc.AshenController = null;
			}
		}

		// Token: 0x06003307 RID: 13063 RVA: 0x00161E8C File Offset: 0x0016008C
		public bool ShouldAshen(Vector3 position)
		{
			float num;
			if (this.m_maxDistanceSqr > 0f)
			{
				float t = (base.gameObject.transform.position - position).sqrMagnitude / this.m_maxDistanceSqr;
				num = Mathf.Lerp(this.m_chanceToAshen.Max, this.m_chanceToAshen.Min, t);
			}
			else
			{
				num = this.m_chanceToAshen.RandomWithinRange();
			}
			return UnityEngine.Random.Range(0f, 1f) < num;
		}

		// Token: 0x04003144 RID: 12612
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04003145 RID: 12613
		[SerializeField]
		private MinMaxFloatRange m_chanceToAshen = new MinMaxFloatRange(0.4f, 0.6f);

		// Token: 0x04003146 RID: 12614
		[NonSerialized]
		private float m_maxDistanceSqr = -1f;
	}
}
