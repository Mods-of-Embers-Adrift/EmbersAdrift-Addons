using System;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.VegetationSystem;
using SoL.Game.Culling;
using SoL.Game.SkyDome;
using SoL.GameCamera;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005D0 RID: 1488
	public class VegetationStudioManagerRegistration : MonoBehaviour
	{
		// Token: 0x06002F59 RID: 12121 RVA: 0x00060B4F File Offset: 0x0005ED4F
		private void Awake()
		{
			if (GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_vspManager = base.gameObject.GetComponent<VegetationStudioManager>();
			if (this.m_vspManager)
			{
				SkyDomeManager.VSPManager = this.m_vspManager;
			}
		}

		// Token: 0x06002F5A RID: 12122 RVA: 0x00156BA4 File Offset: 0x00154DA4
		private void Start()
		{
			CameraSettings.ClipPlaneDistanceChanged += this.ClipPlaneDistanceChanged;
			this.VegetationDistanceOnChanged();
			Options.VideoOptions.VegetationDistance.Changed += this.VegetationDistanceOnChanged;
			this.TreeBufferDistanceOnChanged();
			Options.VideoOptions.TreeDistance.Changed += this.TreeBufferDistanceOnChanged;
			this.TreeImposterDistanceOnChanged();
			Options.VideoOptions.TreeImposterDistance.Changed += this.TreeImposterDistanceOnChanged;
			this.UseImposterBillboardsOnChanged();
			Options.VideoOptions.UseImposterBillboards.Changed += this.UseImposterBillboardsOnChanged;
			this.VegetationDensitySettingOnChanged();
			Options.VideoOptions.VegetationDensity.Changed += this.VegetationDensitySettingOnChanged;
			this.GrassShadowsOnChanged();
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x00156C54 File Offset: 0x00154E54
		private void OnDestroy()
		{
			CameraSettings.ClipPlaneDistanceChanged -= this.ClipPlaneDistanceChanged;
			Options.VideoOptions.VegetationDistance.Changed -= this.VegetationDistanceOnChanged;
			Options.VideoOptions.TreeDistance.Changed -= this.TreeBufferDistanceOnChanged;
			Options.VideoOptions.TreeImposterDistance.Changed -= this.TreeImposterDistanceOnChanged;
			Options.VideoOptions.UseImposterBillboards.Changed -= this.UseImposterBillboardsOnChanged;
			Options.VideoOptions.VegetationDensity.Changed -= this.VegetationDensitySettingOnChanged;
		}

		// Token: 0x06002F5C RID: 12124 RVA: 0x00060B8E File Offset: 0x0005ED8E
		private void ClipPlaneDistanceChanged()
		{
			this.VegetationDistanceOnChanged();
			this.TreeBufferDistanceOnChanged();
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x00060B9C File Offset: 0x0005ED9C
		private float GetClampedDistance(float currentDistance)
		{
			if (CameraSettings.ClipPlaneDistance <= 0f)
			{
				return currentDistance;
			}
			return Mathf.Clamp(currentDistance, 0f, CameraSettings.ClipPlaneDistance);
		}

		// Token: 0x06002F5E RID: 12126 RVA: 0x00156CE0 File Offset: 0x00154EE0
		private void VegetationDistanceOnChanged()
		{
			if (!this.m_vspManager)
			{
				return;
			}
			float value = Options.VideoOptions.VegetationDistance.Value;
			foreach (VegetationSystemPro vegetationSystemPro in this.m_vspManager.VegetationSystemList)
			{
				if (this.m_originalVegetationDistance == null)
				{
					this.m_originalVegetationDistance = new float?(vegetationSystemPro.VegetationSettings.GetVegetationDistance());
				}
				int num = Mathf.FloorToInt(this.m_originalVegetationDistance.Value * value);
				vegetationSystemPro.VegetationSettings.PlantDistance = this.GetClampedDistance((float)num);
			}
			this.TreeImposterDistanceOnChanged();
		}

		// Token: 0x06002F5F RID: 12127 RVA: 0x00156D9C File Offset: 0x00154F9C
		private void TreeBufferDistanceOnChanged()
		{
			if (!this.m_vspManager)
			{
				return;
			}
			foreach (VegetationSystemPro vegetationSystemPro in this.m_vspManager.VegetationSystemList)
			{
				if (this.m_originalAdditionalBillboardDistance == null)
				{
					this.m_originalAdditionalBillboardDistance = new float?(vegetationSystemPro.VegetationSettings.AdditionalBillboardDistance);
				}
				vegetationSystemPro.VegetationSettings.AdditionalBillboardDistance = this.GetClampedDistance(this.m_originalAdditionalBillboardDistance.Value);
			}
		}

		// Token: 0x06002F60 RID: 12128 RVA: 0x00156E3C File Offset: 0x0015503C
		private void TreeImposterDistanceOnChanged()
		{
			if (!this.m_vspManager)
			{
				return;
			}
			float distance = ((TreeImposterCullingDistance)Options.VideoOptions.TreeImposterDistance.Value).GetDistance();
			float num = Options.VideoOptions.UseImposterBillboards.Value ? 10f : 0f;
			foreach (VegetationSystemPro vegetationSystemPro in this.m_vspManager.VegetationSystemList)
			{
				float plantDistance = vegetationSystemPro.VegetationSettings.PlantDistance;
				vegetationSystemPro.VegetationSettings.AdditionalTreeMeshDistance = Mathf.Clamp(distance - plantDistance + num, 0f, float.MaxValue);
			}
		}

		// Token: 0x06002F61 RID: 12129 RVA: 0x00156EF0 File Offset: 0x001550F0
		private void UseImposterBillboardsOnChanged()
		{
			if (!this.m_vspManager)
			{
				return;
			}
			bool useBillboards = !Options.VideoOptions.UseImposterBillboards.Value;
			foreach (VegetationSystemPro vegetationSystemPro in this.m_vspManager.VegetationSystemList)
			{
				foreach (VegetationPackagePro vegetationPackagePro in vegetationSystemPro.VegetationPackageProList)
				{
					foreach (VegetationItemInfoPro vegetationItemInfoPro in vegetationPackagePro.VegetationInfoList)
					{
						if (vegetationItemInfoPro.EnableRuntimeSpawn && vegetationItemInfoPro.VegetationType == VegetationType.Tree)
						{
							vegetationItemInfoPro.UseBillboards = useBillboards;
						}
					}
				}
				vegetationSystemPro.ClearCache();
			}
			this.TreeImposterDistanceOnChanged();
		}

		// Token: 0x06002F62 RID: 12130 RVA: 0x00157000 File Offset: 0x00155200
		private void VegetationDensitySettingOnChanged()
		{
			if (!this.m_vspManager)
			{
				return;
			}
			float num = (float)Options.VideoOptions.VegetationDensity.Value * 0.01f;
			foreach (VegetationSystemPro vegetationSystemPro in this.m_vspManager.VegetationSystemList)
			{
				vegetationSystemPro.VegetationSettings.GrassDensity = num;
				vegetationSystemPro.VegetationSettings.PlantDensity = num;
				vegetationSystemPro.ClearCache();
			}
		}

		// Token: 0x06002F63 RID: 12131 RVA: 0x00157090 File Offset: 0x00155290
		private void GrassShadowsOnChanged()
		{
			if (!this.m_vspManager)
			{
				return;
			}
			foreach (VegetationSystemPro vegetationSystemPro in this.m_vspManager.VegetationSystemList)
			{
				vegetationSystemPro.VegetationSettings.GrassShadows = false;
				vegetationSystemPro.VegetationSettings.PlantShadows = false;
			}
		}

		// Token: 0x04002E68 RID: 11880
		private VegetationStudioManager m_vspManager;

		// Token: 0x04002E69 RID: 11881
		private float? m_originalVegetationDistance;

		// Token: 0x04002E6A RID: 11882
		private float? m_originalAdditionalBillboardDistance;

		// Token: 0x04002E6B RID: 11883
		private const float kMinTreeBufferDistance = 100f;

		// Token: 0x04002E6C RID: 11884
		private const float kMaxTreeBufferDistance = 1000f;
	}
}
