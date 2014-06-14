using UnityEngine;
using UnityEditor;

namespace Cubiquity
{
	// This class is something of a hack. We don't want to save the octree which we build for a volume as we generate
	// this on demand, but there appears to be a bug in Unity that even when the DontSave flag is set a reference to 
	// the object is save even when the object itself is not. This causes errors when loading. So instead of setting
	// the DontSave flag we instead tap into the serialisation code and discard the octree before saving occurs. This
	// means both when really saving the scene to disk but also when simply switching between edit mode and play mode.
	[ExecuteInEditMode]
	public class OnSaveHandler : UnityEditor.AssetModificationProcessor
	{
	    public static void OnWillSaveAssets( string[] assets )
	    {
			Object[] volumes = Object.FindObjectsOfType(typeof(Volume));
			foreach(Object volume in volumes)
			{
				((Volume)volume).FlushInternalData();
			}
	    }
	}
}
