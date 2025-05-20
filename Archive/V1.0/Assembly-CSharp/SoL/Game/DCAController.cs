using System;
using SoL.Game.Culling;
using SoL.Game.Settings;
using SoL.Game.UMA;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UMA;
using UMA.CharacterSystem;
using UMA.PoseTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace SoL.Game
{
	// Token: 0x020005DF RID: 1503
	public class DCAController : GameEntityComponent, ICulledShadowCastingObject, IDcaSource
	{
		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x06002FB4 RID: 12212 RVA: 0x00157AC0 File Offset: 0x00155CC0
		// (remove) Token: 0x06002FB5 RID: 12213 RVA: 0x00157AF8 File Offset: 0x00155CF8
		public event Action DcaInitialized;

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06002FB6 RID: 12214 RVA: 0x00060E8A File Offset: 0x0005F08A
		// (set) Token: 0x06002FB7 RID: 12215 RVA: 0x00060E92 File Offset: 0x0005F092
		public int? Resolution { get; private set; }

		// Token: 0x06002FB8 RID: 12216 RVA: 0x00060E9B File Offset: 0x0005F09B
		public void SetResolution(int? resolution, bool update)
		{
			if (update && this.m_initialized && this.m_dca)
			{
				this.m_dca.ForceUpdate(false, true, true);
			}
			this.Resolution = resolution;
		}

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06002FB9 RID: 12217 RVA: 0x00060ECA File Offset: 0x0005F0CA
		public DynamicCharacterAvatar DCA
		{
			get
			{
				return this.m_dca;
			}
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06002FBA RID: 12218 RVA: 0x00060ED2 File Offset: 0x0005F0D2
		public bool Initialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06002FBB RID: 12219 RVA: 0x0004479C File Offset: 0x0004299C
		public bool ObjectCastsShadows
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002FBC RID: 12220 RVA: 0x00157B30 File Offset: 0x00155D30
		private void Awake()
		{
			if (this.m_dca == null)
			{
				throw new NullReferenceException("m_dca");
			}
			this.m_dca.CharacterCreated.AddListener(new UnityAction<UMAData>(this.UmaCreated));
			if (base.GameEntity != null)
			{
				base.GameEntity.DCAController = this;
				base.GameEntity.EntityVisuals = this;
			}
		}

		// Token: 0x06002FBD RID: 12221 RVA: 0x00157B98 File Offset: 0x00155D98
		private void Start()
		{
			if (base.GameEntity != null)
			{
				if (base.GameEntity.VitalsReplicator != null)
				{
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
				}
				if (base.GameEntity.CulledEntity != null)
				{
					base.GameEntity.CulledEntity.RefreshLimitFlags();
				}
			}
			UMAGlibManager.RegisterController(this);
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x00157C10 File Offset: 0x00155E10
		private void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
			}
			UMAGlibManager.UnregisterController(this);
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x00060EDA File Offset: 0x0005F0DA
		private void HealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious || obj == HealthState.Dead)
			{
				this.ToggleExpressionPlayer(false);
				this.m_preventExpressionChanges = true;
				return;
			}
			this.m_preventExpressionChanges = false;
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x00060EFA File Offset: 0x0005F0FA
		private void UmaCreated(UMAData umaData)
		{
			this.m_dca.CharacterCreated.RemoveListener(new UnityAction<UMAData>(this.UmaCreated));
			this.FinalizeUma();
		}

		// Token: 0x06002FC1 RID: 12225 RVA: 0x00157C68 File Offset: 0x00155E68
		private void FinalizeUma()
		{
			if (this.m_initialized)
			{
				return;
			}
			if (this.m_addExpressionPlayer)
			{
				this.m_expressionPlayer = this.m_dca.AddExpressionPlayer();
			}
			if (base.GameEntity != null)
			{
				if (base.GameEntity.CulledEntity != null)
				{
					base.GameEntity.CulledEntity.RefreshCulling();
				}
				bool flag = base.GameEntity.CharacterData != null;
				if (flag)
				{
					base.GameEntity.CharacterData.UpdateNameplateHeightBasedOnDna();
					base.GameEntity.CharacterData.UpdateAnimatorSpeedBasedOnDna();
				}
				if (this.m_animator != null && flag)
				{
					HumanoidReferencePoints referencePoints = this.m_animator.GetReferencePoints(this.m_dca.GetSexForUmaRace(), GlobalSettings.Values.Uma.CorrectMountPoints);
					if (base.GameEntity.NameplateHeightOffset != null)
					{
						referencePoints.Overhead.transform.SetParent(base.GameEntity.gameObject.transform);
						referencePoints.Overhead.transform.localPosition = base.GameEntity.NameplateHeightOffset.Value;
						referencePoints.Overhead.transform.localRotation = Quaternion.identity;
					}
					base.GameEntity.CharacterData.ReferencePoints = new HumanoidReferencePoints?(referencePoints);
				}
			}
			this.m_initialized = true;
			Action dcaInitialized = this.DcaInitialized;
			if (dcaInitialized != null)
			{
				dcaInitialized();
			}
			if (this.m_initialCullingData != null)
			{
				this.SetCullingDistance(this.m_initialCullingData.Value.Distance, this.m_initialCullingData.Value.Flags);
				this.m_initialCullingData = null;
			}
			if (this.m_isFirstPerson)
			{
				this.SetDcaShadowMode(ShadowCastingMode.ShadowsOnly);
			}
			this.RemoveDecalLayer();
		}

		// Token: 0x06002FC2 RID: 12226 RVA: 0x00060F1E File Offset: 0x0005F11E
		public void SetAtlasResolution(float value)
		{
			if (!this.m_initialized)
			{
				this.m_initialAtlasResolutionScale = new float?(value);
				return;
			}
			this.SetTextureScale(value);
		}

		// Token: 0x06002FC3 RID: 12227 RVA: 0x00157E2C File Offset: 0x0015602C
		public void SetCullingDistance(CullingDistance cullingDistance, CullingFlags flags)
		{
			if (!this.m_initialized)
			{
				this.m_initialCullingData = new DCAController.InitialCullingData?(new DCAController.InitialCullingData
				{
					Distance = cullingDistance,
					Flags = flags
				});
				return;
			}
			float? num = null;
			SkinQuality boneCount = SkinQuality.Bone1;
			switch (cullingDistance)
			{
			case CullingDistance.VeryNear:
				if (this.m_initialAtlasResolutionScale != null)
				{
					num = new float?(this.m_initialAtlasResolutionScale.Value);
				}
				boneCount = SkinQuality.Bone4;
				break;
			case CullingDistance.Near:
				num = new float?(0.2f);
				boneCount = SkinQuality.Bone2;
				break;
			case CullingDistance.Average:
				num = new float?(0.1f);
				break;
			default:
				num = new float?(0.1f);
				break;
			}
			bool isEnabled = cullingDistance.IsLowestBand() && !flags.HasBitFlag(CullingFlags.UmaFeatureLimit);
			this.SetBoneCount(boneCount);
			if (num != null)
			{
				this.SetTextureScale(num.Value);
			}
			this.ToggleShadows(isEnabled);
			this.ToggleExpressionPlayer(isEnabled);
			this.m_initialAtlasResolutionScale = null;
		}

		// Token: 0x06002FC4 RID: 12228 RVA: 0x00157F24 File Offset: 0x00156124
		private void SetBoneCount(SkinQuality quality)
		{
			if (!this.m_initialized || !this.m_dca.umaData)
			{
				return;
			}
			SkinnedMeshRenderer[] renderers = this.m_dca.umaData.GetRenderers();
			if (renderers != null && renderers.Length != 0 && renderers[0])
			{
				renderers[0].quality = quality;
			}
		}

		// Token: 0x06002FC5 RID: 12229 RVA: 0x00157F78 File Offset: 0x00156178
		private void SetTextureScale(float scale)
		{
			if (!this.m_initialized || !this.m_dca.umaData)
			{
				return;
			}
			if (!Mathf.Approximately(this.m_dca.umaData.atlasResolutionScale, scale))
			{
				this.m_dca.umaData.atlasResolutionScale = scale;
				this.m_dca.umaData.Dirty(false, true, false);
			}
		}

		// Token: 0x06002FC6 RID: 12230 RVA: 0x00157FDC File Offset: 0x001561DC
		public void ToggleShadows(bool isEnabled)
		{
			if (!this.m_initialized || !this.m_dca.umaData)
			{
				return;
			}
			ShadowCastingMode dcaShadowMode = isEnabled ? ShadowCastingMode.On : ShadowCastingMode.Off;
			if (this.m_isFirstPerson)
			{
				dcaShadowMode = ShadowCastingMode.ShadowsOnly;
			}
			this.SetDcaShadowMode(dcaShadowMode);
		}

		// Token: 0x06002FC7 RID: 12231 RVA: 0x00158020 File Offset: 0x00156220
		public void ToggleFirstPerson(bool isFirstPerson)
		{
			if (isFirstPerson == this.m_isFirstPerson)
			{
				return;
			}
			if (base.GameEntity && base.GameEntity.Type == GameEntityType.Player && base.GameEntity.NetworkEntity && !base.GameEntity.NetworkEntity.IsLocal)
			{
				return;
			}
			this.m_isFirstPerson = isFirstPerson;
			if (!this.m_initialized)
			{
				return;
			}
			this.SetDcaShadowMode(isFirstPerson ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
		}

		// Token: 0x06002FC8 RID: 12232 RVA: 0x00060F3C File Offset: 0x0005F13C
		private void SetDcaShadowMode(ShadowCastingMode mode)
		{
			if (this.m_dca)
			{
				this.m_dca.SetShadowMode(mode);
			}
		}

		// Token: 0x06002FC9 RID: 12233 RVA: 0x00158094 File Offset: 0x00156294
		private void ToggleExpressionPlayer(bool isEnabled)
		{
			if (!this.m_initialized || !this.m_dca.umaData || !this.m_expressionPlayer || this.m_preventExpressionChanges)
			{
				return;
			}
			this.m_expressionPlayer.enabled = isEnabled;
			this.m_expressionPlayer.enableBlinking = isEnabled;
			this.m_expressionPlayer.enableSaccades = isEnabled;
		}

		// Token: 0x06002FCA RID: 12234 RVA: 0x001580F8 File Offset: 0x001562F8
		private void RemoveDecalLayer()
		{
			if (!this.m_dca || this.m_dca.umaData == null)
			{
				return;
			}
			SkinnedMeshRenderer[] renderers = this.m_dca.umaData.GetRenderers();
			if (renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					if (renderers[i])
					{
						renderers[i].renderingLayerMask &= 4294967039U;
					}
				}
			}
		}

		// Token: 0x04002EA9 RID: 11945
		[SerializeField]
		private DynamicCharacterAvatar m_dca;

		// Token: 0x04002EAA RID: 11946
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04002EAB RID: 11947
		[SerializeField]
		private bool m_addExpressionPlayer = true;

		// Token: 0x04002EAC RID: 11948
		private UMAExpressionPlayer m_expressionPlayer;

		// Token: 0x04002EAD RID: 11949
		private bool m_initialized;

		// Token: 0x04002EAE RID: 11950
		private bool m_preventExpressionChanges;

		// Token: 0x04002EAF RID: 11951
		private bool m_isFirstPerson;

		// Token: 0x04002EB0 RID: 11952
		private DCAController.InitialCullingData? m_initialCullingData;

		// Token: 0x04002EB1 RID: 11953
		private float? m_initialAtlasResolutionScale;

		// Token: 0x020005E0 RID: 1504
		private struct InitialCullingData
		{
			// Token: 0x04002EB2 RID: 11954
			public CullingDistance Distance;

			// Token: 0x04002EB3 RID: 11955
			public CullingFlags Flags;
		}
	}
}
