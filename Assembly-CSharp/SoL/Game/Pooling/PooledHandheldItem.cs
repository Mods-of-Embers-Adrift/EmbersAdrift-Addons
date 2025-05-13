using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.AssetBundles;
using SoL.Game.Audio;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace SoL.Game.Pooling
{
	// Token: 0x020007D8 RID: 2008
	public class PooledHandheldItem : PooledObject, IAddressableSpawnedNotifier
	{
		// Token: 0x17000D61 RID: 3425
		// (get) Token: 0x06003A8A RID: 14986 RVA: 0x0004479C File Offset: 0x0004299C
		internal override bool DelayedDestruction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D62 RID: 3426
		// (get) Token: 0x06003A8B RID: 14987 RVA: 0x0005179F File Offset: 0x0004F99F
		internal override float DelayedDestructionTime
		{
			get
			{
				return 60f;
			}
		}

		// Token: 0x17000D63 RID: 3427
		// (get) Token: 0x06003A8C RID: 14988 RVA: 0x00067A9B File Offset: 0x00065C9B
		private bool m_hasExtraVisuals
		{
			get
			{
				return this.m_extraVisuals != null;
			}
		}

		// Token: 0x17000D64 RID: 3428
		// (get) Token: 0x06003A8D RID: 14989 RVA: 0x00067AA9 File Offset: 0x00065CA9
		public Renderer RendererToAlter
		{
			get
			{
				return this.m_rendererToAlter;
			}
		}

		// Token: 0x17000D65 RID: 3429
		// (get) Token: 0x06003A8E RID: 14990 RVA: 0x00178404 File Offset: 0x00176604
		public AudioEvent[] AudioEvents
		{
			get
			{
				if (this.m_audioControllerScriptable != null && this.m_audioControllerScriptable.AudioEvents != null)
				{
					return this.m_audioControllerScriptable.AudioEvents;
				}
				if (!(this.m_audioController == null))
				{
					return this.m_audioController.AudioEvents;
				}
				return null;
			}
		}

		// Token: 0x17000D66 RID: 3430
		// (get) Token: 0x06003A8F RID: 14991 RVA: 0x00067AB1 File Offset: 0x00065CB1
		public PooledHandheldItem.MountDataWithIKTargets AttachedData
		{
			get
			{
				return this.m_attached;
			}
		}

		// Token: 0x06003A90 RID: 14992 RVA: 0x00178454 File Offset: 0x00176654
		private void Awake()
		{
			if (this.m_visuals)
			{
				this.m_visualsToModify = this.NestVisual(this.m_visuals, "VisualsMod");
				if (this.m_attachedPoint)
				{
					this.m_attachedPoint.gameObject.transform.SetParent(this.m_visualsToModify.transform);
				}
				if (this.m_stowedPoint)
				{
					this.m_stowedPoint.gameObject.transform.SetParent(this.m_visualsToModify.transform);
				}
				if (this.m_audioController)
				{
					Transform transform = this.m_audioController.gameObject.transform;
					transform.SetParent(this.m_visualsToModify.transform);
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.identity;
					transform.localScale = Vector3.one;
				}
				if (this.m_attached.HandTarget)
				{
					this.m_attached.HandTarget.transform.SetParent(this.m_visualsToModify.transform);
				}
				AddressableSpawner component = this.m_visuals.GetComponent<AddressableSpawner>();
				if (component)
				{
					this.m_hasAddressableSpawner = true;
					component.ToNotify = this;
				}
			}
			if (this.m_extraVisuals)
			{
				this.m_extraVisualsToModify = this.NestVisual(this.m_extraVisuals, "ExtraVisualsMod");
			}
		}

		// Token: 0x06003A91 RID: 14993 RVA: 0x00067AB9 File Offset: 0x00065CB9
		public void Initialize(HandheldMountController controller, Transform parent, Vector3 pos, Quaternion rot)
		{
			this.m_controller = controller;
			base.Initialize(parent, pos, rot);
		}

		// Token: 0x06003A92 RID: 14994 RVA: 0x001785A8 File Offset: 0x001767A8
		private GameObject NestVisual(GameObject visual, string visualName)
		{
			GameObject gameObject = new GameObject(visualName);
			gameObject.transform.SetParent(base.gameObject.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			visual.transform.SetParent(gameObject.transform);
			return gameObject;
		}

		// Token: 0x06003A93 RID: 14995 RVA: 0x00178614 File Offset: 0x00176814
		public void MountItem(HumanoidReferencePoints? referencePoints, EquipmentSlot slot, bool isAttached, AnimancerAnimationSet combatSet, GameEntity entity = null)
		{
			if (referencePoints == null)
			{
				this.DisableVisuals();
				return;
			}
			if (this.m_visualsToModify)
			{
				PooledHandheldItem.MountData mountData = isAttached ? this.m_attached : this.m_detached;
				MountPosition mountPosition = mountData.MountPos;
				MountPointGizmos mountPointGizmos = isAttached ? this.m_attachedPoint : this.m_stowedPoint;
				if (this.m_deriveStowPointsFromCombat && combatSet)
				{
					mountPosition = combatSet.GetMountPosition(slot, isAttached);
				}
				Transform parent = referencePoints.Value.GetParent(mountPosition, slot);
				if (!parent)
				{
					this.DisableVisuals();
					return;
				}
				this.EnableVisual(this.m_visualsToModify, parent, mountPointGizmos.gameObject.transform, mountData, slot);
			}
			if (this.m_extraVisualsToModify)
			{
				Transform parent2 = referencePoints.Value.GetParent(this.m_extraVisualsMountData.MountPos, slot);
				this.EnableVisual(this.m_extraVisualsToModify, parent2, null, this.m_extraVisualsMountData, slot);
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06003A94 RID: 14996 RVA: 0x0017870C File Offset: 0x0017690C
		public override void ResetPooledObject()
		{
			this.DisableVisuals();
			base.ResetPooledObject();
			this.m_controller = null;
			if (this.m_visualShadows != null)
			{
				for (int i = 0; i < this.m_visualShadows.Count; i++)
				{
					this.m_visualShadows[i].ResetShadowCastingMode();
				}
			}
		}

		// Token: 0x06003A95 RID: 14997 RVA: 0x00178760 File Offset: 0x00176960
		private void EnableVisual(GameObject visual, Transform parent, Transform itemMountPoint, PooledHandheldItem.MountData mountData, EquipmentSlot slot)
		{
			if (!visual)
			{
				return;
			}
			if (visual.transform.parent != parent)
			{
				visual.transform.SetParentResetScale(parent);
			}
			mountData.MountItem(visual.transform, slot);
			if (!visual.activeSelf)
			{
				visual.SetActive(true);
			}
		}

		// Token: 0x06003A96 RID: 14998 RVA: 0x00067ACC File Offset: 0x00065CCC
		private void DisableVisuals()
		{
			this.ResetVisual(this.m_visualsToModify);
			this.ResetVisual(this.m_extraVisualsToModify);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06003A97 RID: 14999 RVA: 0x00067AF2 File Offset: 0x00065CF2
		private void ResetVisual(GameObject visual)
		{
			if (visual)
			{
				visual.SetActive(false);
				visual.transform.SetParentResetScale(base.gameObject.transform);
			}
		}

		// Token: 0x06003A98 RID: 15000 RVA: 0x00067B19 File Offset: 0x00065D19
		public void SetItemsActive(bool active)
		{
			if (this.m_visualsToModify)
			{
				this.m_visualsToModify.SetActive(active);
			}
			if (this.m_extraVisualsToModify)
			{
				this.m_extraVisualsToModify.SetActive(active);
			}
		}

		// Token: 0x06003A99 RID: 15001 RVA: 0x001787B4 File Offset: 0x001769B4
		private bool RegisterVisualShadows(Renderer[] renderers)
		{
			if (renderers == null || renderers.Length == 0)
			{
				return false;
			}
			if (this.m_visualShadows == null)
			{
				this.m_visualShadows = new List<PooledHandheldItem.VisualsShadowData>(Mathf.Max(renderers.Length, 10));
			}
			bool result = false;
			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] && renderers[i].shadowCastingMode != ShadowCastingMode.Off)
				{
					PooledHandheldItem.VisualsShadowData item = new PooledHandheldItem.VisualsShadowData(renderers[i]);
					this.m_visualShadows.Add(item);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003A9A RID: 15002 RVA: 0x00178824 File Offset: 0x00176A24
		internal void RefreshLocalShadowsOnly()
		{
			if (!this.m_controller || this.m_controller.GameEntity != LocalPlayer.GameEntity)
			{
				return;
			}
			if (this.m_hasAddressableSpawner && !this.m_addressableSpawned)
			{
				return;
			}
			if (!this.m_initializedVisualShadows)
			{
				if (this.m_visuals)
				{
					Renderer[] componentsInChildren = this.m_visuals.GetComponentsInChildren<Renderer>(true);
					this.RegisterVisualShadows(componentsInChildren);
				}
				if (this.m_extraVisuals)
				{
					Renderer[] componentsInChildren2 = this.m_extraVisuals.GetComponentsInChildren<Renderer>(true);
					this.RegisterVisualShadows(componentsInChildren2);
				}
				this.m_initializedVisualShadows = true;
			}
			if (this.m_visualShadows != null && this.m_visualShadows.Count > 0)
			{
				bool shadowsOnly = this.m_controller.ShadowsOnly;
				for (int i = 0; i < this.m_visualShadows.Count; i++)
				{
					this.m_visualShadows[i].SetShadowsOnly(shadowsOnly);
				}
			}
		}

		// Token: 0x06003A9B RID: 15003 RVA: 0x00067B4D File Offset: 0x00065D4D
		void IAddressableSpawnedNotifier.NotifyOfSpawn()
		{
			this.m_addressableSpawned = true;
			this.RefreshLocalShadowsOnly();
		}

		// Token: 0x0400390A RID: 14602
		private const string kVisualsGroup = "Primary Visuals";

		// Token: 0x0400390B RID: 14603
		private const string kVisualsSecondary = "Secondary Visuals";

		// Token: 0x0400390C RID: 14604
		private GameObject m_visualsToModify;

		// Token: 0x0400390D RID: 14605
		private GameObject m_extraVisualsToModify;

		// Token: 0x0400390E RID: 14606
		private HandheldMountController m_controller;

		// Token: 0x0400390F RID: 14607
		private bool m_hasAddressableSpawner;

		// Token: 0x04003910 RID: 14608
		private bool m_addressableSpawned;

		// Token: 0x04003911 RID: 14609
		[SerializeField]
		private GameObject m_visuals;

		// Token: 0x04003912 RID: 14610
		[SerializeField]
		private bool m_deriveStowPointsFromCombat;

		// Token: 0x04003913 RID: 14611
		[SerializeField]
		private MountPointGizmos m_attachedPoint;

		// Token: 0x04003914 RID: 14612
		[SerializeField]
		private MountPointGizmos m_stowedPoint;

		// Token: 0x04003915 RID: 14613
		[SerializeField]
		private PooledHandheldItem.MountDataWithIKTargets m_attached;

		// Token: 0x04003916 RID: 14614
		[SerializeField]
		private PooledHandheldItem.MountData m_detached;

		// Token: 0x04003917 RID: 14615
		[SerializeField]
		private GameObject m_extraVisuals;

		// Token: 0x04003918 RID: 14616
		[SerializeField]
		private PooledHandheldItem.MountData m_extraVisualsMountData;

		// Token: 0x04003919 RID: 14617
		[SerializeField]
		private AudioController m_audioController;

		// Token: 0x0400391A RID: 14618
		[SerializeField]
		private AudioControllerEventScriptable m_audioControllerScriptable;

		// Token: 0x0400391B RID: 14619
		[SerializeField]
		private Renderer m_rendererToAlter;

		// Token: 0x0400391C RID: 14620
		private bool m_initializedVisualShadows;

		// Token: 0x0400391D RID: 14621
		private List<PooledHandheldItem.VisualsShadowData> m_visualShadows;

		// Token: 0x020007D9 RID: 2009
		public enum HandTargetType
		{
			// Token: 0x0400391F RID: 14623
			None,
			// Token: 0x04003920 RID: 14624
			Left,
			// Token: 0x04003921 RID: 14625
			Right
		}

		// Token: 0x020007DA RID: 2010
		[Serializable]
		private class MountPosRot
		{
			// Token: 0x17000D67 RID: 3431
			// (get) Token: 0x06003A9D RID: 15005 RVA: 0x00067B64 File Offset: 0x00065D64
			public Vector3 Pos
			{
				get
				{
					return this.m_pos;
				}
			}

			// Token: 0x17000D68 RID: 3432
			// (get) Token: 0x06003A9E RID: 15006 RVA: 0x00067B6C File Offset: 0x00065D6C
			public Quaternion Rot
			{
				get
				{
					return this.m_rot;
				}
			}

			// Token: 0x04003922 RID: 14626
			[SerializeField]
			private Vector3 m_pos;

			// Token: 0x04003923 RID: 14627
			[SerializeField]
			private Quaternion m_rot;
		}

		// Token: 0x020007DB RID: 2011
		[Serializable]
		public class MountData
		{
			// Token: 0x17000D69 RID: 3433
			// (get) Token: 0x06003AA0 RID: 15008 RVA: 0x00067B74 File Offset: 0x00065D74
			private bool m_showSecondary
			{
				get
				{
					return this.m_mountPosition.IsDynamic();
				}
			}

			// Token: 0x17000D6A RID: 3434
			// (get) Token: 0x06003AA1 RID: 15009 RVA: 0x00067B81 File Offset: 0x00065D81
			private bool m_showSecondaryData
			{
				get
				{
					return this.m_showSecondary && this.m_customSecondary;
				}
			}

			// Token: 0x17000D6B RID: 3435
			// (get) Token: 0x06003AA2 RID: 15010 RVA: 0x00067B93 File Offset: 0x00065D93
			public MountPosition MountPos
			{
				get
				{
					return this.m_mountPosition;
				}
			}

			// Token: 0x17000D6C RID: 3436
			// (get) Token: 0x06003AA3 RID: 15011 RVA: 0x00045BCA File Offset: 0x00043DCA
			public virtual PooledHandheldItem.HandTargetType HandTargetType
			{
				get
				{
					return PooledHandheldItem.HandTargetType.None;
				}
			}

			// Token: 0x17000D6D RID: 3437
			// (get) Token: 0x06003AA4 RID: 15012 RVA: 0x00049FFA File Offset: 0x000481FA
			public virtual GameObject HandTarget
			{
				get
				{
					return null;
				}
			}

			// Token: 0x06003AA5 RID: 15013 RVA: 0x00178908 File Offset: 0x00176B08
			public void MountItem(Transform visual, EquipmentSlot slot)
			{
				if (!visual)
				{
					return;
				}
				PooledHandheldItem.MountPosRot mountPosRot = (this.m_showSecondary && this.m_customSecondary && (slot == EquipmentSlot.PrimaryWeapon_OffHand || slot == EquipmentSlot.SecondaryWeapon_OffHand)) ? this.m_secondary : this.m_primary;
				visual.localPosition = mountPosRot.Pos;
				visual.localRotation = mountPosRot.Rot;
			}

			// Token: 0x04003924 RID: 14628
			[FormerlySerializedAs("MountPosition")]
			[SerializeField]
			private MountPosition m_mountPosition;

			// Token: 0x04003925 RID: 14629
			[SerializeField]
			private PooledHandheldItem.MountPosRot m_primary;

			// Token: 0x04003926 RID: 14630
			[SerializeField]
			private bool m_customSecondary;

			// Token: 0x04003927 RID: 14631
			[SerializeField]
			private PooledHandheldItem.MountPosRot m_secondary;

			// Token: 0x04003928 RID: 14632
			public bool OverridePosition;

			// Token: 0x04003929 RID: 14633
			public Vector3 Position;

			// Token: 0x0400392A RID: 14634
			public bool OverrideRotation;

			// Token: 0x0400392B RID: 14635
			public Vector3 Rotation;
		}

		// Token: 0x020007DC RID: 2012
		[Serializable]
		public class MountDataWithIKTargets : PooledHandheldItem.MountData
		{
			// Token: 0x17000D6E RID: 3438
			// (get) Token: 0x06003AA7 RID: 15015 RVA: 0x00067B9B File Offset: 0x00065D9B
			private bool ShowHandTarget
			{
				get
				{
					return this.m_handTargetType > PooledHandheldItem.HandTargetType.None;
				}
			}

			// Token: 0x17000D6F RID: 3439
			// (get) Token: 0x06003AA8 RID: 15016 RVA: 0x00067BA6 File Offset: 0x00065DA6
			public override PooledHandheldItem.HandTargetType HandTargetType
			{
				get
				{
					return this.m_handTargetType;
				}
			}

			// Token: 0x17000D70 RID: 3440
			// (get) Token: 0x06003AA9 RID: 15017 RVA: 0x00067BAE File Offset: 0x00065DAE
			public override GameObject HandTarget
			{
				get
				{
					return this.m_handTarget;
				}
			}

			// Token: 0x0400392C RID: 14636
			[SerializeField]
			private PooledHandheldItem.HandTargetType m_handTargetType;

			// Token: 0x0400392D RID: 14637
			[SerializeField]
			private GameObject m_handTarget;
		}

		// Token: 0x020007DD RID: 2013
		private readonly struct VisualsShadowData
		{
			// Token: 0x06003AAB RID: 15019 RVA: 0x00067BBE File Offset: 0x00065DBE
			public VisualsShadowData(Renderer renderer)
			{
				if (!renderer)
				{
					throw new ArgumentNullException("renderer");
				}
				this.Renderer = renderer;
				this.DefaultShadowCastingMode = renderer.shadowCastingMode;
			}

			// Token: 0x06003AAC RID: 15020 RVA: 0x00067BE6 File Offset: 0x00065DE6
			internal void SetShadowsOnly(bool shadowsOnly)
			{
				if (this.Renderer && this.DefaultShadowCastingMode != ShadowCastingMode.Off)
				{
					this.Renderer.shadowCastingMode = (shadowsOnly ? ShadowCastingMode.ShadowsOnly : this.DefaultShadowCastingMode);
				}
			}

			// Token: 0x06003AAD RID: 15021 RVA: 0x00067C14 File Offset: 0x00065E14
			internal void ResetShadowCastingMode()
			{
				if (this.Renderer && this.DefaultShadowCastingMode != ShadowCastingMode.Off)
				{
					this.Renderer.shadowCastingMode = this.DefaultShadowCastingMode;
				}
			}

			// Token: 0x0400392E RID: 14638
			public readonly Renderer Renderer;

			// Token: 0x0400392F RID: 14639
			public readonly ShadowCastingMode DefaultShadowCastingMode;
		}
	}
}
