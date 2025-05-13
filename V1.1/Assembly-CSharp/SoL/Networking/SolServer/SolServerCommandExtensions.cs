using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003EF RID: 1007
	public static class SolServerCommandExtensions
	{
		// Token: 0x06001AC8 RID: 6856 RVA: 0x00109D44 File Offset: 0x00107F44
		public static T DeserializeKey<T>(this SolServerCommand cmd, string key)
		{
			if (!cmd.Args.ContainsKey(key))
			{
				Debug.LogError("Unable to find key: " + key);
				return default(T);
			}
			return JsonConvert.DeserializeObject<T>(cmd.Args[key].ToString());
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x00109D90 File Offset: 0x00107F90
		public static bool TryDeserializeKey<T>(this SolServerCommand cmd, string key, out T value)
		{
			value = default(T);
			bool result;
			try
			{
				object obj;
				if (cmd.Args.TryGetValue(key, out obj))
				{
					string value2 = obj.ToString();
					value = JsonConvert.DeserializeObject<T>(value2);
					result = true;
				}
				else
				{
					Debug.LogWarning("Unable to find key: " + key);
					result = false;
				}
			}
			catch
			{
				Debug.LogError(string.Format("Failed to deserialize!  key: {0}, type: {1}", key, typeof(T)));
				result = false;
			}
			return result;
		}
	}
}
