using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenCog;
using OpenCog.Attributes;

/// <summary>
/// The OpenCog Script Scanner.  Scans the project's non-editor scripts for use
/// with the Automated Editor Factory and the resulting auto-generated Editors.
/// Allows for the cross-referencing of missing scripts by public property or
/// field.
/// </summary>
[ExecuteInEditMode]
public class OCScriptScanner : MonoBehaviour
{
	//Holds a scanned script
	public class ScannedScript
	{
		//All of the serialized properties
		public Dictionary<string, PropertyInfo> properties;

		//The instance id
		public int id;

		//The script itself
		public MonoScript script;
	}

	//All of the candidate scripts
	private static List<ScannedScript> m_scripts;

	//Whether we have scanned for scripts
	private static bool m_initialized = false;

	//Are we setup to repaint on changes to the project window?
	private static bool m_willRepaint;

	public static List<ScannedScript> Scripts
	{
		get {return m_scripts;}
	}

	//Initialize the scanned scripts
	public static void Start()
	{
		if( !m_willRepaint )
		{
			EditorApplication.projectWindowChanged += () => {
				//@TODO: Repaint only in the Editor?
				//Repaint();
			};
			m_willRepaint = true;
		}
		if( !m_initialized )
		{
			m_initialized = true;

			ScanAll();
		}
	}

	//Scan all of the scripts in resources (not editor scripts)
	public static void ScanAll()
	{
		//Get all of the scripts
		m_scripts = Resources.FindObjectsOfTypeAll( typeof( MonoScript ) )
			//Make this a collection of MonoScripts
      .Cast<MonoScript>()
			//Make sure that they aren't system scripts
      .Where( c => c.hideFlags == 0 )
			//Make sure that they are compiled and
			//can retrieve a class
      .Where( c => c.GetClass() != null )
			//Create a scanned script for each one
      .Select( c => new ScannedScript { id = c.GetInstanceID(),
        script = c,
				//The properties need to be all public
				//and all private with [SerializeField] set
        properties = c.GetClass()
           .GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
          .Where( p => p.CanWrite || ( !p.CanWrite && p.IsDefined( typeof( SerializeField ), false ) ) )
          .ToDictionary( p => p.Name )
      } )
      .ToList();
	}

}

