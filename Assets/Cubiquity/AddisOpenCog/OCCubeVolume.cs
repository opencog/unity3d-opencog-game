using UnityEngine;
using System.Collections;
using Substrate;
using Substrate.Core;
using System.IO;
using System.Collections.Generic;


namespace OCCubiquity
{

    public class OCCubeVolume
    {
        #region public properties

        public static string MapName = "TestScene1";
        public static Cubiquity.ColoredCubesVolumeData Data
        {
            get;
            set;
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
                }
                return world;
            }
        }

        #endregion




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

        public static void MCSubstrate(Cubiquity.ColoredCubesVolumeData data)
        {
            if (Dir == null)
            {
                return;
            }

            IRegionManager irm = World.GetRegionManager();
            IChunkManager icm = World.GetChunkManager();
            Data = data;
            SubstrateIdToCubiquityColor.ColorType.Clear();
            SubstrateIdToCubiquityColor.SetColor();
            CreateCubiquityRegion(data, icm);


        }

        public static void CreateCubiquityRegion(Cubiquity.ColoredCubesVolumeData data, IChunkManager icm)
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


        #region draw Substrate chunk to Unity using Cubiquity

        /// <summary>
        /// Draw Substrate chunk on Unity using Cubiquity
        /// </summary>
        /// <param name="data">a reference to invoke SetVexel</param>
        /// <param name="col">determines a chunk columns, for instance , col=1 first column witch start loopint at z=(16*(1-1)=0 to z=16*1=16  and 
        /// col=2 second column that starts loopint at z=(16*(2-1))=16 to z=(16*2)=32 and so on </param>
        /// <param name="row">determines a chunk rows,for instance, row=1 first row that starting looping at x=(16(1-1))=0 to x=(16*1)=16
        /// </param>
        /// <param name="chunk">a chunk reference for Substrate blocks</param>
        /// <remarks> the first row will be changed until all the columns are drawn fully in Unity </remarks>
        public static void DrawAchunk(Cubiquity.ColoredCubesVolumeData data, int col, int row, ChunkRef chunk)
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
                        // Debug.Log(x + "   " + y + "   " + z + " color  " + color);
                        if (blockId != 0)
                        {

                            if (blockId > 150)
                                data.SetVoxel(z, y, x, (Cubiquity.QuantizedColor)Color.black);

                            Color32 blockColor = MapColor(blockId, z, y, x);

                            if (blockColor.r != 0 && blockColor.g != 0 && blockColor.b != 0 && blockColor.a != 0)
                            {

                                data.SetVoxel(z, y, x, (Cubiquity.QuantizedColor)blockColor);

                            }


                        }
                    }

                }

            }
        }

        #endregion

        #region MyColor mapping
        /// <summary>
        /// Substrate to Cubiquity MyColor mapping
        /// </summary>
        /// <param name="id">Substrate MyColor id to be mapped</param>
        /// <returns></returns>

        public static Color32 MapColor(int id, int x, int y, int z)
        {
            return SubstrateIdToCubiquityColor.MapIdToColor(id, x, y, z);
        }

        #endregion



    }
}