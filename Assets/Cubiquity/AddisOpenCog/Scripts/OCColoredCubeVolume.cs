using UnityEngine;
using Substrate;
using Substrate.Core;
using System.IO;
namespace OCCubiquity
{
   
public class OCColoredCubeVolume:Cubiquity.ColoredCubesVolume
{
    private static string _mapName = "TestScene1";
    #region public properties 
 
   public static string MapName
   {
       get { return _mapName; }
       set { if (value != null && value.Length != 0) _mapName = value; }
   }
    public static string Dir
    {
        get
        {   
            string path = Path.Combine(Application.streamingAssetsPath, MapName);
            return path;
        }
    }    

    public static AnvilWorld World
    {
        get
        {
            AnvilWorld world = null;
            if (Directory.Exists(Dir))
            {
                world = AnvilWorld.Open(Dir);
            }
            else
            {
                Debug.LogError("Map Name  " + MapName + " Not Found");
                return null;
            }
            return world;
        }
    }

    #endregion


    #region public Methods

    /// <summary>
    /// counts the number of Substrate region
    /// </summary>
    /// <returns>return number of Substrate region </returns>
    public static int GetRegions()
    {
        int numberOfRegions = 0;
        IRegionManager irm = World.GetRegionManager();
        foreach (IRegion ir in irm)
        {
            numberOfRegions++;
        }
        return numberOfRegions;
    }

    public  void MCSubstrate(Cubiquity.ColoredCubesVolumeData data)
    {
        if (Dir == null)
        {
            return;
        }
        IRegionManager irm = World.GetRegionManager();
        IChunkManager icm = World.GetChunkManager();
        SubstrateIdToCubiquityColor.SetColor();
        CreateCubiquityRegion(data, icm);
    }

    public  void CreateCubiquityRegion( Cubiquity.ColoredCubesVolumeData data, IChunkManager icm)
    {
        AnvilRegionManager arm = World.GetRegionManager();
        foreach (AnvilRegion ar in arm)
        {
            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < 32; i++)
                {
                    ChunkRef chunk = ar.GetChunkRef(j, i);
                    if (chunk != null)
                    {
                      DrawAchunk(data, (i), (j), chunk);
                    }
                }
            }
        }
    }  

    /// <summary>
    /// Draw Substrate chunk on Unity using Cubiquity
    /// </summary>
    /// <param tooltip="data">a reference to invoke SetVexel</param>
    /// <param tooltip="col">determines a chunk columns, for instance , col=1 first column witch start loopint at z=(16*(1-1)=0 to z=16*1=16  and 
    /// col=2 second column that starts loopint at z=(16*(2-1))=16 to z=(16*2)=32 and so on </param>
    /// <param tooltip="row">determines a chunk rows,for instance, row=1 first row that starting looping at x=(16(1-1))=0 to x=(16*1)=16
    /// </param>
    /// <param tooltip="chunk">a chunk reference for Substrate blocks</param>
    /// <remarks> the first row will be changed until all the columns are drawn fully in Unity </remarks>
    public  void DrawAchunk(Cubiquity.ColoredCubesVolumeData data, int col, int row, ChunkRef chunk)
    {
        int StartColumns = 16 * (col);
        int StartRows = 16 * (row);
        for (int x = StartRows; x < 16 * (row + 1); x++)
        {
            for (int y = 0; y < 160; y++)
            {
                for (int z = StartColumns; z < 16 * (col + 1); z++)
                {
                    int blockId = chunk.Blocks.GetID(x % 16, y, z % 16);
                    if (blockId != 0)
                    {
                        Color32 blockColor = MapColor(blockId,z,y,x);
                        if (blockColor.r != 0 && blockColor.g != 0 && blockColor.b != 0 && blockColor.a != 0)
                        {
                           
                            data.SetVoxel(z, y, x, (Cubiquity.QuantizedColor)blockColor);
                            
                        }
                    }
                }
            }
        }
    }  

    
    /// <summary>
    /// Substrate to Cubiquity MyColor mapping
    /// </summary>
    /// <param tooltip="id">Substrate MyColor id to be mapped</param>
    /// <returns></returns>

    public  Color32 MapColor(int id,int x, int y,int z)
    {
        return SubstrateIdToCubiquityColor.MapIdToColor(id,x,y,z) ;
    }

    #endregion  

	
}
}