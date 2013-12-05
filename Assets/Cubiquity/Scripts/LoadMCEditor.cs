using UnityEngine;
using System.Collections;
using UnityEditor;
/// <summary>
/// uses only to Recreate the MC region(world) 
/// </summary>
public class LoadMCEditor : MonoBehaviour
{

    [MenuItem("MineCraft Loading/Load Region")]
    static void Init()
    {
        CreateMCRegion.CreateRegion(); 
       
    }    
	
}
