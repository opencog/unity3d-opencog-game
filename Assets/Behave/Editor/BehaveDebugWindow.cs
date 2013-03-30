using UnityEngine;
using UnityEditor;
using Behave.Editor;

public class BehaveDebugWindow : EditorWindow, IDebugWindow
{
	private static BehaveDebugWindow s_Instance;
	
	public BehaveDebugWindow ()
	{
		hideFlags = HideFlags.DontSave;
		
		if (s_Instance != null)
		{
			Debug.LogError ("Trying to create two instances of singleton. Self destruction in 3...");
			Destroy (this);
			return;
		}
		
		DebugWindow.Init (this);
		
		s_Instance = this;
		
		title = "Behave debugger";
	}
	
	public void OnDestroy ()
	{
		s_Instance = null;
		DebugWindow.Instance.OnDestroy();
	}
	
	public static BehaveDebugWindow Instance
	{
		get
		{
			if (s_Instance == null)
			{
				GetWindow (typeof (BehaveDebugWindow));
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

    new public void Repaint ()
	{
		base.Repaint();
	}
	
    new public void Close ()
	{
		base.Close();
	}
	
    new public void Show ()
	{
		base.Show();
	}
	
	new public void Focus ()
	{
		base.Focus();
	}
	
	new public void BeginWindows ()
	{
		base.BeginWindows ();
	}
	
	new public void EndWindows ()
	{
		base.EndWindows ();
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
		DebugWindow.Instance.OnGUI ();
	}
}
