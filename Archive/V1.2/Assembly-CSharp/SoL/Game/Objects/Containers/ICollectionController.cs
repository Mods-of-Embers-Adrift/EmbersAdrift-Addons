using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.HuntingLog;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests;
using SoL.Networking.Database;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A12 RID: 2578
	public interface ICollectionController
	{
		// Token: 0x17001159 RID: 4441
		// (get) Token: 0x06004EC6 RID: 20166
		// (set) Token: 0x06004EC7 RID: 20167
		BaseNetworkInteractiveStation InteractiveStation { get; set; }

		// Token: 0x1700115A RID: 4442
		// (get) Token: 0x06004EC8 RID: 20168
		// (set) Token: 0x06004EC9 RID: 20169
		InteractiveRefinementStation RefinementStation { get; set; }

		// Token: 0x1700115B RID: 4443
		// (get) Token: 0x06004ECA RID: 20170
		// (set) Token: 0x06004ECB RID: 20171
		IGatheringNode GatheringNode { get; set; }

		// Token: 0x1700115C RID: 4444
		// (get) Token: 0x06004ECC RID: 20172
		ContainerInstance Masteries { get; }

		// Token: 0x1700115D RID: 4445
		// (get) Token: 0x06004ECD RID: 20173
		ContainerInstance Abilities { get; }

		// Token: 0x1700115E RID: 4446
		// (get) Token: 0x06004ECE RID: 20174
		ContainerInstance Equipment { get; }

		// Token: 0x1700115F RID: 4447
		// (get) Token: 0x06004ECF RID: 20175
		ContainerInstance Inventory { get; }

		// Token: 0x17001160 RID: 4448
		// (get) Token: 0x06004ED0 RID: 20176
		ContainerInstance Gathering { get; }

		// Token: 0x17001161 RID: 4449
		// (get) Token: 0x06004ED1 RID: 20177
		ContainerInstance Pouch { get; }

		// Token: 0x17001162 RID: 4450
		// (get) Token: 0x06004ED2 RID: 20178
		ContainerInstance ReagentPouch { get; }

		// Token: 0x17001163 RID: 4451
		// (get) Token: 0x06004ED3 RID: 20179
		ContainerInstance PersonalBank { get; }

		// Token: 0x17001164 RID: 4452
		// (get) Token: 0x06004ED4 RID: 20180
		ContainerInstance LostAndFound { get; }

		// Token: 0x17001165 RID: 4453
		// (get) Token: 0x06004ED5 RID: 20181
		ContainerInstance RemoteContainer { get; }

		// Token: 0x17001166 RID: 4454
		// (get) Token: 0x06004ED6 RID: 20182
		LearnableContainerInstance Recipes { get; }

		// Token: 0x17001167 RID: 4455
		// (get) Token: 0x06004ED7 RID: 20183
		LearnableContainerInstance Emotes { get; }

		// Token: 0x17001168 RID: 4456
		// (get) Token: 0x06004ED8 RID: 20184
		LearnableContainerInstance Titles { get; }

		// Token: 0x17001169 RID: 4457
		// (get) Token: 0x06004ED9 RID: 20185
		CharacterRecord Record { get; }

		// Token: 0x1700116A RID: 4458
		// (get) Token: 0x06004EDA RID: 20186
		GameEntity GameEntity { get; }

		// Token: 0x1700116B RID: 4459
		// (get) Token: 0x06004EDB RID: 20187
		// (set) Token: 0x06004EDC RID: 20188
		UniqueId? TradeId { get; set; }

		// Token: 0x06004EDD RID: 20189
		bool TryGetInstance(string containerId, out ContainerInstance containerInstance);

		// Token: 0x06004EDE RID: 20190
		bool TryGetInstance(ContainerType containerType, out ContainerInstance containerInstance);

		// Token: 0x06004EDF RID: 20191
		bool TryGetLearnableInstance(string containerId, out LearnableContainerInstance containerInstance);

		// Token: 0x06004EE0 RID: 20192
		bool TryGetLearnableInstance(ContainerType containerType, out LearnableContainerInstance containerInstance);

		// Token: 0x06004EE1 RID: 20193
		void Initialize(CharacterRecord record);

		// Token: 0x06004EE2 RID: 20194
		void UpdateOutgoingCurrency(ContainerType containerType);

		// Token: 0x06004EE3 RID: 20195
		void RefreshBuybackItems();

		// Token: 0x1700116C RID: 4460
		// (get) Token: 0x06004EE4 RID: 20196
		// (set) Token: 0x06004EE5 RID: 20197
		EmberStone CurrentEmberStone { get; set; }

		// Token: 0x06004EE6 RID: 20198
		int GetEmberEssenceCount();

		// Token: 0x06004EE7 RID: 20199
		void AdjustEmberEssenceCount(int delta);

		// Token: 0x06004EE8 RID: 20200
		void AdjustTravelEssenceCount(int delta);

		// Token: 0x06004EE9 RID: 20201
		int GetAvailableEmberEssenceForTravel();

		// Token: 0x06004EEA RID: 20202
		int GetDisplayValueForTravelEssence();

		// Token: 0x06004EEB RID: 20203
		ValueTuple<int, int> GetEmberAndTravelEssenceCounts();

		// Token: 0x06004EEC RID: 20204
		void PurchaseTravelEssence(int travelToAdd, int essenceCost);

		// Token: 0x140000FE RID: 254
		// (add) Token: 0x06004EED RID: 20205
		// (remove) Token: 0x06004EEE RID: 20206
		event Action EmberStoneChanged;

		// Token: 0x06004EEF RID: 20207
		void InvokeHuntingLogEntryRemoved();

		// Token: 0x06004EF0 RID: 20208
		void InvokeHuntingLogEntryModified();

		// Token: 0x06004EF1 RID: 20209
		bool IncrementHuntingLog(HuntingLogProfile profile, int npcLevel);

		// Token: 0x140000FF RID: 255
		// (add) Token: 0x06004EF2 RID: 20210
		// (remove) Token: 0x06004EF3 RID: 20211
		event Action HuntingLogUpdated;

		// Token: 0x1700116D RID: 4461
		// (get) Token: 0x06004EF4 RID: 20212
		List<BBTask> LocalTaskDiscard { get; }

		// Token: 0x06004EF5 RID: 20213
		void ModifyEventCurrency(ulong delta, bool removing);
	}
}
