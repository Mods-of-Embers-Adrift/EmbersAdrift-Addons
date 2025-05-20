using System;
using SoL.Game.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.Targeting
{
	// Token: 0x0200065B RID: 1627
	public class TargetReticle : MonoBehaviour
	{
		// Token: 0x060032A0 RID: 12960 RVA: 0x00062DE8 File Offset: 0x00060FE8
		public void Init(Color color)
		{
			this.m_renderer.SetMainColor(color);
		}

		// Token: 0x060032A1 RID: 12961 RVA: 0x00062DF6 File Offset: 0x00060FF6
		private void Start()
		{
			this.m_myTransform = base.gameObject.transform;
			if (this.m_renderer)
			{
				this.m_renderer.enabled = false;
			}
		}

		// Token: 0x060032A2 RID: 12962 RVA: 0x0016108C File Offset: 0x0015F28C
		private void Update()
		{
			if (!this.m_decal || !this.m_pulseColor)
			{
				return;
			}
			float t = Mathf.PingPong(Time.time * this.m_pulseSpeed, 1f);
			float value = Mathf.Lerp(this.m_pulseIntensityRange.Min, this.m_pulseIntensityRange.Max, t);
			this.m_sourceMaterial.SetEmissionIntensity(value);
			if (this.m_renderer.gameObject.activeInHierarchy)
			{
				this.m_sourceQuadMaterial.SetEmissionIntensity(value);
			}
		}

		// Token: 0x060032A3 RID: 12963 RVA: 0x00161110 File Offset: 0x0015F310
		public void Init(TargetType targetType)
		{
			if (!this.m_decal)
			{
				return;
			}
			this.m_sourceMaterial = ((targetType == TargetType.Offensive) ? this.m_offensiveMaterial : this.m_defensiveMaterial);
			this.m_decal.material = this.m_sourceMaterial;
			float num = (targetType == TargetType.Offensive) ? this.m_offensiveSize : this.m_defensiveSize;
			this.m_defaultSize = new Vector3(num, num, this.m_decal.size.z);
			this.m_decal.size = this.m_defaultSize;
			this.m_initialIntensity = new float?(this.m_sourceMaterial.GetEmissionIntensity());
			if (this.m_renderer)
			{
				this.m_sourceQuadMaterial = ((targetType == TargetType.Offensive) ? this.m_offensiveQuadMaterial : this.m_defensiveQuadMaterial);
				this.m_renderer.material = this.m_sourceQuadMaterial;
				this.m_renderer.gameObject.transform.localScale = Vector3.one * num;
			}
		}

		// Token: 0x060032A4 RID: 12964 RVA: 0x00161200 File Offset: 0x0015F400
		public void UpdateSize(ITargetable targetable)
		{
			if (targetable != null && this.m_decal)
			{
				float num = (targetable.ReticleRadiusOverride != null) ? (targetable.ReticleRadiusOverride.Value * 2f) : this.m_defaultSize.x;
				if (targetable.Entity && targetable.Entity.CharacterData && targetable.Entity.CharacterData.TransformScale != null)
				{
					num *= 1.PercentModification(targetable.Entity.CharacterData.TransformScale.Value);
				}
				Vector3 vector = new Vector3(Mathf.Clamp(num, 0.2f, 20f), Mathf.Clamp(num, 0.2f, 20f), this.m_defaultSize.z);
				this.m_decal.size = vector;
				if (this.m_renderer)
				{
					this.m_renderer.gameObject.transform.localScale = Vector3.one * vector.x;
				}
			}
		}

		// Token: 0x060032A5 RID: 12965 RVA: 0x00161320 File Offset: 0x0015F520
		public void UpdatePosition(ITargetable targetable)
		{
			if (!this.m_myTransform)
			{
				return;
			}
			Vector3 vector = TargetReticle.m_outOfTheWayPos;
			bool flag = false;
			if (targetable != null && targetable.Entity)
			{
				vector = targetable.Entity.gameObject.transform.position;
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, Vector3.up, out raycastHit, 8f, LayerMap.CameraCollidePlayerIgnore.LayerMask, QueryTriggerInteraction.Ignore) && raycastHit.collider && raycastHit.collider.gameObject && raycastHit.collider.gameObject.CompareTag("Water"))
				{
					vector.y = raycastHit.point.y;
					flag = true;
				}
				vector += TargetReticle.m_reticleOffset;
			}
			this.m_myTransform.position = vector;
			if (this.m_renderer && this.m_renderer.enabled != flag)
			{
				this.m_renderer.enabled = flag;
			}
		}

		// Token: 0x040030FC RID: 12540
		private static readonly Vector3 m_reticleOffset = Vector3.one * 0.01f;

		// Token: 0x040030FD RID: 12541
		private static readonly Vector3 m_outOfTheWayPos = new Vector3(0f, -1000f, 0f);

		// Token: 0x040030FE RID: 12542
		[SerializeField]
		private MeshRenderer m_renderer;

		// Token: 0x040030FF RID: 12543
		[SerializeField]
		private Material m_offensiveQuadMaterial;

		// Token: 0x04003100 RID: 12544
		[SerializeField]
		private Material m_defensiveQuadMaterial;

		// Token: 0x04003101 RID: 12545
		private const string kDecalGroupName = "Decals";

		// Token: 0x04003102 RID: 12546
		private const string kQuadGroupName = "Quads";

		// Token: 0x04003103 RID: 12547
		private const float kMinSize = 0.2f;

		// Token: 0x04003104 RID: 12548
		private const float kMaxSize = 20f;

		// Token: 0x04003105 RID: 12549
		[SerializeField]
		private DecalProjector m_decal;

		// Token: 0x04003106 RID: 12550
		[SerializeField]
		private Material m_offensiveMaterial;

		// Token: 0x04003107 RID: 12551
		[SerializeField]
		private float m_offensiveSize = 1.5f;

		// Token: 0x04003108 RID: 12552
		[SerializeField]
		private Material m_defensiveMaterial;

		// Token: 0x04003109 RID: 12553
		[SerializeField]
		private float m_defensiveSize = 1.15f;

		// Token: 0x0400310A RID: 12554
		[SerializeField]
		private bool m_pulseColor;

		// Token: 0x0400310B RID: 12555
		[SerializeField]
		private MinMaxFloatRange m_pulseIntensityRange = new MinMaxFloatRange(0f, 0f);

		// Token: 0x0400310C RID: 12556
		[SerializeField]
		private float m_pulseSpeed = 2f;

		// Token: 0x0400310D RID: 12557
		private Transform m_myTransform;

		// Token: 0x0400310E RID: 12558
		private Material m_sourceMaterial;

		// Token: 0x0400310F RID: 12559
		private Material m_sourceQuadMaterial;

		// Token: 0x04003110 RID: 12560
		private float? m_initialIntensity;

		// Token: 0x04003111 RID: 12561
		private Vector3 m_defaultSize = Vector3.one;
	}
}
