using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UnityPerlinGenerator))]
public class EditorGenerateTerrain : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		UnityPerlinGenerator myScript = (UnityPerlinGenerator)target;
		if(GUILayout.Button("Generate Terrain"))
		{
			myScript.Generate();
		}
		if(GUILayout.Button("Clear Children"))
		{
			while(myScript.transform.childCount>0){
				foreach(Transform tr in myScript.transform){
					MonoBehaviour.DestroyImmediate(tr.gameObject);
				}
			}
		}
	}
}