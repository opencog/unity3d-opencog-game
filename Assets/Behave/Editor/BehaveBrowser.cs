using UnityEngine;
using UnityEditor;
using Behave.Editor;

public class BehaveBrowser : EditorWindow, IBrowserWindow
{
	private static BehaveBrowser s_Instance;

	public BehaveBrowser ()
	{
		hideFlags = HideFlags.DontSave;
		
		if (s_Instance != null)
		{
			Debug.LogError ("Trying to create two instances of singleton. Self destruction in 3...");
			Destroy (this);
			return;
		}
		
		Behave.Editor.Browser.Init (this);
		
		s_Instance = this;
		
		title = "Behave browser";
	}
	
	public void OnDestroy ()
	{
		s_Instance = null;
		Behave.Editor.Browser.Instance.OnDestroy ();
	}
	
	public static BehaveBrowser Instance
	{
		get
		{
			if (s_Instance == null)
			{
				GetWindow (typeof (BehaveBrowser));
			}
			
			return s_Instance;
		}
	}
	
    public Rect Position
    {
        get
		{
			return position;
		}
        set
		{
			this.position = value;
		}
    }

    public Behave.Editor.Browser Browser
    {
        get
		{
			return Behave.Editor.Browser.Instance;
		}
    }

    new public void Repaint ()
	{
		base.Repaint ();
	}
	
    new public void Close ()
	{
		base.Close ();
	}
	
    new public void Show ()
	{
		base.Show ();
	}
	
	new public void Focus ()
	{
		base.Focus ();
	}
	
	public bool HasFocus
	{
		get
		{
			return EditorWindow.focusedWindow == this;
		}
	}
	
	public void OnGUI ()
	{
		Browser.OnGUI ();
	}
}
