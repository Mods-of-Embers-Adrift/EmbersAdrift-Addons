using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000588 RID: 1416
	public abstract class IdentifiableCollection<TObject, TContainer> : ScriptableObject, ISerializationCallbackReceiver where TObject : UnityEngine.Object where TContainer : IdentifiableObject<TObject>, new()
	{
		// Token: 0x06002C3F RID: 11327 RVA: 0x00148A2C File Offset: 0x00146C2C
		public UniqueId GetEntryIdByIndex(int index)
		{
			if (this.m_identifiableObjects == null || this.m_identifiableObjects.Length == 0 || index >= this.m_identifiableObjects.Length || this.m_identifiableObjects[index] == null)
			{
				return UniqueId.Empty;
			}
			return this.m_identifiableObjects[index].Id;
		}

		// Token: 0x06002C40 RID: 11328 RVA: 0x0005EB9D File Offset: 0x0005CD9D
		public bool TryGetObject(UniqueId id, out TObject obj)
		{
			if (this.m_lookupDict == null)
			{
				Debug.LogWarning("Lookup Dict was null?!");
				obj = default(TObject);
				return false;
			}
			return this.m_lookupDict.TryGetValue(id, out obj);
		}

		// Token: 0x06002C41 RID: 11329 RVA: 0x0005EBC7 File Offset: 0x0005CDC7
		public IEnumerable<TContainer> GetObjects()
		{
			int num;
			for (int i = 0; i < this.m_identifiableObjects.Length; i = num + 1)
			{
				yield return this.m_identifiableObjects[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x06002C42 RID: 11330 RVA: 0x0004475B File Offset: 0x0004295B
		private void UpdateIdentifiableArray()
		{
		}

		// Token: 0x06002C43 RID: 11331 RVA: 0x0004475B File Offset: 0x0004295B
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x06002C44 RID: 11332 RVA: 0x00148A84 File Offset: 0x00146C84
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_lookupDict = new Dictionary<UniqueId, TObject>(default(UniqueIdComparer));
			if (this.m_identifiableObjects != null)
			{
				for (int i = 0; i < this.m_identifiableObjects.Length; i++)
				{
					this.m_lookupDict.AddOrReplace(this.m_identifiableObjects[i].Id, this.m_identifiableObjects[i].Obj);
				}
			}
		}

		// Token: 0x04002C03 RID: 11267
		[SerializeField]
		private List<TObject> m_objects;

		// Token: 0x04002C04 RID: 11268
		[SerializeField]
		private TContainer[] m_identifiableObjects;

		// Token: 0x04002C05 RID: 11269
		private Dictionary<UniqueId, TObject> m_lookupDict;
	}
}
