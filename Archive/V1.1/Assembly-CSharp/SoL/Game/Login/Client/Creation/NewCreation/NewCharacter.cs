using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.UMA;
using SoL.Managers;
using SoL.Networking.Database;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B6B RID: 2923
	public class NewCharacter : MonoBehaviour, IDcaSource
	{
		// Token: 0x170014EB RID: 5355
		// (get) Token: 0x060059CC RID: 22988 RVA: 0x0007C313 File Offset: 0x0007A513
		public CharacterSex Sex
		{
			get
			{
				return this.m_sex;
			}
		}

		// Token: 0x170014EC RID: 5356
		// (get) Token: 0x060059CD RID: 22989 RVA: 0x0007C31B File Offset: 0x0007A51B
		internal CharacterBuildType BuildType
		{
			get
			{
				return this.m_buildType;
			}
		}

		// Token: 0x170014ED RID: 5357
		// (get) Token: 0x060059CE RID: 22990 RVA: 0x0007C323 File Offset: 0x0007A523
		// (set) Token: 0x060059CF RID: 22991 RVA: 0x0007C32B File Offset: 0x0007A52B
		internal NewCharacterKey Key { get; private set; }

		// Token: 0x170014EE RID: 5358
		// (get) Token: 0x060059D0 RID: 22992 RVA: 0x0007C334 File Offset: 0x0007A534
		public DynamicCharacterAvatar Dca
		{
			get
			{
				return this.m_dca;
			}
		}

		// Token: 0x170014EF RID: 5359
		// (get) Token: 0x060059D1 RID: 22993 RVA: 0x001EAB9C File Offset: 0x001E8D9C
		public GameObject HeadPosObj
		{
			get
			{
				if (!this.m_animator)
				{
					return null;
				}
				if (!this.m_headPosObj)
				{
					Transform boneTransform = this.m_animator.GetBoneTransform(HumanBodyBones.Head);
					if (boneTransform)
					{
						this.m_headPosObj = new GameObject("HeadPos");
						this.m_headPosObj.transform.SetParent(boneTransform);
						this.m_headPosObj.transform.localPosition = Vector3.zero;
						this.m_headPosObj.transform.localRotation = Quaternion.identity;
						this.m_headPosObj.transform.localScale = Vector3.one;
					}
				}
				return this.m_headPosObj;
			}
		}

		// Token: 0x170014F0 RID: 5360
		// (get) Token: 0x060059D2 RID: 22994 RVA: 0x0007C33C File Offset: 0x0007A53C
		public Dictionary<string, DnaSetter> Dna
		{
			get
			{
				if (this.m_dna == null || this.m_dna.Count <= 0)
				{
					this.m_dna = this.m_dca.GetDNA(null);
				}
				return this.m_dna;
			}
		}

		// Token: 0x060059D3 RID: 22995 RVA: 0x0007C36C File Offset: 0x0007A56C
		private void Start()
		{
			UMAGlibManager.RegisterController(this);
			if (this.m_humanoidAnimancerController)
			{
				this.m_humanoidAnimancerController.PreventIdleTicks = true;
			}
		}

		// Token: 0x060059D4 RID: 22996 RVA: 0x0007C38D File Offset: 0x0007A58D
		private void OnDestroy()
		{
			UMAGlibManager.UnregisterController(this);
		}

		// Token: 0x060059D5 RID: 22997 RVA: 0x001EAC44 File Offset: 0x001E8E44
		private void Update()
		{
			if (this.m_unhideFrame != null)
			{
				int frameCount = Time.frameCount;
				int? unhideFrame = this.m_unhideFrame;
				if (frameCount > unhideFrame.GetValueOrDefault() & unhideFrame != null)
				{
					this.m_dca.hide = false;
					this.m_unhideFrame = null;
				}
			}
		}

		// Token: 0x060059D6 RID: 22998 RVA: 0x0007C395 File Offset: 0x0007A595
		public void Init(NewCharacterManager manager)
		{
			this.m_manager = manager;
			this.Key = new NewCharacterKey(this.m_sex, this.m_buildType);
			this.m_dca.hide = true;
		}

		// Token: 0x060059D7 RID: 22999 RVA: 0x001EAC98 File Offset: 0x001E8E98
		public void Init(NewCharacterManager manager, CharacterSex sex, CharacterBuildType buildType)
		{
			this.m_manager = manager;
			this.m_sex = sex;
			this.m_buildType = buildType;
			this.Key = new NewCharacterKey(this.m_sex, this.m_buildType);
			this.m_dca.IsCharacterCreator = true;
			this.m_dca.hide = true;
			string umaRace = sex.GetUmaRace(buildType);
			this.m_dca.ChangeRace(umaRace, DynamicCharacterAvatar.ChangeRaceOptions.useDefaults, false);
			this.m_dca.BuildCharacterEnabled = true;
			if (this.m_humanoidAnimancerController != null)
			{
				this.m_animationController = this.m_humanoidAnimancerController;
				this.m_animationController.AssignSex(this.m_sex);
			}
		}

		// Token: 0x060059D8 RID: 23000 RVA: 0x0007C3C1 File Offset: 0x0007A5C1
		public void SelectCharacter(bool isSelected)
		{
			if (isSelected)
			{
				this.m_unhideFrame = new int?(Time.frameCount + 1);
				return;
			}
			this.m_dca.hide = true;
		}

		// Token: 0x060059D9 RID: 23001 RVA: 0x001EAD38 File Offset: 0x001E8F38
		public void UpdateDna(string dnaLeft, string dnaRight, float value)
		{
			float val;
			float val2;
			UMAManager.GetLeftRightDna(value, out val, out val2);
			DnaSetter dnaSetter;
			if (this.Dna.TryGetValue(dnaLeft, out dnaSetter))
			{
				dnaSetter.Set(val);
			}
			DnaSetter dnaSetter2;
			if (this.Dna.TryGetValue(dnaRight, out dnaSetter2))
			{
				dnaSetter2.Set(val2);
			}
		}

		// Token: 0x060059DA RID: 23002 RVA: 0x0007C3E5 File Offset: 0x0007A5E5
		public void ToggleFaceCam(bool faceCamActive)
		{
			if (this.m_humanoidAnimancerController)
			{
				this.m_humanoidAnimancerController.PreventIdleTicks = faceCamActive;
			}
		}

		// Token: 0x170014F1 RID: 5361
		// (get) Token: 0x060059DB RID: 23003 RVA: 0x0007C400 File Offset: 0x0007A600
		// (set) Token: 0x060059DC RID: 23004 RVA: 0x0007C408 File Offset: 0x0007A608
		public int? Resolution { get; private set; }

		// Token: 0x060059DD RID: 23005 RVA: 0x0007C411 File Offset: 0x0007A611
		public void SetResolution(int? resolution, bool update)
		{
			if (update && this.m_dca && this.m_dca.BuildCharacterEnabled)
			{
				this.m_dca.ForceUpdate(false, true, true);
			}
			this.Resolution = resolution;
		}

		// Token: 0x04004EF9 RID: 20217
		private GameObject m_headPosObj;

		// Token: 0x04004EFA RID: 20218
		[SerializeField]
		private DynamicCharacterAvatar m_dca;

		// Token: 0x04004EFB RID: 20219
		[SerializeField]
		private CharacterSex m_sex;

		// Token: 0x04004EFC RID: 20220
		[SerializeField]
		private CharacterBuildType m_buildType;

		// Token: 0x04004EFD RID: 20221
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04004EFE RID: 20222
		[SerializeField]
		private HumanoidAnimancerController m_humanoidAnimancerController;

		// Token: 0x04004EFF RID: 20223
		private IAnimationController m_animationController;

		// Token: 0x04004F00 RID: 20224
		private NewCharacterManager m_manager;

		// Token: 0x04004F01 RID: 20225
		private int? m_unhideFrame;

		// Token: 0x04004F02 RID: 20226
		private Dictionary<string, DnaSetter> m_dna;
	}
}
