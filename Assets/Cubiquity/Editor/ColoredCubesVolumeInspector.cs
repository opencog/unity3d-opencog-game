using UnityEngine;
using UnityEditor;
using System.Collections;
 
namespace Cubiquity
{
	[CustomEditor (typeof(ColoredCubesVolume))]
	public class ColoredCubesVolumeInspector : Editor
	{
		ColoredCubesVolume coloredCubesVolume;
		
		private bool addMode = true;
		private bool deleteMode = false;
		private bool paintMode = false;
		
		Color paintColor = Color.white;
		
		public void OnEnable()
		{
		    coloredCubesVolume = target as ColoredCubesVolume;
		}
		
		public override void OnInspectorGUI()
		{		
			if(EditorGUILayout.Toggle("Add cubes", addMode))
			{
				addMode = true;
				deleteMode = false;
				paintMode = false;
			}
			
			if(EditorGUILayout.Toggle("Delete cubes", deleteMode))
			{
				addMode = false;
				deleteMode = true;
				paintMode = false;
			}
			
			if(EditorGUILayout.Toggle("Paint cubes", paintMode))
			{
				addMode = false;
				deleteMode = false;
				paintMode = true;
			}
			
			paintColor = EditorGUILayout.ColorField(paintColor, GUILayout.Width(200));
		}
		
		public void OnSceneGUI()
		{
			//Debug.Log ("ColoredCubesVolumeEditor.OnSceneGUI()");
			Event e = Event.current;
			
			Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
			Vector3 dir = ray.direction * 1000.0f; //The maximum distance out ray will be cast.
			
			if(((e.type == EventType.MouseDown) || (e.type == EventType.MouseDrag)) && (e.button == 0))
			{
				// Perform the raycasting. If there's a hit the position will be stored in these ints.
				int resultX, resultY, resultZ;
				if(addMode)
				{
					bool hit = ColoredCubesVolumePicking.PickLastEmptyVoxel(coloredCubesVolume, ray.origin.x, ray.origin.y, ray.origin.z, dir.x, dir.y, dir.z, out resultX, out resultY, out resultZ);
					if(hit)
					{
						coloredCubesVolume.data.SetVoxel(resultX, resultY, resultZ, (QuantizedColor)paintColor);
					}
				}
				else if(deleteMode)
				{
					bool hit = ColoredCubesVolumePicking.PickFirstSolidVoxel(coloredCubesVolume, ray.origin.x, ray.origin.y, ray.origin.z, dir.x, dir.y, dir.z, out resultX, out resultY, out resultZ);
					if(hit)
					{
						coloredCubesVolume.data.SetVoxel(resultX, resultY, resultZ, new QuantizedColor(0,0,0,0));
					}
				}
				else if(paintMode)
				{
					bool hit = ColoredCubesVolumePicking.PickFirstSolidVoxel(coloredCubesVolume, ray.origin.x, ray.origin.y, ray.origin.z, dir.x, dir.y, dir.z, out resultX, out resultY, out resultZ);
					if(hit)
					{
						coloredCubesVolume.data.SetVoxel(resultX, resultY, resultZ, (QuantizedColor)paintColor);
					}
				}
				
				Selection.activeGameObject = coloredCubesVolume.gameObject;
			}
			else if ( e.type == EventType.Layout )
		    {
		       // See: http://answers.unity3d.com/questions/303248/how-to-paint-objects-in-the-editor.html
		       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
		    }
			
			coloredCubesVolume.Synchronize();
		}
	}
}
