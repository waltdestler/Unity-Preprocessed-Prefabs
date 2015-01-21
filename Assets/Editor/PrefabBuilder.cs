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
		CreateSpriteSwappedPrefab(Selection.activeGameObject, prefabName + "_hd", "_hd");

		// Probably don't need to save, but it can't hurt.
		AssetDatabase.SaveAssets();
	}

	[MenuItem("PrefabBuilder/Build Prefabs From Selected", true)]
	public static bool ValidateBuildPrefabsFromSelected()
	{
		return Selection.activeGameObject != null;
	}

	private static void CreateStandardPrefab(GameObject createFrom, string prefabName)
	{
		// Create a copy of the object so that it doesn't link to the prefab.
		GameObject copy = (GameObject)Object.Instantiate(createFrom);

		// Create the prefab from the copy.
		PrefabUtility.CreatePrefab("Assets/Resources/" + prefabName + ".prefab", copy);

		// Destroy the copy of the object (otherwise it will be left behind in the scene view).
		Object.DestroyImmediate(copy);
	}

	private static void CreateSpriteSwappedPrefab(GameObject createFrom, string prefabName, string spriteReplaceSuffix)
	{
		// Create a copy of the object so that we can make changes without screwing up the original.
		GameObject copy = (GameObject)Object.Instantiate(createFrom);

		// Find all of the SpriteRenderers whose sprites we want to swap out.
		foreach (SpriteRenderer sr in copy.GetComponentsInChildren<SpriteRenderer>())
		{
			// Don't swap sprite if we don't have one!
			if (sr.sprite != null)
			{
				// Figure out the path of the sprite to swap in.
				string baseSpritePath = AssetDatabase.GetAssetPath(sr.sprite);
				string spriteDirectory = Path.GetDirectoryName(baseSpritePath);
				string spriteName = Path.GetFileNameWithoutExtension(baseSpritePath);
				string spriteExtension = Path.GetExtension(baseSpritePath);
				string newSpriteName = spriteName + spriteReplaceSuffix;
				string newSpritePath = spriteDirectory + "/" + newSpriteName + spriteExtension;

				// Attempt to load the new sprite.
				Sprite newSprite = (Sprite)AssetDatabase.LoadAssetAtPath(newSpritePath, typeof (Sprite));
				if (newSprite != null)
				{
					// Set the new sprite on the SpriteRenderer.
					sr.sprite = newSprite;

					Debug.Log("Replaced '" + baseSpritePath + "' with '" + newSpritePath + "'.");
				}
			}
		}

		// Create the prefab from the modified copy.
		PrefabUtility.CreatePrefab("Assets/Resources/" + prefabName + ".prefab", copy);

		// Destroy the copy of the object (otherwise it will be left behind in the scene view).
		// If we want to debug our pre-processor, we might not destroy it so that it's
		// easier to dive into its output.
		Object.DestroyImmediate(copy);
	}
}
