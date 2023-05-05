using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ReTime))]
public class TimeRewindCustomGUI : Editor {
	public override void OnInspectorGUI(){
		GUILayout.Box (Resources.Load("ReTime Cover") as Texture, GUILayout.Width(375), GUILayout.Height(200));
		ReTime script = (ReTime)target;
		GUILayout.Label ("You can [ StartTimeRewind() ] and [ StopTimeRewind() ] manually in code"); 

		//vars
		script.RewindSeconds = EditorGUILayout.FloatField (new GUIContent ("Rewind Seconds", "The amount of time to rewind in seconds"), script.RewindSeconds);
		script.RewindSpeed = EditorGUILayout.Slider(new GUIContent("Rewind Speed", "The speed scale float in which the rewind will play at. From 0.1 - 5.0"), script.RewindSpeed, 0.1f, 5f);
		script.UseInputTrigger = EditorGUILayout.Toggle (new GUIContent("Use Input Trigger", "Enabling this will let you use a key trigger. If disabled, then you can use the StartRewind() and StopRewind() methods"), script.UseInputTrigger);

		EditorGUI.BeginDisabledGroup (script.UseInputTrigger == false);
		script.KeyTrigger = EditorGUILayout.TextField (new GUIContent("Key Trigger", "Type the key name of trigger. Check the PDF for more info"), script.KeyTrigger);
		EditorGUI.EndDisabledGroup ();

		script.PauseEnd = EditorGUILayout.Toggle (new GUIContent ("Pause on End", "Enabling this will pause everything at the end of the rewind"), script.PauseEnd);
	}
}
