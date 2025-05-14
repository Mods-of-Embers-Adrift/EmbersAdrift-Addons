using System;
using SoL.Game.Animation;
using SoL.Game.Audio;
using SoL.Game.Audio.Ambient;
using SoL.Game.Culling;
using SoL.Game.Dueling;
using SoL.Game.EffectSystem;
using SoL.Game.Influence;
using SoL.Game.Interactives;
using SoL.Game.Nameplates;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Randomization;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.Targeting;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200057B RID: 1403
	public sealed class GameEntity : MonoBehaviour
	{
		// Token: 0x1400008E RID: 142
		// (add) Token: 0x06002B57 RID: 11095 RVA: 0x0014685C File Offset: 0x00144A5C
		// (remove) Token: 0x06002B58 RID: 11096 RVA: 0x00146894 File Offset: 0x00144A94
		public event Action Destroyed;

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06002B59 RID: 11097 RVA: 0x0005E0AA File Offset: 0x0005C2AA
		public GameEntityType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06002B5A RID: 11098 RVA: 0x0005E0B2 File Offset: 0x0005C2B2
		// (set) Token: 0x06002B5B RID: 11099 RVA: 0x0005E0DB File Offset: 0x0005C2DB
		public UserRecord User
		{
			get
			{
				if (GameManager.IsServer)
				{
					return this.m_user;
				}
				if (!(this == LocalPlayer.GameEntity))
				{
					return this.m_user;
				}
				return SessionData.User;
			}
			set
			{
				this.m_user = value;
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06002B5C RID: 11100 RVA: 0x0005E0E4 File Offset: 0x0005C2E4
		public AccessFlags UserFlags
		{
			get
			{
				if (this.User != null)
				{
					return this.User.Flags;
				}
				return AccessFlags.None;
			}
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06002B5D RID: 11101 RVA: 0x0005E0FB File Offset: 0x0005C2FB
		public bool GM
		{
			get
			{
				return this.User != null && this.User.IsGM();
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x06002B5E RID: 11102 RVA: 0x0005E112 File Offset: 0x0005C312
		public bool Subscriber
		{
			get
			{
				return this.User != null && this.User.IsSubscriber();
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x06002B5F RID: 11103 RVA: 0x0005E129 File Offset: 0x0005C329
		public bool IsTrial
		{
			get
			{
				return this.User != null && this.User.IsTrial();
			}
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06002B60 RID: 11104 RVA: 0x0005E140 File Offset: 0x0005C340
		// (set) Token: 0x06002B61 RID: 11105 RVA: 0x0005E148 File Offset: 0x0005C348
		public bool IsDestroying { get; set; }

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x06002B62 RID: 11106 RVA: 0x0005E151 File Offset: 0x0005C351
		// (set) Token: 0x06002B63 RID: 11107 RVA: 0x0005E159 File Offset: 0x0005C359
		public bool RemoveFromActiveSpawns { get; set; }

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06002B64 RID: 11108 RVA: 0x0005E162 File Offset: 0x0005C362
		public bool IsMissingBag
		{
			get
			{
				return this.CharacterData == null || this.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag);
			}
		}

		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x06002B65 RID: 11109 RVA: 0x0005E18A File Offset: 0x0005C38A
		public bool IsAlive
		{
			get
			{
				return this.VitalsReplicator != null && this.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x06002B66 RID: 11110 RVA: 0x0005E1AF File Offset: 0x0005C3AF
		public bool IsDead
		{
			get
			{
				return this.VitalsReplicator != null && this.VitalsReplicator.CurrentHealthState.Value == HealthState.Dead;
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06002B67 RID: 11111 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
		public bool InCombat
		{
			get
			{
				return this.CharacterData != null && this.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat);
			}
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06002B68 RID: 11112 RVA: 0x0005E1FC File Offset: 0x0005C3FC
		public bool IsNoTarget
		{
			get
			{
				return this.CharacterData != null && this.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.NoTarget);
			}
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06002B69 RID: 11113 RVA: 0x0005E228 File Offset: 0x0005C428
		// (set) Token: 0x06002B6A RID: 11114 RVA: 0x0005E230 File Offset: 0x0005C430
		public bool GroupBagDragConsent { get; set; }

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06002B6B RID: 11115 RVA: 0x0005E239 File Offset: 0x0005C439
		// (set) Token: 0x06002B6C RID: 11116 RVA: 0x0005E241 File Offset: 0x0005C441
		public bool AlchemyQuestRequirementsMet { get; set; }

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x06002B6D RID: 11117 RVA: 0x0005E24A File Offset: 0x0005C44A
		public bool IsStunned
		{
			get
			{
				return this.HasBehaviorFlag(BehaviorEffectTypeFlags.Stunned);
			}
		}

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06002B6E RID: 11118 RVA: 0x0005E253 File Offset: 0x0005C453
		public bool IsLulled
		{
			get
			{
				return this.HasBehaviorFlag(BehaviorEffectTypeFlags.Lull);
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06002B6F RID: 11119 RVA: 0x0005E25D File Offset: 0x0005C45D
		public bool IsFeared
		{
			get
			{
				return this.HasBehaviorFlag(BehaviorEffectTypeFlags.Feared);
			}
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06002B70 RID: 11120 RVA: 0x0005E266 File Offset: 0x0005C466
		public bool IsConfused
		{
			get
			{
				return this.HasBehaviorFlag(BehaviorEffectTypeFlags.Confused);
			}
		}

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06002B71 RID: 11121 RVA: 0x0005E270 File Offset: 0x0005C470
		public bool IsRooted
		{
			get
			{
				return this.Motor != null && this.Motor.IsRooted;
			}
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x0005E287 File Offset: 0x0005C487
		private bool HasBehaviorFlag(BehaviorEffectTypeFlags flag)
		{
			return this.VitalsReplicator != null && this.VitalsReplicator.BehaviorFlags.Value.HasBitFlag(flag);
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x001468CC File Offset: 0x00144ACC
		public void AddBehaviorFlags(BehaviorEffectTypeFlags behaviorFlags)
		{
			if (this.Type == GameEntityType.Npc && this.ServerNpcController)
			{
				this.ServerNpcController.BehaviorFlagsInternal |= behaviorFlags;
			}
			if (this.VitalsReplicator)
			{
				this.VitalsReplicator.BehaviorFlags.Value |= behaviorFlags;
			}
		}

		// Token: 0x06002B74 RID: 11124 RVA: 0x00146928 File Offset: 0x00144B28
		public void SetBehaviorFlags(BehaviorEffectTypeFlags behaviorFlags)
		{
			if (this.Type == GameEntityType.Npc && this.ServerNpcController)
			{
				this.ServerNpcController.BehaviorFlagsInternal = behaviorFlags;
			}
			if (this.VitalsReplicator)
			{
				this.VitalsReplicator.BehaviorFlags.Value = behaviorFlags;
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06002B75 RID: 11125 RVA: 0x0005E2AF File Offset: 0x0005C4AF
		// (set) Token: 0x06002B76 RID: 11126 RVA: 0x0005E2B7 File Offset: 0x0005C4B7
		public NetworkEntity NetworkEntity { get; set; }

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06002B77 RID: 11127 RVA: 0x0005E2C0 File Offset: 0x0005C4C0
		// (set) Token: 0x06002B78 RID: 11128 RVA: 0x0005E2C8 File Offset: 0x0005C4C8
		public DCAController DCAController { get; set; }

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06002B79 RID: 11129 RVA: 0x0005E2D1 File Offset: 0x0005C4D1
		// (set) Token: 0x06002B7A RID: 11130 RVA: 0x0005E2D9 File Offset: 0x0005C4D9
		public ICulledShadowCastingObject EntityVisuals { get; set; }

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06002B7B RID: 11131 RVA: 0x0005E2E2 File Offset: 0x0005C4E2
		// (set) Token: 0x06002B7C RID: 11132 RVA: 0x0005E2EA File Offset: 0x0005C4EA
		public CulledEntity CulledEntity { get; set; }

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06002B7D RID: 11133 RVA: 0x0005E2F3 File Offset: 0x0005C4F3
		// (set) Token: 0x06002B7E RID: 11134 RVA: 0x0005E2FB File Offset: 0x0005C4FB
		public IKController IKController { get; set; }

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06002B7F RID: 11135 RVA: 0x0005E304 File Offset: 0x0005C504
		// (set) Token: 0x06002B80 RID: 11136 RVA: 0x0005E30C File Offset: 0x0005C50C
		public EffectController EffectController { get; set; }

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06002B81 RID: 11137 RVA: 0x0005E315 File Offset: 0x0005C515
		// (set) Token: 0x06002B82 RID: 11138 RVA: 0x0005E31D File Offset: 0x0005C51D
		public SkillsController SkillsController { get; set; }

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06002B83 RID: 11139 RVA: 0x0005E326 File Offset: 0x0005C526
		// (set) Token: 0x06002B84 RID: 11140 RVA: 0x0005E32E File Offset: 0x0005C52E
		public Vitals Vitals { get; set; }

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06002B85 RID: 11141 RVA: 0x0005E337 File Offset: 0x0005C537
		// (set) Token: 0x06002B86 RID: 11142 RVA: 0x0005E33F File Offset: 0x0005C53F
		public VitalsReplicator VitalsReplicator { get; set; }

		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06002B87 RID: 11143 RVA: 0x0005E348 File Offset: 0x0005C548
		// (set) Token: 0x06002B88 RID: 11144 RVA: 0x0005E350 File Offset: 0x0005C550
		public BaseTargetController TargetController { get; set; }

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06002B89 RID: 11145 RVA: 0x0005E359 File Offset: 0x0005C559
		// (set) Token: 0x06002B8A RID: 11146 RVA: 0x0005E361 File Offset: 0x0005C561
		public CharacterData CharacterData { get; set; }

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06002B8B RID: 11147 RVA: 0x0005E36A File Offset: 0x0005C56A
		// (set) Token: 0x06002B8C RID: 11148 RVA: 0x0005E372 File Offset: 0x0005C572
		public GroundSampler GroundSampler { get; set; }

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06002B8D RID: 11149 RVA: 0x0005E37B File Offset: 0x0005C57B
		// (set) Token: 0x06002B8E RID: 11150 RVA: 0x0005E383 File Offset: 0x0005C583
		public NpcStanceManager NpcStanceManager { get; set; }

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x06002B8F RID: 11151 RVA: 0x0005E38C File Offset: 0x0005C58C
		// (set) Token: 0x06002B90 RID: 11152 RVA: 0x0005E394 File Offset: 0x0005C594
		public IAnimationController AnimancerController { get; set; }

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06002B91 RID: 11153 RVA: 0x0005E39D File Offset: 0x0005C59D
		// (set) Token: 0x06002B92 RID: 11154 RVA: 0x0005E3A5 File Offset: 0x0005C5A5
		public AnimancerReplicator AnimatorReplicator { get; set; }

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06002B93 RID: 11155 RVA: 0x0005E3AE File Offset: 0x0005C5AE
		// (set) Token: 0x06002B94 RID: 11156 RVA: 0x0005E3B6 File Offset: 0x0005C5B6
		public ICollectionController CollectionController { get; set; }

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06002B95 RID: 11157 RVA: 0x0005E3BF File Offset: 0x0005C5BF
		// (set) Token: 0x06002B96 RID: 11158 RVA: 0x0005E3C7 File Offset: 0x0005C5C7
		public ITargetable Targetable { get; set; }

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06002B97 RID: 11159 RVA: 0x0005E3D0 File Offset: 0x0005C5D0
		// (set) Token: 0x06002B98 RID: 11160 RVA: 0x0005E3D8 File Offset: 0x0005C5D8
		public IInteractive Interactive { get; set; }

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06002B99 RID: 11161 RVA: 0x0005E3E1 File Offset: 0x0005C5E1
		// (set) Token: 0x06002B9A RID: 11162 RVA: 0x0005E3E9 File Offset: 0x0005C5E9
		public IInfluenceSource InfluenceSource { get; set; }

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06002B9B RID: 11163 RVA: 0x0005E3F2 File Offset: 0x0005C5F2
		// (set) Token: 0x06002B9C RID: 11164 RVA: 0x0005E3FA File Offset: 0x0005C5FA
		public ISpawnable Spawnable { get; set; }

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06002B9D RID: 11165 RVA: 0x0005E403 File Offset: 0x0005C603
		// (set) Token: 0x06002B9E RID: 11166 RVA: 0x0005E40B File Offset: 0x0005C60B
		public IMotor Motor { get; set; }

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06002B9F RID: 11167 RVA: 0x0005E414 File Offset: 0x0005C614
		// (set) Token: 0x06002BA0 RID: 11168 RVA: 0x0005E41C File Offset: 0x0005C61C
		public ServerPlayerController ServerPlayerController { get; set; }

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06002BA1 RID: 11169 RVA: 0x0005E425 File Offset: 0x0005C625
		// (set) Token: 0x06002BA2 RID: 11170 RVA: 0x0005E42D File Offset: 0x0005C62D
		public ServerNpcController ServerNpcController { get; set; }

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06002BA3 RID: 11171 RVA: 0x0005E436 File Offset: 0x0005C636
		// (set) Token: 0x06002BA4 RID: 11172 RVA: 0x0005E43E File Offset: 0x0005C63E
		public AudioEventController AudioEventController { get; set; }

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06002BA5 RID: 11173 RVA: 0x0005E447 File Offset: 0x0005C647
		// (set) Token: 0x06002BA6 RID: 11174 RVA: 0x0005E44F File Offset: 0x0005C64F
		public AmbientAudioController AmbientAudioController { get; set; }

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06002BA7 RID: 11175 RVA: 0x0005E458 File Offset: 0x0005C658
		// (set) Token: 0x06002BA8 RID: 11176 RVA: 0x0005E460 File Offset: 0x0005C660
		public ITrailGenerator TrailGenerator { get; set; }

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06002BA9 RID: 11177 RVA: 0x0005E469 File Offset: 0x0005C669
		// (set) Token: 0x06002BAA RID: 11178 RVA: 0x0005E471 File Offset: 0x0005C671
		public EntityAudioController AudioController { get; set; }

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06002BAB RID: 11179 RVA: 0x0005E47A File Offset: 0x0005C67A
		// (set) Token: 0x06002BAC RID: 11180 RVA: 0x0005E482 File Offset: 0x0005C682
		public SpawnController SpawnController { get; set; }

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06002BAD RID: 11181 RVA: 0x0005E48B File Offset: 0x0005C68B
		// (set) Token: 0x06002BAE RID: 11182 RVA: 0x0005E493 File Offset: 0x0005C693
		public NpcCallForHelp CallForHelp { get; set; }

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06002BAF RID: 11183 RVA: 0x0005E49C File Offset: 0x0005C69C
		// (set) Token: 0x06002BB0 RID: 11184 RVA: 0x0005E4A4 File Offset: 0x0005C6A4
		public NpcReferencePoints NpcReferencePoints { get; set; }

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06002BB1 RID: 11185 RVA: 0x0005E4AD File Offset: 0x0005C6AD
		// (set) Token: 0x06002BB2 RID: 11186 RVA: 0x0005E4B5 File Offset: 0x0005C6B5
		public AshenController AshenController { get; set; }

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06002BB3 RID: 11187 RVA: 0x0005E4BE File Offset: 0x0005C6BE
		// (set) Token: 0x06002BB4 RID: 11188 RVA: 0x0005E4C6 File Offset: 0x0005C6C6
		public HandheldMountController HandheldMountController { get; set; }

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06002BB5 RID: 11189 RVA: 0x0005E4CF File Offset: 0x0005C6CF
		// (set) Token: 0x06002BB6 RID: 11190 RVA: 0x0005E4D7 File Offset: 0x0005C6D7
		public WorldSpaceOverheadController WorldSpaceOverheadController { get; set; }

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06002BB7 RID: 11191 RVA: 0x0005E4E0 File Offset: 0x0005C6E0
		// (set) Token: 0x06002BB8 RID: 11192 RVA: 0x0005E4E8 File Offset: 0x0005C6E8
		public IOverheadNameplateSpawner OverheadNameplate { get; set; }

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06002BB9 RID: 11193 RVA: 0x0005E4F1 File Offset: 0x0005C6F1
		// (set) Token: 0x06002BBA RID: 11194 RVA: 0x0005E4F9 File Offset: 0x0005C6F9
		public Vector3? NameplateHeightOffset { get; set; }

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06002BBB RID: 11195 RVA: 0x0005E502 File Offset: 0x0005C702
		// (set) Token: 0x06002BBC RID: 11196 RVA: 0x0005E50A File Offset: 0x0005C70A
		public Transform OverheadReference { get; set; }

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06002BBD RID: 11197 RVA: 0x0005E513 File Offset: 0x0005C713
		// (set) Token: 0x06002BBE RID: 11198 RVA: 0x0005E51B File Offset: 0x0005C71B
		public bool UseFullNameplateHeightOffset { get; set; }

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06002BBF RID: 11199 RVA: 0x0005E524 File Offset: 0x0005C724
		// (set) Token: 0x06002BC0 RID: 11200 RVA: 0x0005E52C File Offset: 0x0005C72C
		public SeedReplicator SeedReplicator { get; set; }

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x06002BC1 RID: 11201 RVA: 0x0005E535 File Offset: 0x0005C735
		// (set) Token: 0x06002BC2 RID: 11202 RVA: 0x0005E53D File Offset: 0x0005C73D
		public LocationReplicator LocationReplicator { get; set; }

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x06002BC3 RID: 11203 RVA: 0x0005E546 File Offset: 0x0005C746
		// (set) Token: 0x06002BC4 RID: 11204 RVA: 0x0005E54E File Offset: 0x0005C74E
		public GroundTorch GroundTorch { get; set; }

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06002BC5 RID: 11205 RVA: 0x0005E557 File Offset: 0x0005C757
		// (set) Token: 0x06002BC6 RID: 11206 RVA: 0x0005E55F File Offset: 0x0005C75F
		public bool HasUnreachableTarget { get; set; }

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06002BC7 RID: 11207 RVA: 0x0005E568 File Offset: 0x0005C768
		// (set) Token: 0x06002BC8 RID: 11208 RVA: 0x0005E570 File Offset: 0x0005C770
		public UpperLineOfSightTarget UpperLineOfSightTarget { get; set; }

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06002BC9 RID: 11209 RVA: 0x00146978 File Offset: 0x00144B78
		public float PrimaryTargetPointDistance
		{
			get
			{
				if (this.m_primaryTargetPointDistance < 0f)
				{
					this.m_primaryTargetPointDistance = (this.PrimaryTargetPoint ? Vector3.Distance(this.PrimaryTargetPoint.transform.position, base.gameObject.transform.position) : 0f);
				}
				return this.m_primaryTargetPointDistance;
			}
		}

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06002BCA RID: 11210 RVA: 0x0005E579 File Offset: 0x0005C779
		// (set) Token: 0x06002BCB RID: 11211 RVA: 0x0005E595 File Offset: 0x0005C795
		public GameObject PrimaryTargetPoint
		{
			get
			{
				if (!this.m_primaryTargetPoint)
				{
					return base.gameObject;
				}
				return this.m_primaryTargetPoint;
			}
			set
			{
				this.m_primaryTargetPoint = value;
			}
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06002BCC RID: 11212 RVA: 0x0005E59E File Offset: 0x0005C79E
		// (set) Token: 0x06002BCD RID: 11213 RVA: 0x0005E5A6 File Offset: 0x0005C7A6
		public AlternateTargetPoints AlternateTargetPoints { get; set; }

		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06002BCE RID: 11214 RVA: 0x0005E5AF File Offset: 0x0005C7AF
		// (set) Token: 0x06002BCF RID: 11215 RVA: 0x0005E5B7 File Offset: 0x0005C7B7
		public PlayerSpawn LocalDefaultPlayerSpawn { get; set; }

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06002BD0 RID: 11216 RVA: 0x0005E5C0 File Offset: 0x0005C7C0
		// (set) Token: 0x06002BD1 RID: 11217 RVA: 0x0005E5C8 File Offset: 0x0005C7C8
		public DuelStatus DuelState { get; set; }

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x0005E5D1 File Offset: 0x0005C7D1
		public HandHeldItemCache HandHeldItemCache
		{
			get
			{
				if (this.m_handHeldItemCache == null)
				{
					this.m_handHeldItemCache = new HandHeldItemCache();
				}
				this.m_handHeldItemCache.AccessReset(this);
				return this.m_handHeldItemCache;
			}
		}

		// Token: 0x06002BD3 RID: 11219 RVA: 0x0005E5F8 File Offset: 0x0005C7F8
		public void ResetHandheldItemCacheFrame()
		{
			HandHeldItemCache handHeldItemCache = this.m_handHeldItemCache;
			if (handHeldItemCache == null)
			{
				return;
			}
			handHeldItemCache.ResetLastFrameAccessed();
		}

		// Token: 0x06002BD4 RID: 11220 RVA: 0x001469D8 File Offset: 0x00144BD8
		public float GetCachedSqrDistanceFromLocalPlayer()
		{
			if (this.m_distanceFromLocalPlayerFrame < Time.frameCount && LocalPlayer.GameEntity)
			{
				this.m_cachedSqrDistanceFromLocalPlayer = (LocalPlayer.GameEntity.PrimaryTargetPoint.transform.position - this.PrimaryTargetPoint.transform.position).sqrMagnitude;
				this.m_distanceFromLocalPlayerFrame = Time.frameCount;
			}
			return this.m_cachedSqrDistanceFromLocalPlayer;
		}

		// Token: 0x06002BD5 RID: 11221 RVA: 0x00146A48 File Offset: 0x00144C48
		public float GetCachedSqrDistanceFromCamera()
		{
			if (this.m_distanceFromCameraFrame < Time.frameCount && ClientGameManager.MainCamera)
			{
				this.m_cachedSqrDistanceFromCamera = (ClientGameManager.MainCamera.gameObject.transform.position - this.PrimaryTargetPoint.transform.position).sqrMagnitude;
				this.m_distanceFromCameraFrame = Time.frameCount;
			}
			return this.m_cachedSqrDistanceFromCamera;
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06002BD6 RID: 11222 RVA: 0x0005E60A File Offset: 0x0005C80A
		// (set) Token: 0x06002BD7 RID: 11223 RVA: 0x0005E612 File Offset: 0x0005C812
		public float SqrDistanceFromLastNpcCallForHelp { get; set; }

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06002BD8 RID: 11224 RVA: 0x0005E61B File Offset: 0x0005C81B
		// (set) Token: 0x06002BD9 RID: 11225 RVA: 0x0005E623 File Offset: 0x0005C823
		public bool BypassLevelDeltaCombatAdjustments { get; set; }

		// Token: 0x06002BDA RID: 11226 RVA: 0x0005E62C File Offset: 0x0005C82C
		public GameObject GetCurrentAlternateTarget()
		{
			if (Time.frameCount != this.m_alternateTargetFrame)
			{
				this.m_alternateTarget = null;
			}
			return this.m_alternateTarget;
		}

		// Token: 0x06002BDB RID: 11227 RVA: 0x0005E648 File Offset: 0x0005C848
		public void SetCurrentAlternateTarget(GameObject obj)
		{
			this.m_alternateTarget = obj;
			this.m_alternateTargetFrame = (this.m_alternateTarget ? Time.frameCount : -1);
		}

		// Token: 0x06002BDC RID: 11228 RVA: 0x00146AB8 File Offset: 0x00144CB8
		public bool CanUseActiveDefense(ActiveDefenseType defenseType)
		{
			float time = Time.time;
			float num = time;
			switch (defenseType)
			{
			case ActiveDefenseType.Avoid:
				num = this.m_lastAvoid;
				break;
			case ActiveDefenseType.Block:
				num = this.m_lastBlock;
				break;
			case ActiveDefenseType.Parry:
				num = this.m_lastParry;
				break;
			}
			return time - num >= (float)GlobalSettings.Values.Combat.ActiveDefenseCooldown;
		}

		// Token: 0x06002BDD RID: 11229 RVA: 0x0005E66C File Offset: 0x0005C86C
		public void MarkActiveDefenseUsed(ActiveDefenseType defenseType)
		{
			switch (defenseType)
			{
			case ActiveDefenseType.Avoid:
				this.m_lastAvoid = Time.time;
				return;
			case ActiveDefenseType.Block:
				this.m_lastBlock = Time.time;
				return;
			case ActiveDefenseType.Parry:
				this.m_lastParry = Time.time;
				return;
			default:
				return;
			}
		}

		// Token: 0x06002BDE RID: 11230 RVA: 0x0005E6A6 File Offset: 0x0005C8A6
		private void OnDestroy()
		{
			HandHeldItemCache handHeldItemCache = this.m_handHeldItemCache;
			if (handHeldItemCache != null)
			{
				handHeldItemCache.ResetReferences();
			}
			this.m_handHeldItemCache = null;
			Action destroyed = this.Destroyed;
			if (destroyed == null)
			{
				return;
			}
			destroyed();
		}

		// Token: 0x04002B76 RID: 11126
		[SerializeField]
		private GameEntityType m_type;

		// Token: 0x04002B77 RID: 11127
		private UserRecord m_user;

		// Token: 0x04002BA6 RID: 11174
		private float m_primaryTargetPointDistance = -1f;

		// Token: 0x04002BA7 RID: 11175
		private GameObject m_primaryTargetPoint;

		// Token: 0x04002BAB RID: 11179
		private HandHeldItemCache m_handHeldItemCache;

		// Token: 0x04002BAC RID: 11180
		private int m_distanceFromLocalPlayerFrame;

		// Token: 0x04002BAD RID: 11181
		private float m_cachedSqrDistanceFromLocalPlayer;

		// Token: 0x04002BAE RID: 11182
		private int m_distanceFromCameraFrame;

		// Token: 0x04002BAF RID: 11183
		private float m_cachedSqrDistanceFromCamera;

		// Token: 0x04002BB2 RID: 11186
		[NonSerialized]
		private int m_alternateTargetFrame = -1;

		// Token: 0x04002BB3 RID: 11187
		[NonSerialized]
		private GameObject m_alternateTarget;

		// Token: 0x04002BB4 RID: 11188
		private float m_lastAvoid;

		// Token: 0x04002BB5 RID: 11189
		private float m_lastBlock;

		// Token: 0x04002BB6 RID: 11190
		private float m_lastParry;
	}
}
