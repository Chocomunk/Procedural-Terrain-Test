using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureNoise))]
public class TextureNoiseInspector : Editor {

	private TextureNoise creator;
	
	private void OnEnable () {
		creator = target as TextureNoise;
		Undo.undoRedoPerformed += RefreshCreator;
	}
	
	private void OnDisable () {
		Undo.undoRedoPerformed -= RefreshCreator;
	}
	
	private void RefreshCreator () {
		if (Application.isPlaying) {
			creator.FillTexture();
		}
	}
	
	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck()) {
			RefreshCreator();
		}
	}
}