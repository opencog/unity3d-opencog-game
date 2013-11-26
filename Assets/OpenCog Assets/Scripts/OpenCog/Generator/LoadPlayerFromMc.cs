using UnityEngine;
using System.Collections;
using Substrate;
using Substrate.Core;
using System.IO;

[System.Serializable]
public class LoadPlayerFromMc
{

    [SerializeField]
    public static UnityEngine.Vector3 position;
    /// <summary>
    /// the path for the minecraft .dat file
    /// </summary>
    public string playerName = "OCPlayer";

    /// <summary>
    /// checks the existance of the directory
    /// </summary>
    /// <returns> true if exist false otherwise</returns>
    public bool IsExist(string path)
    {
        string mapdir = Path.Combine(dir, path);
        return Directory.Exists(mapdir) ? true : false;

    }

    public static UnityEngine.Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public static string dir
    {
        get { return Application.streamingAssetsPath; }
    }
   
    /// <summary>
    /// delete the available player and create a new one and terieve its info spc positon for this demo
    /// </summary>
    /// <returns> the position of the player</returns>
    /// <remarks> u can cancel the deletePlayer it only usefull for demo</remarks>
    public UnityEngine.Vector3 GetPlayerPositionFromMC(string MapName)
    {
        string _MapName = Path.Combine(dir, MapName);
        AnvilWorld world = AnvilWorld.Open(_MapName);
        IPlayerManager pm = world.GetPlayerManager();

        pm.DeletePlayer(playerName);
        Player CreatePlayer = new Player();
        Player FetchPlayer = new Player();

        CreatePlayer.Position.X = 15;
        CreatePlayer.Position.Y = 150;
        CreatePlayer.Position.Z = 25; ;
        pm.SetPlayer(playerName, CreatePlayer);


        FetchPlayer = pm.GetPlayer(playerName);
        position.x = (float)FetchPlayer.Position.X;
        position.y = (float)FetchPlayer.Position.Y;
        position.z = (float)FetchPlayer.Position.Z;

        return position;


    }

}
