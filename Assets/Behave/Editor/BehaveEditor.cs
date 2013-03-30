using UnityEngine;
using UnityEditor;
using System.Collections;
using Behave.Editor;
using Behave.Runtime;
using Behave.Assets;

public class BehaveEditor : ScriptableObject, IEditorWindow
{
	private static BehaveEditor m_Instance;
	private Behave.Editor.Editor m_Editor;
	
	public BehaveEditor ()
	{
		hideFlags = HideFlags.DontSave;
		
		if (m_Instance != null)
		{
			Debug.LogError ("Trying to create two instances of singleton. Self destruction in 3...");
			DestroyImmediate (this);
			return;
		}
		
		if (this.Editor == null)
		{
			Debug.LogError ("Failed to link with library implementation");
			DestroyImmediate (this);
			return;
		}
		
		m_Instance = this;
	}
	
	public void OnDestroy ()
	{
		Editor.OnDestroy();
		m_Instance = null;
	}
	
	public static BehaveEditor Instance
	{
		get
		{
			if (m_Instance == null)
			{
				CreateInstance (typeof (BehaveEditor));
			}
			
			return m_Instance;
		}
	}
	
	public void Init ()
	{
		m_Editor = Behave.Editor.Editor.Instance;
	}
	
    public Behave.Editor.Editor Editor
    {
        get
		{
			if (m_Editor == null)
			{
				if (Behave.Editor.Editor.Instance == null)
				{
					Behave.Editor.Editor.Init (this);
				}
				Init ();
			}
			
			return m_Editor;
		}
    }

    public IBehaveAsset SelectedAsset
    {
        get
		{
			return Selection.activeObject as BehaveAsset;
		}
    }

	public bool SaveLibrary (Library libraryAsset, IBehaveAsset behaveAsset)
	{
		byte[] newData = libraryAsset.GetData ();
		
		if (!behaveAsset.Data.Equals (newData))
		{
			behaveAsset.Data = newData;
			EditorUtility.SetDirty ((BehaveAsset)behaveAsset);
			return true;
		}
		
		return false;
	}
	
	public string GetLibraryName (IBehaveAsset asset)
	{
		string name = AssetDatabase.GetAssetPath ((BehaveAsset)asset);
		name = name.Substring (name.LastIndexOf ("/") + 1);
		return name.Substring (0, name.LastIndexOf ("."));
	}
	
	public void Repaint ()
	{
		EditorUtility.SetDirty (this);
		
		UnityEditor.Editor[] inspectors = FindObjectsOfType (typeof (UnityEditor.Editor)) as UnityEditor.Editor[];

		foreach (UnityEditor.Editor inspector in inspectors)
		{
			inspector.Repaint ();
		}
	}
}