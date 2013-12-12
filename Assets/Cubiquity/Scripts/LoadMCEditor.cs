using UnityEngine;
using System.Collections;
using UnityEditor;
/// <summary>
/// uses only to Recreate the MC region(world) 
/// </summary>
public class LoadMCEditor : MonoBehaviour
{

    [MenuItem("MineCraft Loading/Load Small Region")]
    static void Init()
    {
        CreateMCRegion.CreateSquaredRegion();
       
    }
    [MenuItem("MineCraft Loading/Load Large Region")]
    static void LargeRegion()
    {
        CreateMCRegion.CreateLargeRegion();
    }

	
}
