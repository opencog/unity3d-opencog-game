using UnityEditor;
using UnityEngine;

namespace Cubiquity
{
	[InitializeOnLoad]
	class UpdateAllVolumes
	{
	    static UpdateAllVolumes()
	    {
	        EditorApplication.update += Update;
	    }
	 
	    static void Update ()
	    {
			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// In play mode the volume syncing is driven by a coroutine, but this doesn't run in edit mode. We could try
				// syncing when an action occurs (such as a terrain modification) but this doesn't give us background loading
				// of the terrain. Therefore we use maintain and use a global list of currently enabled volume components.
				// Note that a previous version of this code used 'FindObjectsOfType' for this purpose, but it appeared
				// unreliable (possibly when volums were in a hierarchy - this was never quite clear).
				foreach(Volume volume in Volume.allEnabledVolumes)
				{
					volume.Synchronize();
				}
			}
	    }
	}
}
