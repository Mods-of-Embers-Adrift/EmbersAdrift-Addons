using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Subscription
{
	// Token: 0x020003AB RID: 939
	[CreateAssetMenu(menuName = "SoL/Subscription Perks")]
	public class ScriptableSubscriptionPerks : ScriptableObject
	{
		// Token: 0x0600199B RID: 6555 RVA: 0x00054137 File Offset: 0x00052337
		private void PrintToJson()
		{
			Debug.Log(JsonConvert.SerializeObject(this.Perks, Formatting.None, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			}));
		}

		// Token: 0x0400208A RID: 8330
		[FormerlySerializedAs("Benefits")]
		public SubscriptionPerk[] Perks;
	}
}
