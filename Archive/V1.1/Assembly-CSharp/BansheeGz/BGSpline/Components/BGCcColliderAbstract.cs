using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001AA RID: 426
	[DisallowMultipleComponent]
	public abstract class BGCcColliderAbstract<T> : BGCcSplitterPolyline, BGCcColliderI where T : Component
	{
		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06000F09 RID: 3849 RVA: 0x0004CBE0 File Offset: 0x0004ADE0
		// (set) Token: 0x06000F0A RID: 3850 RVA: 0x0004CBE8 File Offset: 0x0004ADE8
		public bool ShowIfNotSelected
		{
			get
			{
				return this.showIfNotSelected;
			}
			set
			{
				this.showIfNotSelected = value;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000F0B RID: 3851 RVA: 0x0004CBF1 File Offset: 0x0004ADF1
		// (set) Token: 0x06000F0C RID: 3852 RVA: 0x0004CBF9 File Offset: 0x0004ADF9
		public Color CollidersColor
		{
			get
			{
				return this.collidersColor;
			}
			set
			{
				this.collidersColor = value;
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000F0D RID: 3853 RVA: 0x0004CC02 File Offset: 0x0004AE02
		public override string Error
		{
			get
			{
				return base.ChoseMessage(base.Error, delegate
				{
					if (!this.maxExceeded)
					{
						return null;
					}
					return "Maximum number of colliders (" + this.maxNumberOfColliders.ToString() + ") is exceeded. Increase maximum to get rid of this error.";
				});
			}
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0004CC1C File Offset: 0x0004AE1C
		// (set) Token: 0x06000F0F RID: 3855 RVA: 0x0004CC24 File Offset: 0x0004AE24
		public bool InheritLayer
		{
			get
			{
				return this.inheritLayer;
			}
			set
			{
				base.ParamChanged<bool>(ref this.inheritLayer, value);
			}
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0004CC34 File Offset: 0x0004AE34
		// (set) Token: 0x06000F11 RID: 3857 RVA: 0x0004CC3C File Offset: 0x0004AE3C
		public int Layer
		{
			get
			{
				return this.layer;
			}
			set
			{
				base.ParamChanged<int>(ref this.layer, value);
			}
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0004CC4C File Offset: 0x0004AE4C
		// (set) Token: 0x06000F13 RID: 3859 RVA: 0x0004CC54 File Offset: 0x0004AE54
		public int MaxNumberOfColliders
		{
			get
			{
				return this.maxNumberOfColliders;
			}
			set
			{
				base.ParamChanged<int>(ref this.maxNumberOfColliders, value);
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0004CC64 File Offset: 0x0004AE64
		// (set) Token: 0x06000F15 RID: 3861 RVA: 0x000DBE98 File Offset: 0x000DA098
		public GameObject ChildPrefab
		{
			get
			{
				return this.childPrefab;
			}
			set
			{
				if (base.ParamChanged<GameObject>(ref this.childPrefab, value) && this.RequireGameObjects)
				{
					List<T> list = null;
					try
					{
						list = this.WorkingList;
						this.FillChildrenColliders(list);
						foreach (T t in list)
						{
							BGCurve.DestroyIt(t.gameObject);
						}
						this.UpdateUi();
					}
					finally
					{
						if (list != null)
						{
							list.Clear();
						}
					}
				}
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0004479C File Offset: 0x0004299C
		public virtual bool RequireGameObjects
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06000F17 RID: 3863
		protected abstract List<T> WorkingList { get; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool LocalSpace
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0004CC6C File Offset: 0x0004AE6C
		public override void AddedInEditor()
		{
			this.UpdateUi();
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x000DBF34 File Offset: 0x000DA134
		public virtual void UpdateUi()
		{
			bool requireGameObjects = this.RequireGameObjects;
			List<T> workingList = this.WorkingList;
			if (requireGameObjects)
			{
				this.FillChildrenColliders(workingList);
			}
			if (this.LocalSpace && !this.UseLocal)
			{
				this.useLocal = true;
				this.dataValid = false;
			}
			List<Vector3> positions = base.Positions;
			int count = positions.Count;
			this.maxExceeded = false;
			int num = 0;
			if (count > 1)
			{
				int num2 = Mathf.Min(this.maxNumberOfColliders + 1, count);
				if (this.maxNumberOfColliders + 1 < count)
				{
					this.maxExceeded = true;
				}
				if (requireGameObjects)
				{
					bool isPlaying = Application.isPlaying;
					string str = base.gameObject.name + " Collider[";
					int num3 = base.gameObject.layer;
					for (int i = 1; i < num2; i++)
					{
						Component component;
						if (workingList.Count <= num)
						{
							if (this.childPrefab == null)
							{
								component = new GameObject
								{
									transform = 
									{
										parent = base.transform
									}
								}.AddComponent<T>();
							}
							else
							{
								GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childPrefab, base.transform);
								component = gameObject.GetComponent<T>();
								if (component == null)
								{
									component = gameObject.AddComponent<T>();
								}
							}
						}
						else
						{
							component = workingList[num];
						}
						this.CheckCollider(component);
						component.gameObject.layer = (this.inheritLayer ? num3 : this.layer);
						if (!isPlaying)
						{
							try
							{
								component.gameObject.name = str + (i - 1).ToString() + "]";
							}
							catch (MissingReferenceException)
							{
								return;
							}
						}
						this.SetUpGoCollider((T)((object)component), positions[i - 1], positions[i]);
						num++;
					}
				}
				else
				{
					this.FillSingleCollider(positions, num2);
				}
			}
			if (requireGameObjects)
			{
				int count2 = workingList.Count;
				if (num < count2)
				{
					for (int j = count2 - 1; j >= num; j--)
					{
						T t = workingList[j];
						workingList.RemoveAt(j);
						BGCurve.DestroyIt(t.gameObject);
					}
				}
				workingList.Clear();
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x000DC168 File Offset: 0x000DA368
		public void FillChildrenColliders(List<T> resultList)
		{
			resultList.Clear();
			try
			{
				base.GetComponentsInChildren<T>(resultList);
			}
			catch (MissingReferenceException)
			{
				return;
			}
			int count = resultList.Count;
			if (count == 0)
			{
				return;
			}
			Transform transform = base.transform;
			for (int i = count - 1; i >= 0; i--)
			{
				if (resultList[i].transform.parent != transform)
				{
					resultList.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void CheckCollider(Component component)
		{
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetUpGoCollider(T collider, Vector3 from, Vector3 to)
		{
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void FillSingleCollider(List<Vector3> positions, int count)
		{
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0004CC74 File Offset: 0x0004AE74
		protected override void UpdateRequested(object sender, EventArgs e)
		{
			base.UpdateRequested(sender, e);
			this.UpdateUi();
		}

		// Token: 0x04000D1F RID: 3359
		[SerializeField]
		[Tooltip("Show even if not selected")]
		private bool showIfNotSelected;

		// Token: 0x04000D20 RID: 3360
		[SerializeField]
		[Tooltip("Colliders color then showing not selected")]
		private Color collidersColor = Color.white;

		// Token: 0x04000D21 RID: 3361
		[SerializeField]
		[Tooltip("Maximum number of colliders")]
		private int maxNumberOfColliders = 200;

		// Token: 0x04000D22 RID: 3362
		[SerializeField]
		private bool maxExceeded;

		// Token: 0x04000D23 RID: 3363
		[SerializeField]
		[Tooltip("Assign the layer of this GameObject to children colliders")]
		private bool inheritLayer;

		// Token: 0x04000D24 RID: 3364
		[SerializeField]
		[Tooltip("The layer of children GameObjects")]
		private int layer;

		// Token: 0x04000D25 RID: 3365
		[SerializeField]
		[Tooltip("The prefab to use for children objects")]
		private GameObject childPrefab;
	}
}
