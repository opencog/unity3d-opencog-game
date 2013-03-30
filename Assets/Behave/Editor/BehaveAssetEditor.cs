using UnityEngine;
using UnityEditor;
using System.Collections;
using Behave.Editor;

[CustomEditor (typeof (BehaveAsset))]
public class BehaveAssetEditor : UnityEditor.Editor
{
	enum DelayedActionType { None, Edit, BuildDebug, BuildRelease };
	
	DelayedActionType m_Action = DelayedActionType.None;
	
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
		switch (m_Action)
		{
			case DelayedActionType.None:
			return;
			case DelayedActionType.Edit:
				m_Action = DelayedActionType.None;
				BehaveMenu.EditLibrary ();
			break;
			case DelayedActionType.BuildDebug:
				m_Action = DelayedActionType.None;
				BehaveMenu.Compile ();
			break;
			case DelayedActionType.BuildRelease:
				m_Action = DelayedActionType.None;
				BehaveMenu.CompileRelease ();
			break;
			default:
				m_Action = DelayedActionType.None;
				throw new System.ArgumentException ("Unknown action type: " + m_Action);
		}
	}
	
	public override void OnInspectorGUI ()
	{
		Behave.Editor.Resources.VersionBar ();

		EditorGUILayout.Separator ();
		
		GUI.enabled = BehaveMenu.ValidateEditLibrary ();
		if (GUILayout.Button ("Edit library"))
		{
			m_Action = DelayedActionType.Edit;
		}
		GUI.enabled = true;
		
		EditorGUILayout.Separator ();
		
		GUILayout.BeginHorizontal ();
			GUI.enabled = BehaveMenu.ValidateCompile ();
			if (GUILayout.Button ("Build library debug", GUI.skin.GetStyle ("ButtonLeft")))
			{
				m_Action = DelayedActionType.BuildDebug;
			}
			GUI.enabled = BehaveMenu.ValidateCompileRelease ();
			if (GUILayout.Button ("Build library release", GUI.skin.GetStyle ("ButtonRight")))
			{
				m_Action = DelayedActionType.BuildRelease;
			}
			GUI.enabled = true;
		GUILayout.EndHorizontal ();
		
		EditorGUILayout.Separator ();
		
		LibraryInspector.OnGUI ();
	}
}
