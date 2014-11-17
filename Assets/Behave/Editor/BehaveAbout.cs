using UnityEngine;
using UnityEditor;
using System.Collections;

public class BehaveAbout : EditorWindow
{
	private static BehaveAbout s_Instance;
	private Vector3 m_Scroll;
	
	public BehaveAbout ()
	{
		hideFlags = HideFlags.DontSave;
		
		if (s_Instance != null)
		{
			Debug.LogError ("Trying to create two instances of singleton. Self destruction in 3...");
			Destroy (this);
			return;
		}
		
		s_Instance = this;
		
		title = "About Behave";
		position = new Rect ((Screen.width - 500.0f) / 2.0f, (Screen.height - 400.0f) / 2.0f, 500.0f, 400.0f);
		minSize = new Vector2 (500.0f, 400.0f);
		maxSize = new Vector2 (500.0f, 400.0f);
	}
	
	public void OnDestroy ()
	{
		s_Instance = null;
	}
	
	public static BehaveAbout Instance
	{
		get
		{
			if (s_Instance == null)
			{
				GetWindow (typeof (BehaveAbout), true);
			}
			
			return s_Instance;
		}
	}

	public void OnGUI ()
	{
		m_Scroll = Behave.Editor.Resources.About (m_Scroll);
	}
}