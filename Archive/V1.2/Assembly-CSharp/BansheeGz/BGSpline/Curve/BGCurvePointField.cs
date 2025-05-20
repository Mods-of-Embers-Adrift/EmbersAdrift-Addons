using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x0200019D RID: 413
	[Serializable]
	public class BGCurvePointField : MonoBehaviour
	{
		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000E44 RID: 3652 RVA: 0x0004C501 File Offset: 0x0004A701
		// (set) Token: 0x06000E45 RID: 3653 RVA: 0x000DA024 File Offset: 0x000D8224
		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
			set
			{
				if (string.Equals(this.FieldName, value))
				{
					return;
				}
				BGCurvePointField.CheckName(this.curve, value, true);
				this.curve.FireBeforeChange("field name is changed");
				this.fieldName = value;
				this.curve.PrivateUpdateFieldsValuesIndexes();
				this.curve.FireChange(BGCurveChangedArgs.GetInstance(this.curve, BGCurveChangedArgs.ChangeTypeEnum.Fields, "field name is changed"), false, this);
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000E46 RID: 3654 RVA: 0x0004C509 File Offset: 0x0004A709
		public BGCurvePointField.TypeEnum Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0004C511 File Offset: 0x0004A711
		public BGCurve Curve
		{
			get
			{
				return this.curve;
			}
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x0004C519 File Offset: 0x0004A719
		public void Init(BGCurve curve, string fieldName, BGCurvePointField.TypeEnum type)
		{
			if (!string.IsNullOrEmpty(this.fieldName))
			{
				throw new UnityException("You can not init twice.");
			}
			BGCurvePointField.CheckName(curve, fieldName, true);
			this.curve = curve;
			this.fieldName = fieldName;
			this.type = type;
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x000DA090 File Offset: 0x000D8290
		public static string CheckName(BGCurve curve, string name, bool throwException = false)
		{
			string text = null;
			if (string.IsNullOrEmpty(name))
			{
				text = "Field's name can not be null";
			}
			else if (name.Length > 16)
			{
				text = "Name should be 16 chars max. Current name has " + name.Length.ToString() + " chars.";
			}
			else
			{
				char c = name[0];
				if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))
				{
					text = "Name should start with a English letter.";
				}
				else
				{
					for (int i = 1; i < name.Length; i++)
					{
						char c2 = name[i];
						if ((c2 < 'A' || c2 > 'Z') && (c2 < 'a' || c2 > 'z') && (c2 < '0' || c2 > '9'))
						{
							text = "Name should contain English letters or numbers only.";
							break;
						}
					}
					if (text == null && curve.HasField(name))
					{
						text = "Field with name '" + name + "' already exists.";
					}
				}
			}
			if (throwException && text != null)
			{
				throw new UnityException(text);
			}
			return text;
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x0004C551 File Offset: 0x0004A751
		protected bool Equals(BGCurvePointField other)
		{
			return object.Equals(this.curve, other.curve) && string.Equals(this.fieldName, other.fieldName);
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x0004C579 File Offset: 0x0004A779
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((BGCurvePointField)obj)));
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x0004C5A7 File Offset: 0x0004A7A7
		public override int GetHashCode()
		{
			return ((this.curve != null) ? this.curve.GetHashCode() : 0) ^ ((this.fieldName != null) ? this.fieldName.GetHashCode() : 0);
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x0004C501 File Offset: 0x0004A701
		public override string ToString()
		{
			return this.fieldName;
		}

		// Token: 0x04000CDF RID: 3295
		[SerializeField]
		private BGCurve curve;

		// Token: 0x04000CE0 RID: 3296
		[SerializeField]
		private string fieldName;

		// Token: 0x04000CE1 RID: 3297
		[SerializeField]
		private BGCurvePointField.TypeEnum type;

		// Token: 0x0200019E RID: 414
		public enum TypeEnum
		{
			// Token: 0x04000CE3 RID: 3299
			Bool,
			// Token: 0x04000CE4 RID: 3300
			Int,
			// Token: 0x04000CE5 RID: 3301
			Float,
			// Token: 0x04000CE6 RID: 3302
			String,
			// Token: 0x04000CE7 RID: 3303
			Vector3 = 100,
			// Token: 0x04000CE8 RID: 3304
			Bounds,
			// Token: 0x04000CE9 RID: 3305
			Color,
			// Token: 0x04000CEA RID: 3306
			Quaternion,
			// Token: 0x04000CEB RID: 3307
			AnimationCurve = 200,
			// Token: 0x04000CEC RID: 3308
			GameObject,
			// Token: 0x04000CED RID: 3309
			Component,
			// Token: 0x04000CEE RID: 3310
			BGCurve = 300,
			// Token: 0x04000CEF RID: 3311
			BGCurvePointComponent,
			// Token: 0x04000CF0 RID: 3312
			BGCurvePointGO
		}
	}
}
