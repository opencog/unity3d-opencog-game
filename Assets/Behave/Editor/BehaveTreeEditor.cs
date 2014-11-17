using UnityEngine;
using UnityEditor;
using Behave.Editor;

public class BehaveTreeEditor : EditorWindow, ITreeEditorWindow
{
	private static BehaveTreeEditor s_Instance;
	
	private bool m_ContinuousRepaint = false;

	public BehaveTreeEditor ()
	{
		hideFlags = HideFlags.DontSave;
		
		if (s_Instance != null)
		{
			Debug.LogError ("Trying to create two instances of singleton. Self destruction in 3...");
			Destroy (this);
			return;
		}
		
		Behave.Editor.TreeEditor.Init (this);
		
		s_Instance = this;
		
		title = "Behave editor";
	}
	
	public void OnDestroy ()
	{
		s_Instance = null;
		Behave.Editor.TreeEditor.Instance.OnDestroy ();
	}
	
	public void OnFocus ()
	{
		if (BehaveEditor.Instance != null)
		{
			Selection.activeObject = BehaveEditor.Instance;
		}
	}
	
	public static BehaveTreeEditor Instance
	{
		get
		{
			if (s_Instance == null)
			{
				GetWindow (typeof (BehaveTreeEditor));
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

    public Behave.Editor.TreeEditor Editor
    {
        get
		{
			return Behave.Editor.TreeEditor.Instance;
		}
    }

	public bool ContinuousRepaint
	{
		get
		{
			return m_ContinuousRepaint;
		}
		set
		{
			m_ContinuousRepaint = value;
		}
	}
	
	public void Update ()
	{
		if (m_ContinuousRepaint)
		{
			Repaint ();
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
		Editor.OnGUI ();
	}
}
