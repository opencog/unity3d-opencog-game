using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (BehaveEditor))]
public class BehaveComponentEditor : UnityEditor.Editor
{
	public void OnEnable ()
	{
		hideFlags = HideFlags.DontSave;
		EditorApplication.update += Update;
	}
	
	public void OnDisable ()
	{
		EditorApplication.update -= Update;
	}
	
	public void Update ()
	{
		if (Behave.Editor.Editor.Instance.InspectorNeedsRepaint)
		{
			Repaint ();
			Behave.Editor.Editor.Instance.InspectorNeedsRepaint = false;
		}
	}
	
	public override void OnInspectorGUI ()
	{
		if (Behave.Editor.TreeEditor.Instance == null)
		{
			GUILayout.Label ("No tree editor loaded");
			return;
		}
		
		Behave.Editor.TreeEditor.Instance.OnInspectorGUI ();
	}
}
