using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ChunkManager))]
public class EditorProceduralTerrain : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		ChunkManager myScript = (ChunkManager)target;
		if(GUILayout.Button("Generate Terrain"))
		{
			myScript.Awake();
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