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
/// <summary>
/// creates a chunk with 16x10x16 
/// </summary>
/// <remarks> this will be extended in order to be Scalable</remarks>
    public static void CreateRegion(){
        AnvilWorld world = AnvilWorld.Create(Dir);
        if (world == null)
        {
            return;
        }
        IChunkManager icm = world.GetChunkManager();
        ChunkRef chunk = icm.CreateChunk(0, 0);
        chunk.IsTerrainPopulated = true;
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
        world.Save();        
    }
  
}
