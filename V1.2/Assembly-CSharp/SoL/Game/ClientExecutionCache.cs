using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Networking;
using SoL.Networking.Managers;
using SoL.Networking.Objects;

namespace SoL.Game
{
	// Token: 0x020005B9 RID: 1465
	public struct ClientExecutionCache : INetworkSerializable
	{
		// Token: 0x06002E30 RID: 11824 RVA: 0x00151A8C File Offset: 0x0014FC8C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddEnum(this.SourceType);
			buffer.AddEnum(this.AlchemyPowerLevel);
			buffer.AddBool(this.TargetEntity != null);
			if (this.TargetEntity != null)
			{
				buffer.AddUInt(this.TargetEntity.NetworkId.Value);
			}
			buffer.AddNullableUniqueId(this.RecipeId);
			List<ItemUsageSerializable> itemsUsed = this.ItemsUsed;
			buffer.AddNullableInt((itemsUsed != null) ? new int?(itemsUsed.Count) : null);
			if (this.ItemsUsed != null)
			{
				foreach (ItemUsageSerializable itemUsageSerializable in this.ItemsUsed)
				{
					itemUsageSerializable.PackData(buffer);
				}
			}
			buffer.AddNullableInt(this.TargetAbilityLevel);
			buffer.AddNullableUniqueId(this.InstanceId);
			buffer.AddNullableString(this.ContainerId);
			buffer.AddBool(this.UseTargetAtExecutionComplete);
			return buffer;
		}

		// Token: 0x06002E31 RID: 11825 RVA: 0x00151BB4 File Offset: 0x0014FDB4
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.SourceType = buffer.ReadEnum<EffectSourceType>();
			this.AlchemyPowerLevel = buffer.ReadEnum<AlchemyPowerLevel>();
			if (buffer.ReadBool())
			{
				uint id = buffer.ReadUInt();
				this.TargetEntity = NetworkManager.EntityManager.GetNetEntityForId(id);
			}
			this.RecipeId = buffer.ReadNullableUniqueId();
			int? num = buffer.ReadNullableInt();
			if (num != null)
			{
				int? num2 = num;
				int num3 = 0;
				if (num2.GetValueOrDefault() > num3 & num2 != null)
				{
					this.ItemsUsed = new List<ItemUsageSerializable>(num.Value);
					int num4 = 0;
					for (;;)
					{
						int num5 = num4;
						num2 = num;
						if (!(num5 < num2.GetValueOrDefault() & num2 != null))
						{
							break;
						}
						ItemUsageSerializable item = default(ItemUsageSerializable);
						item.ReadData(buffer);
						this.ItemsUsed.Add(item);
						num4++;
					}
				}
			}
			this.TargetAbilityLevel = buffer.ReadNullableInt();
			this.InstanceId = buffer.ReadNullableUniqueId();
			this.ContainerId = buffer.ReadNullableString();
			this.UseTargetAtExecutionComplete = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x04002DB7 RID: 11703
		public UniqueId ArchetypeId;

		// Token: 0x04002DB8 RID: 11704
		public NetworkEntity TargetEntity;

		// Token: 0x04002DB9 RID: 11705
		public EffectSourceType SourceType;

		// Token: 0x04002DBA RID: 11706
		public AlchemyPowerLevel AlchemyPowerLevel;

		// Token: 0x04002DBB RID: 11707
		public UniqueId? RecipeId;

		// Token: 0x04002DBC RID: 11708
		public List<ItemUsageSerializable> ItemsUsed;

		// Token: 0x04002DBD RID: 11709
		public int? TargetAbilityLevel;

		// Token: 0x04002DBE RID: 11710
		public UniqueId? InstanceId;

		// Token: 0x04002DBF RID: 11711
		public string ContainerId;

		// Token: 0x04002DC0 RID: 11712
		public bool UseTargetAtExecutionComplete;
	}
}
