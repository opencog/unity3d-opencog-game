using UnityEngine;
using System.Collections;
using UnityEditor;
/// <summary>
/// window editor that loads player position from minecraft world 
/// and use this position for the unity player.
/// </summary>
public class OCPlayerEditor : EditorWindow 
{
    [MenuItem("LoadPlayer/load")]
    public static void init()
    {
        GetWindow<OCPlayerEditor>();
    }

    public Vector3 PlayerPosition;
    GameObject PlayerGameObject;

    /// <summary>
    /// finds player from Hierarchy with tag name Player
    /// </summary>
    void OnEnable()
    {
        PlayerPosition = LoadPlayerFromMc.Position;
        PlayerGameObject = GameObject.FindWithTag("Player");


    }
    void OnDisable()
    {
        LoadPlayerFromMc.Position = PlayerPosition;

    }
    /// <summary>
    /// default map Name
    /// </summary>
    string MapName = "TestScene2";
  

    public void OnGUI()
    {
        MapName = EditorGUI.TextField(new Rect(10,10,200,15), new GUIContent("Map Name"), MapName);

        if (GUI.Button(new Rect(10,35,150,20),"LoadPlayer Position"))
        {

            LoadPlayerFromMc lpfm = new LoadPlayerFromMc();
            ///checks the existance of the Director.
            /// and loads the player position          
           
            bool alive = lpfm.IsExist(MapName);
            if (alive)
            {
                PlayerPosition = lpfm.GetPlayerPositionFromMC(MapName);
                PlayerGameObject.transform.position = PlayerPosition;
            }
            else
            {
                Debug.LogError("check your Map is under StreamingAssets directory(folder) or Error Map Name");
            }

        }
        EditorGUI.Vector3Field(new Rect(10,60,200,40),"Player Position", PlayerPosition);
        if (GUI.Button(new Rect(10,100,150,20),"Create UPlayer"))
        {
            LoadPlayerFromMc lp = new LoadPlayerFromMc();
            OCGetPlayer.Create(PlayerGameObject.transform.position);

        }
        
       


    }
	
}
