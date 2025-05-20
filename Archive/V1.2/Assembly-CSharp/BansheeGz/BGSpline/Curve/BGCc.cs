using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000178 RID: 376
	[RequireComponent(typeof(BGCurve))]
	public abstract class BGCc : MonoBehaviour
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000C8B RID: 3211 RVA: 0x000D1BC0 File Offset: 0x000CFDC0
		// (remove) Token: 0x06000C8C RID: 3212 RVA: 0x000D1BF8 File Offset: 0x000CFDF8
		public event EventHandler ChangedParams;

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06000C8D RID: 3213 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual string Info
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual string Warning
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06000C8F RID: 3215 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual string Error
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool SupportHandles
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000C91 RID: 3217 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool SupportHandlesSettings
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool HideHandlesInInspector
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000C93 RID: 3219 RVA: 0x0004B210 File Offset: 0x00049410
		public BGCurve Curve
		{
			get
			{
				if (this.curve == null)
				{
					this.curve = base.GetComponent<BGCurve>();
				}
				return this.curve;
			}
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x0004B232 File Offset: 0x00049432
		public void SetParent(BGCc parent)
		{
			this.parent = parent;
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x0004B23B File Offset: 0x0004943B
		public T GetParent<T>() where T : BGCc
		{
			return (T)((object)this.GetParent(typeof(T)));
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x0004B252 File Offset: 0x00049452
		public BGCc GetParent(Type type)
		{
			if (this.parent != null)
			{
				return this.parent;
			}
			this.parent = (BGCc)base.GetComponent(type);
			return this.parent;
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06000C97 RID: 3223 RVA: 0x000D1C30 File Offset: 0x000CFE30
		// (set) Token: 0x06000C98 RID: 3224 RVA: 0x0004B281 File Offset: 0x00049481
		public string CcName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.ccName))
				{
					return this.ccName;
				}
				return base.GetInstanceID().ToString() ?? "";
			}
			set
			{
				this.ParamChanged<string>(ref this.ccName, value);
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06000C99 RID: 3225 RVA: 0x0004B291 File Offset: 0x00049491
		public BGCc.CcDescriptor Descriptor
		{
			get
			{
				if (this.descriptor == null)
				{
					this.descriptor = BGCc.GetDescriptor(base.GetType());
				}
				return this.descriptor;
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x000D1C68 File Offset: 0x000CFE68
		public virtual string HelpURL
		{
			get
			{
				HelpURLAttribute helpUrl = BGCc.GetHelpUrl(base.GetType());
				if (helpUrl != null)
				{
					return helpUrl.URL;
				}
				return null;
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Start()
		{
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnDestroy()
		{
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x000D1C8C File Offset: 0x000CFE8C
		protected bool ParamChanged<T>(ref T oldValue, T newValue)
		{
			bool flag = oldValue == null;
			bool flag2 = newValue == null;
			if (flag && flag2)
			{
				return false;
			}
			if (flag == flag2 && oldValue.Equals(newValue))
			{
				return false;
			}
			oldValue = newValue;
			this.FireChangedParams();
			return true;
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0004B2B2 File Offset: 0x000494B2
		public bool HasError()
		{
			return !string.IsNullOrEmpty(this.Error);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x0004B2C2 File Offset: 0x000494C2
		public bool HasWarning()
		{
			return !string.IsNullOrEmpty(this.Warning);
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0004B2D2 File Offset: 0x000494D2
		protected string ChoseMessage(string baseError, Func<string> childError)
		{
			if (string.IsNullOrEmpty(baseError))
			{
				return childError();
			}
			return baseError;
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0004B2E4 File Offset: 0x000494E4
		public void FireChangedParams()
		{
			if (this.ChangedParams != null && this.transactionLevel == 0)
			{
				this.ChangedParams(this, null);
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AddedInEditor()
		{
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x0004B303 File Offset: 0x00049503
		public Type GetParentClass()
		{
			return BGCc.GetParentClass(base.GetType());
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x000D1CE4 File Offset: 0x000CFEE4
		public static Type GetParentClass(Type ccType)
		{
			object[] customAttributes = BGReflectionAdapter.GetCustomAttributes(ccType, typeof(RequireComponent), true);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			List<Type> list = new List<Type>();
			foreach (RequireComponent requireComponent in customAttributes)
			{
				BGCc.CheckRequired(requireComponent.m_Type0, list);
				BGCc.CheckRequired(requireComponent.m_Type1, list);
				BGCc.CheckRequired(requireComponent.m_Type2, list);
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count > 1)
			{
				throw new BGCc.CcException(((ccType != null) ? ccType.ToString() : null) + " has more than one parent (extended from BGCc class), calculated by RequireComponent attribute");
			}
			return list[0];
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x0004B310 File Offset: 0x00049510
		private static void CheckRequired(Type type, List<Type> result)
		{
			if (type == null || BGReflectionAdapter.IsAbstract(type) || !BGReflectionAdapter.IsClass(type) || !BGReflectionAdapter.IsSubclassOf(type, typeof(BGCc)))
			{
				return;
			}
			result.Add(type);
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0004B345 File Offset: 0x00049545
		public static bool IsSingle(Type ccType)
		{
			return BGReflectionAdapter.GetCustomAttributes(ccType, typeof(DisallowMultipleComponent), true).Length != 0;
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x000D1D84 File Offset: 0x000CFF84
		public void Transaction(Action action)
		{
			this.transactionLevel++;
			try
			{
				action();
			}
			finally
			{
				this.transactionLevel--;
				if (this.transactionLevel == 0 && this.ChangedParams != null)
				{
					this.ChangedParams(this, null);
				}
			}
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x000D1DE4 File Offset: 0x000CFFE4
		public static BGCc.CcDescriptor GetDescriptor(Type type)
		{
			object[] customAttributes = BGReflectionAdapter.GetCustomAttributes(type, typeof(BGCc.CcDescriptor), false);
			if (customAttributes.Length != 0)
			{
				return (BGCc.CcDescriptor)customAttributes[0];
			}
			return null;
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x000D1E14 File Offset: 0x000D0014
		private static HelpURLAttribute GetHelpUrl(Type type)
		{
			object[] customAttributes = BGReflectionAdapter.GetCustomAttributes(type, typeof(HelpURLAttribute), false);
			if (customAttributes.Length != 0)
			{
				return (HelpURLAttribute)customAttributes[0];
			}
			return null;
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x00045BCA File Offset: 0x00043DCA
		[ContextMenu("Reset", true)]
		[ContextMenu("Copy Component", true)]
		[ContextMenu("Paste Component Values", true)]
		[ContextMenu("Remove Component", true)]
		private bool ContextMenuItems()
		{
			return false;
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x0004B35C File Offset: 0x0004955C
		[ContextMenu("Reset")]
		[ContextMenu("Copy Component")]
		[ContextMenu("Paste Component Values")]
		[ContextMenu("Remove Component")]
		private void ContextMenuValidate()
		{
			BGCc.ShowError("BGCurve components do not support this function");
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x0004B368 File Offset: 0x00049568
		[ContextMenu("BGCurve: Why menu items are disabled?")]
		private void WhyDisabled()
		{
			BGCc.ShowError("BGCurve components do not support Resetting, Copy/Pasting and Removing components from standard Unity menu. Use colored tree view to remove components");
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x0004B374 File Offset: 0x00049574
		private static void ShowError(string message)
		{
			Debug.Log(message);
		}

		// Token: 0x04000BFA RID: 3066
		private BGCurve curve;

		// Token: 0x04000BFB RID: 3067
		[SerializeField]
		private BGCc parent;

		// Token: 0x04000BFC RID: 3068
		[SerializeField]
		private string ccName;

		// Token: 0x04000BFD RID: 3069
		private int transactionLevel;

		// Token: 0x04000BFE RID: 3070
		private BGCc.CcDescriptor descriptor;

		// Token: 0x02000179 RID: 377
		[AttributeUsage(AttributeTargets.Class)]
		public class CcDescriptor : Attribute
		{
			// Token: 0x17000384 RID: 900
			// (get) Token: 0x06000CAF RID: 3247 RVA: 0x0004B37C File Offset: 0x0004957C
			// (set) Token: 0x06000CB0 RID: 3248 RVA: 0x0004B384 File Offset: 0x00049584
			public string Name { get; set; }

			// Token: 0x17000385 RID: 901
			// (get) Token: 0x06000CB1 RID: 3249 RVA: 0x0004B38D File Offset: 0x0004958D
			// (set) Token: 0x06000CB2 RID: 3250 RVA: 0x0004B395 File Offset: 0x00049595
			public string Description { get; set; }

			// Token: 0x17000386 RID: 902
			// (get) Token: 0x06000CB3 RID: 3251 RVA: 0x0004B39E File Offset: 0x0004959E
			// (set) Token: 0x06000CB4 RID: 3252 RVA: 0x0004B3A6 File Offset: 0x000495A6
			public string Image { get; set; }

			// Token: 0x17000387 RID: 903
			// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x0004B3AF File Offset: 0x000495AF
			// (set) Token: 0x06000CB6 RID: 3254 RVA: 0x0004B3B7 File Offset: 0x000495B7
			public string Icon { get; set; }
		}

		// Token: 0x0200017A RID: 378
		[AttributeUsage(AttributeTargets.Class)]
		public class CcExcludeFromMenu : Attribute
		{
		}

		// Token: 0x0200017B RID: 379
		public class CcException : Exception
		{
			// Token: 0x06000CB9 RID: 3257 RVA: 0x0004B3C0 File Offset: 0x000495C0
			public CcException(string message) : base(message)
			{
			}
		}
	}
}
