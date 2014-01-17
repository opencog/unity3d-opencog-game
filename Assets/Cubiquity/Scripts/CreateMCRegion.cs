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
    ///deletes existing world and creates 2x2 chunks with different color
    /// </summary>
    public static void CreateSquaredRegion()
    {
        
        AnvilWorld world = AnvilWorld.Create(Dir);
        world.Level.LevelName = "SmallRegion";
        if (world == null)
        {
            return;
        }
        IChunkManager icm = world.GetChunkManager();
        IRegionManager irm = world.GetRegionManager();
        foreach (IRegion ir in irm)
        {

            for (int x = 0; x < 32; x++)
            {

                for (int z = 0; z < 32; z++)
                {
                    ir.DeleteChunk(x, z);
                }
            }
        }

        for (int rows = 0; rows < 2; rows++)
        {
            for (int Columns = 0; Columns < 2; Columns++)
            {
                ChunkRef chunk = icm.CreateChunk(rows, Columns);
                chunk.IsTerrainPopulated = true;
                chunk.Blocks.AutoLight = false;
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            if (rows == 0 && Columns == 0)
                            {

                                chunk.Blocks.SetID(x, y, z, 41);
                            }
                            else if (rows == 0 && Columns == 1)
                            {
                                if (x < 7)
                                    chunk.Blocks.SetID(x, y, z, 133);
                                else
                                    chunk.Blocks.SetID(x, y, z, 41);
                            }
                            else if (rows == 1 && Columns == 0)
                            {

                                chunk.Blocks.SetID(x, y, z, 22);
                            }
                            else if (rows == 1 && Columns == 1)
                            {

                                chunk.Blocks.SetID(x, y, z, 80);
                            }


                        }
                    }
                }

            }
        }   
        
        world.Save();        
    }


    /// <summary>
    /// creates a Large region 32x32 chunks with different color. 
    /// </summary>
    public static void CreateLargeRegion()
    {

        AnvilWorld world = AnvilWorld.Create(Dir);
        world.Level.LevelName = "LargeRegion";
        if (world == null)
        {
            return;
        }
        IChunkManager icm = world.GetChunkManager();
        IRegionManager irm = world.GetRegionManager();
        foreach (IRegion ir in irm)
        {

            for (int x = 0; x < 32; x++)
            {

                for (int z = 0; z < 32; z++)
                {
                    ir.DeleteChunk(x, z);
                }
            }
        }
        for (int rows = 0; rows < 32; rows++)
        {
            for (int Columns = 0; Columns < 32; Columns++)
            {
                ChunkRef chunk = icm.CreateChunk(rows, Columns);
                chunk.IsTerrainPopulated = true;
                chunk.Blocks.AutoLight = false;
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            if (Columns < 4 && rows < 2)
                            {
                                chunk.Blocks.SetID(x, y, z, 41);
                            }
                            else if (Columns > 4 && Columns < 20 && rows > 2 && rows < 20)
                            {
                                chunk.Blocks.SetID(x, y, z, 80);
                            }

                            else if (Columns > 20 && Columns < 30 && rows > 20 && rows < 28)
                            {
                                chunk.Blocks.SetID(x, y, z, 22);
                            }
                            else
                                chunk.Blocks.SetID(x, y, z, 133);

                        }
                    }
                }

            }
        }


        world.Save();

    }
  
}
