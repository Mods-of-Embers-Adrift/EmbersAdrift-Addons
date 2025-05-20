using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Nameplates;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x0200050C RID: 1292
	public class OverheadNameplateManager : MonoBehaviour
	{
		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x060024F2 RID: 9458 RVA: 0x0005A6FC File Offset: 0x000588FC
		// (set) Token: 0x060024F3 RID: 9459 RVA: 0x0005A704 File Offset: 0x00058904
		private int WorldSpaceActive { get; set; }

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x060024F4 RID: 9460 RVA: 0x0005A70D File Offset: 0x0005890D
		// (set) Token: 0x060024F5 RID: 9461 RVA: 0x0005A715 File Offset: 0x00058915
		private int WorldSpaceInactive { get; set; }

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x060024F6 RID: 9462 RVA: 0x0005A71E File Offset: 0x0005891E
		// (set) Token: 0x060024F7 RID: 9463 RVA: 0x0005A726 File Offset: 0x00058926
		private int WorldSpaceTotal { get; set; }

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x060024F8 RID: 9464 RVA: 0x0005A72F File Offset: 0x0005892F
		// (set) Token: 0x060024F9 RID: 9465 RVA: 0x0005A737 File Offset: 0x00058937
		private int UiSpaceActive { get; set; }

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x060024FA RID: 9466 RVA: 0x0005A740 File Offset: 0x00058940
		// (set) Token: 0x060024FB RID: 9467 RVA: 0x0005A748 File Offset: 0x00058948
		private int UiSpaceInactive { get; set; }

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x060024FC RID: 9468 RVA: 0x0005A751 File Offset: 0x00058951
		// (set) Token: 0x060024FD RID: 9469 RVA: 0x0005A759 File Offset: 0x00058959
		private int UiSpaceTotal { get; set; }

		// Token: 0x060024FE RID: 9470 RVA: 0x0005A762 File Offset: 0x00058962
		private OverheadNameplateManager.NameplatePool GetPool(OverheadNameplateMode mode)
		{
			if (mode != OverheadNameplateMode.WorldSpace)
			{
				return this.m_uiSpace;
			}
			return this.m_worldSpace;
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x0012F664 File Offset: 0x0012D864
		private void Awake()
		{
			this.m_activeNameplates = new List<WorldSpaceOverheadController>(512);
			this.m_worldSpace = new OverheadNameplateManager.NameplatePool(OverheadNameplateMode.WorldSpace);
			this.m_uiSpace = new OverheadNameplateManager.NameplatePool(OverheadNameplateMode.UISpace);
			Options.VideoOptions.NvidiaDLSSEnable.Changed += this.NvidiaDLSSEnableOnChanged;
			Options.VideoOptions.ResolutionScale.Changed += this.ResolutionScaleOnChanged;
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x0012F6C8 File Offset: 0x0012D8C8
		private void Start()
		{
			this.m_worldSpace.Init();
			this.m_uiSpace.Init();
			this.RefreshMode();
			UIManager.UiHiddenChanged += this.UIManagerOnUiHiddenChanged;
			LocalPlayer.LocalPlayerOffensiveTargetChanged += this.LocalPlayerOnLocalPlayerOffensiveTargetChanged;
			LocalPlayer.LocalPlayerDefensiveTargetChanged += this.LocalPlayerOnLocalPlayerDefensiveTargetChanged;
			LocalPlayer.HighestMasteryLevelChanged += this.HighestMasteryLevelChanged;
			PlayerCollectionController.AbilityContentsChanged += this.LocalPlayerOnAbilityContentsChanged;
		}

		// Token: 0x06002501 RID: 9473 RVA: 0x0012F748 File Offset: 0x0012D948
		private void OnDestroy()
		{
			UIManager.UiHiddenChanged -= this.UIManagerOnUiHiddenChanged;
			LocalPlayer.LocalPlayerOffensiveTargetChanged -= this.LocalPlayerOnLocalPlayerOffensiveTargetChanged;
			LocalPlayer.LocalPlayerDefensiveTargetChanged -= this.LocalPlayerOnLocalPlayerDefensiveTargetChanged;
			LocalPlayer.HighestMasteryLevelChanged -= this.HighestMasteryLevelChanged;
			PlayerCollectionController.AbilityContentsChanged -= this.LocalPlayerOnAbilityContentsChanged;
			Options.VideoOptions.NvidiaDLSSEnable.Changed -= this.NvidiaDLSSEnableOnChanged;
			Options.VideoOptions.ResolutionScale.Changed -= this.ResolutionScaleOnChanged;
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0012F7D8 File Offset: 0x0012D9D8
		private void LateUpdate()
		{
			int num = 0;
			for (int i = 0; i < this.m_activeNameplates.Count; i++)
			{
				WorldSpaceOverheadController worldSpaceOverheadController = this.m_activeNameplates[i];
				if (!worldSpaceOverheadController.GameEntity)
				{
					this.GetPool(worldSpaceOverheadController.Mode).ReturnController(worldSpaceOverheadController);
					this.m_activeNameplates.RemoveAt(i);
					i--;
				}
				else if (num < 1 && worldSpaceOverheadController.Mode != this.m_mode)
				{
					GameEntity gameEntity = worldSpaceOverheadController.GameEntity;
					OverheadNameplateManager.NameplatePool pool = this.GetPool(worldSpaceOverheadController.Mode);
					pool.ReturnController(worldSpaceOverheadController);
					pool = this.GetPool(this.m_mode);
					WorldSpaceOverheadController controller = pool.GetController();
					controller.Init(gameEntity);
					controller.gameObject.transform.SetParent(pool.ActiveParent);
					this.m_activeNameplates[i] = controller;
					OverheadNameplateManager.EnableIfWithinRange(controller);
					IOverheadNameplateSpawner overheadNameplate = gameEntity.OverheadNameplate;
					if (overheadNameplate != null)
					{
						overheadNameplate.SetController(controller);
					}
					num++;
				}
				else
				{
					worldSpaceOverheadController.UpdatePosition();
					OverheadNameplateManager.EnableIfWithinRange(worldSpaceOverheadController);
				}
			}
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x0012F8EC File Offset: 0x0012DAEC
		public WorldSpaceOverheadController RequestNameplate(GameEntity entity)
		{
			if (GameManager.IsServer)
			{
				return null;
			}
			OverheadNameplateManager.NameplatePool pool = this.GetPool(this.m_mode);
			WorldSpaceOverheadController controller = pool.GetController();
			if (!controller)
			{
				return null;
			}
			if (controller)
			{
				this.m_activeNameplates.Add(controller);
				controller.Init(entity);
				controller.gameObject.transform.SetParent(pool.ActiveParent);
				OverheadNameplateManager.EnableIfWithinRange(controller);
			}
			return controller;
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0012F958 File Offset: 0x0012DB58
		private static void EnableIfWithinRange(WorldSpaceOverheadController controller)
		{
			if (!controller.gameObject.activeSelf && controller.GameEntity && LocalPlayer.GameEntity && LocalPlayer.GameEntity.Vitals && controller.GameEntity.GetCachedSqrDistanceFromCamera() < LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr)
			{
				controller.gameObject.SetActive(true);
			}
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x0005A774 File Offset: 0x00058974
		private void RefreshMode()
		{
			if (NvidiaDLSS.SupportsNvidiaDLSS && Options.VideoOptions.NvidiaDLSSEnable.Value)
			{
				this.m_mode = OverheadNameplateMode.UISpace;
				return;
			}
			if (Options.VideoOptions.ResolutionScale.Value < 1f)
			{
				this.m_mode = OverheadNameplateMode.UISpace;
				return;
			}
			this.m_mode = OverheadNameplateMode.WorldSpace;
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0005A7B1 File Offset: 0x000589B1
		private void ResolutionScaleOnChanged()
		{
			this.RefreshMode();
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x0005A7B1 File Offset: 0x000589B1
		private void NvidiaDLSSEnableOnChanged()
		{
			this.RefreshMode();
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x0012F9C4 File Offset: 0x0012DBC4
		private void UIManagerOnUiHiddenChanged()
		{
			for (int i = 0; i < this.m_activeNameplates.Count; i++)
			{
				if (this.m_activeNameplates[i])
				{
					this.m_activeNameplates[i].UIManagerOnUiHiddenChanged();
				}
			}
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x0012FA0C File Offset: 0x0012DC0C
		private void LocalPlayerOnLocalPlayerDefensiveTargetChanged(GameEntity obj)
		{
			for (int i = 0; i < this.m_activeNameplates.Count; i++)
			{
				this.m_activeNameplates[i].NameplateController.Nameplate.DefensiveTargetChanged(obj);
			}
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x0012FA4C File Offset: 0x0012DC4C
		private void LocalPlayerOnLocalPlayerOffensiveTargetChanged(GameEntity obj)
		{
			for (int i = 0; i < this.m_activeNameplates.Count; i++)
			{
				this.m_activeNameplates[i].NameplateController.Nameplate.OffensiveTargetChanged(obj);
			}
		}

		// Token: 0x0600250B RID: 9483 RVA: 0x0012FA8C File Offset: 0x0012DC8C
		private void HighestMasteryLevelChanged()
		{
			for (int i = 0; i < this.m_activeNameplates.Count; i++)
			{
				this.m_activeNameplates[i].NameplateController.OnLocalPlayerMasteryLevelChanged();
			}
		}

		// Token: 0x0600250C RID: 9484 RVA: 0x0012FAC8 File Offset: 0x0012DCC8
		private void LocalPlayerOnAbilityContentsChanged()
		{
			for (int i = 0; i < this.m_activeNameplates.Count; i++)
			{
				this.m_activeNameplates[i].InteractiveController.AbilitiesOnContentsChanged();
			}
		}

		// Token: 0x040027B0 RID: 10160
		private const int kInitialPool = 256;

		// Token: 0x040027B1 RID: 10161
		private const int kInitialCapacity = 512;

		// Token: 0x040027B2 RID: 10162
		private List<WorldSpaceOverheadController> m_activeNameplates;

		// Token: 0x040027B3 RID: 10163
		private OverheadNameplateMode m_mode;

		// Token: 0x040027B4 RID: 10164
		private OverheadNameplateManager.NameplatePool m_worldSpace;

		// Token: 0x040027B5 RID: 10165
		private OverheadNameplateManager.NameplatePool m_uiSpace;

		// Token: 0x0200050D RID: 1293
		private class NameplatePool
		{
			// Token: 0x1700078B RID: 1931
			// (get) Token: 0x0600250E RID: 9486 RVA: 0x0005A7B9 File Offset: 0x000589B9
			public Transform ActiveParent
			{
				get
				{
					return this.m_activeParent;
				}
			}

			// Token: 0x1700078C RID: 1932
			// (get) Token: 0x0600250F RID: 9487 RVA: 0x0005A7C1 File Offset: 0x000589C1
			// (set) Token: 0x06002510 RID: 9488 RVA: 0x0005A7C9 File Offset: 0x000589C9
			internal int Active { get; private set; }

			// Token: 0x1700078D RID: 1933
			// (get) Token: 0x06002511 RID: 9489 RVA: 0x0005A7D2 File Offset: 0x000589D2
			internal int Inactive
			{
				get
				{
					if (this.m_pool == null)
					{
						return 0;
					}
					return this.m_pool.Count;
				}
			}

			// Token: 0x06002512 RID: 9490 RVA: 0x0005A7E9 File Offset: 0x000589E9
			public NameplatePool(OverheadNameplateMode mode)
			{
				this.m_mode = mode;
				this.m_pool = new Stack<WorldSpaceOverheadController>(512);
			}

			// Token: 0x06002513 RID: 9491 RVA: 0x0012FB04 File Offset: 0x0012DD04
			public void Init()
			{
				OverheadNameplateMode mode = this.m_mode;
				if (mode != OverheadNameplateMode.WorldSpace)
				{
					if (mode == OverheadNameplateMode.UISpace)
					{
						this.m_poolParent = ClientGameManager.UIManager.UiSpaceNameplatePanel;
						this.m_activeParent = ClientGameManager.UIManager.UiSpaceNameplatePanel;
					}
				}
				else
				{
					this.m_poolParent = new GameObject("POOL_WorldSpaceNameplate").transform;
					this.m_activeParent = new GameObject("ACTIVE_WorldSpaceNameplate").transform;
				}
				for (int i = 0; i < 256; i++)
				{
					WorldSpaceOverheadController newController = this.GetNewController();
					if (newController)
					{
						newController.gameObject.SetActive(false);
						this.m_pool.Push(newController);
					}
				}
				if (this.m_poolParent)
				{
					this.m_poolParent.gameObject.SetActive(true);
				}
			}

			// Token: 0x06002514 RID: 9492 RVA: 0x0012FBC4 File Offset: 0x0012DDC4
			private WorldSpaceOverheadController GetNewController()
			{
				GameObject overheadPrefab = GlobalSettings.Values.Nameplates.GetOverheadPrefab(this.m_mode);
				if (!overheadPrefab)
				{
					return null;
				}
				return UnityEngine.Object.Instantiate<GameObject>(overheadPrefab, this.m_poolParent).GetComponent<WorldSpaceOverheadController>();
			}

			// Token: 0x06002515 RID: 9493 RVA: 0x0005A808 File Offset: 0x00058A08
			public WorldSpaceOverheadController GetController()
			{
				this.Active++;
				if (this.m_pool.Count <= 0)
				{
					return this.GetNewController();
				}
				return this.m_pool.Pop();
			}

			// Token: 0x06002516 RID: 9494 RVA: 0x0012FC04 File Offset: 0x0012DE04
			public void ReturnController(WorldSpaceOverheadController controller)
			{
				if (controller)
				{
					this.Active--;
					controller.gameObject.SetActive(false);
					controller.gameObject.transform.SetParent(this.m_poolParent);
					controller.ResetData();
					if (controller.Mode != this.m_mode)
					{
						throw new ArgumentException("Invalid mode! Returning nameplate to the wrong pool!");
					}
					this.m_pool.Push(controller);
				}
			}

			// Token: 0x040027BC RID: 10172
			private readonly OverheadNameplateMode m_mode;

			// Token: 0x040027BD RID: 10173
			private readonly Stack<WorldSpaceOverheadController> m_pool;

			// Token: 0x040027BE RID: 10174
			private Transform m_poolParent;

			// Token: 0x040027BF RID: 10175
			private Transform m_activeParent;
		}
	}
}
