using System;
using LudeonTK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public class WorldFeatureTextMesh_TextMeshPro : WorldFeatureTextMesh
{
	private TextMeshPro textMesh;

	public static readonly GameObject WorldTextPrefab = Resources.Load<GameObject>("Prefabs/WorldText");

	[TweakValue("Interface.World", 0f, 5f)]
	private static float TextScale = 1f;

	public override bool Active => ((Component)(object)textMesh).gameObject.activeInHierarchy;

	public override Vector3 Position => textMesh.transform.position;

	public override Color Color
	{
		get
		{
			return ((UnityEngine.UI.Graphic)(object)textMesh).color;
		}
		set
		{
			((UnityEngine.UI.Graphic)(object)textMesh).color = value;
		}
	}

	public override string Text
	{
		get
		{
			return ((TMP_Text)textMesh).text;
		}
		set
		{
			((TMP_Text)textMesh).text = value;
		}
	}

	public override float Size
	{
		set
		{
			((TMP_Text)textMesh).fontSize = value * TextScale;
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			return textMesh.transform.rotation;
		}
		set
		{
			textMesh.transform.rotation = value;
		}
	}

	public override Vector3 LocalPosition
	{
		get
		{
			return textMesh.transform.localPosition;
		}
		set
		{
			textMesh.transform.localPosition = value;
		}
	}

	private static void TextScale_Changed()
	{
		Find.WorldFeatures.textsCreated = false;
	}

	public override void SetActive(bool active)
	{
		((Component)(object)textMesh).gameObject.SetActive(active);
	}

	public override void Destroy()
	{
		UnityEngine.Object.Destroy(((Component)(object)textMesh).gameObject);
	}

	public override void Init()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(WorldTextPrefab);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		textMesh = gameObject.GetComponent<TextMeshPro>();
		Color = new Color(1f, 1f, 1f, 0f);
		Material[] sharedMaterials = ((Component)(object)textMesh).GetComponent<MeshRenderer>().sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			sharedMaterials[i].renderQueue = 3610;
		}
	}

	public override void WrapAroundPlanetSurface(PlanetLayer layer)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)textMesh).ForceMeshUpdate(false, false);
		TMP_TextInfo textInfo = ((TMP_Text)textMesh).textInfo;
		int characterCount = textInfo.characterCount;
		if (characterCount == 0)
		{
			return;
		}
		float num = ((TMP_Text)textMesh).bounds.extents.x * 2f;
		float num2 = layer.DistOnSurfaceToAngle(num);
		Matrix4x4 localToWorldMatrix = textMesh.transform.localToWorldMatrix;
		Matrix4x4 worldToLocalMatrix = textMesh.transform.worldToLocalMatrix;
		for (int i = 0; i < characterCount; i++)
		{
			TMP_CharacterInfo val = textInfo.characterInfo[i];
			if (val.isVisible)
			{
				int materialReferenceIndex = ((TMP_Text)textMesh).textInfo.characterInfo[i].materialReferenceIndex;
				int vertexIndex = val.vertexIndex;
				Vector3 vector = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3];
				vector /= 4f;
				float num3 = vector.x / (num / 2f);
				bool flag = num3 >= 0f;
				num3 = Mathf.Abs(num3);
				float num4 = num2 / 2f * num3;
				float num5 = (180f - num4) / 2f;
				float num6 = 200f * Mathf.Tan(num4 / 2f * ((float)Math.PI / 180f));
				Vector3 vector2 = new Vector3(Mathf.Sin(num5 * ((float)Math.PI / 180f)) * num6 * (flag ? 1f : (-1f)), vector.y, Mathf.Cos(num5 * ((float)Math.PI / 180f)) * num6);
				Vector3 vector3 = vector2 - vector;
				Vector3 vector4 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] + vector3;
				Vector3 vector5 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] + vector3;
				Vector3 vector6 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] + vector3;
				Vector3 vector7 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3] + vector3;
				Quaternion quaternion = Quaternion.Euler(0f, num4 * (flag ? (-1f) : 1f), 0f);
				vector4 = quaternion * (vector4 - vector2) + vector2;
				vector5 = quaternion * (vector5 - vector2) + vector2;
				vector6 = quaternion * (vector6 - vector2) + vector2;
				vector7 = quaternion * (vector7 - vector2) + vector2;
				vector4 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector4).normalized * (layer.Radius + 0.4f));
				vector5 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector5).normalized * (layer.Radius + 0.4f));
				vector6 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector6).normalized * (layer.Radius + 0.4f));
				vector7 = worldToLocalMatrix.MultiplyPoint(localToWorldMatrix.MultiplyPoint(vector7).normalized * (layer.Radius + 0.4f));
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] = vector4;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] = vector5;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] = vector6;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3] = vector7;
			}
		}
		((TMP_Text)textMesh).UpdateVertexData((TMP_VertexDataUpdateFlags)255);
	}
}
