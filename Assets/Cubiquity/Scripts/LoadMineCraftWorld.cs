using UnityEngine;
using Substrate;
using Substrate.Core;
using System.IO;
public class LoadMineCraftWorld
{
   
    public static string Dir
    {
        get {
            string name = "MCRegion";
            string path = Path.Combine(Application.streamingAssetsPath, name);
            return path;
        }
    }
/// <summary>
/// gets the dimention of a Minecraft using Substrate and pass to
/// the Cubuquity to create a region based on this info.
/// </summary>
/// <returns>x,y,z dimention of MineCraft region </returns>
    public static UnityEngine.Vector3 regionDimension()
    {
        UnityEngine.Vector3 dimension = new UnityEngine.Vector3();
        AnvilWorld world = AnvilWorld.Open(Dir);
        if (world == null)
        {
            Debug.LogError("world is empty");
        }
        IChunkManager icm = world.GetChunkManager();
        foreach (ChunkRef chunk in icm)
        {
            dimension.x += chunk.Blocks.XDim;
            /// you can take any value of x and z . it returns the hight of a block from a chunk
            ///this will not work for modified chunk. i will find another method. just for the time being
            dimension.y = chunk.Blocks.GetHeight(4, 4);
            dimension.z += chunk.Blocks.ZDim;          
            
        }

        return dimension;
    }
/// <summary>
/// gets the deleted blocks from cubiquity and delete this block from Minecraft to using Substrate
/// </summary>
/// <param name="deletedBlocks">gets the x,y,z coordinates of the deleted block</param>

    public static void UpdateMcRegionForDeletedBlocks(UnityEngine.Vector3 deletedBlock)
    {
        AnvilWorld wolrd = AnvilWorld.Open(Dir);
        UnityEngine.Vector3 MCregion = regionDimension();
        int x = (int)deletedBlock.x;
        int y = (int)deletedBlock.y;
        int z = (int)deletedBlock.z;
        if (wolrd == null)
        {
            return;
        }
        IChunkManager icm = wolrd.GetChunkManager();           
        //AnvilWorld world = AnvilWorld.Open();
        foreach (ChunkRef chunk in icm)
        {
            if (x >= 0 && y >= 0 && z >= 0 && x < MCregion.x && z < MCregion.z && y<10)
            {
                chunk.Blocks.SetData(x, y,z, 0);
                chunk.Blocks.SetID(x,y,z, 0);
               
            }

            chunk.IsTerrainPopulated = true;
        }
        wolrd.Save();
    }
    /// <summary>
    /// paint Minecraft blocks 
    /// </summary>
    /// <param name="pintedBlock"> gets painted x,y,z values from Cubiquity</param>

    public static void UpdateMcRegionForPaintBlocks(UnityEngine.Vector3 pintedBlock)
    {
        AnvilWorld wolrd = AnvilWorld.Open(Dir);
        UnityEngine.Vector3 MCregion= regionDimension();
        int x = (int)pintedBlock.x;
        int y = (int)pintedBlock.y;
        int z = (int)pintedBlock.z;
        if (wolrd == null)
        {
            return;
        }
        IChunkManager icm = wolrd.GetChunkManager();
        //AnvilWorld world = AnvilWorld.Open();
        foreach (ChunkRef chunk in icm)
        {
            if (x >= 0 && y >= 0 && z >= 0 && x<MCregion.x && z<MCregion.z&&y<10)
            {
                chunk.Blocks.SetData(x, y, z, 0);
                chunk.Blocks.SetID(x, y, z, 0);
                /// it is for test only. later on, i will change this
                chunk.Blocks.SetID(x, y, z, (int)BlockType.GRASS);

            }
            chunk.IsTerrainPopulated = true;
           

        }
        wolrd.Save();
    }
/// <summary>
/// add block to Minecraft region based on the Substrate added block coordintates. 
/// </summary>
/// <param name="addedBlock">x,y,x informaiton of a block to be added</param>

    public static void UpdateMcRegionForAddBlocks(UnityEngine.Vector3 addedBlock)
    {
        AnvilWorld wolrd = AnvilWorld.Open(Dir);
        UnityEngine.Vector3 MCregion = regionDimension();
        int x = (int)addedBlock.x;
        int y = (int)addedBlock.y;
        int z = (int)addedBlock.z;
        UnityEngine.Vector3 vec = regionDimension();
        if (wolrd == null)
        {
            return;
        }
        IChunkManager icm = wolrd.GetChunkManager();
        //AnvilWorld world = AnvilWorld.Open();
        foreach (ChunkRef chunk in icm)
        {
          if(x >= 0 && y >= 0 && z >= 0 && x<MCregion.x && z<MCregion.z && z<10)
            {
                /// 
                AlphaBlock block = chunk.Blocks.GetBlock(x, y,z);
                if (block != null)
                {
                    chunk.Blocks.SetData(x, y,z, 0);
                    chunk.Blocks.SetID(x, y, z, 0);
                    chunk.Blocks.SetID(x, y,z, 41);
                }
            }
            chunk.IsTerrainPopulated = true;
        }
        wolrd.Save();
    }

    
	
}
