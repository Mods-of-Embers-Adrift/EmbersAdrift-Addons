using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoL.Utilities;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD4 RID: 3540
	public class SkeletonDeltaCalculator : MonoBehaviour
	{
		// Token: 0x06006964 RID: 26980 RVA: 0x00217708 File Offset: 0x00215908
		private void AssignSmallLarge()
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.gameObject.name.Contains("Small"))
				{
					this.m_smallerObject = transform.gameObject;
				}
				else if (transform.gameObject.name.Contains("Large"))
				{
					this.m_largerObject = transform.gameObject;
				}
			}
		}

		// Token: 0x06006965 RID: 26981 RVA: 0x002177A4 File Offset: 0x002159A4
		private void Compare()
		{
			if (this.m_data == null)
			{
				return;
			}
			this.m_compareDict = new Dictionary<string, Transform>();
			Transform[] componentsInChildren = this.m_sourceObject.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.m_compareDict.Add(componentsInChildren[i].name, componentsInChildren[i]);
			}
			this.m_resultDict = new Dictionary<string, ComparisonResult>();
			this.CompareObjects(true);
			this.CompareObjects(false);
			List<ComparisonResult> list = this.m_resultDict.Values.ToList<ComparisonResult>();
			list.Sort(new Comparison<ComparisonResult>(this.Comparison));
			this.m_data.Data = new List<ComparisonResult>(list);
		}

		// Token: 0x06006966 RID: 26982 RVA: 0x00086AD9 File Offset: 0x00084CD9
		private int Comparison(ComparisonResult x, ComparisonResult y)
		{
			return string.CompareOrdinal(x.BoneName, y.BoneName);
		}

		// Token: 0x06006967 RID: 26983 RVA: 0x00217848 File Offset: 0x00215A48
		private void CompareObjects(bool isSmall)
		{
			Transform[] componentsInChildren = (isSmall ? this.m_smallerObject : this.m_largerObject).GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Transform transform;
				if (this.m_compareDict.TryGetValue(componentsInChildren[i].name, out transform))
				{
					ComparisonValues comparisonForTransforms = this.GetComparisonForTransforms(transform, componentsInChildren[i], isSmall);
					if (componentsInChildren[i].name == "Spine1")
					{
						comparisonForTransforms.PosDelta == Vector3.zero;
						comparisonForTransforms.SizeDelta == Vector3.zero;
					}
					if (!(comparisonForTransforms.PosDelta == Vector3.zero) || !(comparisonForTransforms.SizeDelta == Vector3.zero))
					{
						ComparisonResult comparisonResult;
						if (!this.m_resultDict.TryGetValue(transform.name, out comparisonResult))
						{
							comparisonResult = new ComparisonResult
							{
								BoneName = transform.name
							};
							this.m_resultDict.Add(transform.name, comparisonResult);
						}
						if (isSmall)
						{
							comparisonResult.Smaller = comparisonForTransforms;
						}
						else
						{
							comparisonResult.Bigger = comparisonForTransforms;
						}
					}
				}
			}
		}

		// Token: 0x06006968 RID: 26984 RVA: 0x00217950 File Offset: 0x00215B50
		private ComparisonValues GetComparisonForTransforms(Transform sourceTrans, Transform compareTrans, bool isSmall)
		{
			Vector3 b = sourceTrans.localPosition;
			Vector3 a = compareTrans.localPosition;
			Quaternion lhs = sourceTrans.localRotation;
			Quaternion rotation = compareTrans.localRotation;
			Vector3 vector = sourceTrans.localScale;
			Vector3 vector2 = compareTrans.localScale;
			if (this.m_useWorldSpace)
			{
				b = sourceTrans.position;
				a = compareTrans.position;
				lhs = sourceTrans.rotation;
				rotation = compareTrans.rotation;
				vector = sourceTrans.lossyScale;
				vector2 = compareTrans.lossyScale;
			}
			Vector3 pos = a - b;
			Quaternion quaternion = lhs * Quaternion.Inverse(rotation);
			Vector3 size = new Vector3(vector2.x / vector.x - 1f, vector2.y / vector.y - 1f, vector2.z / vector.z - 1f);
			return new ComparisonValues(pos, quaternion.eulerAngles, size);
		}

		// Token: 0x06006969 RID: 26985 RVA: 0x00086AEC File Offset: 0x00084CEC
		private IEnumerable GetDataFile()
		{
			return SolOdinUtilities.GetDropdownItems<SkeletonDeltaData>();
		}

		// Token: 0x04005BD5 RID: 23509
		private const string kFormatSpecifier = "F04";

		// Token: 0x04005BD6 RID: 23510
		[SerializeField]
		private SkeletonDeltaData m_data;

		// Token: 0x04005BD7 RID: 23511
		[SerializeField]
		private bool m_useWorldSpace;

		// Token: 0x04005BD8 RID: 23512
		[SerializeField]
		private GameObject m_sourceObject;

		// Token: 0x04005BD9 RID: 23513
		[SerializeField]
		private GameObject m_smallerObject;

		// Token: 0x04005BDA RID: 23514
		[SerializeField]
		private GameObject m_largerObject;

		// Token: 0x04005BDB RID: 23515
		private Dictionary<string, Transform> m_compareDict;

		// Token: 0x04005BDC RID: 23516
		private Dictionary<string, ComparisonResult> m_resultDict;
	}
}
