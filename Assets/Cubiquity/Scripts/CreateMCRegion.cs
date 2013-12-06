using UnityEngine;
using System.Collections;
using System.IO;
using Substrate;
using Substrate.Core;
/// <summary>
/// create the original Region to be tested by Cubiquity.
/// </summary>
public class CreateMCRegion : MonoBehaviour
{

    public static string Dir
    {
        get
        {
            string name = "MCRegion";
            string path = Path.Combine(Application.streamingAssetsPath, name);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }

    public static void CreateRegion(){
        AnvilWorld world = AnvilWorld.Create(Dir);
        if (world == null)
        {
            return;
        }
        IChunkManager icm = world.GetChunkManager();
        for (int Cwidth = 0; Cwidth < 2; Cwidth++)
        {
            for (int Cdepth = 0; Cdepth < 2; Cdepth++)
            {
                ChunkRef chunk = icm.CreateChunk(Cwidth, Cdepth);
                chunk.IsTerrainPopulated = true;
                CreateChunck(chunk);
            }            
        
        }
        world.Save();        
    }

    static void CreateChunck(ChunkRef chunk)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 16; z++)
                {

                    chunk.Blocks.SetID(x, y, z, (int)BlockType.GOLD_BLOCK);
                }
            }
        }
    }
  
}
