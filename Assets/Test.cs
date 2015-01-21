using UnityEngine;

public class Test : MonoBehaviour
{
	private GameObject kitten;

	public void OnGUI()
	{
		if (GUILayout.Button("Create standard-def kitten."))
		{
			if (kitten != null)
				Destroy(kitten); // :-(

			GameObject prefab = (GameObject)Resources.Load("kitten");
			kitten = (GameObject)Instantiate(prefab);
		}

		if (GUILayout.Button("Create high-def kitten!"))
		{
			if(kitten != null)
				Destroy(kitten); // :-(

			GameObject prefab = (GameObject)Resources.Load("kitten_hd");
			kitten = (GameObject)Instantiate(prefab);
		}
	}
}