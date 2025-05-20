using System;
using TMPro;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000301 RID: 769
	public class WarpTextMeshPro : MonoBehaviour
	{
		// Token: 0x060015A4 RID: 5540 RVA: 0x000FD078 File Offset: 0x000FB278
		public void WarpText()
		{
			if (!this.m_label)
			{
				return;
			}
			this.m_label.havePropertiesChanged = true;
			this.m_label.ForceMeshUpdate(false, false);
			TMP_TextInfo textInfo = this.m_label.textInfo;
			if (textInfo.characterCount <= 0)
			{
				return;
			}
			float x = this.m_label.bounds.min.x;
			float x2 = this.m_label.bounds.max.x;
			this.m_vertexCurve.preWrapMode = WrapMode.Once;
			this.m_vertexCurve.postWrapMode = WrapMode.Once;
			for (int i = 0; i < textInfo.characterCount; i++)
			{
				if (textInfo.characterInfo[i].isVisible)
				{
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
					Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
					Vector3 vector = new Vector3((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine, 0f);
					vertices[vertexIndex] += -vector;
					vertices[vertexIndex + 1] += -vector;
					vertices[vertexIndex + 2] += -vector;
					vertices[vertexIndex + 3] += -vector;
					float num = (vector.x - x) / (x2 - x);
					float num2 = num + 0.0001f;
					float y = this.m_vertexCurve.Evaluate(num) * this.m_curveScale;
					float y2 = this.m_vertexCurve.Evaluate(num2) * this.m_curveScale;
					Vector3 lhs = new Vector3(1f, 0f, 0f);
					Vector3 rhs = new Vector3(num2 * (x2 - x) + x, y2) - new Vector3(vector.x, y);
					float num3 = Mathf.Acos(Vector3.Dot(lhs, rhs.normalized)) * 57.29578f;
					float z = (Vector3.Cross(lhs, rhs).z > 0f) ? num3 : (360f - num3);
					Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.Euler(0f, 0f, z), Vector3.one);
					vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
					vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
					vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
					vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
					vertices[vertexIndex] += vector;
					vertices[vertexIndex + 1] += vector;
					vertices[vertexIndex + 2] += vector;
					vertices[vertexIndex + 3] += vector;
				}
			}
			this.m_label.UpdateVertexData();
		}

		// Token: 0x04001DA6 RID: 7590
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04001DA7 RID: 7591
		[SerializeField]
		private AnimationCurve m_vertexCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.25f, 2f),
			new Keyframe(0.5f, 0f),
			new Keyframe(0.75f, 2f),
			new Keyframe(1f, 0f)
		});

		// Token: 0x04001DA8 RID: 7592
		[SerializeField]
		private float m_curveScale = 1f;
	}
}
