using System.IO;
using UnityEngine;
using UnityEditor;

public class PrefabBuilder
{
	[MenuItem("PrefabBuilder/Build Prefabs From Selected")]
	public static void BuildPrefabsFromSelected()
	{
		string prefabName = Selection.activeGameObject.name;
		CreateStandardPrefab(Selection.activeGameObject, prefabName);
		CreateHighDefPrefab(Selection.activeGameObject, prefabName + "_hd", "_hd");
		AssetDatabase.SaveAssets();
	}

	[MenuItem("PrefabBuilder/Build Prefabs From Selected", true)]
	public static bool ValidateBuildPrefabsFromSelected()
	{
		return Selection.activeGameObject != null;
	}

	private static void CreateStandardPrefab(GameObject createFrom, string prefabName)
	{
		GameObject copy = (GameObject)Object.Instantiate(createFrom);
		PrefabUtility.CreatePrefab("Assets/Resources/" + prefabName + ".prefab", copy);
		Object.DestroyImmediate(copy);
	}

	private static void CreateHighDefPrefab(GameObject createFrom, string prefabName, string spriteReplaceSuffix)
	{
		GameObject copy = (GameObject)Object.Instantiate(createFrom);

		foreach (SpriteRenderer sr in copy.GetComponentsInChildren<SpriteRenderer>())
		{
			if (sr.sprite != null)
			{
				string baseSpritePath = AssetDatabase.GetAssetPath(sr.sprite);
				string spriteDirectory = Path.GetDirectoryName(baseSpritePath);
				string spriteName = Path.GetFileNameWithoutExtension(baseSpritePath);
				string spriteExtension = Path.GetExtension(baseSpritePath);
				string newSpriteName = spriteName + spriteReplaceSuffix;
				string newSpritePath = spriteDirectory + "/" + newSpriteName + spriteExtension;
				Sprite newSprite = (Sprite)AssetDatabase.LoadAssetAtPath(newSpritePath, typeof (Sprite));
				if (newSprite != null)
				{
					sr.sprite = newSprite;
					Debug.Log("Replaced '" + baseSpritePath + "' with '" + newSpritePath + "'.");
				}
			}
		}

		PrefabUtility.CreatePrefab("Assets/Resources/" + prefabName + ".prefab", copy);
		Object.DestroyImmediate(copy);
	}
}
