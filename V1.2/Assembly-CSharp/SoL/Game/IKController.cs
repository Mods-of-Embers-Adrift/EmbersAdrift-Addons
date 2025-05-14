using System;
using RootMotion.FinalIK;
using SoL.Game.EffectSystem;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200058D RID: 1421
	public class IKController : GameEntityComponent
	{
		// Token: 0x06002C55 RID: 11349 RVA: 0x00148D34 File Offset: 0x00146F34
		public static GameObject CreateHandIk(string objName, Transform parent)
		{
			GameObject gameObject = new GameObject(objName);
			gameObject.transform.SetParent(parent);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			return gameObject;
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06002C56 RID: 11350 RVA: 0x0005EC92 File Offset: 0x0005CE92
		public IK MainIK
		{
			get
			{
				return this.m_main;
			}
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06002C57 RID: 11351 RVA: 0x0005EC9A File Offset: 0x0005CE9A
		// (set) Token: 0x06002C58 RID: 11352 RVA: 0x0005ECA2 File Offset: 0x0005CEA2
		public bool IsOverWater { get; set; }

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06002C59 RID: 11353 RVA: 0x0005ECAB File Offset: 0x0005CEAB
		private bool m_showMaxMain
		{
			get
			{
				return this.m_main != null;
			}
		}

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06002C5A RID: 11354 RVA: 0x0005ECB9 File Offset: 0x0005CEB9
		private bool m_showMaxLook
		{
			get
			{
				return this.m_look != null;
			}
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x06002C5B RID: 11355 RVA: 0x0005ECC7 File Offset: 0x0005CEC7
		private bool m_showMaxGrounder
		{
			get
			{
				return this.m_grounder != null;
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x06002C5C RID: 11356 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool HasIkAssignedOnAwake
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x06002C5D RID: 11357 RVA: 0x0005ECD5 File Offset: 0x0005CED5
		protected LayerMask GroundLayers
		{
			get
			{
				if (!this.m_overrideGroundLayers)
				{
					return GlobalSettings.Values.Ik.GroundLayers;
				}
				return this.m_grounderLayers;
			}
		}

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x06002C5E RID: 11358 RVA: 0x0005ECF5 File Offset: 0x0005CEF5
		public Transform LookAtMe
		{
			get
			{
				return this.m_otherLookAtTarget;
			}
		}

		// Token: 0x06002C5F RID: 11359 RVA: 0x00148D84 File Offset: 0x00146F84
		private void Awake()
		{
			if (this.HasIkAssignedOnAwake && this.m_main == null && this.m_look == null && this.m_grounder == null && this.m_matchGroundAngle == null)
			{
				base.enabled = false;
				return;
			}
			if (base.GameEntity != null)
			{
				base.GameEntity.IKController = this;
				this.m_otherLookAtTarget = base.GameEntity.gameObject.transform;
			}
			if (this.m_grounder != null)
			{
				if (this.m_grounder is GrounderQuadruped)
				{
					this.m_grounderQuad = (GrounderQuadruped)this.m_grounder;
					this.CheckGrounderSetup(this.m_grounderQuad.characterRoot);
				}
				else if (this.m_grounder is GrounderIK)
				{
					this.m_grounderIk = (GrounderIK)this.m_grounder;
					this.CheckGrounderSetup(this.m_grounderIk.characterRoot);
				}
			}
			this.SetupGrounder();
			this.SetupLookIK();
			this.SetupIKData();
		}

		// Token: 0x06002C60 RID: 11360 RVA: 0x00148E8C File Offset: 0x0014708C
		private void CheckGrounderSetup(Transform characterRoot)
		{
			if (characterRoot == null)
			{
				Debug.LogWarning("No Character Root ON " + base.gameObject.transform.GetPath() + "!");
				return;
			}
			if (characterRoot.localRotation != Quaternion.identity)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Character Root for ",
					base.gameObject.transform.GetPath(),
					" is NOT ROTATED TO ZERO! (",
					characterRoot.localRotation.eulerAngles.ToString(),
					")"
				}));
			}
		}

		// Token: 0x06002C61 RID: 11361 RVA: 0x00148F34 File Offset: 0x00147134
		protected virtual void Start()
		{
			if (base.GameEntity != null)
			{
				if (base.GameEntity.VitalsReplicator != null)
				{
					this.HealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState.Value);
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
					this.BehaviorFlagsOnChanged(base.GameEntity.VitalsReplicator.BehaviorFlags.Value);
					base.GameEntity.VitalsReplicator.BehaviorFlags.Changed += this.BehaviorFlagsOnChanged;
				}
				if (base.GameEntity.CulledEntity != null)
				{
					base.GameEntity.CulledEntity.RefreshLimitFlags();
				}
			}
			this.m_hasTargetController = (base.GameEntity != null && base.GameEntity.TargetController != null);
		}

		// Token: 0x06002C62 RID: 11362 RVA: 0x00149028 File Offset: 0x00147228
		protected virtual void LateUpdate()
		{
			if (this.m_preventChanges)
			{
				return;
			}
			float num = this.IsOverWater ? 0f : 1f;
			if (this.m_main)
			{
				float num2 = this.m_ikData.MainWeight * this.m_targetWeightFraction * num;
				if (this.m_mainMaxWeight < 1f)
				{
					num2 = num2.Remap(0f, 1f, 0f, this.m_mainMaxWeight);
				}
				if (num2 <= 0f && this.m_ikData.MainSolver.IKPositionWeight <= 0f)
				{
					if (this.m_main.enabled)
					{
						this.m_main.enabled = false;
					}
				}
				else if (num2 > 0f && !this.m_main.enabled)
				{
					this.m_main.enabled = true;
				}
				if (this.m_main.enabled)
				{
					if (this.m_ikData.MainSolver.IKPositionWeight != num2)
					{
						this.m_ikData.MainSolver.IKPositionWeight = Mathf.MoveTowards(this.m_ikData.MainSolver.IKPositionWeight, num2, Time.deltaTime * 2f);
					}
					if (this.m_fbbik && base.GameEntity && base.GameEntity.HandheldMountController)
					{
						base.GameEntity.HandheldMountController.UpdateIkTargets(this.m_fbbik, this.m_targetWeightFraction);
					}
				}
			}
			if (this.m_look)
			{
				float num3 = this.m_ikData.LookWeight * this.m_targetWeightFraction * this.m_lookBehaviorWeightFraction;
				if (this.m_lookMaxWeight < 1f)
				{
					num3 = num3.Remap(0f, 1f, 0f, this.m_lookMaxWeight);
				}
				if (num3 <= 0f && this.m_ikData.LookSolver.IKPositionWeight <= 0f)
				{
					if (this.m_look.enabled)
					{
						this.m_look.enabled = false;
					}
				}
				else if (num3 > 0f && !this.m_look.enabled)
				{
					this.m_look.enabled = true;
				}
				if (this.m_look.enabled)
				{
					if (num3 <= 0f)
					{
						this.m_look.solver.IKPositionWeight = Mathf.MoveTowards(this.m_look.solver.IKPositionWeight, num3, Time.deltaTime * 2f);
					}
					else if (this.m_lookAtTarget)
					{
						this.UpdateLookTarget();
					}
				}
			}
			if (this.m_grounder)
			{
				float num4 = this.m_ikData.GrounderWeight * this.m_targetWeightFraction * num;
				if (this.m_grounderMaxWeight < 1f)
				{
					num4 = num4.Remap(0f, 1f, 0f, this.m_grounderMaxWeight);
				}
				if (num4 <= 0f && this.m_grounder.weight <= 0f)
				{
					if (this.m_grounder.enabled)
					{
						this.ToggleGrounder(false);
					}
				}
				else if (num4 > 0f && !this.m_grounder.enabled)
				{
					this.ToggleGrounder(true);
				}
				if (this.m_grounder.enabled && this.m_grounder.weight != num4)
				{
					this.m_grounder.weight = Mathf.MoveTowards(this.m_grounder.weight, num4, Time.deltaTime * 2f);
				}
			}
			if (this.m_matchGroundAngle)
			{
				float num5 = this.m_targetWeightFraction * num;
				if (this.m_matchGroundAngle.enabled && num5 <= 0f)
				{
					this.m_matchGroundAngle.ResetRotation();
					this.m_matchGroundAngle.enabled = false;
				}
				else if (!this.m_matchGroundAngle.enabled && num5 > 0f)
				{
					this.m_matchGroundAngle.enabled = true;
				}
			}
			if (this.m_matchGroundHeight)
			{
				float num6 = this.m_targetWeightFraction * num;
				if (this.m_matchGroundHeight.enabled && num6 <= 0f)
				{
					this.m_matchGroundHeight.ResetHeight();
					this.m_matchGroundHeight.enabled = false;
					return;
				}
				if (!this.m_matchGroundHeight.enabled && num6 > 0f)
				{
					this.m_matchGroundHeight.enabled = true;
				}
			}
		}

		// Token: 0x06002C63 RID: 11363 RVA: 0x0014944C File Offset: 0x0014764C
		private void OnDestroy()
		{
			if (this.m_lookAtTarget != null)
			{
				UnityEngine.Object.Destroy(this.m_lookAtTarget);
			}
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
				base.GameEntity.VitalsReplicator.BehaviorFlags.Changed -= this.BehaviorFlagsOnChanged;
			}
		}

		// Token: 0x06002C64 RID: 11364 RVA: 0x001494D8 File Offset: 0x001476D8
		protected void SetupIKData()
		{
			this.m_ikData = default(IKController.IKData);
			if (this.m_main)
			{
				this.m_ikData.MainSolver = this.m_main.GetIKSolver();
				this.m_ikData.MainWeight = this.m_ikData.MainSolver.IKPositionWeight;
			}
			if (this.m_look)
			{
				this.m_ikData.LookSolver = this.m_look.solver;
				this.m_ikData.LookWeight = this.m_ikData.LookSolver.IKPositionWeight;
			}
			if (this.m_grounder != null)
			{
				this.m_ikData.GrounderWeight = this.m_grounder.weight;
			}
		}

		// Token: 0x06002C65 RID: 11365 RVA: 0x00149594 File Offset: 0x00147794
		private void ToggleGrounder(bool isEnabled)
		{
			this.m_grounder.enabled = isEnabled;
			if (this.m_grounderQuad)
			{
				this.ToggleLegs(isEnabled, this.m_grounderQuad.legs);
				this.ToggleLegs(isEnabled, this.m_grounderQuad.forelegs);
				if (!isEnabled)
				{
					this.ResetRotation(this.m_grounderQuad.characterRoot);
					return;
				}
			}
			else if (this.m_grounderIk)
			{
				this.ToggleLegs(isEnabled, this.m_grounderIk.legs);
				if (!isEnabled)
				{
					this.ResetRotation(this.m_grounderIk.characterRoot);
				}
			}
		}

		// Token: 0x06002C66 RID: 11366 RVA: 0x00149628 File Offset: 0x00147828
		private void ToggleLegs(bool isEnabled, IK[] legs)
		{
			if (legs != null)
			{
				for (int i = 0; i < legs.Length; i++)
				{
					if (legs[i])
					{
						legs[i].enabled = isEnabled;
					}
				}
			}
		}

		// Token: 0x06002C67 RID: 11367 RVA: 0x0005ECFD File Offset: 0x0005CEFD
		private void ResetRotation(Transform characterRoot)
		{
			if (characterRoot)
			{
				characterRoot.localRotation = Quaternion.identity;
			}
		}

		// Token: 0x06002C68 RID: 11368 RVA: 0x0014965C File Offset: 0x0014785C
		public void RefreshCullee(bool isCulled, float sqrMagDistance)
		{
			this.m_enabled = ((base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal) || !isCulled);
			this.m_sqrMagDistance = sqrMagDistance;
			this.m_targetWeightFraction = (this.m_enabled ? 1f : 0f);
			this.RefreshIKState();
		}

		// Token: 0x06002C69 RID: 11369 RVA: 0x0005ED12 File Offset: 0x0005CF12
		private void RefreshIKState()
		{
			if (!this.m_preventChanges)
			{
				this.SetGroundingQuality(this.GetGroundingQuality(this.m_sqrMagDistance));
			}
		}

		// Token: 0x06002C6A RID: 11370 RVA: 0x0005ED2E File Offset: 0x0005CF2E
		protected void RefreshCullingDistance()
		{
			this.SetGroundingQuality(this.GetGroundingQuality(this.m_sqrMagDistance));
		}

		// Token: 0x06002C6B RID: 11371 RVA: 0x001496D0 File Offset: 0x001478D0
		private void SetGroundingQuality(Grounding.Quality quality)
		{
			if (this.m_grounder)
			{
				this.m_grounder.solver.quality = quality;
				if (this.m_grounderQuad)
				{
					this.m_grounderQuad.solver.quality = quality;
					this.m_grounderQuad.forelegSolver.quality = quality;
				}
			}
		}

		// Token: 0x06002C6C RID: 11372 RVA: 0x0014972C File Offset: 0x0014792C
		private Grounding.Quality GetGroundingQuality(float sqrMagDistance)
		{
			Grounding.Quality result = Grounding.Quality.Fastest;
			if (base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal)
			{
				result = Grounding.Quality.Best;
			}
			else if (sqrMagDistance <= 100f)
			{
				result = Grounding.Quality.Best;
			}
			else if (sqrMagDistance <= 400f)
			{
				result = Grounding.Quality.Simple;
			}
			return result;
		}

		// Token: 0x06002C6D RID: 11373 RVA: 0x00149788 File Offset: 0x00147988
		private void HealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious || obj == HealthState.Dead)
			{
				this.m_preventChanges = true;
				if (this.m_main)
				{
					this.m_main.enabled = false;
				}
				if (this.m_look)
				{
					this.m_look.enabled = false;
				}
				if (this.m_grounder)
				{
					this.ToggleGrounder(false);
					return;
				}
			}
			else
			{
				this.m_preventChanges = false;
				this.RefreshIKState();
			}
		}

		// Token: 0x06002C6E RID: 11374 RVA: 0x0005ED42 File Offset: 0x0005CF42
		private void BehaviorFlagsOnChanged(BehaviorEffectTypeFlags obj)
		{
			this.m_lookBehaviorWeightFraction = (obj.HasBitFlag(BehaviorEffectTypeFlags.Stunned) ? 0f : 1f);
		}

		// Token: 0x06002C6F RID: 11375 RVA: 0x001497F8 File Offset: 0x001479F8
		private void SetupGrounder()
		{
			if (this.m_grounderQuad != null)
			{
				this.m_grounderQuad.forelegSolver.layers = this.GroundLayers;
			}
			if (this.m_grounder != null)
			{
				this.m_grounder.solver.layers = this.GroundLayers;
			}
			if (this.m_matchGroundAngle != null)
			{
				this.m_matchGroundAngle.layers = this.GroundLayers;
			}
			if (this.m_matchGroundHeight != null)
			{
				this.m_matchGroundHeight.layers = this.GroundLayers;
			}
		}

		// Token: 0x06002C70 RID: 11376 RVA: 0x0014988C File Offset: 0x00147A8C
		private void SetupLookIK()
		{
			if (this.m_look != null)
			{
				this.m_lookSource = this.m_look.solver.head.transform;
				this.m_otherLookAtTarget = this.m_lookSource;
				if (this.m_lookAtTarget == null && base.GameEntity != null)
				{
					this.m_lookAtTarget = new GameObject("LOOK_AT_TARGET");
					this.m_lookAtTarget.transform.parent = base.GameEntity.gameObject.transform;
					this.m_lookAtTarget.transform.transform.localRotation = Quaternion.identity;
					this.m_lookAtTarget.transform.transform.localPosition = new Vector3(0f, this.m_lookSource.position.y - base.gameObject.transform.position.y, 10f);
					this.m_look.solver.target = this.m_lookAtTarget.transform;
				}
				if (this.m_lookAtTarget != null)
				{
					this.m_defaultLookPos = new Vector3(0f, this.m_lookAtTarget.transform.position.y, 10f);
				}
			}
		}

		// Token: 0x06002C71 RID: 11377 RVA: 0x001499DC File Offset: 0x00147BDC
		private void UpdateLookTarget()
		{
			Vector3 vector = Vector3.zero;
			bool flag = false;
			bool flag2 = false;
			if (this.m_hasTargetController && base.GameEntity.TargetController.OffensiveTarget)
			{
				vector = this.GetLookAtTargetForEntity(base.GameEntity.TargetController.OffensiveTarget);
				flag = true;
				flag2 = true;
			}
			else if (this.m_hasTargetController && base.GameEntity.TargetController.DefensiveTarget)
			{
				vector = this.GetLookAtTargetForEntity(base.GameEntity.TargetController.DefensiveTarget);
				flag = true;
				flag2 = true;
			}
			else if (base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal && ClientGameManager.MainCamera)
			{
				vector = ClientGameManager.MainCamera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f)).GetPoint(GlobalSettings.Values.Ik.HeadLookDistance);
				flag2 = true;
			}
			else
			{
				vector = this.m_defaultLookPos;
			}
			if (flag2)
			{
				Vector3 vector2 = vector - this.m_lookSource.position;
				float magnitude = vector2.magnitude;
				vector2 /= magnitude;
				if (!flag && Vector3.Dot(vector2, Vector3.up) < 0f)
				{
					vector2.y = 0f;
					vector2.Normalize();
					vector = new Vector3(vector.x, this.m_lookSource.position.y, vector.z);
				}
				float num = Vector3.Angle(base.GameEntity.gameObject.transform.forward, vector2);
				float headLookAngle = GlobalSettings.Values.Ik.HeadLookAngle;
				float num2 = GlobalSettings.Values.Ik.HeadLookDistance * 1.1f;
				flag2 = (num < headLookAngle && magnitude < num2);
			}
			this.m_look.solver.IKPositionWeight = Mathf.MoveTowards(this.m_look.solver.IKPositionWeight, flag2 ? 1f : 0f, Time.deltaTime * GlobalSettings.Values.Ik.HeadLookWeightLerpRate);
			this.m_lookAtTarget.transform.position = Vector3.Lerp(this.m_lookAtTarget.transform.position, vector, Time.deltaTime * GlobalSettings.Values.Ik.HeadLookTargetMoveRate);
		}

		// Token: 0x06002C72 RID: 11378 RVA: 0x00149C40 File Offset: 0x00147E40
		private Vector3 GetLookAtTargetForEntity(GameEntity entity)
		{
			if (!entity)
			{
				return base.gameObject.transform.position;
			}
			if (entity.IKController)
			{
				return entity.IKController.LookAtMe.position;
			}
			if (entity.CharacterData && entity.CharacterData.ReferencePoints != null && entity.CharacterData.ReferencePoints.Value.DamageTarget)
			{
				return entity.CharacterData.ReferencePoints.Value.DamageTarget.transform.position;
			}
			return entity.gameObject.transform.position;
		}

		// Token: 0x04002C0B RID: 11275
		public const string kLeftHandIk = "LeftHandIK";

		// Token: 0x04002C0C RID: 11276
		public const string kRightHandIk = "RightHandIK";

		// Token: 0x04002C0E RID: 11278
		[SerializeField]
		protected IK m_main;

		// Token: 0x04002C0F RID: 11279
		[SerializeField]
		protected LookAtIK m_look;

		// Token: 0x04002C10 RID: 11280
		[SerializeField]
		protected Grounder m_grounder;

		// Token: 0x04002C11 RID: 11281
		[SerializeField]
		private MatchGroundAngle m_matchGroundAngle;

		// Token: 0x04002C12 RID: 11282
		[SerializeField]
		private MatchGroundHeight m_matchGroundHeight;

		// Token: 0x04002C13 RID: 11283
		[SerializeField]
		private bool m_overrideGroundLayers;

		// Token: 0x04002C14 RID: 11284
		[SerializeField]
		private LayerMask m_grounderLayers = -1;

		// Token: 0x04002C15 RID: 11285
		private const string kMaxWeights = "Max Weights";

		// Token: 0x04002C16 RID: 11286
		private const float kMinWeight = 0.1f;

		// Token: 0x04002C17 RID: 11287
		private const float kMaxWeight = 1f;

		// Token: 0x04002C18 RID: 11288
		[Range(0.1f, 1f)]
		[SerializeField]
		private float m_mainMaxWeight = 1f;

		// Token: 0x04002C19 RID: 11289
		[Range(0.1f, 1f)]
		[SerializeField]
		private float m_lookMaxWeight = 1f;

		// Token: 0x04002C1A RID: 11290
		[Range(0.1f, 1f)]
		[SerializeField]
		private float m_grounderMaxWeight = 1f;

		// Token: 0x04002C1B RID: 11291
		protected Transform m_otherLookAtTarget;

		// Token: 0x04002C1C RID: 11292
		protected GameObject m_lookAtTarget;

		// Token: 0x04002C1D RID: 11293
		protected MeshRenderer m_lookAtTargetMesh;

		// Token: 0x04002C1E RID: 11294
		protected FullBodyBipedIK m_fbbik;

		// Token: 0x04002C1F RID: 11295
		private GrounderIK m_grounderIk;

		// Token: 0x04002C20 RID: 11296
		private GrounderQuadruped m_grounderQuad;

		// Token: 0x04002C21 RID: 11297
		private bool m_enabled = true;

		// Token: 0x04002C22 RID: 11298
		private bool m_preventChanges;

		// Token: 0x04002C23 RID: 11299
		protected bool m_hasTargetController;

		// Token: 0x04002C24 RID: 11300
		protected Transform m_lookSource;

		// Token: 0x04002C25 RID: 11301
		private Vector3 m_defaultLookPos = Vector3.zero;

		// Token: 0x04002C26 RID: 11302
		private float m_sqrMagDistance = float.MaxValue;

		// Token: 0x04002C27 RID: 11303
		private float m_targetWeightFraction = 1f;

		// Token: 0x04002C28 RID: 11304
		private float m_lookBehaviorWeightFraction = 1f;

		// Token: 0x04002C29 RID: 11305
		private IKController.IKData m_ikData;

		// Token: 0x04002C2A RID: 11306
		public const float kRate = 2f;

		// Token: 0x0200058E RID: 1422
		private struct IKData
		{
			// Token: 0x04002C2B RID: 11307
			public float MainWeight;

			// Token: 0x04002C2C RID: 11308
			public float LookWeight;

			// Token: 0x04002C2D RID: 11309
			public float GrounderWeight;

			// Token: 0x04002C2E RID: 11310
			public IKSolver MainSolver;

			// Token: 0x04002C2F RID: 11311
			public IKSolver LookSolver;
		}
	}
}
