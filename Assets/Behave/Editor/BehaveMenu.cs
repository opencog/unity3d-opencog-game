using UnityEngine;
using UnityEditor;
using Behave.Runtime;
using Behave.Assets;
using Behave.Editor;
using System.IO;

public class BehaveMenu : ScriptableObject
{
	[MenuItem ("Help/Behave documentation &%?")]
	public static void Documentation ()
	{
		Behave.Editor.Resources.Documentation ();
	}
		
	[MenuItem ("Assets/Behave/Edit library &%e")]
	public static void EditLibrary ()
	{
		EditLibraryAsset ((BehaveAsset)Selection.activeObject);
	}
	
	[MenuItem ("Assets/Behave/Edit library &%e", true)]
	public static bool ValidateEditLibrary ()
	{
		return Selection.activeObject is BehaveAsset;
	}
	
	[MenuItem ("Assets/Behave/Build library debug &%b")]
	public static void Compile ()
	{
		Compile (true);
	}
	
	[MenuItem ("Assets/Behave/Build library debug &%b", true)]
	public static bool ValidateCompile ()
	{
		return Selection.activeObject is BehaveAsset;
	}
	
	[MenuItem ("Assets/Behave/Build library release #&%b")]
	public static void CompileRelease ()
	{
		Compile (false);
	}
	
	[MenuItem ("Assets/Behave/Build library release #&%b", true)]
	public static bool ValidateCompileRelease ()
	{
		return Selection.activeObject is BehaveAsset;
	}	
	
	static void Compile (bool debug)
	{
		BehaveAsset asset = Selection.activeObject as BehaveAsset;
		Compiler.Compile (asset, debug);
	}
	
	[MenuItem ("Assets/Create/Behave library")]
	public static void Create ()
	{
		BehaveAsset asset;
		string name = "NewBehaveLibrary";
		int nameIdx = 0;
		
		while (System.IO.File.Exists (
			Application.dataPath + "/" + name + nameIdx + ".asset"
		))
		{
			nameIdx++;
		}

		asset = new BehaveAsset ();
		asset.Data = (new Library ()).GetData ();
		AssetDatabase.CreateAsset (asset, "Assets/" + name + nameIdx + ".asset");
		Selection.activeObject = asset;
		
		EditorUtility.FocusProjectWindow ();
		
		EditLibrary ();
	}
	
	[MenuItem ("Assets/Behave/Create/Collection")]
	public static void CreateCollection ()
	{
		Behave.Editor.Editor.Instance.CreateCollection();
	}
	
	[MenuItem ("Assets/Behave/Create/Collection", true)]
	public static bool ValidateCreateCollection()
	{
		return Behave.Editor.Editor.Instance != null && Behave.Editor.Editor.Instance.ValidateCreateCollection ();
	}
	
	[MenuItem ("Assets/Behave/Create/Tree")]
	public static void CreateTree ()
	{
		Behave.Editor.Editor.Instance.CreateTree ();
	}
	
	[MenuItem ("Assets/Behave/Create/Tree", true)]
	public static bool ValidateCreateTree ()
	{
		return Behave.Editor.Editor.Instance != null && Behave.Editor.Editor.Instance.ValidateCreateTree ();
	}

	[MenuItem ("CONTEXT/Behave/Form new tree")]
	public static void FormNewTree ()
	{
		Behave.Editor.Editor.Instance.FormNewTree ();
	}
	
	[MenuItem ("CONTEXT/Behave/Form new tree", true)]
	public static bool ValidateFormNewTree ()
	{
		return Behave.Editor.Editor.Instance.ValidateFormNewTree ();
	}
	
	[MenuItem ("CONTEXT/Behave/Purge sub-tree")]
	public static void PurgeSubTree ()
	{
		Behave.Editor.Editor.Instance.PurgeSubTree ();
	}
	
	[MenuItem ("CONTEXT/Behave/Purge sub-tree", true)]
	public static bool ValidatePurgeSubTree ()
	{
		return Behave.Editor.Editor.Instance.ValidatePurgeSubTree ();
	}
	
	[MenuItem ("Window/Behave debugger")]
	public static void ShowDebugger ()
	{
		if (BehaveDebugWindow.Instance == null)
		{
			Debug.LogError ("Failed to set up Behave debugger window");
			return;
		}
		
		Behave.Editor.DebugWindow.Instance.Show ();
		Behave.Editor.DebugWindow.Instance.Focus ();
		Behave.Editor.DebugWindow.Instance.Repaint ();
	}
	
	[MenuItem ("Window/Behave browser")]
	public static void ShowBrowser ()
	{
		if (BehaveBrowser.Instance == null)
		{
			Debug.LogError ("Failed to set up Behave browser");
			return;
		}
		
		Behave.Editor.Browser.Instance.Show ();
		Behave.Editor.Browser.Instance.Focus ();
		Behave.Editor.Browser.Instance.Repaint ();
	}
	
	[MenuItem ("Window/Behave tree editor")]
	public static void ShowTreeEditor ()
	{
		if (BehaveTreeEditor.Instance == null)
		{
			Debug.LogError ("Failed to set up Behave tree editor");
			return;
		}
		
		Behave.Editor.TreeEditor.Instance.Show ();
		Behave.Editor.TreeEditor.Instance.Focus ();
		Behave.Editor.TreeEditor.Instance.Repaint ();
	}
	
	[MenuItem ("Help/About Behave...")]
	public static void About ()
	{
		BehaveAbout.Instance.ShowUtility ();
	}
	
	public static Behave.Editor.Editor EditorInstance
	{
		get
		{
			if (Behave.Editor.Editor.Instance == null)
			{
				BehaveEditor behaveEditor = new BehaveEditor ();
				Behave.Editor.Editor.Init (behaveEditor);
				behaveEditor.Init ();
			}

			return Behave.Editor.Editor.Instance;
		}
	}
	
	public static void EditLibraryAsset (BehaveAsset asset)
	{
		EditorInstance.Asset = asset;
		
		Selection.activeObject = asset;
		
		ShowTreeEditor ();
		ShowBrowser ();
	}
}
